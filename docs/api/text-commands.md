# Text commands

There are **two types** of text commands: commands that get executed instantly, like text
color and effects; and commands that get executed inline, as they're displayed, like wait
commands and character voices. Note that currently, if you skip a text command (with X),
it'll also skip all inline commands that were still in your text. `<CYF>` *Unless*, that
is, you use `playerskipdocommand`.

On line breaks: there are actually two different kinds. In *UI messages* where asterisks
are used, you can use `\n` to start a new line *with* an asterisk. If you want a new line
*without* an asterisk, use `\r`.

This is different for monster dialogue that isn't prefixed with asterisks: *always* use
`\n` for line breaks here. `<CYF>` Or use `autolinebreak`. See
[Special variables](../basics/special-variables.md).

## Instant commands

### `[color:rrggbb]`

This sets the text color for all text after this command to the specified hex code. It
resets per dialogue. `[color:ff0000]` would be red, `[color:555555]` a dark grey.

Here are a few examples:

| Command | Result |
| --- | --- |
| `[color:ff0000]` | Determination |
| `[color:003cff]` | Integrity |
| `[color:00c000]` | Kindness |
| `[color:ffff00]` | Justice |
| `[color:d535d9]` | Perseverance |
| `[color:fca600]` | Bravery |
| `[color:42fcff]` | Patience |

If you have to use colours, try to stick to these. While the option for any colour is
offered, actual usage in Undertale is very limited.

The default UI text is plain white and the default enemy dialogue text is plain black,
`[color:ffffff]` and `[color:000000]` respectively.

### `<CYF>` `[color]`

This command resets any previous usage of the `[color:rrggbb]` command, displaying any
following text in the font's default color.

### `<CYF>` `[alpha:aa]`

This command allows you to set the alpha (transparency) value of your text.

The number you enter should be a two-character hex value between `"00"` and `"ff"`.

### `<CYF>` `[alpha]`

This command resets any previous usage of the `[alpha:aa]` command, displaying any
following text in the font's default alpha.

### `[starcolor:rrggbb]`

Same usage as color but *only affects the first asterisk in a dialogue box that has
asterisks.* This is a dirty workaround, but now you don't need it anymore, it's
deprecated.

`<CYF>` As commands are now executed before the star, you can use `[color:rrggbb]` to
color the star.

### `[effect:x]` or `[effect:x,intensity]` or (new in v0.6.6) `[effect:x,intensity,step]`

Sets the text effect for *the entire message, regardless of position*. You can use the
following effects:

- `none`: No effect on the text.
- `rotate`: Rotating text, most random monsters have this by default. Intensity sets how
  far letters rotate, in pixels. Default is 1.5. New in v0.6.6: Step allows shifting the
  rotation animation of the letters by a given degree in radians.
- `shake`: Shaking text. Flowey uses this sometimes. Intensity sets how far the letters
  offset, in pixels. Default is 1.0. New in v0.6.6: Step allows shifting the frequency of
  the twitching effect. Default is 48 frames. The value will be varied by about `33%`
  every time the delay until the next twitch effect shows up is computed.
- `twitch`: Letters twitch occasionally, the battle UI has this by default. Intensity sets
  how far a letter should shake. Default is 2.0.

### `<CYF>` `[lettereffect:x]` or `[lettereffect:x,intensity]` or (new in v0.6.6) `[lettereffect:x,intensity,step]`

This works the same as `[effect:x]`, but it works **inline**. This means you can have
*multiple* different text effects in a single line.

Keep in mind that, like all other inline text commands, this command will not execute if
the player skips over it with X, unless you set `playerskipdocommand` to `true` first.

New in v0.6.6: On top of that, the default step value for the shake effect is now 500. The
value will be varied by about `80%` every time the delay until the next twitch effect
shows up is computed. It's high so the twitch effects of all letters can play at various
times.

### `[font:x]`

Sets the font for this dialogue. Usually includes a default voice. As the `[font]` command
can change both text colour and voice, if you want to have your own voice or text color do
it after the font change. Possible options:

- `uidialog`: Default large pixel font for UI.
- `monster`: Default font for almost all monsters in the game.
- `sans`: sans. use lowercase. uppercase works, but... sans.
- `papyrus`: PAPYRUS! USE UPPERCASE ONLY. LOWERCASE WON'T WORK.
- `wingdings`: Wingdings.
- `uibattlesmall`: The font used for the character name, HP and level.
- and a lot of others, you can even create your own font.

For all default fonts, check out the `Default/Sprites/UI/Fonts` folder. Every font with a
matching .xml file is mapped.

### `<CYF>` `[charspacing:number]` and `[charspacing:default]`

