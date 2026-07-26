# Misc. functions

This section details functions Unitale adds to your Lua scripts to interact with the game
in various ways. All functions will have a suffix in square brackets to denote in which
scripts they may be used. See [How to read this documentation](../../how-to-read.md) for
more details.

## All-script functions

### `DEBUG(string text)` [E/M/W]

Write text to the debug console (toggleable with `F9`). It will appear automatically the
first time you write text to it. You can use this to check values in your code, or make
sure some pieces of code are actually running.

### `EnableDebugger(boolean bool)` [E/M/W]

Forcefully allows and disallows the showing of the debugger. If you enter `false`, the
debugger will be immediately hidden if it is open, and will no longer show itself for any
reason. If you then enter `true`, the debugger will not appear again immediately, but can
be re-opened if the user presses F9.

The debugger is enabled by default.

Note: The state of the debugger also applies to the state of the FPS display.

### `SetGlobal(string your_variable_name, value)` [E/M/W]

An alternative to setting variables in the Encounter script to be accessed from anywhere
(see [The Script object](script.md)).

Sets a global variable. After setting, you can retrieve it from all your scripts at any
time with `GetGlobal(variable_name)`.

### `GetGlobal(string your_variable_name)` returns variable [E/M/W]

An alternative to setting variables in the Encounter script to be accessed from anywhere
(see [The Script object](script.md)).

Gets a Global Variable that you previously set using `SetGlobal()`.

### `SetRealGlobal(string your_variable_name, value)` [E/M/W]

Sets a Global variable that any script can read. After setting it, you can retrieve it
from all of your scripts at any time with `GetRealGlobal(variable_name)`.

Persists through battles, but not between sessions. These variables are written to the save
file when the game saves.

Note: "Complex" variables such as tables, functions and userdata can not be saved as Real
or AlMighty globals.

Also note: as Real and AlMighty Globals persist across mods, it is possible for mods to
read each others' globals. Be careful when choosing global names.

### `GetRealGlobal(string your_variable_name)` returns variable [E/M/W]

Gets a Global that you previously set using `SetRealGlobal()`.

### `SetAlMightyGlobal(string your_variable_name, value)` [E/M/W]

*AlMighty Globals* are globals that are instantly saved into a file when set: these globals
**persist through sessions**.

However, you can use the `Remove AlMighty Globals` button in modDev to remove them.

Note: "Complex" variables such as tables, functions and userdata can not be saved as Real
or AlMighty globals.

Also note: as Real and AlMighty Globals persist across mods, it is possible for mods to
read each others' globals. Be careful when choosing global names.

### `GetAlMightyGlobal(string your_variable_name)` returns variable [E/M/W]

Gets an AlMighty Global that you previously set using `SetAlMightyGlobal()`.

### `SetFrameBasedMovement(boolean bool)` [E/M/W]

Set to `true` if you want frame-based player movement (2px/frame) instead of time based
player movement (120px/s). Set it to `false` if you already are in frame-based movement and
you want to go back to the time based movement.

By default, time-based movement is used, same as if `SetFrameBasedMovement(false)` were
called.

Note that this function only controls the player's movement with the default control
scheme (see `Player.SetControlOverride` in [The Player object](player.md)).

### `SetAction(string "FIGHT", "ACT", "ITEM" or "MERCY")` [E/M/W]

Used alongside `State("ENEMYSELECT")`, or `EnteringState` when entering the same state, to
force the player to choose FIGHT or ACT. This controls whether you'll see the enemy's
health bar in the menu and whether the next state upon pressing Z is `ATTACKING` or
`ACTMENU`.

If used in the state `ACTIONSELECT`, this function will move the player over the specified
button.

### `AllowPlayerDef(boolean bool)` [E/M/W]

If the given value is true, all damage that the player will take will be reduced, like in
Undertale, by 1 point for each 5 defense, with the player's defense at LV1 not affecting
the damage.

Damage taken can not be fully blocked and will always be at least 1. False by default.

### `SetPPCollision(boolean bool)` [E/M/W]

Setting this to true will force all bullets that don't have a specified collision type to
use the Pixel-Perfect collision system. If you want to use it, see
[The Pixel-Perfect Collision System](../pixel-perfect-collision.md). False by default.

### `BattleDialogue({table of string} list_of_strings)` [E/M/W] or `BattleDialog`

This makes the list of strings you give to the function appear in the UI dialog box. After
skipping through them, you will automatically go to the monster dialogue step by default.
Below is a working example of how you could use it for a Vegetoid encounter.

As of CYF v0.6.4, if you end up with 4 or more lines of battle dialog displayed at once,
the text will move up (9 pixels, one time) to compensate and try to fit your text inside
the box.

You can use `\n` to create a new line **with a star** (\*), and `\r` to create a new line
**without** a star.

