<div align="center">
  <a href="../../README.md"><b>Soulbound</b></a>
</div>

# Milestones and roadmap

Soulbound is a boss rush game based on Soultale. You pick a boss from a menu and
fight it. There is no overworld.

v1.0 ships one playable boss and one teased boss. That target sets the shape of
everything between here and there.

The engine began as Create Your Frisk, a general Undertale-fangame engine, and
roughly a third of it existed to support features this game does not have. v0.1
through v0.3 made the engine fit the game. From v0.4 on, the work is the game.

Art is not the constraint yet. Menus and boss assets arrive at v0.8, so everything
before it is engine and content work with placeholder art, sequenced so that nothing
blocks on a sprite that does not exist. v0.6 rebuilds every screen unskinned for that
reason: layout and art are different problems and only one of them waits on delivery.
A screen built at v0.6 gets used, tested and disliked for two milestones before anyone
draws for it, and the artists get a settled layout to work against instead of a moving
target.

The split from v0.5 onward is a proposal, not a promise. Two milestones, v0.5 and v0.9,
deliberately have no fixed end: they run until their lists are empty and have stopped
growing. That is a real scheduling risk and it is taken on purpose, because both of them
exist to absorb the work that only appears once something is finished enough to use.

## v0.0: a clean foundation

Complete. An unmodified Create Your Frisk v0.6.6 LTS 3 snapshot turned into a
documented base: 37 MB of upstream example content removed, the 644 KB
documentation website converted by hand into 46 Markdown pages, a scaffolded mod
folder, and a build passing CI on Windows, macOS, and Linux. See the
[changelog](../../CHANGELOG.md).

One v0.0 decision is already known to be wrong for this game. The Tiled2Unity map
importer was deliberately kept, with the reasoning that it was needed to author
future maps. A boss rush has no maps. It comes out in v0.1.

## Removing the overworld: v0.1 and v0.2

Taking the overworld out is two milestones, not one, and the line between them is
mechanical rather than a matter of taste.

The engine asks `UnitaleUtil.IsOverworld` in **48 places** outside the overworld
feature itself, and branches on the answer. Alongside it sit
`GlobalControls.isInShop` (9 uses) and `PlayerOverworld.audioKept` (17 uses).
These are not calls into the overworld that break when it is deleted. They are
questions the engine asks itself about which mode it is in.

That gives a clean division:

- **v0.1 makes the answer permanently `false`.** Delete the feature, its assets,
  its Lua API, and its scenes. Keep `IsOverworld` as a shim returning `false`, so
  every branch that consults it collapses to the battle path on its own and the
  project keeps compiling. The edits to battle code are the few that genuinely
  reference overworld types.
- **v0.2 makes the engine stop asking.** With the flag permanently false, walk
  the 48 branches and delete the dead half of each, then remove `IsOverworld`,
  `isInShop`, `audioKept`, `nonOWScenes`, and `canTransOW` entirely.

Doing it in that order matters. v0.1 is destructive but shallow: it touches the
battle path as little as possible, so if the game breaks, the cause is in a small
diff. v0.2 is invasive but mechanical: every edit has a known-correct answer,
because the condition is a constant. Fusing them would mean making risky deletions
and subtle logic changes in the same commits, which is exactly how a working
battle path gets broken quietly.

The risk is worth naming: this is the path the game runs on. The mitigation is CI,
which now builds all three platforms in about three minutes.

## v0.1: strip the overworld

Complete. The feature is gone and the battle path still works. The engine is 111 C#
files and 19,505 lines, down from 129 and 25,334. See the
[changelog](../../CHANGELOG.md).

| Piece | Size |
| ----- | ---- |
| `Assets/Scripts/Overworld` | 12 of its 18 files, 4534 lines |
| `Assets/Scripts/Lua/CLRBindings/Overworld` | 6 files, 1088 lines |
| `Assets/Tiled2Unity` | 53 files, 564 KB |
| Overworld scenes | `TransitionOverworld`, `Shop`, `SpecialAnnouncement` |
| Overworld prefabs | `Canvas OW`, `Main Camera OW`, `Player`, `Background 1`, `Event1`, `ImageEvent`, `Save`, `TP On-the-fly`, `Maps/` |
| Overworld sprites | `FriskUT`, `AsrielOW`, `CharaOW`, `MonsterKidOW`, `BoosterOW`, `SavePoint` |
| Overworld documentation | 11 of the 46 pages, and 2 orphaned images |

