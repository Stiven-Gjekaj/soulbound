# The Input object

### The Input object [E/M/W]

The Input object allows you to retrieve input status.

All keys will return a number. Here are the different values each variable or function
below returns, if it returns a number:

- `2`: The key is being held
- `1`: The key is being pressed this very frame
- `0`: The key isn't being pressed
- `-1`: The key has been released this very frame

Following these values, you can check if a key's value is greater than 0 to see if it's
pressed, or if it's exactly 1 or -1 to only have an action if it was just pressed or
released.

Possible key options are below:

- `Input.Confirm` - Z or Enter
- `Input.Cancel` - X or any Shift key
- `Input.Menu` - C or any Control key
- `Input.Up` - Up arrow or W
- `Input.Down` - Down arrow or S
- `Input.Left` - Left arrow or A
- `Input.Right` - Right arrow or D

Note: do not rely on the Input object to replace proper UI controls. Changing game state in
the UI based on input will likely cause a fair share of issues and is not supported at
this moment, but feel free to see what does and doesn't work.

Here follows a list of other variables and functions available within the Input object.
New in v0.6.6: Keep in mind that the [Key list](../../reference/key-list.md) page shows a
list of most available keys within the engine.

- **number** `Input.MousePosX` (readonly) - Returns the X position of the mouse
  relative to the bottom left corner of the screen, from `0` to `639`.
- **number** `Input.MousePosY` (readonly) - Returns the Y position of the mouse
  relative to the bottom left corner of the screen, from `0` to `479`.
- **boolean** `Input.IsMouseInWindow` (readonly) - Returns true if the mouse is in
  the window, false otherwise.
- New in v0.6.6. **boolean** `Input.IsMouseVisible` - Returns true if the mouse
  cursor is visible, false otherwise. True by default. Can be set in order to hide or show
  the mouse cursor.
- **number** `Input.MouseScroll` (readonly) - Returns a number representing the
  change in the user's scroll wheel position, or movement supplied by the trackpad when
  using a Mac. `0` represents no movement, while a positive number means the user is
  scrolling up, and a negative number means the user is scrolling down.
- **number** `Input.GetKey(string keyname)` - Gets the state of the given key.
- New in v0.6.6. **number** `Input.GetAxisRaw(string axisName)` - Gets the precise
  state of the given axis (joystick, trigger, D-Pad), between `-1` and `1`.
- New in v0.6.6. **{table of string}** `Input.GetPressedKeys()` - Gets a list of
  all the keys which are currently pressed. This function only takes keys into account, not
  axes.
- New in v0.6.6. **{table of string}** `Input.GetHeldKeys()` - Gets a list of all
  the keys which are currently held. This function only takes keys into account, not axes.
- New in v0.6.6. **{table of string}** `Input.GetReleasedKeys()` - Gets a list of
  all the keys which are currently being released. This function only takes keys into
  account, not axes.
- New in v0.6.6. `Input.CreateKeybind(string name, {table of string} keys = { })` -
  Creates a keybind, which can be used as a shortcut to get the state of several keys at
  the same time.

  For quick setup, you can give it a list of keys to bind to it immediately, or you can
  only give it a name and use the functions `Input.SetKeybindKeys()` or
  `Input.BindKeyToKeybind()` to bind keys to it afterwards.
- New in v0.6.6. `Input.RemoveKeybind(string name)` - Completely removes an
  existing keybind.

  Note that base CYF keybinds cannot be deleted, as it would cause errors when the engine
  tries to fetch them.
- New in v0.6.6. `Input.SetKeybindKeys(string name, {table of string} keys)` OR
  `Input[string name] = {table of string} keys` - Replaces a keybind's list of bound keys
  with the given list of keys, erasing any keys set before running this function.
- New in v0.6.6. **boolean** `Input.BindKeyToKeybind(string name, string key)` -
  Adds a key to the keys bound to the given keybind. Returns true if the key was
  successfully bound to the keybind, false otherwise.

  If the key is already bound to the given keybind, nothing will happen and this function
  will return false.
- New in v0.6.6. **boolean** `Input.UnbindKeyFromKeybind(string name, string key)` -
  Removes a key from the keys bound to the given keybind. Returns true if the key was
  successfully unbound from the keybind, false otherwise.

  If the key is not bound to the given keybind, nothing will happen and this function will
  return false.
- New in v0.6.6. **number** `Input.GetKeybind(string name)` OR `Input[string name]` -
  Gets the state of the given keybind.
- New in v0.6.6. **{table of string}** `Input.GetKeybindKeys(string name)` -
  Returns a table of the keys currently bound to the given keybind.
- New in v0.6.6. **{table of {table of string}}** `Input.GetKeybindConflicts()` -
  Returns a table containing one table for each existing keybind conflict. Each conflict
  table contains as its first value the key that currently has a conflict, with each
  following value being the keybinds containing this key.

  Example: If the key `T` is bound to both keybinds `Confirm` and `Cancel`, then the
  resulting conflict table will be `{ "T", "Confirm", "Cancel" }`.
- New in v0.6.6. `Input.ResetKeybinds()` - Resets all created and modified keybinds
  within the encounter, effectively wiping any keybind changes made in this encounter.
