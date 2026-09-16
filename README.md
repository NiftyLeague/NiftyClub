# NiftyClub — Wink

Unity + DarkRift networking project. Licensed under Apache-2.0.

## Excluded commercial plugins

These products are **not** redistributed in this repository because their
licenses prohibit public redistribution:

- `NiftyClubUnity/Assets/Plugins/Sirenix/` — Odin Inspector (Sirenix)
- `NiftyClubUnity/Assets/Free Lives Commonwealth/` — InControl (Gallant
  Games / Free Lives)
- `NiftyClubUnity/Assets/StompyRobot/SRDebugger/` — SRDebugger (Stompy
  Robot). The sibling `SRF` framework is MIT-licensed and remains.
- `NiftyClubUnity/Assets/DarkRift/` and the DarkRift binaries under
  `NiftyClubServer*/` and `NiftyClubPlugins/bin/` — DarkRift Networking,
  including the Pro source archive.

Networking code (`using DarkRift.*`) will not compile until DarkRift is
re-added; restore it from the original vendor. Scenes/prefabs may show
missing-script warnings until Odin and SRDebugger are re-added.
