<div align="center">
  <a href="README.md"><b>Soulbound</b></a>
</div>

# Getting help

Need help with Soulbound? Here is where to look.

## Learn the engine

- Start with the [documentation index](docs/README.md), which covers the whole
  engine API.
- New to modding this engine? Read
  [docs/basics/basic-setup.md](docs/basics/basic-setup.md) for how the engine
  reads a mod folder, then
  [docs/basics/special-variables.md](docs/basics/special-variables.md) for the
  variables it reads out of your scripts.
- To set up Unity, see [docs/basics/unity-setup.md](docs/basics/unity-setup.md).
- To see how the engine is built, read
  [docs/project/engine-architecture.md](docs/project/engine-architecture.md).

## Build problems

- Version, build script, and CI details are in
  [docs/project/building.md](docs/project/building.md).
- If CI fails at the `game-ci/unity-builder` step with "Missing Unity License
  File and no Serial was found", the repository is missing its Unity secrets.
  That is configuration, not a code fault. The licensing section of
  [docs/project/building.md](docs/project/building.md) explains what to add.

## Ask a question or report a problem

- Search the existing
  [issues](https://github.com/Stiven-Gjekaj/soulbound/issues) first, in case
  someone has already asked.
- If you found a bug, open a bug report.
- If you want a feature, open a feature request.

Please do not use the issue tracker for security problems. See
[SECURITY.md](SECURITY.md) for how to report those privately.

## Upstream engine

Soulbound is built on [Create Your Frisk](https://github.com/RhenaudTheLukark/CreateYourFrisk).
Questions about engine behavior that Soulbound has not changed are often best
answered by the upstream project and its community.

## Contributing

If you would like to help improve Soulbound, see
[CONTRIBUTING.md](CONTRIBUTING.md).
