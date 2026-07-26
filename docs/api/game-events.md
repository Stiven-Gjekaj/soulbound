# Game events

This section is all about game events. Game events are functions in your scripts that the
Unitale engine runs at various points in the game. By changing up your behaviour depending
on the actions the player takes, you can go beyond a basic encounter and make it great.

## Script-specific events

*Script-specific events* are, as the name implies, functions that only happen for this
specific script type.

## Encounter script events

### `EncounterStarting()`

Happens once when everything's done initializing but before any encounter actions start.
You should do things like stopping the music here, or using `State()` if you want to start
the fight off with some dialogue.

### `EnemyDialogueStarting()`

Happens when you go to the monster dialogue state. You're still free to modify monster
dialogue here.

### `EnemyDialogueEnding()`

Happens when you go from the monster dialogue state to the defending state.

### `DefenseEnding()`

Happens when you go from the defending state of the game to any other state. If you read
up on the `RandomEncounterText()` function, you'll want to use it here.

### `HandleSpare()`

Happens when you select the Spare option from the Mercy menu, regardless of whether a
monster is spareable or not. This event fires *after* the sparing of monsters is
completed. If you spare the last enemy in the encounter, this function will not happen,
the encounter is over at that point.

### `HandleFlee(boolean success)`

Happens when you select the Flee option from the Mercy menu. If you implement
`HandleFlee()`, the fleeing sequence will not run automatically, and you will have to do
it manually with the `Flee()` function.

- `success`: Whether the fleeing condition is true.

### `HandleItem(string item_ID, number position, boolean silent)`

Happens when you select an item from the item menu.

- `item_ID`: The name of the item used, **IN ALL CAPS**. Similar to `HandleCustomCommand`
  in monster scripts.
- `position`: The position of the item used in the player's inventory. The first item is
  number 1.
- `silent`: If this is true, avoid doing things like writing battle text.

In CYF, you can use the Inventory object to edit the player's inventory. The
items' names will be in caps, like with `HandleCustomCommand()`.

```lua
function HandleItem(ItemID, position)
    if ItemID == "DOGTEST2" then
        BattleDialog({"You selected The Second Dog.", "You are truly great."})
    else
        BattleDialog({"You didn't select The Second Dog.", "You could've picked better."})
    end
    DEBUG("You chose item #" .. position .. " in your inventory.")
end
```

### `EnteringState(string newstate, string oldstate)`

A new, more flexible way of handling state changes. When you enter a new state, this
function will fire with `newstate` containing the new state's name, and `oldstate`
containing the previous state's name. Both are in all caps. One of the most powerful
things about it is that you can use `State()` here to interrupt state changes initiated by
the engine itself.

Possible states and when they execute are below:

- `ACTIONSELECT` - Returning to the main part of the battle, where you can select
  FIGHT/ACT/ITEM/MERCY.
- `ATTACKING` - When you've selected a target with the FIGHT option.
- `DEFENDING` - When the enemy or enemies finish dialogue, and one or more waves start.
- `ENEMYSELECT` - When you've selected either FIGHT or ACT, and need to select an enemy.
- `ACTMENU` - When you've selected an ACT target, and must now select an ACT command.
- `ITEMMENU` - When you've selected ITEM.
- `MERCYMENU` - When you've selected MERCY.
- `ENEMYDIALOGUE` - When your enemy or enemies start their dialogue.
- `DIALOGRESULT` - When you call `BattleDialog()`, or when the UI shows text on its own,
  for example when using an item.

Note: There are three states not mentioned here, because they never occur naturally. You
can find them in [Misc. functions](objects/misc-functions.md).

For a clearer example, here's a code snippet replicating the older events above.

```lua
function EnteringState(newstate, oldstate)
    if newstate == "ENEMYDIALOGUE" then
        --same as EnemyDialogueStarting()
    elseif newstate!= "ENEMYDIALOGUE" and oldstate == "ENEMYDIALOGUE" then
        --same as EnemyDialogueEnding(). Alternatively, check for newstate == "DEFENDING"
    elseif newstate!= "DEFENDING" and oldstate == "DEFENDING" then
        --same as DefenseEnding()
    end
end
```

### `Update()`

