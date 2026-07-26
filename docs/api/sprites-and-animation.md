# Sprites and animation

### `<CYF>` `CreateLayer(string name, string position = "BelowArena", boolean below = false)` returns **boolean** [E/M/W]

Creates a layer named `name` that sprites can be placed in. To create your new sprite
layer, you'll need to choose a pre-existing layer. Returns `true` if the layer was
successfully created, `false` otherwise.

`position` can be one of three values:

- `(Name of existing sprite layer)`: Your new layer will be created above or below the
  specified layer. See below for default layers and how to use `below`.
- `"VeryHighest"`: Your new sprite layer will be created as high as possible, above
  everything else. **This will make your layer appear above the debugger.**
- `"VeryLowest"`: Your new sprite layer will be created as low as possible, below
  everything else.

If `below` is true, the new layer will be created below the layer given in `position`.
Otherwise, it will be above it.

Default layers:

- `"Bottom"`: Under everything, even the background.
- `"BelowUI"`: Above the background.
- `"BelowArena"` or `"BasisNewest"`: Above the background and the UI.
- `"BelowPlayer"`: Above the background, the UI and the Arena.
- `"BelowBullet"`: Above the background, the UI, the Arena and the Player.
- `"Top"`: Above everything.

### `CreateSprite(string spritename,` `<CYF>` `string layer = "BelowArena", number childNumber = -1)` returns **sprite** [E/M/W]

Creates a sprite at the center of the screen (at 320, 240) that you can modify in many
ways.

`<CYF>` You can add a layer if you want, otherwise the sprite will be below the arena.
Enter `"none"` to spawn your sprite outside of any layers, in the same way that the player
is by default, although this doesn't have much use.

`<CYF>` In CYF, you can provide a number argument for `childNumber` if you want. Leave it as
`-1` to have it move to the top of whatever layer it's placed on (default behavior), or
choose a numbered index you want it to appear in, with `1` being the very bottom-most in
the layer, and higher numbers moving it above other elements in sequence.

The best way to see what order sprites come in in their respective layers is to download
Unity, playtest your mod in it, and look at the order of game objects in the Inspector. See
[Unity setup](../basics/unity-setup.md) for instructions on this. Note that this is optional
and only if you would like a better understanding of how layers work.

### `<CYF>` `CreateSprite(string spritename, number childNumber = -1)` returns **sprite** [E/M/W]

The same as `CreateSprite` listed above, except you only need to provide a sprite name and
child number. Uses the default layer `"BelowArena"`.

## The Sprite object

The Sprite object has many controls intended for animation.

New in v0.6.6: As this object exists in CYF's hierarchy, it's possible to manipulate its
parent and children. See the [General objects](objects/general-objects.md) page for more
information.

### `<CYF>` **string** `sprite.spritename` (readonly)

Returns the path of the image used by this sprite, starting from the `Sprites/` folder.

### **number** `sprite.x`

Horizontal position of sprite relative to the bottom left corner of the screen, measured
from its pivot or anchor point (center by default).

If parented, x position is relative to the parent.

### **number** `sprite.y`

Vertical position of sprite relative to the bottom left corner of the screen, measured from
its pivot or anchor point (center by default).

If parented, y position is relative to the parent.

### `<CYF>` **number** `sprite.z`

Z position of the sprite. A negative number brings it forward, closer to the camera, a
positive number sends it backwards.

Battle rendering orders sprites by layer rather than by Z, so this rarely changes what you
see. It can NOT be used to circumvent the layering system for regular sprites.

If parented, z position is relative to the parent.

### **number** `sprite.absx`

Horizontal position of sprite relative to the bottom left corner of the screen, ignoring
its pivot or anchor point and all parents.

### **number** `sprite.absy`

Vertical position of sprite relative to the bottom left corner of the screen, ignoring its
pivot or anchor point and all parents.

### `<CYF>` **number** `sprite.absz`

Z position of the sprite. A negative number brings it forward, closer to the camera, a
positive number sends it backwards.

