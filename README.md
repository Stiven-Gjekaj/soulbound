<div align="center">

# Soulbound

### A boss rush on a purpose-built Undertale-style engine

_A Lua-moddable battle engine, running on Unity. Made by PaperTrail._

<p align="center">
  <img src="https://img.shields.io/badge/Unity-2018.4.36f1-000000?style=for-the-badge&logo=unity&logoColor=white" alt="Unity 2018.4.36f1"/>
  <img src="https://img.shields.io/badge/engine-112_C%23_files,_18.8k_lines-239120?style=for-the-badge" alt="The engine is 112 C# files and about 18,800 lines"/>
  <img src="https://img.shields.io/badge/docs-37_pages-007ec6?style=for-the-badge" alt="37 documentation pages"/>
</p>

<p align="center">
  <a href="https://github.com/Stiven-Gjekaj/soulbound/actions/workflows/build.yml"><img src="https://github.com/Stiven-Gjekaj/soulbound/actions/workflows/build.yml/badge.svg" alt="Build"/></a>
  <a href="https://github.com/Stiven-Gjekaj/soulbound/releases/latest"><img src="https://img.shields.io/github/v/release/Stiven-Gjekaj/soulbound?style=flat-square" alt="Latest release"/></a>
  <img src="https://img.shields.io/badge/license-GPL--3.0-green?style=flat-square" alt="GPL-3.0 License"/>
</p>

<p align="center">
  <a href="#download"><b>Download</b></a> |
  <a href="#quick-start"><b>Quick Start</b></a> |
  <a href="#status"><b>Status</b></a> |
  <a href="#project-structure"><b>Structure</b></a> |
  <a href="#documentation"><b>Documentation</b></a> |
  <a href="#design-notes"><b>Design Notes</b></a> |
  <a href="CHANGELOG.md"><b>Changelog</b></a>
</p>

</div>

---

## Overview

**Soulbound** is a boss rush based on Soultale. You pick a boss from a menu and
fight it.

