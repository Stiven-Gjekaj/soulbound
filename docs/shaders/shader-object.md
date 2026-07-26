# The Shader object

### The Shader object [E/M/W]

The shader object is a Lua side object that can force a sprite object to display using a
certain shader, and manipulate said shader.

A new shader object is created whenever a sprite object is created, this also includes
bullets. In addition, there is a shader object created for the camera itself, to apply a
shader to the whole screen.

You can access the shader objects by using **shader** `sprite.shader` and **shader**
`Misc.ScreenShader`.

## Main functions and variables

### `shader.Set(string bundleName, string shaderName)`

Loads `shaderName` from the AssetBundle named `bundleName` in the `Shaders` folder.

The bundle is either the engine's default shaders bundle, a bundle you got from someone or
somewhere else, or a bundle you built using Unity.

To clarify, `shaderName` is the name of the `.shader` file within the bundle, **not** the
name defined within the shader itself. No extension should be provided.

Note: If the shader is unsupported on the user's graphics card, or the shader compiled with
an error, you will see an error message when loading your shader in game.

It is your responsibility as a modder to account for this. You should use the Lua function
`pcall` to safely load your shader:

```lua
-- Load "shaderName.shader" from the bundle "bundleName"
pcall(sprite.shader.Set, "bundleName", "shaderName")

if sprite.shader.isActive == true then
    -- Shader was successfully loaded
end
```

Alternatively:

```lua
-- Load "shaderName.shader" from the bundle "bundleName"
if pcall(sprite.shader.Set, "bundleName", "shaderName") then
    -- Shader was successfully loaded
else
    -- Shader was NOT successfully loaded
end
```

If the shader failed to load using this method, your sprite will still be using the default
shader, as if you called `shader.Revert()`.

**See [Shaders - Introduction](introduction.md) for setup instructions.**

### `shader.Test(string shaderName)`

Unity Editor only.

Loads the shader with the name `shaderName` and applies it to the sprite or screen. Highly
recommended to use while testing shaders before packaging them into AssetBundles.

This function will work the same as `shader.Set` otherwise, including the potential use of
`pcall` if you see fit. Just remember to replace this function with `shader.Set` when
you're done creating your mod.

You don't need to involve yourself with AssetBundles or directories this time, the name you
enter here is just the name of a shader, as defined in its first line, such as
`Shader "UI/Default"` becoming `"UI/Default"` for the argument `shaderName`.

### `shader.Revert()`

Reverts the sprite's shader to what it was originally. If no shader has been applied yet,
this function simply does nothing.

### **boolean** `shader.isactive` (readonly)

Returns `true` if a shader was successfully loaded through `shader.Set` or `shader.Test`,
and `false` if no shader has been applied yet, or the shader was reverted using
`shader.Revert`.

### `shader.SetWrapMode(string wrapMode, number sides = 0)`

Sets the wrap mode of this sprite's texture, or the screen. Only usable if a shader has
been applied.

This affects what happens when a shader manipulates the position of pixels, such as with
"wavey" effects. It has to do with what should be drawn in the holes outside the boundaries
of the original image.

`wrapMode` can be:

- `"clamp"`: The default wrap mode. When a shader manipulates the positions of pixels, the
  texture will be clamped to the last row or column of pixels of the source image.
- `"repeat"`: The source image is tiled infinitely.
- `"mirror"`: Similar to `"repeat"`, except all the repeated images will alternatingly be
  flipped horizontally or vertically.
- `"mirroronce"`: Similar to `"mirror"`, except only one mirror image gets created, and then
  the behavior of `"clamp"` is followed for all other areas.

`sides` can be:

- `0`: The new wrap mode is applied to both the horizontal and vertical edges of the image.
- `1`: The new wrap mode is applied to the horizontal edges of the image.
- `2`: The new wrap mode is applied to the vertical edges of the image.

