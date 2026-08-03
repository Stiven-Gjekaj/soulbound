using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// One boss's place on the wheel. Every entry keeps its own object; where it sits is worked out
/// each frame from how far it is from the selection, so the wheel turns rather than the
/// contents being shuffled between fixed seats.
/// </summary>
[Serializable]
public class BossSlot {
    public Image         icon;
    public Image         padlock;
    public RectTransform pivot;
}

/// <summary>
/// The screen the game opens on and returns to. It lists the bosses, shows what the player has
/// done against each one, and is the only place progression is visible.
///
/// The bosses sit on a wheel down the left, turning past a fixed point: whichever entry is
/// selected is held at the middle of the arc, and the rest curve away above and below it and
/// drop off the ends. The panel beside it carries that entry's portrait, name, the line under
/// it, and the five records. A row in a table could hold none of those at a readable size,
/// which is what this layout buys and what the table it replaced could not do.
///
/// Seven positions, always, however many bosses the registry holds. A screen is a batch and the
/// roster grows by screens rather than by getting longer, so the wheel is built once and never
/// rearranged. Positions past the end of the registry are drawn the same as locked ones.
///
/// A locked boss shows its own icon with a padlock over it, dimmed. That says what the boss is
/// and that it is not available yet, and it keeps the position exactly the same size locked as
/// unlocked, so nothing on the wheel jumps when one opens.
///
/// Icons are mod content, loaded by id from Sprites/Bosses at runtime rather than referenced
/// from the scene, so adding a boss is still a registry line and a PNG with no scene edit and
/// no import settings. A boss with no icon yet simply shows none, which is most of them.
///
/// Nothing on this screen explains the lock. The player works out that clearing the available
/// one opens the rest.
/// </summary>
public class BossSelect : MonoBehaviour {
    public BossSlot[] slots;

    [Header("The panel")]
    public Image portrait;
    public Image portraitLock;
    public Text  bossName;
    public Text  subtitle;
    public Text  tries;
    public Text  clears;
    public Text  deaths;
    public Text  best;
    public Text  nohit;
    public Text  hint;

    [Header("The wheel")]
    /// <summary>
    /// The rim the entries ride on. Turned by the same amount they are, which is the whole
    /// reason it has notches: a featureless circle rotating looks like a circle standing
    /// still, so the notches are what makes the movement readable.
    /// </summary>
    public RectTransform wheelRim;

    /// <summary>Centre of the circle, in canvas coordinates. Sits off the left edge.</summary>
    public Vector2 wheelCentre = new Vector2(-350f, 0f);
    public float   wheelRadius = 230f;
    /// <summary>Degrees between one position and the next.</summary>
    public float   wheelStep = 27.5f;
    /// <summary>How far from the middle a position can be before it is off the wheel.</summary>
    public float   wheelReach = 2.6f;
    /// <summary>Seconds the wheel takes to settle after a move.</summary>
    public float   spinTime = 0.14f;

    private static int selected;
    private List<BossEntry> bosses;
    private bool ready;
    private bool launching;

    /// <summary>How far the wheel still has to turn, in positions. Decays to zero.</summary>
    private float spin;

    private static readonly Color NameOpen  = new Color(1f,    1f,    1f,    1f);
    private static readonly Color NameShut  = new Color(0.5f,  0.5f,  0.5f,  1f);
    private static readonly Color ValueOpen = new Color(1f,    1f,    0f,    1f);
    private static readonly Color ValueShut = new Color(0.5f,  0.5f,  0.5f,  1f);
    private static readonly Color ArtOpen   = new Color(1f,    1f,    1f,    1f);
    private static readonly Color ArtShut   = new Color(0.42f, 0.42f, 0.42f, 1f);

    /// <summary>Boss icons, kept between redraws because the wheel redraws every frame it turns.</summary>
    private readonly Dictionary<string, Sprite> icons = new Dictionary<string, Sprite>();

    /// <summary>
    /// A boss's icon, or null if it has none yet. Read from the mod by id, so a new boss needs
    /// a registry line and a PNG rather than a scene edit.
    /// </summary>
    private Sprite Icon(int index) {
        if (index >= bosses.Count)
            return null;
        string id = bosses[index].id;
        if (icons.ContainsKey(id))
            return icons[id];

        FileLoader.absoluteSanitizationDictionary.Clear();
        FileLoader.relativeSanitizationDictionary.Clear();

        Sprite sprite;
        try   { sprite = SpriteUtil.FromFile("Bosses/" + id + ".png"); }
        catch { sprite = null; }

        icons.Add(id, sprite);
        return sprite;
    }