```lua
function HandleCustomCommand(command)
    if command == "DINNER" then
        if ate_greens then -- ate_greens is a non-default variable, of course
            currentdialogue = {"Ate\nYour\nGreens"}
        else
            currentdialogue = {"Eat\nYour\nGreens"}
        end
        BattleDialog({"You pat your stomach.\nVegetoid offers a healthy meal."})
    end
end
```

### `CreateState(string name)` [E/M/W]

This function creates a custom state with the name `name`, which will function like the
NONE state. You cannot have two states with the same name.

### `UnloadSprite(string path)` [E/M/W]

This function removes the sprite loaded through `path` from CYF's internal cache, allowing
you to load it from your folder again.

Usually, CYF keeps all sprites it loads in an encounter in memory so the engine does not
have to load it again, which may create a lag spike. However, if the sprite is changed
during the mod, the file will not be reloaded, and only its first version will be kept
until the encounter is over.

### `State(string state_to_go_to)` [E/M/W]

A powerful function that immediately skips the battle to the specified state, rather than
following the default conventions. Below is a list of valid strings you can pass to it,
what state you'll be going to and what that state entails. The states you pass to it are
case-invariant, but uppercase is recommended for readability.

Arguably the best part about `State` is that it can be used in conjunction with the text
command `[func]` to change the state from within your dialogue. An example is below, this
will have a monster say that he will not fight you, then return to the action select
screen, instead of starting a wave.

```lua
currentdialogue = {"I won't fight you.", "[func:State,ACTIONSELECT]"}
```

- `ACTIONSELECT` - While in this state, you can select FIGHT/ACT/ITEM/MERCY.
- `ENEMYDIALOGUE` - When starting this state, `currentdialogue` gets read from the
  Encounter script for every monster and their dialogue is displayed. If that doesn't
  exist, it'll return something at random from the `randomdialogue` list.
- `DEFENDING` - When starting this state, `nextwaves` is read out from the Encounter script
  and all scripts in the Waves folder with matching names will be part of this defense
  step. `wavetimer` is also read from the Encounter script at this time.

There are some other states available, but entering some might have nasty side effects.
It's possible that they might lock up your battle if you enter them with `State`. So, use
at your own risk:

- `ATTACKING` - Starting this state will put you in the screen where you either deal damage
  to an enemy by pressing "Z" at the right time, or you wait and miss entirely. The enemy
  this affects is the same as the last enemy selected in the states `ENEMYSELECT` or
  `ACTMENU`. After this state ends, the encounter moves on to the `ENEMYDIALOGUE` state by
  default.
- `ENEMYSELECT` - This state displays a list of all active enemies in the encounter. This
  state gets entered automatically after choosing FIGHT or ACT. If the Player got here from
  choosing FIGHT, the state loaded from pressing "Z" is `ATTACKING`, if they got here from
  choosing ACT, that state will be `ACTMENU`, but if they got here from calling `State` and
  press "Z", nothing will happen.
- `ACTMENU` - This state displays all of the options from the enemy's `commands` in order.
  If the enemy has `cancheck` set to *true*, then a "Check" option will be displayed here,
  which will follow the syntax seen in the description for `commands` as seen in
  [Basic setup](../../basics/basic-setup.md). Having more than 6 ACT commands may cause
  glitchiness, though, so watch out. Additional note: the enemy chosen will always be the
  same as the last enemy chosen in `ENEMYSELECT`.
- `ITEMMENU` - This state shows all of the player's available items. It's tied with the
  `HandleItem` function (see [Game events](../game-events.md) for more information).
- `MERCYMENU` - This state lets you choose from either "Spare" or "Flee". If at least one
  active enemy has `canspare` set to *true*, then "Spare" will be yellow, and selecting it
  will spare that enemy. Otherwise, if the option is not yellow, then choosing "Spare" will
  activate the encounter function `HandleSpare`. Also, choosing "Flee" prompts some silly
  messages.
- `DIALOGRESULT` - This is the state that is entered whenever `BattleDialog` is called,
  when the victory message displays, when the player fails to flee, or when an item is
  used. When all text is done and the player presses "Z", the state `ENEMYDIALOGUE` is
  entered next. The soul is forcibly invisible during this state.

Finally, there are two "special" states that evoke behavior in the engine itself:

- `DONE` - Changing state to `DONE` will instantly end the current battle. This returns the
  player to the mod selection screen.
- `NONE` - This state does nothing. It is entered for the first frame of the encounter, but
  entering it manually will completely freeze your encounter. It might be useful if you
  want to disable all of Unitale/CYF's basic functionality.
- `RESETTING` - DEPRECATED, DO NOT USE. Only listed for completion purposes. It
  only exists in Unitale 0.2.1a, so trying to use it in CYF won't work. It was removed in
  the Github release of Unitale 0.2.1a, and seemed to be only for testing.

### Freezing states

As of CYF v0.6.2.1, calling `State("PAUSE")` will perfectly "pause" an encounter. The last
active state will remain active, but in a frozen state, until you call `State` again.

