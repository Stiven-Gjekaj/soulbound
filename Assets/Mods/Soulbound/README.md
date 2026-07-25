# Soulbound mod

This is the game. Create Your Frisk loads everything under this folder at runtime, so
all gameplay content belongs here rather than anywhere else in the repository.

As of v0.0 the folder holds placeholders only. There is no gameplay yet.

## Layout

| Folder | Contents |
| --- | --- |
| `Lua/Encounters` | one script per encounter, listing its monsters, waves and arena size |
| `Lua/Monsters` | one script per monster, including bosses: stats, ACT commands, dialogue |
| `Lua/Waves` | bullet patterns, one script per wave, named in an encounter's `nextwaves` |
| `Lua/Libraries` | shared modules pulled in with `require` |
| `Sprites` | sprites for monsters, bullets and backgrounds |
| `Sprites/UI` | interface art |
| `Sounds` | sound effects, `.wav` or `.ogg` |
| `Audio` | music, `.ogg` only |

Two more folders are created here as the game grows, both documented in
`docs/overworld/`:

| Folder | Contents |
| --- | --- |
| `Lua/Events` | overworld event scripts |
| `Maps` | Tiled maps, imported through `Assets/Tiled2Unity` |

## Placeholders

`Lua/Encounters/encounter.lua`, `Lua/Monsters/placeholder.lua` and
`Lua/Waves/placeholder.lua` exist so the engine has one selectable encounter to boot
into. Without at least one mod containing a non-`@` encounter script, the mod selector
stops with "Your mod folder is empty!". Delete them once real encounters exist.

The placeholder monster uses `empty`, a sprite from `Assets/Default/Sprites`. Sprite
lookups fall back to that folder, so the placeholder ships no art of its own.

## Reference

Start with `docs/basics/basic-setup.md` for how the engine reads this folder, then
`docs/api/` for the scripting API.
