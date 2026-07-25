# How to read this documentation

## Reading function examples

Let's say you see something in the documentation that looks like this:

```
Screen.DispImg(string path, number ID, number posX, number posY, number toneR = 255, number toneG = 255, number toneB = 255, number toneA = 255)
```

There are several things to know here. First of all, the arguments have a type and a
name. The name is there just to help you know what the variable is, you don't really
have to care about it. However, the type is important, it determines what kind of
variable you'll need to send.

In this example, **string** is a character chain (`"hello"`), and **number** is any
number (`4`).

One last thing with functions: some of these arguments have a value after them, which is
the variable's *default value*. So you can use the function *without* including that
argument.

In our example, that means you can just call `Screen.DispImg("poseur", 1, 320, 240)`,
but you can add the other values if you want to.

However, if you need to set `toneB` in our example, you need to set `toneR` and `toneG`
before it as well. If you want to keep the default values, just enter the ones the
documentation says.

So if we want to fully call this function and set `toneA` to 128, we have to call it
like this:

```lua
Screen.DispImg("poseur", 1, 320, 240, 255, 255, 255, 128)
```

## E, M and W

All over this documentation, you will find `[E]`, `[M]`, `[W]`, or a mix of the three:

```
<CYF> boolean isCYF [E/M/W]
```

These simply mean that the relevant variable, function or object is accessible from:

- `[E]`: `Encounter` scripts
- `[M]`: `Monster` scripts
- `[W]`: `Wave` scripts

## Version markers

If you're a fan of Unitale and want to use it instead of CYF but still want to use this
documentation, note this: all the variables or functions tagged `<CYF>` are only usable
in CYF.

Additionally, this symbol indicates new or changed content in the most recent major
version of Create Your Frisk, and its sub-versions.
