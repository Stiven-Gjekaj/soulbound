using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The options screen: settings on the left, an explanation of whatever is highlighted on the
/// right.
///
/// The rows are built from the list in <see cref="Build"/> rather than from objects sitting in
/// the scene, which is the point of the rebuild. The screen this replaces had a fixed number of
/// rows, so adding an option meant either opening Unity or taking over a row left behind by a
/// setting that had been retired, which is what "Safe" and "Crate" were doing. Two settings
/// were wearing the names of two others, an eleventh option had nowhere to go at all, and the
/// hover descriptions keyed off object names that no longer described the row. Adding an option
/// is now one entry in a list.
///
/// Destructive rows are pressed twice. The first press arms them and says so, and the window
/// closes on its own, so a mis-timed second press cannot wipe anything.
/// </summary>
public class OptionsScreen : MonoBehaviour {
    /// <summary>Rows are created under this at runtime.</summary>
    public RectTransform rowRoot;
    public Text          description;
    public RectTransform cursor;
    public Font          font;

    public float rowTop   = 160f;
    public float rowPitch = 36f;
    public float rowX     = -149f;
    public float rowWidth = 250f;
    public float cursorX  = -298f;
    public int   rowSize  = 16;

    /// <summary>How long a destructive row stays armed, and how long a result stays on screen.</summary>
    private const float ConfirmWindow = 2f;

    private class Option {
        public Func<string> Label;
        public Action       Press;
        public string       Description;
        public string       ArmedLabel;

        public Text  Text;
        public float Armed;
        public float Flashing;
        public string Flash;
    }

    private readonly List<Option> options = new List<Option>();
    private int selected;

    private static readonly Color Idle   = new Color(1f, 1f, 1f, 1f);
    private static readonly Color Picked = new Color(1f, 1f, 0f, 1f);

    private void Start() {
        Build();
        Lay();
        Select(0);
    }

    /// <summary>Every option on the screen, in the order shown.</summary>
    private void Build() {
        Add(() => "Change name...",
            "Changes your character's name.\n\nThe name is stored as a Permanent Global, so it "
          + "survives wiping your save file, and resetting Permanent Globals makes the game ask "
          + "for it again.",
            () => SceneManager.LoadScene("EnterName"));

        Add(() => "In-fight timer: " + (FightTimer.Enabled ? "On" : "Off"),
            "Shows the running time of the fight in the corner of the screen.\n\nDisplay only. "
          + "Every fight is timed whether this is on or off, so turning it off never costs you a "
          + "record, and turning it on never gives you a second set of times that cannot be "
          + "compared with anyone else's.",
            () => FightTimer.Enabled = !FightTimer.Enabled);

        Add(() => "Fullscreen: " + (Screen.fullScreen ? "On" : "Off"),
            "Fills the screen instead of running in a window.\n\nF4 and Alt+Enter do the same "
          + "thing, where the system lets them through. On a Mac the function keys are usually "
          + "taken by the system, which is why this row exists.",
            () => ScreenResolution.SetFullScreen(!Screen.fullScreen));

        Add(() => "Window scale: " + ScreenResolution.windowScale + "x",
            "Scales the window in Windowed mode.\n\nThis is useful for especially large screens, "
          + "such as 4k monitors.\n\nHas no effect in Fullscreen mode.",
            CycleWindowScale);

        Add(() => "Discord display: " + DiscordControls.ChangeVisibilitySetting(0),
            "Changes how much Discord Rich Presence shows on your profile while you are playing "
          + "Soulbound.\n\n<b>Everything</b>: the boss you are fighting, a timestamp and a "
          + "description.\n\n<b>Game Only</b>: only that you are playing Soulbound.\n\n"
          + "<b>Nothing</b>: disables Discord Rich Presence entirely.\n\nIf the connection to "
          + "Discord is lost, you will have to restart Soulbound to get your rich presence back.",
            () => DiscordControls.ChangeVisibilitySetting(1));

        Add(() => "Keybinds...",
            "Changes the keys bound to the game's controls, such as Confirm or Cancel.\n\nThat "
          + "way your own keyboard scheme still works, and comfortably.",
            () => SceneManager.LoadScene("KeybindSettings"));

        // Everything below this point destroys something, which is why it sits below everything
        // that does not, and why each one has to be pressed twice.
        Add(() => "Reset session globals",
            "Resets all Session Globals, also known as Real Globals.\n\nSession Globals are "
          + "variables that persist through battles, but are deleted when the game is closed.",
            () => { LuaScriptBinder.ClearVariables(); Flash("Session globals erased."); },
            "Are you sure?");

        Add(() => "Reset permanent globals",
            "Resets all Permanent Globals, also known as AlMighty Globals.\n\nPermanent Globals "
          + "are variables saved to a file, and stay even after you close the game.\n\nYour "
          + "records and the options on this screen are stored as Permanent Globals.",
            () => {
                LuaScriptBinder.ClearPermanentGlobals();
                LuaScriptBinder.SetPermanentGlobal("CYFWindowScale", DynValue.NewNumber(ScreenResolution.windowScale));
                Flash("Permanent globals erased.");
            },
            "Are you sure?");

        Add(() => "Wipe save",
            "Clears your save file.\n\nThis holds your name, stats and inventory between "
          + "sessions.\n\nYour save file is at:\n\n<b><size=12>" + Application.persistentDataPath + "/save.gd</size></b>",
            () => {
                File.Delete(Application.persistentDataPath + "/save.gd");
                Flash("Save wiped.");
            },
            "Are you sure?");

        Add(() => "Back to the menu",
            "Returns to the menu.",
            () => SceneManager.LoadScene("TitleScreen"));
    }

