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

Delete the feature. Keep the engine compiling and the battle path working.

| Piece | Size |
| ----- | ---- |
| `Assets/Scripts/Overworld` | 12 of its 18 files, 4534 lines |
| `Assets/Scripts/Lua/CLRBindings/Overworld` | 6 files, 1088 lines |
| `Assets/Tiled2Unity` | 53 files, 564 KB |
| Overworld scenes | `TransitionOverworld`, `Shop`, `SpecialAnnouncement` |
| Overworld prefabs | `Canvas OW`, `Main Camera OW`, `Player`, `Background 1`, `Event1`, `ImageEvent`, `Save`, `TP On-the-fly`, `Maps/` |
| Overworld sprites | `FriskUT`, `AsrielOW`, `CharaOW`, `MonsterKidOW`, `BoosterOW`, `SavePoint` |
| Overworld documentation | 11 of the 46 pages |

`Assets/Scripts/Overworld` is a mixed folder, not an overworld folder. Six of its
files are kept and moved out:

| File | Lines | Why it stays |
| ---- | ----- | ------------ |
| `SaveLoad.cs` | 100 | The save system, and the AlMighty globals v0.4 builds on |
| `PermanentGameState.cs` | 56 | The persistent globals format |
| `GameState.cs` | 164 | The session save format. Gutted of its map fields in v0.2 |
| `Title.cs` | 235 | Backs `TitleScreen` |
| `IntroManager.cs` | 186 | Backs `Intro` |
| `EnterNameScript.cs` | 305 | Backs `EnterName` |

The first three move to `Assets/Scripts/Save`, the last three join the other menu
screens in `Assets/Scripts/PregamePlaceholder`. For the same reason
`TextManager OW.prefab` is kept: `EnterName` instantiates it, and it carries
`EnterNameScript`. Its misleading name is a v0.2 rename.

Six references genuinely point at overworld types and must be rewired rather
than left to a constant:

- `Battle/UIController.cs` returns the player to the overworld when a battle
  ends. It returns to the mod selector instead, until v0.3 gives it a boss
  select to return to.
- `Battle/GameOverBehavior.cs` sends the player back to a save point on death,
  instantiating a teleport prefab to do it. Death restarts without one.
- `Battle/UIController.cs` and `Battle/EnemyEncounter.cs` carry music across the
  overworld boundary. That channel goes.
- `Device/GlobalControls.cs` opens the overworld pause menu.
- `Title.cs` and `EnterNameScript.cs` load the `TransitionOverworld` scene. Both
  are kept scripts loading a deleted scene, so both go to the mod selector.
- `SaveLoad.cs` asks the event manager to snapshot map state before saving.

Two rules order the work. Assets go before scripts, because deleting a scene
never breaks compilation but a scene holding the GUID of a deleted script shows
as a missing component. References go before definitions, because C# compiles
all or nothing, so a type can only be deleted once nothing names it.

Inside the folder the scripts form a reference cycle: `PlayerOverworld`,
`EventManager` and `TransitionOverworld` name each other, and `EventManager`
holds instances of all six Lua bindings that call back into it. There is no
leaves-first order, so the cycle is peeled with small commits that cut a group
of edges before the next group of files is deleted.

## v0.2: clean up after the overworld

Remove what the strip left behind. Nothing here changes behavior, because every
branch being deleted is already unreachable.

- The 48 `IsOverworld` branches, by file: `GameOverBehavior` (18), `SpriteUtil`
  (8), `Inventory` (6), `TextManager` (5), `Misc` (3), and eight others with one
  or two each.
- `isInShop`, `audioKept`, `nonOWScenes`, `canTransOW`, and `IsOverworld` itself.
- The map fields still on `GameState`: `mapInfos`, `tempMapInfos`, `lastScene`,
  and the `MapData`, `TempMapData` and `EventInfos` structs behind them.
- `TextManager OW.prefab` and the three places `TextManager.cs` matches on its
  name. It is the name entry text box, and it should say so.
- The scene flow from the title screen, which currently routes through name entry
  toward a transition that no longer exists.
- Documentation: `engine-architecture.md` and `repository-layout.md` both
  describe the overworld at length, and the docs index is organised around an
  engine that has one.

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
v0.2.11  simplified the scene flow from the title screen
v0.2.12  rewrote the engine architecture doc for a battle-only engine
v0.2.13  rewrote the repository layout doc
v0.2.14  reorganised the documentation index around the battle API
v0.2.15  updated the README for the stripped engine
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
- A save format for the above. Create Your Frisk has AlMighty Globals, which
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
