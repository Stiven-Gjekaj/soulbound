# Soulbound documentation

This documents the Soulbound engine: how it reads a mod, and the Lua API your scripts
call. Everything here applies to the engine as it ships in this repository.

Start with [How to read this documentation](how-to-read.md).

Note: the pages carry casual Undertale spoilers throughout.

## Where this came from

The engine began as Create Your Frisk, and these pages began as a hand conversion of the
documentation it shipped as an HTML site. They have diverged since: v0.1 removed the
overworld and its 11 pages, v0.2 corrected everything that still described overworld
behaviour, and v0.3 dropped the version markers that only made sense while the engine was
a Create Your Frisk fork. What is left describes this engine, not that one.

Full attribution is in the [README](../README.md#credits), and the project is GPLv3
because Create Your Frisk is.

## Basics

| Page | What it covers |
| --- | --- |
| [How to read this documentation](how-to-read.md) | notation used throughout these pages |
| [Controls](basics/controls.md) | the keys the engine uses |
| [Basic setup](basics/basic-setup.md) | mod folder structure and what the engine expects to find. Start here |
| [Adding a boss](basics/adding-a-boss.md) | the boss registry, and how a boss reaches the select screen |
| [Unity setup](basics/unity-setup.md) | setting up Unity, playing mods from the editor, exporting builds |
| [Special variables](basics/special-variables.md) | the variables the engine reads out of your scripts |
| [Terminology](basics/terminology.md) | what things are called, for example the "arena" |

## API

| Page | What it covers |
| --- | --- |
| [Text commands](api/text-commands.md) | effects, colours, voices and skipping inside dialogue boxes |
| [Game events](api/game-events.md) | the functions the engine calls in your script, and when |
| [Projectile management](api/projectiles.md) | creating and controlling bullets |
| [Pixel-perfect collision](api/pixel-perfect-collision.md) | per-pixel hitboxes instead of rectangles |
| [Sprites and animation](api/sprites-and-animation.md) | creating sprites and animating them |

### Functions and objects

| Page | What it covers |
| --- | --- |
| [Misc. functions](api/objects/misc-functions.md) | global functions available everywhere |
| [General objects](api/objects/general-objects.md) | the object type shared by sprites and projectiles |
| [The Player object](api/objects/player.md) | player stats, position, damage and healing |
| [The Script object](api/objects/script.md) | reading and writing variables across scripts |
| [The Audio object](api/objects/audio.md) | music and sound playback |
| [The NewAudio object](api/objects/newaudio.md) | multi-channel audio |
| [The Input object](api/objects/input.md) | keyboard state |
| [The Text object](api/objects/text.md) | text anywhere, any font, with or without a bubble |
| [The Time object](api/objects/time.md) | frame timing |
| [The Inventory object](api/objects/inventory.md) | items |
| [The Misc object](api/objects/misc.md) | window, machine and system access |
| [The Discord object](api/objects/discord.md) | rich presence |
| [The Arena object](api/objects/arena.md) | resizing and moving the arena |
| [The UI object](api/objects/ui.md) | the battle interface |

## Shaders

| Page | What it covers |
| --- | --- |
| [Introduction](shaders/introduction.md) | what the shader system is and how to set it up |
| [The Shader object](shaders/shader-object.md) | applying and controlling shaders from Lua |
| [Coding a shader](shaders/coding-a-shader.md) | writing your own shaders |

## Reference

| Page | What it covers |
| --- | --- |
| [Dialog bubble names](reference/dialog-bubble-names.md) | every name usable with `dialogbubble` |
| [Key list](reference/key-list.md) | keys for `Input.GetKey(key)` and `[waitfor:key]` |
| [Item list](reference/item-list.md) | the engine's built-in items |

## This project

| Page | What it covers |
| --- | --- |
| [Milestones](project/milestones.md) | status and what comes next |
| [The boss rush loop](project/boss-rush-loop.md) | pick a boss, fight it, come back: how that works |
| [Repository layout](project/repository-layout.md) | what lives where and why |
| [Building](project/building.md) | Unity version, build script, CI |
| [Engine architecture](project/engine-architecture.md) | scene flow, Lua bindings, mod loading |
