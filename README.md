# LoupedeckAtemControlerPlugin
A plugin for Loupedeck (CT) whcih connects directly to a Blackmagic ATEM Mini (Pro, Iso, ...) and is inteded to
run some basic operations:

- [x] Set Still Image
- [ ] execute Macro
- [x] Toggle preview transition

## Build
This is a visual studio project and can be build with it.

### Building without Visual Studio

The project can also be built from a terminal with the .NET SDK.

Prerequisites:

- .NET 8 SDK
- Logi Options+ or Loupedeck Software installed, including Logi Plugin Service
- `PluginApi.dll` from the Logi Plugin Service installation
- NuGet access for restoring packages

On macOS the project expects the Logi Plugin Service files here:

```sh
/Applications/Utilities/LogiPluginService.app/Contents/MonoBundle/
```

On Windows the project expects them here:

```bat
C:\Program Files\Logi\LogiPluginService\
```

Build the plugin project from the repository root:

```sh
dotnet restore src/LoupedeckAtemControlerPlugin/LoupedeckAtemControlerPlugin.csproj
dotnet build src/LoupedeckAtemControlerPlugin/LoupedeckAtemControlerPlugin.csproj --configuration Debug
```

The build creates the normal plugin output under `bin/Debug` and writes a `.link` file into the Logi Plugin Service plugins directory. Restart the Logi Plugin Service or the Loupedeck software after building.

Building on Linux is useful for code checks only. The official Logi/Loupedeck Plugin SDK and Logi Plugin Service are available for Windows/macOS, so a full plugin build and runtime test needs one of those systems with `PluginApi.dll` installed.

Packing the plugin: TBD

## Installation
The build automatically generates a ".link" (see Logi SDK documentation) in the plugins folder of Loupedeck. On a restart of Loupedeck
Software the Plugin is started.

## Configuration
The configuration of the plugin contains the following parameters:
1. the IP address or the hostname of the ATEM - this can be set wit hthe Dummy Action "Set ATEM URI" which needs to be saved with a name and which needs
to be set to a button whcih needs to be triggered one time. I put that on a second Touch Page.
2. The folder of the still images is a parameter of the "Still Image Select" action.
All neccesary parameters are then saved to the plugin_settings automatically.

## Set Still image
You need a folder where to sore the still images for the ATEM still function and which contains various JPEG images in a size (e.g. 2k HD 1920x1080).
In the Loupdeck configuration GUI you can now assign the "Still Image Select" adjustment to a dial - this has as well a configuration "Enter Folder to find JPEG Images" -
place here the (absolute) path to the folder with the still images.
Now on turning the dial it loads from the folder all JPEG images and displays those in the small LCD. You can now scroll with the dial
through the images. If content in the folder is changed it will automatically update the list which can be selected by the dial.
From the commands you can assign the command "Set Still Image" to a button - likely the press button function of the dial makes the most sense.
The idea is that you can select a still image for the ATEM and can upload it to the ATEM with a button press.

## Execute Macro
TBD (to be implemented)

## Toggle preview transition 
Just place the Command "Toggle Preview Transition" on the Loupedeck
