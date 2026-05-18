namespace Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel
{
    using System;
    using System.Collections.Generic;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    // Bridge that exposes StillImageWheelTool to the Loupedeck UI.
    // Why: PluginDynamicFolder.GetWheelToolNames() is what makes a custom WheelTool
    // selectable as a wheel-page template. Without a folder declaring our tool,
    // the framework instantiates StillImageWheelTool but never offers it in the UI.
    public class StillImageWheelFolder : PluginDynamicFolder
    {
        // Name of the WheelTool subclass that this folder offers. Verified at runtime
        // via the OnInit log line in StillImageWheelTool.
        private const String StillImageWheelToolTemplateName = "StillImageWheelTool";

        public StillImageWheelFolder()
        {
            this.DisplayName = "ATEM Still Image";
            this.GroupName = "ATEM Control";
            this.Description = "Browse and send still images to ATEM (wheel page)";
        }

        public override IEnumerable<String> GetWheelToolNames() =>
            new[] { StillImageWheelToolTemplateName };

        public override IEnumerable<String> GetWheelToolNames(DeviceType deviceType) =>
            new[] { StillImageWheelToolTemplateName };

        public override IEnumerable<String> GetEncoderRotateActionNames(DeviceType deviceType) =>
            new[] { this.CreateAdjustmentName("scroll") };

        public override IEnumerable<String> GetEncoderPressActionNames(DeviceType deviceType) =>
            new[] { this.CreateCommandName("send") };

        public override BitmapImage GetButtonImage(PluginImageSize imageSize)
        {
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.FillRectangle(0, 0, imageSize.GetWidth(), imageSize.GetHeight(), BitmapColor.Black);
                bitmapBuilder.DrawText("ATEM\nStill\nImage");
                return bitmapBuilder.ToImage();
            }
        }

        public override String GetButtonDisplayName(PluginImageSize imageSize) => "ATEM Still Image";

        public override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize)
        {
            var stillImageData = ServiceDirectory.Get(ServiceDirectory.T_StillImageData) as StillImageData;
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                var text = stillImageData != null && stillImageData.ImageCount > 0
                    ? $"Img\n{stillImageData.SelectionDisplayName}"
                    : "No\nImage";
                AtemVisuals.DrawText(bitmapBuilder, text);
                return bitmapBuilder.ToImage();
            }
        }

        public override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize) =>
            "Scroll Image";

        public override String GetAdjustmentValue(String actionParameter)
        {
            var stillImageData = ServiceDirectory.Get(ServiceDirectory.T_StillImageData) as StillImageData;
            return stillImageData != null && stillImageData.ImageCount > 0
                ? $"{stillImageData.ActualImageIndex + 1}/{stillImageData.ImageCount}"
                : "0/0";
        }

        public override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
            "Send Still";

        public override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                AtemVisuals.DrawText(bitmapBuilder, "Send\nStill");
                return bitmapBuilder.ToImage();
            }
        }

        public override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            PluginLog.Verbose($"[StillImageWheelFolder] ApplyAdjustment '{actionParameter}' diff={diff}");
            // TODO: wire to existing image-scroll logic once the wheel tool itself is selectable in the UI.
        }

        public override void RunCommand(String actionParameter)
        {
            PluginLog.Verbose($"[StillImageWheelFolder] RunCommand '{actionParameter}'");
            // TODO: wire to existing upload-still logic once the wheel tool itself is selectable in the UI.
        }
    }
}
