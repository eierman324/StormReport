namespace ChestCounter;

/// <summary>Size and opacity choices of the overlay window (percent), offered in the settings.</summary>
public static class OverlayAppearance
{
    public static readonly int[] Sizes = { 70, 80, 90, 100, 115, 130, 150, 175 };

    /// <summary>Below 30 % the numbers can no longer be read.</summary>
    public static readonly int[] Opacities = { 100, 90, 80, 70, 60, 50, 40, 30 };

    public static int ClampSize(int percent) => Math.Clamp(percent, Sizes[0], Sizes[^1]);
    public static int ClampOpacity(int percent) => Math.Clamp(percent, Opacities[^1], 100);

    /// <summary>Next value of the list, back to the first one after the last (or for a value edited by hand).</summary>
    public static int Next(int[] values, int current) => values[(Array.IndexOf(values, current) + 1) % values.Length];
}

/// <summary>Builds what the overlays (window and OBS page) show from the saved matches and the match in progress.</summary>
public static class OverlayModel
{
    /// <summary>Caption of the right counter between two matches (also used to keep the overlay width stable).</summary>
    public static string LastMatchCaption => L.T("dernière partie", "last match", "última partida");

    /// <param name="live">Figures of the match in progress, when the replay could be read (null otherwise).</param>
    /// <param name="inProgress">A match is being recorded (even if its figures are not readable yet).</param>
    /// <param name="main">The big figure: "chests" (default), "kills" or "wins" (see <see cref="MatchStats.OverlayMains"/>).</param>
    public static OverlayView Build(TrackerData d, MatchRecord? live, bool inProgress, bool showSquad = true, string main = "chests")
    {
        if (main is "kills" or "wins")
        {
            return BuildGeneral(d, live, inProgress, main);
        }
        var liveChests = live?.Counted ?? 0;
        var liveTreasures = live?.TreasureFinds.Count ?? 0;

        var total = d.ShownTotal + liveChests;
        // Squad total: only once a teammate's chest has been counted (never for a solo player).
        var team = d.TeamChests + (live?.TeamChestsOpened ?? 0);
        var squad = showSquad && team > 0 ? L.N(total + team) : null;
        var session = d.Matches.Where(ReportWriter.IsThisSession).ToList();
        var rows = new List<OverlayRow>();

        // Session: chests, then small grey "n parties · moy. x" (finished matches of this session).
        string? sessionDetail = null;
        if (session.Count > 0)
        {
            var avg = (double)session.Sum(m => m.Counted) / session.Count;
            var avgText = Math.Round(avg, MidpointRounding.AwayFromZero).ToString("0", L.Culture);
            sessionDetail = $"{L.Count(session.Count, "partie", "parties", "match", "matches", "partida", "partidas")} · {L.T("moy.", "avg.", "media")} {avgText}";
        }
        rows.Add(new OverlayRow(L.T("Session", "Session", "Sesión"), L.Count(session.Sum(m => m.Counted) + liveChests, "coffre", "chest", "cofre"), Detail: sessionDetail));

        var last = d.Matches.OrderByDescending(m => m.MatchStartUtc ?? m.AnalyzedAtUtc).FirstOrDefault();
        if (last != null)
        {
            rows.Add(new OverlayRow(L.T("Dernière", "Last", "Última"), L.Count(last.Counted, "coffre", "chest", "cofre")));
        }

        var rates = Stats.Compute(d.Matches, live);
        var sessionRates = Stats.Compute(session, live);
        rows.Add(new OverlayRow(L.T("Coffres / h", "Chests / h", "Cofres / h"), rates.ChestsPerHourText,
            Detail: sessionRates.ChestsPerHour != null ? $"{L.T("session", "session", "sesión")} {sessionRates.ChestsPerHourText}" : null));

        var treasures = d.Matches.Sum(m => m.TreasureFinds.Count) + liveTreasures;
        if (squad != null)
        {
            rows.Add(new OverlayRow(SquadLabel, L.Count(total + team, "coffre", "chest", "cofre")));
        }
        rows.Add(new OverlayRow(L.T("Trésors", "Treasures", "Tesoros"), $"★ {L.N(treasures)}", Treasure: true));

        // Right counter: the match in progress when readable, "…" while it cannot be read yet, else the last match.
        if (live != null)
        {
            return new OverlayView(L.N(total), L.N(live.Counted), L.T("partie en cours", "current match", "partida actual"), true, L.T("● EN DIRECT", "● LIVE", "● EN DIRECTO"), rows, L.N(treasures), L.N(session.Sum(m => m.Counted) + liveChests), last != null ? L.N(last.Counted) : null, squad);
        }
        if (inProgress)
        {
            return new OverlayView(L.N(total), "…", L.T("en cours", "in progress", "en curso"), true, L.T("● PARTIE EN COURS", "● MATCH IN PROGRESS", "● PARTIDA EN CURSO"), rows, L.N(treasures), L.N(session.Sum(m => m.Counted) + liveChests), last != null ? L.N(last.Counted) : null, squad);
        }
        return new OverlayView(L.N(total), last != null ? L.N(last.Counted) : null, LastMatchCaption, false, null, rows, L.N(treasures), L.N(session.Sum(m => m.Counted) + liveChests), last != null ? L.N(last.Counted) : null, squad);
    }

