using System.Collections.Generic;

/// <summary>
/// What the player has opened up, as opposed to what they have done, which is BossRecords.
///
/// The rule is the one the design settles: the first boss in the registry is the tutorial and
/// is always available, and clearing it opens everything else. It is written against the first
/// entry rather than against a named boss so that it keeps working while the registry holds
/// placeholders, and applies to the tutorial boss automatically once she is the one listed
/// first. A boss rush with a free select screen would otherwise let a first-time player open
/// with boss three and meet none of the game.
/// </summary>
public static class BossProgress {
    /// <summary>
    /// How many slots a boss select screen holds. A screen is a batch, and the roster grows by
    /// screens rather than by getting longer, so seven is a layout constant and not a count of
    /// what happens to be in the registry today.
    /// </summary>
    public const int SlotsPerScreen = 7;

    /// <summary>Whether the player may fight the entry in this position.</summary>
    public static bool Unlocked(int index) {
        if (index <= 0)
            return true;

        List<BossEntry> entries = BossRegistry.Entries;
        return entries.Count > 0 && BossRecords.Cleared(entries[0].id);
    }

    /// <summary>Whether the tutorial has been cleared, which is what opens the rest.</summary>
    public static bool TutorialCleared {
        get {
            List<BossEntry> entries = BossRegistry.Entries;
            return entries.Count > 0 && BossRecords.Cleared(entries[0].id);
        }
    }
}
