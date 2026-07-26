# Soulbound documentation

Soulbound is built on Create Your Frisk (CYF), a Lua-moddable Undertale battle engine
forked from Unitale. Everything below documents that engine: how it reads a mod, the
Lua API your scripts call, and how the overworld is put together.

This is a Markdown conversion of the CYF v0.6.6 LTS 3 documentation that upstream
shipped as an HTML site. Content is preserved; only the presentation changed.

Note: the pages carry casual Undertale spoilers throughout.

## Conventions

Two markers appear inline in these pages:

- `<CYF>` marks something added by Create Your Frisk, not present in original Unitale
- `<0.2.1a>` marks something specific to Unitale 0.2.1a

Retrocompatibility mode, toggled from the options screen, makes Unitale mods run under
CYF. Start with [How to read this documentation](how-to-read.md).

## Basics

| Page | What it covers |
| --- | --- |
| [How to read this documentation](how-to-read.md) | notation used throughout these pages |
| [Controls](basics/controls.md) | the keys Unitale and CYF use |
| [Basic setup](basics/basic-setup.md) | mod folder structure and what the engine expects to find. Start here |
| [Unity setup](basics/unity-setup.md) | `<CYF>` setting up Unity, playing mods from the editor, exporting builds |
| [Special variables](basics/special-variables.md) | the variables the engine reads out of your scripts |
| [Terminology](basics/terminology.md) | what things are called, for example the "arena" |

## API

| Page | What it covers |
| --- | --- |
| [Text commands](api/text-commands.md) | effects, colours, voices and skipping inside dialogue boxes |
| [Game events](api/game-events.md) | the functions the engine calls in your script, and when |
| [Projectile management](api/projectiles.md) | creating and controlling bullets |
| [Pixel-perfect collision](api/pixel-perfect-collision.md) | `<CYF>` per-pixel hitboxes instead of rectangles |
| [Sprites and animation](api/sprites-and-animation.md) | creating sprites and animating them |

### Functions and objects

| Page | What it covers |
| --- | --- |
| [Misc. functions](api/objects/misc-functions.md) | global functions available everywhere |
| [General objects](api/objects/general-objects.md) | the object type shared by sprites and projectiles |
| [The Player object](api/objects/player.md) | player stats, position, damage and healing |
| [The Script object](api/objects/script.md) | reading and writing variables across scripts |
| [The Audio object](api/objects/audio.md) | music and sound playback |
| [The NewAudio object](api/objects/newaudio.md) | `<CYF>` multi-channel audio |
| [The Input object](api/objects/input.md) | keyboard state |
| [The Text object](api/objects/text.md) | `<CYF>` text anywhere, any font, with or without a bubble |
| [The Time object](api/objects/time.md) | frame timing |
| [The Inventory object](api/objects/inventory.md) | `<CYF>` items |
| [The Misc object](api/objects/misc.md) | `<CYF>` window, machine and system access |
| [The Discord object](api/objects/discord.md) | `<CYF>` rich presence |
| [The Arena object](api/objects/arena.md) | resizing and moving the arena |
| [The UI object](api/objects/ui.md) | `<CYF>` the battle interface |

## Shaders

`<CYF>` Added in Create Your Frisk v0.6.5.

| Page | What it covers |
| --- | --- |
| [Introduction](shaders/introduction.md) | what the shader system is and how to set it up |
| [The Shader object](shaders/shader-object.md) | applying and controlling shaders from Lua |
| [Coding a shader](shaders/coding-a-shader.md) | writing your own shaders |

## Overworld

`<CYF>` Added by Create Your Frisk.

| Page | What it covers |
| --- | --- |
| [Basics](overworld/basics.md) | how the overworld is put together. Read this first |
| [How to create a map](overworld/how-to-create-a-map.md) | building maps with Tiled2Unity |
| [How to create an event](overworld/how-to-create-an-event.md) | objects, interactables and cutscenes |
| [How to animate an event](overworld/how-to-animate-an-event.md) | the CYFAnimator system |
| [How to create a shop](overworld/how-to-create-a-shop.md) | everything available inside a shop |

### Overworld objects

| Page | What it covers |
| --- | --- |
| [The General object](overworld/objects/general.md) | saving, loading and general control |
| [The Event object](overworld/objects/event.md) | moving and controlling events |
| [The Player object](overworld/objects/player.md) | the player in the overworld |
| [The Screen object](overworld/objects/screen.md) | the camera and screen effects |
| [The Inventory object](overworld/objects/inventory.md) | items in the overworld |
| [The Map object](overworld/objects/map.md) | map data and transitions |

## Reference

| Page | What it covers |
| --- | --- |
| [Dialog bubble names](reference/dialog-bubble-names.md) | every name usable with `dialogbubble` |
| [Key list](reference/key-list.md) | `<CYF>` keys for `Input.GetKey(key)` and `[waitfor:key]` |
| [Item list](reference/item-list.md) | `<CYF>` the engine's built-in items |

## This project

| Page | What it covers |
| --- | --- |
| [Milestones](project/milestones.md) | status and what comes next |
| [Repository layout](project/repository-layout.md) | what lives where and why |
| [Building](project/building.md) | Unity version, build script, CI |
| [Engine architecture](project/engine-architecture.md) | scene flow, Lua bindings, mod loading |
