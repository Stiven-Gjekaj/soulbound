# `<CYF>` The Event Overworld object

Note: You can provide `"Player"` as the `name` argument for any of these functions to instead
apply them to the actual Player.

### `Event.Exists(string name)` returns **boolean**

Returns `true` if an event with this name exists, `false` otherwise.

Note that this refers to the name given to the GameObject in the Unity scene, rather than the
name of the script attached to it.

### `Event.Teleport(string name, number dirX, number dirY)`

Teleports the given event to a given point in space. The position (dirX and dirY) is computed
based off of the bottom left corner of the map.

### `Event.MoveToPoint(string name, number dirX, number dirY, boolean wallPass = false, boolean waitEnd = true)`

Moves an event to a given point in space. The event's speed will be the speed variable of its
`EventOW` component, so choose that wisely, or use `Event.SetSpeed()`.

Set `wallPass` to true to ignore any obstacles that the event may encounter while moving.

By default, the script will pause until the event has stopped moving. Set `waitEnd` to false
to bypass this and continue the script instead.

By calling this function on an Event that is already moving, the event's current movement
will be cancelled, and will start over moving towards the newly provided target destination.
This also means you can provide the Event's current position when interrupting movement to
force the Event to stop moving instantly.

### `Event.isMoving(string name)`

Returns `true` if an event is currently moving by means of `Event.MoveToPoint`, `false`
otherwise.

Note: this will also return `true` if being used with the Player, given the Player is walking
around manually with the arrow keys or WASD.

### `Event.SetAnimHeader(string name, string anim)`

If the event given has a `CYFAnimator` component, this will set its `Special Header` property
to `anim`.

Set `anim` to `""` to reset to the default animation, `StopDown`.

Read about how the `Special Header` works in
[How to animate an event](../how-to-animate-an-event.md).

### `Event.GetAnimHeader(string name)` returns **string**

If the event given has a `CYFAnimator` component, this will return its `Special Header`
value.

### `Event.GetDirection(string name)` returns **number**

If the event has a CYFAnimator component, this returns the direction the event is facing.

Possible values:

- `2`: Down
- `4`: Left
- `6`: Right
- `8`: Up

### `Event.SetDirection(string name, number dir)`

If the event has a CYFAnimator component, this sets the direction the event is facing.

Possible values:

- `2`: Down
- `4`: Left
- `6`: Right
- `8`: Up

### `Event.Rotate(string name, number rotateX, number rotateY, number rotateZ, boolean anim = true, boolean waitEnd = true)`

This function rotates the event. The Z rotation is the rotation normally applied in battles,
while the X and Y rotations are 3D rotations.

Set `anim` to false to make the rotation instantaneous.

By default, the script will pause until the event has stopped rotating. Set `waitEnd` to
false to bypass this and continue the script instead.

By calling this function on an Event that is already rotating, the event's current rotation
will be cancelled, and will start over moving towards the newly provided target angles.

### `Event.isRotating(string name)`

Returns `true` if an event is currently being rotated by means of `Event.Rotate`, `false`
otherwise.

### `Event.Stop()`

If this function is called in a normal event, it stops the event instantly.

If called in a coroutine event, this will call `Event.StopCoroutine()` instead.

If you want to stop the coroutine instantly, just add `return` after this function.

### `Event.StopCoroutine(string eventName = "thisevent")`

Stops the coroutine of the given event.

Use the string `thisevent` to stop the coroutine of the calling event.

### `Event.Remove(string name)`

Removes the given event from the scene. Then, it won't be accessible until the player
re-enters the room.

To permanently remove an event, set its page to -1.

### `Event.SetPage(string name, number page)`

Sets the page of a given event. Set it to -1 to totally remove the event.

### `Event.GetPage(string name)` returns **number**

Returns the current page of the given event.

### `Event.GetSprite(string name)` returns **sprite**

Returns the sprite object of the given event.

### `Event.CenterOnCamera(string name, number speed = 5, boolean straightLine = false, boolean waitEnd = true)`

Centers the camera on the given event.

The camera's speed will be `speed` pixels per second in each direction, unless you set
`straightLine` to true. In that case, the camera will follow a line to the center of the
event.

By default, the script will halt execution until the camera has stopped moving. Set `waitEnd`
to false to bypass this and continue the script instead.

### `Event.GetName()` returns **string**

Returns the name of the calling event.

### `Event.GetPosition(string name)` returns **{table of number, number}**

Returns a table of two values containing the position of the given event.

### `Event.IgnoreCollision(string name, boolean ignore)`

Sets the collision mode of the event. If `ignore` is set to true, all other events may pass
through it normally.

Events activated when the player touches them, touch-enabled events, see
[How to create an event](../how-to-create-an-event.md), need their collision to be ignored in
order to function properly.

### `Event.SetSpeed(string name, number speed)`

Sets the set movement speed of an event.

### `Event.GetSpeed(string name)` returns **number**

Gets the set movement speed of an event.
