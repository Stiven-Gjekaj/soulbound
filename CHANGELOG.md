<div align="center">
  <a href="README.md"><b>Soulbound</b></a>
</div>

# Changelog

All notable changes to Soulbound are recorded here. The format is based on
Keep a Changelog (https://keepachangelog.com), and the project aims to follow
semantic versioning.

Version stamps appear in every commit subject as `v0.X.N`, so the history reads
as a sequence of small, individually described changes rather than a few large
ones.

## 0.1 (2026-07-26)

Removes the overworld. Soulbound is a boss rush: you pick a boss from a menu and
fight it, and there are no maps, events, cutscenes or shops. Roughly a third of
Create Your Frisk existed to support them.

This is the first of two milestones on the overworld. v0.1 deletes the feature and
leaves `UnitaleUtil.IsOverworld` as a shim returning `false`, so every branch that
consults it collapses to the battle path on its own. v0.2 walks those 48 branches
and deletes the dead half of each. Splitting it that way keeps the destructive work
and the subtle logic changes in separate diffs.

### Changed

- A battle now always returns to the mod selector. It used to return the player to
  the map they came from unless the battle was started from the mod selector. The
  mod selector stands in for the boss select screen that v0.3 builds.
- Death restarts instead of respawning. The game over sequence used to reload the
  save, instantiate a teleport prefab, and drop the player back at their last save
  point on the map they died on.
- The title screen and name entry lead to the mod selector, not into a map.
- `Assets/Scripts/Overworld` is now `Assets/Scripts/Save`. Six of its 18 files were
  never overworld code. `SaveLoad`, `GameState` and `PermanentGameState` stayed
  where they were under the folder's new name; `Title`, `IntroManager` and
  `EnterNameScript` moved to `Assets/Scripts/PregamePlaceholder` with the other
  menu screens.
- `UnitaleUtil.IsOverworld` is a constant `false`.
- `UnitaleUtil.ExitOverworld` kept only its non-overworld teardown: music channels,
  session globals, the inventory, and the player character.
- The engine is 111 C# files and 19,505 lines, down from 129 and 25,334.

### Removed

- `Assets/Scripts/Overworld`: `PlayerOverworld`, `EventManager`, `ShopScript`,
  `TransitionOverworld`, `ItemBoxUI`, `CYFAnimator`, `TPHandler`, `Fading`,
  `EventOW`, `MapInfos`, `MapLoader` and `SpecialAnnouncementScript`. 12 files,
  4,534 lines.
- `Assets/Scripts/Lua/CLRBindings/Overworld`: the `Event`, `General`, `Player`,
  `Screen`, `Map` and `Inventory` overworld Lua objects, their registrations in
  `LuaScriptBinder`, and the `FPlayer`, `FEvent`, `FGeneral`, `FInventory`,
  `FScreen` and `FMap` globals. 6 files, 1,088 lines.
- The `TransitionOverworld`, `Shop` and `SpecialAnnouncement` scenes, and their
  entries in `EditorBuildSettings.asset`. Nine scenes remain.
- Eight prefabs: `Canvas OW`, `Main Camera OW`, `Player`, `Background 1`, `Event1`,
  `ImageEvent`, `Save`, `TP On-the-fly`, and the empty `Prefabs/Maps` folder.
  `TextManager OW.prefab` was kept: `EnterName` instantiates it.
- Six sprite folders from `Assets/Default/Sprites`: `FriskUT`, `AsrielOW`,
  `CharaOW`, `MonsterKidOW`, `BoosterOW` and `SavePoint`. About 1.2 MB.
- `Assets/Tiled2Unity`, the Tiled map importer. 53 files, 564 KB. v0.0 kept it on
  the reasoning that it would be needed to author future maps; a boss rush has no
  maps.
- The overworld pause menu, the overworld camera shift, the overworld rich
  presence, the kept-audio channel that carried music across the overworld
  boundary, and `UnitaleUtil.MapCorrespondanceList`.
- The 11 overworld documentation pages and the two images only they used. 35 pages
  remain.

## 0.0 (2026-07-26)

The first version. Turns an unmodified Create Your Frisk v0.6.6 LTS 3 snapshot
into a clean, documented base to build a game on. No gameplay is implemented.

### Added

- A documentation set of 46 Markdown pages under `docs/`, converted by hand from
  the 644 KB Bootstrap website that Create Your Frisk shipped. It covers the
  whole engine: text commands, game events, every Lua object, projectiles,
  pixel-perfect collision, sprites and animation, shaders, and the overworld,
  plus the key, item, and dialog bubble references. Content is preserved; only
  the presentation changed.
- Three project pages written for this repository:
  [repository layout](docs/project/repository-layout.md),
  [building](docs/project/building.md), and
  [engine architecture](docs/project/engine-architecture.md).
- `Assets/Mods/Soulbound/`, the mod folder the game will live in, scaffolded with
  the standard Create Your Frisk layout and a placeholder encounter, monster, and
  wave. Without at least one mod containing a non-`@` encounter script, the mod
  selector stops with "Your mod folder is empty!", so the placeholder exists to
  keep the engine bootable. The placeholder monster draws the `empty` sprite from
  `Assets/Default/Sprites`, so the mod ships no art of its own.
- A `tools/` directory for build and content tooling.
- Community documents: a contributing guide, a code of conduct, a security
  policy, a support page, and project terms.

### Changed

- The Unity project moved from `engine/` to the repository root. This restored
  the root-relative paths that `.gitignore` and `.github/workflows/build.yml`
  already assumed.
- `Build.py` and the CI workflow now copy `docs/` into builds, in place of the
  removed website folder.
- `README.md` and the bug report template were rewritten for this project rather
  than the upstream engine.
- `actions/checkout` moved from v2 to v5, which runs on Node 24.
- `game-ci/unity-builder` moved from v2 to v4. The v2 action used a legacy
  licensing module that depended on an activation file flow GameCI has since
  deprecated, which left it with no working way to activate a licence. This was
  the change that got CI building.

### Removed

- Roughly 37 MB of upstream example content: the six example mods
  (`@0.5.0_SEE_CRATE`, `@OverWorld Test`, `Examples`, `Examples 2`, `RTLGeno`,
  `Encounter Skeleton`), eleven demo overworld scenes, the demo Tiled2Unity map
  data, and the demo map prefabs. The Tiled2Unity importer itself was kept, since
  it is needed to author maps.
- `Assets/Scripts/Tests`, six scripts with no scene or code references.
  `Assets/Scripts/Why` was kept: `DogGyrator.cs` is used by `Error.unity` and
  `Temmify.cs` by five engine files.
- The Bootstrap documentation website and the GitHub Pages workflow that
  published it.
- The upstream Discord notification workflows, which targeted the Create Your
  Frisk Discord and could not work here.
- The Unity activation workflow. Its core action was deprecated upstream and now
  fails on purpose; licences are obtained locally through Unity Hub instead. See
  [docs/project/building.md](docs/project/building.md).

### Fixed

- Four places in the engine named the removed demo content directly and were
  repaired: the scene list in `ProjectSettings/EditorBuildSettings.asset`,
  `AddKeysToMapCorrespondanceList()` in `Assets/Scripts/Util/UnitaleUtil.cs`, and
  the hardcoded `FirstLevelToLoad` and `ModFolder` values in
  `Main Camera OW.prefab`, `Canvas OW.prefab`, and `Background 1.prefab`.

### Notes

- The repository dropped from 3841 to 2311 tracked files, and from about 101 MB
  to 65 MB.
- Scenes are loaded by name from C#, never by build index, which is what made
  removing the demo maps safe. Twelve engine scenes remain.
- The overworld has no first map yet, so `FirstLevelToLoad` is empty on both
  overworld prefabs. `TransitionOverworld` guards this with
  `FileLoader.SceneExists()` and shows a readable error, so it degrades cleanly
  until a map exists.
- CI builds Windows, macOS, and Linux successfully, which is the first
  confirmation that the cleanup compiles.