Same as `sprite.z`, except that if this sprite has a parent, `sprite.absz` will not be
relative to the z value of the parent.

### **number** `sprite.xscale`

Horizontal scaling of sprite (`1.0` by default). `2.0` is twice as large, `0.5` is half as
large.

Scaling applies based on the sprite's pivot point, see `SetPivot`, `<CYF>` `xpivot` and
`ypivot`.

### **number** `sprite.yscale`

Vertical scaling of sprite (`1.0` by default). `2.0` is twice as large, `0.5` is half as
large.

Scaling applies based on the sprite's pivot point, see `SetPivot`, `<CYF>` `xpivot` and
`ypivot`.

### **boolean** `sprite.isactive` (readonly)

`<0.2.1a>` Returns `true` if the sprite has been removed and `false` otherwise.

`<CYF>` **In retromode, behaves the same as 0.2.1a**. If not in retromode, this is `false`
if the sprite has been removed and `true` otherwise.

### **number** `sprite.width` (readonly)

Gives the width of the sprite object's active image in pixels.

This never changes until the sprite itself is swapped.

Note: Does not take `sprite.xscale` into account.

### **number** `sprite.height` (readonly)

Gives the height of the sprite object's active image in pixels.

This never changes until the sprite itself is swapped.

Note: Does not take `sprite.yscale` into account.

### `<CYF>` **number** `sprite.xpivot`

Horizontal pivot point of the sprite. `0` is the left side, and `1` is the right side. Can
be any number inside or outside of this range.

`0.5` by default.

See `sprite.SetPivot`.

### `<CYF>` **number** `sprite.ypivot`

Vertical pivot point of the sprite. `0` is the bottom side, and `1` is the top side. Can be
any number inside or outside of this range.

`0.5` by default.

See `sprite.SetPivot`.

### `<CYF>` **boolean** `sprite.animcomplete` (readonly)

If a sprite has started an animation, this tells you if the animation is complete.

If a sprite does not have an active animation, if the animation's loop mode is `LOOP`, or
if the animation isn't finished yet, this will be `false`. Otherwise, it will be `true`.

### `<CYF>` **number** `sprite.currentframe`

If a sprite has an active animation running, this represents the index of the active frame
in the animation. Otherwise, this will return `0`.

For example: If this code is run:

```lua
sprite.SetAnimation({"sans_head1", "sans_head2", "sans_head3", "sans_head2"})
```

Then this variable will be `1` when the first image is shown, `2` for the second, and so
on, up to `4` for the last one.

Likewise, setting `sprite.currentframe = 3` with the above example will jump the animation
to the first frame where `"sans_head3"` would be the currently shown image.

### `<CYF>` **number** `sprite.currenttime`

If a sprite has an active animation running, this represents its current play time, in
seconds. Can both be read and set.

If an animation is not running, this will be `0`.

Similar in function to `NewAudio.GetPlayTime` / `NewAudio.GetCurrentTime`.

### `<CYF>` **number** `sprite.totaltime` (read-only)

If a sprite has an active animation running, this represents the total amount of time its
animation will last for. Note that this is based on the speed the animation runs at, and
will always be equal to (`sprite.animationspeed` * the number of frames in a sprite's
animation).

If an animation is not running, this will be `0`.

Similar in function to `NewAudio.GetTotalTime`.

### `<CYF>` **number** `sprite.animationspeed`

If a sprite has an active animation running, this represents the amount of seconds each
frame will be displayed for.

Can both be read and set. Must be greater than 0.

### `<CYF>` **boolean** `sprite.animationpaused`

If a sprite has an active animation running, you can set this to `true` to pause it, or
`false` to resume it.

### `<CYF>` **string** `sprite.loopmode`

Gets the loop mode of a sprite's current animation, or sets the loop mode of a sprite's
*next* animation.

Can be:

- `LOOP` - The default mode: when the animation is finished, it plays again. `animcomplete`
  is always false with this mode.