It runs on its own engine: a Lua-moddable, Undertale-style battle engine that
started life as [Create Your Frisk](https://github.com/RhenaudTheLukark/CreateYourFrisk)
and has since had its overworld, its map importer and its fork-era baggage removed.
What is left handles battles, shaders, and a scripting API. The game itself is
written as Lua and art inside a mod folder.

This repository is the Unity project. The engine lives in `Assets/`, the game
lives in `Assets/Mods/Soulbound/`, and every page of engine documentation is
Markdown under [`docs/`](docs/README.md).

```lua
-- an encounter, in the engine's scripting API
encountertext = "Soulbound is not built yet."
nextwaves = { "placeholder" }
wavetimer = 4.0
arenasize = { 155, 130 }

enemies = { "placeholder" }
enemypositions = { { 0, 0 } }
```

---

## Status

**v0.5.** The loop exists, keeps score, and now keeps its own time. You pick a boss
from a list, you fight it, you come back to the list, and it tells you how you did:
cleared, cleared without taking a hit, best time, tries, deaths. The bosses are
placeholders, but the game is a game.

v0.0 turned an unmodified Create Your Frisk snapshot into a documented base: about
37 MB of example content removed, the 644 KB documentation website converted by
hand into Markdown, a scaffolded mod folder, and a build passing CI on Windows,
macOS, and Linux.

v0.1 removed the overworld. A boss rush has no maps, events, cutscenes or shops,
and roughly a third of the engine existed to support them. Out went 18 scripts and
6 Lua bindings, three scenes, eight prefabs, six sprite folders, the Tiled map
importer, and 11 documentation pages: 5,800 lines of C# and about 1.5 MB.

v0.2 removed what that left behind: the 40 places the engine still asked itself
whether it was in a map, the flag they consulted, the map data in the save format,
and the last identifiers named after a feature that no longer exists. Nothing
observable changed, because every branch it deleted was already unreachable.

v0.3 built the boss rush loop. A boss registry designers can edit without touching
C#, a select screen that reads it, a fight that starts from it and returns to it,
and a per-boss record of attempts, clears and best times. It also finished leaving
the fork behind: a tag-driven release pipeline, safe mode and Crate Your Frisk gone,
the player naming their own character, and the product renamed to Soulbound.

v0.4 put those records on the boss select and added the two worth keeping beside
them, deaths and no-hit clears, along with an optional in-fight timer. It also fixed
the problems that only appear once people can download a build: sparing a boss did
nothing at all, an error screen named the wrong project and then ignored the key it
told you to press, and the disclaimer still introduced somebody else's engine. Most
of those were found by playing a build rather than reading one, which is part of
shipping now.

v0.5 tied off the loose ends, and none of it is a feature. The fight runs on a fixed
clock of sixty steps a second instead of once per drawn frame, which it turns out was
making the game easier on a slow machine rather than slower: bullets covered half the
ground they should while the soul kept full speed. Records count battle steps now, so
a stutter cannot inflate a time. The letter grid is gone and you type your name. Retro
mode is gone, and four of the bugs hiding inside it were behaviour rather than
appearance. The window title and Discord stopped naming somebody else's engine.

Next is v0.6, which rebuilds every screen the game has as a purpose-built screen
rather than an inherited one, on placeholder art, so the layouts are settled and
argued with before the real art is drawn for them. The [changelog](CHANGELOG.md) has the full account,
and [milestones](docs/project/milestones.md) covers the whole road to v1.0.

---

## Features

<table>
<tr>
<td width="50%" valign="top">

### Engine

- A boss rush loop: pick, fight, return, repeat
- Turn-based battles with the FIGHT/ACT/ITEM/MERCY loop
- Bullet patterns, with rectangular or pixel-perfect collision
- Sprites, animation, text objects, and dialogue bubbles
- Shaders, applied to a sprite or the whole screen
- An inventory, with consumables, weapons, and armor
- Every fight timed, with per-boss attempts, clears and best times
- Every fight scripted in Lua, hot-swappable from a mod folder

</td>
<td width="50%" valign="top">

### Project

- Unity 2018.4.36f1, C#, with MoonSharp as the Lua interpreter
- Game content isolated in one mod folder the engine loads at runtime
- Bosses listed in one Lua data file, no C# change needed to add one
- 37 pages of engine documentation, converted from the upstream site
- CI building Windows, macOS, and Linux on every push
- Tagged releases, built and published on all three by GitHub Actions
- No overworld, no demo content, no fork-era mode toggles

</td>
</tr>
</table>

---

## Download

Builds for Windows, macOS and Linux are on the
[releases page](https://github.com/Stiven-Gjekaj/soulbound/releases/latest). Unzip
anywhere and run `Soulbound-<platform>`. On macOS, read
`How to run Soulbound on Mac.txt` inside the zip first, or Gatekeeper will refuse to
open the app.

Every release also carries a `SHA256SUMS.txt`. Put it in the same folder as the zips
and run `sha256sum -c SHA256SUMS.txt` to check you got the real files.

The current build is placeholder content: three bosses that are a 10 HP monster with
no sprite and a wave that fires nothing. It is the loop, not the game. See
[Status](#status).

To build from source instead, carry on below.

---

## Quick Start

You need **Unity 2018.4.36f1**, the exact version the engine targets. Get it from
the [Unity version archive](https://unity3d.com/get-unity/download/archive) or
the Unity Hub.

Open the repository root as a project in Unity, then load the disclaimer scene
and press play:

```
Assets/Scenes/Disclaimer.unity
```

Always start play mode from that scene. The engine sets up global state there
that later scenes assume.

To produce standalone builds, run the build script from the repository root:

```
Build.py [--single <target>] [--nozip]
```

Builds land in `bin/`, each with `Default`, `Mods` and `docs` copied alongside
the executable. `Build.py` is the developer build; a tagged release ships without
the documentation. Full setup, build, and CI notes are in
[docs/project/building.md](docs/project/building.md).

Save data and settings live under the company and product names Unity is
configured with, `PaperTrail` and `Soulbound`:

| | |
| --- | --- |
| Windows | `%USERPROFILE%\AppData\LocalLow\PaperTrail\Soulbound\` |
| macOS | `~/Library/Application Support/PaperTrail/Soulbound/` |
| Linux | `~/.config/unity3d/PaperTrail/Soulbound/` |

That folder holds `save.gd`, `AlMightySave.gd` (your name, boss records and
options) and `output_log.txt`.

---

## Project structure

The repository root is the Unity project root. Game content is kept in a single
mod folder, because that is where the engine loads it from at runtime.

| Area | Files | Lines | Responsibility |
| ---- | ----- | ----- | -------------- |
| **Lua bindings** | `Assets/Scripts/Lua` | 5130 | The scripting API: object bindings, file loading, asset registries |
| **Battle** | `Assets/Scripts/Battle` | 4007 | The encounter state machine, enemies, the arena |
| **Menus** | `Assets/Scripts/PregamePlaceholder` | 2402 | Title, intro, name entry, mod selector, options, keybinding |
| **Text** | `Assets/Scripts/Text` | 1734 | Text objects, typing, commands, dialogue bubbles |
| **Device** | `Assets/Scripts/Device` | 1605 | Input, screen resolution, global controls, Discord |
| **Util** | `Assets/Scripts/Util` | 1455 | Shared helpers, error reporting, static init |
| **Save** | `Assets/Scripts/Save` | 245 | Session saves and the persistent AlMighty globals |
| **Other** | inventory, rendering, projectiles, players, animation | 2705 | Supporting systems |
| **Total** | **111 files** | **19283** | Battle engine, no overworld and no map importer |

```
Assets/Scripts/     the engine
Assets/Mods/        game content, loaded at runtime
  Soulbound/          the game
  @Title/             engine title screen, a hard dependency
Assets/Default/     engine sprites, sounds, music, and shaders
Assets/Scenes/      the nine engine scenes
Assets/Editor/      editor tooling, shaders, the CI build script
docs/               all documentation
tools/              build and content tooling
```

Where each piece goes, and the one place in the engine that names content
directly, are covered in
[docs/project/repository-layout.md](docs/project/repository-layout.md).

---

## Documentation

<table>
<tr>
<td align="center" width="25%" valign="top">
<h3>Learn</h3>
<p>How the engine<br/>reads a mod</p>
<a href="docs/basics/basic-setup.md"><b>Basics</b></a>
</td>
<td align="center" width="25%" valign="top">
<h3>Look up</h3>
<p>Text commands, events,<br/>and every Lua object</p>
<a href="docs/README.md"><b>Reference</b></a>
</td>
<td align="center" width="25%" valign="top">
<h3>Internals</h3>
<p>Scene flow, Lua<br/>bindings, mod loading</p>
<a href="docs/project/engine-architecture.md"><b>Architecture</b></a>
</td>
<td align="center" width="25%" valign="top">
<h3>Roadmap</h3>
<p>Status and what<br/>comes next</p>
<a href="docs/project/milestones.md"><b>Milestones</b></a>
</td>
</tr>
</table>

Start with [how to read the documentation](docs/how-to-read.md).

---

## Design notes

The reasoning behind the game, as opposed to the engine, lives in a separate repository:
[**Soulbound Notes**](https://github.com/Stiven-Gjekaj/soulbound-notes).

It is an Obsidian vault shared by the people making the game, and it holds boss designs,
arguments about difficulty and pacing, art direction, and the decisions those produced along
with the options that lost. Nothing in it runs. This repository is what the game does; that one
is why it does it.

---

## Building and CI

There is no automated test suite. The build is the check, and
[`.github/workflows/build.yml`](.github/workflows/build.yml) runs it on Windows,
macOS, and Linux for every push and pull request, using
[game-ci/unity-builder](https://github.com/game-ci/unity-builder).

CI needs `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD` as repository
secrets. Without them the build stops in a couple of seconds at the licensing
step, before Unity compiles anything, so a red build with that message says
nothing about whether the code is sound. The licensing section of
[docs/project/building.md](docs/project/building.md) explains what to add.

---

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) to get
started, follow the [Code of Conduct](CODE_OF_CONDUCT.md), and check
[SUPPORT.md](SUPPORT.md) if you need help. The [changelog](CHANGELOG.md) records
what changed between versions.

---

## Support

If you find Soulbound useful, you can support its development here.

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/U1G623RXOE)

---

## Credits

Soulbound is made by **PaperTrail**.

Built on [Create Your Frisk](https://github.com/RhenaudTheLukark/CreateYourFrisk)
by RhenaudTheLukark and contributors, itself a fork of
[Unitale](https://github.com/lvk/Unitale/) by lvk. The documentation under
`docs/` is a Markdown conversion of the documentation that ships with Create Your
Frisk v0.6.6 LTS 3.

Lua is interpreted by [MoonSharp](https://www.moonsharp.org/), written by Marco
Mastropaolo. The binary is in `Assets/Plugins`.

Undertale is the property of Toby Fox. This project is not affiliated with,
endorsed by, or sponsored by him or any associated party.

---

## License

Released under the **GNU General Public License v3.0**. See [LICENSE](LICENSE)
for the full text, and [TERMS.md](TERMS.md) for the project terms.

The GPL is inherited rather than chosen: Create Your Frisk is GPLv3, so Soulbound
is a derivative work and carries the same copyleft license. MoonSharp is
separately licensed under BSD, see [MOONSHARP_LICENSE](MOONSHARP_LICENSE).

<div align="center">
<sub>Start with the <a href="docs/README.md">documentation</a>.</sub>
</div>
