# LoupedeckAtemControlerPlugin
A plugin for Loupedeck (CT) whcih connects directly to a Blackmagic ATEM Mini (Pro, Iso, ...) and is inteded to
run some basic operations:

- Set Still Image
- execute Macro

## Build
This is a visual studio project and can be build with it.
Packing the plugin: TBD

## Installation
currently the packing doesn't work - I'm copying the entire folder /bin/Debug/* folder to
`/Users/USERNAME/Library/Application Support/Logi/LogiPluginService/Plugins/LoupedeckAtemControlerPlugin/mac/`
The content of `metadata` folder has to go to 
`/Users/USERNAME/Library/Application Support/Logi/LogiPluginService/Plugins/LoupedeckAtemControlerPlugin/metadata/`

## Configuration
TBD (to be implemented)



## Set Still image
You need a folder where to sore the still images for the ATEM still function and which contains various JPEG images in a size (e.g. 2k HD 1920x1080).
In the Loupdeck configuration GUI you can now assign the "Still Image Select" adjustment to a dial - this has as well a configuration "Enter Folder to find JPEG Images" -
place here the (absolute) path to the folder with the still images.
Now on turning the dial it loads from the folder all JPEG images and displays those in the small LCD. You can now scroll with the dial
through the images. If content in the folder is changed it will automatically update the list which can be selected by the dial.
From the commands you can assign the command "Set Still Image" to a button - likely the press button function of the dial makes the most sense.
The idea is that you can select a still image for the ATEM and can upload it to the ATEM with a button press.


# Execute Macro
TBD (to be implemented)