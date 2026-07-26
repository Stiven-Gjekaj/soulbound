# Adding a boss

Soulbound is a boss rush. The boss select screen shows one row per boss, and that list
comes from a single file: `Assets/Mods/Soulbound/Lua/bosses.lua`, the boss registry.

Adding a boss is two steps: write the encounter, then list it in the registry. Nothing in
C# needs to change.

## The registry

The registry is a Lua data file that returns a list of tables, one per boss:

```lua
return {
    { id = "placeholder",       name = "Placeholder",        subtitle = "Not built yet" },
    { id = "placeholder_two",   name = "Second Placeholder", subtitle = "Also not built yet" },
    { id = "placeholder_three", name = "Third Placeholder",  subtitle = "Here so the list has something to page through" },
}
```

| Key | Required | What it does |
| --- | --- | --- |
| `id` | yes | names the encounter script under `Lua/Encounters`, without the `.lua` |
| `name` | no | the boss name, shown large. Falls back to `id` |
| `subtitle` | no | one line under the name. May be left out |

The list order is the order the select screen shows, so moving a row moves the boss.

Keys the engine does not recognise are ignored rather than rejected, so a portrait or a
music track can be written into the registry before the menu is ready to read it.

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
- every `id` has a matching `Lua/Encounters/<id>.lua`
- the list is not empty

## Related

- [Basic setup](basic-setup.md) for the folder layout and what each script holds
- [Special variables](special-variables.md) for the variables an encounter script sets
- [Game events](../api/game-events.md) for the functions the engine calls in a fight