    /// <summary>Kills or wins as the big figure; the rows give the rest of the session.</summary>
    private static OverlayView BuildGeneral(TrackerData d, MatchRecord? live, bool inProgress, string main)
    {
        var session = d.Matches.Where(ReportWriter.IsThisSession).ToList();
        var all = MatchStats.Summarize(d.Matches);
        var ses = MatchStats.Summarize(session);
        var last = d.Matches.OrderByDescending(m => m.MatchStartUtc ?? m.AnalyzedAtUtc).FirstOrDefault();
        var liveKills = live?.Eliminations ?? 0;
        var kills = main == "kills";
        var rows = new List<OverlayRow>
        {
            new(L.T("Session", "Session", "Sesión"), kills ? L.N(ses.Kills + liveKills) + " " + L.T("élim.", "elims", "elim.") : L.N(ses.Wins) + " " + L.T("victoires", "wins", "victorias"),
                Detail: L.Count(session.Count, "partie", "parties", "match", "matches", "partida", "partidas")),
        };
        if (last != null)
        {
            rows.Add(new OverlayRow(L.T("Dernière", "Last", "Última"), (last.Placement is > 0 ? "#" + last.Placement : "—") + " · " + L.N(last.Eliminations ?? 0) + " " + L.T("élim.", "elims", "elim.")));
        }
        rows.Add(kills ? new OverlayRow("K/D", all.KD, Detail: L.T($"{all.Wins} victoires", $"{all.Wins} wins", $"{all.Wins} victorias"))
                       : new OverlayRow(L.T("Élim.", "Elims", "Elim."), L.N(all.Kills), Detail: "K/D " + all.KD));
        rows.Add(new OverlayRow(L.T("Coffres", "Chests", "Cofres"), L.N(d.ShownTotal + (live?.Counted ?? 0))));
        var treasures = d.Matches.Sum(m => m.TreasureFinds.Count) + (live?.TreasureFinds.Count ?? 0);
        rows.Add(new OverlayRow(L.T("Trésors", "Treasures", "Tesoros"), $"★ {L.N(treasures)}", Treasure: true));

        var total = kills ? L.N(all.Kills + liveKills) : L.N(all.Wins);
        var sessionText = kills ? L.N(ses.Kills + liveKills) : L.N(ses.Wins);
        var lastText = last == null ? null : kills ? L.N(last.Eliminations ?? 0) : last.Placement is > 0 ? "#" + last.Placement : "—";
        if (live != null)
        {
            return new OverlayView(total, kills ? L.N(liveKills) : "…", L.T("partie en cours", "current match", "partida actual"), true, L.T("● EN DIRECT", "● LIVE", "● EN DIRECTO"), rows, L.N(treasures), sessionText, lastText, null, main);
        }
        if (inProgress)
        {
            return new OverlayView(total, "…", L.T("en cours", "in progress", "en curso"), true, L.T("● PARTIE EN COURS", "● MATCH IN PROGRESS", "● PARTIDA EN CURSO"), rows, L.N(treasures), sessionText, lastText, null, main);
        }
        return new OverlayView(total, lastText, LastMatchCaption, false, null, rows, L.N(treasures), sessionText, lastText, null, main);
    }

    /// <summary>Label of the squad total (the player + every teammate, duo, trio or squad).</summary>
    public static string SquadLabel => L.T("Équipe", "Squad", "Equipo");
}
