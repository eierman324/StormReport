using System.Globalization;
using System.Net;
using System.Text;

namespace ChestCounter;

public static class Pretty
{
    public static string Playlist(string? playlist)
    {
        if (string.IsNullOrEmpty(playlist))
        {
            return L.T("mode inconnu", "unknown mode", "modo desconocido");
        }

        var p = playlist.ToLowerInvariant();
        var parts = new List<string>();
        if (p.Contains("habanero") || p.Contains("ranked")) parts.Add(L.T("Classée", "Ranked", "Clasificatoria"));
        if (p.Contains("nobuild")) parts.Add(L.T("Zéro construction", "Zero Build", "Cero construcción"));
        if (p.Contains("solo")) parts.Add(L.T("Solo", "Solo", "Solitario"));
        else if (p.Contains("duo")) parts.Add(L.T("Duo", "Duos", "Dúos"));
        else if (p.Contains("trio")) parts.Add(L.T("Trio", "Trios", "Tríos"));
        else if (p.Contains("squad")) parts.Add(L.T("Section", "Squads", "Escuadrones"));
        return parts.Count > 0 ? string.Join(" · ", parts) : playlist.Replace("Playlist_", "");
    }

    public static string Placement(MatchRecord m)
    {
        if (m.Placement is not int place || place <= 0)
        {
            return L.T("place inconnue", "unknown placement", "puesto desconocido");
        }
        if (place == 1)
        {
            return L.T("Top 1 !", "Victory Royale!", "¡Victoria magistral!");
        }
        return m.TotalPlayers is int total && total > 0 ? $"{L.Ordinal(place)} / {total}" : L.Ordinal(place);
    }

    public static string Clock(float seconds)
    {
        var s = (int)seconds;
        return $"{s / 60}:{s % 60:00}";
    }

    /// <summary>"à 7:16 · Coffre rare" (certain) or "vers 5:04 · Coffre rare (probable)".</summary>
    public static string Obtained(TreasureFind f)
    {
        if (f.ObtainedAtSeconds is not float t)
        {
            return L.T("moment et source inconnus", "time and source unknown", "momento y origen desconocidos");
        }
        var when = f.TimeExact ? $"{L.T("à", "at", "a los")} {Clock(t)}" : $"{L.T("vers", "around", "hacia los")} {Clock(t)}";
        return f.SourceCertain ? $"{when} · {f.SourceLabel}" : $"{when} · {f.SourceLabel} ({Likely})";
    }

    public static string Likely => L.T("probable", "likely", "probable");

    public static string ItemName(string item)
    {
        var name = item;
        foreach (var prefix in new[] { "WID_", "Athena", "AmmoData" })
        {
            name = name.Replace(prefix, "");
        }
        return name.Replace("_", " ").Trim();
    }
}

/// <summary>Replays analyzed so far / to analyze, shown as a loading card in the report.</summary>
public sealed record ScanProgress(int Done, int Total);

public static class ReportWriter
{
    public static readonly DateTime SessionStartUtc = DateTime.UtcNow;

    /// <summary>Set while several replays are being analyzed: the report shows a loading card and refreshes faster.</summary>
    public static volatile ScanProgress? Progress;

    /// <summary>A match belongs to this session if it ended after the tool was started (backlog replays do not count).</summary>
    public static bool IsThisSession(MatchRecord m)
    {
        var start = m.MatchStartUtc ?? m.AnalyzedAtUtc;
        if (start.Kind == DateTimeKind.Unspecified)
        {
            start = DateTime.SpecifyKind(start, DateTimeKind.Utc);
        }
        var end = start.ToUniversalTime().AddSeconds(m.DurationSeconds ?? 0);
        return end >= SessionStartUtc.AddMinutes(-2);
    }
    private static CultureInfo Fr => L.Culture;

    public static void WriteAll(Store store, Config config, TrackerData data)
    {
        try
        {
            var view = Profiles.View(data, config);
            WriteAtomic(store.ReportPath, Report(config, view, Profiles.List(data)));
            // The OBS page keeps what the overlay shows (a match in progress stays "live"), in the chosen style.
            OverlayStyle = config.OverlayStyle;
            WriteAtomic(store.OverlayPath, Overlay(LastOverlay ?? OverlayModel.Build(view, null, false, config.ShowSquadTotal, config.OverlayMain)));
            WriteAtomic(store.CsvPath, Csv(data));
        }
        catch (Exception ex)
        {
            Console.WriteLine(L.T($"Impossible d'écrire le rapport : {ex.Message}", $"Cannot write the report: {ex.Message}", $"No se puede escribir el informe: {ex.Message}"));
        }
    }

    private static void WriteAtomic(string path, string content)
    {
        var tmp = path + ".tmp";
        File.WriteAllText(tmp, content, new UTF8Encoding(false));
        File.Move(tmp, path, overwrite: true);
    }

    private static string H(string? s) => WebUtility.HtmlEncode(s ?? "");

    /// <summary>Start of a match as shown (local time or UTC, from the settings).</summary>
    private static DateTime When(MatchRecord m) => L.Shown(m.MatchStartUtc ?? m.AnalyzedAtUtc);

    /// <summary>A short date and time ("Nov 8, 6:00 AM"), shown in local time or UTC.</summary>
    private static string ShortWhen(DateTime utc) => L.Shown(utc).ToString(L.T("d MMM ", "MMM d, ", "d MMM ") + L.TimePattern(), Fr) + L.Zone;