    private void Start() {
        Destroy(GameObject.Find("Player"));
        UnitaleUtil.firstErrorShown = false;

        // There is one mod, and it is the game.
        StaticInits.MODFOLDER = StaticInits.GAME_MODFOLDER;

        // Name the character before the first fight. Returning from a fight is not a first
        // run, and checking that also means a failed name entry cannot loop.
        if (StaticInits.ENCOUNTER == "" && !PlayerProfile.HasName) {
            SceneManager.LoadScene("EnterName");
            return;
        }

        // Re-read the registry every time, so editing bosses.lua does not need a restart.
        BossRegistry.Reload();
        bosses = BossRegistry.Entries;

        // BossRegistry has already sent the player to the error screen saying why.
        if (bosses.Count == 0)
            return;

        // Arriving from anywhere but a fight is a fresh run, so start at the top of the wheel.
        // Coming back from a fight keeps the cursor on the boss just fought.
        if (StaticInits.ENCOUNTER == "")
            selected = 0;
        if (selected >= BossProgress.SlotsPerScreen)
            selected = 0;

        // Reset it to tell a later arrival from the menu apart from one from a battle.
        StaticInits.ENCOUNTER = "";

        spin = 0f;
        DrawPanel();
        TurnWheel();
        ready = true;
    }

    private void Update() {
        if (!ready)
            return;

        // The wheel keeps settling even while a fight is loading, so the screen does not
        // freeze mid-turn on the way out.
        if (spin != 0f) {
            float step = Time.deltaTime / Mathf.Max(0.01f, spinTime);
            spin = spin > 0f ? Mathf.Max(0f, spin - step) : Mathf.Min(0f, spin + step);
            TurnWheel();
        }

        if (launching)
            return;

        // Down and Right both turn the wheel the same way. On an arc there is no useful
        // difference between along and around.
        if (GlobalControls.input.Down == ButtonState.PRESSED || GlobalControls.input.Right == ButtonState.PRESSED) {
            Move(1);
        } else if (GlobalControls.input.Up == ButtonState.PRESSED || GlobalControls.input.Left == ButtonState.PRESSED) {
            Move(-1);
        } else if (GlobalControls.input.Confirm == ButtonState.PRESSED) {
            Choose();
        } else if (GlobalControls.input.Cancel == ButtonState.PRESSED) {
            SceneManager.LoadScene("TitleScreen");
        } else if (GlobalControls.input.Menu == ButtonState.PRESSED) {
            SceneManager.LoadScene("Options");
        }
    }

    /// <summary>
    /// Turns the wheel one position. The selection changes immediately and the wheel catches
    /// up, so holding a direction cannot outrun the panel or leave it showing the wrong boss.
    /// </summary>
    private void Move(int direction) {
        selected = Math.Mod(selected + direction, BossProgress.SlotsPerScreen);

        // Carry over whatever is left of the last turn rather than resetting, so a fast player
        // gets a wheel that keeps up instead of one that stutters back to the start each time.
        spin = Mathf.Clamp(spin + direction, -2f, 2f);

        DrawPanel();
        TurnWheel();
    }

    /// <summary>
    /// Places every position on the arc according to how far it is from the selection, plus
    /// however much of the turn is still outstanding.
    /// </summary>
    private void TurnWheel() {
        int count = BossProgress.SlotsPerScreen;
        int half  = count / 2;

        // The rim turns with the entries rather than sitting still behind them. Entries move
        // up the arc as the selection advances, which is counter-clockwise on the right hand
        // side of the circle, so the rotation runs with the selection and against the spin
        // still outstanding.
        if (wheelRim != null)
            wheelRim.localRotation = Quaternion.Euler(0f, 0f, (selected - spin) * wheelStep);

        for (int i = 0; i < slots.Length; i++) {
            BossSlot slot = slots[i];
            if (slot.pivot == null || slot.icon == null)
                continue;

            // Signed distance from the selection, wrapped, so the wheel is a loop rather than
            // a list with two ends.
            float offset = Math.Mod(i - selected + half, count) - half + spin;
            float away   = Mathf.Abs(offset);

            if (away > wheelReach) {
                slot.icon.enabled = false;
                if (slot.padlock != null)
                    slot.padlock.enabled = false;
                continue;
            }

            float radians = offset * wheelStep * Mathf.Deg2Rad;
            slot.pivot.anchoredPosition = wheelCentre + new Vector2(Mathf.Cos(radians), -Mathf.Sin(radians)) * wheelRadius;

            // Nearest the middle is biggest and brightest; the ends taper off so entries
            // arrive and leave rather than popping.
            float near  = Mathf.Clamp01(1f - away / wheelReach);
            float scale = Mathf.Lerp(0.55f, 1.35f, near);
            slot.pivot.localScale = new Vector3(scale, scale, 1f);

            bool   unlocked = Unlocked(i);
            Sprite art      = Icon(i);
            float  fade     = Mathf.Lerp(0.15f, 1f, near);

            // A boss with no icon yet shows none. Most of them have none, and an empty box is
            // a truer placeholder than a stand-in that says something about art nobody drew.
            slot.icon.enabled = art != null;
            if (art != null) {
                slot.icon.sprite = art;
                Color tint = unlocked ? ArtOpen : ArtShut;
                tint.a = fade;
                slot.icon.color = tint;
            }

            // The padlock rides over the icon, and shows on anything not yet available,
            // including positions with no boss behind them at all.
            if (slot.padlock != null) {
                slot.padlock.enabled = !unlocked;
                slot.padlock.color   = new Color(1f, 1f, 1f, fade);
            }
        }
    }

