using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace ChestCounter;

/// <summary>
/// The player's Epic account, read only: profile (locker, V-Bucks, past seasons, purchases), stats, linked accounts,
/// friends count, and the "My Locker" link of fortnite.gg. Uses the same public Epic endpoints and Fortnite game
/// clients as community tools (fortnite.gg locker import, FNGG-LockerGenerator). Nothing is ever changed on the account.
///
/// Sign-in: the browser opens the Epic login, which ends on a page showing {"authorizationCode": "..."}; the player
/// copies that text (the code is picked up from the clipboard by itself) or pastes it in the window.
/// "Stay signed in" keeps an Epic device login (or a refresh token when Epic refuses one), encrypted with Windows DPAPI
/// for this Windows user only. Signing out deletes it here AND at Epic.
/// </summary>
[SupportedOSPlatform("windows")]
public static class EpicAccount
{
    // Public clients of the Fortnite games (the same ones the game and community tools use).
    private static readonly (string Id, string Secret) Pc = ("ec684b8c687f479fadea3cb2ad83f5c6", "e1f31c211f28413186262d37a13fc84d");
    private static readonly (string Id, string Secret) Ios = ("3446cd72694c4a4485d81b77adbb2141", "9209d4a5e25a457fb9b07489d313b41a");

    private const string AccountApi = "https://account-public-service-prod.ol.epicgames.com/account/api";
    private const string ProfileApi = "https://fortnite-public-service-prod11.ol.epicgames.com/fortnite/api/game/v2/profile";
    private const string StatsApi = "https://statsproxy-public-service-live.ol.epicgames.com/statsproxy/api/statsv2/account";
    private const string FriendsApi = "https://friends-public-service-prod.ol.epicgames.com/friends/api/v1";
    private const string FnggItems = "https://fortnite.gg/api/items.json";
    private const string FnggBundles = "https://fortnite.gg/api/bundles.json";
    private const string BuiltInUrl = "https://fortnite-api.com/v2/cosmetics/br";

    public static string CodePageUrl => $"https://www.epicgames.com/id/api/redirect?clientId={Pc.Id}&responseType=code";
    public static string LoginUrl => "https://www.epicgames.com/id/login?redirectUrl=" + Uri.EscapeDataString(CodePageUrl);

