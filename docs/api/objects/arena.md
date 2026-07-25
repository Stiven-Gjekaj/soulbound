# The Arena object

The following section is dedicated exclusively to the arena. This is the area inside the
white box that the player is allowed to move within. Projectiles can spawn and move
relative to the arena, and the arena can be resized `<CYF>` and moved.

### The Arena object

You can use this object to obtain information about the arena, resize it or move it.

Don't forget that the Arena's position is reset at the end of the wave.

- **number** `Arena.width` (readonly) - the width of the arena in pixels, after resizing.
  Since the reference for the player and bullets is the arena's center, you can get the
  left and right sides of the arena with `-Arena.width/2` and `Arena.width/2` respectively.
- **number** `Arena.height` (readonly) - the height of the arena in pixels, after resizing.
  Like with width, you can get the bottom and top with `-Arena.height/2` and
  `Arena.height/2` respectively.
- `<CYF>` **number** `Arena.x` (readonly) - the x position of the center of the Arena,
  after resizing.
- `<CYF>` **number** `Arena.y` (readonly) - the position of the bottom of the Arena, after
  resizing. To get that of the center, just do `Arena.y + Arena.height/2`.

  Note: `Arena.y` is the position of the bottom of the *outside* (white part) of the Arena.
  It will be 5 pixels (the arena's width) less than `Arena.currenty`.
- **number** `Arena.currentwidth` (readonly) - the *current* width of the arena in pixels.
  Differs from `width` in that it will accurately reflect the arena size in the middle of
  resizing, too.
- **number** `Arena.currentheight` (readonly) - the *current* height of the arena in
  pixels. Differs from `height` in that it will accurately reflect the arena size in the
  middle of resizing, too.
- `<CYF>` **number** `Arena.currentx` (readonly) - the *current* x of the arena in pixels.
  Differs from `x` in that it will accurately reflect the arena position in the middle of
  moving, too.
- `<CYF>` **number** `Arena.currenty` (readonly) - the *current* y of the arena in pixels.
  Differs from `y` in that it will accurately reflect the arena position in the middle of
  moving, too.

  Note: `Arena.currenty` is the position of the bottom of the *inside* (black part) of the
  Arena. It will be 5 pixels (the arena's width) greater than `Arena.y`.
- `<CYF>` **{table of number, number, number, number = 1}** `Arena.innerColor` - Set RGBA
  color (0-1) of Arena's inner sprite.
- `<CYF>` **{table of number, number, number, number = 255}** `Arena.innerColor32` - Set
  RGBA color (0-255) of Arena's inner sprite.
- `<CYF>` **{table of number, number, number, number = 1}** `Arena.outerColor` - Set RGBA
  color (0-1) of Arena's outer sprite.
- `<CYF>` **{table of number, number, number, number = 255}** `Arena.outerColor32` - Set
  RGBA color (0-255) of Arena's outer sprite.
- `<CYF>` **boolean** `Arena.isResizing` (readonly) - Tells you if the Arena is currently
  being resized.

  `<0.2.1a>`: There's an equivalent of this in 0.2.1a, just use
  `Arena.currentwidth ~= Arena.width or Arena.currentheight ~= Arena.height`.
- `<CYF>` **boolean** `Arena.isMoving` (readonly) - Tells you if the Arena is currently
  being moved.
- `<CYF>` **boolean** `Arena.isModifying` (readonly) - Returns true if the Arena is being
  moved *or* resized, false otherwise.
- `Arena.Resize(number width, number height)` - Resizes the arena to the new size.
  Currently, monsters stay on top of the arena. This was going to be changed around the
  animation update. `<CYF>` But, in CYF, you can use `BindToArena` to control that.
- `Arena.ResizeImmediate(number width, number height)` - Resizes the arena instantly,
  without the animation.
- `<CYF>` `Arena.Move(number x, number y, boolean movePlayer = true, boolean immediate = false)` -
  Moves the Arena based on its current position. Set `movePlayer` to true if you want the
  Player to move with the Arena and set `immediate` to true if you want to move the Arena
  immediately.
- `<CYF>` `Arena.MoveTo(number x, number y, boolean movePlayer = true, boolean immediate = false)` -
  Moves the Arena based on the bottom-left corner of the window. Set `movePlayer` to true
  if you want the Player to move with the Arena and set `immediate` to true if you want to
  move the Arena immediately.
- `<CYF>` `Arena.MoveAndResize(number x, number y, number width, number height, boolean movePlayer = true, boolean immediate = false)` -
  Moves the Arena based on its current position and resizes it at the same time. Set
  `movePlayer` to true if you want the Player to move with the Arena and set `immediate`
  to true if you want to move the Arena immediately.
- `<CYF>` `Arena.MoveToAndResize(number x, number y, number width, number height, boolean movePlayer = true, boolean immediate = false)` -
  Move the Arena based on the bottom-left corner of the window and resizes it at the same
  time. Set `movePlayer` to true if you want the Player to move with the Arena and set
  `immediate` to true if you want to move the Arena immediately.
- New in v0.6.6. `<CYF>` `Arena.Hide(boolean showWhenWaveEnds = true)` - Makes the Arena
  invisible, but it will stay active. If `showWhenWaveEnds` is set to true, the Arena will
  be shown again after the end of the wave. Setting it to false will keep it hidden until
  it's manually shown again.
- `<CYF>` `Arena.Show()` - Makes the Arena visible after using `Arena.Hide`.

  New in v0.6.6: This is called automatically whenever a wave ends if the
  `showWhenWaveEnds` argument of `Arena.Hide()` is set to true upon its last use.
