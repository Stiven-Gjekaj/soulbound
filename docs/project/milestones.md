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

## v0.1: remove the overworld

Strip the feature this game does not use, and rewire the paths that assumed it.

The overworld is not a module that can simply be deleted. It is the feature, plus
its Lua API, plus a set of assumptions baked into the battle flow:

| Piece | Size |
| ----- | ---- |
| `Assets/Scripts/Overworld` | 18 files, 5580 lines |
| `Assets/Scripts/Lua/CLRBindings/Overworld` | 6 files, 1088 lines |
| `Assets/Tiled2Unity` | 53 files, 564 KB |
| Overworld scenes | `TransitionOverworld`, `Shop`, `SpecialAnnouncement` |
| Overworld documentation | 11 of the 46 pages |

The integration surface is smaller than it looks. Outside the overworld feature
itself, 42 references across 13 files touch it, and most are null-guarded statics
or branches only reached when a battle was entered from a map. The ones that
matter:

- `Battle/UIController.cs` returns the player to the overworld when a battle
  ends. In a boss rush it returns to boss select.
- `Battle/GameOverBehavior.cs` sends the player back to their save point on
  death, instantiating a teleport prefab to do it. A boss rush needs retry or
  return to select. This is the same code path a death counter hooks into, which
  is why the two belong in adjacent milestones rather than the same one.
- `Battle/UIController.cs` and `Battle/EnemyEncounter.cs` carry music across the
  overworld boundary, the CORE-style "keep the music playing" feature. Without an
  overworld this becomes dead weight.
- `Device/GlobalControls.cs` holds `nonOWScenes` and the overworld pause menu.

Removing it makes every later milestone smaller: less to reason about, less to
document, less to keep compiling. Do it before building on top, not after.

The risk is real and worth naming: this touches the battle path, and the battle
path is the game. The mitigation is that CI now builds all three platforms, so a
break is caught in about three minutes rather than discovered by hand.

## v0.2: the boss rush loop

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

## v0.3: persistence and quality of life

- Death count, per boss and overall. The hook is the game-over path rewired in
  v0.1.
- Whatever per-boss records are worth keeping: attempts, best time, no-hit runs.
- A save format for the above. Create Your Frisk has AlMighty Globals, which
  persist to disk across sessions and are the cheapest place to start, though a
  purpose-built format may be worth it once there is more than a counter.

## v0.4: engine work for bosses

Boss fights push harder on the engine than ordinary encounters. This milestone is
deliberately vague because its content comes from building v0.2 and finding out
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
