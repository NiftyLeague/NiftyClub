# Nifty Club

## Project Structure
Project is divided into 4 folders. They can be described as follows:

1. **NiftyClubPlugins:** .NET project covering the plugins we develop
2. **NiftyClubServer:** Server folder where we place plugins (under **Plugins folder**) and server executable mostly used for local testing purposes
3. **NiftyClubServerCore:** Server folder where we place plugins (under **Plugins folder**) and server executable mostly used for remote testing purposes
4. **NiftyClubUnity:** Unity project

## How to Run Locally
By default, project is communicating with local instance (**127.0.0.1** on port **4296**) of **Dark Rift**. That's why you don't need to do any tweaks in the Unity Editor or builds to run it locally.

1. Open up **NiftyClubServer folder** in the repo root.
2. Run **DarkRift.Server.Console.exe executable** in that folder.
3. You can either run the project in the Unity Editor or generate builds and run them instead.

## How to Run Remotely
THe project can be used to connect remotely as well. In that case Core build should be hosted on some server.

1. Arrange a server and install required dependencies (to be expanded on)
2. Deploy **NiftyClubServerCore folder** into the server
3. Get server IP
4. Set correct IP and port settings in the project (Init scene: Dark Rift Networking > Unity Client).

### How to Setup Remote Server

1. Create a server instance
2. SSH into it
3. Follow Microsoft guidelines to install PowerSheell into your instance (e.g. https://docs.microsoft.com/en-us/powershell/scripting/install/install-ubuntu?view=powershell-7.2 for Ubuntu)
4. Install v3.1 of .Net into your instance (e.g. https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu for Ubuntu)
5. Start PowerShell: pwsh
6. Run the executable: ./Run.ps1
