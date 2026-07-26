# Building

## Requirements

- **Unity 2018.4.36f1**, Unity 2018's Long Term Support release. This is the exact version
  the engine targets. Get it from the
  [Unity version archive](https://unity3d.com/get-unity/download/archive) or the Unity Hub.
  `ProjectSettings/ProjectVersion.txt` pins it.
- **Python 3.7 or newer** for `Build.py`, only if you build locally.
- **7-Zip**, optional, only if you want `Build.py` to package the builds into archives.

Opening the repository root as a Unity project is all the setup there is. The first import
takes a while because Unity registers every asset; later opens are fast.

For a walkthrough of the editor setup, including setting the Game view to the engine's native
640x480, see [Unity setup](../basics/unity-setup.md).

## Running from the editor

Load `Assets/Scenes/Disclaimer.unity` and press play. Always start play mode from that
scene; the engine sets up global state there that later scenes assume.

## Building locally

`Build.py` sits at the repository root and must run from there, since it uses paths relative
to the project root.

Before running it:

1. Set `unityPath` to your Unity installation if the paths already listed do not match.
2. Set `CYFversion` if you want different output folder and executable names.
3. Make sure this project was the last one opened in Unity, then close Unity.

Then run it:

```
Build.py [--single <target>] [--nozip]
```

- No arguments builds all five targets and zips them.
- `--single <1-5>` builds one target: 1 Windows 32-bit, 2 Windows 64-bit, 3 Linux 32-bit,
  4 Linux 64-bit, 5 Mac.
- `--nozip` skips the 7-Zip packaging step.

Output lands in `bin/`. For each target the script copies `Assets/Default`,
`Assets/Mods` and `docs` next to the executable, strips the `.meta` files from the copies,
and for Mac also copies `How to run Soulbound on Mac.txt`.

That text file explains the `chmod +x` steps a Mac user needs before the app will run, and
where to drop mods inside the .app bundle. It stays at the repository root because the build
ships it.

## Continuous integration

`.github/workflows/build.yml` builds Windows, macOS and Linux on every push and pull request
using [game-ci/unity-builder](https://github.com/game-ci/unity-builder). It calls
`UnityBuilderAction.BuildScript.Build`, which lives at
`Assets/Editor/UnityBuilderAction/BuildScript.cs`, then copies `Assets/Default`,
`Assets/Mods` and `docs` into the artifact the same way `Build.py` does.

CI is the authoritative check that the project still compiles, since the build needs Unity.

## Unity licensing for CI

The build cannot run until the repository carries Unity credentials as Actions secrets.
Which secrets depends on the licence.

**Personal licence**, the free tier:

1. Sign in to Unity Hub on your own machine. Activating there writes a `.ulf` licence file.
   On Windows it lands in `C:\ProgramData\Unity`, on macOS in
   `/Library/Application Support/Unity`, on Linux in `~/.local/share/unity3d/Unity`.
2. Open that file and copy its entire contents, XML and all.
3. Add it as the repository secret `UNITY_LICENSE`.
4. Add `UNITY_EMAIL` and `UNITY_PASSWORD` for the same Unity account.

**Professional licence**, Unity Plus or Pro: use `UNITY_SERIAL` holding the serial key from
your Unity subscription instead of `UNITY_LICENSE`, plus the same `UNITY_EMAIL` and
`UNITY_PASSWORD`.

Until those secrets exist, every `build.yml` run fails at the `game-ci/unity-builder` step
with:

```
Missing Unity License File and no Serial was found.
```

That is a repository configuration gap, not a fault in the project's code. The failure
happens in a couple of seconds, before Unity compiles anything, so a red build with that
message says nothing about whether the code is sound.

There is deliberately no activation workflow in this repository. GameCI's
`unity-request-activation-file` action was deprecated and now fails on purpose, and the
`.ulf` procedure above replaces it. Do not add one back.

The upstream reference is [GameCI's activation docs](https://game.ci/docs/github/activation).

## Releasing

Releases are cut by pushing a version tag. The tag is the source of truth for the version:
nothing else needs editing to change what a build reports.

```
git tag v0.3.0
git push origin v0.3.0
```

That fires [`.github/workflows/release.yml`](../../.github/workflows/release.yml), which
builds all three platforms, assembles the same payload `build.yml` produces, zips each one
as `Soulbound-<version>-<platform>.zip`, and publishes a GitHub release with those three
archives attached.

Three things worth knowing:

- **Releases are marked pre-release.** The workflow always passes `--prerelease`. There is
  no finished game to ship yet, and the flag keeps that clear on the releases page. Remove
  the flag when that stops being true.
- **The notes come from the changelog.** The workflow pulls the section matching the tag's
  version out of [`CHANGELOG.md`](../../CHANGELOG.md). Write that section before tagging.
  If no section matches, the release still publishes, with a pointer to the changelog
  instead of notes.
- **`build.yml` is unchanged.** It still runs on every push and pull request and uploads
  unzipped artifacts. That is the per-commit check; `release.yml` is the distribution path.
  Tagging does not skip the normal build.

The version reaches the executable through `versioning: Custom`, which passes the tag to
`unity-builder`, which passes it to `BuildScript.Build` as `buildVersion`, which sets
`PlayerSettings.bundleVersion`.

`Build.py` at the repository root is the local equivalent for producing builds by hand. It
does not tag or publish anything.
