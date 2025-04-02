namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    // This class implements an example command that counts button presses.
    public class SetStillImageCommand : PluginDynamicCommand
    {
        protected StillImageData _stillImageData;


        // Initializes the command class.
           public SetStillImageCommand(StillImageData stillImageData)
               : base(displayName: "Set Still Image", description: "Uploads the currently selected Image to the ATEM mini", groupName: "Commands")
           {
               this._stillImageData = stillImageData;
            
           }
        /*   
        public SetStillImageCommand()
            : base(displayName: "Set Still Image", description: "Uploads the currently selected Image to the ATEM mini", groupName: "Commands")
        {
            
        }
              */


        public void StillImageData(StillImageData stillImageData) => this._stillImageData = stillImageData;

        // This method is called when the user executes the command.
        protected override void RunCommand(String actionParameter)
        {
            //      this._counter++;
            //    this.ActionImageChanged(); // Notify the plugin service that the command display name and/or image has changed.
            //   PluginLog.Info($">>>> Sending Image !!! {this._stillImageData.ImagePath}"); // Write the current counter value to the log file.
            if (this._stillImageData != null)
            {
                PluginLog.Info($">>>> Sending Image !!! {this._stillImageData.ImagePath}");
            }
            else
            {
                PluginLog.Warning($"ERROR Not possible to send image "); // Write the current counter value to the log file.
            }
            
        }

        // This method is called when Loupedeck needs to show the command on the console or the UI.
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            $"Press Counter Sending Image";
    }
}
