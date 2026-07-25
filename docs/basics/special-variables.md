# Special variables

It's time to set up the basics of an encounter. The fastest way to get started is to copy
a minimal encounter and play with the values in it, then either copy over existing
examples' code or write your own. This section serves to explain the variables you see.

## All-script variables

### `<CYF>` **boolean** `isCYF` [E/M/W]

A value, true by default, that indicates that you're on CYF. nil if not on CYF. Test it
with `if not isCYF then` or `if isCYF then`.

### `<CYF>` **boolean** `isRetro` [E/M/W]

A value which is true whenever CYF's retrocompatibility mode is active and false when it
isn't. Test it with `if not isRetro then` or `if isRetro then`.

### `<CYF>` **boolean** `safe` [E/M/W]

True if CYF's safe mode is enabled, false otherwise.

### `<CYF>` **boolean** `windows` [E/M/W]

Returns true if the user is on Windows, false otherwise.

### `<CYF>` **string** `CYFversion` [E/M/W]

Returns a different string based on the version of CYF you are using.

- `Versions before v0.6`: Previous version's number. For example, in CYF v0.5.5, this
  will be "0.5.4".
- `Versions between v0.6 and v0.6.1.2`: always "1.0".
- `Versions after v0.6.1.2`: Current version's number. For example, in CYF v0.6.2, this
  will be "0.6.2".

Tip: Lua has a very useful built-in string comparing function. You can very easily check
for `if CYFversion < "0.6.2.2" then`, `if CYFversion >= "0.6.1.2" then` and other
combinations.

### `<CYF>` **number** `LTSversion` [E/M/W]

Returns a different number based on the LTS version of CYF you are using.

You can compare this number to your own values to make sure the right version of CYF is
used to play your mod, if needed.

You may also want to check if this value exists at all in case older versions of CYF are
used to play your mod.

## Encounter script variables

```lua
music = "yourmusicname_without_extension"
encountertext = "Vegetoid came out of\rthe earth!"
nextwaves = {"bullettest_wavy", "bullettest_homing"}
wavetimer = 4.0
arenasize = {155, 130}

enemies = { "vegetoid" }

enemypositions = {
{0, 50},
{-70, 30},
{70, 30}
}

deathtext = {
"[voice:v_fluffybuns][waitall:2]You cannot give up\njust yet...",
"[voice:v_fluffybuns][waitall:2]Frisk!\n[w:15]Stay determined..."
}
```

**string** `music` - Name of your encounter's starting music, without the file extension.
If this variable isn't present, it'll play Undertale's default battle theme. If you don't
want any music, call `Audio.Stop()` in the `EncounterStarting()` function. For more
information see [Game events](../api/game-events.md).

**string** `encountertext` - Set the initial text of your encounter here. After that, you
can modify it at any time in preparation for the next turn. `encountertext` gets read out
at the start of every new turn, meaning you going back to the FIGHT/ACT/ITEM/MERCY
selection.

You can use `\n` to create a new line **with a star** (\*), and `\r` to create a new line
**without** a star.

As of CYF v0.6.4, if you end up with 4 or more lines of encounter text displayed at once,
the text will move up (9 pixels, one time) to compensate and try to fit your text inside
the box.

**{table of string}** `nextwaves` - A list of all simultaneous attack waves you want when
the monsters start their attacks. You can modify this at any time, and it'll get read out
before the enemies start their attack. For most boss-type encounters, you'll likely only
want one wave simultaneously, but you can get creative here.

**number** `wavetimer` - How long it takes for the defending step to end. If this isn't
set anywhere, it'll be the default *4.0 seconds*.

**{table of number}** `arenasize` - The inner size of the box the player's constrained
to. `{155, 130}` is the default size for a lot of basic Undertale encounters. Papyrus'
battle, for instance, has this at `{245, 130}` most of the time. You may modify this at
any time, it'll only get read out before the enemies start their attack.

Note: lowest possible setting is `{16, 16}`, this is the size of the player's soul.
Anything lower will be set to 16 anyway.

**{table of string}** `enemies` - Defines the names of your enemy scripts that will be
used in your encounter. In this example, `vegetoid.lua` will be used from the Monsters
folder. After initialization, the names will be replaced by Script controller objects you
can use to control your monster scripts. Refer to
[Functions and objects](../api/objects/misc-functions.md) for more information.

**{table of {number, number}}** `enemypositions` - Defines where the enemies are on the
screen. `{0, 0}` means they're centered just above the arena, with 1 pixel of space in
between. `{-30, 0}` means above the arena to the left; `{50, 80}` means 50 pixels to the
right and 80 pixels above that center.

You will always need at least as many enemy positions as enemies in your encounter. In
this example we have 3 enemy positions set to show you how you can define more than one,
but since this example only contains Vegetoid you only really need one position.

`<CYF>` **boolean** `autolinebreak` - False by default. If this value is set to true, the
auto linebreak system will automatically add line breaks (`\r`) to the text. No need to
use `\r` or `\n` anymore.

