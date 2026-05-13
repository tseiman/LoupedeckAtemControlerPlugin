namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;
    using System.IO;
    using log4net.Plugin;

    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    public class SetStillImageCommand : PluginDynamicCommand
    {
        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;

        private AtemCommandUploadStill _atemCommandUploadStill;
        private AtemCommandSetStillMedia _atemCommandSetStillMedia;

        private const UInt32 MEDIA_SLOT = 19;


        public SetStillImageCommand()
               : base(groupName: "Still Image Selection", displayName: "Set Still Image", description: "Uploads the currently selected Image to the ATEM mini")
        {
            this.IsWidget = true;
            this.GroupName = "Still Image Selection";
            this.DisplayName = "Set Still Image";
            this.Description = "Uploads the currently selected Image to the ATEM mini";
            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;
        }


        private void OnPluginReady()
        {
            PluginLog.Warning($"[SetStillImageCommand] OnPluginReady");

            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);

            this._atemCommandUploadStill = new();
            this._atemCommandSetStillMedia = new();

            // Redraw when ATEM connects / disconnects.
            AtemVisuals.RegisterConnectionRefresh(this.OnImageSelectionChanged);

            // Redraw whenever ImageScrollAdjustment selects a different image,
            // so the MultiWheel command button preview stays in sync.
            StillImageChangedEvent.OnChanged += this.OnImageSelectionChanged;
        }


        // Called both by the ATEM connection-refresh and by StillImageChangedEvent.
        private void OnImageSelectionChanged()
        {
            // ActionImageChanged() with no argument tells the Plugin Service to
            // re-call GetCommandImage() for every currently visible instance of
            // this command, which covers the MultiWheel central display and any
            // touch buttons showing the same command.
            this.ActionImageChanged();
        }


        protected override void RunCommand(String actionParameter)
        {
            if (!AtemVisuals.IsAtemConnected())
            {
                return;
            }

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
                        PluginLog.Warning($"[SetStillImageCommand] ERROR when sending image {e} ");
                    }
                });
            }
            else
            {
                PluginLog.Warning($"ERROR Not possible to send image ");
            }

            this._atemCommandSetStillMedia.SetStillImageMediaToPlayer(MEDIA_SLOT);
        }


        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            $"Set Still Image";


        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                if (stillImageData != null && File.Exists(stillImageData.ActualFullImagePath))
                {
                    try
                    {
                        PluginLog.Verbose($"[SetStillImageCommand] Rendering image preview {imageSize.GetWidth()}x{imageSize.GetHeight()} from {stillImageData.ActualFullImagePath}");
                        bitmapBuilder.SetBackgroundImage(StillImagePreview.Load(stillImageData.ActualFullImagePath, imageSize));
                        AtemVisuals.ApplyOfflineOverlay(bitmapBuilder, imageSize);
                    }
                    catch (Exception e)
                    {
                        PluginLog.Warning($"[SetStillImageCommand] Could not render image preview {stillImageData.ActualFullImagePath}: {e}");
                        AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                    }
                }
                else
                {
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                }

                AtemVisuals.DrawText(bitmapBuilder, "Set Still\nImage");

                return bitmapBuilder.ToImage();
            }
        }
    }
}

