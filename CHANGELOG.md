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

## 0.4 (unreleased)

The records v0.3 wrote but did not show, plus the first-release problems that only
appeared once people could download a build.

### Added

- Deaths per boss and overall, as `boss_<id>_deaths` and `deaths_total`. Counted when the
  game over runs to its end rather than at the moment the player dies, so a boss that
  kills them as a story beat and revives them does not charge a death.
- No-hit clears, as `boss_<id>_nohit`. Set when a boss is beaten without anything taking
  HP off the player; healing and the `Player.Hurt(0)` call used for the invulnerability
  flash both leave it intact. Once set it is never cleared.
- All five records now show on the boss select, sharing the one spare line
  `ModSelect.unity` has: cleared and clean-cleared state, best time to a tenth, tries and
  deaths. A boss the player has never picked shows nothing rather than a row of zeroes.
- An in-fight timer, off by default, with a toggle on the options screen. Display only:
  the clock runs on every fight either way, so turning it off never costs a record and
  turning it on never produces a second set of times that cannot be compared. `Battle.unity`
  has no object for it, so the text is built at runtime from the same prefab Lua's
  `CreateText` uses.

### Changed

- Releases are marked pre-release by the shape of the tag rather than always. A plain
  version tag such as `v0.4.0` publishes as a full release; a tag carrying a semver
  pre-release identifier, `v0.9.0-rc1`, publishes with `--prerelease`.

  v0.3.0 published correctly and then did not appear in the repository's Releases panel,
  because GitHub excludes pre-releases from "latest" and that panel shows the latest
  release. Marking every build before v1.0 a pre-release would have left it empty for
  years. The 0.x version number and the warning at the top of each release's notes are
  the honest signals about maturity.
- The README's version badge was static and had to be edited every milestone. It reads
  the latest release now, with a download counter beside it.
- The splash screen background is black rather than dark grey, so it runs into the
  disclaimer screen instead of stepping to it. The Unity logo stays: the project is on a
  Personal licence, which requires it. A Soulbound logo can sit alongside it once one
  exists, which is recorded against v0.7.

### Fixed

- The error shown when the engine cannot find its `Mods` folder. It read "error in script
  CYF's Startup", asked whether the folder exists, and told the player to press ESC to
  restart, which reloads the title screen, runs the same lookup and returns to the same
  screen. It now names Soulbound, says the likely cause (running the build from inside its
  zip, which Windows allows by unpacking the program alone into a temp folder), says to
  extract the archive, and prints the path it searched from. ESC closes the game, since
  nothing else helps.

  The v0.3.0 zips are correct. `Mods` sits beside the executable in all three.

- Fourteen strings naming the fork that a player or a boss author can reach: the crash
  handler's log pointer, the infinite loop handler, the keybind parse and load errors,
  four item pool warnings, the item box, and four options descriptions. v0.3 cleared the
  documentation and the menus and missed everything that only renders when something goes
  wrong. The save incompatibility error was rewritten rather than renamed, since it ended
  with "thanks for following my fork! ^^".

  Identifiers are deliberately untouched: `CYFException`, `isCYF`, `CYFSwitch`,
  `CYFDiscord`, `CYFRetroMode`, `CYFWindowScale`, `cyfshaders` and the two shader keywords
  are API and storage keys that mods and shaders bind to by name.

### Added

- `Read me first.txt`, shipped in every release zip. Extracting the archive before running
  is the one thing a player has to know and the only failure the engine cannot recover
  from, so it needs saying before they reach an error screen. It also records where saves
  and the output log live.
- A Download section in the README. The only instructions there were for building from
  source in Unity, with no link to the builds.

### Notes

- The release notes extractor keys on the minor version: it takes the tag, drops the
  patch component, and looks for that heading. So tagging `v0.4.1` pulls the `## 0.4`
  section, not a `## 0.4.1` one. That is deliberate, patch releases share their minor's
  notes, but it is surprising if you have not read
  [`release.yml`](.github/workflows/release.yml).

## 0.3 (2026-07-26)

**This is a technical pre-release. There is no game in it yet.**

What works is the loop: name your character, pick one of three placeholder bosses,
fight it, and come back to the list with it marked CLEARED. The bosses are a 10 HP
monster with no sprite and a wave that fires nothing, so a fight is over in seconds.
That is deliberate. v0.3 was about building the loop; v0.6 is the first real boss.

### Running it

Download the zip for your platform, unzip it anywhere, and run `Soulbound-<platform>`.
On macOS, read `How to run Soulbound on Mac.txt` inside the zip first, or Gatekeeper
will refuse to open the app.

Save data lives in `PaperTrail/Soulbound/` under your platform's application data
folder. Nothing from an earlier build carries over: v0.3 renamed the product, which
moved that folder.

Bug reports are welcome. The rest of this section is the engineering detail.

### What this milestone was

The boss rush loop: pick a boss, fight it, come back to the list. That is the loop
the whole game hangs off, so it lands before anything decorative. Alongside it, the
three things that had to happen before a first release: a way to cut one, an engine
that does not call itself somebody else's name, and a product that does not either.

### Added

- The boss registry, `Assets/Mods/Soulbound/Lua/bosses.lua`. An ordered list of
  bosses, one entry per row of the select screen, each naming an encounter script by
  `id`. Adding a boss needs no C# change. Unrecognised keys are ignored, so a portrait
  or a music field can be written before the menu is ready to read it.
- `Assets/Scripts/Menus/BossRegistry.cs` reads it. Because the file is data rather
  than gameplay it runs in a bare MoonSharp sandbox instead of through `ScriptWrapper`,
  which binds a battle API that does not exist outside the Battle scene. A missing
  file, invalid Lua, a missing or duplicate `id`, an `id` with no matching encounter
  script, or an empty list all reach the error screen naming the problem.
