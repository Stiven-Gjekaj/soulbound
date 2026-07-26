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

## 0.3 (unreleased)

The boss rush loop, plus the two things that had to happen before a first release:
a way to cut one, and an engine that does not call itself somebody else's name.

### Added

- `.github/workflows/release.yml`. Pushing a `v*` tag builds Windows, macOS and Linux,
  zips each as `Soulbound-<version>-<platform>.zip`, and publishes a GitHub release with
  the three archives attached. The tag is the only place the version is written: it
  reaches the executable through `versioning: Custom`, `BuildScript.Build` and
  `PlayerSettings.bundleVersion`.
- Release notes are pulled from the section of this changelog matching the tag. If no
  section matches, the release still publishes with a pointer here instead.
- A releasing section in [docs/project/building.md](docs/project/building.md).

### Changed

- The documentation describes the Soulbound engine rather than Create Your Frisk. Gone
  are 273 `<CYF>` markers, 5 `<0.2.1a>` markers and roughly 150 prose references across
  35 pages. Attribution, the origin note, and the passages where Unitale genuinely is the
  subject all stay. The project is GPLv3 by inheritance, and the licence requires the
  attribution.
- `docs/how-to-read.md` lost the version-marker section and its example was rebuilt: it
  demonstrated `Screen.DispImg`, an overworld function deleted in v0.1.
- The Unity editor menu for building shader AssetBundles is `Soulbound`, not
  `Create Your Frisk`.
- `How to use CYF and add mods (Mac).txt` became `How to run Soulbound on Mac.txt`,
  rewritten so it no longer hardcodes an application name.
- `CONTRIBUTING.md` listed four engine files that name content directly. Three of them
  were deleted in v0.1 and v0.2. It lists the one that remains.

### Not done yet

- `productName` and `companyName` still say Create Your Frisk. They determine
  `Application.persistentDataPath`, so changing them relocates save files. That has to
  land before the first tag, and it needs a company name.

## 0.2 (2026-07-26)

Removes what the overworld strip left behind. Nothing here changes what the game
does, because every branch deleted was already unreachable: v0.1 left
`UnitaleUtil.IsOverworld` as a shim returning `false`, and this milestone walks the
branches that consulted it and deletes the dead half of each.

### Changed

- `GlobalControls.OverworldVersion`, a version string, became `GlobalControls.SaveVersion`,
  an integer. The old check compared version strings ordinally, which would have accepted a
  pre-v0.2 save and then failed to read it. `SaveLoad` now refuses any save written to an
  older format with a readable message.
- `UnitaleUtil.ExitOverworld` became `ResetSession`. Since v0.1 it only cleared music
  channels, session globals, the inventory and the player character.
- `GlobalControls.canTransOW` became `escapableScenes`, and
  `GlobalControls.overworldTimestamp` became `sessionTimestamp`.
- `TextManager OW.prefab` became `TextManager Name.prefab`, keeping its GUID so the three
  scenes that use it are unaffected.
- The disclaimer screen said "Press Menu to go to the Overworld". It now says "Title
  Screen", which is where that key has actually led since v0.1.
- The options screen described the save file as "the save file used for CYF's Overworld".
  It now describes what the file holds.
- The engine is 111 C# files and 19,283 lines, down from 19,505.

### Removed

- All 40 remaining `UnitaleUtil.IsOverworld` branches and the flag itself:
  `GameOverBehavior` (18), `SpriteUtil` (8), `Inventory` (5), `TextManager` (3),
  `LuaScriptBinder` (2), and one each in `LuaProjectile`, `LuaTextManager` and
  `GlobalControls`.
- `GlobalControls.isInShop`, `nonOWScenes`, `realName`, and the `GameMapData`, `EventData`
  and `TempGameMapData` dictionaries. `EventData` was never read or written at all.
- The map fields on `GameState`: `lastScene`, `mapInfos`, `tempMapInfos`, and the
  `MapData`, `TempMapData`, `EventInfos` and `Vect` structs. This changes the save format,
  which is why `SaveVersion` exists.
- `UnitaleUtil.VectToVector` and `VectorToVect`, which had no callers.
- The `PlayerPosX`, `PlayerPosY`, `PlayerPosZ` and `PlayerMap` session globals. These held
  overworld coordinates. They were readable from Lua with `GetRealGlobal`, so this is a
  small API change.
- The four `GameOverBehavior` fields that fell dead with the branches:
  `gameOverContainerOw`, `canvasOW`, `canvasTwo` and `utHeart`.
- The two `TextManager` branches matching on the name `"TextManager OW"`. All ten scene
  instances of that prefab override the object name, so neither branch could ever run.
- The title screen's last-map line, which had been showing a value nothing set since v0.1.
- Dead `Canvas OW` and `Canvas Two` lookups in `ErrorDisplay` and `SelectOMatic`.

### Documentation

- 35 overworld references corrected across eight API pages, heaviest in
  `misc-functions.md` (11), `sprites-and-animation.md` (9) and `discord.md` (6). Several
  described behaviour that genuinely changed in v0.1, such as `Misc.ResetCamera` and the
  Discord presence states.
- `sprite.z` and `sprite.absz` were documented as working "in the Overworld only". They are
  plain transform Z accessors and still work; the pages now say battle rendering orders by
  layer, so they rarely change what you see.
- The engine architecture and repository layout pages record the v0.2 state, including why
  the save path is kept even though only name entry calls `SaveLoad.Save()`.

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
