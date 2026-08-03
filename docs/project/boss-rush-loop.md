# The boss rush loop

Pick a boss, fight it, come back to the list. That is the whole game, and everything else
in the project hangs off it. This page describes the machinery. For adding a boss to the
list, see [Adding a boss](../basics/adding-a-boss.md).

## The loop

| Step | What happens | Where |
| --- | --- | --- |
| Boot | `GlobalControls.Awake` loads the AlMighty globals, so the records are ready | `GlobalControls` |
| Boss select opens | the registry is re-read, the seven slots are drawn | `BossSelect.Start` |
| The player picks a boss | the attempt is counted, `StaticInits.ENCOUNTER` is set, the Battle scene loads | `BossSelect.LaunchBoss` |
| The fight begins | the encounter script has run and the first state is entered, so the clock starts | `UIController.Start` |
| The last enemy leaves | the boss is marked cleared and the time is kept if it beats the stored one | `UIController.CheckAndTriggerVictory` |
| The battle ends | the clock stops, the boss select loads again | `UIController.EndBattle` |

Every way out of a fight goes through `EndBattle`: winning, dying, fleeing, and pressing
Escape. Only the victory path writes a clear.

Returning from a fight leaves the cursor on the boss just fought. Arriving from anywhere
else, the disclaimer screen or the title screen, starts at the first boss.

## Records

`Assets/Scripts/Save/BossRecords.cs` keeps three values per boss, stored as AlMighty
globals so they are written to disk the moment they change:

| Global | Type | Meaning |
| --- | --- | --- |
| `boss_<id>_cleared` | boolean | the boss has been beaten at least once |
| `boss_<id>_attempts` | number | fights started, including the ones that went badly |
| `boss_<id>_best` | number | fastest clear, in seconds |
| `boss_<id>_deaths` | number | times the player died to this boss |
| `boss_<id>_nohit` | boolean | the boss has been cleared without taking a hit |
| `deaths_total` | number | deaths across every boss |

They are AlMighty rather than session globals because they survive a wiped save file,
which is the right behaviour for a record of what you have done.

Three of them have rules worth knowing:

- **Deaths are counted when the game over runs to its end**, not at the moment the player
  dies. A boss that kills the player as a story beat and revives them does not charge a
  death for it.
- **A no-hit clear is one where nothing took HP off the player.** Healing does not break
  it, and neither does the `Player.Hurt(0)` call bosses use for the invulnerability flash
  alone. Once set it is never cleared, so a clean clear stays on the record even if the
  next win is messier.
- **Attempts are counted once the encounter has loaded.** A boss whose script fails to
  compile does not cost the player a try.

A boss script can read them like any other AlMighty global:

```lua
if GetAlMightyGlobal("boss_placeholder_cleared") then
    -- the player has beaten this boss before
end
```

Writing them from Lua is possible and not recommended. Nothing stops it, but a boss that
marks itself cleared has made the record mean nothing.

All of it appears on one line in the boss select, under the boss's name: whether it is
beaten and whether it was beaten clean, the best time, tries, and deaths. A boss the
player has never picked shows nothing, so an untouched list stays clean.

That is one line because `ModSelect.unity` has exactly one spare. A screen with room for a
table is v0.6, when every screen is rebuilt unskinned.

## The player profile

The player's name is the other thing stored per player rather than per session. It sits in
the AlMighty global `player_name`, beside the boss records, so one file holds the whole
profile and wiping `save.gd` cannot clear the name while leaving the records behind.

The boss select asks for it once. Arriving there with no stored name, and not returning
from a fight, sends the player to the `EnterName` scene first. Both routes off the
disclaimer screen end at the boss select, so that one check catches every player exactly
once. After that, the options screen has a "Change name..." row.

`PlayerCharacter.Reset` runs at the end of every battle and restores the name from the
profile. That has a useful consequence: a boss script may set `Player.name` mid-fight for
effect, and the player's real name comes back when the fight ends.

## Sparing counts

The clear is recorded in `CheckAndTriggerVictory`, which fires when the fight has no
enemies left. Killing the boss and sparing it both get there, and both count. A boss you
got past is a boss you got past.

## Timing

Timing is unconditional. Every fight is timed whether or not anything displays it. Making
that a setting would produce two sets of times that cannot be compared with each other, and
a way to lose a personal best by forgetting a toggle. There is no speedrun mode for the
same reason: it would only gate something that is always on.

The clock counts battle steps. A fight advances in whole steps of a sixtieth of a second
(see [the battle tick](#the-battle-tick) below), and a time is the number of steps the
fight took divided by sixty. On a machine holding 60fps that is the same number a
stopwatch would give. Where it differs, the step count is the honest one: it measures what
the fight actually did rather than how long the player sat in front of it, so a stutter, a
slow machine, or a boss slowing time down cannot inflate a record for the same work. The
clock starts after the encounter script has run, so loading is not counted against the
player.

The options screen has an in-fight timer, off by default, which shows the running time in
the corner of the battle screen. It is a display setting, not a mode: the clock runs
either way, so turning it off never costs a record and turning it on never produces a
second set of times that cannot be compared with anyone else's. `Battle.unity` has no
object for it, so `FightTimer` builds the text at runtime from the same prefab Lua's
`CreateText` uses; it wants a proper scene object when the battle screen is rebuilt at
v0.6.

## The battle tick

The fight advances on its own clock rather than on the frame rate. `BattleTick` accumulates
elapsed time and runs whole steps of `1/60` of a second, and every part of the fight that
decides what happens advances one step at a time, in this order:

| Order | What | Why here |
| --- | --- | --- |
| 1 | the arena | it is the boundary everything else is measured inside |
| 2 | the attack bar | so the state machine reads where it is when Confirm is pressed |
| 3 | the player | the soul moves |
| 4 | the encounter and wave scripts | the boss's `Update` hooks, which move bullets |
| 5 | projectiles | hitboxes and collision, against where everything ended up |

Rendering still happens every frame and shows the last step's state.

This exists because the fight used to advance once per rendered frame while the player
moved by elapsed time. A machine holding 30fps ran a four second wave 120 times instead of
240, so every bullet written as movement per update covered half the ground while the soul
kept its full speed. The fight was not slower on slow hardware, it was easier.

Two consequences worth knowing:

- **A wave script's `Update` runs exactly 60 times a second.** It used to run once per
  frame, "usually 60, depending on the player's framerate". Patterns can be written against
  that number now.
- **A frame that owes more than five steps drops the backlog** instead of repaying it.
  Catching up without a ceiling turns one slow frame into a slower one, which owes more
  steps again.

A fixed step was chosen over scaling movement by elapsed time because the Lua API is public
and its patterns are written as movement per `Update`. Scaling would have silently changed
the speed of every pattern ever written; this leaves them meaning exactly what they meant.

## What is not here

The records are keyed per boss. A full-gauntlet run, all bosses back to back, would want a
run-level record alongside these rather than instead of them. That is after v1.0, and it is
the feature that makes mid-run saving matter, which is why `save.gd` and `SaveLoad.Save()`
are being kept.

## Related

- [Adding a boss](../basics/adding-a-boss.md) for the registry format
- [Engine architecture](engine-architecture.md) for the scene flow around this
- [Misc. functions](../api/objects/misc-functions.md) for the AlMighty global API
- [Milestones](milestones.md) for what comes next