- `Assets/Scripts/Save/BossRecords.cs`. Three AlMighty globals per boss:
  `boss_<id>_cleared`, `boss_<id>_attempts` and `boss_<id>_best`. v0.3 shows only the
  cleared marker; v0.4 gives the other two somewhere to appear.
- Every fight is timed, unconditionally. The clock is `Time.realtimeSinceStartup`, so
  `Time.timeScale` cannot distort it from Lua, and it starts after the encounter script
  has run so loading is not counted against the player. There is no toggle: two sets of
  times that cannot be compared, and a way to lose a personal best by forgetting a
  setting, are worse than always measuring.
- `Assets/Scripts/Save/PlayerProfile.cs`. The player's name, stored as the AlMighty
  global `player_name` beside the boss records, so one file holds the whole profile and
  wiping `save.gd` cannot clear the name while leaving the records.
- The boss select sends anyone with no stored name through the existing `EnterName`
  scene first. Both routes off the disclaimer screen end at the boss select, so that one
  check catches every player exactly once. Before this, the default route never asked.
- A "Change name..." row on the options screen.
- Two more placeholder bosses, so the list, paging and selection are exercised by more
  than one row.
- [docs/basics/adding-a-boss.md](docs/basics/adding-a-boss.md) and
  [docs/project/boss-rush-loop.md](docs/project/boss-rush-loop.md).
- `.github/workflows/release.yml`. Pushing a `v*` tag builds Windows, macOS and Linux,
  zips each as `Soulbound-<version>-<platform>.zip`, and publishes a GitHub release with
  the three archives attached. The tag is the only place the version is written: it
  reaches the executable through `versioning: Custom`, `BuildScript.Build` and
  `PlayerSettings.bundleVersion`.
- Release notes are pulled from the section of this changelog matching the tag. If no
  section matches, the release still publishes with a pointer here instead.
- A releasing section in [docs/project/building.md](docs/project/building.md).

### Changed

- `productName` is `Soulbound` and `companyName` is `PaperTrail`. Both feed
  `Application.persistentDataPath`, so this moves `save.gd`, `AlMightySave.gd` and the
  output log. Done once, immediately before the first tag: doing it after a release
  would orphan real save files. `GlobalControls.SaveVersion` goes to 2 so a save copied
  across from the old path gets a readable message.
- The mod selector is the boss select. It paged through mod folders found by scanning
  the disk; it pages through the registry. The title is the boss name, the line under it
  the subtitle, the scrolling list jumps straight to a boss, and Confirm starts the
  fight. `ModSelect.unity` is untouched: `SelectOMatic` binds to it through inspector
  fields, so changing what it lists needed no scene edit.
- Portraits come from `Sprites/Bosses/<id>.png`, falling through to the engine's black
  background until art exists.
- `DiscordControls.StartModSelect` became `StartBossSelect`, and the two presence
  strings a player sees went from "Selecting a Mod" and "Playing Mod: X" to "Choosing a
  boss" and "Fighting X".
- The options list is stacked and hover-tested from the buttons' real positions rather
  than from fixed scene coordinates and a ladder of hardcoded bands, so an option can be
  retired without leaving a hole.
- `ControlPanel.BasisName` was `Rhenao`, upstream's joke default, visible in the battle
  stats bar and in every `[name]` substitution. It is `Soul`, and since the boss select
  asks for a name it should never be seen.
- `build.yml` and its three jobs were named after the fork, and its artifacts and
  staging folder were `CreateYourFrisk`. `release.yml` already produced `Soulbound`
  zips, so the two disagreed.
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

### Removed

- **Safe mode**, a swear filter: a `ControlPanel` field, two call sites, fifteen lines
  of options UI, and a commented-out block of flee texts that was the only place it
  filtered anything. The `safe` Lua global goes with it, which is an API break for a mod
  reading it. There are no such mods, and it never gets cheaper.
- **Crate Your Frisk**, a joke reskin, across 21 files: the options toggle, the garbled
  alternative for every string on the disclaimer, title, name entry, boss select,
  options and keybinding screens, the crate logos, the meowing ACT, the Temmie item
  message, the 24-line MERCY monologue, the negative damage on a hit, `Temmify.cs`, and
  nine sprites including `tiembt_0.png`, which nothing referenced at all. It was in this
  milestone specifically because the options screen offered a toggle labelled Crate Your
  Frisk, which is the branding the rest of v0.3 spent eleven commits removing.
- Mod browsing from the selection screen: the four-level deep search, the folder
  hierarchy it built, the two passes that sorted it, the `ModPage` model behind them, and
  the encounter sub-list. There is one mod and the registry is the list.

### Not done yet

- **Retro mode.** 54 lines across 18 files, and unlike safe and crate modes it changes
  gameplay semantics: enemy HP clamping, wave argument parsing, state transition rules,
  projectile positioning and rotation, sprite active semantics, and script
  call-existence checks. It is the same shape of problem `IsOverworld` was and gets the
  same treatment in its own milestone. Doing it alongside cosmetic work is how a working
  battle path gets broken quietly.
- **The timing display.** v0.3 records best times and attempts and shows neither.
- **A purpose-built boss select.** The screen is the repurposed mod selector, which is
  why its objects are still named `ModTitle`, `EncounterCount` and `encounterBox`.
  Renaming them means editing the scene, which is v0.7 work alongside the art. The
  retired options rows are hidden at runtime for the same reason.

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
