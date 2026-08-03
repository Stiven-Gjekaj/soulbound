# Adding a boss

Soulbound is a boss rush. The boss select screen shows one row per boss, and that list
comes from a single file: `Assets/Mods/Soulbound/Lua/bosses.lua`, the boss registry.

Adding a boss is two steps: write the encounter, then list it in the registry. Nothing in
C# needs to change.

## The registry

The registry is a Lua data file that returns a list of tables, one per boss:

```lua
return {
    { id = "placeholder", name = "Placeholder", subtitle = "Not built yet" },
    { id = "teased",      name = "???",         teased = true },
    { id = "stress",      name = "Stress Test", subtitle = "Not a boss" },
}
```

| Key | Required | What it does |
| --- | --- | --- |
| `id` | yes | names the encounter script under `Lua/Encounters`, without the `.lua` |
| `name` | no | the boss name, shown large. Falls back to `id` |
| `subtitle` | no | one line under the name. May be left out |
| `teased` | no | `true` for a boss the game advertises rather than ships |

The list order is the order the select screen shows, so moving a row moves the boss.

A boss's icon is `Sprites/Bosses/<id>.png`, loaded by id when the screen opens. It needs no
import settings and no scene edit, and a boss without one simply shows none.

Keys the engine does not recognise are ignored rather than rejected, so a portrait or a
music track can be written into the registry before the menu is ready to read it.

## Teasing a boss

A row marked `teased = true` appears on the select screen and can never be picked:

```lua
{ id = "teased", name = "???", teased = true },
```

It needs no encounter script, and the registry does not ask for one, because having something
to start is exactly what would stop it being a tease. It is never unlocked however far the
player has got, so it does not sit behind the tutorial the way a locked boss does, and it
records nothing, because nothing can be recorded against a fight that cannot begin.

That last part has a useful consequence: a teased entry's `id` can be changed freely later,
when it becomes a real boss, without orphaning any record. Once a boss has been fought its id
is load bearing, because the records are keyed on it.

The select screen tells a tease apart from a lock. A locked boss says nothing, because the
player is meant to work out that clearing the first one opens the rest; a teased one says it
is not in this build, because "you have not earned this" and "this does not exist yet" are
different answers and the screen should not give the same silence to both.

## What the registry may not do

It is a data file, not a script. It runs in a sandbox with no access to the filesystem,
the operating system, `require`, or any part of the game's Lua API. `string`, `table`,
`math` and `bit32` are available, so a registry may compute its list, but it cannot reach
outside itself. Anything a boss needs to do belongs in its encounter script.

## Adding one

1. Write `Lua/Encounters/<id>.lua`, along with whatever it needs under `Lua/Monsters`
   and `Lua/Waves`. See [Basic setup](basic-setup.md) for what an encounter script holds.
2. Add a row to `Lua/bosses.lua` with that `id`.

That is the whole process. The select screen reads the registry at startup.

## When it goes wrong

The registry is checked as it loads, and a problem sends you to the error screen naming
what is wrong rather than leaving an empty or broken menu. The checks are:

- the file exists, and is valid Lua that returns a list
- every entry is a table
- every entry has an `id`
- no two entries share an `id`
- every `id` has a matching `Lua/Encounters/<id>.lua`, unless the row is `teased`
- the list is not empty

## Related

- [The boss rush loop](../project/boss-rush-loop.md) for what happens after a boss is picked
- [Basic setup](basic-setup.md) for the folder layout and what each script holds
- [Special variables](special-variables.md) for the variables an encounter script sets
- [Game events](../api/game-events.md) for the functions the engine calls in a fight
