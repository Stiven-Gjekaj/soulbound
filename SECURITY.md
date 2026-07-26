<div align="center">
  <a href="README.md"><b>Soulbound</b></a>
</div>

# Security Policy

## Supported versions

Soulbound is an early-stage project. Security fixes are applied to the latest
state of the default branch. Older versions are not maintained.

## Reporting a vulnerability

Please report security issues privately, not through public issues.

- Preferred: open a private security advisory with the "Report a vulnerability"
  button on the repository's Security tab.
- Alternatively, email the maintainer at stivenagostingjekaj@gmail.com.

Please include steps to reproduce, the affected version or commit, and the
impact as you understand it. You can expect an initial response within a few
days. Once a fix is ready it will be released, and your report will be
acknowledged unless you prefer to remain anonymous.

## Scope

Soulbound runs on Create Your Frisk, which executes mod Lua scripts with the full
trust of your user account. It is a game engine, not a sandbox: a mod can do
anything the game process can do, including reading and writing files through the
`Misc` object's file functions.

Do not run untrusted mods expecting isolation. Reports about the lack of mod
sandboxing are out of scope, since that is a known and documented property of the
engine rather than a vulnerability.

Vulnerabilities in the upstream engine itself are best reported to
[Create Your Frisk](https://github.com/RhenaudTheLukark/CreateYourFrisk), though
we are glad to know about them too so we can carry a fix.