    private static readonly string[] Profiles = { "athena", "common_core", "common_public", "collections", "creative", "campaign", "metadata" };

    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(90) };
    private static readonly object Gate = new();

    private static string _folder = "";
    private static string LoginFile => Path.Combine(_folder, "epic_login.bin");
    private static JsonObject? _token;
    private static CancellationTokenSource? _waiting;
    private static string? _finalMessage; // set by a task that ends with its own message

    /// <summary>"out" (not signed in), "waiting" (browser open, waiting for the code), "busy", "in".</summary>
    public static string State { get; private set; } = "out";
    public static string Message { get; private set; } = "";
    public static string? DisplayName { get; private set; }
    public static bool Remember { get; set; } = true;

    /// <summary>Everything read from Epic (raw), null before the first load.</summary>
    public static JsonObject? Data { get; private set; }

    /// <summary>Changes each time <see cref="Data"/> changes (the window reloads the account page then).</summary>
    public static string Stamp { get; private set; } = "";

    /// <summary>The last fortnite.gg link made (also put in the clipboard).</summary>
    public static string? FnggLink { get; private set; }

    public static bool HasSavedLogin => File.Exists(LoginFile);

    /// <summary>Called once at startup: with a saved login, signs in and loads everything in the background.</summary>
    public static void Init(string dataFolder, bool remember)
    {
        _folder = dataFolder;
        Remember = remember;
        if (LoadSaved() is { } saved)
        {
            DisplayName = saved["name"]?.GetValue<string>();
            Run(L.T("Connexion avec l'accès enregistré…", "Signing in with the saved login…", "Iniciando sesión con el acceso guardado…"), () => FetchWithSaved(saved));
        }
    }

    // ---- Actions (from the window) ---------------------------------------------------------------------------------

    /// <summary>Opens the Epic login in the browser and watches the clipboard for the code (5 minutes).</summary>
    public static void BeginLogin(Action<string> open)
    {
        lock (Gate)
        {
            if (State == "busy")
            {
                return;
            }
            _waiting?.Cancel();
            _waiting = new CancellationTokenSource();
            State = "waiting";
            Message = L.T("Connecte-toi à Epic dans le navigateur. Quand le texte avec « authorizationCode » s'affiche : Ctrl+A, Ctrl+C (détecté tout seul), ou colle-le ici.",
                "Sign in to Epic in the browser. When the text with \"authorizationCode\" shows: Ctrl+A, Ctrl+C (picked up by itself), or paste it here.",
                "Inicia sesión en Epic en el navegador. Cuando aparezca el texto con «authorizationCode»: Ctrl+A, Ctrl+C (se detecta solo), o pégalo aquí.");
        }
        var token = _waiting.Token;
        var before = ReadClipboard();
        var watcher = new Thread(() =>
        {
            var end = DateTime.UtcNow.AddMinutes(5);
            while (!token.IsCancellationRequested && DateTime.UtcNow < end)
            {
                Thread.Sleep(500);
                var clip = ReadClipboard();
                if (clip != before && ExtractCode(clip) is { } code)
                {
                    ClearClipboard(); // the code must not stay in the clipboard
                    SubmitCode(code);
                    return;
                }
            }
            lock (Gate)
            {
                if (!token.IsCancellationRequested && State == "waiting")
                {
                    State = Data != null ? "in" : "out";
                    Message = L.T("Connexion expirée (5 minutes) : recommence.", "Sign-in expired (5 minutes): start again.", "Inicio de sesión caducado (5 minutos): vuelve a empezar.");
                }
            }
        }) { IsBackground = true, Name = "Epic code" };
        watcher.SetApartmentState(ApartmentState.STA); // the clipboard needs an STA thread
        watcher.Start();
        open(LoginUrl);
    }

    public static void CancelLogin()
    {
        lock (Gate)
        {
            _waiting?.Cancel();
            if (State == "waiting")
            {
                State = Data != null ? "in" : "out";
                Message = L.T("Connexion annulée.", "Sign-in cancelled.", "Inicio de sesión cancelado.");
            }
        }
    }

    /// <summary>Text pasted by the player (the whole page or just the code). Returns false when no code is in it.</summary>
    public static bool SubmitText(string text)
    {
        if (ExtractCode(text) is not { } code)
        {
            Message = L.T("Aucun authorizationCode dans ce texte.", "No authorizationCode in this text.", "No hay ningún authorizationCode en este texto.");
            return false;
        }
        SubmitCode(code);
        return true;
    }

    private static void SubmitCode(string code)
    {
        lock (Gate)
        {
            _waiting?.Cancel();
        }
        var remember = Remember;
        Run(L.T("Code reçu, connexion…", "Code received, signing in…", "Código recibido, iniciando sesión…"), async () =>
        {
            var tok = await TokenRequest(Pc, "grant_type=authorization_code&code=" + Uri.EscapeDataString(code));
            _token = tok;
            DeleteSavedFile();
            if (remember)
            {
                if (await SetupSavedLogin(tok) is { } saved)
                {
                    Save(saved);
                }
                else
                {
                    _finalMessage = L.T("Rester connecté n'a pas marché chez Epic : il faudra te reconnecter au prochain lancement.",
                        "Staying signed in did not work at Epic: you will have to sign in again next time.",
                        "Mantener la sesión no funcionó en Epic: tendrás que volver a iniciar sesión la próxima vez.");
                }
            }
            return await FetchAll(tok);
        });
    }

    public static void Reload()
    {
        Run(L.T("Chargement…", "Loading…", "Cargando…"), async () =>
        {
            if (_token != null && !Expired(_token))
            {
                return await FetchAll(_token);
            }
            if (LoadSaved() is { } saved)
            {
                return await FetchWithSaved(saved);
            }
            throw new InvalidOperationException(L.T("Session expirée : reconnecte-toi.", "Session expired: sign in again.", "Sesión caducada: vuelve a iniciar sesión."));
        });
    }

    /// <summary>Signs out: removes the device login at Epic, the saved file and the session.</summary>
    public static void Logout()
    {
        var saved = LoadSaved();
        var tok = _token;
        Run(L.T("Déconnexion…", "Signing out…", "Cerrando sesión…"), async () =>
        {
            var t = tok;
            if (saved != null)
            {
                try
                {
                    t ??= (await LoginFromSaved(saved)).Token;
                    if (Str(saved, "method") == "device")
                    {
                        await Req(HttpMethod.Delete, $"{AccountApi}/public/account/{Str(saved, "account_id")}/deviceAuth/{Str(saved, "device_id")}", Bearer(t));
                    }
                }
                catch
                {
                    // already revoked or offline: the local file goes anyway
                }
            }
            DeleteSavedFile();
            if (t != null)
            {
                await KillToken(t);
            }
            _token = null;
            Data = null;
            DisplayName = null;
            FnggLink = null;
            Stamp = DateTime.UtcNow.Ticks.ToString();
            return null;
        }, signedOut: true);
    }

    /// <summary>Called when the counter closes: without "stay signed in" the session ends at once.</summary>
    public static void Shutdown()
    {
        _waiting?.Cancel();
        if (_token is { } t && !HasSavedLogin)
        {
            try { KillToken(t).Wait(TimeSpan.FromSeconds(3)); } catch { }
        }
    }

    /// <summary>Builds the fortnite.gg "My Locker" link of the loaded account; opens it and copies it.</summary>
    public static void OpenFortniteGg(Action<string> open)
    {
        var data = Data;
        if (data == null)
        {
            return;
        }
        Run(L.T("Préparation du lien fortnite.gg…", "Preparing the fortnite.gg link…", "Preparando el enlace de fortnite.gg…"), async () =>
        {
            var (url, count) = await FortniteGgUrl(data);
            FnggLink = url;
            SetClipboard(url);
            open(url);
            _finalMessage = L.T($"{count} objets envoyés à fortnite.gg (le lien est aussi dans le presse-papiers).",
                $"{count} items sent to fortnite.gg (the link is in the clipboard too).",
                $"{count} objetos enviados a fortnite.gg (el enlace también está en el portapapeles).");
            return data;
        });
    }

    /// <summary>Saves everything read from Epic as JSON in the data folder (exports\). Returns the file, or null.</summary>
    public static string? Export()
    {
        if (Data is not { } data)
        {
            return null;
        }
        var dir = Path.Combine(_folder, "exports");
        Directory.CreateDirectory(dir);
        var file = Path.Combine(dir, $"epic_account_{DateTime.Now:yyyy-MM-dd_HH-mm}.json");
        var copy = JsonNode.Parse(data.ToJsonString())!.AsObject();
        copy.Remove("_names");
        File.WriteAllText(file, copy.ToJsonString(new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        return file;
    }

    // ---- Background work -------------------------------------------------------------------------------------------

    private static void Run(string message, Func<Task<JsonObject?>> work, bool signedOut = false)
    {
        lock (Gate)
        {
            if (State == "busy")
            {
                return;
            }
            State = "busy";
            Message = message;
            _finalMessage = null;
        }
        Task.Run(async () =>
        {
            try
            {
                var data = await work();
                lock (Gate)
                {
                    if (signedOut)
                    {
                        State = "out";
                        Message = L.T("Déconnecté.", "Signed out.", "Sesión cerrada.");
                        return;
                    }
                    if (data != null && !ReferenceEquals(data, Data))
                    {
                        Data = data;
                        Stamp = DateTime.UtcNow.Ticks.ToString();
                        DisplayName = Str(data["account"], "displayName") ?? Str(_token, "displayName") ?? DisplayName;
                    }
                    State = Data != null ? "in" : "out";
                    var errors = (Data?["_errors"] as JsonObject)?.Count ?? 0;
                    var n = Cosmetics(Data).Count;
                    Message = _finalMessage ?? (Data == null ? "" : L.T($"{n} objets cosmétiques chargés.", $"{n} cosmetic items loaded.", $"{n} objetos cosméticos cargados.")
                        + (errors > 0 ? L.T($" {errors} parties non disponibles.", $" {errors} parts not available.", $" {errors} partes no disponibles.") : ""));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Write("Epic account", ex);
                lock (Gate)
                {
                    State = Data != null ? "in" : "out";
                    Message = ex.Message;
                }
            }
        });
    }

    private static async Task<JsonObject?> FetchWithSaved(JsonObject saved)
    {
        (JsonObject Token, JsonObject Saved) r;
        try
        {
            r = await LoginFromSaved(saved);
        }
        catch (EpicException e) when (e.Status is 400 or 401 or 403 or 404)
        {
            DeleteSavedFile(); // revoked or expired at Epic
            throw new InvalidOperationException(L.T("L'accès enregistré n'est plus valable : reconnecte-toi.", "The saved login is no longer valid: sign in again.", "El acceso guardado ya no es válido: vuelve a iniciar sesión."));
        }
        Save(r.Saved); // a refresh token changes at each sign-in
        _token = r.Token;
        return await FetchAll(r.Token);
    }

    private static async Task<JsonObject> FetchAll(JsonObject tok)
    {
        var aid = Str(tok, "account_id")!;
        var auth = Bearer(tok);
        var output = new JsonObject();
        var errors = new JsonObject();

        async Task Grab(string key, Func<Task<JsonNode?>> fn)
        {
            Message = L.T($"Chargement : {key}…", $"Loading {key}…", $"Cargando {key}…");
            try { output[key] = await fn(); }
            catch (Exception e) { errors[key] = e.Message; }
        }

        await Grab("account", () => Req(HttpMethod.Get, $"{AccountApi}/public/account/{aid}", auth));
        await Grab("external_auths", () => Req(HttpMethod.Get, $"{AccountApi}/public/account/{aid}/externalAuths", auth));
        await Grab("friends_summary", () => Req(HttpMethod.Get, $"{FriendsApi}/{aid}/summary", auth));
        await Grab("stats", () => Req(HttpMethod.Get, $"{StatsApi}/{aid}", auth));
        foreach (var p in Profiles)
        {
            await Grab("profile_" + p, () => Req(HttpMethod.Post, $"{ProfileApi}/{aid}/client/QueryProfile?profileId={p}&rvn=-1", auth, "{}"));
        }
        if (output["profile_athena"] == null)
        {
            throw new InvalidOperationException(L.T("Le profil Fortnite n'a pas pu être chargé : ", "The Fortnite profile could not be loaded: ", "No se pudo cargar el perfil de Fortnite: ")
                + (Str(errors, "profile_athena") ?? ""));
        }
        output["_errors"] = errors;
        Message = L.T("Chargement des noms (fortnite-api.com)…", "Loading the item names (fortnite-api.com)…", "Cargando los nombres (fortnite-api.com)…");
        output["_names"] = await CosmeticNames();
        return output;
    }

    // ---- Sign-in -----------------------------------------------------------------------------------------------------

    private static async Task<JsonObject?> SetupSavedLogin(JsonObject tok)
    {
        foreach (var name in new[] { "ios", "pc" })
        {
            var client = name == "ios" ? Ios : Pc;
            JsonObject? t = null;
            try
            {
                t = name == "pc" ? tok : await ExchangeTo(tok, client);
                var d = (await Req(HttpMethod.Post, $"{AccountApi}/public/account/{Str(t, "account_id")}/deviceAuth", Bearer(t), "{}"))!.AsObject();
                var dev = new JsonObject { ["account_id"] = Str(d, "accountId"), ["device_id"] = Str(d, "deviceId"), ["secret"] = Str(d, "secret") };
                try
                {
                    var test = await DeviceLogin(dev, client); // does this way really work?
                    await KillToken(test);
                }
                catch
                {
                    try { await Req(HttpMethod.Delete, $"{AccountApi}/public/account/{Str(dev, "account_id")}/deviceAuth/{Str(dev, "device_id")}", Bearer(t)); } catch { }
                    throw;
                }
                dev["method"] = "device";
                dev["client"] = name;
                dev["name"] = Str(tok, "displayName") ?? "";
                return dev;
            }
            catch
            {
                // next way
            }
            finally
            {
                if (t != null && !ReferenceEquals(t, tok))
                {
                    await KillToken(t);
                }
            }
        }
        if (Str(tok, "refresh_token") is { } refresh)
        {
            return new JsonObject { ["method"] = "refresh", ["client"] = "pc", ["refresh_token"] = refresh, ["account_id"] = Str(tok, "account_id"), ["name"] = Str(tok, "displayName") ?? "" };
        }
        return null;
    }

    private static async Task<(JsonObject Token, JsonObject Saved)> LoginFromSaved(JsonObject saved)
    {
        if (Str(saved, "method") == "refresh")
        {
            var tok = await TokenRequest(Pc, "grant_type=refresh_token&refresh_token=" + Uri.EscapeDataString(Str(saved, "refresh_token") ?? ""));
            var updated = JsonNode.Parse(saved.ToJsonString())!.AsObject();
            updated["refresh_token"] = Str(tok, "refresh_token") ?? Str(saved, "refresh_token");
            return (tok, updated);
        }
        return (await DeviceLogin(saved, Str(saved, "client") == "pc" ? Pc : Ios), saved);
    }

    private static Task<JsonObject> DeviceLogin(JsonObject dev, (string Id, string Secret) client) =>
        TokenRequest(client, $"grant_type=device_auth&account_id={Uri.EscapeDataString(Str(dev, "account_id") ?? "")}&device_id={Uri.EscapeDataString(Str(dev, "device_id") ?? "")}&secret={Uri.EscapeDataString(Str(dev, "secret") ?? "")}");

    private static async Task<JsonObject> ExchangeTo(JsonObject tok, (string Id, string Secret) client)
    {
        var ex = await Req(HttpMethod.Get, $"{AccountApi}/oauth/exchange", Bearer(tok));
        return await TokenRequest(client, "grant_type=exchange_code&exchange_code=" + Str(ex, "code"));
    }

    private static async Task<JsonObject> TokenRequest((string Id, string Secret) client, string body) =>
        (await Req(HttpMethod.Post, $"{AccountApi}/oauth/token", "basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{client.Id}:{client.Secret}")), body, "application/x-www-form-urlencoded"))!.AsObject();

    private static async Task KillToken(JsonObject tok)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(8));
            await Req(HttpMethod.Delete, $"{AccountApi}/oauth/sessions/kill/{Str(tok, "access_token")}", Bearer(tok), ct: cts.Token);
        }
        catch
        {
            // it expires by itself anyway
        }
    }

    private static bool Expired(JsonObject tok) =>
        DateTime.TryParse(Str(tok, "expires_at"), null, System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal, out var exp)
        && exp < DateTime.UtcNow.AddMinutes(1);

    public static string? ExtractCode(string? text)
    {
        text ??= "";
        var m = Regex.Match(text, "\"authorizationCode\"\\s*:\\s*\"([0-9a-f]{32})\"");
        if (m.Success)
        {
            return m.Groups[1].Value;
        }
        m = Regex.Match(text, @"^\s*([0-9a-f]{32})\s*$");
        return m.Success ? m.Groups[1].Value : null;
    }

    // ---- HTTP ---------------------------------------------------------------------------------------------------------

    public sealed class EpicException(string message, string code, int status) : Exception(message)
    {
        public string Code { get; } = code;
        public int Status { get; } = status;
    }

    private static string Bearer(JsonObject? tok) => "bearer " + Str(tok, "access_token");

    private static async Task<JsonNode?> Req(HttpMethod method, string url, string? auth = null, string? body = null, string contentType = "application/json", CancellationToken ct = default)
    {
        using var req = new HttpRequestMessage(method, url);
        req.Headers.UserAgent.ParseAdd($"StormReport/{AppInfo.VersionText}");
        if (auth != null)
        {
            req.Headers.TryAddWithoutValidation("Authorization", auth);
        }
        if (body != null)
        {
            req.Content = new StringContent(body, Encoding.UTF8);
            req.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        }
        using var res = await Http.SendAsync(req, ct);
        var text = await res.Content.ReadAsStringAsync(ct);
        if (!res.IsSuccessStatusCode)
        {
            string msg = text, code = "";
            try
            {
                var j = JsonNode.Parse(text);
                msg = Str(j, "errorMessage") ?? text;
                code = Str(j, "errorCode") ?? "";
            }
            catch
            {
                // not JSON
            }
            if (msg.Length > 300) msg = msg[..300];
            throw new EpicException($"HTTP {(int)res.StatusCode} ({new Uri(url).Host}): {msg}", code, (int)res.StatusCode);
        }
        return string.IsNullOrWhiteSpace(text) ? null : JsonNode.Parse(text);
    }

    private static string? Str(JsonNode? node, string key)
    {
        try { return node?[key] is JsonValue v ? v.ToString() : null; }
        catch { return null; }
    }

    // ---- Saved login (DPAPI, this Windows user only) -----------------------------------------------------------------

    private static void Save(JsonObject saved)
    {
        Directory.CreateDirectory(_folder);
        File.WriteAllBytes(LoginFile, Dpapi(Encoding.UTF8.GetBytes(saved.ToJsonString()), protect: true));
    }

    private static JsonObject? LoadSaved()
    {
        try { return File.Exists(LoginFile) ? JsonNode.Parse(Encoding.UTF8.GetString(Dpapi(File.ReadAllBytes(LoginFile), protect: false)))?.AsObject() : null; }
        catch { return null; }
    }

    private static void DeleteSavedFile()
    {
        try { File.Delete(LoginFile); } catch { }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct DataBlob { public int Size; public IntPtr Data; }

    [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CryptProtectData(ref DataBlob input, string? description, IntPtr entropy, IntPtr reserved, IntPtr prompt, int flags, out DataBlob output);

    [DllImport("crypt32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool CryptUnprotectData(ref DataBlob input, IntPtr description, IntPtr entropy, IntPtr reserved, IntPtr prompt, int flags, out DataBlob output);

    [DllImport("kernel32.dll")]
    private static extern IntPtr LocalFree(IntPtr mem);

    private static byte[] Dpapi(byte[] data, bool protect)
    {
        var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
        try
        {
            var input = new DataBlob { Size = data.Length, Data = handle.AddrOfPinnedObject() };
            var ok = protect
                ? CryptProtectData(ref input, "Storm Report", IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 1 /* UI_FORBIDDEN */, out var output)
                : CryptUnprotectData(ref input, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 1, out output);
            if (!ok)
            {
                throw new InvalidOperationException("DPAPI failed");
            }
            try
            {
                var result = new byte[output.Size];
                Marshal.Copy(output.Data, result, 0, output.Size);
                return result;
            }
            finally
            {
                LocalFree(output.Data);
            }
        }
        finally
        {
            handle.Free();
        }
    }

    // ---- Clipboard (STA thread) --------------------------------------------------------------------------------------

    private static string ReadClipboard()
    {
        try { return System.Windows.Forms.Clipboard.ContainsText() ? System.Windows.Forms.Clipboard.GetText() : ""; }
        catch { return ""; }
    }

    private static void ClearClipboard()
    {
        try { System.Windows.Forms.Clipboard.Clear(); } catch { }
    }

    /// <summary>Puts text in the clipboard from any thread.</summary>
    public static void SetClipboard(string text)
    {
        var t = new Thread(() => { try { System.Windows.Forms.Clipboard.SetText(text); } catch { } });
        t.SetApartmentState(ApartmentState.STA);
        t.Start();
        t.Join(TimeSpan.FromSeconds(3));
    }

    // ---- Cosmetic names (fortnite-api.com, cached a day) -------------------------------------------------------------

    private static async Task<JsonObject> CosmeticNames()
    {
        var lang = L.Code is "fr" or "es" ? L.Code : "en";
        var path = Path.Combine(_folder, $"cosmetics_{lang}.json");
        JsonObject? Cached() { try { return JsonNode.Parse(File.ReadAllText(path))?.AsObject(); } catch { return null; } }
        if (File.Exists(path) && DateTime.UtcNow - File.GetLastWriteTimeUtc(path) < TimeSpan.FromDays(1) && Cached() is { } fresh)
        {
            return fresh;
        }
        JsonNode? j;
        try
        {
            j = await Req(HttpMethod.Get, $"https://fortnite-api.com/v2/cosmetics?language={lang}");
        }
        catch
        {
            return Cached() ?? new JsonObject();
        }
        var names = new JsonObject();
        if (j?["data"] is JsonObject groups)
        {
            foreach (var (_, list) in groups)
            {
                if (list is not JsonArray items)
                {
                    continue;
                }
                foreach (var c in items)
                {
                    var id = Str(c, "id");
                    if (string.IsNullOrEmpty(id))
                    {
                        continue;
                    }
                    names[id.ToLowerInvariant()] = new JsonObject
                    {
                        ["name"] = Str(c, "name") ?? Str(c, "title") ?? id,
                        ["rarity"] = Str(c?["rarity"], "displayValue") ?? "",
                        ["set"] = Str(c?["set"], "value") ?? "",
                        ["intro"] = Str(c?["introduction"], "text") ?? "",
                        ["img"] = Str(c?["images"], "smallIcon") ?? Str(c?["images"], "icon") ?? "",
                    };
                }
            }
        }
        try { File.WriteAllText(path, names.ToJsonString()); } catch { }
        return names;
    }

    // ---- Reading the data ------------------------------------------------------------------------------------------

    private static JsonObject? Profile(JsonObject? data, string name)
    {
        try { return data?["profile_" + name]?["profileChanges"]?[0]?["profile"]?.AsObject(); }
        catch { return null; }
    }

    private static readonly Dictionary<string, (string Fr, string En, string Es)> Categories = new()
    {
        ["AthenaCharacter"] = ("Tenues", "Outfits", "Atuendos"),
        ["AthenaBackpack"] = ("Accessoires de dos", "Back Blings", "Accesorios mochileros"),
        ["AthenaPetCarrier"] = ("Accessoires de dos", "Back Blings", "Accesorios mochileros"),
        ["AthenaPet"] = ("Compagnons", "Pets", "Mascotas"),
        ["AthenaPickaxe"] = ("Outils de collecte", "Pickaxes", "Herramientas de recolección"),
        ["AthenaGlider"] = ("Planeurs", "Gliders", "Planeadores"),
        ["AthenaSkyDiveContrail"] = ("Traînées", "Contrails", "Estelas"),
        ["AthenaItemWrap"] = ("Revêtements", "Wraps", "Envoltorios"),
        ["AthenaMusicPack"] = ("Musiques", "Music", "Música"),
        ["AthenaLoadingScreen"] = ("Écrans de chargement", "Loading Screens", "Pantallas de carga"),
        ["CosmeticShoes"] = ("Chaussures", "Shoes", "Zapatillas"),
        ["AthenaDance"] = ("Emotes", "Emotes", "Gestos"),
        ["SparksGuitar"] = ("Instruments", "Instruments", "Instrumentos"),
        ["SparksBass"] = ("Instruments", "Instruments", "Instrumentos"),
        ["SparksDrums"] = ("Instruments", "Instruments", "Instrumentos"),
        ["SparksMicrophone"] = ("Instruments", "Instruments", "Instrumentos"),
        ["SparksKeyboard"] = ("Instruments", "Instruments", "Instrumentos"),
        ["SparksSong"] = ("Pistes musicales", "Jam Tracks", "Pistas musicales"),
        ["SparksAura"] = ("Auras", "Auras", "Auras"),
        ["VehicleCosmetics_Body"] = ("Voitures", "Cars", "Coches"),
        ["VehicleCosmetics_Skin"] = ("Peintures de voiture", "Car Skins", "Pinturas de coche"),
        ["VehicleCosmetics_Wheel"] = ("Roues", "Wheels", "Ruedas"),
        ["VehicleCosmetics_DriftTrail"] = ("Traces de dérapage", "Drift Trails", "Estelas de derrape"),
        ["VehicleCosmetics_Booster"] = ("Boosters", "Boosters", "Propulsores"),
        ["JunoBuildingSet"] = ("Kits LEGO", "LEGO Kits", "Kits LEGO"),
        ["JunoBuildingProp"] = ("Pièces LEGO", "LEGO Props", "Piezas LEGO"),
        ["HomebaseBannerIcon"] = ("Bannières", "Banners", "Estandartes"),
    };

    private static readonly HashSet<string> NotCosmetic = new()
    {
        "Quest", "ChallengeBundle", "ChallengeBundleSchedule", "Accolades", "CosmeticLocker", "Token", "HomebaseBannerColor",
        "CosmeticVariantToken", "ConditionalAction", "RepeatableDailiesCard", "ChallengeBundleCompletionToken", "AthenaSeason", "AccountResource",
    };

    public sealed record Cosmetic(string Category, string Name, string Rarity, string Set, string Intro, bool Favorite, int Styles, string Id, string Image);

    private static (string Key, List<Cosmetic> Rows)? _cosmeticsCache;

    /// <summary>The cosmetics of <see cref="Data"/>, built once per load and language (the window asks every 2 s).</summary>
    public static List<Cosmetic> LoadedCosmetics()
    {
        var key = Stamp + "|" + L.Code;
        if (_cosmeticsCache is { } c && c.Key == key)
        {
            return c.Rows;
        }
        var rows = Cosmetics(Data);
        _cosmeticsCache = (key, rows);
        return rows;
    }

    public static List<Cosmetic> Cosmetics(JsonObject? data)
    {
        var rows = new List<Cosmetic>();
        var names = data?["_names"] as JsonObject;
        foreach (var pname in new[] { "athena", "common_core" })
        {
            if (Profile(data, pname)?["items"] is not JsonObject items)
            {
                continue;
            }
            foreach (var (_, it) in items)
            {
                var tid = Str(it, "templateId") ?? "";
                var colon = tid.IndexOf(':');
                if (colon < 0)
                {
                    continue;
                }
                var type = tid[..colon];
                var raw = tid[(colon + 1)..];
                if (NotCosmetic.Contains(type) || type.StartsWith("Currency", StringComparison.Ordinal))
                {
                    continue;
                }
                string category;
                if (Categories.TryGetValue(type, out var c))
                {
                    category = L.T(c.Fr, c.En, c.Es);
                }
                else if (pname == "athena")
                {
                    category = L.T($"Autres ({type})", $"Other ({type})", $"Otros ({type})");
                }
                else
                {
                    continue;
                }
                var iid = raw.ToLowerInvariant();
                if (type == "AthenaDance")
                {
                    category = iid.StartsWith("spid_") ? L.T("Tags", "Sprays", "Grafitis")
                        : iid.StartsWith("emoji_") ? L.T("Émoticônes", "Emoticons", "Emoticonos")
                        : iid.StartsWith("toy_") ? L.T("Jouets", "Toys", "Juguetes")
                        : L.T("Emotes", "Emotes", "Gestos");
                }
                var a = it?["attributes"];
                var n = names?[iid];
                rows.Add(new Cosmetic(category, Str(n, "name") is { Length: > 0 } nm ? nm : raw, Str(n, "rarity") ?? "", Str(n, "set") ?? "", Str(n, "intro") ?? "",
                    a?["favorite"] is JsonValue f && f.ToString() == "true", (a?["variants"] as JsonArray)?.Count ?? 0, raw, Str(n, "img") ?? ""));
            }
        }
        return rows.OrderBy(r => r.Category).ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    public sealed record StatRow(string Mode, long Matches, long Wins, string WinRate, long Kills, string KD, long Minutes);

    public static List<StatRow> Stats(JsonObject? data)
    {
        var agg = new Dictionary<string, Dictionary<string, long>>();
        var rx = new Regex("^br_([a-z0-9]+)_([a-z]+)_m0_playlist_(.+)$");
        if (data?["stats"]?["stats"] is JsonObject stats)
        {
            var total = L.T("TOTAL", "TOTAL", "TOTAL");
            foreach (var (k, v) in stats)
            {
                var m = rx.Match(k);
                if (!m.Success || v is not JsonValue jv || !long.TryParse(jv.ToString(), out var value))
                {
                    continue;
                }
                var (stat, input, pl) = (m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);
                var size = pl.Contains("solo") ? "Solo" : pl.Contains("duo") ? "Duo" : pl.Contains("trio") ? "Trio" : pl.Contains("squad") ? "Squad" : L.T("Autres", "Other", "Otros");
                var mode = (pl.Contains("nobuild") ? "Zero Build " : "") + size;
                foreach (var key in new[] { mode, total, L.T("Entrée : ", "Input: ", "Entrada: ") + input })
                {
                    var d = agg.TryGetValue(key, out var x) ? x : agg[key] = new();
                    d[stat] = d.GetValueOrDefault(stat) + value;
                }
            }
        }
        var rows = agg.Select(kv =>
        {
            var d = kv.Value;
            long m = d.GetValueOrDefault("matchesplayed"), w = d.GetValueOrDefault("placetop1"), k = d.GetValueOrDefault("kills");
            return new StatRow(kv.Key, m, w, m > 0 ? (100.0 * w / m).ToString("0.0", L.Culture) + " %" : "—", k,
                m - w > 0 ? ((double)k / (m - w)).ToString("0.00", L.Culture) : "—", d.GetValueOrDefault("minutesplayed"));
        });
        return rows.OrderBy(r => r.Mode != "TOTAL").ThenBy(r => r.Mode.Contains(':')).ThenBy(r => r.Mode).ToList();
    }

    public static List<(string Section, string Field, string Value)> Overview(JsonObject? data)
    {
        var rows = new List<(string, string, string)>();
        string Val(JsonNode? n) => n switch { null => "", JsonValue v => v.ToString(), _ => n.ToJsonString() };
        var acc = data?["account"] as JsonObject;
        var account = L.T("Compte", "Account", "Cuenta");
        foreach (var k in new[] { "displayName", "id", "email", "country", "preferredLanguage", "emailVerified", "tfaEnabled", "lastLogin", "numberOfDisplayNameChanges", "lastDisplayNameChange" })
        {
            if (acc?[k] is { } v)
            {
                rows.Add((account, k, Val(v)));
            }
        }
        if (data?["external_auths"] is JsonArray ext)
        {
            foreach (var ea in ext)
            {
                rows.Add((L.T("Comptes liés", "Linked accounts", "Cuentas vinculadas"), Str(ea, "type") ?? "", Str(ea, "externalDisplayName") ?? Str(ea, "externalAuthId") ?? ""));
            }
        }
        if (data?["friends_summary"] is JsonObject fs)
        {
            foreach (var k in new[] { "friends", "incoming", "outgoing", "blocklist" })
            {
                if (fs[k] is JsonArray list)
                {
                    rows.Add((L.T("Amis", "Friends", "Amigos"), k, list.Count.ToString(L.Culture)));
                }
            }
        }
        if (Profile(data, "athena") is { } ath)
        {
            var s = ath["stats"]?["attributes"];
            foreach (var k in new[] { "season_num", "level", "xp", "accountLevel", "book_level", "book_purchased", "battlestars", "lifetime_wins", "last_match_end_datetime" })
            {
                if (s?[k] is { } v)
                {
                    rows.Add(("Battle Royale", k, Val(v)));
                }
            }
            rows.Add(("Battle Royale", L.T("profil créé", "profile created", "perfil creado"), Str(ath, "created") ?? ""));
        }
        if (Profile(data, "common_core") is { } cc)
        {
            long total = 0;
            var by = new SortedDictionary<string, long>();
            if (cc["items"] is JsonObject items)
            {
                foreach (var (_, it) in items)
                {
                    var tid = Str(it, "templateId") ?? "";
                    if (!tid.StartsWith("Currency:Mtx", StringComparison.Ordinal) || !long.TryParse(Str(it, "quantity"), out var q))
                    {
                        continue;
                    }
                    total += q;
                    var key = $"{tid.Split(':')[1]} ({Str(it?["attributes"], "platform") ?? "?"})";
                    by[key] = by.GetValueOrDefault(key) + q;
                }
            }
            rows.Add(("V-Bucks", L.T("total", "total", "total"), total.ToString("N0", L.Culture)));
            rows.AddRange(by.Select(kv => ("V-Bucks", kv.Key, kv.Value.ToString("N0", L.Culture))));
            var s = cc["stats"]?["attributes"];
            var shop = L.T("Boutique", "Shop", "Tienda");
            foreach (var k in new[] { "current_mtx_platform", "mtx_affiliate" })
            {
                if (s?[k] is { } v)
                {
                    rows.Add((shop, k, Val(v)));
                }
            }
            if (s?["gift_history"] is JsonObject gh)
            {
                rows.Add((shop, L.T("cadeaux", "gifts", "regalos"), L.T($"envoyés {Str(gh, "num_sent") ?? "0"}, reçus {Str(gh, "num_received") ?? "0"}", $"sent {Str(gh, "num_sent") ?? "0"}, received {Str(gh, "num_received") ?? "0"}", $"enviados {Str(gh, "num_sent") ?? "0"}, recibidos {Str(gh, "num_received") ?? "0"}")));
            }
            rows.Add((shop, L.T("tickets de remboursement", "refund tickets", "tickets de reembolso"), Str(s?["mtx_purchase_history"], "refundCredits") ?? ""));
        }
        if (data?["_errors"] is JsonObject errors)
        {
            foreach (var (k, v) in errors)
            {
                rows.Add((L.T("Non disponible", "Not available", "No disponible"), k, (Val(v).Split('\n')[0])));
            }
        }
        return rows;
    }

    public sealed record Purchase(string Date, long VBucks, bool Refunded, string Items);

    public static List<Purchase> Purchases(JsonObject? data)
    {
        var rows = new List<Purchase>();
        if (Profile(data, "common_core")?["stats"]?["attributes"]?["mtx_purchase_history"]?["purchases"] is JsonArray list)
        {
            foreach (var p in list)
            {
                var loot = string.Join(", ", (p?["lootResult"] as JsonArray ?? new JsonArray()).Select(l => (Str(l, "itemType") ?? "").Split(':').Last()));
                var date = (Str(p, "purchaseDate") ?? "").Replace('T', ' ');
                rows.Add(new Purchase(date.Length > 19 ? date[..19] : date, long.TryParse(Str(p, "totalMtxPaid"), out var v) ? v : 0,
                    !string.IsNullOrEmpty(Str(p, "refundDate")), loot.Length > 0 ? loot : Str(p, "offerId") ?? ""));
            }
        }
        return rows.OrderByDescending(r => r.Date).ToList();
    }

    public sealed record Season(int Number, string Level, string PassLevel, bool Pass, string Wins, string Xp);

    public static List<Season> Seasons(JsonObject? data)
    {
        var rows = new List<Season>();
        if (Profile(data, "athena")?["stats"]?["attributes"]?["past_seasons"] is JsonArray list)
        {
            foreach (var p in list)
            {
                rows.Add(new Season(int.TryParse(Str(p, "seasonNumber"), out var n) ? n : 0, Str(p, "seasonLevel") ?? "", Str(p, "bookLevel") ?? "",
                    Str(p, "purchasedVIP") == "true", Str(p, "numWins") ?? "", Str(p, "seasonXp") ?? ""));
            }
        }
        return rows.OrderByDescending(r => r.Number).ToList();
    }

    // ---- fortnite.gg ---------------------------------------------------------------------------------------------------

    /// <summary>
    /// The fortnite.gg "My Locker" link: the fortnite.gg ids of every owned item (plus the built-in emotes and the
    /// bundles that are complete), sorted, delta-encoded, prefixed with the profile creation date, raw-deflated and
    /// base64url-encoded (same method as Liqutch/FNGG-LockerGenerator, MIT).
    /// </summary>
    private static async Task<(string Url, int Count)> FortniteGgUrl(JsonObject data)
    {
        var items = (await Req(HttpMethod.Get, FnggItems))?.AsObject() ?? throw new InvalidOperationException("fortnite.gg items.json");
        var lower = new Dictionary<string, string>();
        foreach (var (k, _) in items)
        {
            lower.TryAdd(k.ToLowerInvariant(), k);
        }
        var locker = new List<string>();
        var seen = new HashSet<string>();
        void Add(string id)
        {
            if (lower.TryGetValue(id.ToLowerInvariant(), out var key) && seen.Add(key))
            {
                locker.Add(key);
            }
        }
        foreach (var (pname, only) in new[] { ("athena", (string?)null), ("common_core", "HomebaseBannerIcon:") })
        {
            if (Profile(data, pname)?["items"] is not JsonObject owned)
            {
                continue;
            }
            foreach (var (_, it) in owned)
            {
                var tid = Str(it, "templateId") ?? "";
                if (tid.Contains(':') && (only == null || tid.StartsWith(only, StringComparison.Ordinal)))
                {
                    Add(tid[(tid.IndexOf(':') + 1)..]);
                }
            }
        }
        try
        {
            Message = L.T("Emotes intégrées…", "Built-in emotes…", "Gestos integrados…");
            var builtin = new Dictionary<string, List<string>>();
            if ((await Req(HttpMethod.Get, BuiltInUrl))?["data"] is JsonArray all)
            {
                foreach (var c in all)
                {
                    if (c?["builtInEmoteIds"] is JsonArray ids && ids.Count > 0 && Str(c, "id") is { } id)
                    {
                        builtin[id.ToLowerInvariant()] = ids.Select(x => x?.ToString() ?? "").ToList();
                    }
                }
            }
            for (var i = 0; i < locker.Count; i++)
            {
                if (builtin.TryGetValue(locker[i].ToLowerInvariant(), out var emotes))
                {
                    emotes.ForEach(Add);
                }
            }
        }
        catch
        {
            // optional
        }
        try
        {
            Message = L.T("Packs fortnite.gg…", "fortnite.gg bundles…", "Lotes de fortnite.gg…");
            if (await Req(HttpMethod.Get, FnggBundles) is JsonObject bundles)
            {
                foreach (var (name, info) in bundles)
                {
                    if (info?["items"] is JsonArray parts && parts.Count > 0 && parts.All(p => seen.Contains(p?.ToString() ?? "")))
                    {
                        Add(name);
                    }
                }
            }
        }
        catch
        {
            // optional
        }
        if (locker.Count == 0)
        {
            throw new InvalidOperationException(L.T("Aucun objet ne correspond à la liste de fortnite.gg.", "No item matches the fortnite.gg list.", "Ningún objeto coincide con la lista de fortnite.gg."));
        }
        var ints = locker.Select(k => long.Parse(items[k]!.ToString(), System.Globalization.CultureInfo.InvariantCulture)).Distinct().OrderBy(x => x).ToList();
        var deltas = ints.Select((v, i) => (i == 0 ? v : v - ints[i - 1]).ToString(System.Globalization.CultureInfo.InvariantCulture));
        var payload = Encoding.UTF8.GetBytes($"{Str(Profile(data, "athena"), "created") ?? ""},{string.Join(",", deltas)}");
        var b64 = Convert.ToBase64String(RawDeflate(payload)).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        return ("https://fortnite.gg/my-locker?items=" + b64, locker.Count);
    }

    /// <summary>
    /// Raw deflate with a 512-byte window (zlib wbits -9), like the Python generator: a decoder set to that window
    /// accepts it. Falls back to the default window when this .NET does not offer it.
    /// </summary>
    private static byte[] RawDeflate(byte[] data)
    {
        using var ms = new MemoryStream();
        Stream? z = null;
        try
        {
            var ctor = typeof(DeflateStream).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
                new[] { typeof(Stream), typeof(CompressionLevel), typeof(bool), typeof(int) }, null);
            z = ctor?.Invoke(new object[] { ms, CompressionLevel.Optimal, true, -9 }) as Stream;
        }
        catch
        {
            z = null;
        }
        z ??= new DeflateStream(ms, CompressionLevel.Optimal, leaveOpen: true);
        using (z)
        {
            z.Write(data);
        }
        return ms.ToArray();
    }
}
