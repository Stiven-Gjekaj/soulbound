# Soulbound

An Undertale-style game built on Create Your Frisk, a Lua-moddable battle and overworld
engine.

This repository is the Unity project. The engine lives in `Assets/`, the game lives in
`Assets/Mods/Soulbound/`, and all documentation is in [`docs/`](docs/README.md).

## Status

v0.0. The repository is a clean, documented base: the engine, its licenses, and the full
engine documentation converted to Markdown. There is no gameplay yet, only a placeholder
encounter so the engine boots into a playable state.

## Getting started

Open the repository root as a project in **Unity 2018.4.36f1**, then load
`Assets/Scenes/Disclaimer.unity` and press play. Always start play mode from that scene.

Full setup, build and CI instructions are in
[docs/project/building.md](docs/project/building.md).

## Documentation

| Page | What it covers |
| --- | --- |
| [Documentation index](docs/README.md) | everything below, plus the full engine API |
| [Repository layout](docs/project/repository-layout.md) | what lives where and why |
| [Building](docs/project/building.md) | Unity version, build script, CI |
| [Engine architecture](docs/project/engine-architecture.md) | scene flow, Lua bindings, mod loading |
| [Basic setup](docs/basics/basic-setup.md) | how the engine reads a mod folder |
| [Special variables](docs/basics/special-variables.md) | the variables the engine reads from your scripts |

## Credits

Built on [Create Your Frisk](https://github.com/RhenaudTheLukark/CreateYourFrisk) by
RhenaudTheLukark and contributors, itself a fork of
[Unitale](https://github.com/lvk/Unitale/) by lvk. The documentation under `docs/` is a
Markdown conversion of the documentation that ships with Create Your Frisk v0.6.6 LTS 3.

Lua is interpreted by [MoonSharp](https://www.moonsharp.org/), written by Marco Mastropaolo.
The binary is in `Assets/Plugins`.

## License

Create Your Frisk is released under the GNU General Public License v3.0, so this project is
too. See [LICENSE](LICENSE).

MoonSharp is separately licensed. See [MOONSHARP_LICENSE](MOONSHARP_LICENSE).
