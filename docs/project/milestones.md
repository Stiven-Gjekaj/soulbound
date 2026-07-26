<div align="center">
  <a href="../../README.md"><b>Soulbound</b></a>
</div>

# Milestones and roadmap

Soulbound is a boss rush game based on Soultale, built on a heavily modified
Create Your Frisk engine. You pick a boss from a menu and fight it. There is no
overworld.

That last sentence drives most of this roadmap. Create Your Frisk is a general
Undertale-fangame engine, and roughly a third of it exists to support a feature
this game does not have. The near-term work is making the engine fit the game.

Art is not the constraint right now. Menus and boss assets arrive later, so the
early milestones are engine work with placeholder art, sequenced so that nothing
blocks on a sprite that does not exist yet.

## v0.0: a clean foundation

Complete. An unmodified Create Your Frisk v0.6.6 LTS 3 snapshot turned into a
documented base: 37 MB of upstream example content removed, the 644 KB
documentation website converted by hand into 46 Markdown pages, a scaffolded mod
folder, and a build passing CI on Windows, macOS, and Linux. See the
[changelog](../../CHANGELOG.md).

One v0.0 decision is already known to be wrong for this game. The Tiled2Unity map
importer was deliberately kept, with the reasoning that it was needed to author
future maps. A boss rush has no maps. It comes out in v0.1.

## Removing the overworld: v0.1 and v0.2

Taking the overworld out is two milestones, not one, and the line between them is
mechanical rather than a matter of taste.

The engine asks `UnitaleUtil.IsOverworld` in **48 places** outside the overworld
feature itself, and branches on the answer. Alongside it sit
`GlobalControls.isInShop` (9 uses) and `PlayerOverworld.audioKept` (17 uses).
These are not calls into the overworld that break when it is deleted. They are
questions the engine asks itself about which mode it is in.

That gives a clean division:

- **v0.1 makes the answer permanently `false`.** Delete the feature, its assets,
  its Lua API, and its scenes. Keep `IsOverworld` as a shim returning `false`, so
  every branch that consults it collapses to the battle path on its own and the
  project keeps compiling. The edits to battle code are the few that genuinely
  reference overworld types.
- **v0.2 makes the engine stop asking.** With the flag permanently false, walk
  the 48 branches and delete the dead half of each, then remove `IsOverworld`,
  `isInShop`, `audioKept`, `nonOWScenes`, and `canTransOW` entirely.

Doing it in that order matters. v0.1 is destructive but shallow: it touches the
battle path as little as possible, so if the game breaks, the cause is in a small
diff. v0.2 is invasive but mechanical: every edit has a known-correct answer,
because the condition is a constant. Fusing them would mean making risky deletions
and subtle logic changes in the same commits, which is exactly how a working
battle path gets broken quietly.

The risk is worth naming: this is the path the game runs on. The mitigation is CI,
which now builds all three platforms in about three minutes.

## v0.1: strip the overworld

Complete. The feature is gone and the battle path still works. The engine is 111 C#
files and 19,505 lines, down from 129 and 25,334. See the
[changelog](../../CHANGELOG.md).

| Piece | Size |
| ----- | ---- |
| `Assets/Scripts/Overworld` | 12 of its 18 files, 4534 lines |
| `Assets/Scripts/Lua/CLRBindings/Overworld` | 6 files, 1088 lines |
| `Assets/Tiled2Unity` | 53 files, 564 KB |
| Overworld scenes | `TransitionOverworld`, `Shop`, `SpecialAnnouncement` |
| Overworld prefabs | `Canvas OW`, `Main Camera OW`, `Player`, `Background 1`, `Event1`, `ImageEvent`, `Save`, `TP On-the-fly`, `Maps/` |
| Overworld sprites | `FriskUT`, `AsrielOW`, `CharaOW`, `MonsterKidOW`, `BoosterOW`, `SavePoint` |
| Overworld documentation | 11 of the 46 pages, and 2 orphaned images |

`Assets/Scripts/Overworld` was a mixed folder, not an overworld folder. Six of its
files were kept and moved out:

