# tools

Build and content tooling for this project.

Empty as of v0.0. This folder is where scripts that support development go: content
generators, asset converters, validation checks, release helpers.

Two things deliberately live elsewhere:

- `Build.py` stays at the repository root. It resolves `Assets`, `ProjectSettings` and
  `docs` relative to the working directory, so it has to run from the project root.
- Editor-only C# tooling belongs in `Assets/Editor`, because Unity only compiles editor code
  from that folder. `Assets/Editor/BundleShaders.cs` and
  `Assets/Editor/UnityBuilderAction/BuildScript.cs` are the existing examples.

Anything that runs outside Unity, and outside the project root, belongs here.
