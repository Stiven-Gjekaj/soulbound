using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// One row of the boss select: the art, the name, and the five records.
/// </summary>
[Serializable]
public class BossSlot {
    public Image silhouette;
    public Text  bossName;
    public Text  tries;
    public Text  clears;
    public Text  deaths;
    public Text  best;
    public Text  nohit;
}

/// <summary>
/// The screen the game opens on and returns to. It lists the bosses, shows what the player has
/// done against each one, and is the only place progression is visible.
///
/// Seven slots, always. A screen is a batch and the roster grows by screens rather than by
/// getting longer, so the layout is laid out once and never rearranged: arrivals fill slots
/// that were already there rather than pushing anything around. Slots past the end of the
/// registry are drawn the same as locked ones, which is what "announced but not built" looks
/// like and is honest about there being more to come.
///
/// The silhouette is the reason the slot holds its shape. A locked entry and an unlocked one
/// occupy exactly the same box, so nothing on the screen moves when a boss unlocks. This pass
/// every slot carries the same placeholder silhouette, including the unlocked one, because
/// that is what proves the claim: if the layout is going to jump, it jumps here.
///
/// Nothing on this screen explains the lock. The player works out that clearing the available
/// one opens the rest, and a line of text saying so would be telling them what the layout
/// already said.
/// </summary>
public class BossSelect : MonoBehaviour {
    public BossSlot[] slots;

    /// <summary>The selected boss's one-line description, under the table.</summary>
    public Text footer;

    private static int selected;
    private List<BossEntry> bosses;
    private bool ready;
    private bool launching;

    private static readonly Color NameIdle     = new Color(1f,    1f,    1f,   1f);
    private static readonly Color NamePicked   = new Color(1f,    1f,    0f,   1f);
    private static readonly Color LockedIdle   = new Color(0.45f, 0.45f, 0.45f, 1f);
    private static readonly Color LockedPicked = new Color(0.65f, 0.65f, 0.35f, 1f);
    private static readonly Color ArtOpen      = new Color(1f,    1f,    1f,   1f);
    private static readonly Color ArtLocked    = new Color(0.35f, 0.35f, 0.35f, 1f);

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

        // Arriving from anywhere but a fight is a fresh run, so start at the top. Coming back
        // from a fight keeps the cursor on the boss just fought.
        if (StaticInits.ENCOUNTER == "")
            selected = 0;
        if (selected >= BossProgress.SlotsPerScreen)
            selected = 0;

        // Reset it to tell a later arrival from the disclaimer apart from one from a battle.
        StaticInits.ENCOUNTER = "";

        Refresh();
        ready = true;
    }

    private void Update() {
        if (!ready || launching)
            return;

        if (GlobalControls.input.Down == ButtonState.PRESSED) {
            selected = Math.Mod(selected + 1, BossProgress.SlotsPerScreen);
            Refresh();
        } else if (GlobalControls.input.Up == ButtonState.PRESSED) {
            selected = Math.Mod(selected - 1, BossProgress.SlotsPerScreen);
            Refresh();
        } else if (GlobalControls.input.Confirm == ButtonState.PRESSED) {
            Choose();
        } else if (GlobalControls.input.Cancel == ButtonState.PRESSED) {
            SceneManager.LoadScene("TitleScreen");
        } else if (GlobalControls.input.Menu == ButtonState.PRESSED) {
            SceneManager.LoadScene("Options");
        }
    }

    /// <summary>
    /// Starts the selected fight, if it is one. A slot past the end of the registry and a
    /// locked slot both do nothing: the screen says no by not responding, which is the same
    /// answer the greying already gave.
    /// </summary>
    private void Choose() {
        if (selected >= bosses.Count || !BossProgress.Unlocked(selected))
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

    /// <summary>Redraws every slot from the registry and the records.</summary>
    private void Refresh() {
        for (int i = 0; i < slots.Length; i++)
            Draw(slots[i], i);

        if (footer != null)
            footer.text = selected < bosses.Count && BossProgress.Unlocked(selected)
                        ? bosses[selected].subtitle
                        : "";
    }

    private void Draw(BossSlot slot, int index) {
        bool picked   = index == selected;
        bool present  = index < bosses.Count;
        bool unlocked = present && BossProgress.Unlocked(index);

        // Every slot keeps its silhouette whether or not there is a boss behind it. That is
        // the whole point of the silhouette: the box is the same size either way.
        if (slot.silhouette != null)
            slot.silhouette.color = unlocked ? ArtOpen : ArtLocked;

        Color tint = unlocked ? (picked ? NamePicked : NameIdle)
                              : (picked ? LockedPicked : LockedIdle);

        if (slot.bossName != null) {
            // A locked boss shows its name, so the player can see how much game is waiting.
            // A slot with nothing behind it yet has no name to show.
            slot.bossName.text  = present ? bosses[index].name : "???";
            slot.bossName.color = tint;
        }

        if (unlocked) {
            BossEntry boss = bosses[index];
            Set(slot.tries,  tint, BossRecords.Attempts(boss.id).ToString());
            Set(slot.clears, tint, BossRecords.Clears(boss.id).ToString());
            Set(slot.deaths, tint, BossRecords.Deaths(boss.id).ToString());
            float best = BossRecords.BestTime(boss.id);
            Set(slot.best,   tint, best >= 0f ? BossRecords.FormatTime(best) : "-");
            Set(slot.nohit,  tint, BossRecords.NoHit(boss.id) ? "YES" : "-");
        } else {
            // The question mark stands in for the records, which are the part that genuinely
            // has no value yet. The name and the shape are known; these are not.
            Set(slot.tries,  tint, "?");
            Set(slot.clears, tint, "?");
            Set(slot.deaths, tint, "?");
            Set(slot.best,   tint, "?");
            Set(slot.nohit,  tint, "?");
        }
    }

    private static void Set(Text text, Color color, string value) {
        if (text == null)
            return;
        text.text  = value;
        text.color = color;
    }
}