- `ONESHOT` - Plays the animation once. The sprite object will remain on the last frame of
  the animation.
- `ONESHOTEMPTY` - Same as `ONESHOT`, except that when the animation is finished, the
  sprite object will use an empty sprite.

### **{table of number, number, number,** `<CYF>` **number = 1}** `sprite.color`

Gets or sets the coloration of a sprite, as a table of 3 or 4 values from 0 to 1.

For example, `sprite.color = {1.0, 0.0, 0.0}` colors the sprite red. This actually overlays
the sprite's original color, so if you want full control over the color, make sure your
sprite is white. *Black areas are not affected by coloration.*

`<CYF>`: You can provide a 4th value, which sets the alpha (transparency) of the sprite.

### `<CYF>` **{table of number, number, number, number = 255}** `sprite.color32`

Gets or sets the coloration of a sprite, as a table of 3 or 4 values from 0 to 255.

For example, `sprite.color32 = {255, 0, 0}` colors the sprite red. This actually overlays
the sprite's original color, so if you want full control over the color, make sure your
sprite is white. *Black areas are not affected by coloration.*

You can provide a 4th value, which sets the alpha (transparency) of the sprite.

### **number** `sprite.alpha`

Gets or sets a sprite's transparency, as a value from 0 to 1.

### `<CYF>` **number** `sprite.alpha32`

Gets or sets a sprite's transparency, as a value from 0 to 255.

### **number** `sprite.rotation`

Gets or sets a sprite's rotation, in degrees.

It's clamped between 0 and 360, so if you set it to 365, it will become 5.

### New in v0.6.6. `<CYF>` **number** `sprite.localRotation`

Gets or sets a sprite's local rotation, as in its rotation compared to its parent, in
degrees.

It's clamped between 0 and 360, so if you set it to 365, it will become 5.

### `<CYF>` **string** `sprite.layer`

Gets or sets the current layer a sprite is on. Does nothing if you set it to a layer that
doesn't exist.

Default value: `"BelowArena"`.

Text Object letter sprites can not have their layers set.

Note: It is common practice to use `sprite.layer` to deparent a sprite if you need to do
so. Setting it again will parent the sprite to the given layer, removing its previous
parenting altogether.

### New in v0.6.6. `<CYF>` **number** `sprite.characterNumber`

Only used for letters fetched through `Text.GetLetters()`. Gets the index of the character
this sprite displays. If the current letter shows the first character of the text object,
then its number will be `0`.

Non-displayed letters and text commands are taken into account, meaning if text commands
are before the first shown letter of the text, its index won't be 0.

### `sprite.Set(string newSprite)`

Change a sprite's current image. It retains its scaling and rotation.

If you have an animation running with `SetAnimation`, the animation will override your
sprite change.

Note: Using this function with a sprite which has a parent with a different rotation will
reset the sprite's rotation to its usual value. Setting the variable `noscalerotationbug`
to true in the Encounter script prevents that effect.

### `<CYF>` `sprite.Mask(string mode)`

Sets the masking mode of this sprite object. Does not function for Text Object letter
sprites.

Available modes are:

- `off` - The default mode. Has no special properties.
- `box` - Any objects parented to this sprite are restricted to the bounding box of the
  sprite. Recommended for masking bullets to the Arena and such things. Unfortunately
  doesn't seem to work well with rotated sprites, but this is still the recommended mode
  for performance.
- `sprite` - Any objects parented to this sprite will be "cookie cuttered" to the current
  image of the sprite.
- `stencil` - Same as `sprite`, except that the parent sprite itself is not visible.
- `invertedsprite` - The reverse of `sprite`; while the sprite itself is visible, any of
  its children will only display when not inside the bounds of the sprite.
- `invertedstencil` - Identical to `invertedsprite` except that the parent sprite is not
  shown.

Anything parented to this sprite object through the use of `SetParent`, including **Other
Sprite Objects**, **Projectiles**, and even **Text Objects**, will all be masked according
to the mask mode entered here.

