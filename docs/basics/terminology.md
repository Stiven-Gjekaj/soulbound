# Terminology

`arena` - The inside of the white box in which the player is allowed to move.

`bullet` - Everything in a wave that can collide with you. Flowey's pellets would be
referred to as bullets, but so would Papyrus' bones, anything Woshua can shoot (of any
colour), even the dancing Migosp.

`encounter text` - The text that shows up before you've selected FIGHT/ACT/ITEM/MERCY.

`monster dialogue` - Text from monsters in an encounter, often seen before attacking. Can
also be multiple dialogue boxes for special encounters.

`dialog` - A user interface component that contains text. For example, the battle dialog
window. The distinction between "dialog" and "dialogue" is that "dialog" refers to
interface windows containing text, and "dialogue" refers to the speech content of
monsters.

`wave` - A single attack behaviour (or attack "wave", to say), measured from when you
start defending until when it stops. Vegetoid's bouncing vegetables attack would count as
a wave. Papyrus' special Cool Dude attack would also count as a wave. The engine works with
"wave scripts" for attacks; you can use multiple wave scripts at the same time for when
you have various monsters.

`modDev` - This is a feature specific to this engine. In the engine, you can access some extra
options from the boss select screen. These mainly include the options to wipe
`RealGlobals` and `AlMightyGlobals`, as well as toggle `retrocompatibilty mode`.

the modDev screen is located in an "options menu" accessible
by clicking "Options" from within the boss select screen.

`retrocompatibilty mode` - This feature is designed in such a way where, if
it's enabled, functions from 0.2.1a that were changed in the engine will function exactly as
they did in 0.2.1a.
