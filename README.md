# LoupedeckAtemControlerPlugin
A plugin for Loupedeck (CT) whcih connects directly to a Blackmagic ATEM Mini (Pro, Iso, ...) and is inteded to
run some basic operations.


## Build
TBD

## Configuration
TBD



## Set Still image
Create in the user folder of Loupedeck a plugin data directory e.g. under MacOS:
`/Users/USERNAME/Library/Application Support/Logi/LogiPluginService/PluginData/LoupedeckAtemControlerPlugin/still_images`
which contains various JPEG images in a size (e.g. 2k HD 1920x1080).
In the Loupdeck configuration GUI you can now assign the "Still Image Select" adjustment to a dial.
This loads from the `still_images` folder all JPEG images and displays those in the small LCD. You can now scroll with hte dial
through the images.
From the commands you can assign the command "Set Still Image" to a button - likely the press button function of the dial makes the most sense.
The idea is that you can select a still image for the ATEM and can upload it to the ATEM with a button press.