    private bool Unlocked(int index) {
        return index < bosses.Count && BossProgress.Unlocked(index);
    }

    /// <summary>
    /// Starts the selected fight, if it is one. A position past the end of the registry and a
    /// locked one both do nothing: the screen says no by not responding, which is the same
    /// answer the greying already gave.
    /// </summary>
    private void Choose() {
        if (!Unlocked(selected))
            return;
        launching = true;
        StartCoroutine(LaunchBoss(bosses[selected]));
    }

    private IEnumerator LaunchBoss(BossEntry boss) {
        // Make sure the encounter is still here and can be opened.
        if (!File.Exists(Path.Combine(FileLoader.ModDataPath, "Lua/Encounters/" + boss.id + ".lua"))) {
            UnitaleUtil.DisplayLuaError(BossRegistry.FileName, "The boss \"" + boss.id + "\" had an encounter script when the list loaded, but it is gone now.");
            launching = false;
            yield break;
        }

        StaticInits.ENCOUNTER = boss.id;

        yield return new WaitForEndOfFrame();
        try {
            StaticInits.InitAll(StaticInits.MODFOLDER, true);
            if (UnitaleUtil.firstErrorShown)
                throw new Exception();
            GlobalControls.isInFight = true;
            // Count the attempt only once the encounter has loaded. A boss whose script fails
            // to load was never fought, and should not cost the player a try.
            BossRecords.FightStarting(boss.id);
            DiscordControls.StartBattle(boss.name);
            SceneManager.LoadScene("Battle");
        } catch (Exception e) {
            launching = false;
            Debug.LogError("An error occured while starting a boss fight:\n" + e.Message + "\n\n" + e.StackTrace);
        }
    }

    private void DrawPanel() {
        bool present  = selected < bosses.Count;
        bool unlocked = Unlocked(selected);

        if (portrait != null) {
            Sprite art = present ? Icon(selected) : null;
            portrait.enabled = art != null;
            if (art != null) {
                portrait.sprite = art;
                portrait.color  = unlocked ? ArtOpen : ArtShut;
            }
        }
        if (portraitLock != null)
            portraitLock.enabled = !unlocked;

        if (bossName != null) {
            // A locked boss shows its name, so the player can see how much game is waiting.
            // A position with nothing behind it yet has no name to show.
            bossName.text  = present ? bosses[selected].name : "???";
            bossName.color = unlocked ? NameOpen : NameShut;
        }

        if (subtitle != null) {
            subtitle.text  = unlocked ? bosses[selected].subtitle : "";
            subtitle.color = NameShut;
        }

        if (unlocked) {
            BossEntry boss = bosses[selected];
            Set(tries,  ValueOpen, BossRecords.Attempts(boss.id).ToString());
            Set(clears, ValueOpen, BossRecords.Clears(boss.id).ToString());
            Set(deaths, ValueOpen, BossRecords.Deaths(boss.id).ToString());
            float b = BossRecords.BestTime(boss.id);
            Set(best,   ValueOpen, b >= 0f ? BossRecords.FormatTime(b) : "-");
            Set(nohit,  ValueOpen, BossRecords.NoHit(boss.id) ? "YES" : "-");
        } else {
            // The question mark stands in for the records, which are the part that genuinely
            // has no value yet. The name and the shape are known; these are not.
            Set(tries,  ValueShut, "?");
            Set(clears, ValueShut, "?");
            Set(deaths, ValueShut, "?");
            Set(best,   ValueShut, "?");
            Set(nohit,  ValueShut, "?");
        }

        if (hint != null) {
            // A teased boss says so. A locked one says nothing, because the player is meant to
            // work the lock out, but "you have not earned this" and "this does not exist yet"
            // are different answers and the screen should not give the same silence to both.
            if (unlocked)                        hint.text = "Confirm to fight";
            else if (BossProgress.Teased(selected)) hint.text = "Not in this build";
            else                                 hint.text = "";
        }
    }

    private static void Set(Text text, Color color, string value) {
        if (text == null)
            return;
        text.text  = value;
        text.color = color;
    }
}
