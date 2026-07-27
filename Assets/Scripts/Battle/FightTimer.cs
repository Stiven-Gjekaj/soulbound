using UnityEngine;

/// <summary>
/// Draws the running fight time in the corner of the battle screen.
///
/// Display only. The clock in BossRecords runs on every fight whether this is on or off,
/// so turning it off never costs a record and turning it on never creates a second set of
/// times. That is why this is a display setting and not a mode.
///
/// The text object is built at runtime from the same prefab Lua's CreateText uses, because
/// Battle.unity has no object for it and adding one needs the Unity editor. When the battle
/// screen is rebuilt for art in v0.7 this wants a proper scene object instead.
/// </summary>
public class FightTimer : MonoBehaviour {
    public const string SettingKey = "show_fight_timer";

    /// <summary>Off unless the player turned it on in the options screen.</summary>
    public static bool Enabled {
        get {
            MoonSharp.Interpreter.DynValue v = LuaScriptBinder.GetPermanentGlobal(SettingKey);
            return v != null && v.Type == MoonSharp.Interpreter.DataType.Boolean && v.Boolean;
        }
        set { LuaScriptBinder.SetPermanentGlobal(SettingKey, MoonSharp.Interpreter.DynValue.NewBoolean(value)); }
    }

    private LuaTextManager text;
    private string shown = "";

    private void Start() {
        GameObject go = Instantiate(Resources.Load<GameObject>("Prefabs/CstmTxtContainer"));
        text = go.GetComponentInChildren<LuaTextManager>();
        if (text == null) {
            Destroy(go);
            Destroy(this);
            return;
        }

        text.SetCaller(EnemyEncounter.script);
        text.SetFont(SpriteFontRegistry.Get(SpriteFontRegistry.UI_SMALLTEXT_NAME));
        text.progressmode = "NONE";
        text.HideBubble();
        text.layer = "Top";
        // Top right, clear of the arena and of the stats bar along the bottom.
        text.MoveToAbs(524, 452);
        Refresh();
    }

    private void Update() { Refresh(); }

    /// <summary>
    /// Rebuilding the text rebuilds a sprite per letter, so only do it when the string
    /// actually changes. At tenths that is ten times a second rather than sixty.
    /// </summary>
    private void Refresh() {
        if (text == null)
            return;

        string now = BossRecords.FormatTime(BossRecords.Elapsed);
        if (now == shown)
            return;

        shown = now;
        text.SetText(new TextMessage(now, false, true));
    }

    private void OnDestroy() {
        if (text != null && text.transform.parent != null)
            Destroy(text.transform.parent.gameObject);
    }
}
