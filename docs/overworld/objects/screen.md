# `<CYF>` The Screen Overworld object

### `Screen.DispImg(string path, number id, number posX, number posY, number toneR = 255, number toneG = 255, number toneB = 255, string toneA = 255)`

This function displays an image on a given point on the map. This image will take the form of
an event, and this event will be named `"Image" + id`.

Also, you'll have to give the position of your image using `posX` and `posY`.

You can add a color to the image with the 4 last values, if you want to. If you do, these
values must be between 0 and 255.

### `Screen.SupprImg(string id)`

Deletes an image previously created with a `Screen.DispImg()` function.

In application, it's basically the same as `Event.Remove("Image" .. id)`.

### `Screen.SetTone(boolean anim, boolean waitEnd, number r = 255, number g = 255, number b = 255, number a = 128)`

Creates a solid color image over the whole screen with a given color, for instance you can
use this for a sepia tone.

Set `anim` to true to gradually transition to the tone. If `anim` is set to true, you can
also set `waitEnd` to true to make the event's code pause until the transition is done. For
the other values, they must be between 0 and 255.

128 is the recommended value for `a`. The `a` value can go up to 255, which means fully
opaque, but at that point the screen will be fully obscured.

Note: To undo the effects of this function, call `SetTone` again but with the argument `a` as
`0`. You can remove it instantly as well if you set `anim` to false.

### `Screen.Flash(number frames, number colorR = 255, number colorG = 255, number colorB = 255, number colorA = 255, boolean waitEnd = true)`

Flashes the screen for a given number of frames. A flash is an image that is displayed on top
of the entire screen and fades afterwards.

Change the color of the flash by changing the values of the 4 other variables. Each one must
be between 0 and 255.

By default, the script will halt execution until the flash has stopped. Set `waitEnd` to
false to bypass this and continue the script instead.

### `Screen.CenterEventOnCamera(string name, number speed = 5, boolean straightLine = false, boolean waitEnd = true)`

This function is an alias of `Event.CenterOnScreen()`.

Centers the camera on the given event. The camera will move at `speed` pixels per second in
each direction, unless you set `straightLine` to true. In that case, the camera will follow a
line to the center of the event.

By default, the script will halt execution until the camera has stopped moving. Set `waitEnd`
to false to bypass this and continue the script instead.

### `Screen.MoveCamera(number pixX, number pixY, number speed = 5, boolean straightLine = false, boolean waitEnd = true)`

Moves the camera to a given position from the bottom left center of the screen.

The camera's speed will be `speed` pixels per second in each direction, unless you set
`straightLine` to true. In that case, the camera will follow a line to reach the destination
you entered.

By default, the script will halt execution until the camera has stopped moving. Set `waitEnd`
to false to bypass this and continue the script instead.

### `Screen.ResetCameraPosition(number speed = 5, boolean straightLine = false, boolean waitEnd = true)`

Centers the camera on the player.

The camera's speed will be `speed` pixels per second in each direction, unless you set
`straightLine` to true. In that case, the camera will follow a line to the player's center.

By default, the script will halt execution until the camera has stopped moving. Set `waitEnd`
to false to bypass this and continue the script instead.
