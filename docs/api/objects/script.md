# The Script object

### The Script object (and the `Encounter` variable) [varies]

Script objects are a bit of a special case. They're used to refer to other scripts that
were loaded by the engine itself.

In the encounter script, the `enemies` table is filled with Script objects after the
encounter starts, and the `Wave` table is filled with Script objects when entering
the state `DEFENDING`.

The variable `Encounter` is also a script object that refers to the current encounter, and
is **accessible from anywhere except from the Encounter itself**. *Do not enter the name
of your encounter file*, it will always just be `Encounter`. Case-sensitive.

- `script.GetVar(string variable_name)` - gets `variable_name` from the script.
- `script[string variable_name]` - same as the above.
- `script.SetVar(string variable_name, value)` - sets `variable_name` in the script.
- `script[string variable_name] = value` - same as the above.
- `script.Call(string function_name)` - runs `function_name` from within the target script.
- `script.Call(string function_name, argument)` - runs `function_name` from within the
  target script with one argument.
- `script.Call(string function_name, {table of any types} arguments)` - runs
  `function_name` from within the target script with all arguments in `arguments` as the
  arguments.

  So, for example, you can call an encounter function `TakeTwoArguments` with two
  arguments by using:

  ```lua
  Encounter.Call("TakeTwoArguments", {"argument 1!", "argument 2!"})
  ```

  and the two arguments given can be any variable types.
- **string** `script.scriptname` - this is equal to the file name of the script in
  question.

## Examples

Here are some quick examples:

```lua
-- This is the ENCOUNTER file, and I want to deactivate my first enemy from here
-- To do that, I'll call the function "SetActive" (see Misc. Functions) in the MONSTER file from here
enemies[1].Call("SetActive", false)
```

```lua
-- here is a simple way to change the encounter's "wavetimer" from another script, for instance,
-- a monster script if a player ACTs right, or just from a Wave that needs to last a certain time
Encounter.SetVar("wavetimer", 10)
-- alternatively:
Encounter["wavetimer"] = 10
```

```lua
-- let's say we're in the MONSTER file, and we want to turn a Sprite in the ENCOUNTER file to dust
-- we can do it in at least two different ways:
local spriteToDust = Encounter.GetVar("torso")
spriteToDust.Dust(true, true) -- see Sprites & Animation.
-- alternatively:
Encounter["torso"].Dust(true, true)
```
