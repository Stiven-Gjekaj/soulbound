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
    Scenes/                the twelve engine scenes
    Scripts/               engine source, 135 C# files
    Sprites/               menu and title sprites
    Tiled2Unity/           Tiled map importer
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
| `Lua/Events` | overworld event scripts, added when the overworld starts |
| `Sprites` | sprites, with `Sprites/UI` for interface art |
| `Sounds` | sound effects |
| `Audio` | music |

There is deliberately no top-level `assets/` directory. Windows and macOS use
case-insensitive filesystems, so a root `assets/` and Unity's `Assets/` would be the same
directory and would break checkouts.

## What v0.0 removed

v0.0 started from an unmodified Create Your Frisk v0.6.6 LTS 3 snapshot and removed the
upstream example content:

- Six example mods: `@0.5.0_SEE_CRATE`, `@OverWorld Test`, `Examples`, `Examples 2`,
  `RTLGeno`, `Encounter Skeleton`
- Eleven demo overworld scenes: `test`, `test2`, `test4`, `test5`, `test-1`, `BasicOWScene`,
  `newhome1`, `newhome2`, `newhome3`, `Void`, `Secret`
- The demo Tiled2Unity map data: meshes, imported XML, textures, prefabs and the
  map-specific materials. The importer scripts, shaders and generic materials stayed.
- `Assets/Resources/Prefabs/Maps`, the demo map prefabs
- `Assets/Scripts/Tests`, six scripts with no scene or code references
- The Bootstrap documentation website, replaced by this `docs` folder

`Assets/Scripts/Why` was kept: `DogGyrator.cs` is used by `Error.unity` and `Temmify.cs` by
five engine files.

## Engine files that reference content

Four places name content directly. If you add or remove maps and mods, check them:

- `ProjectSettings/EditorBuildSettings.asset` lists every scene included in a build
- `Assets/Scripts/Util/UnitaleUtil.cs`, `AddKeysToMapCorrespondanceList()` maps scene names
  to the save point names shown to the player
- `Assets/Resources/Prefabs/Main Camera OW.prefab`, `FirstLevelToLoad`
- `Assets/Resources/Prefabs/Canvas OW.prefab`, `FirstLevelToLoad`, `LevelToLoad`,
  `ModFolder`, `FirstModFolder`

The two prefabs currently point at the `Soulbound` mod with no first map set, because the
game has no overworld map yet.

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
