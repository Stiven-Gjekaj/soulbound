# Engine architecture

This page describes how the Soulbound engine is put together, so you know which parts to
touch and which to leave alone. It began as Create Your Frisk v0.6.6 LTS 3, itself a fork
of Unitale, and has diverged since.

Unity 2018.4.36f1, C#, with MoonSharp as the Lua interpreter. 111 C# files under
`Assets/Scripts`.

## Scene flow

Nine scenes ship in the build. Every one of them is loaded **by name** from C#, never by
build index, so reordering `EditorBuildSettings.asset` is safe but renaming a scene is not.

```
Disclaimer --[Menu]----> Intro -> TitleScreen -> EnterName --+
     |                                                       |
     +--[Confirm]-----------------------------------------> ModSelect -> Battle
                                                             |
                                                             +-> Options -> KeybindSettings
```

| Scene | Role | Loaded from |
| --- | --- | --- |
| `Disclaimer` | entry point, always start play mode here | `SelectOMatic`, `GlobalControls` |
| `Intro` | the intro sequence | `DisclaimerScript` |
| `TitleScreen` | title screen | `Title`, `EnterNameScript` |
| `EnterName` | name entry | `Title` |
| `ModSelect` | the boss select, still named for the mod picker it was | `DisclaimerScript`, `OptionsScript`, `UIController`, `GameOverBehavior`, `Title`, `EnterNameScript` |
| `Options` | options menu | `SelectOMatic`, `KeybindSettings` |
| `KeybindSettings` | key rebinding | `OptionsScript` |
| `Battle` | the encounter | `SelectOMatic` |
| `Error` | the Lua error screen | `UnitaleUtil` |

Both routes end at the boss select, and every battle returns to it, win or lose. That is
the whole loop: pick a boss, fight it, come back to the list.

There is no longer a mode flag. Up to v0.1 the engine asked `UnitaleUtil.IsOverworld` in 40
places to decide whether it was in a battle or on a map; v0.2 removed the flag and every
branch that consulted it. Battle is the only mode.

## Mod loading

`Assets/Scripts/Lua/FileLoader.cs` is the single place that resolves paths.

- `PathToModFile(name)` resolves inside the active mod, `Assets/Mods/<ModFolder>/`
- `PathToDefaultFile(name)` resolves inside `Assets/Default/`

Lookups check the mod folder first and fall back to `Default`. That is why a mod can
override `Sounds/hurtsound.wav` just by shipping a file at the same relative path, and why
the placeholder monster can use the `empty` sprite without shipping any art.

`StaticInits.MODFOLDER` holds the active mod, and since v0.3 it is always
`StaticInits.GAME_MODFOLDER`, the constant `"Soulbound"`. Up to v0.2 the selection screen
searched `Assets/Mods` four levels deep for anything that looked like a mod; that search,
the folder hierarchy it built and the two passes that sorted it are gone.

## The boss select

`Assets/Scripts/Menus/BossRegistry.cs` reads `Assets/Mods/Soulbound/Lua/bosses.lua` into an
ordered `List<BossEntry>`. That list is what the select screen shows, one row per boss, and
the `id` on each entry names the encounter script the fight loads.

The registry is data, not gameplay, so it runs in a bare MoonSharp sandbox
(`CoreModules.Preset_HardSandbox`) rather than through `ScriptWrapper`. `ScriptWrapper`
binds the battle API, and most of that API is null outside the Battle scene.

`SelectOMatic` reloads the registry every time the screen opens, so editing `bosses.lua`
does not need a restart. Everything it validates, and what happens when a check fails, is
in [Adding a boss](../basics/adding-a-boss.md).

`SelectOMatic` binds to `ModSelect.unity` entirely through inspector fields, so changing
what it lists needs no scene edit. That is why the screen is still built out of objects
named `ModTitle`, `EncounterCount` and `encounterBox`: renaming them would mean editing the
scene, and a purpose-built boss select is v0.7 work, to be done alongside the art.

