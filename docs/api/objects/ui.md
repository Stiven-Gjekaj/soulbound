# `<CYF>` The UI object

### `<CYF>` The UI object [E/M/W]

This section details the various elements you can use to manipulate CYF's internal user
interface.

If you want to manipulate UI elements related to the enemy (attack animation, health bar,
position of the damage text), see the [Misc. functions](misc-functions.md) section of the
documentation.

Keep in mind that removing any of these elements has a great chance of making the engine
unstable, so take caution when manipulating the UI.

### **sprite** `UI.background`

Sprite object allowing you to manipulate the background image set using the `background`
variable.

### **text** `UI.namelv`

Text object displaying the Player's name as well as its level. Its contents are usually the
Player's name followed by two spaces, the text `LV`, another space, then finally the
Player's level.

### **sprite** `UI.hplabel`

Sprite for the `HP` text next to the Player's life bar.

### **bar** `UI.hpbar`

Bar object used as the Player's life bar.

### **text** `UI.hptext`

Text object displaying the Player's HP. Its contents display the Player's current HP, a
space, a slash, another space, and finally the Player's max HP.

### New in v0.6.6. **text** `UI.maintext`

Text object used in the Arena to display most things. Take caution when handling this
object, as it's used absolutely everywhere in encounters.

### New in v0.6.6. **sprite** `UI.mugshot`

Sprite used to display a character's face sprite on the left side of the arena, through the
usage of the `[mugshot:x]` text command.

### New in v0.6.6. **sprite** `UI.mugshotmask`

Mask sprite used to keep the mugshot within the bounds of the arena. Its size is usually
130x130, although its height scales with the arena's, meaning changing its height may not
work as intended. Changing its anchor will stop that effect from happening.

### New in v0.6.6. **{table of bar}** `UI.enemylifebarlist`

List of life bars displayed next to each enemy whenever the Player selects the FIGHT
button. All life bars are destroyed whenever the Player changes the current page if there
are more than 3 active enemies, so you need to get this variable again to fetch the new
life bars if the page is changed.

### **sprite** `UI.fightbtn`

Sprite for the `FIGHT` button. Do not use its `Remove` function, or it may cause unexpected
errors. If you want to remove it, set its alpha to 0 instead.

### **sprite** `UI.actbtn`

Sprite for the `ACT` button. Do not use its `Remove` function, or it may cause unexpected
errors. If you want to remove it, set its alpha to 0 instead.

### **sprite** `UI.itembtn`

Sprite for the `ITEM` button. Do not use its `Remove` function, or it may cause unexpected
errors. If you want to remove it, set its alpha to 0 instead.

### **sprite** `UI.mercybtn`

Sprite for the `MERCY` button. Do not remove it in any way, or it may cause unexpected
errors. If you want to remove it, set its alpha to 0 instead.

### New in v0.6.6. **object** `UI.root`

Root of the entire battle scene. See [General objects](general-objects.md) for more
information on how to handle this object.

### `UI.StopUpdate(boolean toggle)`

If `toggle` is true, the Player's UI (except the buttons) will not be updated by the
engine, whether it is the Player's name and level text or the Player's hp bar and text.

False by default.

### `UI.Hide(boolean hide)`

If `hide` is true, all of the Player's UI, including the buttons, will be hidden.

False by default.

### `UI.RepositionHPElements()`

Updates the Player's life bar's position, and the player's hp text's position depending on
the sizes of the hp bar and the hp label.

### `UI.Reset()`

Tries to reset all modifications to the Player's UI, making it look like it originally is
at the beginning of the battle by resetting most of the UI elements' parameters.

### `UI.GetCurrentButton()` returns **string**

Returns the name of the currently selected button. Its return value can either be `FIGHT`,
`ACT`, `ITEM` or `MERCY`.

### `UI.DisableButton(string button)`

Disables the given button, preventing its selection in the state `ACTIONSELECT`.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.EnableButton(string button)`

Reenables the given button if it was previously disabled using `UI.DisableButton`.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.ResetButtonPosition(string button, boolean resetX = true, boolean resetY = true)`

Resets the position of the given button. Setting `resetX` to `true` resets the button's
horizontal position, while setting `resetY` to `true` resets the button's vertical
position.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.GetPlayerXPosOnButton(string button)` returns **number**

Returns the player's horizontal position offset when selecting the given button, starting
from the button's bottom left corner. The default value for all buttons is `16`.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.GetPlayerYPosOnButton(string button)` returns **number**

Returns the player's vertical position offset when selecting the given button, starting
from the button's bottom left corner. The default value for all buttons is `19`.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.SetPlayerXPosOnButton(string button, number newX)`

Sets the player's horizontal position offset when selecting the given button, starting from
the button's bottom left corner.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.SetPlayerYPosOnButton(string button, number newY)`

Sets the player's vertical position offset when selecting the given button, starting from
the button's bottom left corner.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.ResetPlayerPosOnButton(string button, boolean resetX = true, boolean resetY = true)`

Resets the player's position offset when selecting the given button, starting from the
button's bottom left corner. Setting `resetX` to `true` resets the player's horizontal
position offset, while setting `resetY` to `true` resets the player's vertical position
offset.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.SetButtonActiveSprite(string button, string sprite)`

Sets the sprite of the given button whenever it is selected.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.ResetButtonActiveSprite(string button)`

Resets the sprite of the given button whenever it is selected to the one that was used by
the engine at the beginning of the encounter.

Accepted values for the `button` argument are `FIGHT`, `ACT`, `ITEM` or `MERCY`.

### `UI.UpdateButtons()`

Sets the active button's active sprite and moves the Player where it needs to be on its
currently selected button. This function is automatically called after most functions
related to button manipulation.

## `<CYF>` The Bar object [E/M/W]