`<CYF>` **boolean** `playerskipdocommand` - False by default. If this value is set to
true, text commands will be called even if the player skips the text, except for `[w]`
and `[letters]` commands, and commands with the tag "`skipover`".

`<CYF>` **boolean** `unescape` - False by default. If this value is set to true, you
can't exit the battle with the ESC key anymore.

`<CYF>` **boolean** `flee` - True by default. If this value is set to false, the Flee
option will not appear in the Mercy menu.

`<CYF>` **boolean** `fleesuccess` - `nil` by default. Set this to `true` or `false` to
force the Flee option to succeed or fail, respectively. Otherwise, Undertale's formula is
used, which starts at a 50% chance to flee on the first turn, and increases by 10% every
turn after that, regardless of if those turns were spent trying to flee as well.

`<CYF>` **{table of string}** `fleetexts` - If you set this to a table filled with
strings, a random one of your strings will be displayed whenever the player flees the
battle, if that's enabled.

`<CYF>` **boolean** `revive` - If this variable is set to true, the player will be revived
when they hit 0 HP. By default, there will be no special text for the player being
revived; however, if you set `deathtext`, that will be used.

`<CYF>` **{table of string}** `deathtext` - Text displayed when the player dies, in the
Game Over screen. By default, it'll use the normal death text. This text is also used if
the player gets revived while `revive` is true. Otherwise, there is no revive text.

`<CYF>` **string** `deathmusic` - Sets the death music. The music is played if `revive` is
not set.

`<CYF>` **{table of script}** `Wave` - A table returning the current wave scripts used.
Returns a table with a length of 0 if not in the state DEFENDING.

`<CYF>` **boolean** `noscalerotationbug` - If this variable is set to true, the rotation
of any child sprite with a rotated parent will no longer be reset after either changing
its sprite in any way or scaling it.

New in v0.6.6. **boolean** `adjusttextdisplay` - False if not set. If set to true, CYF
will try to adjust the text's position and scale to prevent jagged lines appearing if the
text's scale or position is slightly off. Can be overridden for each text object by
setting their `adjustTextDisplay` value.

New in v0.6.6. **{table of number, number, number, number = 1}** `sparecolor` - Changes
the RGBA color of the Spare option in the Mercy menu if one or several enemies are
spareable. Each given number will be clamped between 0 and 1. If neither this or
`sparecolor32` are set, the default color is `{ 1, 1, 0 }`, or yellow.

New in v0.6.6. **{table of number, number, number, number = 255}** `sparecolor32` -
Changes the RGBA color of the Spare option in the Mercy menu if one or several enemies are
spareable. Each given number will be clamped between 0 and 255. This value is ignored if
`sparecolor` is set.

## Monster script variables

```lua
comments = {"Vegetoid cackles softly.", "Vegetoid's here for your health."}
commands = {"Talk", "Devour", "Dinner"}
randomdialogue = {"Fresh\nMorning\nTaste", "Farmed\nLocally,\nVery\nLocally"}
currentdialogue = {'Eat\nYour\nGreens'}
cancheck = true
canspare = false

sprite = "vegetoid_sprite"
dialogbubble = "rightshort"
name = "Vegetoid"
hp = 20
atk = 6
def = 6
xp = 6
gold = 1
check = "Serving Size: 1 Monster\nNot monitored by the USDA"
```

**{table of string}** `comments` - A list of random comments attached to this monster. You
can retrieve one at random using the `RandomEncounterText()` function in your Encounter
script. See [Misc. functions](../api/objects/misc-functions.md) for details.

**{table of string}** `commands` - A list of ACT commands you can do. Listed in the ACT
menu and used in `HandleCustomCommand()`. See [Game events](../api/game-events.md) for
details. Note that the behaviour for Check is built-in, and shows you the monster's `name`
followed by the `ATK` and `DEF`, and then the `check` variable you'll see all the way
down.

**{table of string}** `randomdialogue` - A list of random dialogue the monster can have.
One of these is selected at random if `currentdialogue` is `nil`, meaning it has no value.

`<CYF>` Note: The dialogue bubble will not be shown so long as it has no displayable
letters. Set `randomdialogue` to a line with only text commands, such as
`"[noskip][next]"`, to use this to your advantage.

**{table of string}** `currentdialogue` - The next dialogue for this monster. This
overrides the random dialogue and is meant for special actions, for example you hit
Vegetoid's green carrots after selecting Dinner from the ACT menu. This variable gets
*cleared every time after it's read out in the monster dialogue phase*. This is done so
you don't have to take care of managing it manually.

`<CYF>` Note: The dialogue bubble will not be shown so long as it has no displayable
letters. Set `currentdialogue` to a line with only text commands, such as
`"[noskip][next]"`, to use this to your advantage.

`<CYF>` **string** `defensemisstext` - The text which will be displayed if the Player's
attack is successful but deals 0 damage. "MISS" by default.

`<CYF>` **string** `noattackmisstext` - The text which will be displayed if the Player
doesn't press Z when attacking. "MISS" by default.

**boolean** `cancheck` - Either true or false. You can leave this line out; it will be
true by default. If set to false, it will disable the default Check action that shows up
in your ACT menu. If you want a custom Check action, you can add it back into your
`commands` table, and handle it like any other custom command. See
[Game events](../api/game-events.md) for details.

