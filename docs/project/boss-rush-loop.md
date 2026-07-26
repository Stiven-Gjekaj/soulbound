# The boss rush loop

Pick a boss, fight it, come back to the list. That is the whole game, and everything else
in the project hangs off it. This page describes the machinery. For adding a boss to the
list, see [Adding a boss](../basics/adding-a-boss.md).

## The loop

| Step | What happens | Where |
| --- | --- | --- |
| Boot | `GlobalControls.Awake` loads the AlMighty globals, so the records are ready | `GlobalControls` |
| Boss select opens | the registry is re-read, the list is drawn | `SelectOMatic.Start` |
| The player picks a boss | the attempt is counted, `StaticInits.ENCOUNTER` is set, the Battle scene loads | `SelectOMatic.LaunchBoss` |
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

They are AlMighty rather than session globals for two reasons. They survive a wiped save
file, which is the right behaviour for a record of what you have done; and v0.4 hangs its
death counts and its record display off the same mechanism, so this is the milestone that
proves it works.

A boss script can read them like any other AlMighty global:

```lua
if GetAlMightyGlobal("boss_placeholder_cleared") then
    -- the player has beaten this boss before
end
```

Writing them from Lua is possible and not recommended. Nothing stops it, but a boss that
marks itself cleared has made the record mean nothing.

v0.3 shows only the cleared marker. Best time and attempts are recorded and have nowhere to
appear until v0.4 gives them one.

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

The clock is `Time.realtimeSinceStartup`, which `Time.timeScale` cannot distort, so a boss
script cannot slow the timer down by slowing the game down. It starts after the encounter
script has run, so loading is not counted against the player.

Two questions have to be answered before times are comparable between machines, and both
belong to v0.5:

- **Framerate drops.** The game caps at 60 with vsync off in `ScreenResolution.Start`, which
  settles high-refresh displays, but a slow machine still ticks per-frame wave logic fewer
  times per second.
- **`Time.timeScale` during a timed fight.** The timer is immune to it, but the fight is
  not. A boss that halves time scale halves how much the player has to do per wall-clock
  second.

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