`Assets/Scripts/Overworld` was a mixed folder, not an overworld folder. Six of its
files were kept and moved out:

| File | Lines | Why it stays |
| ---- | ----- | ------------ |
| `SaveLoad.cs` | 100 | The save system, and the AlMighty globals v0.4 builds on |
| `PermanentGameState.cs` | 56 | The persistent globals format |
| `GameState.cs` | 164 | The session save format. Gutted of its map fields in v0.2 |
| `Title.cs` | 235 | Backs `TitleScreen` |
| `IntroManager.cs` | 186 | Backs `Intro` |
| `EnterNameScript.cs` | 305 | Backs `EnterName` |

The first three kept the folder under its new name, `Assets/Scripts/Save`; the last
three joined the other menu screens in `Assets/Scripts/PregamePlaceholder`. For the
same reason `TextManager OW.prefab` was kept: `EnterName` instantiates it, and it
carries `EnterNameScript`. Its misleading name is a v0.2 rename.

Six references genuinely pointed at overworld types and were rewired rather than
left to a constant:

- `Battle/UIController.cs` returned the player to the overworld when a battle
  ended. It returns to the mod selector now, until v0.3 gives it a boss select to
  return to.
- `Battle/GameOverBehavior.cs` sent the player back to a save point on death,
  instantiating a teleport prefab to do it. Death restarts without one.
- `Battle/UIController.cs` and `Battle/EnemyEncounter.cs` carried music across the
  overworld boundary. That channel is gone.
- `Device/GlobalControls.cs` opened the overworld pause menu.
- `Title.cs` and `EnterNameScript.cs` loaded the `TransitionOverworld` scene. Both
  are kept scripts loading a deleted scene, so both go to the mod selector.
- `SaveLoad.cs` asked the event manager to snapshot map state before saving.

Two rules ordered the work. Assets went before scripts, because deleting a scene
never breaks compilation but a scene holding the GUID of a deleted script shows as
a missing component. References went before definitions, because C# compiles all or
nothing, so a type can only be deleted once nothing names it.

Inside the folder the scripts formed a reference cycle: `PlayerOverworld`,
`EventManager` and `TransitionOverworld` name each other, and `EventManager` held
instances of all six Lua bindings that call back into it. Only `MapLoader` and
`SpecialAnnouncementScript` were true leaves; the other 16 files were one strongly
connected component, so they came out together once nothing outside the overworld
named them.

## v0.2: clean up after the overworld

Complete. The engine no longer asks what mode it is in, and nothing observable
changed, because every branch deleted was already unreachable. The engine is 111 C#
files and 19,283 lines. See the [changelog](../../CHANGELOG.md).

Three things were deliberately left alone, and are worth knowing about:

- The `"event"` sprite tag in `LuaSpriteController` and `UnitaleUtil`. It is assigned
  to any sprite backed by a `SpriteRenderer` rather than an `Image`, so it may still be
  reachable in battle. Proving that either way costs more than the tidiness is worth,
  so the logic and its error messages stay as they are.
- The retro-mode flee lines in `UIController`, which joke about the overworld being
  missing. They are Unitale's original text and remain literally true here.
- The boot chain. Trimming it would change what the player sees, and the menus want
  designing properly alongside the art, in v0.7.

The save path is also kept on purpose. `SaveLoad.Save()` has one caller, name entry.
It is not dead code: a checkpoint feature, such as saving between phases of a
multi-phase boss, or the mid-run saving the gauntlet needs, would be built on
`GameState` plus session or AlMighty globals rather than on anything new.

The counts below are recounted from the tree after v0.1, not estimated before it.
v0.1 consumed 8 of the branches and all 17 `audioKept` uses on its way past.

- The **40** `IsOverworld` branches, by file: `GameOverBehavior` (18), `SpriteUtil`
  (8), `Inventory` (5), `TextManager` (3), `LuaScriptBinder` (2), and one each in
  `LuaProjectile`, `LuaTextManager`, `GlobalControls` and `UnitaleUtil` itself.
  `Misc` is already clean.
- `isInShop`, down to 3 uses from 9, `nonOWScenes`, `canTransOW`, and `IsOverworld`
  itself. `audioKept` is already gone; it went with `PlayerOverworld`.
