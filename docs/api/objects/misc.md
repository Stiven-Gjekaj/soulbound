# The Misc object

### The Misc object [E/M/W]

The Misc object has some window-related and computer-related functions. Most of them are
functions.

- **string** `Misc.MachineName` (read-only) - Returns the name of the player's session.
- **string** `Misc.OSType` - Returns `Windows`, `Mac` or `Linux`.
- **shader** `Misc.ScreenShader` - A shader object that affects the entire screen, instead
  of a single sprite. See [The Shader object](../../shaders/shader-object.md) for more
  information.
- `Misc.ShakeScreen(number durationInFrames, number intensity = 3, boolean isIntensityDecreasing = true)` -
  Shakes the screen for `durationInFrames` frames, with a maximum of `intensity` pixels
  away from the normal position. Set `isIntensityDecreasing` as true to reduce the
  intensity over time.
- `Misc.StopShake()` - Stops a previous `Misc.ShakeScreen` call, does nothing otherwise.
- `Misc.MoveCamera(number x, number y)` - Moves the camera right `x` pixels and up `y`
  pixels.
- `Misc.MoveCameraTo(number x, number y)` - Moves the bottom-left of the camera directly to
  the point (`x`, `y`). `(0, 0)` is the starting position.
- `Misc.ResetCamera()` - Resets the position of the camera. Same as calling
  `MoveCameraTo(0, 0)`.
- **number** `Misc.cameraX` - Gets or sets the current x position of the bottom-left corner
  of the camera.
- **number** `Misc.cameraY` - Gets or sets the current y position of the bottom-left corner
  of the camera.
- **boolean** `Misc.FullScreen` - Gets or sets whether the user is in fullscreen mode.
- **boolean** `Misc.ResizeWindow(number width, number height)` - Resizes CYF's window to
  the given width and height, in pixels. The parameters `width` and `height` must both be
  positive, and the function will return `true` if the window was successfully resized,
  false otherwise. The window will not be resized if the requested width or height are
  bigger than the player's screen's respective width or height.

  It also works in fullscreen; this function will make sure the given area will be
  displayed on screen at all times by adding black borders if needed.

  Note: Only allowed during battles.
- **number** `Misc.WindowWidth` - Returns the width of the game window when running in
  windowed mode, even in fullscreen.

  If set, it will set the width of CYF's window in pixels. CYF's default width is 640
  pixels. Will do nothing if the player's screen's width is lower than the requested width.

  Note: Only allowed during battles.
- **number** `Misc.WindowHeight` - Returns the height of the game window when running in
  windowed mode, even in fullscreen.

  If set, it will set the height of CYF's window in pixels. CYF's default height is 480
  pixels. Will do nothing if the player's screen's height is lower than the requested
  height.

  Note: Only allowed during battles.
- **number** `Misc.ScreenWidth` (read-only) - Returns the width of the user's screen in
  windowed mode, or the number of displayed pixel columns when in fullscreen.
- **number** `Misc.ScreenHeight` (read-only) - Returns the height of the user's screen in
  windowed mode, or the number of displayed pixel rows when in fullscreen.
- **number** `Misc.MonitorWidth` (read-only) - Returns the width of the user's monitor,
  regardless of the user's settings or if `Misc.SetWideFullscreen` has been used.
- **number** `Misc.MonitorHeight` (read-only) - Returns the height of the user's monitor,
  regardless of the user's settings or if `Misc.SetWideFullscreen` has been used.
- `Misc.SetWideFullscreen(boolean wide)` - If the user is playing with an aspect ratio wider
  than 4:3, this function will expand the view in fullscreen to show the space to the left
  and right of the 640x480 screen space.

  It will only take effect when in fullscreen, and will automatically resize the view if
  already in fullscreen. If the user's aspect ratio is 4:3 or thinner, nothing will change.
  Note that object positions, such as projectiles and sprites, will remain the same.

  Note: Only allowed during battles.

  You can use this code to determine how much space you have available on either side,
  still using the relative position system in which the screen is always assumed to be
  640x480:

  ```lua
  local displayedWidth = Misc.MonitorHeight / 0.75
  local scale = 640 / displayedWidth
  local widthOnEachSide = math.max(scale * (Misc.MonitorWidth - displayedWidth) / 2, 0)

  CreateProjectileAbs("bullet", 0 - widthOnEachSide, 240) -- creates a bullet at the absolute left side of the screen when in wide fullscreen
  ```

  Here, if `widthOnEachSide` is 0, the user is using a resolution that is 4:3 or thinner.
  Be sure to check if this number is greater than 0 before doing operations that require
  fullscreen.
- `Misc.DestroyWindow()` - Closes the window instantly.

## The debugger

- **number** `Misc.debuggerX` - Change the debugger's x position relative to the camera's
  position.
- **number** `Misc.debuggerY` - Change the debugger's y position relative to the camera's
  position.
- **number** `Misc.debuggerAbsX` - Change the debugger's x position relative to the
  encounter's bottom-left corner.
- **number** `Misc.debuggerAbsY` - Change the debugger's y position relative to the
  encounter's bottom-left corner.
- `Misc.MoveDebugger(number x, number y)` - Move the debugger relative to its current
  position.