This function runs for every frame (usually at 60FPS, depends on the player's framerate)
for all of the encounter, even during waves. This is an extremely powerful function, as it
can run any code at any time, no matter what. The only exception is the game over state,
if the player dies, no code from within this function will be run.

### `BeforeDeath()`

This function runs the moment the Player takes mortal damage (by any means, including
bullet damage, scripted damage, setting `Player.hp` to `0`, and even text commands), just
before activating the Game Over sequence. This is the perfect place to set Real and
AlMighty Globals you want set when the player dies (see
[Misc. functions](objects/misc-functions.md)).

If you use `Player.hp` or `Player.Heal` here to bring the Player's hp back to greater than
0, they will live and the Game Over sequence will be cancelled.

### New in v0.6.6. `OnTextDisplay(text text)`

Every time any text object's letters are created, this function gets called. This function
is the best place to manipulate the text object's letters using `Text.GetLetters`.

The argument `text` is the text object itself.

This event isn't called for text objects which have their own `Text.OnTextDisplay()`
function.

## Monster script events

### `HandleAttack(number damage)`

Happens the moment the player's attack has applied damage, this is when you hear the
hitting sound after the slash animation.

`damage` will be -1 if the player pressed Fight, but didn't press any buttons and let it
end by itself. The monster's `hp` variable will have updated at this time, too.

Don't call `BattleDialog()` here, it's a bit buggy right now.

### `OnDeath()`

Happens after your attack's shaking animation has completed and the monster's HP is 0. If
you implement `OnDeath()`, your monster will not die automatically, and you will have to
do it manually with the `Kill()` function.

`OnDeath()` will only happen through monster kills that happened with the FIGHT command;
scripted `Kill()` calls will not trigger it.

Calling `BattleDialog()` here will probably screw up the battle UI.

### `OnSpare()`

Happens after you successfully spared a monster. If you implement `OnSpare()`, your
monster will not be spared automatically, and you will have to do it manually with the
`Spare()` function.

`OnSpare()` will only happen through a monster spare that happened with the SPARE command;
scripted `Spare()` calls will not trigger it.

### `BeforeDamageCalculation()`

Happens before the damage calculation the moment you press Z when attacking. You can
easily use `SetDamage()` in this function. This is also the best place to initiate a dodge
animation, if you want such a thing.

### `BeforeDamageValues(number damage)`

Happens before the damage UI is displayed on the monster (the life bar and the damage
number) and before the hp changing. You can still change the target with
`Player.ChangeTarget(targetNumber)` in this function, but you *can not* use `SetDamage`
here.

The argument `damage` is equal to the incoming damage the enemy is about to take. Note
that this damage has not been applied yet, unlike in `HandleAttack`.

### `HandleCustomCommand(string command)`

Happens when you select an Act command on this monster. `command` will be the same as how
you defined it in the `commands` list, except it will be **IN ALL CAPS**. Intermediate
example below, showing how you can use it and spice it up a little.

```lua
commands = {"Sing", "Dance", "Wiggle"} --somewhere at the beginning
wigglecounter = 0 --let's keep a counter to check how often we've wiggled

function HandleCustomCommand(command)
    if command == "SING" then
        BattleDialog({"You sing your heart out. It's in the arena now."})
    elseif command == "DANCE" then
        BattleDialog({"You busted out your best moves."})
    elseif command == "WIGGLE" then
        if wigglecounter == 0 then --you can use variables to make commands more exciting!
            BattleDialog({"You just kind of stood there and wiggled."})
        elseif wigglecounter == 1 then
            BattleDialog({"You're still kind of standing there and wiggling."})
        else
            BattleDialog({"Your wiggled so often that your wiggling technique\ris now legendary."})
        end
        wigglecounter = wigglecounter + 1 --be sure to increase the wiggle counter, or it'll stay at 0
    end
end
```

## Wave script events

### `Update()`

This function is called every frame (usually at 60FPS) while monsters are attacking, the
defense step.

That's pretty much it. Update your bullets here, more on bullet creation and control is on
the [Projectile management](projectiles.md) page.

### `EndingWave()`

This function is called just before the wave ends. It allows you to easily reset some
variables and other such things.

### `OnHit(bullet bullet)`

Every time a bullet collides with a player, this function gets called from the script that
created the projectile. The bullet object in this function can be modified if you feel
like it. For more information on the bullet object, see the section
[Projectile management](projectiles.md).

If you implement this function in your script, you have to manually define what should
happen after bullet collision. This is what allows you to create orange, cyan and green
projectiles, and much much more. If you don't implement this function in your script,
it'll stick to the default of dealing 3 damage on hit.

## All-script events

*All-script events* are events that can exist in all types of scripts, that's encounter,
monster, and wave scripts.

### `OnHit(bullet bullet)`

Every time a bullet collides with a player, this function gets called from the script that
created the projectile. The bullet object in this function can be modified if you feel
like it. For more information on the bullet object, see the section
[Projectile management](projectiles.md).

If you implement this function in your script, you have to manually define what should
happen after bullet collision. This is what allows you to create orange, cyan and green
projectiles, and much much more. If you don't implement this function in your script,
it'll stick to the default of dealing 3 damage on hit.
