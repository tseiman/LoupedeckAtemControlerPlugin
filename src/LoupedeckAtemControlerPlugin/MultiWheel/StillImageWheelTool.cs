namespace Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel
{
    using System;
    using System.IO;

    using Loupedeck.Devices.Loupedeck2Devices;
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    // Renders the currently selected ATEM still image at the wheel's native resolution.
    // Why a custom WheelTool: PluginDynamicCommand/Adjustment images on the wheel center are
    // capped at touch-button size (~80x80) — about 1/3 of the physical wheel. Subclassing
    // WheelTool lets us draw directly on the wheel-sized BitmapBuilder.
    public class StillImageWheelTool : WheelTool
    {
        public StillImageWheelTool()
            : base(templateDisplayName: "ATEM Still Image", templateGroupName: "ATEM Control")
        {
        }

        protected override void OnInit()
        {
            PluginLog.Verbose($"[StillImageWheelTool] OnInit");
            base.OnInit();
        }

        protected override void OnStart()
        {
            PluginLog.Verbose($"[StillImageWheelTool] OnStart");
            base.OnStart();
            StillImageChangedEvent.OnChanged += this.OnStillImageChanged;
            AtemVisuals.RegisterConnectionRefresh(this.OnStillImageChanged);
        }

        protected override void OnStop()
        {
            PluginLog.Verbose($"[StillImageWheelTool] OnStop");
            StillImageChangedEvent.OnChanged -= this.OnStillImageChanged;
            base.OnStop();
        }

        protected override BitmapImage CreateImage()
        {
            var stillImageData = ServiceDirectory.Get(ServiceDirectory.T_StillImageData) as StillImageData;

            using (var bitmapBuilder = this.CreateBitmapBuilder())
            {
                if (stillImageData != null && File.Exists(stillImageData.ActualFullImagePath))
                {
                    try
                    {
                        var image = BitmapImage.FromFile(stillImageData.ActualFullImagePath);
                        bitmapBuilder.SetBackgroundImage(image);
                        PluginLog.Verbose($"[StillImageWheelTool] CreateImage rendered {stillImageData.ActualFullImagePath}");
                    }
                    catch (Exception e)
                    {
                        PluginLog.Warning($"[StillImageWheelTool] Could not load image {stillImageData.ActualFullImagePath}: {e}");
                        bitmapBuilder.DrawText("Image\nError");
                    }
                }
                else
                {
                    bitmapBuilder.DrawText("No Still\nImage");
                }

                return bitmapBuilder.ToImage();
            }
        }

        private void OnStillImageChanged()
        {
            try
            {
                this.Draw();
            }
            catch (Exception e)
            {
                PluginLog.Warning($"[StillImageWheelTool] Draw failed: {e}");
            }
        }
    }
}
