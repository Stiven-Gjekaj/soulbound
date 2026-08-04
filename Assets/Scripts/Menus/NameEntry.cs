using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Names the player's character. Reached once before the first fight, and again from the
/// options screen whenever they want to change it.
///
/// The screen has two phases. In the first the name is typed; in the second it is confirmed,
/// and a handful of names answer back instead of asking. Answering no returns to typing rather
/// than starting over, so a near miss costs one keystroke.
///
/// Letters are typed rather than picked off a grid, which v0.5 settled. This rebuild keeps that
/// and drops what the grid left behind: two text objects that were filled with the alphabet at
/// runtime and switched off again, a Backspace button that duplicated the Backspace key, and a
/// set of buttons that were bare sprites with no colliders, hit-tested against their own world
/// bounds because Unity's pointer events could not see them.
/// </summary>
public class NameEntry : MonoBehaviour {
    public Text  heading;
    public Text  nameText;
    public Image underline;
    public Image fade;
    public Text  quitLabel;
    public Text  doneLabel;

    /// <summary>The longest name the game stores. PlayerCharacter truncates past this.</summary>
    private const int MaxLength = 9;

    private const string Instruction = "Name the fallen human.";

    private string playerName = "";
    private bool   onDone     = true;
    private bool   confirming;
    private bool   leaving;
    private bool   isNewGame  = true;
    private string answer;

    private AudioSource audio;

    private static readonly Color Idle   = new Color(1f, 1f, 1f, 1f);
    private static readonly Color Picked = new Color(1f, 1f, 0f, 1f);
    private static readonly Color Hidden = new Color(1f, 1f, 1f, 0f);

    private void Start() {
        isNewGame = SaveLoad.savedGame == null;
        if (!isNewGame)
            playerName = PlayerCharacter.instance.Name;

        audio = Camera.main ? Camera.main.GetComponent<AudioSource>() : null;
        // Through MenuAudio, because a clip the registry cannot reach arrives as a thrown
        // exception rather than as a null. mus_menu is on disk but inside the @Title mod, and
        // the registry only searches the loaded mod and Default, so the lookup misses. Asked
        // for directly it took out everything below: the fade cover was left opaque and the
        // heading unwritten, on a screen with no way to report either.
        if (isNewGame)
            MenuAudio.PlayMusic("mus_menu");

        if (fade != null)
            fade.color = new Color(0f, 0f, 0f, 0f);

        heading.text = Instruction;
        Refresh();
    }

    private void Update() {
        if (leaving)
            return;

        // Typed characters are read before anything else looks at the keyboard. W, A, S and D
        // are bound to the four directions by default, so without this a name with a W in it
        // would move between the buttons while spelling itself.
        if (!confirming && Typed())
            return;

        if ((GlobalControls.input.Left == ButtonState.PRESSED || GlobalControls.input.Right == ButtonState.PRESSED)
         && CanSwitch()) {
            onDone = !onDone;
            Play("menumove");
            Refresh();
        } else if (GlobalControls.input.Confirm == ButtonState.PRESSED) {
            Press();
        } else if (!confirming && GlobalControls.input.Cancel == ButtonState.PRESSED) {
            Backspace();
        }
    }

    /// <summary>
    /// Whether the two buttons can be swapped between. A forbidden name leaves only one way
    /// out of the confirm screen, so there is nothing to swap to.
    /// </summary>
    private bool CanSwitch() {
        return !confirming || !Forbidden(playerName);
    }

    /// <summary>Takes whatever was typed this frame and puts it in the name.</summary>
    private bool Typed() {
        string typed = Input.inputString;
        if (typed.Length == 0)
            return false;

        bool changed = false;
        foreach (char c in typed) {
            // Return and Enter are Confirm, which presses the highlighted button. Let them
            // through rather than swallowing them here.
            if (c == '\r' || c == '\n')
                continue;

            if (c == '\b') {
                changed |= Backspace(true);
                continue;
            }

            // Letters only. Anything else could sit invisibly inside a saved name, because
            // the font has no glyph to draw it with.
            if (c < 'A' || c > 'z' || (c > 'Z' && c < 'a'))
                continue;

            if (playerName.Length < MaxLength) playerName += c;
            else                               playerName  = playerName.Substring(0, MaxLength - 1) + c;
            changed = true;
        }

        if (!changed)
            return false;

        Play("menuconfirm");
        Refresh();
        return true;
    }

    private bool Backspace(bool silent = false) {
        if (playerName.Length == 0)
            return false;
        playerName = playerName.Substring(0, playerName.Length - 1);
        if (!silent) {
            Play("menumove");
            Refresh();
        }
        return true;
    }

