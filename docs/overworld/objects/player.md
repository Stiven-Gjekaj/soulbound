# `<CYF>` The Player Overworld object

Note: This Player object is not the same as the one used in battle. This one is specific to
the overworld.

### `Player.GetLevel()` returns **number**

Returns the player's level (LV).

### `Player.SetLevel(number value)`

Sets the player's level (LV).

### `Player.GetHP()` returns **number**

Returns the player's HP.

### `Player.SetHP(number value)`

Sets the player's HP.

### `Player.GetMaxHP()` returns **number**

Returns the player's max HP.

### `Player.SetMaxHP(number value)`

Sets the player's max HP.

### `Player.ResetMaxHP()`

Resets the player's max HP to the value it would have based on their level (LV).

### `Player.GetName()` returns **string**

Returns the player's name.

### `Player.SetName(string value)`

Sets the player's name.

### `Player.GetWeaponATK()` returns **number**

Returns the player's weapon's ATK.

### `Player.GetArmorDEF()` returns **number**

Returns the player's armor's DEF.

### `Player.GetATK()` returns **number**

Returns the player's total ATK.

### `Player.GetDEF()` returns **number**

Returns the player's total DEF.

### `Player.GetGold()` returns **number**

Returns the player's amount of gold.

### `Player.SetGold(number value)`

Sets the player's amount of gold.

### `Player.GetWeapon()` returns **string**

Returns the name of the player's weapon.

### `Player.SetWeapon(string value)`

Makes the player equip the weapon given in `value`, but only if a weapon with that name is in
their inventory.

### `Player.GetArmor()` returns **string**

Returns the player's armor's name.

### `Player.SetArmor(string value)`

Makes the player equip the armor given in `value`, but only if any armor with that name is in
their inventory.

### `Player.GetEXP()` returns **number**

Returns the player's EXP (XP).

### `Player.SetEXP(number value)`

Sets the player's EXP (XP) to `value`.

### `Player.ForceHP(number value)`

Sets the player's hp. You can set it up to 1.5 times the player's max HP.

### `Player.Hurt(number value)`

Removes `value` HP from the player's current HP. The Player can't be killed by use of this
function.

### `Player.Heal(number value)`

Adds `value` HP to the player's current HP, but can not exceed the player's maximum HP.

### `Player.CanMove(boolean canMove)`

This function determines whether the player can move or not.

However, if `canMove` is set to *true* and the function is called while a main event is
running, then you can't trigger any other main events, teleport to other maps or open the
menu until the main event is stopped.

### `Player.Teleport(string mapName, number posX, number posY, number direction = 0, boolean noFadeIn = false, boolean noFadeOut = false)`

Teleports the player to the map named `mapName`.

You can set the player's direction using `direction`.

`noFadeIn` and `noFadeOut` determine whether there will be fades before and after the
teleport, respectively.