| File | Lines | Why it stays |
| ---- | ----- | ------------ |
| `SaveLoad.cs` | 100 | The save system, and the AlMighty globals v0.4 builds on |
| `PermanentGameState.cs` | 56 | The persistent globals format |
| `GameState.cs` | 164 | The session save format. Gutted of its map fields in v0.2 |
| `Title.cs` | 235 | Backs `TitleScreen` |
| `IntroManager.cs` | 186 | Backs `Intro` |
| `EnterNameScript.cs` | 305 | Backs `EnterName` |

The first three kept the folder under its new name, `Assets/Scripts/Save`; the last
three joined the other menu screens in `Assets/Scripts/PregamePlaceholder`. For the
same reason `TextManager OW.prefab` was kept: `EnterName` instantiates it, and it
carries `EnterNameScript`. Its misleading name is a v0.2 rename.

Six references genuinely pointed at overworld types and were rewired rather than
left to a constant:

- `Battle/UIController.cs` returned the player to the overworld when a battle
  ended. It returns to the mod selector now, until v0.3 gives it a boss select to
  return to.
- `Battle/GameOverBehavior.cs` sent the player back to a save point on death,
  instantiating a teleport prefab to do it. Death restarts without one.
- `Battle/UIController.cs` and `Battle/EnemyEncounter.cs` carried music across the
  overworld boundary. That channel is gone.
- `Device/GlobalControls.cs` opened the overworld pause menu.
- `Title.cs` and `EnterNameScript.cs` loaded the `TransitionOverworld` scene. Both
  are kept scripts loading a deleted scene, so both go to the mod selector.
- `SaveLoad.cs` asked the event manager to snapshot map state before saving.

Two rules ordered the work. Assets went before scripts, because deleting a scene
never breaks compilation but a scene holding the GUID of a deleted script shows as
a missing component. References went before definitions, because C# compiles all or
nothing, so a type can only be deleted once nothing names it.

Inside the folder the scripts formed a reference cycle: `PlayerOverworld`,
`EventManager` and `TransitionOverworld` name each other, and `EventManager` held
instances of all six Lua bindings that call back into it. Only `MapLoader` and
`SpecialAnnouncementScript` were true leaves; the other 16 files were one strongly
connected component, so they came out together once nothing outside the overworld
named them.

## v0.2: clean up after the overworld

Complete. The engine no longer asks what mode it is in, and nothing observable
changed, because every branch deleted was already unreachable. The engine is 111 C#
files and 19,283 lines. See the [changelog](../../CHANGELOG.md).

Three things were deliberately left alone, and are worth knowing about:

- The `"event"` sprite tag in `LuaSpriteController` and `UnitaleUtil`. It is assigned
  to any sprite backed by a `SpriteRenderer` rather than an `Image`, so it may still be
  reachable in battle. Proving that either way costs more than the tidiness is worth,
  so the logic and its error messages stay as they are.
- The retro-mode flee lines in `UIController`, which joke about the overworld being
  missing. They are Unitale's original text and remain literally true here.
- The boot chain. Trimming it would change what the player sees, and the menus want
  designing properly in v0.3 alongside boss select.

The save path is also kept on purpose. `SaveLoad.Save()` has one caller, name entry,
and may briefly have none after v0.3 replaces the menus. It is not dead code: a
checkpoint feature, such as saving between phases of a multi-phase boss, would be
built on `GameState` plus session or AlMighty globals rather than on anything new.

The counts below are recounted from the tree after v0.1, not estimated before it.
v0.1 consumed 8 of the branches and all 17 `audioKept` uses on its way past.

- The **40** `IsOverworld` branches, by file: `GameOverBehavior` (18), `SpriteUtil`
  (8), `Inventory` (5), `TextManager` (3), `LuaScriptBinder` (2), and one each in
  `LuaProjectile`, `LuaTextManager`, `GlobalControls` and `UnitaleUtil` itself.
  `Misc` is already clean.
- `isInShop`, down to 3 uses from 9, `nonOWScenes`, `canTransOW`, and `IsOverworld`
  itself. `audioKept` is already gone; it went with `PlayerOverworld`.
- `GlobalControls.EventData`, which is never read or written, and
  `GlobalControls.realName`, which is assigned `null` once and never read.