You can have a different wrap mode for both the vertical and horizontal edges of the image
this way.

Note: You may find all three wrap modes, especially the default `"clamp"` mode, unappealing
and wish for something else. This is indeed possible, but it is handled on the shader side.
Some of the example shaders in [Shaders - Introduction](introduction.md) use keywords to
show transparency instead of clamping the texture to the last pixels.

See the paragraph on texture wrapping in [Coding a shader](coding-a-shader.md) for more
information.

## Shader property functions and variables

These all have to do with getting and setting properties within the shader file. It's pretty
much based on your knowledge of ShaderLab. See [Coding a shader](coding-a-shader.md) for
some helpful links and information on writing shaders.

For all "Set" functions listed below, if a variable with the given name does not exist on
the shader side, it will be created with the value you gave.

### **boolean** `shader.HasProperty(string name)`

Returns `true` if the active shader has a property with the name `name`, `false` otherwise.
Properties must be defined within the shader's `Properties` block to be persistent.

For all "Get" functions listed below, the function will either return the data it found, or
throw an error if the property does not exist on the shader side. Check if the property
exists first using this function.

Note: The non-persistent data types (see the section below) can not be defined in the
Properties block at the top of a shader file. They can be defined in the shader's `CPROGRAM`
code, but their data is likely to be lost whenever the window refreshes. Before that
happens, `HasProperty` will return true, and after that happens, `HasProperty` will return
false.

### `shader.EnableKeyword(string name)`, `shader.DisableKeyword(string name)`

Enables or disables a keyword named `name` within the shader script.

See the "keywords" section in [Coding a shader](coding-a-shader.md) for more information.

### **{table of number, number, number, number}** `shader.GetColor(string name)`, `shader.SetColor(string name, {table of number, number, number, number = 1} color)`

Gets or sets a color in the active shader, in the property named `name`.

Here, a color is a table of either 3 or 4 number values, each from `0.0` to `1.0`, following
the RGBA format. If no fourth argument is provided, `1.0` is given as the fourth argument
instead.

### **number** `shader.GetFloat(string name)`, `shader.SetFloat(string name, number float)`

Gets or sets a float in the active shader, in the property named `name`.

### **number** `shader.GetInt(string name)`, `shader.SetInt(string name, number int)`

Gets or sets an integer, a "whole number" with no decimal point, in the active shader, in
the property named `name`.

### `shader.SetTexture(string name, string texture)`

Sets a texture in the active shader, in the property named `name`.

This function loads an image in the same way as `CreateProjectile` or `CreateSprite`, it
searches for an image named `texture` in your mod's "Sprites" folder first, then the Default
folder's "Sprites" folder last.

*Note that there is no `shader.GetTexture`.*

### **{table of number, number, number, number}** `shader.GetVector(string name)`, `shader.SetVector(string name, {table of number, number, number, number} vector)`

