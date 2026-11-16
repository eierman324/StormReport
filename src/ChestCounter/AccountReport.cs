using System.Net;
using System.Runtime.Versioning;
using System.Text;
using System.Text.Json;

namespace ChestCounter;

/// <summary>
/// The "Locker" page of the window: what <see cref="EpicAccount"/> read, as tables in the style of the report
/// (overview, locker with search / category / favourites filters, stats per mode, past seasons, purchases).
/// Built on request, never written to disk (it holds account details).
/// </summary>
[SupportedOSPlatform("windows")]
public static class AccountReport
{
    private static string H(string? s) => WebUtility.HtmlEncode(s ?? "");

    public static string Html()
    {
        var data = EpicAccount.Data;
        var sb = new StringBuilder();
        var title = L.T("Compte Epic", "Epic account", "Cuenta de Epic");
        sb.Append($"""
            <!doctype html><html lang="{L.Code}"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
            <title>{H(title)}</title><style>{ReportWriter.Style}{Extra}</style></head><body><main>
            """);
        if (data == null)
        {
            sb.Append($"""<section><h2>{H(title)}</h2><div class="card"><p class="muted">{H(L.T("Connecte-toi dans l'onglet Compte pour voir ton casier, tes stats et tes achats ici.", "Sign in on the Account tab to see your locker, stats and purchases here.", "Inicia sesión en la pestaña Cuenta para ver aquí tu taquilla, tus estadísticas y tus compras."))}</p></div></section></main></body></html>""");
            return sb.ToString();
        }

        var cos = EpicAccount.Cosmetics(data);
        var stats = EpicAccount.Stats(data);
        var seasons = EpicAccount.Seasons(data);
        var buys = EpicAccount.Purchases(data);
        var over = EpicAccount.Overview(data);

        string V(string section, string field) => over.FirstOrDefault(r => r.Section == section && r.Field == field).Value ?? "";
        var total = stats.FirstOrDefault(r => r.Mode == "TOTAL");
        sb.Append($"""
            <header><div class="brand"><h1>{H(EpicAccount.DisplayName ?? title)}</h1></div><span class="muted">{H(L.T("lecture seule · rien n'est modifié sur ton compte", "read only · nothing is changed on your account", "solo lectura · no se cambia nada en tu cuenta"))}</span></header>
            <section class="tiles">
              {Tile(cos.Count.ToString("N0", L.Culture), L.T("objets dans le casier", "items in the locker", "objetos en la taquilla"))}
              {Tile(cos.Count(c => c.Category == L.T("Tenues", "Outfits", "Atuendos")).ToString("N0", L.Culture), L.T("tenues", "outfits", "atuendos"))}
              {Tile(V("V-Bucks", L.T("total", "total", "total")), "V-Bucks")}
              {Tile(V("Battle Royale", "accountLevel"), L.T("niveau du compte", "account level", "nivel de cuenta"))}
              {Tile(total != null ? total.Wins.ToString("N0", L.Culture) : "—", L.T("victoires", "wins", "victorias"))}
              {Tile(total != null ? total.Matches.ToString("N0", L.Culture) : "—", L.T("parties", "matches", "partidas"))}
            </section>
            """);

        // Locker: the rows go to the page as JSON and are filtered there (search, category, favourites).
        var cats = cos.Select(c => c.Category).Distinct().OrderBy(c => c).ToList();
        var rows = JsonSerializer.Serialize(cos.Select(c => new object[] { c.Category, c.Name, c.Rarity, c.Set, c.Intro, c.Favorite ? 1 : 0, c.Styles, c.Id, c.Image }));
        sb.Append($"""
            <section><h2>{H(L.T("Casier", "Locker", "Taquilla"))}</h2><div class="card">
            <div class="filters"><input id="q" type="search" placeholder="{H(L.T("Rechercher (nom, set, id…)", "Search (name, set, id…)", "Buscar (nombre, set, id…)"))}">
            <select id="cat"><option value="">{H(L.T("Toutes les catégories", "All categories", "Todas las categorías"))}</option>{string.Concat(cats.Select(c => $"<option>{H(c)}</option>"))}</select>
            <label><input id="fav" type="checkbox"> {H(L.T("favoris seulement", "favourites only", "solo favoritos"))}</label><span id="count" class="muted"></span></div>
            <table><thead><tr><th></th><th>{H(L.T("Catégorie", "Category", "Categoría"))}</th><th>{H(L.T("Nom", "Name", "Nombre"))}</th><th>{H(L.T("Rareté", "Rarity", "Rareza"))}</th><th>Set</th><th>{H(L.T("Introduit", "Introduced", "Introducido"))}</th><th>★</th><th>{H(L.T("Styles", "Styles", "Estilos"))}</th><th>ID</th></tr></thead><tbody id="loc"></tbody></table>
            <div class="more"><button type="button" id="moreBtn" class="btn">{H(L.T("Afficher plus", "Show more", "Mostrar más"))}</button></div>
            </div></section>
            """);

        sb.Append(Table(L.T("Statistiques", "Stats", "Estadísticas"),
            new[] { L.T("Mode", "Mode", "Modo"), L.T("Parties", "Matches", "Partidas"), L.T("Victoires", "Wins", "Victorias"), L.T("% victoires", "Win rate", "% victorias"), "Kills", "K/D", L.T("Minutes", "Minutes", "Minutos") },
            stats.Select(r => new[] { r.Mode, r.Matches.ToString("N0", L.Culture), r.Wins.ToString("N0", L.Culture), r.WinRate, r.Kills.ToString("N0", L.Culture), r.KD, r.Minutes.ToString("N0", L.Culture) }), numericFrom: 1));
        sb.Append("<div class=\"two\">");
        sb.Append(Table(L.T("Saisons passées", "Past seasons", "Temporadas pasadas"),
            new[] { L.T("Saison", "Season", "Temporada"), L.T("Niveau", "Level", "Nivel"), L.T("Palier du passe", "Pass tier", "Nivel del pase"), L.T("Passe acheté", "Pass bought", "Pase comprado"), L.T("Victoires", "Wins", "Victorias"), "XP" },
            seasons.Select(r => new[] { r.Number.ToString(L.Culture), r.Level, r.PassLevel, r.Pass ? "✓" : "", r.Wins, r.Xp }), numericFrom: 1));
        sb.Append(Table(L.T("Vue d'ensemble", "Overview", "Resumen"),
            new[] { L.T("Partie", "Section", "Sección"), L.T("Champ", "Field", "Campo"), L.T("Valeur", "Value", "Valor") },
            over.Select(r => new[] { r.Section, r.Field, r.Value })));
        sb.Append("</div>");
        sb.Append(Table(L.T("Achats", "Purchases", "Compras"),
            new[] { L.T("Date", "Date", "Fecha"), "V-Bucks", L.T("Statut", "Status", "Estado"), L.T("Objets", "Items", "Objetos") },
            buys.Select(r => new[] { r.Date, r.VBucks.ToString("N0", L.Culture), r.Refunded ? L.T("remboursé", "refunded", "reembolsado") : "", r.Items })));

        sb.Append($$"""
            </main><script>
            const ROWS={{rows}};const PAGE=300;let shown=PAGE;
            const $=s=>document.querySelector(s);
            const esc=s=>String(s??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
            function draw(){
              const q=$('#q').value.toLowerCase(),cat=$('#cat').value,fav=$('#fav').checked;
              const list=ROWS.filter(r=>(!cat||r[0]===cat)&&(!fav||r[5])&&(!q||(r[1]+' '+r[7]+' '+r[3]+' '+r[2]).toLowerCase().includes(q)));
              $('#loc').innerHTML=list.slice(0,shown).map(r=>`<tr><td>${r[8]?`<img class="ico" loading="lazy" src="${esc(r[8])}" alt="">`:''}</td><td class="nw">${esc(r[0])}</td><td>${esc(r[1])}</td><td class="nw">${esc(r[2])}</td><td>${esc(r[3])}</td><td class="muted">${esc(r[4])}</td><td>${r[5]?'★':''}</td><td class="n">${r[6]||''}</td><td class="muted id">${esc(r[7])}</td></tr>`).join('');
              $('#count').textContent=list.length+' / '+ROWS.length;
              $('#moreBtn').style.display=list.length>shown?'':'none';
            }
            ['#q','#cat','#fav'].forEach(s=>$(s).addEventListener('input',()=>{shown=PAGE;draw()}));
            $('#moreBtn').onclick=()=>{shown+=PAGE;draw()};
            draw();
            </script></body></html>
            """);
        return sb.ToString();
    }

