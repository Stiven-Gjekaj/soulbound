# Working on Soulbound

Soulbound is a boss rush running on its own Undertale-style battle engine, forked from
Create Your Frisk and cut down. The engine is C# under `Assets/Scripts`, the game is Lua and
art under `Assets/Mods/Soulbound`, and every page of engine documentation is Markdown under
[`docs/`](docs/README.md). Made by PaperTrail.

## Conventions

These are not negotiable defaults, they are how this repository is kept.

**Commits are authored `Stiven-Gjekaj <stivenagostingjekaj@gmail.com>`.**

**Commit subjects carry a version stamp**, exactly one, at the front:

```
v0.6.7: the milestones page says unskinned means placeholder, not bare
```

`0.6` is the milestone. `N` counts commits within that milestone and climbs a couple of dozen
times before it ends. This is not the release version: a release tag is semver and counts
published builds. [`docs/project/building.md`](docs/project/building.md) explains why they look
alike and mean different things.

**No trailers of any kind.** No co-authors, no tooling footers, no links back to wherever the
work was done. The subject and the diff are the whole record.

**Many small commits.** One coherent change each. Never `git add -A`, because it bundles
unrelated work into a subject that describes one thing.

**The changelog entry lands in the same commit as the change**, not batched at the end of a
milestone. [`CHANGELOG.md`](CHANGELOG.md) is Keep a Changelog, newest milestone first.

**No em-dashes and no emoji.** In source, in documentation, in the changelog, in commit
messages. A comma, a colon or a full stop covers every case an em-dash would have.

**Branches.** Work happens on the current milestone branch, `v0.6/wireframe` at the time of
writing. Do not push anywhere else without being asked, and do not open a pull request unless
asked.

## Where things are

```
Assets/Scripts/     the engine
Assets/Mods/        game content, loaded from disk at runtime
  Soulbound/          the game
  @Title/             engine title screen, a hard dependency
Assets/Default/     engine fallback sprites, sounds, music, shaders
Assets/Scenes/      the nine engine scenes
docs/               all documentation
```

## Engine facts worth not rediscovering

- **Unity 2018.4.36f1**, pinned in `ProjectSettings/ProjectVersion.txt`, which CI also reads.
  Do not let Unity Hub upgrade the project. On Apple Silicon it runs under Rosetta 2.
- **The game is 640x480 native.** Sprites should be authored near display size.
- **Mod sprites need no import settings.** `SpriteUtil.FromFile` reads the PNG bytes at runtime
  and hardcodes `FilterMode.Point`, `TextureWrapMode.Clamp`, a centred pivot and 100 pixels per
  unit. Unity's importer never touches them and `Build.py` strips `.meta` files from the copies
  it ships. Drop the PNG in `Assets/Mods/Soulbound/Sprites/` and reference it by filename.
- **`Assets/Default` is name-addressed**, not referenced. `FileLoader.PathToDefaultFile`
  resolves sprites by filename at runtime, so nothing in that folder appears as a reference and
  "unused" means "nothing names it" rather than "no code points at it".
- **Battle menus are two columns.** `TextManager.columnNumber` is 2 and `columnShift` is 265
  pixels per column, so ITEM shows four per page and ACT six. Raising the column count without
  lowering the shift puts the third column off the right edge of a 640 pixel screen.
- **The fight runs on a fixed tick**, 60 steps a second, with a catch-up ceiling. Records count
  battle steps rather than wall time, so a stutter cannot inflate a time.
- **There is no test suite.** The build is the check, and it needs Unity, so CI is the
  authoritative verification. See [`docs/project/building.md`](docs/project/building.md).

## Where the design lives

Gameplay design is argued out in a separate repository,
[Soulbound Notes](https://github.com/Stiven-Gjekaj/soulbound-notes), an Obsidian vault holding
boss designs, difficulty and pacing decisions, art direction, and the options that lost. None
of it runs. Read it before designing anything here, because most questions have already been
settled there and the reasoning is written down.

Its own commit convention differs from this one, `[vN - date]` rather than `v0.X.N`, because a
notes vault has no milestones. Everything else, authorship and trailers and style, is the same.

## Current milestone

**v0.6: wireframe.** Every screen the player sees, rebuilt as a purpose-built screen rather
than an inherited one, on placeholder art. Unskinned here means placeholder, not bare: a screen
with nothing on it cannot be judged. v0.8 replaces all of it.

[`docs/project/milestones.md`](docs/project/milestones.md) has the whole road to v1.0.