If using `invertedsprite` or `invertedstencil`, you may be unable to change the mask mode
back to a normal mode afterwards, other than "off". This issue is unavoidable, as these
inverted mask modes work by directly altering Unity's image display code.

**Note:** If you want to have a child that's not masked, you'll need to create a new
invisible sprite (using the default sprite "empty" is recommended), parent both the
intended parent and child sprite to the invisible sprite, and apply motion to the invisible
sprite rather than the parent sprite.

### `<CYF>` **shader** `sprite.shader`

The shader object linked to this sprite object.

Be aware that due to the nature of shaders, it is possible for some shaders to break
certain Create Your Frisk properties, such as sprite layering and sprite masking. It is
recommended that shaders used be based on the template shader provided in
[Coding a shader](../shaders/coding-a-shader.md).

See [The Shader object](../shaders/shader-object.md) for more information.

### `sprite.SetPivot(number x, number y)`

Changes the point a sprite rotates and scales around.

`(0, 0)` is the bottom-left corner of the sprite, `(1, 1)` is the top-right corner. You can
also have values outside the 0-1 range.

**Note:** The origin point of the sprite will change after the usage of this function,
meaning moving the sprite to a given relative position (through `sprite.Move()`,
`sprite.x` or `sprite.y`) will move the sprite to a different position, centering the
sprite's pivot on the new position.

### `sprite.SetAnchor(number x, number y)`

Changes the point a sprite anchors to when moving. Works much like `sprite.SetPivot()`,
except it takes the sprite's parent into account.

Most useful when rescaling a parent sprite and making this child sprite stick to a certain
edge of it. `x` and `y` should be between 0 and 1.

**Note:** The origin point of the sprite will change after the usage of this function,
meaning moving the sprite to a given relative position (through `sprite.Move()`, `sprite.x`
or `sprite.y`) will move the sprite to a different position, meaning the current pivot of
the sprite will now be moved relative to its parent. An anchor of `(0, 0)` means the
sprite's pivot point will be on the bottom left corner of its parent's own sprite.

If a sprite has no parent, then all layers span across the entire screen.

### `<CYF>` `sprite.Move(number x, number y)`

Moves the sprite `x` pixels right, and `y` pixels up.

See `sprite.x` and `sprite.y`.

### `sprite.MoveTo(number x, number y)`

Same as setting x and y simultaneously.

See `sprite.x` and `sprite.y`.

### `sprite.MoveToAbs(number x, number y)`

Same as setting absx and absy simultaneously. Moves a sprite to an absolute screen
position, regardless of its parent settings.

See `sprite.absx` and `sprite.absy`.

### `sprite.Scale(number xscale, number yscale)`

Same as setting xscale and yscale simultaneously.

See `sprite.xscale` and `sprite.yscale`.

Note: Using this function with a sprite which has a parent with a different rotation will
reset the sprite's rotation to its usual value. Setting the variable `noscalerotationbug`
to true in the Encounter script prevents that effect.

### `sprite.SetAnimation({table of string} spriteTable, number timePerFrame = 1/30,` `<CYF>` `string prefix = "")`

Performs frame-by-frame animation with your own time between frames, in seconds. It's the
same as changing the sprite object's image with `sprite.Set` on a set timer. If
`time_per_frame` is 1, it takes 1 second to move to the next sprite.

Example: `sprite.SetAnimation({"sans_head1", "sans_head2", "sans_head3"})`

`<CYF>` `prefix`: An optional string providing the path to a folder contained within your
Sprites folder. This path will be automatically added to the beginning of every sprite's
name.

For example, you can load your collection of 6 sprites in `Sprites/character` with
`sprite.SetAnimation({"spr1", "spr2", "spr3", "spr4", "spr5", "spr6"}, 1/30, "character")`.

`<CYF>` This can use sprites with different sizes.

Note: Using this function with a sprite which has a parent with a different rotation will
reset the sprite's rotation to its usual value. Setting the variable `noscalerotationbug`
to true in the Encounter script prevents that effect.