    // Obsidian look (the mspaint UI library): near-black panels, groupboxes with a title bar, IBM Plex Mono, purple accent.
    internal const string Style = """
        :root{color-scheme:dark;--bg:#0d0d0d;--panel:#0f0f0f;--main:#191919;--line:#282828;--line2:#363636;--ink:#ffffff;--ink2:#ffffff;--muted:rgba(255,255,255,.6);--dim:rgba(255,255,255,.4);--title:#fff;--card:#0f0f0f;--cink:#fff;--cmuted:rgba(255,255,255,.6);--track:#191919;--gold:#7d55ff;--accent:#7d55ff;--accentSoft:rgba(125,85,255,.14);--star:#a68bff;--team:#2fd28a;--warn:#ff5a4f;--warnbg:rgba(255,90,79,.10);--hot:#ff8a3d;--hotSoft:rgba(255,138,61,.12);--tipbg:#191919;--tipink:#fff;--mono:"IBM Plex Mono","Cascadia Mono","Cascadia Code",Consolas,monospace}
        *{box-sizing:border-box}
        body{margin:0;background:var(--bg);color:var(--ink);font:12.5px/1.45 var(--mono)}
        button{font-family:var(--mono)}
        main{max-width:1080px;margin:0 auto;padding:16px 12px 32px;display:flex;flex-direction:column;gap:10px}
        .pirate{font-family:inherit;font-weight:400}
        .num{font-family:var(--mono);font-variant-numeric:tabular-nums}
        header{display:flex;flex-wrap:wrap;align-items:center;justify-content:space-between;gap:12px;background:var(--panel);border:1px solid var(--line);border-radius:4px;padding:8px 12px}
        .brand{display:flex;align-items:center;gap:10px}
        .brand>svg{width:18px;height:18px;color:var(--accent);flex:none}
        h1{margin:0;font-size:14px;line-height:1.3;font-weight:500;color:var(--title)}
        .who{font-size:12px;color:var(--muted)}.who b{color:var(--ink2);font-weight:400}
        .muted{color:var(--muted)}
        h2.part{margin:12px 0 0;font-size:13px;font-weight:500;color:var(--accent);letter-spacing:.04em;text-transform:uppercase}
        th.r{text-align:right}
        h2{margin:0;font-size:13px;line-height:1.3;font-weight:400;color:var(--ink)}
        /* Every section is an Obsidian groupbox: a panel with a title bar. */
        main>section:not(.tiles),.two>section{background:var(--panel);border:1px solid var(--line);border-radius:4px;min-width:0}
        main>section:not(.tiles)>h2,.two>section>h2,section>.head{display:flex;align-items:center;min-height:34px;margin:0;padding:0 10px;border-bottom:1px solid var(--line)}
        section>.card,.foldbody>.card{background:transparent;border:0;border-radius:0}
        .foldbody>:not(.card){margin:10px}
        .tiles{display:grid;grid-template-columns:repeat(6,minmax(0,1fr));gap:10px}
        .tile{background:var(--panel);color:var(--cink);border:1px solid var(--line);border-radius:4px;padding:10px;display:flex;flex-direction:column;gap:2px}
        .tile .v{font-size:24px;font-weight:400;line-height:1.15}
        .tile .l{font-size:11.5px;color:var(--cmuted)}
        .tile .s{font-size:11px;color:var(--dim);margin-top:4px}
        .tile.hero{grid-column:span 2;position:relative;justify-content:space-between;border-color:var(--accent)}
        .editbtn{display:none;position:absolute;top:6px;right:6px;width:22px;height:22px;padding:0;border-radius:4px;border:1px solid var(--line);background:var(--main);color:var(--muted);cursor:pointer;place-items:center}
        .editbtn svg{width:12px;height:12px}
        .editbtn:hover{color:var(--ink);border-color:var(--line2)}.editbtn:focus-visible{outline:1px solid var(--accent)}
        html.incounter .editbtn{display:grid}
        .tile.hero .k{font-size:12.5px;color:var(--ink)}
        .tile.hero .v{font-size:40px;color:var(--accent);line-height:1.1;margin:4px 0}
        .tile.hero .l{color:var(--cmuted)}
        @media (max-width:820px){.tiles{grid-template-columns:repeat(2,minmax(0,1fr))}.tile.hero{grid-column:span 2}}
        .card{background:var(--panel);color:var(--cink);border:1px solid var(--line);border-radius:4px;padding:4px 10px;overflow-x:auto}
        .two{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:10px;align-items:start}
        @media (max-width:820px){.two{grid-template-columns:1fr}}
        table{width:100%;border-collapse:collapse;font-variant-numeric:tabular-nums}
        th,td{text-align:left;padding:6px 6px;border-bottom:1px solid var(--line);vertical-align:middle}
        th{white-space:nowrap;font-size:11px;color:var(--dim);font-weight:400}
        tr:last-child td{border-bottom:none}
        tbody tr:hover td{background:rgba(255,255,255,.025)}
        td.n{text-align:right}
        td.nw{white-space:nowrap}
        .big{font-size:14px}
        .items{color:var(--cmuted);font-size:12px}
        .star{color:var(--star)}
        .gold{color:var(--accent)}
        .sw{display:inline-block;width:8px;height:8px;border-radius:2px;margin-right:8px;vertical-align:0}
        .meter{display:block;width:140px;height:8px;background:var(--main);border:1px solid var(--line);border-radius:3px;overflow:hidden}
        .meter i{display:block;height:100%}
        .flag{display:flex;gap:10px;align-items:flex-start;background:var(--warnbg);color:var(--cink);border:1px solid rgba(255,90,79,.4);border-radius:4px;padding:8px 10px;font-size:12px;margin-top:10px}
        /* Your chances */
        .ch-intro{margin:0 0 12px;color:var(--muted);font-size:14px;max-width:760px}
        .chances2{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:12px}
        @media (max-width:820px){.chances2{grid-template-columns:1fr}}
        .hcard{background:var(--card);color:var(--cink);border:1px solid var(--line);border-radius:8px;padding:14px 16px;display:flex;flex-direction:column;gap:6px}
        .hcard.got{border-color:var(--team);box-shadow:inset 6px 0 0 var(--team);padding-left:22px}
        .hcard.boosted{border-color:#e2702f;box-shadow:inset 6px 0 0 #e2702f;padding-left:22px}
        .htop{display:flex;align-items:center;gap:10px}
        .cthumb{width:44px;height:44px;border-radius:6px;object-fit:cover;background:var(--track);flex:none}
        .hname{flex:1;min-width:0}
        .hname .k{font-size:12px;letter-spacing:.12em;text-transform:uppercase;color:var(--cmuted)}
        .ciname{font-weight:700;font-size:15px}
        .hval{display:flex;align-items:center;gap:10px;flex-wrap:wrap}
        .hval .big{font-size:40px;font-weight:700;line-height:1.05}
        .hexp{font-size:14px}
        .hcard .s{font-size:13px;color:var(--cmuted)}
        .cfoot{font-size:13px;color:var(--cmuted);border-top:1px dashed var(--line);padding-top:6px;margin-top:2px}
        .halfway{margin-top:2px}.hrow{display:flex;justify-content:space-between;gap:8px;font-size:13px;color:var(--cmuted)}
        .tag{display:inline-block;padding:2px 9px;border-radius:999px;font:600 11px var(--mono);letter-spacing:.08em;text-transform:uppercase;flex:none}
        .tag.weekly{background:#1e3a5c;color:#9cc8ff}
        .tag.season{background:#2d2350;color:#c4b2ff}
        .chances2 .gotbadge,.ctable .gotbadge{display:none;padding:2px 8px;border-radius:999px;background:var(--team);color:#fff;font:600 11px var(--mono);margin-left:8px;white-space:nowrap}
        .got .gotbadge{display:inline-block!important}
        .vchip{display:inline-block;padding:2px 9px;border-radius:999px;font:500 12px var(--mono);white-space:nowrap}
        .vchip.vunlikely{background:#262626;color:#9a9a9a}
        .vchip.unlikely{background:#2e2616;color:#e0b25c}
        .vchip.possible{background:#3a2c12;color:#f0b44a}
        .vchip.likely{background:#1d2e1a;color:#8fd07a}
        .vchip.vlikely{background:#173a1f;color:#7fe0a0}
        .vchip.certain{background:#2fd28a;color:#0b0f18}
        .cbar{display:block;height:6px;background:var(--track);border-radius:3px;overflow:hidden;margin-top:5px}
        .cbar.wide{width:100%}
        .cbar i{display:block;height:6px;border-radius:3px}
        .cbar i.vunlikely,.cbar i.unlikely{background:#5b4a8a}.cbar i.possible{background:#7d55ff}
        .cbar i.likely,.cbar i.vlikely{background:#5f9c4a}.cbar i.certain{background:#2f7d4f}.cbar i.gold{background:#a68bff}
        .cgroup{margin-top:18px}
        .ghead{display:flex;align-items:center;gap:10px;margin:0 0 8px}
        .ghead .gt{font-weight:700;font-size:16px;color:var(--ink2)}
        .ghead .gs{font-size:13px;color:var(--muted)}
        .cdbox{margin-left:auto;text-align:right;flex:none}
        .cdbox .cdl{display:block;font-size:11px;letter-spacing:.1em;text-transform:uppercase;color:var(--muted)}
        .cdbox .cd{font:600 18px var(--mono);color:var(--ink2)}
        .cd{font-variant-numeric:tabular-nums}.cd.soon{color:#f0a04b}
        .ctable:not(.mini){table-layout:fixed;width:100%}
        .ctable:not(.mini) th:nth-child(1){width:38%}
        .ctable:not(.mini) th:nth-child(2){width:16%}
        .ctable:not(.mini) th:nth-child(3){width:32%}
        .ctable:not(.mini) th:nth-child(4){width:14%}
        .ctable th.r,.ctable td.n{text-align:right}
        .ctable td.n{white-space:nowrap;font-weight:700}
        .ctable td{vertical-align:middle}
        .tflex{display:flex;align-items:center;gap:10px}
        .thumb{width:36px;height:36px;border-radius:6px;object-fit:cover;background:var(--track);flex:none}
        .tk{font-weight:600}
        .iname{display:block;font-size:12px;color:var(--cmuted)}
        .tchance{min-width:0}
        .tcv{display:flex;align-items:center;justify-content:space-between;gap:8px}
        tr.got .tk{color:var(--team)}
        tr.boosted td:first-child{box-shadow:inset 4px 0 0 #e2702f;padding-left:14px}
        .cmore{margin-top:14px;color:var(--ink)}
        .cmore summary{cursor:pointer;font-weight:600;color:var(--ink2)}
        .cmore p,.cmore ul{color:var(--muted);font-size:14px;max-width:760px}
        .cmore li{margin:4px 0}
        .cmore a{color:var(--accent)}
        .ctable.mini td,.ctable.mini th{padding-top:6px;padding-bottom:6px}
        time[title]{border-bottom:1px dotted currentColor;cursor:help}
        .mo{display:none}
        @media (max-width:600px){
          .ctable th:nth-child(2),.ctable td.pc{display:none}
          .ctable:not(.mini) th:nth-child(1){width:42%}
          .ctable:not(.mini) th:nth-child(3){width:38%}
          .ctable:not(.mini) th:nth-child(4){width:20%}
          .iname.mo{display:block}
          .thumb{display:none}
          .ctable th{white-space:normal;letter-spacing:.04em}
          .tcard{padding:4px 10px}
          /* Word and percentage do not fit side by side: the percentage goes under the word */
          .tcv{flex-wrap:wrap;justify-content:flex-start;gap:2px 8px}
          .ctable td.n{white-space:normal}
          .tk{display:flex;flex-wrap:wrap;align-items:center;gap:4px}.tk .gotbadge{margin-left:0}
          .hval .big{font-size:34px}
          .ghead{align-items:flex-start;flex-wrap:wrap}
          .cdbox{margin-left:0;text-align:left;width:100%;padding-left:2px}
        }
        /* A treasure the counter ticked by itself (seen in the replays): checked, not greyed, cannot be unticked */
        .prize input[data-auto]{cursor:default}
        tr.unusual td{background:var(--warnbg)}
        .warn{color:var(--warn)}
        .chart{padding:12px 10px 8px}
        .bars{display:flex;align-items:stretch;gap:3px;height:150px;padding-top:26px}
        .col{flex:1;min-width:5px;display:flex;align-items:flex-end;position:relative;cursor:default}
        .bar{width:100%;background:rgba(125,85,255,.55);border-radius:2px 2px 0 0}
        .bar.top{background:var(--accent)}
        .bar.zero{background:var(--main);height:2px!important}
        .col:hover .bar{background:var(--accent)}
        .tip{position:absolute;left:50%;transform:translateX(-50%);white-space:nowrap;background:var(--tipbg);color:var(--tipink);border:1px solid var(--line);
             padding:2px 6px;border-radius:4px;font-size:11px;opacity:0;pointer-events:none;transition:opacity .1s;z-index:2}
        .col:nth-child(-n+3) .tip{left:0;transform:none}
        .col:nth-last-child(-n+3) .tip{left:auto;right:0;transform:none}
        .col:hover .tip{opacity:1}
        .axis{display:flex;justify-content:space-between;gap:8px;color:var(--dim);font-size:11px;margin-top:6px;border-top:1px solid var(--line);padding-top:6px}
        .head{display:flex;flex-wrap:wrap;align-items:center;justify-content:space-between;gap:12px}
        .head h2{margin:0}
        .tsw{position:relative;display:inline-flex;align-items:center;gap:8px;min-height:26px;font-size:12px;color:var(--muted);cursor:pointer;user-select:none}
        .tsw input{position:absolute;opacity:0;width:1px;height:1px;margin:0}
        .tsw .trk{position:relative;width:30px;height:16px;border-radius:8px;background:var(--main);border:1px solid var(--line);flex:none;transition:background .12s}
        .tsw .trk:after{content:"";position:absolute;top:2px;left:2px;width:10px;height:10px;border-radius:50%;background:#fff;transition:transform .12s}
        .tsw input:checked+.trk{background:var(--accent);border-color:var(--accent)}.tsw input:checked+.trk:after{transform:translateX(14px)}
        .tsw input:focus-visible+.trk{outline:1px solid var(--accent);outline-offset:1px}
        .tsw .badge{display:inline-block;margin-left:2px;padding:0 5px;border-radius:3px;border:1px solid rgba(255,90,79,.45);color:var(--warn);font-size:10.5px}
        .statsbar{display:flex;justify-content:flex-end;margin:-4px 0 -4px}
        .squadtile,.sq{display:none}
        body.showsquad .squadtile{display:flex}body.showsquad .sq{display:table-cell}
        body.showsquad .tiles.hassquad{grid-template-columns:repeat(7,minmax(0,1fr))}
        .fold .head h2{display:flex;align-items:center;gap:4px;border:0;padding:0;min-height:0}
        .foldbtn{display:inline-flex;align-items:center;gap:8px;background:none;border:0;padding:4px 0;margin:0;font:inherit;color:inherit;cursor:pointer;text-align:left}
        .foldbtn:focus-visible{outline:1px solid var(--accent);outline-offset:3px}
        .chev{order:2;width:7px;height:7px;border-right:1.5px solid currentColor;border-bottom:1.5px solid currentColor;transform:rotate(45deg);margin:0 2px 3px 6px;transition:transform .12s}
        .fold.closed .chev{transform:rotate(-45deg);margin-bottom:0}
        .fold.closed .foldbody{display:none}
        .fold.closed .head{border-bottom:0}
        .fold.closed .head>:not(h2){display:none}
        .team{display:none}
        body.showteam tr.team{display:table-row}
        body.showteam span.team{display:inline}
        tr.team td{opacity:.7}
        .by{white-space:nowrap}
        .by svg{vertical-align:-3px;margin-right:6px}
        .by svg *{stroke:var(--team)}
        footer{font-size:11.5px;color:var(--muted);text-align:center;background:#171717;border:1px solid var(--line);border-radius:4px;padding:5px 10px}
        .loading{display:flex;gap:14px;align-items:center;background:var(--panel);color:var(--cink);border:1px solid var(--line);border-radius:4px;padding:12px}
        .loading .muted{color:var(--cmuted)}
        .spin{width:20px;height:20px;flex:none;border:2px solid var(--main);border-top-color:var(--accent);border-radius:50%;animation:spin .8s linear infinite}
        @keyframes spin{to{transform:rotate(360deg)}}
        .loading .grow{flex:1;min-width:0}
        .pbar{height:17px;background:var(--main);border:1px solid var(--line);border-radius:4px;overflow:hidden;margin:8px 0 6px}
        .pbar i{display:block;height:100%;background:var(--accent)}
        .profiles{font-size:12px}
        .embed,.embed body{background:transparent;overflow:hidden}.embed main{padding:0;max-width:none}.embed header,.embed footer,.embed .profiles,.embed .notice{display:none}
        .info{position:relative;display:inline-flex;align-items:center;justify-content:center;width:15px;height:15px;margin-left:6px;border-radius:3px;border:1px solid var(--line2);color:var(--muted);font:500 10px/1 var(--mono);cursor:help;vertical-align:1px;outline:none}
        .info:hover,.info:focus{border-color:var(--accent);color:var(--ink)}
        .infotip{display:none;position:absolute;z-index:5;top:22px;right:-12px;width:300px;max-width:80vw;background:var(--tipbg);color:var(--tipink);border:1px solid var(--line);padding:8px 10px;border-radius:4px;font:400 12px/1.45 var(--mono);text-align:left;box-shadow:0 10px 30px rgba(0,0,0,.6)}
        .info:hover .infotip,.info:focus .infotip{display:block}
        h2 .infotip{left:-12px;right:auto}
        tr.pg{display:none!important}
        .pager{display:flex;flex-wrap:wrap;align-items:center;gap:6px;padding:8px 0 6px;border-top:1px solid var(--line)}
        .pager[hidden],.pager button[hidden]{display:none}
        .pager .count{color:var(--dim);font-size:12px;margin-right:auto}
        .pager button{min-height:24px;padding:2px 10px;border-radius:4px;border:1px solid var(--line);background:var(--main);color:var(--ink);font:400 12px var(--mono);cursor:pointer}
        .pager button:hover{border-color:var(--line2)}
        .notice{background:var(--panel);color:var(--cink);border:1px solid var(--line);border-left:2px solid var(--accent);border-radius:4px;padding:8px 12px;white-space:pre-line}
        .notice.warning{border-left-color:var(--warn)}
        .exper{display:inline-block;font:400 10.5px var(--mono);padding:0 5px;border-radius:3px;border:1px solid rgba(255,90,79,.45);color:var(--warn);margin-left:8px;vertical-align:1px}
        .thour{background:var(--main);color:var(--cink);border:1px solid var(--line);border-left:2px solid var(--accent);border-radius:4px;padding:8px 10px}
        .thour.live{border-left-color:var(--hot);background:var(--hotSoft)}
        .thour .exp{color:var(--muted)}
        .boost{display:inline-block;margin-left:8px;padding:0 5px;border-radius:3px;border:1px solid rgba(255,138,61,.5);color:var(--hot);font:400 10.5px var(--mono);vertical-align:1px}
        .boost[hidden]{display:none}
        .odds.boosted{border-color:var(--hot)}
        tr.boosted td:first-child{box-shadow:inset 2px 0 0 var(--hot);padding-left:10px}
        .boostv{color:var(--hot)}
        .thour .tb{display:flex;flex-wrap:wrap;gap:4px 16px;font-size:12px;margin-top:4px}
        .chances{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:10px}
        @media (max-width:820px){.chances{grid-template-columns:1fr}}
        .odds{background:var(--main);color:var(--cink);border:1px solid var(--line);border-radius:4px;padding:10px;display:flex;flex-direction:column;gap:3px}
        .odds .k{font-size:12px;color:var(--ink);display:flex;justify-content:space-between;gap:8px}
        .odds .v{font-size:28px;line-height:1.15}
        .odds .l,.odds .s{font-size:11.5px;color:var(--cmuted)}
        .meter.wide{width:100%;height:17px;margin:6px 0 2px}.meter.wide i{background:var(--accent)}
        .gotbadge{display:none;color:var(--team)}
        .odds.got{border-color:var(--team)}.odds.got .gotbadge{display:inline}
        .collection{padding:4px 10px 10px}
        .wk{padding:8px 0}.wk+.wk{border-top:1px solid var(--line)}
        .wt{margin-bottom:8px}.wk.now .wt{color:var(--accent)}
        .prizes{display:grid;grid-template-columns:repeat(auto-fill,minmax(130px,1fr));gap:8px}
        .prize{position:relative;display:flex;flex-direction:column;gap:2px;padding:6px;border:1px solid var(--line);border-radius:4px;cursor:default;background:var(--main)}
        .prize:hover{border-color:var(--line2)}
        .prize input{display:none}
        .prize .gotmark{position:absolute;top:10px;left:10px;width:18px;height:18px;border-radius:50%;background:var(--accent);color:#fff;font-size:11px;display:grid;place-items:center;z-index:1}
        .prize .pimg{display:block;aspect-ratio:1;border-radius:3px;background:var(--panel);overflow:hidden;margin-bottom:4px}
        .prize img{width:100%;height:100%;object-fit:cover;display:block;filter:grayscale(.8);opacity:.5}
        .prize .pn{font-size:12px;line-height:1.3}
        .prize .pk{font-size:11px;color:var(--dim)}
        .prize.got{border-color:var(--accent)}
        .prize.got img{filter:none;opacity:1}
        ::-webkit-scrollbar{width:8px;height:8px}::-webkit-scrollbar-track{background:transparent}::-webkit-scrollbar-thumb{background:var(--line);border-radius:4px}
        """;

    private const string ChestIcon = """<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M4 10a4 4 0 0 1 4-4h8a4 4 0 0 1 4 4v9H4z"/><path d="M4 12h16"/><rect x="10.5" y="10.5" width="3" height="4" rx="1"/></svg>""";
    private const string TeamIcon = """<svg width="16" height="16" viewBox="0 0 24 24" aria-hidden="true"><circle cx="9" cy="8" r="3.2" fill="none" stroke="#2fb48a" stroke-width="2"/><path d="M3 20c0-3.5 2.7-6 6-6s6 2.5 6 6" fill="none" stroke="#2fb48a" stroke-width="2"/><circle cx="17" cy="9" r="2.6" fill="none" stroke="#2fb48a" stroke-width="2"/><path d="M16 14c3 0 5 2.2 5 5" fill="none" stroke="#2fb48a" stroke-width="2"/></svg>""";
    private const string FlagIcon = """<svg width="18" height="18" viewBox="0 0 24 24" aria-hidden="true" style="flex:none;margin-top:2px"><path d="M5 21V4M5 4h11l-2 4 2 4H5" fill="none" stroke="#ff8a6a" stroke-width="2" stroke-linejoin="round"/></svg>""";

    private static readonly Dictionary<string, string> KindColor = new()
    {
        [ChestKinds.Normal] = "#e0a93b",
        [ChestKinds.Rare] = "#4f8fe0",
        [ChestKinds.Buried] = "#a7774a",
        [ChestKinds.Extraction] = "#2fb48a",
        [ChestKinds.PosterChest] = "#9a7ae8",
        [ChestKinds.Other] = "#8d8e9a",
    };

