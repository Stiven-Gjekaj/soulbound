<div align="center">
  <a href="README.md"><b>Soulbound</b></a>
</div>

# Contributing to Soulbound

Thanks for your interest in Soulbound, an Undertale-style game built on Create
Your Frisk. Contributions of all kinds are welcome: bug reports, documentation
fixes, encounters, sprites, and engine work.

Soulbound is at v0.0. The repository is a clean, documented base with no gameplay
yet, so most contributions right now are groundwork rather than content.

## Ways to contribute

- Report a bug or request a feature by opening an issue.
- Improve the documentation in `docs/`.
- Write an encounter, monster, or wave in `Assets/Mods/Soulbound/Lua/`.
- Work on the engine in `Assets/Scripts/`.

Before starting significant work, please open an issue to discuss it, so we can
agree on the approach before you spend time on a pull request.

## Development setup

You need **Unity 2018.4.36f1**, the exact version the engine targets. Get it from
the [Unity version archive](https://unity3d.com/get-unity/download/archive) or
the Unity Hub. `ProjectSettings/ProjectVersion.txt` pins it.

    git clone https://github.com/Stiven-Gjekaj/soulbound
    cd soulbound

Open the repository root as a project in Unity, load
`Assets/Scenes/Disclaimer.unity`, and press play. Always start play mode from
that scene: the engine sets up global state there that later scenes assume.

Full setup notes, including how to configure the Game view to the native 640x480,
are in [docs/basics/unity-setup.md](docs/basics/unity-setup.md).

## Before you open a pull request

There is no automated test suite. The build itself is the check, and it runs in
CI on every push and pull request across Windows, macOS, and Linux.

Verify your change by hand before opening a pull request:

- Play the game from `Assets/Scenes/Disclaimer.unity` and confirm your change
  behaves as intended.
- If you touched Lua, exercise the encounter that uses it. Lua errors surface on
  the in-game error screen, not at compile time, so a broken script builds fine
  and fails at runtime.
- If you touched C#, confirm the project still compiles in the editor with no
  console errors.
- If you can, run a local build with `Build.py` and launch it. See
  [docs/project/building.md](docs/project/building.md).

CI needs `UNITY_LICENSE`, `UNITY_EMAIL`, and `UNITY_PASSWORD` configured on the
repository. A fork will not have them, so expect the build job to fail on
licensing in a fork; that is configuration, not your change.

## Coding style

- Match the surrounding code. The engine is inherited from Create Your Frisk, so
  new code in `Assets/Scripts/` should read like what is already there rather
  than like a fresh project.
- Keep game content in `Assets/Mods/Soulbound/`. That is where the engine loads
  it from at runtime. Do not scatter gameplay into `Assets/Default/`, which holds
  engine-provided resources.
- Adding a mod folder means adding a matching pair of allowlist lines to
  `.gitignore`, which excludes `/Assets/Mods/*` by default so third-party mods
  dropped in for testing are never committed by accident.
- Delete a Unity asset together with its `.meta` file. A folder removed without
  its `.meta` leaves Unity re-importing a ghost asset.
- Write documentation and comments in plain prose. Do not use em-dashes or
  emoji in source, docs, commit messages, or examples.

## Where things live

See [docs/project/repository-layout.md](docs/project/repository-layout.md) for
what sits where, and
[docs/project/engine-architecture.md](docs/project/engine-architecture.md) for
the scene flow, the Lua binding layer, and how mods are loaded.

Four places in the engine name content directly. If you add or remove maps and
mods, check them:

- `ProjectSettings/EditorBuildSettings.asset`
- `Assets/Scripts/Util/UnitaleUtil.cs`, `AddKeysToMapCorrespondanceList()`
- `Assets/Resources/Prefabs/Main Camera OW.prefab`
- `Assets/Resources/Prefabs/Canvas OW.prefab`

## Commit messages and pull requests

- Commit subjects follow `v0.X.N: description`, one version stamp per commit.
- Keep each commit focused on one logical change. Prefer several small commits
  over one large one.
- In your pull request, describe what changed and why, and note how you tested
  it.

## Reporting security issues

Please do not open a public issue for a security problem. See
[SECURITY.md](SECURITY.md) for how to report it privately.

## Code of conduct

By taking part in this project you agree to abide by the
[Code of Conduct](CODE_OF_CONDUCT.md).