- The map fields still on `GameState`: `mapInfos`, `tempMapInfos`, `lastScene`,
  and the `MapData`, `TempMapData`, `EventInfos` and `Vect` structs behind them.
  `UnitaleUtil.VectToVector` and `VectorToVect` go with `Vect`, having no callers.
- `TextManager OW.prefab` and the two places `TextManager.cs` matches on its
  name. It is the name entry text box, and it should say so.
- `UnitaleUtil.ExitOverworld`, which v0.1 reduced to session teardown and no longer
  exits anything. It needs a name that matches what it does.
- The `Canvas OW` and `Canvas Two` lookups still left in `ErrorDisplay` and
  `SelectOMatic`, destroying objects that can no longer exist.
- Documentation: the API reference still describes overworld behaviour in **35
  places** across eight pages, heaviest in `misc-functions.md` (11),
  `sprites-and-animation.md` (9) and `discord.md` (6). v0.1 fixed the links and the
  project pages; this is the prose pass, and it wants doing once rather than twice,
  which is why it waited for the branch collapse.

Two things need care. `nonOWScenes` now lists all nine scenes that exist, so the
check in `GameState.cs` can never be false: `lastScene` is dead by construction,
not merely unused. And `GameState` is serialized to `save.gd` with a
`BinaryFormatter`, so removing fields breaks existing saves. `SaveLoad` already
guards that by version, so the same commit has to bump the constant, or the player
gets a raw deserialization exception instead of the readable message.

The scene flow from the title screen stays. Trimming it would change what the player
sees, which is not what this milestone is for, and the menus want designing properly
in v0.3 alongside boss select.

Indicative commits:

```
v0.2.1   collapsed the overworld branches in the game over sequence
v0.2.2   collapsed the overworld branches in sprite handling
v0.2.3   collapsed the overworld branches in the inventory
v0.2.4   collapsed the overworld branches in the text system
v0.2.5   collapsed the remaining overworld branches
v0.2.6   removed the IsOverworld flag
v0.2.7   removed the shop and kept-audio state
v0.2.8   removed the non-overworld scene lists
v0.2.9   removed the map fields from the session save format
v0.2.10  renamed the overworld text box to the name entry text box
v0.2.11  renamed the overworld teardown to a session reset
v0.2.12  simplified the scene flow from the title screen
v0.2.13  removed the overworld notes from the sprite and animation reference
v0.2.14  removed the overworld notes from the misc and general references
v0.2.15  removed the overworld notes from the Discord, text and time references
v0.2.16  updated the README for the stripped engine
```

That commit list is indicative. The shape is what matters: many small commits,
each one a change you could revert on its own.

## v0.3: the boss rush loop

The actual game loop, with programmer art.

- A boss registry: what bosses exist, and what each one needs to load.
- A boss select menu. The existing mod selector (`PregamePlaceholder`, 6 files,
  1688 lines) is the closest thing in the engine and the obvious starting point,
  though selecting a boss is not the same as selecting a mod and it may end up
  replaced rather than adapted.
- Fight, then a result state, then back to select. This is the loop the whole
  game hangs off, so it lands before anything decorative.

Everything here uses placeholder visuals. The point is a loop that runs
end to end, not a loop that looks finished.

## v0.4: persistence and quality of life

- Death count, per boss and overall. The hook is the game-over path rewired in
  v0.1.
- Whatever per-boss records are worth keeping: attempts, best time, no-hit runs.
- A save format for the above. The engine has AlMighty Globals, which
  persist to disk across sessions and are the cheapest place to start, though a
  purpose-built format may be worth it once there is more than a counter.

## v0.5: engine work for bosses

Boss fights push harder on the engine than ordinary encounters. This milestone is
deliberately vague because its content comes from building v0.3 and finding out
what hurts. Likely candidates: bullet pattern performance under load, better wave
composition, and whatever the Lua API makes awkward when a fight gets long.

## Later: art integration

When menu and boss assets arrive, replace the placeholder art. Kept separate on
purpose: an art milestone that is blocked on a delivery should not also be
blocking engine work.

## How versions are cut

Within a milestone, work lands as small, numbered commits (for example `v0.1.1`
through the final `v0.1.x`). The last commit of a milestone marks it complete.
Commit subjects carry exactly one version stamp, and the
[changelog](../../CHANGELOG.md) records what each milestone changed.
