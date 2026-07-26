# The Discord object

### The Discord object [E/M/W]

This class is used to manipulate the data the `Discord Rich Presence Status` bar displays,
if Discord is open and if this feature is enabled. This feature can be customized in
Create Your Frisk's options screen, accessible from the mod selection screen. This object
*cannot* override the user's settings for Discord Rich Presence.

- `Discord.SetName(string name)`: Sets the top row of the `Discord Rich Presence Status`
  bar, if enabled in the user's options.

  By default, this line displays `Playing Mod: MODNAME`, with `MODNAME` being the name of
  your mod.

  Setting `name` to `""` doesn't display the line at all.
- `Discord.ClearName(boolean reset = false)`: Resets the top row of the
  `Discord Rich Presence Status` bar, if enabled in the user's options.

  If `reset` is true, the entire line will be hidden. Otherwise, it will be reset to its
  default value (see `Discord.SetName` above).
- `Discord.SetDetails(string details)`: Sets the second row of the
  `Discord Rich Presence Status` bar, if enabled in the user's options.

  By default, this line displays the name of the currently running encounter file.

  Setting `details` to `""` doesn't display the line at all.
- `Discord.ClearDetails(boolean reset = true)`: Resets the second row of the
  `Discord Rich Presence Status` bar, if enabled in the user's options.

  If `reset` is true, the entire line will be hidden. Otherwise, it will be reset to its
  default value (see `Discord.SetDetails` above).
- `Discord.SetTime(number time, boolean countdown = false)`: Sets the current time
  displayed in the `Discord Rich Presence Status` bar, if enabled in the user's options.

  The `time` value is a number of seconds. The displayed time is in the `minutes:seconds`
  format, so every multiple of 60 in `time` will display as one minute.

  If `countdown` is true, the given time will be displayed as a countdown timer, counting
  down to 0:00 from the value you entered (in the format `mm:ss remaining`). Otherwise,
  the given time will be displayed as an elapsed time counter, counting upwards from the
  value you entered (in the format `mm:ss elapsed`).

  By default, this line will display the time elapsed since the encounter was started.
- `Discord.ClearTime(boolean reset = false)`: Resets the current time displayed in the
  `Discord Rich Presence Status` bar, if enabled in the user's options.

  If `reset` is true, the entire line will be hidden. Otherwise, it will be reset to its
  default value (see `Discord.SetTime` above).