Sets the number of pixels that separate characters on the same line. Negative means less
space between characters, positive means more space between characters.

The default value is `3`. Fonts can specify a `<charspacing>` tag that *sets* this value.

Enter `[charspacing:default]` to reset this value to the value provided by the font, or
the default value of 3.

### `<CYF>` `[linespacing:number]`

Number of pixels that separate different lines. The number you enter here gets *added on*
to the active font's regular line spacing. Negative means less space between lines,
positive means more space between lines.

By default, the amount of pixels between lines is `1.5 *` the height of the font's "space"
character. If the font has a `<linespacing>` tag, its number value will be the amount of
pixels between lines instead.

Enter `[linespacing:0]` to reset the line spacing to its original value.

## Inline commands

### `[noskip]`

Prevents this dialogue from being skipped by pressing X.

It is effective only whenever the text reaches it, but if it is placed at the beginning of
a line of text, it will always work, even if it is not the first command.

### `<CYF>` `[noskip:off]`

Cancels a previous `[noskip]` command.

### `[instant]`

Instantly shows the entire text without having to wait or press anything.

`<CYF>` Note: This command works as an inline command when placed at the beginning of a
line of text.

### `<CYF>` `[instant:allowcommand]`

The same as `[instant]`, but here other text commands are called, except `[w]`,
`[letters]` and commands with the "`skipover`" tag.

Note: This command works as an inline command when placed at the beginning of a line of
text.

### `<CYF>` `[instant:stop]`

Cancels a previous `[instant]` or `[instant:allowcommand]` command, and resumes typing.

### `<CYF>` `[instant:stopall]`

Cancels a previous `[instant]`, `[instant:allowcommand]` command or a Player skip, and
resumes typing.

### `[w:x]`

The wait command. Pauses your textbox temporarily based on the number you enter.

Technically: This will pause the textbox for `x * 4` in-game frames. Calling `[w:1]` will
freeze the textbox for 4 in-game frames.

`[w:4]` is 16 in-game frames, `[w:10]` is 40, and so on.

### `[waitall:x]`

Makes all letters after this command take `x` times as long to type out, slightly
different from `[w:x]`.

Use `[waitall:1]` to reset text to its default speed.

**Overrides `[speed:x]`.**

### `[voice:filename]`

Sets the voice (sound per letter) to a sound located in `YOUR MOD/Sounds/Voices`. Applies
to all letters after the command. It resets per dialogue. This has to be a .wav file, and
you shouldn't include the file extension when using `[voice]`. If your voice sound is
`YOUR MOD/Sounds/Voices/mettaton.wav`, you can use it with `[voice:mettaton]`.

`[voice:default]` resets to the default voice (beeps). If you have a voice sound named
'default', it will be ignored.

### `[novoice]`

Removes the voice for the letters after this command. It resets per dialogue.

### `[next]`

Skips to the next line of the current text object automatically. If used in a monster's
text object, skips the dialogue of all monsters, without concern for the other monsters'
dialogue's state. You can also use this for textbox trickery. Here's an example to
replicate Flowey's text-changing effect if you dodge the Friendliness Pellets twice.

```lua
first line:  "[noskip][voice:flowey][effect:none]RUN. [w:30]INTO. [w:30]THE.\n[w:30]BULLETS!![w:30][next]"
second line: "[instant][effect:none]RUN. INTO. THE.\nfriendliness\npellets"
```

### `<CYF>` `[nextthisnow]`

Skips to the next dialogue of the current monster only, even if several monsters are
speaking, effectively desyncing their text. It acts like `[next]` outside of monster text
objects, its only difference is that it only affects the current monster if used on a
monster's text object.

### `<CYF>` `[finished]`

Sets this dialogue box as "finished". To go through the next dialogue box, you'll have to
wait until the other monsters' dialogue boxes are also finished.

### `<CYF>` `[waitfor:key]`

Waits for the user to press the given key to continue the message. Check the
[Key list](../reference/key-list.md) to see the available keys.

Note that keybinds are also accepted here. For more info, check the page on
[The Input object](objects/input.md).

### `[func:x]`, `[func:x,argument]` `<CYF>` or `[func:x,{argument1, argument2...}]`

The most powerful command. `[func]` allows you to execute *any function from your script
in line with the text*. Refer to the examples below.

```lua
your dialogue: "hoi hoi this is dog [func:dog] and now the music changed"

function dog()
    Audio.LoadFile("dog_music")
    --plays dog_music.ogg (or .wav) from your Audio folder! for built-in functions like this, refer to section API - Functions & Objects
    --insert more code here, any code!
end
```