    private const string Extra = """
        .filters{display:flex;flex-wrap:wrap;gap:8px;align-items:center;padding:8px 0}
        .filters input[type=search],.filters select{min-height:24px;padding:2px 8px;border-radius:4px;border:1px solid var(--line);background:var(--main);color:var(--ink);font:12px var(--mono);outline:none}
        .filters input[type=search]{width:240px}.filters input:focus,.filters select:focus{border-color:var(--accent)}
        .filters label{display:flex;gap:6px;align-items:center;color:var(--muted)}
        .filters input[type=checkbox]{accent-color:var(--accent)}
        #count{margin-left:auto}
        .ico{width:28px;height:28px;border-radius:3px;object-fit:cover;background:var(--main);display:block}
        td.id{font-size:11px;word-break:break-all}
        .more{padding:8px 0;text-align:center}
        .btn{min-height:24px;padding:2px 12px;border-radius:4px;border:1px solid var(--line);background:var(--main);color:var(--ink);cursor:pointer}
        .btn:hover{border-color:var(--accent)}
        """;

    private static string Tile(string value, string label) =>
        $"""<div class="tile"><div class="v num">{H(value.Length > 0 ? value : "—")}</div><div class="l">{H(label)}</div></div>""";

    private static string Table(string title, string[] head, IEnumerable<string[]> rows, int numericFrom = int.MaxValue)
    {
        var sb = new StringBuilder($"<section><h2>{H(title)}</h2><div class=\"card\"><table><thead><tr>");
        foreach (var (h, i) in head.Select((h, i) => (h, i)))
        {
            sb.Append(i >= numericFrom ? $"<th style=\"text-align:right\">{H(h)}</th>" : $"<th>{H(h)}</th>");
        }
        sb.Append("</tr></thead><tbody>");
        var any = false;
        foreach (var r in rows)
        {
            any = true;
            sb.Append("<tr>");
            for (var i = 0; i < r.Length; i++)
            {
                sb.Append(i >= numericFrom ? $"<td class=\"n\">{H(r[i])}</td>" : $"<td>{H(r[i])}</td>");
            }
            sb.Append("</tr>");
        }
        if (!any)
        {
            sb.Append($"<tr><td colspan=\"{head.Length}\" class=\"muted\">—</td></tr>");
        }
        sb.Append("</tbody></table></div></section>");
        return sb.ToString();
    }
}
