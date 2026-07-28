# Controls

## In battle

`Arrow keys, Z (or Enter), X (or left/right Shift)` - The same as in Undertale.

`Esc` - Give up and return to the boss select screen. Disabled if the encounter has
`unescape` set to true.

`F9` - Toggle the debug console. You can write text to this with `DEBUG("your text here")`
in your scripts.

`F4, Alt+Enter` - Toggle fullscreen mode.
You can change how fullscreen displays with the option "cropped fullscreen" in
the Options menu, found in the boss select screen.

`H` with debugger open (`F9`) - Show or hide hitboxes of bullets and the player.

## Boss select screen (main)

`Z, Enter, or Mouse Click` - Fight the boss on screen.

`X or any Shift key` - Return to the disclaimer (title) screen.

`Up or C` - Open the boss list, used to jump straight to any boss. See below.

`Left or Right arrows` - Move to the previous or next boss.

## Boss select screen (boss list)

`Z, Enter, or Mouse Click` - Jump to the highlighted boss and close the list.

`X or any Shift key` - Close the list.

`Up and Down, Mouse Scroll, Click and Drag` - Move up and down through the list.

## Name entry

Type your name. There is no letter grid to walk any more.

`Any letter key` - Types. Letters only, up to nine of them.

`Backspace` - Deletes the last letter.

`Enter, or a mouse click` - Presses the highlighted button. `Left and Right` move between
`Quit`, `Backspace` and `Done`, and the mouse highlights whichever button it is over.

Typed characters are taken before anything else reads the keyboard, which is what makes
this work at all: `W`, `A`, `S` and `D` are bound to the four directions by default, so
without it a name with a `W` in it would move the cursor while spelling itself. For the
same reason `Z` and `X` type rather than acting as Confirm and Cancel on this screen only.

**On a controller there is no way to type**, so `Done` accepts an empty name and gives you
the default one rather than refusing to move on.
