using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Draws the running fight time in the corner of the battle screen.
///
/// Display only. The clock in BossRecords runs on every fight whether this is on or off, so
/// turning it off never costs a record and turning it on never creates a second set of times.
/// That is why this is a display setting and not a mode.
///
/// The readout is an object in Battle.unity now. It used to be built at runtime from the same
/// prefab Lua's CreateText uses, because the scene had nothing for it: that meant a
/// Resources.Load, an Instantiate, a caller bound to the encounter script, and a text object
/// that rebuilt one sprite per letter whenever the string changed, which is why refreshing it
/// had to be rationed to the tenths that actually differed. A Text component costs none of
/// that, so Update can simply write the time.
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

    /// <summary>The readout itself, bound in the scene.</summary>
    public Text display;

    private void Start() {
        // The object is always in the scene; whether it draws is the player's setting. Read it
        // once here rather than every frame, because a fight cannot change it.
        if (display != null)
            display.gameObject.SetActive(Enabled);
    }

    private void Update() {
        if (display == null || !display.gameObject.activeSelf)
            return;
        display.text = BossRecords.FormatTime(BossRecords.Elapsed);
    }
}