    public static string Report(Config config, TrackerData data, IReadOnlyList<Profile> profiles)
    {
        var progress = Progress;
        var matches = data.Matches.OrderByDescending(When).ToList();
        var totalChests = matches.Sum(m => m.ChestsOpened);
        // Shown as the total: counted chests + those added by hand. Averages and odds stay on the counted ones.
        // With the Count* settings, the totals shown include ammo boxes, loot cannons...; the odds never do.
        var grandTotal = data.ShownTotal;
        var shownChests = matches.Sum(m => m.Counted);
        // The squad total is shown only when asked, with the switch above the tiles (it is inaccurate: teammates far from
        // the player are not in the replays).
        var teamChests = data.TeamChests;
        var avg = matches.Count > 0 ? (double)shownChests / matches.Count : 0;
        var best = matches.Count > 0 ? matches.Max(m => m.Counted) : 0;
        var treasures = matches.Sum(m => m.TreasureFinds.Count);
        var teamTreasures = matches.Sum(m => m.TeamTreasureFinds.Count);
        var session = matches.Where(IsThisSession).Sum(m => m.Counted);
        var rates = Stats.Compute(matches);
        var all = MatchStats.Summarize(matches);
        var player = matches.Select(m => m.PlayerName).FirstOrDefault(n => !string.IsNullOrEmpty(n)) ?? "";
        // Own treasures only: the team's are found with chests that are not counted here.
        var every = treasures > 0 ? (int)Math.Round((double)totalChests / treasures) : 0;

        var sb = new StringBuilder();
        var title = AppInfo.Name;
        var updated = L.T(
            $"Mis à jour le {L.Shown(DateTime.UtcNow).ToString("dd/MM/yyyy", Fr)} à {L.Clock(L.Shown(DateTime.UtcNow), seconds: true)} · actualisation auto",
            $"Updated {L.Shown(DateTime.UtcNow).ToString("d MMM yyyy", Fr)} at {L.Clock(L.Shown(DateTime.UtcNow), seconds: true)} · auto refresh",
            $"Actualizado el {L.Shown(DateTime.UtcNow).ToString("dd/MM/yyyy", Fr)} a las {L.Clock(L.Shown(DateTime.UtcNow), seconds: true)} · actualización automática");
        string Tile(string value, string label, string? sub = null) =>
            $"""<div class="tile"><div class="v num">{value}</div><div class="l">{label}</div>{(sub != null ? $"<div class=\"s\">{sub}</div>" : "")}</div>""";

        sb.Append($"""
            <!doctype html><html lang="{L.Code}"><head><meta charset="utf-8">
            <meta name="viewport" content="width=device-width,initial-scale=1">
            <meta http-equiv="refresh" content="{(progress != null ? 2 : 15)}">
            <title>{title}</title><style>{Style}</style></head><body data-showsquad="{(config.ShowSquadInReport ? 1 : 0)}" data-showteam="{(config.ShowTeamTreasuresInReport ? 1 : 0)}"><main>
            <header><div class="brand">{ChestIcon}<div><h1 class="pirate">{title}</h1>{(player.Length > 0 ? $"<div class=\"who\"><b>{H(player)}</b></div>" : "")}</div></div>
            <span class="muted" style="font-size:13px">{updated}</span></header>
            {LoadingCard(progress)}{NoticeCards()}{ProfilesLine(profiles, player)}
            <section class="tiles">
              <div class="tile hero"><div class="k">{L.T("Victoires", "Wins", "Victorias")}</div><div class="v num">{all.Wins.ToString("N0", Fr)}</div><div class="l">{L.T($"{all.WinRate} des {all.Matches.ToString("N0", Fr)} parties analysées · top 10 : {all.Top10Rate}", $"{all.WinRate} of {all.Matches.ToString("N0", Fr)} matches analyzed · top 10: {all.Top10Rate}", $"{all.WinRate} de {all.Matches.ToString("N0", Fr)} partidas analizadas · top 10: {all.Top10Rate}")}</div></div>
              {Tile(all.Kills.ToString("N0", Fr), L.T("éliminations", "eliminations", "eliminaciones"), L.T($"K/D {all.KD} · {all.KillsPerMatch} par partie", $"K/D {all.KD} · {all.KillsPerMatch} per match", $"K/D {all.KD} · {all.KillsPerMatch} por partida"))}
              {Tile(all.AvgPlacementText, L.T("place moyenne", "average placement", "puesto medio"))}
              {Tile(all.AvgDamageText, L.T("dégâts par partie", "damage per match", "daño por partida"), L.T($"précision {all.AccuracyText}", $"accuracy {all.AccuracyText}", $"precisión {all.AccuracyText}"))}
              {Tile(all.Assists.ToString("N0", Fr), L.T("assistances", "assists", "asistencias"), L.T($"{all.Revives.ToString("N0", Fr)} réanimations", $"{all.Revives.ToString("N0", Fr)} revives", $"{all.Revives.ToString("N0", Fr)} reanimaciones"))}
              {Tile(all.HoursText, L.T("temps de jeu", "time played", "tiempo de juego"), L.T($"{all.Materials.ToString("N0", Fr)} matériaux récoltés", $"{all.Materials.ToString("N0", Fr)} materials gathered", $"{all.Materials.ToString("N0", Fr)} materiales recogidos"))}
            </section>
            {EpicSection()}{ModesSection(matches)}
            <h2 class="part">{L.T("Coffres et trésors", "Chests & treasures", "Cofres y tesoros")}</h2>
            <section class="tiles{(teamChests > 0 ? " hassquad" : "")}">
              <div class="tile hero"><button type="button" class="editbtn" id="editchests" title="{H(L.T("Modifier mon nombre de coffres", "Edit my number of chests", "Modificar mi número de cofres"))}" aria-label="{H(L.T("Modifier mon nombre de coffres", "Edit my number of chests", "Modificar mi número de cofres"))}">{PencilIcon}</button><div class="k">{L.T("Coffres au total", "Chests in total", "Cofres en total")}</div><div class="v num">{grandTotal.ToString("N0", Fr)}</div><div class="l">{L.T($"ouverts ou détruits par toi · {session.ToString("N0", Fr)} depuis le lancement", $"opened or destroyed by you · {session.ToString("N0", Fr)} since the tool started", $"abiertos o destruidos por ti · {session.ToString("N0", Fr)} desde que se abrió")}{(data.ManualAdjust != 0 ? "<br>" + L.T($"dont {data.ManualAdjust.ToString("+#,0;-#,0", Fr)} ajoutés à la main", $"{data.ManualAdjust.ToString("+#,0;-#,0", Fr)} of them added by hand", $"de ellos, {data.ManualAdjust.ToString("+#,0;-#,0", Fr)} añadidos a mano") : "")}</div></div>
              {(teamChests > 0 ? Tile((grandTotal + teamChests).ToString("N0", Fr), L.T("total de l'équipe", "squad total", "total del equipo") + SquadInfo, L.T($"toi + {teamChests.ToString("N0", Fr)} par tes coéquipiers", $"you + {teamChests.ToString("N0", Fr)} by your teammates", $"tú + {teamChests.ToString("N0", Fr)} de tus compañeros")).Replace("<div class=\"tile\">", "<div class=\"tile squadtile\">") : "")}
              {Tile(matches.Count.ToString("N0", Fr), L.T("parties analysées", "matches analyzed", "partidas analizadas"))}
              {Tile(avg.ToString("0.0", Fr), L.T("coffres par partie", "chests per match", "cofres por partida"))}
              {Tile(best.ToString("N0", Fr), L.T("record en une partie", "best match", "récord en una partida"))}
              {Tile(rates.ChestsPerHourText, L.T("coffres par heure", "chests per hour", "cofres por hora"), L.T($"{rates.HoursPlayedText} de jeu", $"{rates.HoursPlayedText} played", $"{rates.HoursPlayedText} jugadas"))}
              {Tile(rates.TreasuresPerHourText, L.T("trésors par heure", "treasures per hour", "tesoros por hora"))}
              {Tile(rates.ChestsSinceTreasure.ToString("N0", Fr), rates.AnyTreasure ? L.T("coffres depuis le dernier trésor", "chests since the last treasure", "cofres desde el último tesoro") : L.T("coffres sans trésor pour l'instant", "chests without a treasure so far", "cofres sin tesoro por ahora"))}
              {Tile(treasures.ToString("N0", Fr), L.T("trésors trouvés", "treasures found", "tesoros encontrados"),
                  (every > 0 ? L.T($"1 tous les {every.ToString("N0", Fr)} coffres", $"1 every {every.ToString("N0", Fr)} chests", $"1 cada {every.ToString("N0", Fr)} cofres") : null)
                  + (teamTreasures > 0 ? (every > 0 ? "<br>" : "") + L.T($"+{teamTreasures} par l'équipe (estimation)", $"+{teamTreasures} by the team (estimate)", $"+{teamTreasures} del equipo (estimación)") : ""))}
            </section>
            {(teamChests > 0 ? $"<div class=\"statsbar\">{Switch("showsquad", L.T("Afficher le total d'équipe", "Show the squad total", "Mostrar el total del equipo") + $" <span class=\"badge\">{L.T("imprécis", "inaccurate", "impreciso")}</span>")}</div>" : "")}
            """);

        // Last 30 days, one bar per day (the best day stands out).
        var today = L.Shown(DateTime.UtcNow).Date;
        var days = Enumerable.Range(0, 30).Select(i => today.AddDays(i - 29)).ToList();
        var perDay = days.Select(d => matches.Where(m => When(m).Date == d).Sum(m => m.Counted)).ToList();
        var max = Math.Max(1, perDay.Max());
        sb.Append($"<section><h2 class=\"pirate\">{L.T("Coffres par jour", "Chests per day", "Cofres por día")}</h2><div class=\"card chart\"><div class=\"bars\">");
        for (var i = 0; i < days.Count; i++)
        {
            var v = perDay[i];
            var h = (int)Math.Round(100.0 * v / max);
            var tip = $"{days[i].ToString(L.T("ddd dd/MM", "ddd d MMM", "ddd dd/MM"), Fr)}{L.T(" : ", ": ", ": ")}{L.Count(v, "coffre", "chest", "cofre")}";
            var cls = v == 0 ? " zero" : v == max ? " top" : "";
            sb.Append($"<div class=\"col\" aria-label=\"{H(tip)}\"><span class=\"tip\" style=\"bottom:calc({Math.Max(h, 3)}% + 6px)\">{H(tip)}</span><div class=\"bar{cls}\" style=\"height:{h}%\"></div></div>");
        }
        var bestDay = perDay[perDay.IndexOf(perDay.Max())];
        var bestDate = days[perDay.IndexOf(perDay.Max())];
        sb.Append($"</div><div class=\"axis\"><span>{days[0].ToString(L.T("dd/MM", "d MMM", "dd/MM"), Fr)}</span>"
            + (bestDay > 0 ? $"<span>{L.T("meilleur jour", "best day", "mejor día")} : {bestDate.ToString(L.T("dd/MM", "d MMM", "dd/MM"), Fr)} · {L.Count(bestDay, "coffre", "chest", "cofre")}</span>" : "")
            + $"<span>{L.T("aujourd'hui", "today", "hoy")}</span></div></div></section>");

        // By chest type (with the other interactions, not in the total) | where the player's treasures come from.
        var byKind = matches.SelectMany(m => m.Chests).GroupBy(c => ChestKinds.Canonical(c.Kind)).OrderByDescending(g => g.Count()).ToList();
        var kindMax = Math.Max(1, byKind.Count > 0 ? byKind.Max(g => g.Count()) : 1);
        var otherKinds = matches
            .SelectMany(m => m.OtherInteractions.Count > 0
                ? m.OtherInteractions
                : new Dictionary<string, int> { [ChestKinds.AmmoBox] = m.AmmoBoxesOpened, [ChestKinds.OtherContainer] = m.OtherContainersOpened })
            .GroupBy(kv => kv.Key)
            .Select(g => (Kind: g.Key, Count: g.Sum(kv => kv.Value)))
            .Where(x => x.Count > 0)
            .OrderByDescending(x => x.Count)
            .ToList();
        sb.Append($"<div class=\"two\"><section><h2 class=\"pirate\">{L.T("Par type de coffre", "By chest type", "Por tipo de cofre")}</h2><div class=\"card\"><table>");
        foreach (var g in byKind)
        {
            var color = KindColor.GetValueOrDefault(g.Key, "#8d8e9a");
            var w = Math.Max(2, (int)Math.Round(100.0 * g.Count() / kindMax));
            sb.Append($"<tr><td><span class=\"sw\" style=\"background:{color}\"></span>{H(L.Name(g.Key))}</td><td style=\"width:150px\"><span class=\"meter\"><i style=\"width:{w}%;background:{color}\"></i></span></td><td class=\"n num big\">{g.Count().ToString("N0", Fr)}</td></tr>");
        }
        var destroyedTotal = matches.Sum(m => m.Chests.Count(c => c.Destroyed));
        var notes = new List<string>();
        if (destroyedTotal > 0)
        {
            notes.Add(L.T($"dont {destroyedTotal.ToString("N0", Fr)} détruits (cassés au lieu d'être ouverts)", $"of which {destroyedTotal.ToString("N0", Fr)} destroyed (broken instead of opened)", $"de ellos, {destroyedTotal.ToString("N0", Fr)} destruidos (rotos en lugar de abiertos)"));
        }
        foreach (var (kind, count) in otherKinds)
        {
            sb.Append($"<tr class=\"items\"><td>{H(L.Name(kind))} {(Counting.Includes(kind) ? L.T("(compté)", "(counted)", "(contado)") : L.T("(hors total)", "(not in the total)", "(fuera del total)"))}</td><td></td><td class=\"n num\">{count.ToString("N0", Fr)}</td></tr>");
        }
        sb.Append("</table>");
        if (notes.Count > 0)
        {
            sb.Append($"<div class=\"items\" style=\"padding:10px 0 8px\">{string.Join(" · ", notes)}</div>");
        }
        sb.Append("</div></section>");

        var treasureRows = matches.SelectMany(m => m.TreasureFinds.Select(f => (m, f, Team: false))).ToList();
        var teamRows = matches.SelectMany(m => m.TeamTreasureFinds.Select(f => (m, f, Team: true))).ToList();
        sb.Append($"<section><h2 class=\"pirate\">{L.T("D'où viennent tes trésors", "Where your treasures come from", "De dónde vienen tus tesoros")}</h2><div class=\"card\"><table>");
        if (treasureRows.Count == 0)
        {
            sb.Append($"<tr><td class=\"items\">{L.T("Pas encore de trésor.", "No treasure yet.", "Todavía ningún tesoro.")}</td></tr>");
        }
        foreach (var g in treasureRows.GroupBy(r => r.f.SourceLabel).OrderByDescending(g => g.Count()))
        {
            var isChest = g.First().f.SourceIsChest;
            sb.Append(isChest
                ? $"<tr><td>{H(g.Key)}</td><td class=\"n num big\">{g.Count()}</td></tr>"
                : $"<tr class=\"unusual\"><td class=\"warn\">{H(g.Key)} <span style=\"font-size:12px\">· {L.T("fiabilité faible", "low reliability", "fiabilidad baja")}</span></td><td class=\"n num big warn\">{g.Count()}</td></tr>");
        }
        sb.Append("</table></div>");
        var unusual = treasureRows.Where(r => !r.f.SourceIsChest).ToList();
        if (unusual.Count > 0)
        {
            var count = L.Count(unusual.Count, "trésor obtenu", "trésors obtenus", "treasure", "treasures", "tesoro obtenido", "tesoros obtenidos");
            sb.Append($"<div class=\"flag\">{FlagIcon}<span>" + L.T(
                $"<b>{count} autrement qu'avec un coffre.</b> Fiabilité faible : il s'agit très probablement d'une erreur dans la façon dont les replays sont analysés.",
                $"<b>{count} obtained other than from a chest.</b> Low reliability: most likely a mistake in the way the replays are analyzed.",
                $"<b>{count} de otra forma que con un cofre.</b> Fiabilidad baja: muy probablemente un error en la forma de analizar las repeticiones.") + "</span></div>");
        }
        sb.Append("</section></div>");

        sb.Append(ChancesSection(data.TotalChests, matches, data.ManualEntries)); // real chests only, not the counted extras
        sb.Append(SeenSection(matches));
        sb.Append(CollectionSection(matches));

        // Treasures: the player's own, then (hidden unless asked) those found by teammates, shared with the whole team.
        if (treasureRows.Count > 0 || teamRows.Count > 0)
        {
            var namesInfo = L.T(
                "Pourquoi certains trésors ont un nom et d'autres non ? Un trésor majeur (tenue, pioche, piste, écran, véhicule, accès à la boutique pirate) sort d'un coffre doré qui porte son identifiant exact : il est nommé à coup sûr (★★), à condition que ce coffre soit dans ton replay, donc à moins d'environ 150 m de toi. Un gizmo est nommé quand il apparaît dans tes mains. Sinon le replay dit seulement qu'un trésor a été reçu, sans dire lequel : « Trésor (type inconnu) ». C'est toujours le cas pour un trésor trouvé par un coéquipier loin de toi.",
                "Why do some treasures have a name and others not? A major treasure (outfit, pickaxe, track, loading screen, vehicle, Pirate Shop Access) comes out of a golden chest that carries its exact id: it is named for sure (★★), as long as that chest is in your replay, that is within about 150 m of you. A gizmo is named when it shows up in your hands. Otherwise the replay only says that a treasure was received, not which one: \"Treasure (unknown type)\". That is always the case for a treasure found by a teammate far from you.",
                "¿Por qué unos tesoros tienen nombre y otros no? Un tesoro mayor (traje, pico, pista, pantalla de carga, vehículo, acceso a la tienda pirata) sale de un cofre dorado que lleva su identificador exacto: se nombra con seguridad (★★), siempre que ese cofre esté en tu repetición, es decir a menos de unos 150 m de ti. Un artilugio se nombra cuando aparece en tus manos. Si no, la repetición solo dice que se recibió un tesoro, no cuál: «Tesoro (tipo desconocido)». Siempre es el caso de un tesoro encontrado por un compañero lejos de ti.");
            var teamSwitch = teamRows.Count > 0
                ? Switch("showteam", L.T($"Afficher aussi l'équipe (+{teamRows.Count}, estimation)", $"Also show the team (+{teamRows.Count}, estimate)", $"Mostrar también el equipo (+{teamRows.Count}, estimación)"))
                : "";
            sb.Append(FoldStart("treasures", $"{L.T("Trésors trouvés", "Treasures found", "Tesoros encontrados")} · <span class=\"num\">{treasures}</span>", open: false, Info(namesInfo), teamSwitch));
            sb.Append($"<div class=\"card\"><table id=\"t-treasures\"><tr><th>{L.T("Partie", "Match", "Partida")}</th><th>{L.T("Trésor", "Treasure", "Tesoro")}</th><th>{L.T("Obtenu à", "Obtained at", "Momento")}</th><th>{L.T("Source", "Source", "Origen")}</th><th>{L.T("Par", "By", "Por")}</th></tr>");
            foreach (var (m, f, team) in treasureRows.Concat(teamRows).OrderByDescending(r => When(r.m)).ThenBy(r => r.f.ObtainedAtSeconds ?? 0))
            {
                var when = f.ObtainedAtSeconds is float t ? (f.TimeExact ? Pretty.Clock(t) : $"{L.T("vers", "around", "hacia los")} {Pretty.Clock(t)}") : "?";
                string source;
                if (team && string.IsNullOrEmpty(f.Teammate))
                {
                    source = $"<span class=\"items\">{L.T("coffre hors de ta vue", "chest out of your sight", "cofre fuera de tu vista")}</span>";
                }
                else if (!f.SourceIsChest)
                {
                    source = $"<span class=\"warn\">{H(f.SourceLabel)} · {L.T("fiabilité faible", "low reliability", "fiabilidad baja")}</span>";
                }
                else
                {
                    source = H(f.SourceLabel) + (f.SourceCertain ? "" : $" <span class=\"items\">({Pretty.Likely})</span>");
                }
                var who = !team ? L.T("toi", "you", "tú")
                    : TeamIcon + (string.IsNullOrEmpty(f.Teammate) ? L.T("un coéquipier", "a teammate", "un compañero") : H(f.Teammate));
                var cls = team ? "r team" : f.SourceIsChest ? "r" : "r unusual";
                var stars = f.FromGoldenChest ? "<span class=\"gold\">★★</span>" : "<span class=\"star\">★</span>";
                sb.Append($"<tr class=\"{cls}\"><td class=\"nw num items\">{L.DateTimeShort(When(m))}</td><td>{stars} <b>{H(f.DisplayName)}</b></td><td class=\"nw num\">{when}</td><td>{source}</td><td class=\"by\">{who}</td></tr>");
            }
            sb.Append($"</table>{Pager("t-treasures")}</div>{FoldEnd}");
        }

        // Latest matches.
        sb.Append(FoldStart("matches", $"{L.T("Dernières parties", "Latest matches", "Últimas partidas")} · <span class=\"num\">{matches.Count.ToString("N0", Fr)}</span>", open: false));
        sb.Append($"<div class=\"card\"><table id=\"t-matches\"><tr><th>{L.T("Date", "Date", "Fecha")}</th><th>{L.T("Mode", "Mode", "Modo")}</th><th>{L.T("Place", "Placement", "Puesto")}</th><th style=\"text-align:right\">{L.T("Élim.", "Elims", "Elim.")}</th><th style=\"text-align:right\">{L.T("Dégâts", "Damage", "Daño")}</th><th style=\"text-align:right\">{L.T("Coffres", "Chests", "Cofres")}</th>{(teamChests > 0 ? $"<th class=\"sq\" style=\"text-align:right\">{H(OverlayModel.SquadLabel)}</th>" : "")}<th>{L.T("Trésor", "Treasure", "Tesoro")}</th></tr>");
        foreach (var m in matches)
        {
            var treasure = string.Join("<br>", m.TreasureFinds.Select(f => $"<span class=\"{(f.FromGoldenChest ? "gold" : "star")}\">{(f.FromGoldenChest ? "★★" : "★")} {H(f.DisplayName)}</span>"))
                + string.Concat(m.TeamTreasureFinds.Select((f, i) => $"<span class=\"team\">{(i == 0 && m.TreasureFinds.Count == 0 ? "" : "<br>")}{TeamIcon} ★ {H(f.DisplayName)}</span>"));
            var win = m.Placement == 1;
            sb.Append($"<tr class=\"r\"><td class=\"nw num items\">{L.DateTimeShort(When(m))}</td><td>{H(Pretty.Playlist(m.Playlist))}</td><td class=\"nw num\"{(win ? " style=\"color:var(--accent);font-weight:600\"" : "")}>{H(Pretty.Placement(m))}</td><td class=\"n num\">{(m.Eliminations?.ToString(Fr) ?? "—")}{(m.Assists is > 0 ? $"<span class=\"items\"> +{m.Assists}</span>" : "")}</td><td class=\"n num\">{(m.DamageDealt?.ToString("N0", Fr) ?? "—")}</td><td class=\"n num big\">{m.Counted}{(m.VehicleChestsMissing ? $" <span class=\"warn\" title=\"{H(VehicleWarning)}\">⚠</span>" : "")}</td>{(teamChests > 0 ? $"<td class=\"n num sq\" title=\"{H(string.Join(", ", m.TeamChests.Where(x => x.Chests > 0).Select(x => $"{x.Name} {x.Chests}")))}\">{(m.TeamChests.Count > 0 ? "+" + m.TeamChestsOpened : "")}</td>" : "")}<td class=\"items\">{treasure}</td></tr>");
        }
        sb.Append($"</table>{Pager("t-matches")}</div>");
        var blind = matches.Count(m => m.VehicleChestsMissing);
        if (blind > 0)
        {
            sb.Append($"<div class=\"flag\">{FlagIcon}<span><b>⚠ {L.Count(blind, "partie", "parties", "match", "matches", "partida", "partidas")}</b> · {H(VehicleWarning)}</span></div>");
        }
        sb.Append(FoldEnd);

        // Page script: team rows shown or not, long tables folded ("show more"), treasures ticked as obtained.
        // Everything is remembered in this browser only (the page is rewritten every few seconds by the counter).
        const string script = """
            <script>(function(){
            function get(k){try{return localStorage.getItem(k)}catch(e){return null}}
            function put(k,v){try{localStorage.setItem(k,v)}catch(e){}}
            var pagers=[];
            [].forEach.call(document.querySelectorAll('.pager'),function(box){
              var t=document.getElementById(box.getAttribute('data-for'));if(!t)return;
              var step=+box.getAttribute('data-step')||10,key='chestcounter-rows-'+t.id,n=parseInt(get(key),10);if(!(n>=step))n=step;
              var more=box.querySelector('.more'),all=box.querySelector('.all'),less=box.querySelector('.less'),info=box.querySelector('.count');
              function rows(){return [].filter.call(t.querySelectorAll('tr.r'),function(r){return !r.classList.contains('team')||document.body.classList.contains('showteam')})}
              function apply(){
                [].forEach.call(t.querySelectorAll('tr.r'),function(r){r.classList.remove('pg')});
                var v=rows();v.forEach(function(r,i){if(i>=n)r.classList.add('pg')});
                var shown=Math.min(n,v.length);
                info.textContent=info.getAttribute('data-fmt').replace('{0}',shown).replace('{1}',v.length);
                more.hidden=all.hidden=shown>=v.length;less.hidden=n<=step;box.hidden=v.length<=step;
              }
              more.onclick=function(){n+=step*2;put(key,n);apply()};
              all.onclick=function(){n=1e9;put(key,n);apply()};
              less.onclick=function(){n=step;put(key,n);apply()};
              apply();pagers.push(apply);
            });
            if(window.self!==window.top)document.documentElement.classList.add('embed');
            // Opened in the counter's window (?t=...): choices are saved by the counter itself (config, data.json).
            var tok=new URLSearchParams(location.search).get('t');
            // In the counter's window, the pencil of "chests in total" opens the window's edit box.
            if(tok&&window.self!==window.top){document.documentElement.classList.add('incounter');var eb=document.getElementById('editchests');if(eb)eb.onclick=function(){try{window.parent.postMessage({cc:'editChests'},location.origin)}catch(e){}}}
            function post(action,value){if(!tok)return;try{fetch('/api/action',{method:'POST',headers:{'X-Token':tok,'Content-Type':'application/json'},body:JSON.stringify({action:action,value:value})})}catch(e){}}
            // Switches: the counter's setting, or (report opened as a file) what this browser remembers.
            function toggle(id,cls,action){
              var b=document.getElementById(id),key='chestcounter-'+cls,v=document.body.getAttribute('data-'+cls)==='1';
              if(!tok){var saved=get(key);if(saved!==null)v=saved==='1'}
              function set(x){document.body.classList.toggle(cls,x);if(b)b.checked=x;pagers.forEach(function(p){p()})}
              set(v);
              if(b)b.addEventListener('change',function(){set(b.checked);put(key,b.checked?'1':'0');post(action,b.checked)});
            }
            toggle('showteam','showteam','showTeamInReport');
            toggle('showsquad','showsquad','showSquadInReport');
            // Folding sections: the title opens or closes the whole section.
            [].forEach.call(document.querySelectorAll('section.fold'),function(s){
              var key='chestcounter-fold-'+s.getAttribute('data-fold'),btn=s.querySelector('.foldbtn'),open=s.getAttribute('data-open')==='1';
              var saved=get(key);if(saved!==null)open=saved==='1';
              function set(o){s.classList.toggle('closed',!o);btn.setAttribute('aria-expanded',o?'true':'false')}
              set(open);
              btn.addEventListener('click',function(){var o=s.classList.contains('closed');set(o);put(key,o?'1':'0')});
            });
            var boxes=document.querySelectorAll('input[data-prize]');
            function refresh(){
              var n=0;[].forEach.call(boxes,function(c){c.parentNode.classList.toggle('got',c.checked);if(c.checked)n++});
              var cc=document.getElementById('colcount');if(cc)cc.textContent=n+'/'+boxes.length;
              [].forEach.call(document.querySelectorAll('[data-items]'),function(card){
                var got=card.getAttribute('data-items').split(' ').some(function(id){var c=document.querySelector('input[data-prize="'+id+'"]');return c&&c.checked});
                card.classList.toggle('got',got);
              });
            }
            refresh();
            })();</script>
            """;
        sb.Append(L.T(
            "<footer>Compte les coffres que tu as ouverts ou détruits toi-même, d'après tes replays (« rediffusions » dans Fortnite), analysés après chaque partie. Les caisses de munitions et de nourriture ne sont pas comptées. ★★ = coffre doré de la chasse au trésor, détecté de façon certaine. ★ = trésor reçu grâce à un de tes coffres (moment exact), nommé quand l'objet est identifiable. L'icône équipe marque un trésor trouvé par un coéquipier (estimation : déduit des trésors que tu reçois sans avoir ouvert de coffre, car la chasse au trésor les partage avec toute l'équipe) ; il n'entre pas dans tes totaux. Le total de l'équipe ajoute aux tiens les coffres ouverts ou détruits par tes coéquipiers, comptés de la même façon dans tes replays ; un coffre ouvert à plus de 150 m de toi peut manquer, car le replay ne l'enregistre pas.</footer>",
            "<footer>Counts the chests you opened or destroyed yourself, from your replays, analyzed after each match. Ammo boxes and food boxes are not counted. ★★ = golden treasure hunt chest, detected for sure. ★ = treasure received from one of your own chests (exact moment), named when the item can be identified. The team icon marks a treasure found by a teammate (estimate: deduced from the treasures you receive without opening a chest, as the treasure hunt shares them with the whole team); it is not in your totals. The squad total adds the chests opened or destroyed by your teammates, counted the same way in your replays; a chest opened more than 150 m away from you can be missing, as the replay does not record it.</footer>",
            "<footer>Cuenta los cofres que has abierto o destruido tú mismo, a partir de tus repeticiones, analizadas después de cada partida. Las cajas de munición y de comida no se cuentan. ★★ = cofre dorado de la búsqueda del tesoro, detectado con seguridad. ★ = tesoro recibido de uno de tus cofres (momento exacto), con nombre cuando el objeto se puede identificar. El icono de equipo marca un tesoro encontrado por un compañero (estimación: deducido de los tesoros que recibes sin abrir un cofre, ya que la búsqueda del tesoro los comparte con todo el equipo); no cuenta en tus totales. El total del equipo añade a los tuyos los cofres abiertos o destruidos por tus compañeros, contados igual en tus repeticiones; un cofre abierto a más de 150 m de ti puede faltar, porque la repetición no lo graba.</footer>"));
        sb.Append(script).Append("</main></body></html>");
        return sb.ToString();
    }

    /// <summary>
    /// Start of a section the player can fold: its title is a button (with a chevron), <paramref name="right"/> sits at the
    /// right of the title. The page remembers the state in the browser. Ends with <see cref="FoldEnd"/>.
    /// </summary>
    private static string FoldStart(string key, string title, bool open, string info = "", string right = "") =>
        $"""<section class="fold{(open ? "" : " closed")}" data-fold="{key}" data-open="{(open ? 1 : 0)}"><div class="head"><h2 class="pirate"><button type="button" class="foldbtn" aria-expanded="{(open ? "true" : "false")}"><span class="chev" aria-hidden="true"></span>{title}</button>{info}</h2>{right}</div><div class="foldbody">""";

    private const string FoldEnd = "</div></section>";

    /// <summary>A small pencil (edit), drawn with the current text color.</summary>
    public const string PencilIcon = """<svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M4 20h4L19 9l-4-4L4 16z"/><path d="M14 6l4 4"/></svg>""";

    /// <summary>A small on/off switch (same look as the settings of the counter's window).</summary>
    private static string Switch(string id, string label) =>
        $"""<label class="tsw"><input type="checkbox" id="{id}"><span class="trk" aria-hidden="true"></span><span>{label}</span></label>""";

    /// <summary>Folded table controls: the page shows the first rows, "show more" adds some, "show all" / "fold" (script).</summary>
    private static string Pager(string tableId) =>
        $"""<div class="pager" data-for="{tableId}" data-step="10" hidden><span class="count" data-fmt="{H(L.T("{0} sur {1}", "{0} of {1}", "{0} de {1}"))}"></span><button type="button" class="more">{L.T("Afficher 20 de plus", "Show 20 more", "Mostrar 20 más")}</button><button type="button" class="all">{L.T("Tout afficher", "Show all", "Mostrar todo")}</button><button type="button" class="less" hidden>{L.T("Réduire", "Show less", "Reducir")}</button></div>""";

    /// <summary>A small percentage with enough decimals to be meaningful (0.0064 %, 2.8 %, 63 %).</summary>
    private static string Percent(double p)
    {
        var v = p * 100;
        if (v >= 99.95)
        {
            // Would round to 100: at least once over many chests is never certain.
            var most = 99.9.ToString("F1", Fr);
            return L.Code == "en" ? $">{most}%" : $"> {most} %";
        }
        var decimals = v >= 10 ? 1 : v <= 0 ? 0 : Math.Clamp((int)Math.Ceiling(-Math.Log10(v)) + 1, 1, 8);
        var text = v.ToString("F" + decimals, Fr);
        return L.Code == "en" ? text + "%" : text + " %";
    }

    private static string Info(string text) =>
        $"""<span class="info" tabindex="0" role="note" aria-label="{H(text)}">i<span class="infotip">{H(text)}</span></span>""";

    /// <summary>
    /// "Your chances": for each kind of treasure, the chance it already dropped at least once (1 − (1 − p)^n), with a
    /// word for it ("Very likely"). The kinds that change every week (gold outfit, vehicle and decal, pickaxe or back
    /// bling, jam track, loading screen) count the chests of the week shown only: a chest only gives that week's item.
    /// Pirate Shop Access and gizmos count every chest of the hunt. The weekly ones over the whole season and the way
    /// it is computed are in two folded parts at the bottom.
    /// </summary>
    /// <summary>Lifetime stats from the Epic account (all platforms and inputs), when signed in.</summary>
    private static string EpicSection()
    {
        if (!OperatingSystem.IsWindows() || EpicAccount.Data is not { } data)
        {
            return "";
        }
        var rows = EpicAccount.Stats(data).Where(r => !r.Mode.Contains(':')).ToList();
        if (rows.Count == 0)
        {
            return "";
        }
        var sb = new StringBuilder();
        sb.Append(FoldStart("epic", L.T("Compte Epic · depuis toujours", "Epic account · all time", "Cuenta de Epic · desde siempre"), open: true,
            Info(L.T("Statistiques Battle Royale de ton compte Epic (toutes plateformes), y compris les parties sans replay.", "Battle Royale stats of your Epic account (all platforms), matches without a replay included.", "Estadísticas de Battle Royale de tu cuenta de Epic (todas las plataformas), incluidas las partidas sin repetición."))));
        sb.Append($"<div class=\"card\"><table><tr><th>{L.T("Mode", "Mode", "Modo")}</th><th class=\"r\">{L.T("Parties", "Matches", "Partidas")}</th><th class=\"r\">{L.T("Victoires", "Wins", "Victorias")}</th><th class=\"r\">{L.T("% vict.", "Win %", "% vict.")}</th><th class=\"r\">{L.T("Élim.", "Elims", "Elim.")}</th><th class=\"r\">K/D</th><th class=\"r\">{L.T("Heures", "Hours", "Horas")}</th></tr>");
        foreach (var r in rows)
        {
            var bold = r.Mode == "TOTAL" ? " style=\"font-weight:600;color:var(--accent)\"" : "";
            sb.Append($"<tr{bold}><td>{H(r.Mode)}</td><td class=\"n num\">{r.Matches.ToString("N0", Fr)}</td><td class=\"n num\">{r.Wins.ToString("N0", Fr)}</td><td class=\"n num\">{H(r.WinRate)}</td><td class=\"n num\">{r.Kills.ToString("N0", Fr)}</td><td class=\"n num\">{H(r.KD)}</td><td class=\"n num\">{(r.Minutes / 60.0).ToString("N0", Fr)}</td></tr>");
        }
        sb.Append("</table></div>").Append(FoldEnd);
        return sb.ToString();
    }

    /// <summary>The analyzed matches per mode: wins, placement, kills, damage, chests.</summary>
    private static string ModesSection(IReadOnlyList<MatchRecord> matches)
    {
        if (matches.Count == 0)
        {
            return "";
        }
        var sb = new StringBuilder();
        sb.Append(FoldStart("modes", L.T("Par mode", "By mode", "Por modo"), open: true));
        sb.Append($"<div class=\"card\"><table><tr><th>{L.T("Mode", "Mode", "Modo")}</th><th class=\"r\">{L.T("Parties", "Matches", "Partidas")}</th><th class=\"r\">{L.T("Victoires", "Wins", "Victorias")}</th><th class=\"r\">Top 10</th><th class=\"r\">{L.T("Place moy.", "Avg place", "Puesto medio")}</th><th class=\"r\">{L.T("Élim. / partie", "Elims / match", "Elim. / partida")}</th><th class=\"r\">{L.T("Dégâts / partie", "Damage / match", "Daño / partida")}</th><th class=\"r\">{L.T("Coffres / partie", "Chests / match", "Cofres / partida")}</th></tr>");
        foreach (var g in matches.GroupBy(m => Pretty.Playlist(m.Playlist)).OrderByDescending(g => g.Count()))
        {
            var x = MatchStats.Summarize(g);
            sb.Append($"<tr><td>{H(g.Key)}</td><td class=\"n num\">{x.Matches.ToString("N0", Fr)}</td><td class=\"n num\">{x.Wins.ToString("N0", Fr)}</td><td class=\"n num\">{H(x.Top10Rate)}</td><td class=\"n num\">{H(x.AvgPlacementText)}</td><td class=\"n num\">{H(x.KillsPerMatch)}</td><td class=\"n num\">{H(x.AvgDamageText)}</td><td class=\"n num\">{H(x.ChestsPerMatch)}</td></tr>");
        }
        sb.Append("</table></div>").Append(FoldEnd);
        return sb.ToString();
    }

    private static string ChancesSection(int chests, IReadOnlyList<MatchRecord> matches, IReadOnlyList<ManualEntry> manualEntries)
    {
        var now = DateTime.UtcNow;
        if (now >= TreasureCatalog.Weeks[^1].To.AddDays(14))
        {
            return ""; // the hunt is over: odds and prizes no longer apply
        }

        var sb = new StringBuilder();
        static DateTime Start(MatchRecord m) => DateTime.SpecifyKind(m.MatchStartUtc ?? m.AnalyzedAtUtc, DateTimeKind.Utc).ToUniversalTime();
        // When each counted chest was opened (UTC): a chest of a Treasure Hour counts with the boosted odds.
        var chestTimes = matches.SelectMany(m => m.Chests.Select(c => Start(m).AddSeconds(c.TimeSeconds))).ToList();
        // Chests opened before the treasure hunt could not give a treasure.
        var huntStart = TreasureCatalog.HuntStart;
        var huntEnd = TreasureCatalog.Weeks[^1].To;
        var huntTimes = chestTimes.Where(t => t >= huntStart).OrderBy(t => t).ToList();
        // Added by hand (the pencil): no date, so normal odds, and in a week only when the player ticked "Opened this
        // week" (they are in the season too). A negative adjustment removes chests from the season (its boosts are then
        // unknown).
        var manual = chests - chestTimes.Count;
        var seasonChests = Math.Max(0, huntTimes.Count + manual);
        var week = TreasureCatalog.ShownWeek(now);
        var weekTimes = week == null ? new List<DateTime>() : huntTimes.Where(t => t >= week.From && t < week.To).ToList();
        var weekManual = week == null ? 0 : Math.Max(0, manualEntries.Where(e => e.WeekFromUtc == week.From).Sum(e => e.Chests));
        var weekChests = weekTimes.Count + weekManual;
        var boosted = huntTimes.Count(t => TreasureCatalog.Boost(TreasureCatalog.GoldOutfit, t) > 1);

        Chance Season(TreasureOdds odds) => manual >= 0
            ? Chance.Of(odds, huntTimes, manual)
            : Chance.Of(odds, Array.Empty<DateTime>(), seasonChests);
        Chance Week(TreasureOdds odds) => Chance.Of(odds, weekTimes, weekManual);
        Chance Shown(TreasureOdds odds) => odds.Weekly ? Week(odds) : Season(odds);
        TreasureOdds Odds(string kind) => TreasureCatalog.Odds.First(o => o.Kind == kind);
        TreasurePrize? PrizeOf(string kind) => week?.Prizes.FirstOrDefault(p => p.Kind == kind);

        // Pace: chests per day this week (from its second day), else over the hunt. Used to project the chance at the
        // end of the week (or of the hunt) if the player keeps going like this, with the normal odds.
        static double PerDay(int count, DateTime from, DateTime to) => (to - from).TotalDays is var d && d >= 1 ? count / d : 0;
        var seasonPace = huntTimes.Count > 0 ? PerDay(huntTimes.Count, huntTimes[0], now) : 0;
        var weekPace = week != null ? PerDay(weekChests, week.From, now) : 0;
        if (weekPace <= 0)
        {
            weekPace = seasonPace;
        }
        string Projection(TreasureOdds odds, Chance c, bool weekly)
        {
            var end = weekly ? week?.To ?? now : huntEnd;
            var pace = weekly ? weekPace : seasonPace;
            // Only while it is still in doubt: a projection of ">99.9%" says nothing.
            if (pace <= 0 || now >= end || c.P >= 0.75)
            {
                return "";
            }
            var log = c.LogNever + pace * (end - now).TotalDays * double.LogP1(-1.0 / odds.OneIn);
            var p = PercentHtml(new Chance(-double.ExpM1(log), log, 0, 0));
            return weekly
                ? L.T($"≈ {p} d'ici la fin de la semaine à ton rythme", $"≈ {p} by the end of the week at your pace", $"≈ {p} al final de la semana a tu ritmo")
                : L.T($"≈ {p} d'ici la fin de la chasse à ton rythme", $"≈ {p} by the end of the hunt at your pace", $"≈ {p} al final de la búsqueda a tu ritmo");
        }

        // Gizmos: treasures that did not come from a golden chest (the others are counted by the collection).
        var gizmoTimes = matches.SelectMany(m => m.TreasureFinds.Where(f => !f.FromGoldenChest).Select(f => Start(m).AddSeconds(f.ObtainedAtSeconds ?? 0)))
            .Where(t => t >= huntStart).OrderBy(t => t).ToList();

        string Tag(bool weekly) => weekly
            ? $"<span class=\"tag weekly\">{L.T("Hebdo", "Weekly", "Semanal")}</span>"
            : $"<span class=\"tag season\">{L.T("Saison", "Season", "Temporada")}</span>";
        string Chip(double p)
        {
            var (cls, text) = Verdict(p);
            return $"<span class=\"vchip {cls}\">{text}</span>";
        }
        string Bar(double p) => $"<span class=\"cbar\"><i class=\"{Verdict(p).Cls}\" style=\"width:{Math.Clamp(p * 100, 0.8, 100).ToString("F1", CultureInfo.InvariantCulture)}%\"></i></span>";
        string Img(TreasurePrize? prize, string cls) => TreasureImages.Src(prize?.Image) is { } src ? $"<img class=\"{cls}\" src=\"{H(src)}\" alt=\"\" onerror=\"this.style.visibility='hidden'\">" : "";
        string OneIn(int n) => L.T($"1 sur {n.ToString("N0", Fr)}", $"1 in {n.ToString("N0", Fr)}", $"1 entre {n.ToString("N0", Fr)}");
        string N(int n) => n.ToString("N0", Fr);
        string GotBadge() => $"<span class=\"gotbadge\">✓ {L.T("Obtenu", "Got it", "Conseguido")}</span>";

        var info = L.T(
            "La chance que chaque trésor soit déjà tombé pour toi au moins une fois. Détails : « Comment c'est calculé ? » en bas.",
            "The chance each treasure has already dropped for you at least once. Details: \"How is this calculated?\" at the bottom.",
            "La probabilidad de que cada tesoro ya te haya tocado al menos una vez. Detalles: «¿Cómo se calcula?» abajo.");
        sb.Append(FoldStart("chances", L.T("Tes chances", "Your chances", "Tus probabilidades"), open: true, Info(info)));
        sb.Append(TreasureHourBanner());
        sb.Append($"<p class=\"ch-intro\">{L.T(
            "Chaque coffre est un nouveau tirage : tes chances ne s'accumulent pas. Ces chiffres montrent la chance qu'un trésor soit <b>déjà tombé pour toi au moins une fois</b>, avec les coffres que tu as ouverts.",
            "Every chest is a fresh roll: your odds don't build up. These numbers show the chance a treasure has <b>already dropped for you at least once</b>, from the chests you've opened.",
            "Cada cofre es una tirada nueva: tus probabilidades no se acumulan. Estas cifras muestran la probabilidad de que un tesoro <b>ya te haya tocado al menos una vez</b>, con los cofres que has abierto.")}</p>");

        // ---- The two rarest: the gold outfit of the week and Pirate Shop Access (all season).
        string Card(string kind)
        {
            var odds = Odds(kind);
            var weekly = odds.Weekly;
            var c = Shown(odds);
            var prize = weekly ? PrizeOf(kind) : TreasureCatalog.PirateShopAccess;
            var items = prize?.Id ?? "";
            var name = weekly ? prize?.Name ?? TreasureCatalog.KindName(kind) : L.T("Toute la saison", "Lasts all season", "Toda la temporada");
            var about = c.P > 0 && c.P < 0.5 ? " · " + L.T($"environ 1 sur {N((int)Math.Round(1 / c.P))}", $"about 1 in {N((int)Math.Round(1 / c.P))}", $"aproximadamente 1 entre {N((int)Math.Round(1 / c.P))}") : "";
            var scope = weekly
                ? L.T("chance qu'il soit tombé pour toi cette semaine", "chance it dropped for you this week", "probabilidad de que te haya tocado esta semana")
                : L.T("chance qu'il soit tombé pour toi cette saison", "chance it dropped for you this season", "probabilidad de que te haya tocado esta temporada");
            var line = L.T($"Chaque coffre : {OneIn(odds.OneIn)} · {N(c.Chests)} coffres {(weekly ? "cette semaine" : "cette saison")}",
                $"Each chest: {OneIn(odds.OneIn)} · {N(c.Chests)} chests {(weekly ? "this week" : "this season")}",
                $"Cada cofre: {OneIn(odds.OneIn)} · {N(c.Chests)} cofres {(weekly ? "esta semana" : "esta temporada")}");
            var extra = new StringBuilder();
            if (weekly)
            {
                var s = Season(odds);
                extra.Append($"<div class=\"cfoot\">{L.T($"Toutes les semaines réunies : <b>{PercentHtml(s)}</b> sur {N(s.Chests)} coffres", $"All weeks combined: <b>{PercentHtml(s)}</b> over {N(s.Chests)} chests", $"Todas las semanas juntas: <b>{PercentHtml(s)}</b> en {N(s.Chests)} cofres")}</div>");
                if (week != null && now < week.To)
                {
                    extra.Append($"<div class=\"cfoot\">{L.T("Cette tenue part dans", "This outfit leaves in", "Este traje se va en")} <b>{Countdown(week.To)}</b></div>");
                }
            }
            else
            {
                var half = TreasureCatalog.ChestsFor(odds.OneIn, 0.5);
                var progress = Math.Clamp(c.Chests * 100.0 / half, 0.8, 100).ToString("F1", CultureInfo.InvariantCulture);
                extra.Append($"<div class=\"halfway\"><div class=\"hrow\"><span>{L.T("Vers une chance sur deux", "Halfway to a 50/50 chance", "Hacia una probabilidad del 50 %")}</span><span><b>{N(c.Chests)}</b> / {N((int)Math.Min(int.MaxValue, half))} {L.T("coffres", "chests", "cofres")}</span></div><span class=\"cbar wide\"><i class=\"gold\" style=\"width:{progress}%\"></i></span></div>");
            }
            var projection = Projection(odds, c, weekly);
            if (projection.Length > 0)
            {
                extra.Append($"<div class=\"cfoot\">{projection}</div>");
            }
            return $"""
                <div class="hcard" data-items="{items}" data-kind="{kind}" data-p="{c.P.ToString("R", CultureInfo.InvariantCulture)}">
                <div class="htop">{Img(prize, "cthumb")}<div class="hname"><div class="k">{H(TreasureCatalog.KindName(kind))}<span class="boost" hidden></span></div><div class="ciname">{H(name)}</div></div>{Tag(weekly)}{GotBadge()}</div>
                <div class="hval"><span class="big num">{PercentHtml(c)}</span>{Chip(c.P)}</div>
                <div class="hexp">{scope}{about}</div>
                <div class="s sline">{line}</div>{extra}</div>
                """;
        }
        sb.Append("<div class=\"chances2\">");
        sb.Append(Card(TreasureCatalog.GoldOutfit));
        sb.Append(Card(TreasureCatalog.PirateShop));
        sb.Append("</div>");

        // ---- Tables: the weekly treasures (this week's items), then the season ones. Same columns, same widths.
        var head = $"<tr><th>{L.T("Trésor", "Treasure", "Tesoro")}</th><th class=\"r\">{L.T("Chaque coffre", "Each chest", "Cada cofre")}</th><th>{L.T("Chance de l'avoir eu", "Chance you've had it", "Probabilidad de tenerlo")}</th><th class=\"r\">{L.T("Attendu", "Expected", "Esperado")}</th></tr>";
        string Row(TreasureOdds odds, string note)
        {
            var c = Shown(odds);
            var prize = odds.Weekly ? PrizeOf(odds.Kind) : null;
            var name = prize != null ? $"<span class=\"iname\">{H(prize.Name)}</span>" : "";
            var projection = odds.Weekly ? Projection(odds, c, weekly: true) : "";
            var below = string.Join("", new[] { note, projection }.Where(x => x.Length > 0).Select(x => $"<span class=\"iname\">{x}</span>"));
            return $"<tr data-kind=\"{odds.Kind}\"{(prize != null ? $" data-items=\"{prize.Id}\"" : "")}><td><div class=\"tflex\">{Img(prize, "thumb")}<div><div class=\"tk\">{H(TreasureCatalog.KindName(odds.Kind))}<span class=\"boost\" hidden></span>{GotBadge()}</div>{name}<span class=\"iname mo\">{OneIn(odds.OneIn)} {L.T("par coffre", "each chest", "por cofre")}</span>{below}</div></div></td>"
                + $"<td class=\"n pc\">{OneIn(odds.OneIn)}</td><td class=\"tchance\"><div class=\"tcv\">{Chip(c.P)}<b>{PercentHtml(c)}</b></div>{Bar(c.P)}</td><td class=\"n\">{Expected(c.Average)}</td></tr>";
        }

        if (week != null)
        {
            var leaves = now < week.To
                ? $" · {L.T("part", "leaves", "se va")} {TimeEl(week.To, "dayTime")}</div></div><div class=\"cdbox\"><span class=\"cdl\">{L.T("Part dans", "Leaves in", "Se va en")}</span>{Countdown(week.To)}</div></div>"
                : "</div></div></div>";
            sb.Append($"<div class=\"cgroup\"><div class=\"ghead\">{Tag(true)}<div><div class=\"gt\">{L.T("Semaine", "Week", "Semana")} {week.Number} · {H(week.Title)}</div><div class=\"gs\"><b>{L.Count(weekChests, "coffre", "coffres", "chest", "chests", "cofre", "cofres")}</b> {L.T("cette semaine", "this week", "esta semana")}{(weekManual > 0 ? L.T($", dont {N(weekManual)} ajoutés à la main", $", including {N(weekManual)} added by hand", $", incluidos {N(weekManual)} añadidos a mano") : "")}{leaves}");
            sb.Append($"<div class=\"card tcard\"><table class=\"ctable\">{head}");
            foreach (var odds in TreasureCatalog.Odds.Where(o => o.Weekly && o.Kind != TreasureCatalog.GoldOutfit))
            {
                sb.Append(Row(odds, ""));
            }
            sb.Append("</table></div></div>");
        }

        var byHand = manual > 0 ? L.T($", dont {N(manual)} ajoutés à la main", $", including {N(manual)} added by hand", $", incluidos {N(manual)} añadidos a mano") : "";
        sb.Append($"<div class=\"cgroup\"><div class=\"ghead\">{Tag(false)}<div><div class=\"gt\">{L.T("Toute la saison", "All season", "Toda la temporada")}</div><div class=\"gs\"><b>{L.Count(seasonChests, "coffre", "coffres", "chest", "chests", "cofre", "cofres")}</b> {L.T("depuis le début de la chasse", "since the hunt started", "desde el inicio de la búsqueda")} ({TimeEl(huntStart, "date")}){byHand}</div></div></div>");
        sb.Append($"<div class=\"card tcard\"><table class=\"ctable\">{head}");
        foreach (var odds in TreasureCatalog.Odds.Where(o => !o.Weekly && o.Kind != TreasureCatalog.PirateShop))
        {
            var note = "";
            if (odds.Kind == TreasureCatalog.Gizmo && gizmoTimes.Count > 0)
            {
                var since = huntTimes.Count(t => t > gizmoTimes[^1]);
                note = L.T($"Tu en as trouvé {N(gizmoTimes.Count)} · le dernier il y a {L.Count(since, "coffre", "coffres", "", "", "", "")}",
                    $"You found {N(gizmoTimes.Count)} · last one {L.Count(since, "", "", "chest", "chests", "", "")} ago",
                    $"Has encontrado {N(gizmoTimes.Count)} · el último hace {L.Count(since, "", "", "", "", "cofre", "cofres")}");
            }
            sb.Append(Row(odds, note));
        }
        sb.Append("</table></div></div>");

        // ---- Folded: the weekly treasures over the whole season, and how it is computed.
        sb.Append($"<details class=\"cmore\"><summary>{L.T("Trésors hebdo, toutes les semaines réunies", "Weekly treasures, all weeks combined", "Tesoros semanales, todas las semanas juntas")}</summary>");
        sb.Append($"<p>{L.T($"La chance d'en avoir eu au moins un, de <i>n'importe quelle</i> semaine, sur les {N(seasonChests)} coffres de la chasse.", $"The chance you've had at least one of them from <i>any</i> week, over all {N(seasonChests)} chests of the hunt.", $"La probabilidad de haber conseguido al menos uno, de <i>cualquier</i> semana, en los {N(seasonChests)} cofres de la búsqueda.")}</p>");
        sb.Append($"<div class=\"card tcard\"><table class=\"ctable mini\"><tr><th>{L.T("Trésor", "Treasure", "Tesoro")}</th><th class=\"r\">{L.T("Chance", "Chance", "Probabilidad")}</th><th class=\"r\">{L.T("Attendu", "Expected", "Esperado")}</th></tr>");
        foreach (var odds in TreasureCatalog.Odds.Where(o => o.Weekly))
        {
            var s = Season(odds);
            sb.Append($"<tr><td>{H(TreasureCatalog.KindName(odds.Kind))}</td><td class=\"n\">{PercentHtml(s)}</td><td class=\"n\">{Expected(s.Average)}</td></tr>");
        }
        sb.Append("</table></div></details>");

        var hours = NextRegularHours(now);
        var hoursText = hours.Count > 0
            ? L.T($" Elles ont lieu chaque {string.Join(" et ", hours.Select(h => TimeEl(h, "weekdayTime")))}, pendant 2 heures.", $" They run every {string.Join(" and ", hours.Select(h => TimeEl(h, "weekdayTime")))}, for 2 hours.", $" Son cada {string.Join(" y ", hours.Select(h => TimeEl(h, "weekdayTime")))}, durante 2 horas.")
            : "";
        string Link(string url, string text) => $"<a href=\"{H(url)}\" target=\"_blank\" rel=\"noopener\">{H(text)}</a>";
        sb.Append($"<details class=\"cmore\"><summary>{L.T("Comment c'est calculé ?", "How is this calculated?", "¿Cómo se calcula?")}</summary><ul>");
        sb.Append(L.T(
            $"<li><b>Chaque coffre est indépendant.</b> Un écran de chargement, c'est {OneIn(Odds(TreasureCatalog.LoadingScreen).OneIn)} à chaque coffre, peu importe combien tu en as ouvert.</li><li><b>Chance de l'avoir eu</b> = 1 − (1 − 1/X)<sup>coffres</sup>. Elle se rapproche de 100 % sans jamais l'atteindre.</li><li><b>Hebdo</b> : ces trésors changent chaque dimanche, et un coffre ne donne que celui de sa semaine : seuls les coffres de cette semaine comptent.</li><li><b>Saison</b> : l'accès à la boutique pirate et les gizmos comptent tous les coffres depuis le début de la chasse, y compris ceux ajoutés à la main. Ceux-là n'ont pas de date : ils ne comptent pour une semaine que si tu as coché « Ouverts cette semaine ».</li><li><b>Heures du trésor</b> : certaines chances sont multipliées (par exemple ×10 pour les véhicules).{hoursText} Les coffres ouverts pendant l'une d'elles comptent avec les chances augmentées ({N(boosted)} des tiens).</li><li><b>Attendu</b> : combien tu en aurais en moyenne avec ces coffres. <b>À ton rythme</b> : la même chance à la fin de la semaine (ou de la chasse) si tu continues à ouvrir autant de coffres par jour.</li><li>Cotes publiées par {Link(TreasureCatalog.OddsSource, "Epic Games (Fortnite Status)")}, horaires des heures du trésor d'après {Link(TreasureCatalog.HoursSource, "fortnite.bot")}.</li>",
            $"<li><b>Each chest is independent.</b> A loading screen is {OneIn(Odds(TreasureCatalog.LoadingScreen).OneIn)} on every chest, no matter how many you've opened.</li><li><b>Chance you've had it</b> = 1 − (1 − 1/X)<sup>chests</sup>. It gets close to 100% but never reaches it.</li><li><b>Weekly</b> treasures change every Sunday, and a chest only gives that week's one, so only this week's chests count.</li><li><b>Season</b> treasures (Pirate Shop Access, gizmos) count every chest since the hunt started, including the ones you added by hand. Those have no date, so they count toward a week only if you ticked \"Opened this week\".</li><li><b>Treasure Hours</b> boost some odds (for example ×10 for vehicles).{hoursText} Chests opened during one count with the boosted odds ({N(boosted)} of yours).</li><li><b>Expected</b> is how many you'd get on average with these chests. <b>At your pace</b> is the same chance at the end of the week (or of the hunt) if you keep opening as many chests a day.</li><li>Odds published by {Link(TreasureCatalog.OddsSource, "Epic Games (Fortnite Status)")}, Treasure Hour times from {Link(TreasureCatalog.HoursSource, "fortnite.bot")}.</li>",
            $"<li><b>Cada cofre es independiente.</b> Una pantalla de carga es {OneIn(Odds(TreasureCatalog.LoadingScreen).OneIn)} en cada cofre, sin importar cuántos hayas abierto.</li><li><b>Probabilidad de tenerlo</b> = 1 − (1 − 1/X)<sup>cofres</sup>. Se acerca al 100 % sin llegar nunca.</li><li><b>Semanal</b>: estos tesoros cambian cada domingo y un cofre solo da el de su semana, así que solo cuentan los cofres de esta semana.</li><li><b>Temporada</b>: el acceso a la tienda pirata y los artilugios cuentan todos los cofres desde el inicio de la búsqueda, incluidos los añadidos a mano. Esos no tienen fecha: solo cuentan para una semana si marcaste «Abiertos esta semana».</li><li><b>Horas del tesoro</b>: algunas probabilidades se multiplican (por ejemplo ×10 para los vehículos).{hoursText} Los cofres abiertos durante una de ellas cuentan con la probabilidad aumentada ({N(boosted)} de los tuyos).</li><li><b>Esperado</b>: cuántos tendrías de media con esos cofres. <b>A tu ritmo</b>: la misma probabilidad al final de la semana (o de la búsqueda) si sigues abriendo tantos cofres al día.</li><li>Probabilidades publicadas por {Link(TreasureCatalog.OddsSource, "Epic Games (Fortnite Status)")}, horarios de las horas del tesoro según {Link(TreasureCatalog.HoursSource, "fortnite.bot")}.</li>"));
        sb.Append($"</ul></details>{FoldEnd}");
        return sb.ToString();
    }

    /// <summary>A word for a chance, and its color class.</summary>
    private static (string Cls, string Text) Verdict(double p) => (p * 100) switch
    {
        >= 99.95 => ("certain", L.T("Presque sûr", "Almost certain", "Casi seguro")),
        >= 95 => ("vlikely", L.T("Très probable", "Very likely", "Muy probable")),
        >= 75 => ("likely", L.T("Probable", "Likely", "Probable")),
        >= 25 => ("possible", L.T("Peut-être", "Maybe", "Quizás")),
        >= 5 => ("unlikely", L.T("Peu probable", "Unlikely", "Poco probable")),
        _ => ("vunlikely", L.T("Très peu probable", "Very unlikely", "Muy poco probable")),
    };

    /// <summary>How many on average ("≈ 4.3", "&lt; 0.1").</summary>
    private static string Expected(double average) => average < 0.05
        ? "&lt; " + 0.1.ToString("N1", Fr)
        : "≈ " + average.ToString(average >= 10 ? "N0" : "N1", Fr);

    /// <summary>A live countdown to a moment (drawn by the Treasure Hour script, every second).</summary>
    private static string Countdown(DateTime utc) => $"<span class=\"cd\" data-end=\"{utc.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}\"></span>";

    /// <summary>
    /// A moment in local time or UTC (Settings), with both in the tooltip. Formats: "dayTime" (Sunday, Nov 15, 6:00
    /// AM), "weekdayTime" (Saturday 11:00 AM), "date" (November 1).
    /// </summary>
    private static string TimeEl(DateTime utc, string format)
    {
        var shown = L.Shown(utc);
        var text = format switch
        {
            "date" => shown.ToString(L.T("d MMMM", "MMMM d", "d 'de' MMMM"), Fr),
            "weekdayTime" => shown.ToString("dddd ", Fr) + L.Clock(shown),
            _ => shown.ToString(L.T("dddd d MMM, ", "dddd, MMM d, ", "dddd d MMM, "), Fr) + L.Clock(shown),
        };
        var local = utc.ToLocalTime();
        var tip = $"{local.ToString("D", Fr)}, {local.ToString(L.TimePattern(), Fr)} ({L.LocalOffset}) · {utc.ToString("D", Fr)}, {utc.ToString(L.TimePattern(), Fr)} UTC";
        return $"<time datetime=\"{utc.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture)}\" title=\"{H(tip)}\">{H(text)}</time>";
    }

    /// <summary>The next regular Treasure Hours (Saturday 19:00 and Sunday 02:00 UTC), for the explanation.</summary>
    private static List<DateTime> NextRegularHours(DateTime now)
    {
        DateTime Next(DayOfWeek day, int hour)
        {
            var t = new DateTime(now.Year, now.Month, now.Day, hour, 0, 0, DateTimeKind.Utc).AddDays(((int)day - (int)now.DayOfWeek + 7) % 7);
            return t.AddHours(2) > now ? t : t.AddDays(7);
        }
        return new[] { Next(DayOfWeek.Saturday, 19), Next(DayOfWeek.Sunday, 2) }.OrderBy(t => t).ToList();
    }

    /// <summary>A chance of "at least once" and what it is based on.</summary>
    /// <param name="P">Chance of at least once (rounds to exactly 1 for the common kinds).</param>
    /// <param name="LogNever">Natural log of the chance of never: still exact when <paramref name="P"/> is 1.</param>
    /// <param name="Average">How many of them on average.</param>
    /// <param name="Chests">Chests counted.</param>
    private readonly record struct Chance(double P, double LogNever, double Average, int Chests)
    {
        public static Chance Of(TreasureOdds odds, IReadOnlyCollection<DateTime> times, long plain)
        {
            var log = TreasureCatalog.LogNever(odds, times, plain);
            return new Chance(-double.ExpM1(log), log, TreasureCatalog.Expected(odds, times, plain), (int)(times.Count + Math.Max(0, plain)));
        }
    }

    /// <summary>
    /// A chance as text, never "100%": at least once over many chests is never certain. Close to it, the exact gap is
    /// in the tooltip ("never had it: 1 chance in 5.2 × 10²⁷").
    /// </summary>
    private static string PercentHtml(Chance c)
    {
        var text = Percent(c.P);
        if (c.P * 100 < 99.95)
        {
            return text;
        }
        var x = OneInHuge(c.LogNever);
        var never = L.T($"ne pas l'avoir eu : 1 chance sur {x}", $"never had it: 1 chance in {x}", $"no haberlo conseguido: 1 entre {x}");
        return $"<span title=\"{H(never)}\">{text}</span>";
    }

    /// <summary>"1 in X" from the log of a tiny chance: X written plainly up to a billion, then as a power of ten.</summary>
    private static string OneInHuge(double logChance)
    {
        var log10 = -logChance / Math.Log(10);
        if (log10 < 9)
        {
            return Math.Round(Math.Exp(-logChance)).ToString("N0", Fr);
        }
        var exp = (int)Math.Floor(log10);
        var mantissa = Math.Pow(10, log10 - exp);
        const string sup = "⁰¹²³⁴⁵⁶⁷⁸⁹";
        return $"{mantissa.ToString("F1", Fr)} × 10{string.Concat(exp.ToString(CultureInfo.InvariantCulture).Select(d => sup[d - '0']))}";
    }

    /// <summary>The collection of this season's treasures, to tick what was obtained (folded by default).</summary>
    private static string CollectionSection(IReadOnlyList<MatchRecord> matches)
    {
        var now = DateTime.UtcNow;
        if (now >= TreasureCatalog.Weeks[^1].To.AddDays(14))
        {
            return ""; // the hunt is over
        }
        var sb = new StringBuilder();
        // What is owned: prizes seen in the replays (golden chests, a teammate's too: the hunt gives it to the whole team)
        // and prizes found in the Epic locker. Nothing is ticked by hand any more.
        var seen = matches.SelectMany(m => m.TreasureFinds.Concat(m.TeamTreasureFinds)).Select(f => f.PrizeId).Where(id => id != null).Select(id => id!.ToLowerInvariant()).ToHashSet();
        var locker = OperatingSystem.IsWindows() ? EpicAccount.OwnedPrizes(TreasureCatalog.Weeks.SelectMany(w => w.Prizes)) : new HashSet<string>();
        string Prize(TreasurePrize p)
        {
            var inReplays = p.PrizeIds.Any(id => seen.Contains(id.ToLowerInvariant()));
            var inLocker = locker.Contains(p.Id);
            var got = inReplays || inLocker;
            var src = TreasureImages.Src(p.Image);
            var img = src != null ? $"""<img src="{H(src)}" alt="" loading="lazy" onerror="this.remove()">""" : "";
            var how = inLocker ? L.T("dans ton casier Epic", "in your Epic locker", "en tu taquilla de Epic") : inReplays ? L.T("vu dans tes replays", "seen in your replays", "visto en tus repeticiones") : "";
            return $"""<div class="prize{(got ? " got" : "")}"><input type="checkbox" data-prize="{p.Id}"{(got ? " checked" : "")} disabled>{(got ? "<span class=\"gotmark\">✓</span>" : "")}<span class="pimg">{img}</span><span class="pn">{H(p.Name)}</span><span class="pk">{H(TreasureCatalog.KindName(p.Kind))}{(how.Length > 0 ? " · " + how : "")}</span></div>""";
        }
        var signedIn = OperatingSystem.IsWindows() && EpicAccount.Data != null;
        var collectionInfo = L.T(
            "Rempli tout seul : avec ton casier Epic (même nom et même type d'objet) et les coffres dorés reconnus dans tes replays. L'accès à la boutique Pirate n'est pas un objet du casier : il n'apparaît que s'il est vu dans un replay. Images : fortnite.bot.",
            "Filled in by itself: from your Epic locker (same name and kind of item) and the golden chests recognized in your replays. Pirate Shop Access is not a locker item: it only shows when seen in a replay. Images: fortnite.bot.",
            "Se rellena sola: con tu taquilla de Epic (mismo nombre y tipo de objeto) y los cofres dorados reconocidos en tus repeticiones. El acceso a la tienda Pirata no es un objeto de la taquilla: solo aparece si se ve en una repetición. Imágenes: fortnite.bot.");
        if (!signedIn)
        {
            collectionInfo += " " + L.T("Connecte-toi à Epic (onglet Compte) pour la remplir avec ton casier.", "Sign in to Epic (Account tab) to fill it from your locker.", "Inicia sesión en Epic (pestaña Cuenta) para rellenarla con tu taquilla.");
        }
        sb.Append(FoldStart("collection", $"{L.T("Ta collection", "Your collection", "Tu colección")} · <span class=\"num\" id=\"colcount\"></span>", open: false, Info(collectionInfo)));
        sb.Append("<div class=\"card collection\">");
        sb.Append($"<div class=\"wk\"><div class=\"wt\">{L.T("Toute la saison", "All season", "Toda la temporada")}</div><div class=\"prizes\">{Prize(TreasureCatalog.PirateShopAccess)}</div></div>");
        foreach (var week in TreasureCatalog.Weeks)
        {
            var current = now >= week.From && now < week.To;
            var dates = $"{ShortWhen(week.From)} – {ShortWhen(week.To)}";
            sb.Append($"<div class=\"wk{(current ? " now" : "")}\"><div class=\"wt\">{L.T("Semaine", "Week", "Semana")} {week.Number} · {H(week.Title)} <span class=\"items\">{dates}{(current ? " · " + L.T("cette semaine", "this week", "esta semana") : "")}</span></div>");
            sb.Append(week.Prizes.Count > 0
                ? $"<div class=\"prizes\">{string.Concat(week.Prizes.Select(Prize))}</div></div>"
                : $"<div class=\"items\" style=\"padding:4px 0 8px\">{L.T("Récompenses pas encore dévoilées : elles arriveront avec une mise à jour du compteur.", "Prizes not revealed yet: they will come with an update of the counter.", "Recompensas aún no reveladas: llegarán con una actualización del contador.")}</div></div>");
        }
        sb.Append("</div>").Append(FoldEnd);
        return sb.ToString();
    }

    /// <summary>
    /// The Treasure Hour banner: drawn by the page script from the schedule, against the clock of the PC, so that it
    /// switches by itself when an hour starts or ends ("live now, until 22:00 · ends in 37m" / "next one: Saturday
    /// 20:00 · in 2d 4h"). Times follow the settings (local time or UTC, 12 or 24 h).
    /// The same script draws every countdown of the page (<see cref="Countdown"/>) each second.
    /// </summary>
    private static string TreasureHourBanner()
    {
        var hours = TreasureCatalog.TreasureHours.Select(h => new
        {
            s = new DateTimeOffset(h.StartUtc).ToUnixTimeMilliseconds(),
            e = new DateTimeOffset(h.EndUtc).ToUnixTimeMilliseconds(),
            b = h.Boosts.ToDictionary(kv => kv.Key, kv => kv.Value),
            c = h.Confirmed,
        });
        var odds = TreasureCatalog.Odds.ToDictionary(o => o.Kind, o => o.OneIn);
        var names = new Dictionary<string, string>
        {
            [TreasureCatalog.GoldOutfit] = TreasureCatalog.KindName(TreasureCatalog.GoldOutfit),
            [TreasureCatalog.PirateShop] = TreasureCatalog.KindName(TreasureCatalog.PirateShop),
            [TreasureCatalog.VehicleDecal] = TreasureCatalog.KindName(TreasureCatalog.VehicleDecal),
        };
        var text = new
        {
            live = L.T("Heure du trésor en cours, jusqu'à {0}", "Treasure Hour live now, until {0}", "Hora del tesoro en curso, hasta las {0}"),
            next = L.T("Prochaine heure du trésor : {0}", "Next Treasure Hour: {0}", "Próxima hora del tesoro: {0}"),
            endsIn = L.T("se termine dans {0}", "ends in {0}", "termina en {0}"),
            startsIn = L.T("dans {0}", "in {0}", "en {0}"),
            expected = L.T("prévue, pas encore annoncée", "expected, not announced yet", "prevista, aún no anunciada"),
            perChest = L.T("1 sur {0} par coffre", "1 in {0} per chest", "1 entre {0} por cofre"),
            oneIn = L.T("1 sur {0}", "1 in {0}", "1 entre {0}"),
            boostPerChest = L.T("1 sur {0} à chaque coffre pendant l'heure du trésor", "1 in {0} per chest during the Treasure Hour", "1 entre {0} por cofre durante la hora del tesoro"),
            boostHalf = L.T(" · une chance sur deux dans {0} coffres de plus à ce rythme", " · one chance in two within {0} more chests at this rate", " · una de cada dos en {0} cofres más a este ritmo"),
            units = L.T("j h min s", "d h m s", "d h m s").Split(' '),
            now = L.T("maintenant", "now", "ahora"),
            locale = L.Culture.Name,
        };
        // Time settings: UTC or local time, 12- or 24-hour clock (the same as the times written by the counter).
        var clock = new { utc = L.UtcTimes, h12 = !L.Clock24 };
        // The default encoder escapes < > & as <...: the JSON cannot close the <script> element it sits in.
        var json = System.Text.Json.JsonSerializer.Serialize(new { hours, odds, names, text, clock });
        return $$"""
            <div class="thour" id="thour" hidden></div>
            <script type="application/json" id="thdata">{{json}}</script>
            <script>(function(){
            var d=JSON.parse(document.getElementById('thdata').textContent),el=document.getElementById('thour');
            function f(n){return Math.round(n).toLocaleString(d.text.locale)}
            function t(ms,withDay){var o=withDay?{weekday:'long',day:'numeric',month:'short',hour:'numeric',minute:'2-digit'}:{hour:'numeric',minute:'2-digit'};o.hour12=d.clock.h12;if(d.clock.utc)o.timeZone='UTC';return new Date(ms).toLocaleString(d.text.locale,o)+(d.clock.utc?' UTC':'')}
            function iso(ms){return new Date(ms).toISOString().slice(0,19)+'Z'}
            function cd(ms){return '<span class="cd" data-end="'+iso(ms)+'"></span>'}
            function boosts(h,live){return Object.keys(h.b).map(function(k){return '<span><b>'+d.names[k]+' ×'+h.b[k].toLocaleString(d.text.locale)+'</b>'+(live?' · '+d.text.perChest.replace('{0}',f(d.odds[k]/h.b[k])):'')+'</span>'}).join('')}
            function draw(){
              var now=Date.now(),cur=d.hours.filter(function(h){return now>=h.s&&now<h.e})[0],nxt=d.hours.filter(function(h){return h.s>now}).sort(function(a,b){return a.s-b.s})[0];
              if(cur){el.className='thour live';el.innerHTML='<div class="tt">⚡ '+d.text.live.replace('{0}',t(cur.e,false))+' · '+d.text.endsIn.replace('{0}',cd(cur.e))+'</div><div class="tb">'+boosts(cur,true)+'</div>';el.hidden=false}
              else if(nxt){el.className='thour';el.innerHTML='<div class="tt">'+d.text.next.replace('{0}',t(nxt.s,true))+' · '+d.text.startsIn.replace('{0}',cd(nxt.s))+(nxt.c?'':' <span class="exp">('+d.text.expected+')</span>')+'</div><div class="tb">'+boosts(nxt,false)+'</div>';el.hidden=false}
              else el.hidden=true;
              boost(cur);tick();
            }
            // During a Treasure Hour, the odds of the boosted kinds (cards and table) show the boosted rate, in orange.
            function boost(cur){
              [].forEach.call(document.querySelectorAll('[data-kind]'),function(x){
                var k=x.getAttribute('data-kind'),b=cur&&cur.b[k]>1?cur.b[k]:0,badge=x.querySelector('.boost'),line=x.querySelector('.sline'),pc=x.querySelector('.pc');
                x.classList.toggle('boosted',!!b);
                if(badge){badge.hidden=!b;badge.textContent=b?'⚡ ×'+b.toLocaleString(d.text.locale):''}
                if(line){
                  if(!line.hasAttribute('data-n'))line.setAttribute('data-n',line.innerHTML);
                  if(b){var one=d.odds[k]/b,p=+x.getAttribute('data-p'),s=d.text.boostPerChest.replace('{0}',f(one));
                    if(p<0.5)s+=d.text.boostHalf.replace('{0}',f(Math.ceil(Math.log(0.5/(1-p))/Math.log(1-1/one))));
                    line.innerHTML='<span class="boostv">'+s+'</span>'}
                  else line.innerHTML=line.getAttribute('data-n');
                }
                if(pc){if(!pc.hasAttribute('data-n'))pc.setAttribute('data-n',pc.innerHTML);pc.innerHTML=b?'<span class="boostv">'+d.text.oneIn.replace('{0}',f(d.odds[k]/b))+'</span>':pc.getAttribute('data-n')}
              });
            }
            // Every countdown of the page: "3d 11h 58m", with seconds in the last hour, orange in the last day.
            function tick(){
              var now=Date.now(),u=d.text.units;
              [].forEach.call(document.querySelectorAll('.cd'),function(x){
                var ms=new Date(x.getAttribute('data-end'))-now;
                if(ms<=0){x.textContent=d.text.now;x.classList.add('done');return}
                var s=Math.floor(ms/1000),dd=Math.floor(s/86400),h=Math.floor(s%86400/3600),m=Math.floor(s%3600/60),sec=s%60;
                x.textContent=dd>0?dd+u[0]+' '+h+u[1]+' '+m+u[2]:h>0?h+u[1]+' '+m+u[2]+' '+sec+u[3]:m+u[2]+' '+sec+u[3];
                x.classList.toggle('soon',ms<86400000);
              });
            }
            draw();document.addEventListener('DOMContentLoaded',draw);setInterval(draw,30000);setInterval(tick,1000);
            })();</script>
            """;
    }

    /// <summary>
    /// The rarest treasures (Pirate Shop Access, gold outfits) that other players got in the player's matches, from
    /// golden chests opened within reach of the replay. Experimental: always shown, even empty, to be checked with
    /// friends before it is trusted.
    /// </summary>
    private static string SeenSection(IReadOnlyList<MatchRecord> matches)
    {
        var rows = matches
            .SelectMany(m => m.SeenPrizes.Select(s => (m, s, Prize: TreasureCatalog.Find(s.PrizeId))))
            .Where(r => r.Prize?.Kind is TreasureCatalog.PirateShop or TreasureCatalog.GoldOutfit)
            .OrderByDescending(r => When(r.m)).ThenBy(r => r.s.TimeSeconds)
            .ToList();
        var info = L.T(
            "Un coffre doré est envoyé à tous les joueurs à environ 150 m, avec le trésor et le joueur qui l'a ouvert : ces trésors-là ont été vus dans tes replays. Un joueur plus loin de toi n'y figure pas.",
            "A golden chest is sent to every player within about 150 m, with the treasure and the player who opened it: these were seen in your replays. A player farther away from you is not in them.",
            "Un cofre dorado se envía a todos los jugadores a unos 150 m, con el tesoro y el jugador que lo abrió: estos se vieron en tus repeticiones. Un jugador más lejos de ti no aparece.");
        var sb = new StringBuilder();
        sb.Append(FoldStart("seen", $"{L.T("Ils l'ont eu dans tes parties", "They got it in your matches", "Lo consiguieron en tus partidas")} · <span class=\"num\">{rows.Count}</span>", open: false,
            $" <span class=\"exper\">{L.T("expérimental", "experimental", "experimental")}</span>{Info(info)}"));
        sb.Append("<div class=\"card\">");
        if (rows.Count == 0)
        {
            sb.Append($"<p class=\"items\" style=\"margin:8px 0\">{L.T("Aucun pour l'instant : quand un joueur à moins de 150 m de toi décroche l'accès à la boutique pirate ou une tenue en or, il apparaît ici avec la partie, son pseudo et le moment.", "None yet: when a player within 150 m of you gets the Pirate Shop Access or a gold outfit, they show up here with the match, their name and the moment.", "Ninguno por ahora: cuando un jugador a menos de 150 m de ti consigue el acceso a la tienda pirata o un traje de oro, aparece aquí con la partida, su nombre y el momento.")}</p></div>{FoldEnd}");
            return sb.ToString();
        }
        sb.Append($"<table id=\"t-seen\"><tr><th>{L.T("Partie", "Match", "Partida")}</th><th>{L.T("Joueur", "Player", "Jugador")}</th><th>{L.T("Trésor", "Treasure", "Tesoro")}</th><th>{L.T("Moment", "Time", "Momento")}</th></tr>");
        foreach (var (m, s, prize) in rows)
        {
            var name = prize!.Kind == TreasureCatalog.PirateShop ? TreasureCatalog.KindName(prize.Kind) : prize.Name;
            sb.Append($"<tr class=\"r\"><td class=\"nw num items\">{L.DateTimeShort(When(m))}</td><td><b>{H(s.Player)}</b></td><td><span class=\"gold\">★★</span> {H(name)}</td><td class=\"nw num\">{Pretty.Clock(s.TimeSeconds)}</td></tr>");
        }
        sb.Append($"</table>{Pager("t-seen")}</div>{FoldEnd}");
        return sb.ToString();
    }

    /// <summary>"i" next to the squad total: why the teammates' count is lower than what they really opened.</summary>
    private static string SquadInfo
    {
        get
        {
            var text = L.T(
                "Pourquoi ce nombre est bas ? Il vient de TES replays. Fortnite n'y enregistre que ce qui se passe à environ 150 m autour de toi : un coffre ouvert par un coéquipier plus loin n'y figure pas et ne peut pas être compté. Rien non plus après ton départ de la partie (élimination puis retour au salon). Plus vous lootez ensemble, plus le nombre est juste. Les coffres comptés, eux, le sont avec les mêmes règles que les tiens.",
                "Why is this number low? It comes from YOUR replays. Fortnite only records what happens within about 150 m of you: a chest a teammate opens farther away is not in it and cannot be counted. Nothing either once you leave the match (eliminated, back to the lobby). The more you loot together, the closer it gets. The chests that are counted are counted with the same rules as yours.",
                "¿Por qué este número es bajo? Viene de TUS repeticiones. Fortnite solo graba lo que pasa a unos 150 m a tu alrededor: un cofre que un compañero abre más lejos no está en ella y no se puede contar. Tampoco nada después de que salgas de la partida (eliminado, de vuelta al lobby). Cuanto más looteéis juntos, más exacto es. Los cofres contados se cuentan con las mismas reglas que los tuyos.");
            return $"""<span class="info" tabindex="0" role="note" aria-label="{H(text)}">i<span class="infotip">{H(text)}</span></span>""";
        }
    }

    private static string VehicleWarning => L.T(
        "Fortnite n'a pas enregistré ce que ton véhicule a détruit dans ce replay : du butin de coffres a été vu le long de ton trajet sans aucun coffre enregistré, ces coffres ne peuvent pas être comptés. Désactive « Enregistrer des rediffusions de haute qualité » dans Fortnite (Paramètres → Jeu → Rediffusions) puis relance le jeu.",
        "Fortnite did not record what your vehicle destroyed in this replay: chest loot was seen along your drive with no chest recorded, so those chests cannot be counted. Turn off \"Record High Quality Replays\" in Fortnite (Settings → Game → Replays), then restart the game.",
        "Fortnite no grabó lo que destruyó tu vehículo en esta repetición: se vio botín de cofres a lo largo de tu trayecto sin ningún cofre grabado, así que esos cofres no se pueden contar. Desactiva «Grabar repeticiones de alta calidad» en Fortnite (Ajustes → Juego → Repeticiones) y reinicia el juego.");

    /// <summary>Messages of content.json for these days (text only, escaped).</summary>
    private static string NoticeCards() => string.Concat(TreasureCatalog.ActiveNotices(DateTime.UtcNow)
        .Select(n => $"<div class=\"notice{(n.Level == "warning" ? " warning" : "")}\">{(n.TitleIn(L.Code) is { Length: > 0 } title ? $"<b>{H(title)}</b><br>" : "")}{H(n.TextIn(L.Code))}{(n.Link != null ? $"<br><a href=\"{H(n.Link)}\" target=\"_blank\" rel=\"noopener\">{L.T("En savoir plus", "More info", "Más información")}</a>" : "")}</div>"));

    private static string LoadingCard(ScanProgress? p)
    {
        if (p == null)
        {
            return "";
        }
        var pct = p.Total > 0 ? (int)Math.Round(100.0 * p.Done / p.Total) : 0;
        return L.T(
            $"""<div class="loading"><div class="spin"></div><div class="grow"><b>Analyse de tes replays… {p.Done} / {p.Total}</b><div class="pbar"><i style="width:{pct}%"></i></div><span class="muted">Cette page se met à jour toute seule : les chiffres ci-dessous augmentent au fil de la lecture des replays.</span></div></div>""",
            $"""<div class="loading"><div class="spin"></div><div class="grow"><b>Analyzing your replays… {p.Done} / {p.Total}</b><div class="pbar"><i style="width:{pct}%"></i></div><span class="muted">This page updates by itself: the numbers below grow as the replays are read.</span></div></div>""",
            $"""<div class="loading"><div class="spin"></div><div class="grow"><b>Analizando tus repeticiones… {p.Done} / {p.Total}</b><div class="pbar"><i style="width:{pct}%"></i></div><span class="muted">Esta página se actualiza sola: las cifras de abajo aumentan a medida que se leen las repeticiones.</span></div></div>""");
    }

    private static string ProfilesLine(IReadOnlyList<Profile> profiles, string player)
    {
        var others = profiles.Where(p => p.Name != player).Select(p => H(p.Name)).ToList();
        if (others.Count == 0)
        {
            return "";
        }
        return $"""<div class="profiles muted">{L.T("Autres profils sur ce PC : ", "Other profiles on this PC: ", "Otros perfiles en este PC: ")}{string.Join(", ", others)} · {L.T("« Changer de profil » dans le compteur", "\"Switch profile\" in the counter", "«Cambiar de perfil» en el contador")}</div>""";
    }

    /// <summary>Style of the OBS page: the one chosen for the overlay ("hud" or "classic").</summary>
    public static volatile string OverlayStyle = "hud";

    /// <summary>What the overlay shows right now (also written by <see cref="WriteAll"/>, so that the OBS page never goes back).</summary>
    private static volatile OverlayView? LastOverlay;

    /// <summary>Rewrites only the OBS overlay page (called on every live update).</summary>
    public static void WriteOverlay(Store store, OverlayView view)
    {
        LastOverlay = view;
        try
        {
            WriteAtomic(store.OverlayPath, Overlay(view));
        }
        catch
        {
            // the page is a convenience: never stop the tracker for it
        }
    }

    public static string Overlay(OverlayView v) => OverlayStyle == "classic" ? OverlayClassic(v) : OverlayHud(v);

    /// <summary>
    /// The HUD overlay as a web page, for OBS ("Browser" source, local file): transparent, figures outlined like the
    /// game's, same rows as the overlay on screen. Refreshes itself every 5 seconds.
    /// </summary>
    private static string OverlayHud(OverlayView v)
    {
        // The last finished match stays during the next one; the red dot next to the total says a match is in progress.
        var state = Row(L.T("Dernière", "Last", "Última"), v.Last ?? "—", "");
        var squad = v.Squad != null ? Row(OverlayModel.SquadLabel, v.Squad, "") : "";
        static string Row(string label, string value, string cls) =>
            $"<div class=\"r {cls}\"><span>{H(label)}</span>{(value.Length > 0 ? $"<span class=\"sep\">•</span><b>{H(value)}</b>" : "")}</div>";
        return $$"""
            <!doctype html><html lang="{{L.Code}}"><head><meta charset="utf-8"><meta http-equiv="refresh" content="5">
            <title>{{L.T("Overlay coffres", "Chest overlay", "Overlay de cofres")}}</title>
            <style>
            html,body{margin:0;background:transparent}
            body{padding:10px 14px;display:inline-block;color:#fff;font-family:"IBM Plex Mono","Cascadia Mono",Consolas,monospace;font-weight:600;text-align:right;
                 -webkit-text-stroke:4px #0b0f18;paint-order:stroke fill}
            .total{display:flex;align-items:center;justify-content:flex-end;gap:10px;font-size:40px;font-weight:700;line-height:1.05}
            .total.live{color:#a68bff}
            .total svg{width:30px;height:26px;flex:none}
            .r{display:flex;align-items:center;justify-content:flex-end;gap:8px;font-size:18px;line-height:1.35}
            .r span{color:#c8c8c8}
            .r.gold b{color:#a68bff}
            .sep{opacity:.9}
            .dot{width:10px;height:10px;border-radius:50%;background:#ff3b30;box-shadow:0 0 0 2px #0b0f18;flex:none}
            .t{color:#a68bff}.t .star{color:#7d55ff}
            </style></head><body>
            <div class="total{{(v.CurrentLive && v.Current is { } c && c != "…" ? " live" : "")}}">{{(v.CurrentLive ? "<i class=\"dot\"></i>" : "")}}<svg viewBox="0 0 30 26" aria-hidden="true"><rect x="2" y="10" width="26" height="14" rx="2" fill="#7d55ff" stroke="#0b0f18" stroke-width="2.5"/><path d="M2 12 Q15 -2 28 12" fill="#623ee6" stroke="#0b0f18" stroke-width="2.5"/><rect x="12.5" y="12" width="5" height="6" rx="1" fill="#0b0f18"/></svg>{{H(v.Total)}}</div>
            {{state}}
            {{Row(L.T("Session", "Session", "Sesión"), v.Session, "")}}
            {{squad}}
            <div class="r t"><span class="star">★</span><b>{{H(v.Treasures)}}</b></div>
            </body></html>
            """;
    }

    /// <summary>The classic overlay (dark card) as a web page, for OBS.</summary>
    private static string OverlayClassic(OverlayView v)
    {
        var rows = new StringBuilder();
        foreach (var r in v.Rows)
        {
            rows.Append($"<div class=\"row{(r.Treasure ? " t" : "")}\"><span>{H(r.Label)}</span><b>{H(r.Value)}{(r.Detail != null ? $" <small>{H(r.Detail)}</small>" : "")}</b></div>");
        }
        return $$"""
            <!doctype html><html lang="{{L.Code}}"><head><meta charset="utf-8"><meta http-equiv="refresh" content="5">
            <title>{{L.T("Overlay coffres", "Chest overlay", "Overlay de cofres")}}</title>
            <style>
            html,body{margin:0;background:transparent;color:#fff;font:400 14px/1.35 "IBM Plex Mono","Cascadia Mono",Consolas,monospace}
            .box{display:inline-block;min-width:260px;padding:10px 14px;border-radius:4px;background:rgba(15,15,15,.92);border:1px solid #282828}
            .head{display:flex;justify-content:space-between;gap:16px;font-size:12px;color:#999;padding-bottom:6px;border-bottom:1px solid #282828}
            .live{color:#2fd28a}
            .counters{display:flex;align-items:stretch;gap:18px;margin:4px 0 10px}
            .counters>div{display:flex;flex-direction:column}
            .sep{width:1px;background:#282828;margin:8px 0 2px}
            .big{font-size:36px;font-weight:600;line-height:1.1;font-variant-numeric:tabular-nums}
            .total{color:#8c69ff}.cur{color:#2fd28a}.none{color:#505050}
            .cap{font-size:11px;color:#888}
            .rows{border-top:1px solid #282828;padding-top:8px;display:grid;grid-template-columns:auto 1fr;gap:4px 12px}
            .row{display:contents}.row span{font-size:12px;color:#999}.row b{font-weight:400}
            .row.t b{color:#a68bff}.row small{font-size:11px;color:#888}
            </style></head><body><div class="box">
            <div class="head"><span>{{L.T("COFFRES", "CHESTS", "COFRES")}}</span>{{(v.Badge != null ? $"<span class=\"live\">{H(v.Badge)}</span>" : "")}}</div>
            <div class="counters">
              <div><span class="big total">{{H(v.Total)}}</span><span class="cap">total</span></div>
              <div class="sep"></div>
              <div><span class="big {{(v.Current == null ? "none" : v.CurrentLive ? "cur" : "")}}">{{H(v.Current ?? "—")}}</span><span class="cap">{{H(v.CurrentCaption)}}</span></div>
            </div>
            <div class="rows">{{rows}}</div>
            </div></body></html>
            """;
    }

    public static string Csv(TrackerData data)
    {
        var sb = new StringBuilder();
        sb.AppendLine(L.T("date;mode;joueur;place;joueurs;coffres;coffres_coequipiers;caisses_munitions;tresors;fichier", "date;mode;player;placement;players;chests;teammate_chests;ammo_boxes;treasures;file", "fecha;modo;jugador;puesto;jugadores;cofres;cofres_companeros;cajas_municion;tesoros;archivo"));

        foreach (var m in Profiles.Counted(data).OrderBy(When))
        {
            var t = string.Join(" + ", m.TreasureFinds.Select(f => f.DisplayName));
            sb.AppendLine(string.Join(";",
                When(m).ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                Pretty.Playlist(m.Playlist), m.PlayerName, m.Placement, m.TotalPlayers,
                m.ChestsOpened, m.TeamChestsOpened, m.AmmoBoxesOpened, t, m.ReplayFile));
        }
        return sb.ToString();
    }
}