Starting a fight is four lines, at the end of `SelectOMatic.LaunchBoss`:

```csharp
StaticInits.InitAll(StaticInits.MODFOLDER, true);
GlobalControls.isInFight = true;
DiscordControls.StartBattle(boss.name);
SceneManager.LoadScene("Battle");
```

Where the record and the clock hook into that, and what they store, is in
[The boss rush loop](boss-rush-loop.md).

## The `@Title` dependency

`Assets/Mods/@Title` is not optional and not example content. Five engine files reference the
literal string `"@Title"`:

- `Assets/Scripts/Util/StaticInits.cs`, `EDITOR_MODFOLDER`
- `Assets/Scripts/Util/UnitaleUtil.cs`, `InitAll("@Title")`
- `Assets/Scripts/PregamePlaceholder/Title.cs`
- `Assets/Scripts/PregamePlaceholder/IntroManager.cs`
- `Assets/Scripts/Device/GlobalControls.cs`

It supplies the intro images, the title sprites and the menu music.

## The Lua binding layer

`Assets/Scripts/Lua` is where C# meets Lua.

- `LuaScriptBinder.cs` builds a MoonSharp script instance and injects the globals every
  script gets
- `CLRBindings/` holds one class per Lua object: `LuaPlayerStatus`, `LuaInputBinding`,
  `LuaTextManager`, `LuaSpriteController`, `LuaProjectile`, `LuaArenaStatus`
- `StaticRegistries/` caches loaded assets: `SpriteRegistry`, `AudioClipRegistry`,
  `ShaderRegistry`, `FontRegistry`

Every function documented under [API](../api/text-commands.md) is a method on one of the
`CLRBindings` classes. If a Lua call errors, `UnitaleUtil.DisplayLuaError` routes to the
`Error` scene, which is why that scene must stay in the build.

Globals come in three lifetimes:

| Kind | Lifetime | Set with |
| --- | --- | --- |
| Globals | the current encounter | `SetGlobal` |
| Real Globals | the session, across battles | `SetRealGlobal` |
| AlMighty Globals | forever, written to disk immediately | `SetAlMightyGlobal` |

Real and AlMighty globals are shared across mods, so pick names that will not collide.

## Battle flow

`Assets/Scripts/Battle` drives the encounter. `EnemyEncounter.cs` reads the encounter script
and requires `encountertext`, `enemies` and `enemypositions`; `LoadEnemiesAndPositions()`
errors out if `enemies` is missing or if there are more enemies than positions.
`EnemyController.cs` reads each monster's stats and dialogue. `UIController.cs` owns the
state machine that `State()` and `EnteringState()` expose to Lua.

## Saving

`Assets/Scripts/Save` holds the three pieces of the save system. `SaveLoad` reads and writes
`save.gd` and `AlMightySave.gd` under the platform's persistent data path. `GameState` is the
session save format, holding the player character, the inventory, the item box, elapsed play
time and every session global. `AlMightyGameState`, in `PermanentGameState.cs`, is the
persistent globals format, written the moment a value is set, and is what v0.4 will build its
death counts on.

`GameState` is serialized with a `BinaryFormatter`, so its field list *is* the file format.
Changing those fields breaks existing saves, which is why `GlobalControls.SaveVersion` exists:
bump it in the same commit as any field change, and `SaveLoad.Start()` rejects older saves
with a readable message instead of a deserialization error. It is at `1` as of v0.2.

The save path is kept deliberately even though nothing calls `SaveLoad.Save()` except name
entry. A later checkpoint feature, such as saving between phases of a multi-phase boss, would
be built on `GameState` plus session or AlMighty globals rather than on anything new.

## Licensing

Create Your Frisk is GPLv3, so this project is too. `LICENSE` carries the GPL text.
MoonSharp is separately licensed under BSD, see `MOONSHARP_LICENSE`; its binary lives in
`Assets/Plugins`.
