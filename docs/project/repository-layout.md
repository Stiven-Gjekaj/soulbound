# Repository layout

The repository root is the Unity project root. Opening this folder in Unity 2018.4.36f1
opens the game.

```
/
  Assets/                  Unity assets, the engine and the game
    Default/               engine sprites, sounds, music and shaders
    Editor/                editor-only tooling, shaders, the CI build script
    Fonts/                 UI fonts
    Mods/                  game content, loaded at runtime
      @Title/              engine title screen, a hard dependency
      Soulbound/           the game
    Plugins/               MoonSharp, Discord SDK, user32.dll
    Resources/             prefabs, sprites and audio loaded by name at runtime
    Scenes/                the nine engine scenes
    Scripts/               engine source, 111 C# files
    Sprites/               menu and title sprites
  Packages/                Unity package manifest
  ProjectSettings/         Unity project configuration
  docs/                    this documentation
  tools/                   build and content tooling
  Build.py                 local multi-target build script
  LICENSE                  GNU GPL v3
  MOONSHARP_LICENSE        MoonSharp BSD license
```

## Where game content goes

All gameplay content lives in `Assets/Mods/Soulbound/`, because that is where Create Your
Frisk loads it from at runtime. The layout inside that folder is described in
`Assets/Mods/Soulbound/README.md`:

| Folder | Contents |
| --- | --- |
| `Lua/Encounters` | one script per encounter |
| `Lua/Monsters` | one script per monster, including bosses |
| `Lua/Waves` | bullet patterns |
| `Lua/Libraries` | shared Lua modules |
| `Sprites` | sprites, with `Sprites/UI` for interface art |
| `Sounds` | sound effects |
| `Audio` | music |

There is deliberately no top-level `assets/` directory. Windows and macOS use
case-insensitive filesystems, so a root `assets/` and Unity's `Assets/` would be the same
directory and would break checkouts.

## Sprites live in three places, and only one of them cares about import settings

Which folder a PNG goes in decides how it reaches the screen, and the three routes are not
interchangeable.

| Folder | Reached by | Import settings |
| --- | --- | --- |
| `Assets/Mods/<mod>/Sprites` | filename, at runtime | ignored |
| `Assets/Default/Sprites` | filename, at runtime, when a mod has no file of that name | ignored |
| `Assets/Sprites` | a scene or prefab, by GUID | **they are the art** |

The first two go through `SpriteUtil.FromFile`, which reads the PNG bytes itself and sets
`FilterMode.Point`, `TextureWrapMode.Clamp`, a centred pivot and 100 pixels per unit in code.
Unity's importer never touches them, and `Build.py` strips the `.meta` files out of the copies
it ships, so a `.meta` beside a mod sprite is editor bookkeeping and nothing more. Drop the PNG
in and reference it by name.

`Assets/Sprites` is the opposite. Menu and title art is referenced by the scenes that draw it,
so Unity imports it and the `.meta` is what decides how it looks. **Pixel art put here needs
`filterMode: 0`**, which is Point, or it arrives on screen blurred and nothing in the engine
will correct it.

## What v0.0 and v0.1 removed

v0.0 started from an unmodified Create Your Frisk v0.6.6 LTS 3 snapshot and removed the
upstream example content:

- Six example mods: `@0.5.0_SEE_CRATE`, `@OverWorld Test`, `Examples`, `Examples 2`,
  `RTLGeno`, `Encounter Skeleton`
- Eleven demo overworld scenes: `test`, `test2`, `test4`, `test5`, `test-1`, `BasicOWScene`,
  `newhome1`, `newhome2`, `newhome3`, `Void`, `Secret`
- The demo Tiled2Unity map data: meshes, imported XML, textures, prefabs and the
  map-specific materials
- `Assets/Scripts/Tests`, six scripts with no scene or code references
- The Bootstrap documentation website, replaced by this `docs` folder

v0.1 removed the overworld itself, because this game does not have one:

- 12 of the 18 files in `Assets/Scripts/Overworld`, and all six overworld Lua bindings.
  The folder is now `Assets/Scripts/Save`, holding the three save files that were never
  overworld code. `Title`, `IntroManager` and `EnterNameScript` moved to
  `Assets/Scripts/PregamePlaceholder` with the other menu screens
- The `TransitionOverworld`, `Shop` and `SpecialAnnouncement` scenes
- Eight overworld prefabs and the six overworld sprite folders
- `Assets/Tiled2Unity`, the map importer v0.0 kept on the assumption that maps were coming
- The 11 overworld documentation pages

v0.2 removed what the strip left behind. Nothing there changed behaviour, because every
branch it deleted was already unreachable:

- The 40 `UnitaleUtil.IsOverworld` branches and the flag itself, plus `isInShop`,
  `nonOWScenes`, `GlobalControls.realName` and the three map-data dictionaries
- The map fields and structs on `GameState`, which shrank the save format. The old
  `OverworldVersion` string became an integer `SaveVersion`, so a save written by an older
  build is now refused with a readable message
- `TextManager OW.prefab`, renamed to `TextManager Name.prefab`, and the two code branches
  that matched on the old name. Every scene instance overrode that name, so neither branch
  could ever run
- The disclaimer screen's "Press Menu to go to the Overworld" prompt, which had been
  pointing at a feature that no longer existed since v0.1

`Assets/Scripts/Why` was kept for `DogGyrator.cs`, which `Error.unity` uses. Its other
file, `Temmify.cs`, went with Crate Your Frisk in v0.3.

## Engine files that reference content

One place names content directly: `ProjectSettings/EditorBuildSettings.asset` lists every
scene included in a build. Scenes are loaded by name from C#, so that file's order does not
matter but its contents do. The three prefabs that used to name a starting map and mod went
with the overworld in v0.1.

## Mods and .gitignore

`.gitignore` excludes `/Assets/Mods/*` and then allowlists the mods that belong to this
project, so third-party mods dropped into the folder for testing are never committed by
accident:

```
/Assets/Mods/*

!/Assets/Mods/@Title
!/Assets/Mods/@Title.meta
!/Assets/Mods/Soulbound
!/Assets/Mods/Soulbound.meta
```

Add a matching pair of lines for any new mod folder this project owns.
