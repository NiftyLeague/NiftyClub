# Nifty Club

## Project structure

1. **NiftyClubPlugins** – the .NET project containing the server plugins.
2. **NiftyClubServer** – the Windows local-test server distribution.
3. **NiftyClubServerCore** – the cross-platform server distribution.
4. **NiftyClubUnity** – the Unity client project.

## Prerequisites

- Unity **6000.3.20f1**, the newest Unity editor currently installed for this
  project.
- A current .NET SDK to build `NiftyClubPlugins`. The project targets
  `netstandard2.0` for compatibility with the bundled DarkRift 2.10.1 server
  libraries.
- A current .NET runtime to run the bundled Core server. PowerShell 7 (`pwsh`)
  is optional; `Run.sh` is provided for macOS and Linux.

The Unity project uses the Unity 6 package set recorded in
`NiftyClubUnity/Packages/manifest.json` and `packages-lock.json`. The obsolete
Unity VS Code package is intentionally not included; Unity's Visual Studio IDE
package is the supported route for VS Code integration in this editor
generation. The bundled Odin Inspector and SRDebugger assets are retained as
legacy files for now, but the application does not depend on either paid or
restricted feature: Odin's inspector-only attributes use Unity's built-in
attributes, and SRDebugger is disabled with the reversible
`DISABLE_SRDEBUGGER` project define. Use Unity's built-in Console/Profiler or
the configured Unity MCP relay for debugging.

## Build the server plugins

From the repository root:

```sh
dotnet restore NiftyClubPlugins/NiftyClubPlugins.csproj
dotnet build NiftyClubPlugins/NiftyClubPlugins.csproj
```

The build copies `NiftyClubPlugins.dll` into both server distributions' `Plugins`
folders.

## Run locally

The Unity client defaults to `127.0.0.1:4296`.

On macOS or Linux, after building the plugins:

```sh
cd NiftyClubServerCore
./Run.sh
```

Both launchers enable .NET major-version roll-forward so the bundled .NET Core
3.1 DarkRift server can run with a current installed .NET runtime. The server
was verified on this machine with .NET 9 and loaded RoomSyncPlugin,
PlayerSyncPlugin, and ChatSyncPlugin before listening on port 4296.

On Windows, the legacy local distribution can be started from
`NiftyClubServer` with `DarkRift.Server.Console.exe`.

With the server running, open `NiftyClubUnity` in Unity 6000.3.20f1 and press
Play, or create a client build.

## Run on a remote host

1. Build the plugins and deploy `NiftyClubServerCore` to the host.
2. Install a current .NET runtime on the host.
3. Start the server with `./Run.sh` (or `pwsh ./Run.ps1`) from
   `NiftyClubServerCore`.
4. Update the server address and port in the Init scene under
   `Dark Rift Networking > Unity Client` before building the client.

The bundled server binary is still DarkRift 2.10.1, but the current upstream
DarkRift 2 project is open source and free of CCU limits. This migration keeps
the existing DarkRift protocol and plugins intact instead of introducing a
networking rewrite.
