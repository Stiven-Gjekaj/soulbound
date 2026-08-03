# Soulbound mod

This is the game. The engine loads everything under this folder at runtime, so all
gameplay content belongs here rather than anywhere else in the repository.

As of v0.3 the folder holds placeholders only. There is no gameplay yet.

## Layout

| Path | Contents |
| --- | --- |
| `Lua/bosses.lua` | the boss registry: the ordered list the boss select screen shows |
| `Lua/Encounters` | one script per encounter, listing its monsters, waves and arena size |
| `Lua/Monsters` | one script per monster, including bosses: stats, ACT commands, dialogue |
| `Lua/Waves` | bullet patterns, one script per wave, named in an encounter's `nextwaves` |
| `Lua/Libraries` | shared modules pulled in with `require` |
| `Sprites` | sprites for monsters, bullets and backgrounds |
| `Sprites/UI` | interface art |
| `Sounds` | sound effects, `.wav` or `.ogg` |
| `Audio` | music, `.ogg` only |

## The registry

`Lua/bosses.lua` is what makes a boss appear in the select screen. Every entry names an
encounter script by `id`, and the screen shows them in the order listed:

```lua
return {
    { id = "illia",  name = "Illia, The Tutor", subtitle = "Not built yet" },
    { id = "teased", name = "???",              teased = true },
}
```

An entry pointing at a script that does not exist stops the game with an error naming the
boss, rather than showing a row that does nothing. See `docs/basics/adding-a-boss.md`.

## Placeholders

`Lua/Encounters/illia.lua` is the tutorial boss. The boss is hers; the fight is not built,
so the monster and the wave it pulls in are still `Lua/Monsters/placeholder.lua` and
`Lua/Waves/placeholder.lua`. They keep those names until there is something of hers to put
in their place, because naming them after her would claim they were her fight.

`Sprites/Bosses/<id>.png` is a boss's icon on the select screen, loaded by id when the
screen opens. It takes no import settings and needs no scene edit, and a boss without one
simply shows none.

The `teased` entry has no encounter script at all. It is advertised rather than playable,
which is what `teased = true` means, and the registry does not ask it for one.

The placeholder monster uses `empty`, a sprite from `Assets/Default/Sprites`. Sprite
lookups fall back to that folder, so the placeholders ship no art of their own.

## Reference

Start with `docs/basics/adding-a-boss.md` for how a boss reaches the select screen, then
`docs/basics/basic-setup.md` for how the engine reads this folder and `docs/api/` for the
scripting API.