Only the `Update` function of the Encounter script will remain active here.

To unfreeze a state, you must use `State` to switch to another state, preferably the last
active state. `GetCurrentState()` will tell you the name of the frozen state.

### `GetCurrentState()` returns **string** [E/M/W]

Returns the name of the current state (see above for all states).

## Encounter script functions

### `RandomEncounterText()` returns **string** [E]

Select a random monster from the encounter, then get a random entry from the `comments`
table there. You'll want to use this to replicate default encounter behaviour. See code
below.

```lua
function DefenseEnding() --This built-in function fires after the defense round ends.
    encountertext = RandomEncounterText()
end
```

### `SetButtonLayer(string layer)` [E]

Changes the `layer` of the FIGHT, ACT, ITEM and MERCY buttons **and the Player's name, lv
and hp**.

The usable layers are created sprite layers and these base layers:

- `"Bottom"`: Under everything, even the background.
- `"BelowUI"`: Above the background.
- `"BelowArena"`: Above the background and the UI.
- `"BelowPlayer"`: Above the background, the UI and the Arena.
- `"BelowBullet"`: Above the background, the UI, the Arena and the Player.
- `"Top"`: Above everything.

Note: Enter `"default"` to reset the Buttons and UI.

### `CreateEnemy(string scriptName, number x, number y)` [E]

This function creates an `enemy script` using the script `scriptName` in the
`Lua/Monsters` folder of your mod, which will act as a new enemy in battle, and returns a
reference to it.

It will move the enemy so that it is centered just above the arena, with 1 pixel of space
in between, and will move the enemy's sprite by `x` pixels horizontally and `y` pixels
vertically.

Be aware that creating an enemy this way will NOT add it to the Encounter script's
`enemies` table, so you have to manage the table yourself by adding this new enemy script
to it.

### `Flee()` [E]

This function runs the fleeing sequence.

## Monster script functions

### `SetSprite(string filename)` [M]

Sets the monster's sprite from the Sprites folder to `filename.png`. Can be used with
`[func]` to change sprites mid-dialogue.

### `SetActive(boolean active)` [M]

Either true or false. If false, this monster will stay on screen but will not show up in
menus, do its dialogue or run any of its events. You can use this to introduce monsters to
an encounter at a later point. The battle will end when a monster is killed or spared and
there are no active monsters left. Having no active monsters at all will likely cause a
bunch of errors right now.

### `SetDamage(number amount)` [M]

Set the amount of damage the monster will take the next time it is attacked. Can be
negative.

Set `amount` to the amount of damage the monster will take, or `0` to make the attack miss.

This function can only be used within the [Game event](../game-events.md)
`BeforeDamageCalculation`, or just before using the function `Player.ChangeTarget`.

You also don't want to use it with a monster who's been disabled with `SetActive(false)`.

### `Kill(boolean playSound = true)` [M]

Kills the monster immediately. If this was the last monster, the battle ends.

Does NOT call the [Game event](../game-events.md) `OnDeath`.

If `playSound` is set to `false`, the dusting sound will not be played.

### `Spare(boolean playSound = true)` [M]

Spares the monster immediately. Similar to `Kill()`, if this was the last monster, the
battle ends.

Does NOT call the [Game event](../game-events.md) `OnSpare`.

If `playSound` is set to `false`, the sparing sound will not be played.

### `Move(number x, number y)` [M]

Moves the enemy's sprite relative to its current position.

### `MoveTo(number x, number y)` [M]

Moves the enemy's sprite relative to the bottom left corner of the screen.

This is effectively the same as setting `enemypositions` again, except `x` is 320px left
and `y` is 231px down.

### `BindToArena(boolean bind, boolean isUnderArena = false)` [M]

Controls whether the enemy's sprite will follow the Arena's movements.

If `bind` is true, the enemy will be parented to the Arena and follow all of its movements.
Otherwise, it will be either behind or in front of the arena, depending on the value of
`isUnderArena`.

### `SetBubbleOffset(number x, number y)` [M]

Makes the enemy's dialogue bubble appear `x` pixels horizontally and `y` pixels vertically
relative to its original position.

### `SetDamageUIOffset(number x, number y)` [M]

Changes the offset of the enemy's damage UI (the enemy's health bar and the red numbers).
Note that the damage UI is on a layer above that of the arena.

### `SetSliceAnimOffset(number x, number y)` [M]

Changes the offset of the attack animation (the red slice by default) for when the player
attacks this monster.

### `Remove()` [M]

This function immediately removes this enemy from the encounter, including the script it's
been called from.

Be aware that removing an enemy this way will NOT remove it from the Encounter script's
`enemies` table, so you have to manage the table yourself by removing this enemy script
from it before running this function, because any reference to it will be invalid.

## Wave script functions

### `EndWave()` [W]

Ends this wave immediately. You can only call this function from the Update function.