```lua
your dialogue: "dog with arguments!! [func:newmusic,temietheme] so intense!"

function newmusic(yourargumentname)
    Audio.LoadFile(yourargumentname) --with this example, it'll load 'temietheme.ogg (or .wav)'...
    --and then play it! THE FUTURE IS NOW! By using an argument, your function can be more versatile.
end
```

**Please note** that the target function must exist in the source script. More commonly,
if you are using it in monster dialogue through `currentdialogue`, your function must
exist within the monster's Lua script, regardless of what script is actually setting the
monster's dialogue.

### `<CYF>` `[speed:x]`

Makes the text handler print `x` characters over every 4 frames. The default text speed in
CYF (`[speed:1]`) is 1 character every 4 frames.

For example, `[speed:4]` will type text at 4x the regular speed: 4 characters every 4
frames, or 1 per frame.

**Supports fractional numbers.** `[speed:0.25]` writes text at 1/4th the regular speed,
for instance.

**Overrides `[waitall:x]`.**

### `<CYF>` `[letters:x]`

For this frame only, the text box will show the next x characters.

New in v0.6.6: If used with the command `[lettersperframe:y]`, the next `x+y-1` letters
will be displayed.

### New in v0.6.6. `[lettersperframe:x]`

For the rest of the line, the text box will show the next x characters every time one
character should be displayed.

### `<CYF>` `[name]`

This text gets replaced by the name of the player. You can also use `Player.name`.

### `<CYF>` `[music:x]`

Plays the music given as an argument on the main music channel, unless the argument is one
of these keywords:

- `play` = Only plays the last played music, from the beginning.
- `pause` = Pauses the music.
- `unpause` = Unpauses the music.
- `stop`, `null`, `nil` or an empty string = Stops the current music.

### `<CYF>` `[sound:x]`

Plays the sound given as an argument.

### `<CYF>` `[mugshot:face]`, `[mugshot:null]` or `[mugshot:{face1,face2,...,time}]`

New in v0.6.6: Only usable with the Overworld's main text as well as the main text object
in battles.

Displays a set face sprite, or sequence of face sprites, next to your dialogue set by
`General.SetDialog`. Note that the overworld's automatic line break system will not
account for face sprites set this way; instead, it's recommended to set a face sprite in
`General.SetDialog` (see [The General object](../overworld/objects/general.md)).

This function loads face sprites from `YOUR MOD/Sprites/Mugshots`, and if none exists,
from `Default/Sprites/Mugshots`. You may put folder names in here, such as
`[mugshot:Papyrus/suspicious]` to get the file
`YOUR MOD/Sprites/Mugshots/Papyrus/suspicious.png`, or if it doesn't exist,
`Default/Sprites/Mugshots/Papyrus/suspicious.png`.

Use `[mugshot:null]` or `[mugshot]` to revert the text box back to normal.

To use a sequence of face sprites, enter something such as
`[mugshot:{Papyrus/happy,Papyrus/suspicious,0.1}]`. This is a list of normal paths to put
into `[mugshot:x]` as described above, followed by the amount of real time seconds between
faces, such as with `sprite.SetAnimation`.

If you do not provide a time, or the time you enter is lower than 0, a default value of
`0.2` will be used.

New in v0.6.6. Note: While the overworld's mugshots have a size of 140x140, in battles the
mugshots are 130x130, so if bigger mugshots are applied in battles, 5 pixels around each
edge are hidden.

### `<CYF>` `[health:x,y]`

Heals the Player by x points. Can damage the player if x is negative, but the Player's HP
will not go under 1 unless certain conditions are met. The parameter y is optional.

Examples:

- `[health:3]` => Heals 3 HP.
- `[health:-5]` => Damages 5 HP. Doesn't go under 1HP.

Tags:

- x (y is unused if these tags are used):
  - `kill` = Kills the Player.
  - `Max-1` = Sets the Player's HP at Player's Max HP minus 1.
  - `Max` = Sets the Player's HP at Player's Max HP.
- y:
  - `killable` = The possible HP reduction can now kill the Player.
  - `set` = The amount of HP given in the first argument will be the Player's current HP.

## `<CYF>` Command tags

These tags can be added after a text command and have different behaviours. To use them,
you must do this: `[command:tag]` or `[command:arguments:tag]`. For now, only one tag can
be added at a time.

### `skipover`

Makes a command only be activated when the text is not skipped, by using any `[instant]`
command or by the player skipping. Has no effect on the commands `[w]` and `[letters]`.

### `skiponly`

The command this is attached to will only be activated if the text has been skipped by
using any `[instant]` command, or if the player skips it manually. This tag disables the
commands `[w]` and `[letters]` if used with them.