    private void Add(Func<string> label, string description, Action press, string armedLabel = null) {
        options.Add(new Option { Label = label, Description = description, Press = press, ArmedLabel = armedLabel });
    }

    private static void CycleWindowScale() {
        #if UNITY_EDITOR
            int maxScale = 0;
        #else
            int maxScale = Mathf.FloorToInt(System.Math.Min(Screen.currentResolution.width / 640f,
                                                            Screen.currentResolution.height / 480f));
        #endif
        if (ScreenResolution.windowScale < maxScale) ScreenResolution.windowScale += 1;
        else                                         ScreenResolution.windowScale  = 1;
        ScreenResolution.tempWindowScale = ScreenResolution.windowScale;
        ScreenResolution.SetFullScreen(Screen.fullScreen);
        LuaScriptBinder.SetPermanentGlobal("CYFWindowScale", DynValue.NewNumber(ScreenResolution.windowScale));
    }

    /// <summary>Says what just happened, on the row it happened to.</summary>
    private void Flash(string message) {
        options[selected].Flash    = message;
        options[selected].Flashing = ConfirmWindow;
    }

    /// <summary>Makes one text row per option and hangs it under the row root.</summary>
    private void Lay() {
        for (int i = 0; i < options.Count; i++) {
            Option option = options[i];

            GameObject go = new GameObject("Option" + (i + 1), typeof(Text));
            Text text = go.GetComponent<Text>();
            text.font               = font;
            text.fontSize           = rowSize;
            text.alignment          = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow   = VerticalWrapMode.Overflow;
            text.raycastTarget      = false;

            RectTransform rt = go.GetComponent<RectTransform>();
            rt.SetParent(rowRoot, false);
            rt.anchorMin        = new Vector2(0.5f, 0.5f);
            rt.anchorMax        = new Vector2(0.5f, 0.5f);
            rt.pivot            = new Vector2(0.5f, 0.5f);
            rt.sizeDelta        = new Vector2(rowWidth, rowPitch - 4f);
            rt.anchoredPosition = new Vector2(rowX, rowTop - i * rowPitch);

            option.Text = text;
        }
        Refresh();
    }

    private void Update() {
        bool changed = false;
        for (int i = 0; i < options.Count; i++) {
            if (options[i].Armed > 0f) {
                options[i].Armed -= Time.deltaTime;
                if (options[i].Armed <= 0f) changed = true;
            }
            if (options[i].Flashing > 0f) {
                options[i].Flashing -= Time.deltaTime;
                if (options[i].Flashing <= 0f) { options[i].Flash = null; changed = true; }
            }
        }
        if (changed)
            Refresh();

        if (GlobalControls.input.Down == ButtonState.PRESSED)
            Select(Math.Mod(selected + 1, options.Count));
        else if (GlobalControls.input.Up == ButtonState.PRESSED)
            Select(Math.Mod(selected - 1, options.Count));
        else if (GlobalControls.input.Confirm == ButtonState.PRESSED)
            Press(selected);
        else if (GlobalControls.input.Cancel == ButtonState.PRESSED)
            SceneManager.LoadScene("TitleScreen");
    }

    private void Press(int index) {
        Option option = options[index];

        // A destructive row asks first. The window closes on its own, so leaving the screen
        // alone is the same as saying no.
        if (option.ArmedLabel != null && option.Armed <= 0f) {
            option.Armed = ConfirmWindow;
            Refresh();
            return;
        }

        option.Armed = 0f;
        option.Press();
        Refresh();
    }

    private void Select(int index) {
        selected = index;
        Refresh();

        if (cursor == null)
            return;
        cursor.anchoredPosition = new Vector2(cursorX, rowTop - index * rowPitch);
    }

    private void Refresh() {
        for (int i = 0; i < options.Count; i++) {
            Option option = options[i];
            if (option.Text == null)
                continue;
            option.Text.text  = option.Flash ?? (option.Armed > 0f ? option.ArmedLabel : option.Label());
            option.Text.color = i == selected ? Picked : Idle;
        }

        if (description != null && options.Count > 0)
            description.text = options[selected].Description;
    }
}
