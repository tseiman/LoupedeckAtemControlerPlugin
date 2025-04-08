namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;
    using log4net.Plugin;

    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    // This class implements an example command that counts button presses.
    public class SetStillImageCommand : PluginDynamicCommand
    {
        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;

        private  AtemCommandUploadStill _atemCommandUploadStill;
        private AtemCommandSetStillMedia _atemCommandSetStillMedia;

        private const UInt32 MEDIA_SLOT = 19;


        // Initializes the command class.
        public SetStillImageCommand()
               : base(groupName: "Still Image Selection", displayName: "Set Still Image", description: "Uploads the currently selected Image to the ATEM mini")
        {
       //     this.IsWidget = true;
            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;
        }

        private void OnPluginReady()
        {
            // Now it's safe to initialize anything plugin-dependent

            PluginLog.Warning($"[SetStillImageCommand] OnPluginReady");

            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);


            this._atemCommandUploadStill = new();
           // this._plugin.initAtemCommand(this._atemCommandUploadStill);


            this._atemCommandSetStillMedia = new();
           // this._plugin.initAtemCommand(this._atemCommandSetStillMedia);


        }







        // This method is called when the user executes the command.
        protected override void RunCommand(String actionParameter)
        {
            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);

            if (stillImageData != null)
            {

                Task.Run(async () =>
                {
                    try
                    {
                        await this._atemCommandUploadStill.SetStillImageAsync(stillImageData.ActualFullImagePath, MEDIA_SLOT);
                    }
                    catch (Exception e)
                    {
                        PluginLog.Warning($"[SetStillImageCommand] ERROR when sendign image {e} ");
                    }
                });


            }
            else
            {
                PluginLog.Warning($"ERROR Not possible to send image "); // Write the current counter value to the log file.
            }


            this._atemCommandSetStillMedia.SetStillImageMediaToPlayer(MEDIA_SLOT);

        }

        // This method is called when Loupedeck needs to show the command on the console or the UI.
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            $"Press Counter Sending Image";
    }
}