**boolean** `canspare` - Either true or false. If you leave this line out, it'll be set to
false by default. If you change this to true, your monster's name will turn yellow and it
will be spareable.

`<CYF>` **boolean** `isactive` - Tells you whether this enemy is active. Will be false if
they have been manually de-activated, killed or spared.

**Setting this will do nothing. You must call** `SetActive` (see
[Misc. functions](../api/objects/misc-functions.md)).

**string** `sprite` - Name of the sprite in your Sprites folder, without the .PNG
extension. This is the initial sprite for your monster. It can be changed using
`SetSprite(name)`; see [Sprites and animation](../api/sprites-and-animation.md) for
details.

`<CYF>` **sprite** `monstersprite` - Sprite handler of the monster.

**string** `dialogbubble` - What dialogue bubble will be used for the monster's dialogue.
You can change this at any time, but this *must* be initially set to something. For a list
of all possible options, check the
[dialog bubble names](../reference/dialog-bubble-names.md) chart. Positioning of the
bubbles is done automatically.

New in v0.6.6. This value can also be nil, which means the monster's text will use an
automatically sized bubble much like text objects do. In which case, you need to set the
value of the variables `bubbleside` and `bubblewidth`. See their entries below for
details.

New in v0.6.6. **string** `bubbleside` - Only useful when `dialogbubble` is nil.
Determines which side the bubble and its tail are on. Can only accept the values `RIGHT`,
`LEFT`, `UP`, `DOWN` and `NONE`.

The `NONE` value places the bubble on the right side of the monster with no tail, while
the other values place the bubble on their corresponding side.

New in v0.6.6. **number** `bubblewidth` - Only useful when `dialogbubble` is nil.
Determines the width of the automatic bubble in pixels. This value cannot be under 16.

New in v0.6.6. **sprite** `bubblesprite` - Only useful when `dialogbubble` is not nil.
This is the sprite object used for the bubble sprite used for monster dialogues. This
object has `textobject` as child, so moving it will also move that object.

New in v0.6.6. **text** `textobject` - Text object used to display the monster's dialogue.
Moving it also moves the dialog bubble if dialogbubble is nil.

`<CYF>` **string** `dialogueprefix` - A string, appended to the beginning of every
monster's dialogue. The default is `"[effect:rotate]"`.

**string** `name` - Monster name. Fairly self-explanatory; shows up in the FIGHT/ACT menus.
Can also be changed at any time.

**number** `hp` - Your monster's max HP, initially. After the fight has started, this
value will always accurately reflect your monster's current HP. You can then modify this
value to change your monster's current HP.

`<CYF>` **number** `maxhp` - Your monster's max HP. After the fight has started this value
will be always the same, unless you change it. It is mainly used for lifebars and such.
You better not set it as 0 or as a negative number, though.

**number** `atk` - Your monster's ATK. Only used in the default Check handler; bullet
damage is set through wave scripts. If you're not using the default Check you can leave
this out.

**number** `def` - Your monster's DEF.

**number** `xp` - Your monster's XP upon actually defeating them. You only get this by
killing the monster.

**number** `gold` - Gold you get from either killing or sparing this monster. Since the
gold can change based on whether you kill or spare the monster, you can modify this at any
time up until the fight ends.

**string** `check` - When checking with the default Check option, this is what's listed
under the monster's name, ATK and DEF.

`<CYF>` **boolean** `unkillable` - Set it to true and the monster will not be killed if it
has less than 1 HP. However, it can still be killed with `Kill()`.

`<CYF>` **boolean** `canmove` - Deprecated, always returns true. Old behavior: Returns
true if you are able to move or unbind `monstersprite`, false otherwise.

`<CYF>` **number** `posx` - The x position of the enemy's sprite.

`<CYF>` **number** `posy` - The y position of the enemy's sprite.

`<CYF>` **string** `font` - The default font used by the monster. Set it to `nil` if you
want to use the normal monster font.

`<CYF>` **string** `voice` - The default voice used by the monster. Set it to `nil` if you
want to use the default voice.

New in v0.6.6. **{table of number, number, number, number = 1}** `sparecolor` - Changes
the RGBA color of the enemy's name in the enemy choice menu if they are spareable. Each
given number will be clamped between 0 and 1. If neither this or `sparecolor32` are set,
the default color is `{ 1, 1, 0 }`, or yellow.

New in v0.6.6. **{table of number, number, number, number = 255}** `sparecolor32` -
Changes the RGBA color of the enemy's name in the enemy choice menu if they are spareable.
Each given number will be clamped between 0 and 255. This value is ignored if `sparecolor`
is set.

## Wave script variables

### `<CYF>` **string** `wavename` [W]

Returns the name of the wave file, without the extension, from the Waves folder.

Other than the above, wave scripts don't have any variables that are read out from the
start, but you can define your own. An instance of a wave script is made when you start
defending, and is destroyed when the defending step ends. As such, you can't store
variables in a wave script for reusing later. Use the Encounter script to keep track of
things.
