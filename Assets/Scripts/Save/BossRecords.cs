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
/// </summary>
public static class BossRecords {
    private static string currentId = "";
    private static float clockStart;
    private static bool clockRunning;

    /// <summary>Seconds the fight in progress has been running, or 0 if none is.</summary>
    public static float Elapsed {
        get { return clockRunning ? Time.realtimeSinceStartup - clockStart : 0f; }
    }

    /// <summary>
    /// The player picked a boss. Counts the attempt, but does not start the clock: loading
    /// the encounter and the Battle scene happens between here and the first frame of the
    /// fight, and that time is not the player's.
    /// </summary>
    public static void FightStarting(string id) {
        currentId    = id;
        clockRunning = false;
        SetNumber(id, "attempts", Attempts(id) + 1);
    }

    /// <summary>The fight is actually under way. Starts the clock.</summary>
    public static void ClockStart() {
        clockStart   = Time.realtimeSinceStartup;
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

        float best = BestTime(currentId);
        if (best < 0f || time < best)
            SetNumber(currentId, "best", time);
    }

    /// <summary>The fight ended without a win: the player died, fled or gave up.</summary>
    public static void FightEnded() {
        clockRunning = false;
        currentId    = "";
    }

    public static bool Cleared(string id) {
        DynValue v = Get(id, "cleared");
        return v != null && v.Type == DataType.Boolean && v.Boolean;
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
