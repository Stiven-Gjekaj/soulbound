using MoonSharp.Interpreter;
using UnityEngine;

/// <summary>
/// What the player has done to each boss: how many times they have tried it, whether they
/// have ever beaten it, and their fastest clear.
///
/// Records live in AlMighty globals, so they are written to disk the moment they change and
/// survive a wiped save file. Boss scripts can read them with
/// <c>GetAlMightyGlobal("boss_&lt;id&gt;_cleared")</c> and friends.
///
/// Timing is unconditional. Every fight is timed whether or not anything displays it,
/// because making that a setting would produce two sets of times that cannot be compared,
/// and a way to lose a personal best by forgetting a toggle.
///
/// A fight is timed in battle steps rather than wall-clock seconds. On a machine holding
/// 60fps the two are the same number. Where they differ, the step count is the honest one:
/// it measures what the fight actually did rather than how long the player sat in front of
/// it, so a stutter, a slow machine, or a boss slowing time down cannot inflate a record
/// for the same amount of work.
/// </summary>
public static class BossRecords {
    private static string currentId = "";
    private static int  clockStart;
    private static bool clockRunning;
    private static bool tookAHit;

    /// <summary>Seconds the fight in progress has been running, or 0 if none is.</summary>
    public static float Elapsed {
        get { return clockRunning ? (BattleTick.Ticks - clockStart) * BattleTick.Step : 0f; }
    }

    /// <summary>
    /// The player picked a boss. Counts the attempt, but does not start the clock: loading
    /// the encounter and the Battle scene happens between here and the first frame of the
    /// fight, and that time is not the player's.
    /// </summary>
    public static void FightStarting(string id) {
        currentId    = id;
        clockRunning = false;
        tookAHit     = false;
        SetNumber(id, "attempts", Attempts(id) + 1);
    }

    /// <summary>The fight is actually under way. Starts the clock.</summary>
    public static void ClockStart() {
        clockStart   = BattleTick.Ticks;
        clockRunning = true;
    }

    /// <summary>
    /// The last enemy has left the fight. Marks the boss cleared and keeps the time if it
    /// beats the stored one.
    /// </summary>
    public static void FightWon() {
        if (currentId == "" || !clockRunning)
            return;

        float time = Elapsed;
        clockRunning = false;

        SetBool(currentId, "cleared", true);
        SetNumber(currentId, "clears", Clears(currentId) + 1);

        float best = BestTime(currentId);
        if (best < 0f || time < best)
            SetNumber(currentId, "best", time);

        // Only ever set, never cleared: a clean clear stays on the record even if the
        // player takes a hit off the boss the next time they beat it.
        if (!tookAHit && !NoHit(currentId))
            SetBool(currentId, "nohit", true);
    }

    /// <summary>The fight ended without a win: the player died, fled or gave up.</summary>
    public static void FightEnded() {
        clockRunning = false;
        currentId    = "";
        tookAHit     = false;
    }

    /// <summary>
    /// The player died and the game over ran to its end. Called before EndBattle, which
    /// clears the boss this was counted against.
    ///
    /// A scripted revive does not reach here, so a boss that kills the player as a story
    /// beat and brings them back does not cost them a death.
    /// </summary>
    public static void Died() {
        if (currentId == "")
            return;
        SetNumber(currentId, "deaths", Deaths(currentId) + 1);
        LuaScriptBinder.SetPermanentGlobal(TotalDeathsKey, DynValue.NewNumber(TotalDeaths + 1));
    }

    /// <summary>The player took damage during the fight in progress.</summary>
    public static void PlayerHit() { tookAHit = true; }

    public static int Deaths(string id) {
        DynValue v = Get(id, "deaths");
        return v != null && v.Type == DataType.Number ? (int)v.Number : 0;
    }

    /// <summary>Whether the boss has ever been cleared without taking a hit.</summary>
    public static bool NoHit(string id) {
        DynValue v = Get(id, "nohit");
        return v != null && v.Type == DataType.Boolean && v.Boolean;
    }

    public const string TotalDeathsKey = "deaths_total";

    /// <summary>Deaths across every boss, which is the number the player quotes.</summary>
    public static int TotalDeaths {
        get {
            DynValue v = LuaScriptBinder.GetPermanentGlobal(TotalDeathsKey);
            return v != null && v.Type == DataType.Number ? (int)v.Number : 0;
        }
    }

    /// <summary>A record time as m:ss.d. Tenths matter when a fight lasts seconds.</summary>
    public static string FormatTime(float seconds) {
        if (seconds < 0)
            return "";
        int minutes = (int)(seconds / 60f);
        float rest  = seconds - minutes * 60f;
        return minutes + ":" + rest.ToString("00.0", System.Globalization.CultureInfo.InvariantCulture);
    }

    public static bool Cleared(string id) {
        DynValue v = Get(id, "cleared");
        return v != null && v.Type == DataType.Boolean && v.Boolean;
    }

    /// <summary>
    /// How many times the boss has been beaten, as opposed to whether it ever has.
    ///
    /// The boolean above came first and stays, because it is what the lock reads and what
    /// saves written before this counter existed still carry. A save from then reports zero
    /// clears against a boss it knows is cleared, which is wrong by one at worst and only
    /// until the player beats it again.
    /// </summary>
    public static int Clears(string id) {
        DynValue v = Get(id, "clears");
        return v != null && v.Type == DataType.Number ? (int)v.Number : 0;
    }

    public static int Attempts(string id) {
        DynValue v = Get(id, "attempts");
        return v != null && v.Type == DataType.Number ? (int)v.Number : 0;
    }

    /// <summary>Fastest clear in seconds, or -1 if the boss has never been beaten.</summary>
    public static float BestTime(string id) {
        DynValue v = Get(id, "best");
        return v != null && v.Type == DataType.Number ? (float)v.Number : -1f;
    }

    private static string Key(string id, string field) { return "boss_" + id + "_" + field; }

    private static DynValue Get(string id, string field) {
        return LuaScriptBinder.GetPermanentGlobal(Key(id, field));
    }

    private static void SetNumber(string id, string field, double value) {
        LuaScriptBinder.SetPermanentGlobal(Key(id, field), DynValue.NewNumber(value));
    }

    private static void SetBool(string id, string field, bool value) {
        LuaScriptBinder.SetPermanentGlobal(Key(id, field), DynValue.NewBoolean(value));
    }
}