    private void Press() {
        Play("menuconfirm");

        if (!confirming) {
            if (!onDone) {
                leaving = true;
                if (audio != null) audio.Stop();
                SceneManager.LoadScene("TitleScreen");
                return;
            }

            // An empty name is accepted and becomes the default. Letters can only be typed, so
            // a player on a controller can reach this button with no way to put anything in
            // front of it, and refusing them would leave them on a screen with no way out.
            if (playerName.Length == 0)
                playerName = ControlPanel.instance.BasisName;

            confirming = true;
            SpecialAnswers.TryGetValue(playerName.ToLower(), out answer);
            // A forbidden name can only be gone back on, so the cursor starts on the way back.
            onDone = !Forbidden(playerName);
            Refresh();
            return;
        }

        if (!onDone) {
            confirming = false;
            answer     = null;
            onDone     = true;
            Refresh();
            return;
        }

        leaving = true;
        PlayerCharacter.instance.Name = playerName;
        // Store what PlayerCharacter kept rather than what was typed, so the name round trips.
        PlayerProfile.Name = PlayerCharacter.instance.Name;
        StartCoroutine(Leave());
    }

    private IEnumerator Leave() {
        if (!isNewGame) {
            SaveLoad.Save();
            SceneManager.LoadScene("TitleScreen");
            yield break;
        }

        if (audio != null) {
            audio.Stop();
            // intro_holdup is in the @Title mod, which the registry does not search from here,
            // so it is a miss and a miss throws. This coroutine still has to fade the screen
            // out and load the boss select, so the screen would stop half faded and stay there.
            AudioClip holdup = MenuAudio.Sound("intro_holdup");
            if (holdup != null)
                audio.PlayOneShot(holdup);
        }

        while (fade != null && fade.color.a < 1f) {
            fade.color = new Color(0f, 0f, 0f, fade.color.a + 0.02f);
            yield return null;
        }
        while (audio != null && audio.isPlaying)
            yield return null;

        GlobalControls.modDev = true;
        DiscordControls.StartBossSelect();
        SceneManager.LoadScene("ModSelect");
    }

    private void Play(string sound) {
        AudioClip clip = MenuAudio.Sound(sound);
        if (audio != null && clip != null)
            audio.PlayOneShot(clip);
    }

    private void Refresh() {
        nameText.text = playerName;

        if (confirming) {
            heading.text = answer ?? "Is this name correct?";
            if (underline != null) underline.gameObject.SetActive(false);
            quitLabel.text = "No";
            doneLabel.text = "Yes";
            // A forbidden name offers no way forward, only back.
            doneLabel.color = Forbidden(playerName) ? Hidden : (onDone ? Picked : Idle);
            quitLabel.color = onDone && !Forbidden(playerName) ? Idle : Picked;
            return;
        }

        heading.text = Instruction;
        if (underline != null) underline.gameObject.SetActive(true);
        quitLabel.text  = "Quit";
        doneLabel.text  = "Done";
        quitLabel.color = onDone ? Idle : Picked;
        doneLabel.color = onDone ? Picked : Idle;
    }

    private static bool Forbidden(string name) {
        return ForbiddenNames.Contains(name.ToLower());
    }

    private static readonly string[] ForbiddenNames = { "lukark", "rtl", "rhenaud" };

    /// <summary>
    /// Names that answer back instead of asking whether they are correct.
    ///
    /// Inherited wholesale from Create Your Frisk, and left alone on purpose: this is a screen
    /// pass, and what the game says is a writing decision rather than a layout one. Several of
    /// these name the fork or its author, several are marked broken, and several point at
    /// features this game does not have. That wants a pass of its own.
    /// </summary>
    private static readonly Dictionary<string, string> SpecialAnswers = new Dictionary<string, string> {
        { "lukark",    "Hey, that's my name!\nDon't copy me." },
        { "rtl",       "Still my name, dude." },
        { "rhenao",    "The basis name." },
        { "rhenaud",   "My real name." },
        { "uduu",      "(Broken) The path to victory. Go to\nthe 2nd map. Real name: UDUUL" },
        { "thefail",   "(Broken) DO 3 BARREL ROLLS!!!" },
        { "exception", "(Broken) It's me." },
        { "fugitive",  "(Broken) *flees*\n/me flees" },
        { "four",      "4" },
        { "outbounds", "Go behind that dog!" },
        { "soulless",  "They shall fall, one\nafter another." },
        { "notfound",  "404" },
        { "404",       "Name not found." },
        { "cyf",       "The true name.\nCreate Your Frisk FTW!" },
        { "credits",   "RhenaudTheLukark and lvkuln.\nThat's it, I think." },
        { "mmmmmmmmm", "You just want to watch\nthe engine burn." },
        { "wwwwwwwww", "You just want to watch\nthe engine burn." },
        { "undertale", "Without this game,\nthis wouldn't exist." },
        { "frisk",     "That'll do nothing here." },
        { "chara",     "Classic af. Your \"The true name.\"\nis in another castle." },
        { "undyne",    "It's not like you'll\nfind her, anyway." },
        { "alphys",    "Chances to see\nher: 0%." },
        { "asgore",    "Goatdad can't kill\nyou here." },
        { "toriel",    "Goatmom can't help\nyou here." },
        { "papyrus",   "HAVING MORE THAN\n6 CHARACTERS HELPS!" },
        { "sans",      "no bad time for ya." },
        { "asriel",    "If that's your\nchoice..." },
        { "flowey",    "You sadist." },
    };
}
