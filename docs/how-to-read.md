# How to read this documentation

## Reading function examples

Let's say you see something in the documentation that looks like this:

```
Arena.Resize(number width, number height, boolean immediate = false)
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

In our example, that means you can just call `Arena.Resize(155, 130)`, but you can add
the other value if you want to.

Where a function has several optional arguments, you can only skip the ones at the end.
To set a later argument you must also pass the ones before it. If you want to keep their
default values, just enter the ones the documentation says.

So to call this function and set `immediate` to true, we write:

```lua
Arena.Resize(155, 130, true)
```

## E, M and W

All over this documentation, you will find `[E]`, `[M]`, `[W]`, or a mix of the three:

```
boolean unescape [E/M/W]
```

These simply mean that the relevant variable, function or object is accessible from:

- `[E]`: `Encounter` scripts
- `[M]`: `Monster` scripts
- `[W]`: `Wave` scripts
