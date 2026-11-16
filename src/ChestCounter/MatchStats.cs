namespace ChestCounter;

/// <summary>General figures of a set of matches (wins, kills, damage...), from the replays.</summary>
public sealed record MatchSummary(
    int Matches, int Wins, int Top10, double? AvgPlacement, int Kills, int Assists, int Knocks, double? AvgDamage,
    double? Accuracy, int Revives, int Materials, double Hours, int Chests, int Treasures)
{
    public string WinRate => Matches > 0 ? (100.0 * Wins / Matches).ToString("0.0", L.Culture) + " %" : "—";
    public string Top10Rate => Matches > 0 ? (100.0 * Top10 / Matches).ToString("0", L.Culture) + " %" : "—";

    /// <summary>Kills per match not won (Fortnite's K/D).</summary>
    public string KD => Matches - Wins > 0 ? ((double)Kills / (Matches - Wins)).ToString("0.00", L.Culture) : Kills > 0 ? "∞" : "—";
    public string KillsPerMatch => Matches > 0 ? ((double)Kills / Matches).ToString("0.0", L.Culture) : "—";
    public string AvgPlacementText => AvgPlacement is double p ? "#" + p.ToString("0.0", L.Culture) : "—";
    public string AvgDamageText => AvgDamage is double d ? Math.Round(d).ToString("N0", L.Culture) : "—";
    public string AccuracyText => Accuracy is double a ? a.ToString("0.0", L.Culture) + " %" : "—";
    public string HoursText => Hours.ToString(Hours < 10 ? "0.0" : "N0", L.Culture) + " h";
    public string ChestsPerMatch => Matches > 0 ? ((double)Chests / Matches).ToString("0.0", L.Culture) : "—";
}

public static class MatchStats
{
    public static MatchSummary Summarize(IEnumerable<MatchRecord> source)
    {
        var m = source.ToList();
        var placed = m.Where(x => x.Placement is > 0).ToList();
        var dmg = m.Where(x => x.DamageDealt != null).ToList();
        var acc = m.Where(x => x.Accuracy is > 0).ToList();
        return new MatchSummary(
            m.Count,
            m.Count(x => x.Won),
            m.Count(x => x.Top10),
            placed.Count > 0 ? placed.Average(x => x.Placement!.Value) : null,
            m.Sum(x => x.Eliminations ?? 0),
            m.Sum(x => x.Assists ?? 0),
            m.Sum(x => x.Knocks ?? 0),
            dmg.Count > 0 ? dmg.Average(x => x.DamageDealt!.Value) : null,
            acc.Count > 0 ? acc.Average(x => x.Accuracy!.Value) : null,
            m.Sum(x => x.Revives ?? 0),
            m.Sum(x => x.MaterialsGathered ?? 0),
            m.Sum(x => x.DurationSeconds ?? 0) / 3600.0,
            m.Sum(x => x.Counted),
            m.Sum(x => x.TreasureFinds.Count));
    }

    /// <summary>Main figure of the overlay: "chests", "kills" or "wins".</summary>
    public static readonly string[] OverlayMains = { "chests", "kills", "wins" };

    public static string MainLabel(string main) => main switch
    {
        "kills" => L.T("Éliminations", "Eliminations", "Eliminaciones"),
        "wins" => L.T("Victoires", "Wins", "Victorias"),
        _ => L.T("Coffres", "Chests", "Cofres"),
    };
}