### `sprite.StopAnimation()`

Stops a frame-by-frame animation if it was running.

Does NOT reset your sprite's image back to its previous one. Instead, use `sprite.Set()`
just after this function.

### `sprite.SendToTop()`

Sends this sprite to the top of its layer's hierarchy. If a sprite has 5 children, for
instance, you can use this to rearrange them internally. However, child sprites will always
appear on top of their parents, regardless of this function being called.

### `sprite.SendToBottom()`

Sends this sprite to the bottom of its layer's hierarchy. Similar rules apply as with
`SendToTop()`.

### `<CYF>` `sprite.MoveBelow(sprite otherSpriteObject)`

If both sprites have the same parent, this will move the calling sprite just below the
other sprite.

### `<CYF>` `sprite.MoveAbove(sprite otherSpriteObject)`

If both sprites have the same parent, this will move the calling sprite just above the
other sprite.

### `sprite.Remove()`

Removes a sprite object. Calling anything other than `isactive` after this will give you an
error. Removing a sprite object will also remove all *children* of the sprite object.

`<CYF>` Note: Calling `bullet.sprite.Remove` will instead call `bullet.Remove`, unless you
are using retromode.

### `<CYF>` `sprite.SetVar(string yourVariableName, value)` or `sprite[string yourVariableName] = value`

Sets a variable in a sprite object that you can retrieve with `sprite.GetVar`. Identical to
`SetVar` in projectiles.

### `<CYF>` `sprite.GetVar(string yourVariableName)` or `sprite[string yourVariableName]`

Gets a variable in a sprite object that you previously set with `sprite.SetVar`. Identical
to `GetVar` in projectiles.

### `<CYF>` `sprite.Dust(boolean playSound = true, boolean removeObject = false)`

Turns a sprite into dust, just like what happens in Undertale when an enemy is killed.

Will remove a sprite if `removeObject` is set to true.

Set `playSound` to false to deactivate the dust sound.

Note: Calling `bullet.sprite.Dust(..., true)` will call `bullet.Remove`, unless you are
using retromode.

## Example

An animation script, shown for reference.

```lua
-- First, we can create the torso, legs and head.
sanstorso = CreateSprite("sans/sanstorso")
sanslegs = CreateSprite("sans/sanslegs")
sanshead = CreateSprite("sans/sanshead1")

--We parent the torso to the legs, so when you move the legs, the torso moves too.
--We do the same for attaching the head to the torso.
sanstorso.SetParent(sanslegs)
sanshead.SetParent(sanstorso)

--Now we adjust the height for the individual parts so they look more like a skeleton and less like a pile of bones.
sanslegs.y = 240
sanstorso.y = -5 --The torso's height is relative to the legs they're parented to.
sanshead.y = 40 --The head's height is relative to the torso it's parented to.

--We set the torso's pivot point to halfway horizontally, and on the bottom vertically,
--so we can rotate it around the bottom instead of the center.
sanstorso.SetPivot(0.5, 0)

--We set the torso's anchor point to the top center. Because the legs are pivoted on the bottom (so rescaling them only makes them move up),
--we want the torso to move along upwards with them.
sanstorso.SetAnchor(0.5, 1)
sanslegs.SetPivot(0.5, 0)

--Finally, we do some frame-by-frame animation just to show off the feature. You put in a list of sprites,
--and the time you want a sprite change to take. In this case, it's 1/2 of a second.
sanshead.SetAnimation({"sans/sanshead1", "sans/sanshead2", "sans/sanshead3"}, 1/2)

function AnimateSans()
    sanslegs.Scale(1, 1+0.1*math.sin(Time.time*2))
    sanshead.MoveTo(2*math.sin(Time.time), 40 + 2*math.cos(Time.time))
    sanshead.rotation = 10*math.sin(Time.time + 1)
    sanstorso.rotation = 10*math.sin(Time.time + 2)
end
```
