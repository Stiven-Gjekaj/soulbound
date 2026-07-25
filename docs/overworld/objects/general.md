# `<CYF>` The General Overworld object

### `General.SetDialog({table of string} texts, boolean formatted = true, {table of string} mugshots = nil, boolean forcePosition = nil)`

Displays text.

- `texts` = The different lines of text displayed.
- `formatted` = Adds asterisks to your text. If true: `\n` for asterisks, `\r` for no
  asterisks. True by default.
- `mugshots` = A sequence of mugshots (dialogue faces) to display, one per line of text. All
  images are loaded from `YourMod/Sprites/Mugshots/`.

  Each mugshot can either be:

  - A single string, for one face for the whole line of text, or `null` to hide the face for
    this line. Examples: `"Punder/normal"`, `"null"`.
  - Another table, containing multiple strings representing what faces to animate between
    for this line and, optionally, a number, the amount of time between faces, 0.2 by
    default. Examples: `{"Booster/normalT", "Booster/normal"}`,
    `{"Asriel/happyT", "Asriel/happy", 0.3}`.

  See `mugshot` in [Text commands](../../api/text-commands.md), as it works the same way as
  this.
- `forcePosition` = `true` to place the text box at the top of the screen, `false` to place
  it at the bottom of the screen, or `nil` to decide automatically.

  Warning: If you want to use this argument, `mugshots` must not be `nil`. Set it to `{}`
  instead.

### `General.SetChoice({table of string} choices, string question = null, boolean forcePosition = nil)`

Displays a choice screen.

The response will be returned to a variable named `lastChoice` as either `0` for the first
option or `1` for the second option.

Each choice must have no more than 3 lines. If you want there to be a question, a prompt to
appear above the options, it must be no more than one line long.

As with `General.SetDialog`, set `forcePosition` to `true` to place the text box at the top
of the screen, `false` to place it at the bottom of the screen, or `nil` to decide
automatically.

Warning: If you want to use this argument, `question` must not be `nil`. Set it to `""`
instead.

### `General.EndDialog()`

Forcefully closes the Overworld text box and continues the event that opened the textbox if
applicable. Applies to `General.SetDialog`, `General.SetChoice`, and text sequences started
from the pause menu, such as dropping an item or using the cell phone.

Can only be run from a **Parallel Process** event page, also called a coroutine.

If used with `General.SetChoice`, `lastChoice` will NOT be set, so be careful.

If the Overworld text box is not open, does nothing.

### `General.Wait(number frames)`

Pauses code execution for a given amount of frames.

### `General.WaitForInput()`

Waits for the player to press the `Confirm` input (Enter or Z) to continue.

### `General.GameOver({table of string} deathText = nil, string deathMusic = nil)`

Starts the Game Over cutscene, putting the player back at their save point afterwards. You
can choose the text and the music if you want to. Otherwise, it'll play the normal music and
show a default message.

### `General.PlayBGM(string bgm, number volume)`

Plays music on the main music channel with a volume between 0 and 1.

Watch out, `NewAudio["src"]` ISN'T the main music channel when music is kept between
battles.

### `General.StopBGM(number fadeFrames = 0, boolean waitEnd = false)`

Stops the music currently being played in the main music channel.

If `fadeFrames` is greater than 0, the music will fade out over that many frames. If
`waitEnd` is set to true, the script will wait until the BGM has stopped before continuing.

### `General.PlaySound(string sound, number volume = 0.65)`

Plays a sound in a temporary sound channel with a volume between 0 and 1.

### `General.Save(boolean forced = false)`

Opens the Save UI that allows the player to save their progress.

Note that the "save point name", the text displayed in the Save UI after saving and on the
Undertale continue/load game screen, uses a set string for the map you are in. You can change
these values by changing the function `AddKeysToMapCorrespondanceList` in the file
`Assets/Scripts/Util/UnitaleUtil.cs`.

Set `forced` to true to instantly save the game without asking the player's permission.

### `General.TitleScreen()`

Exits the current game and sends the player back to the title screen.

### `General.SetBattle(string encounterName = "", string anim = "normal", boolean ForceNoFlee = false)`

Initiates a battle with a chosen encounter, leave out the `.lua` in `encounterName`.

Set `encounterName` to `""` to choose a random encounter from the current map's set mod
folder, except for ones that have `#` in their name. If you want to load an encounter with
`#` at the beginning of its name, make SURE to include the `#` character as well.

If you want the battle's intro to be quick like Undyne's battle, set `anim` to `"fast"`. You
can also set it to `"instant"` to enter the battle immediately on the next frame.

Finally, `ForceNoFlee` removes the flee option from the encounter, unless your encounter
sets `flee` to true.

**Very important:** Calling `General.SetBattle` will STOP the current Event sequence. This
means that **code you place AFTER** `General.SetBattle` **will NOT activate**.

However, all events' `EventPage0()` functions will automatically be called when the battle
ends. Use this, and possibly Real Globals, to your advantage to detect when a battle ends.

### `General.EnterShop(string scriptName, boolean instant = false)`

Enters a Shop scene, using the shop .lua script given in `scriptName`. The Shop script must
be in `YOURMOD/Lua/Shops/`. Check the [How to create a shop](../how-to-create-a-shop.md)
section for more information.

Set `instant` to true to instantly warp to the shop. Otherwise, a short fade animation will
play.

Note: Just like with `General.SetBattle`, using this function will halt the rest of your
event script.