This separate section lists the various functions and variables usable when dealing with a
bar object, which can be used for life bars, for example.

These objects are used in CYF as life bars, whether it's the Player's, the life bars
appearing next to the enemy's names after selecting the `FIGHT` button, or the life bar
appearing after attacking an enemy.

Bar objects are composed of three sprites, four of them if it has an outline, which are all
children of one another: `fill` is the child of `mask` which is itself the child of
`background`. If the bar's `outline` exists, then `background` will be its child.

### `CreateBar(number x, number y, number width, number height = 20)` returns **bar** [E/M/W]

Creates a bar object whose bottom left corner is at the absolute position `x` and `y`, with
a size of `width` pixels horizontally and `height` pixels vertically.

### `CreateBarWithSprites(number x, number y, string backgroundSprite, string fillSprite = backgroundSprite)` returns **bar** [E/M/W]

Creates a bar object whose bottom left corner is at the absolute position `x` and `y`, and
which uses the sprites `backgroundSprite` and `fillSprite` to create the life bar.

Both sprites must have the same size, and if no value is given for `fillSprite`, then it
uses the same sprite as `backgroundSprite`.

### **sprite** `Bar.background`

Background sprite of the bar object, which is also the parent of all other standard bar
elements.

### **sprite** `Bar.mask`

Mask sprite of the bar object, used to hide part of the `fill` sprite. A mask is used in
order to cut the `fill` sprite if it's a sprite other than a single pixel scaled up,
allowing for the bar's `fill` to be cut instead of squished.

### **sprite** `Bar.fill`

Fill sprite of the bar object, which is used to represent the amount of the bar which is
still full.

### **sprite** `Bar.outline`

Outline sprite of the bar object, which adds a black outline to the bar object. It needs to
be created first using the function `AddOutline()`.

If it exists, it will be the parent of the `background` sprite.

### **number** `Bar.currentFill` (readonly)

Value usually between 0 and 1 that gives the percentage of the bar which is currently full.
`0` means the bar is empty, while `1` means it is full.

Can be outside of these bounds if `SetInstant()` or `SetLerp()` are used without clamping.

### **boolean** `Bar.hasOutline` (readonly)

Returns true if the bar object has an `outline` sprite, false otherwise.

### **number** `Bar.outlineThickness`

Returns the thickness in pixels of the `outline` sprite. Returns 0 if the outline doesn't
exist.

Can be set to resize the outline's thickness.

### New in v0.6.6. **number** `Bar.isactive`

True if the lifebar hasn't been removed, false otherwise.

### `Bar.SetInstant(number fillValue, boolean allowNonClamped = false)`

Sets the fill percentage of the bar object between 0 and 1. `0` means the bar is empty,
while `1` means it is full.

If `allowNonClamped` is set to true, you can use values lower than 0 and higher than 1.

Note that the bar will be updated properly when `fillValue` is beyond its usual bounds only
if the `bar-px` sprite is used for all of the bar's sprites.

### New in v0.6.6. `Bar.SetLerp(number fillValue, number time = 60, boolean allowNonClamped = false)`

Gradually sets the fill percentage of the bar object between 0 and 1 from the bar's current
fill to `fillValue` over `time` frames, with 60 frames being equal to one second. `0` means
the bar is empty, while `1` means it is full.

If `allowNonClamped` is set to true, you can use values lower than 0 and higher than 1.

Note that the bar will be updated properly when `fillValue` is beyond its usual bounds only
if the `bar-px` sprite is used for all of the bar's sprites.

### New in v0.6.6. `Bar.SetLerpFull(number originalValue, number fillValue, number time = 60, boolean allowNonClamped = false)`

Gradually sets the fill percentage of the bar object between 0 and 1 from `originalValue`
to `fillValue` over `time` frames, with 60 frames being equal to one second. `0` means the
bar is empty, while `1` means it is full.

If `allowNonClamped` is set to true, you can use values lower than 0 and higher than 1.

Note that the bar will be updated properly when `fillValue` is beyond its usual bounds only
if the `bar-px` sprite is used for all of the bar's sprites.

### `Bar.AddOutline(number thickness, number r = 0, number g = 0, number b = 0)`

Creates a rectangle outline sprite which has a thickness of `thickness` pixels on all
sides. This new sprite will be the parent of the `background` sprite, which means it needs
to be moved if you want to move the entire bar object if it exists.

The standard color of the outline is black, but you can change the color of the `outline`
sprite by setting the values `r`, `g` and `b` between 0 and 1.

### `Bar.RemoveOutline()`

Removes the `outline` sprite if it exists without removing the bar object.

### `Bar.Resize(number width, number height, boolean updateOutline = true)`

Sets the scale of the various elements of the bar object. If the `outline` sprite shouldn't
be updated, set `updateOutline` to `false`.

If used on a bar object using the `bar-px` sprite for both its `background` and its `fill`
sprite, then the bar object will be `width` pixels wide and `height` pixels tall.

### `Bar.SetSprites(string bgSprite, string fSprite = bgSprite, string mSprite = nil, string oSprite = nil)`

Sets the sprites of the various elements of the bar object. `bgSprite` will replace the
image of the `background` sprite, `fSprite` will replace the image of the `fill` sprite,
`mSprite` will replace the image of the `mask` sprite and `oSprite` will replace the image
of the `outline` sprite, if it exists.

The size of the `background` and `fill` sprites must always be the same, hence why if
`fSprite` is not given, it will use the same sprite as `bgSprite`.

### `Bar.SetVisible(boolean visible)`

Hides the bar object completely if `visible` is set to `false`, shows it otherwise.

### `Bar.Destroy()`

Destroys this bar object, removing all of its sprites. The Player's lifebar cannot be
destroyed.
