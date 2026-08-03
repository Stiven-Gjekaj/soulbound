<div align="center">
  <a href="README.md"><b>Soulbound</b></a>
</div>

# Changelog

All notable changes to Soulbound are recorded here. The format is based on
Keep a Changelog (https://keepachangelog.com), and the project aims to follow
semantic versioning.

Version stamps appear in every commit subject as `v0.X.N`, so the history reads
as a sequence of small, individually described changes rather than a few large
ones.

## 0.6 (unreleased)

### Removed

- `Photo.png`, `PhotoBack.png` and `spr_chestbox_0.png` are out of `Assets/Default/Sprites`.
  The first is a group photo of Undertale's cast and the others are stray leftovers, and no
  mod, script, scene or prefab asks for any of the three by name or by GUID.

  `Default/` is not referenced the way the rest of the project is. `SpriteUtil` and
  `SpriteRegistry` resolve sprites by filename at runtime through
  `FileLoader.PathToDefaultFile`, so nothing in that folder ever appears as a reference and
  "unused" has to be established by asking whether anything names it. Nothing does. The
  mugshot folders stay: they are the fallback the engine falls through to when a mod has no
  portrait of its own, and Soulbound has no portraits yet.

- Lu and Punder, Create Your Frisk's own characters, are out of `Assets/Resources/Sprites`.
  Fifty sprites and 433 KB, none of them loaded by anything: no `Resources.Load` call names
  either folder, and their GUIDs appear in no scene, prefab or asset in the project. They were
  in a `Resources` folder, which Unity compiles into the build whether or not anything reads
  it, so they shipped in every release and were never drawn.

### Added

- The read me points at [Soulbound Notes](https://github.com/Stiven-Gjekaj/soulbound-notes),
  the repository the game's design is argued out in. It is an Obsidian vault holding boss
  designs, difficulty and pacing arguments, art direction, and the decisions those produced
  along with the options that lost. None of it runs, which is why it is a separate repository:
  this one is built by CI on every push and everything in it is either shipped or checked,
  and half-formed ideas do not survive that setting. This repository is what the game does,
  that one is why it does it.

### Changed

- The player starts with fists and a worn coat instead of a stick and a bandage. The two
  inherited names were Undertale's, and one of them collided with a healing item Soulbound
  wants to keep calling Bandage, so the player would have been wearing a bandage while
  carrying more of them in the bag. The new names also say who the player is: an outsider
  who arrived with nothing and gets stronger by fighting, so they start with themselves and
  the clothes they crossed in.

  This is not only a text change. `Inventory.UpdateEquipBonuses` resolves ATK and DEF by
  looking the equipment name up in the item pools, and the engine's fallback logic reverts
  to those two names whenever a mod removes the equipped item from the library, so both had
  to move together. Fists and Worn Coat are now entries in the weapon and armor pools at 0,
  which is what the old defaults should always have been: neither Stick nor Bandage was in
  either pool, so looking up the engine's own starting gear used to warn that it did not
  exist. Stick and Bandage stay in the library as ordinary items.

- The building page says which Unity Hub modules to install. It named the editor version
  and stopped, leaving the rest of a long module list to guess at. Two matter, Windows and
  Linux build support, and the **Mono** variants rather than IL2CPP, because
  `scriptingBackend: Standalone: 0` is Mono and the IL2CPP modules are a large download that
  never gets used. Your own platform needs no module: the editor ships with build support
  for the system it runs on.

  It also records that Unity 2018.4 is an Intel build and runs under Rosetta 2, which is the
  supported way to work on this from an Apple Silicon Mac. The native ARM64 editor arrives
  in Unity 2021.2, and going there is not a version bump: that release removed the legacy
  .NET 3.5 scripting runtime this project uses, and the upgrade re-serialises every scene
  and prefab. And a warning not to let the Hub upgrade the project, since CI picks its
  editor from `ProjectVersion.txt` and would then be building something else.

Every screen the player sees, rebuilt as a purpose-built screen rather than an
inherited one, and deliberately unskinned. Disclaimer, title, name entry, boss
select, options, keybinds, and the battle UI itself.

This is scene work rather than script work, so it is the first milestone that needs
someone with the Unity editor open. Several things have been waiting on exactly
that: the boss select is still the repurposed mod selector and shows all five
records on one line because that is the only spare line its scene has, the options
screen has used all ten of its rows, the disclaimer is rebranded at runtime from C#
because none of it is reachable by field, and the in-fight timer builds its text
from a Lua prefab because `Battle.unity` has no object for it.

Unskinned on purpose. The layouts get finished and lived with before anyone draws
art for them, so the artists at v0.8 get a settled target rather than a moving one.

## 0.5 (2026-07-30)

Tying off every loose end before the game gets screens, a real boss and art. The
milestone was written with no fixed end, to run until its list was empty and had
stopped growing. It stopped growing.

Nothing here is visible as a feature. What changed is that the fight now keeps its
own time, records measure what the fight did rather than how long you sat there,
and a long list of things that were quietly wrong underneath are not wrong any
more. Most of them were found by playing the build rather than reading it.

### Added

- **The battle tick.** The fight advances in whole steps of a sixtieth of a second rather
  than once per rendered frame. Each step runs the arena, the attack bar, the player, the
  encounter and wave scripts, then projectile movement and collision, in that order.

  Before this the fight ran once per frame while the player moved by elapsed time, so a
  machine holding 30fps ran a four second wave 120 times instead of 240. Every bullet
  written as movement per update covered half the ground while the soul kept its full
  speed. The fight was not slower on slow hardware, it was easier.

  A fixed step rather than scaling movement by elapsed time, because the Lua API is public
  and its patterns are written as movement per `Update`. Scaling would have silently
  changed the speed of every pattern ever written; this leaves them meaning what they meant.
  The order was previously undefined: `PlayerController` and `Projectile` each had their own
  `Update` with no execution order between them, so whether a bullet was tested against this
  step's or last step's player position was decided by nothing.
- **Typing your name.** The name screen takes letters from the keyboard, up to nine, with
  Backspace to delete. The three buttons respond to the mouse as well as to Enter.
- **A stress encounter**, listed in the registry. Three turns of identical length, each
  running a fixed number of wave updates before ending itself, differing only in how many
  bullets they push. Each reports the game time it took, the wall clock it actually took and
  the worst frame it saw, so the claim above can be measured rather than argued.
- **`Time.dropped`**, the number of steps the catch-up ceiling has thrown away. Above zero
  means the machine could not keep up and the fight skipped forward rather than running
  every step: bullets did not move for those steps and no collision was tested for them.

  It was added because the first stress run could only infer whether the ceiling had been
  reached, from a frame rate, and inference is not measurement. The run after it settled the
  question: a 10fps frame in the 288-bullet turn owed six steps against a ceiling of five
  and dropped one. Neither the game time nor the wall clock moved for it, which is exactly
  why reading the frame rate was not good enough.

### Removed

- **Retro mode**, the last inherited mode flag: 53 references across 18 files, the flag, the
  options row and its description, the `isRetro` Lua global, and the warning banner. It
  changed gameplay semantics rather than appearance, which is why it went in its own pass
  with the same shim-collapse-delete order v0.2 used on `IsOverworld`.

  The retro flee lines go with it. v0.2 kept them on the grounds that jokes about a missing
  overworld were literally true here, which held while retro mode still existed to reach
  them. Nothing reaches them now.
- **`SetFrameBasedMovement`** and the `ControlPanel` field behind it. Both of its settings
  mean the same thing once the fight runs on a fixed step, and a function whose two options
  are identical is worse than no function.
- **The letter grid on the name screen.** It existed because there was no other way to enter
  a name, and once letters could be typed it only created an argument over the Z key, which
  is a letter and Confirm at once. With nothing to confirm into, Z is only ever a letter.
- **The Backspace button** on the same screen, which the grid needed for the same reason.
  The Backspace key deletes a letter and so does X, so the button was a third way to do one
  thing sitting in the middle of the row. `Quit` and `Done` are the whole screen now, and
  they move up under the name while it is being typed rather than sitting at the bottom of
  an empty screen. They drop back down for the confirm screen, where the name is scaled up
  and takes the middle.

### Changed

- The roadmap from v0.5 to v1.0. Six milestones rather than five. v0.5 becomes open-ended
  loose ends instead of a fixed list of engine work. Rebuilding the screens splits out of
  the art milestone into v0.6 and happens unskinned, so the layouts are finished and lived
  with before anyone draws for them and the artists get a settled target rather than a
  moving one. The first boss moves to v0.7, art and audio to v0.8, and v0.9 becomes a
  public demo rather than an internal release candidate.

  Three consequences are recorded with it. The demo makes the save format public, so any
  change to the AlMighty global keys or `GlobalControls.SaveVersion` has to land before
  v0.9 rather than after. Publishing to GameJolt and itch.io is real packaging work rather
  than an upload of what the release workflow already produces. And the framerate model has
  to be chosen before the first boss is written, because every pattern in that fight gets
  tuned against whichever model exists at the time.
- The loop page's account of timing. It described framerate drops and `Time.timeScale` as
  two open questions. They are one disagreement with two faces: wave duration is wall-clock
  at `Time.time + wavetimer` while wave content ticks once per rendered frame through
  `UIController.Update`, so a machine holding 30fps runs a four second wave 120 times
  instead of 240 and every bullet written as movement per update covers half the ground.
  `Time.timeScale` then moves `Time.time` without moving the record clock. The page names
  the line each face comes from now.
- **A fight is timed in battle steps rather than wall-clock seconds.** On a machine holding
  60fps that is the same number a stopwatch gives. Where they differ the step count is the
  honest one: it measures what the fight did rather than how long the player sat in front of
  it, so a stutter, a slow machine or a boss slowing time down cannot inflate a record for
  the same work. Existing records were set at 60fps and stay meaningful, so no save
  migration is needed.
- **A wave script's `Update` runs exactly 60 times a second.** It used to run once per
  rendered frame, which the documentation described as "usually at 60FPS, depends on the
  player's framerate". Patterns can be written against that number now.
- **`Time.mult` stopped being advice and became a reading.** The API page told authors to
  multiply movement by it "so your waves will be consistent on lower framerates", which was
  true while a wave's `Update` ran once per rendered frame and is now backwards: `Update`
  runs sixty times a second everywhere, so movement per `Update` is already equal and
  scaling it makes a pattern *faster* on a slow machine. The same page now separates what
  follows the fight from what follows the renderer, because `Time.dt`, `Time.mult` and
  `Time.frameCount` are all the second kind and none of them said so. This matters before
  v0.7 rather than after: every pattern in the first boss gets written against this page.
- The bullet tutorial stopped counting in frames. Its `Update` comments said "every frame"
  and "every 30 frames", which was the truth when a wave ran once per rendered frame and is
  now both wrong and vaguer than the code deserves: 30 iterations is exactly half a second
  on every machine. It is the example boss authors copy from, so its vocabulary is the
  vocabulary they learn.
- The stress boss says its three lines. It never had: it defined `EnemyDialogueStarting`,
  and so does its encounter, and the engine's `CallOnSelfOrChildren` returns as soon as the
  encounter script handles a hook. A monster's copy of any hook the encounter also defines
  is dead code and nothing says so. The monster exposes a plainly named function that the
  encounter calls instead.
- **The attack instances that "otherwise break" no longer needed a workaround.** The engine
  destroyed every attack instance belonging to an active enemy immediately before starting
  an attack, with a note saying they broke otherwise and nobody knew why. The reason is that
  `Init` appends to the instance lists and nothing emptied them when `ATTACKING` ended, so
  instances survived into the next turn. `ChangeTarget` destroys everything past index 0 and
  aims index 0, so a stale entry in front meant it killed the live instances and retargeted
  a dead one. `Finished` aggregates over the list, so a stale instance that never finishes
  left the fight stuck in `ATTACKING`. The freeze code read index 0 with no bounds check.

  Starting an attack now clears the instances itself, which is where that invariant belongs
  and which also covers the case the workaround missed: an enemy a script deactivated
  without killing or sparing is not in `EnabledEnemies`, so its instances survived the
  sweep. The two index-0 reads are guarded, because killing the last enemy mid-attack empties
  the list while the state is still `ATTACKING`.
- **`Text.GetTextHeight` works before the text has typed.** It is documented as giving the
  same answer whether or not typing has finished, which is what `Text.GetTextWidth` does
  through `PredictTextWidth`. There was no `PredictTextHeight`, so height returned zero
  until the letters existed. Screens that size themselves to their content have to ask
  before they draw, which is v0.6's whole job.
- **The text command stripper is written once.** `TextMessage` and `SelectMessage` each had
  their own hand-rolled scan for the commands at the start of a line, and the copies had
  drifted. `SelectMessage` never reset the flag meaning "I found a closing bracket", so a
  second command without a `]` left it looping over a string it had stopped changing: an
  unclosed bracket at the start of a menu option **hung the game**. It also indexed the line
  without rechecking it was non-empty. Neither fault was in the other copy, which is what
  two copies of a parser buys you.

  Both call `UnitaleUtil.ExtractLeadingCommands` now. It also fixes an inconsistency both
  shared: `[starcolor]` and `[letters]` are meant to stay where they are, but the check for
  them required ten characters for both names, so the nine-character line `[letters]` failed
  it and was stripped while `[letters]x` was kept. This matters before v0.7, a boss written
  largely in text commands.
- **A fourth stress turn, meant to hurt.** Turns one to three all held close to sixty frames
  a second, so the catch-up ceiling was never really reached and what happens past it was
  still argument. This one pushes bullets in the thousands across a 500x300 arena, spread
  over the first sixty steps so the spawn does not become the measurement. Its bullet count
  is meant to be turned up: a run reporting `d0` means the load was too light, not that the
  ceiling is unreachable.
- **Encounter text that crossed the text box border.** The battle text box does not wrap
  unless the encounter sets `autolinebreak`, so a line wider than the box runs through its
  border and is clipped by the screen edge rather than continuing on the next line. Three of
  our own lines were doing it, including the stress encounter's own opening, which lost its
  last word. They carry their own `\n` now.
- **A bad text command says so.** Fourteen of them caught their own errors, wrote a
  well-phrased usage message to a console nobody reads, and carried on as if nothing had
  happened, so `[color:notacolour]` in a boss's dialogue produced silence and no colour.
  They reach the error screen now, which is what `[font:x]` in the same switch always did.
  This matters more from here on: v0.7 is a boss written largely in text commands.
- **The window title and Discord Rich Presence stop naming Create Your Frisk.**
  `ControlPanel.WindowBasisName` built "Create Your Frisk v0.6.6 LTS 4" out of three
  variables, which is why v0.3's 273-marker sweep and v0.4's fourteen both missed it:
  nothing in the file contained the fork's name to search for. It is the Windows title bar
  and, on every platform, the game name and icon tooltip in Discord, so it was in people's
  friends lists. It reads the build's own version now.
- **The name screen accepts an empty name**, falling back to the default. A controller can
  reach the buttons and cannot produce a letter, so refusing would leave a pad player on a
  screen with no way out.
- Twenty inherited notes stopped citing a version number that reads as ours. They said
  "Restore in 0.7" or "Remove this for 0.7", meaning Create Your Frisk's 0.7, in a project
  that now has its own. Each states Soulbound's decision instead. Two were design questions
  rather than cleanups and are recorded as open: whether a boss may heal the player above
  their maximum, and what `sprite.spritename` should report once an animation is running.

### Fixed

Four of these were found by collapsing retro mode's branches. They had been hiding as
compatibility for years and none of them were cosmetic:

- Enemy HP silently raised its own maximum when a script set `hp` above `maxhp`.
- A wave script's exceptions were swallowed whole, so a broken wave failed in silence.
- Empty dialogue tables were accepted where a non-empty array or a string was required.
- Non-persistent bullets survived the wave that spawned them.

Two more came from playing the rebuilt name screen, and neither was visible in the diff
that caused them:

- **The arrow keys looked dead on the name screen** whenever the pointer happened to rest
  over a button. The mouse re-selected whatever it was over on every frame, so a keypress
  moved the selection and the pointer dragged it back before the next frame drew. The mouse
  only claims the selection when it has actually moved now; a click still works wherever
  the pointer is sitting.
- **The name screen's instruction overlapped the name being typed.** It was two lines and
  the name is drawn just under it. One line now, and a short one: the text is anchored on
  the left rather than centred, so it runs off the right edge at around thirty characters.
- **Answering "no" on the confirm screen put the deleted letter grid back.** The name
  screen switches the grid off when it loads, and the confirm screen switched it back on
  when a player said the name was wrong and went back to edit it. The grid was removed one
  commit earlier and the path that resurrected it was never played. It also showed a
  different instruction line from the one the screen loads with; there is one string now.
- **The error screen sent most players to a log file that was not there.** It named
  `output_log.txt` in the save folder, which is where Unity puts it on Windows and nowhere
  else: Linux writes `Player.log` to that folder and macOS writes it under
  `~/Library/Logs`. Three call sites, all of them printed at the moment someone is being
  asked to find a log and attach it. `Read me first.txt` had the same single wrong path
  and now lists all three.

### Notes

- **The catch-up ceiling was reached and it costs a tenth of a second.** `MaxCatchUp = 5` was
  inherited rather than chosen, and nothing had ever been slow enough to test it. The fourth
  stress turn is: 1500 bullets in a 500x300 arena drag the software renderer to 8fps and drop
  steps, reporting `t4 1500b g10.0 w10.1 8fps d7` and `d5` on a second run. Seven discarded
  steps out of six hundred cost 0.1s on a ten second wave, and the soul still moved under the
  load. The ceiling stays at five, now for a reason.

  Turn two of the same run says where the line is: at 12fps a frame owes exactly five steps,
  all five run, and nothing drops. Worse than 12fps drops. 288 bullets had been sitting on
  that line, which is why every run before this one reported zero.
- **Text typing is already frame-rate independent**, which is the opposite of what the
  milestone recorded for a while. `TextManager` accumulates `Time.deltaTime` against a
  constant named `singleFrameTiming` holding a twentieth of a **second**, and catches up the
  letters a long frame owed rather than losing them, so `[speed:x]`, `[w:x]` and
  `[waitall:x]` measure real time on any frame rate and follow `Time.timeScale` already.
  Nothing needed moving onto the battle tick.

  It was written down wrong because the constant is named after frames and the API page
  described those commands in frames. Reading the names rather than the arithmetic is what
  produced the mistake.

  The page says seconds now. `[w:x]` waits `x` twentieths of a second, not `x * 4` frames,
  and `[speed:1]` is one character every twentieth of a second rather than one every four
  frames. `[waitall:x]` was already right, since "x times as long" is what the code does
  whatever the unit is.
- **The battle text box does not wrap.** A line wider than the box crosses its border and is
  clipped by the screen rather than continuing underneath. This is the inherited default and
  it stays: `autolinebreak` exists to turn wrapping on per encounter, and encounters place
  their own `\n` otherwise. Recorded because the failure is silent and looks like a rendering
  bug rather than a setting.
- **The ten inherited TODOs are answered.** Four became the changes above: game events
  reaching monster scripts, the attack instance lifetime, `Text.GetTextHeight` prediction,
  and the duplicated command stripper, which was three of the ten describing one problem.
  One is deferred to v0.8 with the reason written down: pixel-perfect collision re-reads a
  bullet's texture on every frame change, which costs nothing until bullets have animation.

  Three are closed as decisions rather than work, stated where the code is so the question
  is not reopened by the next person to read it. Pixel-perfect collision stays specific to
  the Player, because a boss rush needs soul against bullet and the generality would cost
  the innermost loop of every fight. The 16 pixel minimum arena stays hardcoded, because it
  is downstream of whether the Player can be resized, which nobody has asked for. And
  `isMoving` stays as it is: it exists for orange and blue bullets, so the only case that
  could tell script-driven movement from the player's own is a boss moving the soul during
  blue bullets, and Soulbound will not do that, because taking damage for movement you did
  not make is unfair.
- **The Discord application ID is still Create Your Frisk's**, and no change here can alter
  it. Discord draws the displayed game name and the icon from whatever is registered against
  the ID on its developer portal, so a player with Discord open announces the wrong game
  whatever this code sets. Soulbound needs its own application registered before anyone
  outside the team plays it, which is recorded against v0.9.
- The engine gained a local syntax check, `syntax-check.sh`. It is not a build and cannot be
  one, since UnityEngine and MoonSharp are not on the path outside the editor, but it parses
  every C# and Lua file and reports the mistakes that need no Unity to find. CI took three
  minutes to say "unexpected else"; this takes four seconds.
- This section was written as the milestone went rather than at the end of it, which is why
  it reads as a running account. v0.5 was deliberately open-ended, to run until its list was
  empty and had stopped growing, and it closed when that happened.

## 0.4 (2026-07-27)

The records v0.3 wrote but did not show, plus the first-release problems that only
appeared once people could download a build.

Shipped as `v0.4.0` and then `v0.4.1`. Everything in the second build came out of
playing the first one: sparing a boss did nothing, the folders a build needs were
missing from CI artifacts, the new title on the disclaimer screen was drawing off the
top edge, and the startup error's ESC key did not close the game it told you to close.

### Added

- Deaths per boss and overall, as `boss_<id>_deaths` and `deaths_total`. Counted when the
  game over runs to its end rather than at the moment the player dies, so a boss that
  kills them as a story beat and revives them does not charge a death.
- No-hit clears, as `boss_<id>_nohit`. Set when a boss is beaten without anything taking
  HP off the player; healing and the `Player.Hurt(0)` call used for the invulnerability
  flash both leave it intact. Once set it is never cleared.
- All five records now show on the boss select, sharing the one spare line
  `ModSelect.unity` has: cleared and clean-cleared state, best time to a tenth, tries and
  deaths. A boss the player has never picked shows nothing rather than a row of zeroes.
- An in-fight timer, off by default, with a toggle on the options screen. Display only:
  the clock runs on every fight either way, so turning it off never costs a record and
  turning it on never produces a second set of times that cannot be compared. `Battle.unity`
  has no object for it, so the text is built at runtime from the same prefab Lua's
  `CreateText` uses.

### Changed

- The disclaimer screen is Soulbound's rather than the fork's. The Create Your Frisk logo
  is hidden and the name stands in its place as text, the version reads the one the build
  was stamped with, and the four labels under it describe this game. The credits below the
  screen are deliberately untouched: they are the attribution the GPLv3 requires, and they
  name the right people.
- The options screen's last row read "Exit" and left the screen it was on ambiguous. It
  says "Exit to Boss Select".
- Releases are marked pre-release by the shape of the tag rather than always. A plain
  version tag such as `v0.4.0` publishes as a full release; a tag carrying a semver
  pre-release identifier, `v0.9.0-rc1`, publishes with `--prerelease`.

  v0.3.0 published correctly and then did not appear in the repository's Releases panel,
  because GitHub excludes pre-releases from "latest" and that panel shows the latest
  release. Marking every build before v1.0 a pre-release would have left it empty for
  years. The 0.x version number and the warning at the top of each release's notes are
  the honest signals about maturity.
- The README's version badge was static and had to be edited every milestone. It reads
  the latest release now.
- The splash screen background is black rather than dark grey, so it runs into the
  disclaimer screen instead of stepping to it. The Unity logo stays: the project is on a
  Personal licence, which requires it. A Soulbound logo can sit alongside it once one
  exists, which is recorded against v0.7.

### Fixed

- Sparing a placeholder boss did nothing. All three encounters set `Encounter.Spare` aside
  in a way that never removed the enemy, so the only way out of a fight was to kill it or
  die, and the boss select's clear could not be reached by the route the docs describe.
  Sparing now ends the fight and records the clear, the same as killing does.
- The mod's empty folders were missing from the builds CI attaches to a run.
  `actions/upload-artifact` drops dotfiles unless told otherwise, which took the `.gitkeep`
  files with it and, since the folders held nothing else, the folders too. A build taken
  from a run therefore started with an error about `Sprites`. Release zips were never
  affected, because `release.yml` packs them on the runner before uploading.
- The disclaimer's title rendered off the top of the screen. It was cloned from the version
  label, which anchors to the top edge, and then given the logo's position, which is
  measured from the middle of the screen.
- The error shown when the engine cannot find its `Mods` folder. It read "error in script
  CYF's Startup", asked whether the folder exists, and told the player to press ESC to
  restart, which reloads the title screen, runs the same lookup and returns to the same
  screen. It now names Soulbound, says the likely cause (running the build from inside its
  zip, which Windows allows by unpacking the program alone into a temp folder), says to
  extract the archive, and prints the path it searched from. ESC closes the game, since
  nothing else helps.

  The v0.3.0 zips are correct. `Mods` sits beside the executable in all three.

- ESC did not close the game from that screen, which is the only thing the screen tells
  the player to do. Failing the lookup left the data root unset, and the next registry
  call combined a null path and threw out of `GlobalControls.Awake`. Unity disables a
  component that throws in `Awake`, so `GlobalControls.Update` stopped running, and the
  Escape handler lives there. `StaticInits.Start` now settles the data root before it
  touches a registry and gives up if there is none.

- Fourteen strings naming the fork that a player or a boss author can reach: the crash
  handler's log pointer, the infinite loop handler, the keybind parse and load errors,
  four item pool warnings, the item box, and four options descriptions. v0.3 cleared the
  documentation and the menus and missed everything that only renders when something goes
  wrong. The save incompatibility error was rewritten rather than renamed, since it ended
  with "thanks for following my fork! ^^".

  Identifiers are deliberately untouched: `CYFException`, `isCYF`, `CYFSwitch`,
  `CYFDiscord`, `CYFRetroMode`, `CYFWindowScale`, `cyfshaders` and the two shader keywords
  are API and storage keys that mods and shaders bind to by name.

### Added

- `Read me first.txt`, shipped in every release zip. Extracting the archive before running
  is the one thing a player has to know and the only failure the engine cannot recover
  from, so it needs saying before they reach an error screen. It also records where saves
  and the output log live.
- A Download section in the README. The only instructions there were for building from
  source in Unity, with no link to the builds.

### Notes

- The release notes extractor keys on the minor version: it takes the tag, drops the
  patch component, and looks for that heading. So tagging `v0.4.1` pulls the `## 0.4`
  section, not a `## 0.4.1` one. That is deliberate, patch releases share their minor's
  notes, but it is surprising if you have not read
  [`release.yml`](.github/workflows/release.yml).
- v0.4 is the first milestone whose build was played end to end before it shipped, rather
  than reasoned about from the source. Three of the fixes above came out of that and
  nothing else would have found them: sparing did not work, the artifacts were missing
  folders, and the new title was drawing off the top of the screen. Reading the code had
  said all three were fine.

## 0.3 (2026-07-26)

**This is a technical pre-release. There is no game in it yet.**

What works is the loop: name your character, pick one of three placeholder bosses,
fight it, and come back to the list with it marked CLEARED. The bosses are a 10 HP
monster with no sprite and a wave that fires nothing, so a fight is over in seconds.
That is deliberate. v0.3 was about building the loop; v0.6 is the first real boss.

### Running it

Download the zip for your platform, unzip it anywhere, and run `Soulbound-<platform>`.
On macOS, read `How to run Soulbound on Mac.txt` inside the zip first, or Gatekeeper
will refuse to open the app.

Save data lives in `PaperTrail/Soulbound/` under your platform's application data
folder. Nothing from an earlier build carries over: v0.3 renamed the product, which
moved that folder.

Bug reports are welcome. The rest of this section is the engineering detail.

### What this milestone was

The boss rush loop: pick a boss, fight it, come back to the list. That is the loop
the whole game hangs off, so it lands before anything decorative. Alongside it, the
three things that had to happen before a first release: a way to cut one, an engine
that does not call itself somebody else's name, and a product that does not either.

### Added

- The boss registry, `Assets/Mods/Soulbound/Lua/bosses.lua`. An ordered list of
  bosses, one entry per row of the select screen, each naming an encounter script by
  `id`. Adding a boss needs no C# change. Unrecognised keys are ignored, so a portrait
  or a music field can be written before the menu is ready to read it.
- `Assets/Scripts/Menus/BossRegistry.cs` reads it. Because the file is data rather
  than gameplay it runs in a bare MoonSharp sandbox instead of through `ScriptWrapper`,
  which binds a battle API that does not exist outside the Battle scene. A missing
  file, invalid Lua, a missing or duplicate `id`, an `id` with no matching encounter
  script, or an empty list all reach the error screen naming the problem.
- `Assets/Scripts/Save/BossRecords.cs`. Three AlMighty globals per boss:
  `boss_<id>_cleared`, `boss_<id>_attempts` and `boss_<id>_best`. v0.3 shows only the
  cleared marker; v0.4 gives the other two somewhere to appear.
- Every fight is timed, unconditionally. The clock is `Time.realtimeSinceStartup`, so
  `Time.timeScale` cannot distort it from Lua, and it starts after the encounter script
  has run so loading is not counted against the player. There is no toggle: two sets of
  times that cannot be compared, and a way to lose a personal best by forgetting a
  setting, are worse than always measuring.
- `Assets/Scripts/Save/PlayerProfile.cs`. The player's name, stored as the AlMighty
  global `player_name` beside the boss records, so one file holds the whole profile and
  wiping `save.gd` cannot clear the name while leaving the records.
- The boss select sends anyone with no stored name through the existing `EnterName`
  scene first. Both routes off the disclaimer screen end at the boss select, so that one
  check catches every player exactly once. Before this, the default route never asked.
- A "Change name..." row on the options screen.
- Two more placeholder bosses, so the list, paging and selection are exercised by more
  than one row.
- [docs/basics/adding-a-boss.md](docs/basics/adding-a-boss.md) and
  [docs/project/boss-rush-loop.md](docs/project/boss-rush-loop.md).
- `.github/workflows/release.yml`. Pushing a `v*` tag builds Windows, macOS and Linux,
  zips each as `Soulbound-<version>-<platform>.zip`, and publishes a GitHub release with
  the three archives attached. The tag is the only place the version is written: it
  reaches the executable through `versioning: Custom`, `BuildScript.Build` and
  `PlayerSettings.bundleVersion`.
- Release notes are pulled from the section of this changelog matching the tag. If no
  section matches, the release still publishes with a pointer here instead.
- A releasing section in [docs/project/building.md](docs/project/building.md).

### Changed

- `productName` is `Soulbound` and `companyName` is `PaperTrail`. Both feed
  `Application.persistentDataPath`, so this moves `save.gd`, `AlMightySave.gd` and the
  output log. Done once, immediately before the first tag: doing it after a release
  would orphan real save files. `GlobalControls.SaveVersion` goes to 2 so a save copied
  across from the old path gets a readable message.
- The mod selector is the boss select. It paged through mod folders found by scanning
  the disk; it pages through the registry. The title is the boss name, the line under it
  the subtitle, the scrolling list jumps straight to a boss, and Confirm starts the
  fight. `ModSelect.unity` is untouched: `SelectOMatic` binds to it through inspector
  fields, so changing what it lists needed no scene edit.
- Portraits come from `Sprites/Bosses/<id>.png`, falling through to the engine's black
  background until art exists.
- `DiscordControls.StartModSelect` became `StartBossSelect`, and the two presence
  strings a player sees went from "Selecting a Mod" and "Playing Mod: X" to "Choosing a
  boss" and "Fighting X".
- The options list is stacked and hover-tested from the buttons' real positions rather
  than from fixed scene coordinates and a ladder of hardcoded bands, so an option can be
  retired without leaving a hole.
- `ControlPanel.BasisName` was `Rhenao`, upstream's joke default, visible in the battle
  stats bar and in every `[name]` substitution. It is `Soul`, and since the boss select
  asks for a name it should never be seen.
- `build.yml` and its three jobs were named after the fork, and its artifacts and
  staging folder were `CreateYourFrisk`. `release.yml` already produced `Soulbound`
  zips, so the two disagreed.
- The documentation describes the Soulbound engine rather than Create Your Frisk. Gone
  are 273 `<CYF>` markers, 5 `<0.2.1a>` markers and roughly 150 prose references across
  35 pages. Attribution, the origin note, and the passages where Unitale genuinely is the
  subject all stay. The project is GPLv3 by inheritance, and the licence requires the
  attribution.
- `docs/how-to-read.md` lost the version-marker section and its example was rebuilt: it
  demonstrated `Screen.DispImg`, an overworld function deleted in v0.1.
- The Unity editor menu for building shader AssetBundles is `Soulbound`, not
  `Create Your Frisk`.
- `How to use CYF and add mods (Mac).txt` became `How to run Soulbound on Mac.txt`,
  rewritten so it no longer hardcodes an application name.
- `CONTRIBUTING.md` listed four engine files that name content directly. Three of them
  were deleted in v0.1 and v0.2. It lists the one that remains.

### Removed

- **Safe mode**, a swear filter: a `ControlPanel` field, two call sites, fifteen lines
  of options UI, and a commented-out block of flee texts that was the only place it
  filtered anything. The `safe` Lua global goes with it, which is an API break for a mod
  reading it. There are no such mods, and it never gets cheaper.
- **Crate Your Frisk**, a joke reskin, across 21 files: the options toggle, the garbled
  alternative for every string on the disclaimer, title, name entry, boss select,
  options and keybinding screens, the crate logos, the meowing ACT, the Temmie item
  message, the 24-line MERCY monologue, the negative damage on a hit, `Temmify.cs`, and
  nine sprites including `tiembt_0.png`, which nothing referenced at all. It was in this
  milestone specifically because the options screen offered a toggle labelled Crate Your
  Frisk, which is the branding the rest of v0.3 spent eleven commits removing.
- Mod browsing from the selection screen: the four-level deep search, the folder
  hierarchy it built, the two passes that sorted it, the `ModPage` model behind them, and
  the encounter sub-list. There is one mod and the registry is the list.

### Not done yet

- **Retro mode.** 54 lines across 18 files, and unlike safe and crate modes it changes
  gameplay semantics: enemy HP clamping, wave argument parsing, state transition rules,
  projectile positioning and rotation, sprite active semantics, and script
  call-existence checks. It is the same shape of problem `IsOverworld` was and gets the
  same treatment in its own milestone. Doing it alongside cosmetic work is how a working
  battle path gets broken quietly.
- **The timing display.** v0.3 records best times and attempts and shows neither.
- **A purpose-built boss select.** The screen is the repurposed mod selector, which is
  why its objects are still named `ModTitle`, `EncounterCount` and `encounterBox`.
  Renaming them means editing the scene, which is v0.7 work alongside the art. The
  retired options rows are hidden at runtime for the same reason.

## 0.2 (2026-07-26)

Removes what the overworld strip left behind. Nothing here changes what the game
does, because every branch deleted was already unreachable: v0.1 left
`UnitaleUtil.IsOverworld` as a shim returning `false`, and this milestone walks the
branches that consulted it and deletes the dead half of each.

### Changed

- `GlobalControls.OverworldVersion`, a version string, became `GlobalControls.SaveVersion`,
  an integer. The old check compared version strings ordinally, which would have accepted a
  pre-v0.2 save and then failed to read it. `SaveLoad` now refuses any save written to an
  older format with a readable message.
- `UnitaleUtil.ExitOverworld` became `ResetSession`. Since v0.1 it only cleared music
  channels, session globals, the inventory and the player character.
- `GlobalControls.canTransOW` became `escapableScenes`, and
  `GlobalControls.overworldTimestamp` became `sessionTimestamp`.
- `TextManager OW.prefab` became `TextManager Name.prefab`, keeping its GUID so the three
  scenes that use it are unaffected.
- The disclaimer screen said "Press Menu to go to the Overworld". It now says "Title
  Screen", which is where that key has actually led since v0.1.
- The options screen described the save file as "the save file used for CYF's Overworld".
  It now describes what the file holds.
- The engine is 111 C# files and 19,283 lines, down from 19,505.

### Removed

- All 40 remaining `UnitaleUtil.IsOverworld` branches and the flag itself:
  `GameOverBehavior` (18), `SpriteUtil` (8), `Inventory` (5), `TextManager` (3),
  `LuaScriptBinder` (2), and one each in `LuaProjectile`, `LuaTextManager` and
  `GlobalControls`.
- `GlobalControls.isInShop`, `nonOWScenes`, `realName`, and the `GameMapData`, `EventData`
  and `TempGameMapData` dictionaries. `EventData` was never read or written at all.
- The map fields on `GameState`: `lastScene`, `mapInfos`, `tempMapInfos`, and the
  `MapData`, `TempMapData`, `EventInfos` and `Vect` structs. This changes the save format,
  which is why `SaveVersion` exists.
- `UnitaleUtil.VectToVector` and `VectorToVect`, which had no callers.
- The `PlayerPosX`, `PlayerPosY`, `PlayerPosZ` and `PlayerMap` session globals. These held
  overworld coordinates. They were readable from Lua with `GetRealGlobal`, so this is a
  small API change.
- The four `GameOverBehavior` fields that fell dead with the branches:
  `gameOverContainerOw`, `canvasOW`, `canvasTwo` and `utHeart`.
- The two `TextManager` branches matching on the name `"TextManager OW"`. All ten scene
  instances of that prefab override the object name, so neither branch could ever run.
- The title screen's last-map line, which had been showing a value nothing set since v0.1.
- Dead `Canvas OW` and `Canvas Two` lookups in `ErrorDisplay` and `SelectOMatic`.

### Documentation

- 35 overworld references corrected across eight API pages, heaviest in
  `misc-functions.md` (11), `sprites-and-animation.md` (9) and `discord.md` (6). Several
  described behaviour that genuinely changed in v0.1, such as `Misc.ResetCamera` and the
  Discord presence states.
- `sprite.z` and `sprite.absz` were documented as working "in the Overworld only". They are
  plain transform Z accessors and still work; the pages now say battle rendering orders by
  layer, so they rarely change what you see.
- The engine architecture and repository layout pages record the v0.2 state, including why
  the save path is kept even though only name entry calls `SaveLoad.Save()`.

## 0.1 (2026-07-26)

Removes the overworld. Soulbound is a boss rush: you pick a boss from a menu and
fight it, and there are no maps, events, cutscenes or shops. Roughly a third of
Create Your Frisk existed to support them.

This is the first of two milestones on the overworld. v0.1 deletes the feature and
leaves `UnitaleUtil.IsOverworld` as a shim returning `false`, so every branch that
consults it collapses to the battle path on its own. v0.2 walks those 48 branches
and deletes the dead half of each. Splitting it that way keeps the destructive work
and the subtle logic changes in separate diffs.

### Changed

- A battle now always returns to the mod selector. It used to return the player to
  the map they came from unless the battle was started from the mod selector. The
  mod selector stands in for the boss select screen that v0.3 builds.
- Death restarts instead of respawning. The game over sequence used to reload the
  save, instantiate a teleport prefab, and drop the player back at their last save
  point on the map they died on.
- The title screen and name entry lead to the mod selector, not into a map.
- `Assets/Scripts/Overworld` is now `Assets/Scripts/Save`. Six of its 18 files were
  never overworld code. `SaveLoad`, `GameState` and `PermanentGameState` stayed
  where they were under the folder's new name; `Title`, `IntroManager` and
  `EnterNameScript` moved to `Assets/Scripts/PregamePlaceholder` with the other
  menu screens.
- `UnitaleUtil.IsOverworld` is a constant `false`.
- `UnitaleUtil.ExitOverworld` kept only its non-overworld teardown: music channels,
  session globals, the inventory, and the player character.
- The engine is 111 C# files and 19,505 lines, down from 129 and 25,334.

### Removed

- `Assets/Scripts/Overworld`: `PlayerOverworld`, `EventManager`, `ShopScript`,
  `TransitionOverworld`, `ItemBoxUI`, `CYFAnimator`, `TPHandler`, `Fading`,
  `EventOW`, `MapInfos`, `MapLoader` and `SpecialAnnouncementScript`. 12 files,
  4,534 lines.
- `Assets/Scripts/Lua/CLRBindings/Overworld`: the `Event`, `General`, `Player`,
  `Screen`, `Map` and `Inventory` overworld Lua objects, their registrations in
  `LuaScriptBinder`, and the `FPlayer`, `FEvent`, `FGeneral`, `FInventory`,
  `FScreen` and `FMap` globals. 6 files, 1,088 lines.
- The `TransitionOverworld`, `Shop` and `SpecialAnnouncement` scenes, and their
  entries in `EditorBuildSettings.asset`. Nine scenes remain.
- Eight prefabs: `Canvas OW`, `Main Camera OW`, `Player`, `Background 1`, `Event1`,
  `ImageEvent`, `Save`, `TP On-the-fly`, and the empty `Prefabs/Maps` folder.
  `TextManager OW.prefab` was kept: `EnterName` instantiates it.
- Six sprite folders from `Assets/Default/Sprites`: `FriskUT`, `AsrielOW`,
  `CharaOW`, `MonsterKidOW`, `BoosterOW` and `SavePoint`. About 1.2 MB.
- `Assets/Tiled2Unity`, the Tiled map importer. 53 files, 564 KB. v0.0 kept it on
  the reasoning that it would be needed to author future maps; a boss rush has no
  maps.
- The overworld pause menu, the overworld camera shift, the overworld rich
  presence, the kept-audio channel that carried music across the overworld
  boundary, and `UnitaleUtil.MapCorrespondanceList`.
- The 11 overworld documentation pages and the two images only they used. 35 pages
  remain.

## 0.0 (2026-07-26)

The first version. Turns an unmodified Create Your Frisk v0.6.6 LTS 3 snapshot
into a clean, documented base to build a game on. No gameplay is implemented.

### Added

- A documentation set of 46 Markdown pages under `docs/`, converted by hand from
  the 644 KB Bootstrap website that Create Your Frisk shipped. It covers the
  whole engine: text commands, game events, every Lua object, projectiles,
  pixel-perfect collision, sprites and animation, shaders, and the overworld,
  plus the key, item, and dialog bubble references. Content is preserved; only
  the presentation changed.
- Three project pages written for this repository:
  [repository layout](docs/project/repository-layout.md),
  [building](docs/project/building.md), and
  [engine architecture](docs/project/engine-architecture.md).
- `Assets/Mods/Soulbound/`, the mod folder the game will live in, scaffolded with
  the standard Create Your Frisk layout and a placeholder encounter, monster, and
  wave. Without at least one mod containing a non-`@` encounter script, the mod
  selector stops with "Your mod folder is empty!", so the placeholder exists to
  keep the engine bootable. The placeholder monster draws the `empty` sprite from
  `Assets/Default/Sprites`, so the mod ships no art of its own.
- A `tools/` directory for build and content tooling.
- Community documents: a contributing guide, a code of conduct, a security
  policy, a support page, and project terms.

### Changed

- The Unity project moved from `engine/` to the repository root. This restored
  the root-relative paths that `.gitignore` and `.github/workflows/build.yml`
  already assumed.
- `Build.py` and the CI workflow now copy `docs/` into builds, in place of the
  removed website folder.
- `README.md` and the bug report template were rewritten for this project rather
  than the upstream engine.
- `actions/checkout` moved from v2 to v5, which runs on Node 24.
- `game-ci/unity-builder` moved from v2 to v4. The v2 action used a legacy
  licensing module that depended on an activation file flow GameCI has since
  deprecated, which left it with no working way to activate a licence. This was
  the change that got CI building.

### Removed

- Roughly 37 MB of upstream example content: the six example mods
  (`@0.5.0_SEE_CRATE`, `@OverWorld Test`, `Examples`, `Examples 2`, `RTLGeno`,
  `Encounter Skeleton`), eleven demo overworld scenes, the demo Tiled2Unity map
  data, and the demo map prefabs. The Tiled2Unity importer itself was kept, since
  it is needed to author maps.
- `Assets/Scripts/Tests`, six scripts with no scene or code references.
  `Assets/Scripts/Why` was kept: `DogGyrator.cs` is used by `Error.unity` and
  `Temmify.cs` by five engine files.
- The Bootstrap documentation website and the GitHub Pages workflow that
  published it.
- The upstream Discord notification workflows, which targeted the Create Your
  Frisk Discord and could not work here.
- The Unity activation workflow. Its core action was deprecated upstream and now
  fails on purpose; licences are obtained locally through Unity Hub instead. See
  [docs/project/building.md](docs/project/building.md).

### Fixed

- Four places in the engine named the removed demo content directly and were
  repaired: the scene list in `ProjectSettings/EditorBuildSettings.asset`,
  `AddKeysToMapCorrespondanceList()` in `Assets/Scripts/Util/UnitaleUtil.cs`, and
  the hardcoded `FirstLevelToLoad` and `ModFolder` values in
  `Main Camera OW.prefab`, `Canvas OW.prefab`, and `Background 1.prefab`.

### Notes

- The repository dropped from 3841 to 2311 tracked files, and from about 101 MB
  to 65 MB.
- Scenes are loaded by name from C#, never by build index, which is what made
  removing the demo maps safe. Twelve engine scenes remain.
- The overworld has no first map yet, so `FirstLevelToLoad` is empty on both
  overworld prefabs. `TransitionOverworld` guards this with
  `FileLoader.SceneExists()` and shows a readable error, so it degrades cleanly
  until a map exists.
- CI builds Windows, macOS, and Linux successfully, which is the first
  confirmation that the cleanup compiles.