- `GlobalControls.EventData`, which is never read or written, and
  `GlobalControls.realName`, which is assigned `null` once and never read.
- The map fields still on `GameState`: `mapInfos`, `tempMapInfos`, `lastScene`,
  and the `MapData`, `TempMapData`, `EventInfos` and `Vect` structs behind them.
  `UnitaleUtil.VectToVector` and `VectorToVect` go with `Vect`, having no callers.
- `TextManager OW.prefab` and the two places `TextManager.cs` matches on its
  name. It is the name entry text box, and it should say so.
- `UnitaleUtil.ExitOverworld`, which v0.1 reduced to session teardown and no longer
  exits anything. It needs a name that matches what it does.
- The `Canvas OW` and `Canvas Two` lookups still left in `ErrorDisplay` and
  `SelectOMatic`, destroying objects that can no longer exist.
- Documentation: the API reference still describes overworld behaviour in **35
  places** across eight pages, heaviest in `misc-functions.md` (11),
  `sprites-and-animation.md` (9) and `discord.md` (6). v0.1 fixed the links and the
  project pages; this is the prose pass, and it wants doing once rather than twice,
  which is why it waited for the branch collapse.

Two things need care. `nonOWScenes` now lists all nine scenes that exist, so the
check in `GameState.cs` can never be false: `lastScene` is dead by construction,
not merely unused. And `GameState` is serialized to `save.gd` with a
`BinaryFormatter`, so removing fields breaks existing saves. `SaveLoad` already
guards that by version, so the same commit has to bump the constant, or the player
gets a raw deserialization exception instead of the readable message.

The scene flow from the title screen stays. Trimming it would change what the player
sees, which is not what this milestone is for, and the menus want designing properly
in v0.3 alongside boss select.

Indicative commits:

```
v0.2.1   collapsed the overworld branches in the game over sequence
v0.2.2   collapsed the overworld branches in sprite handling
v0.2.3   collapsed the overworld branches in the inventory
v0.2.4   collapsed the overworld branches in the text system
v0.2.5   collapsed the remaining overworld branches
v0.2.6   removed the IsOverworld flag
v0.2.7   removed the shop and kept-audio state
v0.2.8   removed the non-overworld scene lists
v0.2.9   removed the map fields from the session save format
v0.2.10  renamed the overworld text box to the name entry text box
v0.2.11  renamed the overworld teardown to a session reset
v0.2.12  simplified the scene flow from the title screen
v0.2.13  removed the overworld notes from the sprite and animation reference
v0.2.14  removed the overworld notes from the misc and general references
v0.2.15  removed the overworld notes from the Discord, text and time references
v0.2.16  updated the README for the stripped engine
```

That commit list is indicative. The shape is what matters: many small commits,
each one a change you could revert on its own.

## v0.3: the boss rush loop

Complete, and the first tagged pre-release. The actual game loop, with programmer art,
plus the release pipeline and the engine's own identity, because a first release should
not go out labelled as somebody else's engine. The engine is 112 C# files and 18,843
lines, down from 19,283: v0.3 added the registry, the records and the profile, and
removed more than that again in fork-era modes. See the
[changelog](../../CHANGELOG.md).

### The loop

- The boss registry, `Assets/Mods/Soulbound/Lua/bosses.lua`. An ordered list, one
  entry per row of the select screen, each naming an encounter script. Adding a boss
  needs no C# change. See [adding a boss](../basics/adding-a-boss.md).
- The boss select. The mod selector paged through folders it found by scanning the
  disk; it pages through the registry instead. That removed the four-level deep
  search, the folder hierarchy, the two sort passes and the encounter sub-list.
  `ModSelect.unity` was not touched: `SelectOMatic` binds to it through inspector
  fields, so changing what it lists needed no scene edit.
- Fight, then back to select, win or lose. Every fight is timed, and beating a boss
  marks it cleared. See [the boss rush loop](boss-rush-loop.md).
- Per boss records as AlMighty globals: attempts, cleared, best time. v0.3 shows only
  the cleared marker, which is what proves the mechanism v0.4 depends on.
- The player names their own character, once before the first fight and afterwards
  from the options screen. The name is an AlMighty global beside the records, so one
  file holds the whole profile.

### Releases and identity

