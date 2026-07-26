using MoonSharp.Interpreter;

/// <summary>
/// Who the player is: the name they chose for their character.
///
/// It lives in an AlMighty global next to the boss records rather than in save.gd, so one
/// file holds the whole player profile and wiping the save cannot clear the name while
/// leaving the records behind.
///
/// Boss scripts can read it with <c>GetAlMightyGlobal("player_name")</c>, though
/// <c>Player.name</c> is the normal way to reach it from Lua.
/// </summary>
public static class PlayerProfile {
    public const string NameKey = "player_name";

    /// <summary>The name the player chose, or "" if they have not chosen one yet.</summary>
    public static string Name {
        get {
            DynValue v = LuaScriptBinder.GetPermanentGlobal(NameKey);
            return v != null && v.Type == DataType.String ? v.String : "";
        }
        set { LuaScriptBinder.SetPermanentGlobal(NameKey, DynValue.NewString(value)); }
    }

    /// <summary>False on a first run, and after the player wipes their permanent globals.</summary>
    public static bool HasName { get { return Name != ""; } }

    /// <summary>Forgets the name, so the game asks for one again.</summary>
    public static void Clear() {
        LuaScriptBinder.SetPermanentGlobal(NameKey, DynValue.NewString(""));
    }
}