Gets or sets a vector
([Vector4](https://docs.unity3d.com/2018.4/Documentation/ScriptReference/Vector4.html)) in
the active shader, in the property named `name`.

Here, a vector is a table of 4 number values, with any range.

Note: The two functions don't use the same kind of vectors. `shader.GetVector()` uses
`(w, x, y, z)` vectors, where `shader.SetVector()` uses `(x, y, z, w)` vectors. This
behavior will be corrected to only use `(x, y, z, w)` vectors in a later version.

## Non-persistent data

All of the data types presented below can not be initialized in a shader's `Properties`
block, which means that Unity will not treat them as persistent data, and they are likely to
be lost if the window gets re-loaded or re-drawn. Unity expects these data types to be set
in the shader on every frame, such as through `Update`. Use with caution.

### **{table of {table of number, number, number, number}}** `shader.GetColorArray(string name)`, `shader.SetColorArray(string name, {table of {table of number, number, number, number = 1}} colorArray)`

Gets or sets a color **array** in the active shader, in the property named `name`.

Just like with `shader.SetColor`, a color is a table of either 3 or 4 number values.
However, keep in mind that this is an *array* of colors. So, a table with multiple smaller
tables inside, each with 3 or 4 numbers.

### **{table of number}** `shader.GetFloatArray(string name)`, `shader.SetFloatArray(string name, {table of number} floatArray)`

Gets or sets a float **array** in the active shader, in the property named `name`.

### **{table of {table of number, number, number, number}}** `shader.GetVectorArray(string name)`, `shader.SetVectorArray(string name, {table of {table of number, number, number, number}} vectorArray)`

Gets or sets a vector **array** in the active shader, in the property named `name`.

Just like with `shader.SetVector`, a vector
([Vector4](https://docs.unity3d.com/2018.4/Documentation/ScriptReference/Vector4.html)) is a
table of 4 number values. However, keep in mind that this is an *array* of vectors. So, a
table with multiple smaller tables inside, each with 4 numbers.

Note: The two functions don't use the same kind of vectors. `shader.GetVectorArray()` uses
`(w, x, y, z)` vectors, where `shader.SetVectorArray()` uses `(x, y, z, w)` vectors. This
behavior will be corrected to only use `(x, y, z, w)` vectors in a later version.

## The Matrix object

There is a Unity property called the
[Matrix4x4](https://docs.unity3d.com/2018.4/Documentation/ScriptReference/Matrix4x4.html),
which can be used as a property within shaders. The Matrix object is a way to manipulate a
Matrix4x4 on the Lua side.

A Matrix object represents a 4x4 matrix filled with numbers. You'll create it by supplying 4
rows of 4 numbers, and after that, you can modify each individual number one at a time.

Note: The Matrix object is also a non-persistent data type, it can not be defined in the
shader's Properties block. See the previous section for more information.

### **matrix** `shader.Matrix({table of number, number, number, number} row1, {table of number, number, number, number} row2, {table of number, number, number, number} row3, {table of number, number, number, number} row4)`

Creates a new matrix object with its rows set to `row1`, `row2`, `row3`, and `row4`, in that
order, from top to bottom.

An easy way to visualize it while creating it is like this:

```lua
-- Create a simple numbered matrix
matrix = shader.Matrix( {  1,  2,  3,  4 },
                        {  5,  6,  7,  8 },
                        {  9, 10, 11, 12 },
                        { 13, 14, 15, 16 } )
```

After creation, you can get and set any values within the 4x4 range by using multi-indexing,
like so:

```lua
-- Check what number is in row 1, column 2
DEBUG(matrix[1, 2])
-- Set the number in row 3, column 4
matrix[3, 4] = 4.2
```

The matrix object is involved in the functions `shader.SetMatrix`, `shader.SetMatrixArray`,
`shader.GetMatrix`, and `shader.GetMatrixArray`.

### **matrix** `shader.GetMatrix(string name)`, `shader.SetMatrix(string name, matrix matrix)`

Gets or sets a Matrix4x4 in the active shader, in the property named `name`.
`shader.GetMatrix` will return a matrix object (see above), while `shader.SetMatrix`
requires one to be created and passed as the argument `matrix`.

Note: manipulating a matrix this way does **not** create a "link" between it and the shader
side. If you change values of the matrix object after retrieving it with `shader.GetMatrix`,
you will need to call `shader.SetMatrix` to update its values on the shader side. Likewise,
if you continue to change values of the matrix object passed to `shader.SetMatrix` after
calling the function, you will need to call it once again to update its values on the shader
side.

### **{table of matrix}** `shader.GetMatrixArray(string name)`, `shader.SetMatrixArray(string name, {table of matrix} matrixArray)`

Gets or sets a Matrix4x4 **array** in the active shader, in the property named `name`.

Just like with `shader.SetMatrix`, matrix objects are used (see above). This is an array of
them, so basically a table containing as many matrix objects as you like.