- `release.yml` cuts a release when a `v*` tag is pushed. The tag is the version, and
  it reaches the executable through `versioning: Custom`. Three platforms, zipped,
  attached to a GitHub release marked pre-release, with notes pulled from the changelog.
  `build.yml` stays the per-push check. See [building](building.md#releasing).
- The documentation no longer presents this as Create Your Frisk. The 273 `<CYF>` and
  5 `<0.2.1a>` markers are gone, along with about 150 prose references. What remains is
  attribution, the origin note, and the places where Unitale genuinely is the subject,
  such as `isCYF` and retrocompatibility mode. Attribution stays because the project is
  GPLv3 by inheritance and the licence requires it.
- `productName` is `Soulbound` and `companyName` is `PaperTrail`. Both build
  `Application.persistentDataPath`, so this moved the save files. It landed immediately
  before the first tag on purpose: free now, disruptive once anyone has a build.
- Safe mode and Crate Your Frisk are gone. Both were cosmetic, and the options screen
  offered a toggle labelled Crate Your Frisk, which is the branding the rest of the
  milestone removed.
- The `cyfshaders` AssetBundle name is staying. It is baked into seven `.meta` files and
  a built binary bundle, and mods reference it by name, so renaming it is a breaking
  change for no real gain.

## v0.4: persistence and quality of life

Complete. The records v0.3 wrote but did not show, the counters that belong beside them,
and the first-release problems that only appeared once people could download a build.

- Deaths per boss and overall, counted when the game over runs to its end so a scripted
  revive does not charge one.
- No-hit clears. Healing and the invulnerability-flash `Hurt(0)` leave one intact.
- All five records on the boss select, sharing the one spare line the scene has.
- An in-fight timer with an options toggle, off by default. Display only: timing is
  unconditional, which is why there is no speedrun mode. A mode would gate something
  already always on.
- Releases are marked pre-release by the shape of the tag. v0.3.0 published correctly and
  then did not appear on the repository front page, because GitHub excludes pre-releases
  from "latest" and that is what the Releases panel shows.
- The missing-Mods error, which is what a player sees if they run the build from inside
  its zip. It named the fork, asked a question instead of giving an instruction, and
  offered a restart that fails identically. Every release now carries `Read me first.txt`,
  which is the only thing that reaches a player before they hit it.
- Fourteen more strings naming the fork, all in failure states and hover text, which v0.3
  missed because they only render when something goes wrong.

AlMighty globals are the store, proven by v0.3 and now carrying six values per player. A
purpose-built format is still not worth it.

Two things are held back by having no scene editor here, and both belong to v0.7 with the
art: the boss select shows its records on one line because that is all `ModSelect.unity`
has spare, and the options screen is now using all ten of its rows, two of them adopted
from retired settings.

## v0.5: loose ends

The largest milestone before release, and the first of the two with no fixed end. It runs
until the list is empty and has stopped growing. Everything deferred, worked around, or
noticed and never written down belongs here, because everything after this is screens,
content and art, and none of those should be built on top of a known defect.

Still open. What follows is what has been closed so far; the [changelog](../../CHANGELOG.md)
has the detail.

### Done

- **The fight runs on its own clock.** `BattleTick` advances the battle in whole steps of a
  sixtieth of a second: arena, attack bar, player, encounter and wave scripts, then
  projectile movement and collision, in that order. It used to run once per rendered frame
  while the player moved by elapsed time, so a slow machine gave the player full speed
  against half-speed bullets. The timing decision was settled as a fixed step rather than
  delta-time scaling, because the Lua API is public and its patterns are written as movement
  per `Update`.
- **A fight is timed in steps rather than seconds**, so a stutter cannot inflate a record.
- **Retro mode is gone.** 53 references across 18 files, the flag, the `isRetro` Lua global,
  the options row, and the warning banner. Four of its branches turned out to be behaviour
  rather than appearance and were fixes rather than deletions.
- **The fight keeps its own time, measured.** The stress encounter's three turns each run
  600 wave updates: 12 bullets, then 288, then 288 in an arena more than twice the size.
  All three took 10.0s of game time and 10.0s of wall clock, at worst frames of 16, 10 and
  48fps.

  The 10fps turn dropped a step: a frame that long owes six against a ceiling of five, and
  `Time.dropped` reported the one discarded. Neither the game time nor the wall clock moved
  for it, because one step in six hundred is a sixtieth of a second and vanishes into a
  decimal place. That is the reason the counter exists: reading the frame rate and inferring
  the rest was giving answers that changed between runs.
- **Names are typed.** The letter grid is gone and the screen is a text field with two
  buttons, worked by keyboard or mouse. A controller cannot type, so `Done` accepts an empty
  name and falls back to the default rather than trapping a pad player.
- **The Time page stopped contradicting the tick.** It told authors to multiply movement by
  `Time.mult`, which was right when a wave's `Update` ran once per rendered frame and is
  backwards now that it runs sixty times a second everywhere.
- **Bad text commands report** instead of failing silently, across 14 sites.
- **Twenty inherited notes** stopped citing Create Your Frisk's 0.7, which collides with our
  own. Two of them were design questions and are recorded as open rather than settled.
- **The window title and Discord** stopped naming the fork. One part of this cannot be fixed
  from code and is recorded against v0.9.

### Open

- **Whether five is the right catch-up ceiling.** It has now been reached and measured
  rather than argued about, but nothing has been decided. The question is what a fight
  should do on a machine that cannot keep up: skip forward and stay in step with the wave
  timer and the music, or run every step and fall behind them. Five is the current answer
  by inheritance rather than by argument.
- **A load that is slow for longer than one frame.** The stress encounter turns out to
  measure startup cost rather than sustained load. Its worst frame is the one that compiles
  the wave and creates every bullet, and once that is paid the renderer keeps up: the
  288-bullet turn in the largest arena saw a worst frame of 48fps, better than the same
  wave in a smaller arena that had to compile it. Nothing here has produced a machine that
  is slow for a whole wave, which is the condition a real player on old hardware is in.
- **Text still types per rendered frame.** `TextManager` has its own `Update` and is not on
  the battle tick, so `[speed:x]`, `[w:x]` and `[waitall:x]` are counted in rendered frames:
  dialogue types at half speed on a 30fps machine and double on a 120fps one. The
  documentation is accurate about this, which is how it was found. It was left alone rather
  than fixed quietly, because `TextManager` also runs on screens that have no battle tick
  and moving it is a decision rather than a repair. v0.7 is a boss written largely in text
  commands, so it wants settling before then.
- **Whatever else the stress encounter exposes.** Bullet performance, wave composition, and
  whatever the Lua API makes awkward when a fight runs long are all expected to land here.
- **Thirteen remaining TODOs**, none of which name a version any more, to be triaged into a
  v0.5 item or an explicit decision not to do them.
- **Anything only visible off Linux.** The Discord leak was invisible to every playtest here
  and sat in people's friends lists. Windows-only and macOS-only paths need reading, and
  anything found there needs a human on that platform to confirm.

### Ending it

v0.5 is done when the list is empty and the stress encounter plays correctly: several
phases, a few hundred simultaneous projectiles, a fight long enough to drift, and dialogue
between waves. If it does not hold up, the gap it exposes is v0.5 work that was not on the
list, which is the reason for building it.

It was meant to be thrown away rather than shipped. It ships: it is in the registry and a
player sees it at the end of the boss list, labelled as not a boss. A measuring instrument
that only exists on a developer's machine cannot be pointed at the machine that is actually
having the problem, which is the case for keeping it. It leaves when it stops earning that.

## v0.6: wireframe

Every screen the player sees, rebuilt as a purpose-built screen rather than an inherited
one, and deliberately unskinned. Disclaimer, title, name entry, boss select, options,
keybinds, and the battle UI itself.

This is the milestone that most needs someone with the Unity editor open, because it is
scene work rather than script work. Everything below has been waiting on exactly that:

- The boss select is still the repurposed mod selector. Its objects are named `ModTitle`,
  `EncounterCount` and `encounterBox`, and it shows all five records on one line because
  that is the only spare line the scene has. It wants a real table.
- The options screen is using all ten of its rows, two of them adopted from settings that
  were retired. An eleventh option currently has nowhere to go.
- The disclaimer screen is rebranded at runtime from C#, because none of it is reachable by
  field: the inherited logo is hidden and the title is cloned out of the version label.
  `DisclaimerScript.Rebrand` should not survive this milestone.
- The in-fight timer builds its text at runtime from the same prefab Lua's `CreateText`
  uses, because `Battle.unity` has no object for it.
- A locked entry in the boss select, for the boss v1.0 teases rather than ships. The
  registry, the select screen and the records all have to handle an entry that cannot be
  fought. Building that into a new screen costs almost nothing and adding it to a finished
  one costs a rebuild, which is why the capability lands here and the boss it advertises
  lands at v0.8.

## v0.7: the first boss

The first Soultale boss, built as content rather than engine work: phases, patterns,
dialogue, ACT options, balance. Placeholder art and audio throughout, on the finished
layouts from v0.6, because the point is a fight that plays well before it looks or sounds
finished.

This is the milestone that proves the engine. Anything the engine cannot express is v0.5
work that was missed, and belongs back there rather than worked around in Lua.

It is also the first milestone with testers in the loop, which the release workflow already
handles: a tag carrying a pre-release identifier, `v0.7.0-rc3`, publishes a real release
with real builds attached and stays out of "latest", so a test build never becomes the
headline download. Expect a lot of them, and expect the fight to be rebuilt more than once.

## v0.8: assets

Art and audio arrive and are integrated: the boss's sprites, music and sound, and the skin
for every screen v0.6 built. Long, large, and mostly not code.

Blocked on delivery, which is why it sits after the content rather than inside it. Nothing
from v0.5 through v0.7 may wait on a sprite that does not exist yet.

- The teased boss gets its identity here: whatever the locked entry built at v0.6 displays,
  presented well enough to say what is coming without saying too much.
- A Soulbound logo on the splash screen. The project is on a Unity Personal licence, so the
  Unity logo cannot be removed, but a custom logo can sit with it: add the sprite to
  `m_SplashScreenLogos` in `ProjectSettings` with `m_SplashScreenDrawMode` left at `0`,
  which draws both on one screen rather than as two sequential ones. v0.4 already set the
  splash background to black so it runs into the disclaimer screen without a flash.

## v0.9: the demo

The public demo, and the longest phase of the project. It ends when everything is tied up
and not before.

This is everything between "the parts work" and "a stranger can play it": first-run
experience, packaging, a pass over the options screen, and the bug list that only exists
once people outside the team have played it. v0.4 was the first milestone played end to end
before shipping, and three of its fixes exist only because of that. A demo is the same
lesson at a scale the team cannot reproduce on its own.

One thing changes permanently here. **Once the demo is public, the save format is public.**
Players will have records on disk, and from that point renaming an AlMighty global key or
bumping `GlobalControls.SaveVersion` orphans real progress rather than test data. Any
change to what v0.3 and v0.4 wrote has to land before this ships, not after.

**Soulbound needs its own Discord application before this ships**, and that is an action on
Discord's developer portal rather than a code change. `DiscordControls.Start` connects with
Create Your Frisk's application ID, and Discord draws the game name and the icon from
whatever is registered against that ID, so a player with Discord open currently announces to
their friends list that they are playing Create Your Frisk. v0.5 fixed everything about this
that code can reach, which is the window title and the icon tooltip; the name and the icon
itself need the new ID pasted into `DiscordControls.cs`.

What the demo contains is a decision for the start of this milestone rather than now. The
one constraint worth setting in advance is that it should not be all of v1.0.

## v1.0: ship

One playable boss, one teased boss, on Windows, macOS and Linux, plus the small tweaks the
demo made obvious.

Distribution is real work rather than a button press. GameJolt and itch.io each want their
own packaging and metadata, and the release workflow currently produces three zips and a
checksum file shaped for GitHub. Neither store is a straight upload of what already exists.

The other known problem is that the builds are unsigned, so SmartScreen and Gatekeeper will
warn about them. `SHA256SUMS.txt` makes "this is our build" checkable for anyone who thinks
to check, which is not the same as making the warning go away, and a signing certificate
costs money every year.

## After v1.0: the gauntlet

All bosses back to back without stopping. Naming needs care: this game is already a boss
rush, so the mode wants a distinct name in code and docs, and the player-facing name is a
separate decision. It is also the feature that makes mid-run saving matter, which is what
`save.gd` and `SaveLoad.Save()` are being kept for. v0.3's records are keyed per boss and
do not preclude a run-level record alongside them.

## How versions are cut

Within a milestone, work lands as small, numbered commits (for example `v0.1.1`
through the final `v0.1.x`). The last commit of a milestone marks it complete.
Commit subjects carry exactly one version stamp, and the
[changelog](../../CHANGELOG.md) records what each milestone changed.
