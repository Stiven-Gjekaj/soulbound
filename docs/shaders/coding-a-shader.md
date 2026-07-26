# Coding a shader

The purpose of this page is to go over everything necessary to know about actually coding
the `.shader` files that can be made into shader AssetBundles for Create Your Frisk v0.6.5
and higher. You will need to code at least a little in Unity ShaderLabs, so some helpful
links are provided right at the top for you to look at.

Please read [Shaders - Introduction](introduction.md) before continuing.

## Getting started with shaders

The first thing you should know about shaders is that most of the process is not related to
Create Your Frisk. It almost solely depends on your own ability to code in Unity's
ShaderLabs format. There are some sample shaders provided that you may look at for
reference (see [Shaders - Introduction](introduction.md)), but ultimately the deciding
factor is your own skill and knowledge.

The helpful links below definitely will come in handy, especially as reference material,
but it is not guaranteed you will be able to code your own shader as easily as you can code
in CYF.

The basics:

- [Writing Shaders](https://docs.unity3d.com/2018.4/Documentation/Manual/ShadersOverview.html) -
  Basic descriptions and terminology to help you begin
- [ShaderLab Syntax](https://docs.unity3d.com/2018.4/Documentation/Manual/SL-Shader.html) -
  Basic overview of the full shader file's syntax
- [ShaderLab: Properties](https://docs.unity3d.com/2018.4/Documentation/Manual/SL-Properties.html) -
  How to set variables up in the Properties block at the top of the file
- [ShaderLab: Access shader properties in Cg/HLSL](https://docs.unity3d.com/2018.4/Documentation/Manual/SL-PropertiesInPrograms.html) -
  How to access said variables later in the file

Bit more advanced, but still recommended:

- [Built-in shader helper functions](https://docs.unity3d.com/2018.4/Documentation/Manual/SL-BuiltinFunctions.html) -
  Helper functions you can use within your shader scripts
- [Built-in shader variables](https://docs.unity3d.com/2018.4/Documentation/Manual/SL-UnityShaderVariables.html) -
  Built in variables you can use. `_Time`, `_SinTime` and `_CosTime` are highly recommended
  for performance.
- [Making multiple shader program variants](https://docs.unity3d.com/2018.4/Documentation/Manual/SL-MultipleProgramVariants.html) -
  Covers setting up keywords in your shader script (see below)

Other links:

- [Cg Standard Library Documentation](https://developer.download.nvidia.com/cg/index_stdlib.html) -
  A list of functions you can use within your shader's `CGPROGRAM` that are not necessarily
  in the Unity documentation
- [Cg Programming](https://en.wikibooks.org/wiki/Cg_Programming) - Some additional guides
  and tutorials on writing the `CGPROGRAM`

## Properties of CYF shaders

### Name

The shader's name is defined at the very top of the script. You will see something like
this:

```
Shader "CYF/ScreenTest"
{
```

The name is what is between quotation marks here. This is also the same name you will
provide if you are using `shader.Test` within the editor. This name must be unique if you
want to use `shader.Test` with no issues. For that reason, it is good practice to start
your shader's name with `CYF/`, or some other identifier.

### Keywords

Because of Create Your Frisk's Unity version (2018.4), the concept of global and local
keywords does not exist, as they were only added in a future Unity version. Make sure you
read
[this page](https://docs.unity3d.com/2018.4/Documentation/Manual/SL-MultipleProgramVariants.html),
which applies to our version of Unity.

Keywords can be enabled and disabled from the Lua side using `shader.EnableKeyword` and
`shader.DisableKeyword`. The proper syntax for defining a shader from the shader script side
is:

```
#pragma compile_mode KEYWORD_DISABLED KEYWORD_ENABLED
```

but this would require you to type `shader.EnableKeyword("KEYWORD_ENABLED")` and
`shader.DisableKeyword("KEYWORD_DISABLED")`. To remove the confusion, try something like
this:

```
#pragma multi_compile __ NO_WRAP
```

which would use `shader.EnableKeyword("NO_WRAP")` and `shader.DisableKeyword("NO_WRAP")`.

You may also check out the source code for the sample shaders included with CYF if you
would like more examples.

### Check if shader is on the camera or a sprite

Shaders can be applied to both the screen camera (using `Misc.ScreenShader`) and sprite
objects (using `sprite.shader`). It is not unusual to want your shader to function
differently depending on which one of these it is applied to.

The keyword `CYF_SHADER_IS_CAMERA` is automatically applied whenever a shader object is
applied to the camera rather than a script. You may check for this within your shader script
like so:

```
#ifdef CYF_SHADER_IS_CAMERA
```

to run code specifically if it is on the camera.

Note that you do not have to define it using `#pragma` first.

### Sprite masking

Sprite masking is the CYF feature controlled by `sprite.Mask`. The default mode is "off",
but depending on its other 5 values, it behaves differently. Here is how to make your shader
compatible with sprite masking.

First thing to note: When in "box" mode, the keyword `UNITY_UI_CLIP_RECT` will be enabled,
it is disabled otherwise.

And when in the "sprite", "stencil", "invertedsprite" or "invertedstencil" modes, the
keyword `UNITY_UI_ALPHACLIP` will be enabled.

Within the shader template, both of these keywords are defined through `#pragma`, and have
their own code in the fragment shader at the bottom of the script.

Finally, all of the code related to "stencils" and the "color mask" variable are also needed
for sprite masking compatibility, the properties are defined at about two places each.

By including all of this code in your shader, you will maintain compatibility with CYF
sprite shaders.

Additionally: When in the mask modes "invertedsprite" and "invertedstencil", the keyword
`CYF_INVERTED_MASK` will be enabled on **children** of the inverted parent. In these modes,
the child's stencil properties are exactly the same as they would be in "sprite" and
"stencil", except that the stencil comparison (`_StencilComp`) is set to 6 (not equal).

### Little warning on the Properties block

This is something standard to Unity shaders, but it still very much warrants a mention here.
See the "Properties" block at the top of the example shaders? One might assume it's useless,
because it defines properties for the shader to be edited in the Unity editor. But it
actually serves another purpose: It's a list of "important" variables to store in the
shader. In other words, every property you want to persist in the shader should be set here.
Any variables that are not set up here are likely to be **lost** when the window refreshes.

In addition, here is something else to consider. There are a certain list of shader object
properties, listed under "non-persistent data" on
[The Shader object](shader-object.md) page, that actually *can not be defined* in the
shader's Properties block. All examples in the Unity documentation that use them show them
being applied *on every frame*, meaning in `Update`. So, if you use any of the properties:

- `shader.SetColorArray`
- `shader.SetFloatArray`
- `shader.SetVectorArray`
- `shader.SetMatrix`
- `shader.SetMatrixArray`

be sure you are applying them every frame within `Update` on the Lua side, or their values
may be lost suddenly.

## Recommended enhancements

### Pixel snap

Create Your Frisk is designed with the intent to output its screen image in a pixelated
format, obviously comparable to Undertale itself. However, shaders are not guaranteed to be
precise, and can often result in non-integer coordinates of pixels. As a result, you may see
"blurriness" output to the screen with some shaders, and you may wish to avoid that.

Assuming that the float2 `offset` is your coordinates AFTER modification, and that you have
added a `_MainTex_TexelSize` property alongside your `_MainTex` property, simply use

```
offset.x = (floor(offset.x * _MainTex_TexelSize.z) + 0.5) / _MainTex_TexelSize.z;
offset.y = (floor(offset.y * _MainTex_TexelSize.w) + 0.5) / _MainTex_TexelSize.w;
```

to snap the coordinates to pixels, to avoid the texture being smoothed out.

### Controlling texture wrapping

Let's say you have a fragment shader that is changing UVs, moving pixels on screen.
Something like the "Wave" sample shader, for instance. Whenever you move the whole source
texture out of its normal boundaries, a void is left. Depending on the value of
`shader.SetWrapMode`, it will look different, most commonly "stretching" the last pixel of
the texture very long.

But what if you don't want that? Well, you can put some code in your fragment shader to
force pixels to not be drawn if they are outside of the original texture that you moved.

The float2 `offset` here represents the coordinates of a pixel AFTER modification in your
fragment shader `IN.uv`, compare to `IN.uv` in the template shader below:

```
(offset.x < 0 || offset.x > 1) || (offset.y < 0 || offset.y > 1) ? 0 : col.a;
```

You can find examples in the sample shaders "Displacement", "Rotation", "ScreenScale", or
"Wave". As a side note, in the "Wave" shader, pixels are only ever moved horizontally and
not vertically, so the check for `y` is omitted there.

## Template shader

Here is a template shader for you to edit to make your own. It provides compatibility with
sprite masking (see above), but does not necessarily make use of anything special, it being
a template to build off of. It is based on the Unity default shader, UI/Default.

```
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "CYF/Template"
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}

        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255

        _ColorMask("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest[unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask[_ColorMask]

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            fixed4 _TextureSampleAdd;
            float4 _ClipRect;
            float4 _MainTex_ST;

            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half4 color = (tex2D(_MainTex, IN.uv) + _TextureSampleAdd) * IN.color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }
        ENDCG
        }
    }
}
```
