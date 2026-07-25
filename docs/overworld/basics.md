# `<CYF>` Overworld basics

## Presentation

Create Your Frisk is different from Unitale because of its numerous new features, but the
overworld is the key feature that everyone was waiting to use for a long time.

The overworld feature includes an efficient event handler with a lot of functions that
you'll be able to use to create your own Undertale-like fangame.

However, to be able to use the overworld part of CYF, you will need to have at least some
experience with Unity and you'll need to know at least some of the Game Objects' mechanics.

## Get started

The full instructions for setting up Unity for Create Your Frisk are in
[Unity setup](../basics/unity-setup.md). Please read and follow them before you continue.

Once you've completed the setup mentioned above, you're ready. You can launch CYF and start
fiddling with the engine.

## How does it work?

Some software will have to be used to create an overworld in CYF. First of all, you can use
a software called [Tiled](http://www.mapeditor.org/) that allows you to create maps out of
tilesets, images that contain tiles used to fill in maps. This tiling system is absent from
Unity, though, but it's easier to use to create a map, so being able to translate a Tiled
map to a Unity scene thanks to [Tiled2Unity](http://www.seanba.com/tiled2unity) would be a
great idea.

Well, now we're going to go deep into the details. If you still haven't learned Unity's
basics, you should research those beforehand, as some technical language may be used here.

There are mostly two things to look at when making a map: the map itself (how the map will
be made) and the events (how the map will work). Let's delve into the more complex stuff
now.

As said above, each map is converted into a scene with Tiled2Unity. This scene will have to
contain at least one element, which is the map object called `Background`. To see more
about how to create a map, look at the tutorial on
[How to create a map](how-to-create-a-map.md).

Now, the events. These are universal and used the same way, with the exception of one
"add-on" that is a `CYFAnimator` component. The events have different function-calling
conditions, such as pressing the confirm button to call it, colliding with it, it being
called automatically, or the function being called as a parallel process. The functions
themselves have to be created in Lua scripts, like battles. To see more about how to use
the events, check out [How to create an event](how-to-create-an-event.md).

Of course, there are other things to see, but these are the two main parts of CYF's
overworld coding. Now, let's look at the functions that will let you do whatever you want
with the overworld.

## The event code

For a long time, events' code was accessible and could only be modified by going into
`YOURMOD/Lua/Events/`. Now that the system is stable and easier to use, event programming
may be as easy as creating a battle: all that you have to do is to follow the documentation
and know what you're doing.

## Shared functions and objects

The overworld has a debugger like battles', and it's used the same way:

### `DEBUG(string value)`

Prints one string value to the Overworld debugger.

### `<CYF>` `EnableDebugger(boolean bool)` [E/M/W]

Forcefully disables or re-enables the debugger. See this function in
[Misc. functions](../api/objects/misc-functions.md) for more information and specifics.

The `Audio`, `NewAudio`, `Inventory`, `Input`, `Misc` and `Time` objects from battles are
usable in the overworld too, as well as `RealGlobals` and `AlMightyGlobals`.

`CreateSprite` and `CreateText` all work exactly the same, except that the default value for
the "layer" argument is `"Default"` instead of `"BelowArena"` or `"BelowPlayer"`.
`CreateLayer` also works exactly the same as before.

Along with text objects being accessible, their [Game event](../api/game-events.md)
`OnTextAdvance` is usable here too. When a text object advances in the Overworld, it will
call this function in the event script that created it.

## Shared variables

### **boolean** `isCYF`

A value that indicates if you are using CYF. Because the Overworld is a feature exclusive to
CYF, this will always be true.

### **boolean** `safe`

Indicates whether CYF's "safe mode" has been enabled from the modDev screen.

### **boolean** `windows`

This value indicates whether the user is playing on Windows.

### **string** `CYFversion` [E/M/W]

Returns a different string based on the version of CYF you are using.

- `Versions before v0.6`: Previous version's number. For example, in CYF v0.5.5, this will
  be "0.5.4".
- `Versions between v0.6 and v0.6.1.2`: always "1.0".
- `Versions after v0.6.1.2`: Current version's number. For example, in CYF v0.6.2, this will
  be "0.6.2".

Tip: Lua has a very useful built-in string comparing function. You can very easily check for
`if CYFversion < "0.6.2.2" then`, `if CYFversion >= "0.6.1.2" then` and other combinations.

### **boolean** `playerskipdocommand`

False by default. If this value is set to true, text commands will be called even if the
player skips the text, except for `[w]` and `[letters]` commands, and commands with the tag
"`skipover`".
