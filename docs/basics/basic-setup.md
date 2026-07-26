# Basic setup

By this point you'll probably want to set up an encounter of your own. Currently,
scripts are set up like this:

- **Monster scripts** contain information about your enemies. Here, you'll set things
  like their ATK, DEF, HP, random comments that might show up as encounter texts, random
  dialogue, and what ACT commands they have.
- **Encounter scripts** contain a set of monsters, a set of wave scripts (that you can
  modify at any point), size of the arena, custom interactions for items, and so on. A
  mod can have multiple encounter scripts.
- **Wave scripts** contain an update function. You may use these to spawn, track, modify
  and otherwise interact with bullets during the defending phase of the game.

The boss select screen lists one row per boss, taken from the boss registry, and each
row names one encounter script. See [Adding a boss](adding-a-boss.md).

the only folder required within a mod is the
`YOUR MOD/Lua/Encounters/` folder. For all older versions, your encounter will break if
the other folders are missing.

## Files and directories

It's fairly self-explanatory. If you just want to move on fast, feel free to skip this
section and go to the next one. If for any reason some of your files don't work, you
might want to read this anyway.

### Scripts

The *Encounter scripts* are located in `YOUR MOD/Lua/Encounters/`. The *Monster scripts*
are in `YOUR MOD/Lua/Monsters/`, and your wave scripts at `YOUR MOD/Lua/Waves/`. If
you're getting started, check out these files in example encounters to see how they're
put together.

You may also create a `YOUR MOD/Lua/Libraries/` folder. You can put libraries other
people have made in here, or create your own, for use in your other scripts.
Libraries and modules are more Lua functionality than engine functionality, so
please read up about them at the
[Lua modules tutorial](http://lua-users.org/wiki/ModulesTutorial) instead.

### Music

Music can be put in `YOUR MOD/Audio/`. Your music must be in .ogg or .wav format.
[Audacity](http://audacityteam.org/download/) can export to .ogg if you're missing the
appropriate software.

### Sounds

Sounds can be put in `YOUR MOD/Sounds/`. They must be in .ogg or .wav format. You can
play them with `Audio.PlaySound(filename)`; more on this in
[The Audio object](../api/objects/audio.md).

### Voices

Voices can be put in `YOUR MOD/Voices/`. They must be in .ogg or .wav format, although
.wav is generally recommended. You can use them with the `[voice]` text command; more on
this in [Text commands](../api/text-commands.md).

### Sprites

Sprites can be put in `YOUR MOD/Sprites/`. They must be in the .png format. Note that
most vanilla Undertale monster sprites start with a small base resolution, then resize
the sprite to 2x its original resolution for an oldschool look.

To *add a background* you can have one file titled `bg.png` in the sprites folder. This
image will stretch over the entire background, so 640x480 resolution is recommended.
Actually modifying and animating the background from the Lua side is not possible;
however, in the engine, you may use sprite layers to create a sprite behind the built-in
background layer and animate *that* instead.

## The Default directory

The engine has a "Default" directory. This is where resources
from Undertale reside. It is not advised to modify files in this directory, as they are
expected to be the same across all installations.

If you wish to replace any of the files for your mod, create a file with the same name
at the same location instead. For instance, if you want to change the player soul hurt
sound, don't replace `Default/Sounds/hurtsound.wav`. Instead, create a new file located
at `YOUR MOD/Sounds/hurtsound.wav`. This also goes for sprites, music, and even fonts.