- `Misc.MoveDebuggerTo(number x, number y)` - Move the debugger relative to the camera's
  current position.
- `Misc.MoveDebuggerToAbs(number x, number y)` - Move the debugger relative to the
  encounter's bottom-left corner.
- **boolean** `Misc.isDebuggerAttachedToCamera` - Whether or not the camera should move the
  debugger along with it when it moves. True by default.

Additional notes on the debugger:

- The debugger's pivot point is its top-right corner.
- Its default absolute position is (620, 480).
- Its width is 320 and its height 140.
- If `Misc.isDebuggerAttachedToCamera` is true when the window is resized, the debugger
  will stick to the top right corner of the screen.

## Windows only

The following variables and functions are **Windows only**. Don't forget to check if the
user is playing with a Windows OS by using the variable `windows`.

- **string** `Misc.WindowName` - Gets or sets the name of the window.
- **number** `Misc.WindowX` - Gets or sets the X position of the bottom left corner of the
  window.
- **number** `Misc.WindowY` - Gets or sets the Y position of the bottom left corner of the
  window.
- `Misc.MoveWindow(number X, number Y)` - Moves the window X pixels horizontally and Y
  pixels vertically.
- `Misc.MoveWindowTo(number X, number Y)` - Moves the bottom left corner of the window to
  the given coordinates.

## File functions

### **boolean** `Misc.FileExists(string path)`

Use this function to check if a file exists.

- `path`: Path to the file to open, relative to your Mod folder. Can enter subfolders such
  as `Lua`.

Use this before `Misc.OpenFile` if you plan to read from a file.

### **{table of string}** `Misc.ListDir(string path, boolean getFolders = false)`

This function returns a list of either files or folders.

- `path`: Path to the file to open, relative to your Mod folder. Can enter subfolders such
  as `Lua`.
- `getFolders`: If true, this function will return the names of all **Folders** in `path`.
  Otherwise, this function will return the names of all **Files** in `path`.

### **boolean** `Misc.DirExists(string path)`

Use this function to check if a folder exists.

- `path`: Path to the file to open, relative to your Mod folder. Can enter subfolders such
  as `Lua`.

### **boolean** `Misc.CreateDir(string path)`

Attempts to create a folder with the path `path` relative to your Mod folder.

Returns `true` if the operation was successful, `false` otherwise.

### **boolean** `Misc.MoveDir(string path, string newPath)`

Attempts to move a folder with the path `path` relative to your Mod folder to a new
location `newPath` also relative to your mod folder.

Returns `true` if the operation was successful, `false` otherwise.

### **boolean** `Misc.RemoveDir(string path, boolean force = false)`

Attempts to delete a folder with the path `path` relative to your Mod folder.

- `force`: If true, this function will forcefully remove a folder. Otherwise, a folder will
  only be removed if it is empty.

Returns `true` if the operation was successful, `false` otherwise.

### `Misc.OpenFile(string path, string mode = "rw")` returns **File**

This function opens a file in CYF's Mods folder ONLY for writing, and anywhere for reading.

- `path`: Path to the file to open, relative to your Mod folder. Can enter subfolders such
  as `Lua`.
- `mode`: Determines how the file should be opened:
  - `r`: The file can only be read from.
  - `w`: The file can only be written to.
  - `rw`: The file can be read from AND written to.

  Files can be created through use of `File.Write` (see below).

This function returns a **File** object representing the file.

## The File object [E/M/W]

### All modes

- **string** `File.openMode` (read-only): Returns the file mode being used by this File
  object. This is what you entered for `mode` in `Misc.OpenFile`.
- **number** `File.lineCount` (read-only): The number of lines in the file.
- **string** `File.filePath` (read-only): The full file path to the opened file.
- `File.Move(string new_path)`: Moves the opened file to the path `new_path`. Can be used
  to rename a file.
- `File.Copy(string new_path, boolean overwrite = false)`: Copies the opened file to the
  path `new_path`. If `overwrite` is true, a file at the target path that already exists
  will be replaced with the opened file.

### `"r"`

- **string** `File.ReadLine(number line)`: Returns the contents of line #`line` as a
  string.
- **{table of string}** `File.ReadLines()`: Returns every line in the file as a separate
  string, all within one table. Same as using `File.ReadLine` on every line.
- **{table of number}** `File.ReadBytes()`: Returns a table containing every individual
  byte in the file, start to finish, represented as a number.

  Be prepared for a lag spike as soon as you use this function. Depending on the size of
  the file, the resulting table can be very large.

### `"w"`

- `File.Write(string data, boolean append = true)`: Writes the text in `data` to the file.
  Supports `\n`.

  Creates the file if it doesn't exist.

  If `append` is true, the new text will be added to the end of the file, without a new
  line. Otherwise, the entire file will be replaced by `data`.
- `File.ReplaceLine(number line, string data)`: Sets the contents of line #`line` in the
  file to `data`. Supports `\n`.
- `File.DeleteLine(number line)`: Removes line #`line` from the file. The line that came
  after it previously is now line #`line`.
- `File.WriteBytes({table of number} data)`: Writes a series of bytes to a file. This will
  replace all contents of the file.
- `File.Delete()`: Deletes the file. If using `"w"` or `"rw"` mode, you can use
  `File.Write` to create the file again.
