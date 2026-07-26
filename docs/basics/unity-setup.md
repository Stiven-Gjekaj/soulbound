# Unity setup (optional)

This page covers how to set up the project in the Unity editor, which is necessary
for setting up shaders. It is also helpful for seeing the way encounters work from the
Unity side, and in particular how sprite layers work.

## Get started

The Unity project is the repository root, so clone the repository and open that folder as
a project in Unity.

The engine targets Unity Personal 2018.4.36f1, also known as Unity 2018's Long Term
Support version, which you can find in the
[Unity version archive](https://unity3d.com/get-unity/download/archive). Use that exact
version.

Unity may take a lot of time to install and to load the project for the first time, due
to a lot of resources being registered by Unity, but you won't have to do it again in the
future.

## Once you're in

Once you're in, you'll have to load the scene `Assets/Scenes/Disclaimer.unity` using the
command `File` => `Open Scene` at the top left corner of Unity's window. You should
always start play mode from this scene.

In order to set up the viewport properly, first switch to the "Game" tab (`Window` =>
`General` => `Game`, or click it in the editor). Within the Game tab, find the button
labelled `Display 1`. Next to it should be another button, most likely labelled
`Free Aspect`. Click on it, click the "plus" sign at the bottom of the list that appears,
and add an option with a fixed resolution of `640` x `480`.

*This is the engine's native resolution, and viewing it like this ensures that
everything you see in the Unity editor will look exactly the same in a build.*

## Where is everything?

Look in the folder `Assets`. In here, the folder named `Default` is the same as the
`Default` folder in a build. Likewise, `Mods` is the same as the `Mods` folder in a build.
Load all of your mods here if you want them playable in the editor.

Another folder here is `Scenes`. It holds the engine's scenes: the disclaimer, the
intro, the title screen, name entry, the mod selector, the options and keybinding
screens, the battle, and the error screen.

Also within `Assets` the folder `Editor` is used for shaders. It contains a folder
`Shaders`, which is where all of your original shader files go, and another folder
`Output`, which is where your shaders will be compiled into AssetBundles. Read
[Shaders - Introduction](../shaders/introduction.md) for more information.

## Building locally

This is an optional step, for if you want or need to export the game to a built
executable. At the repository root is a python script named `Build.py`. It uses
[Python v3.7.4](https://www.python.org/downloads/release/python-374/). This is the script
used to build executables locally. You can optionally install
[7-zip](https://www.7-zip.org/) as well, if you want the script to automatically package
your builds into archives.

You should first peek inside the script and edit it as needed, you only need change one
or two lines. You can change the variable `CYFversion` to dictate the names of the
created folders, or edit them yourself in `buildTargets` and `macTarget`. Next, check the
variable `unityPath`, you need to set it to the path of your machine's Unity
installation, if it is not already set to it.

Make sure this project is the last one opened in Unity. Then, close Unity before running
the script.

Actually running the script also has some documentation at the top of the file itself.
You can either run the script directly (double click it) or run it from the command line
(varies depending on your operating system). If you do the former option, or do the
second option with no extra setup, the script will attempt to create all 5 builds, and
archive them if possible.

If running from the command line, you may provide two arguments to specify the build
process:

- `--nozip`: Does not attempt to package the resulting builds into archives with 7-zip.
- `--single [id]`: Only creates one of the possible builds. `id` should be from 1 to 5:
  1. Windows 32-bit
  2. Windows 64-bit
  3. Linux 32-bit
  4. Linux 64-bit
  5. Mac

Your builds will appear in a folder named `bin` at the repository root.
