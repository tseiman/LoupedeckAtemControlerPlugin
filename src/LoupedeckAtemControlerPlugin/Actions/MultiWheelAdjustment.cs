
namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;
    using Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel;

    public class MultiWheelAdjustment : PluginDynamicAdjustment
    {
        private MultiWheelDispatch _mwd;

        // private Int32 _currentFaderPos;

        //  private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;

        public MultiWheelAdjustment()
               : base(groupName: "Wheel Select", displayName: "Multi wheel Adjustment", description: "Can do Cross Mix, TBD",hasReset: false)
        {

            this.IsWidget = true;

            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;

        }




        private void OnPluginReady()
        {
            PluginLog.Verbose($"[MultiWheelAdjustment] OnPluginReady");
            this._mwd = (MultiWheelDispatch)ServiceDirectory.Get(ServiceDirectory.T_MultiWheelDispatch);
            this._mwd.DisplayChanged += this.ActionImageChanged;
            AtemVisuals.RegisterConnectionRefresh(this.ActionImageChanged);

        }


        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            if (!AtemVisuals.IsAtemConnected())
            {
                return;
            }

            this._mwd.ApplyAdjustment(diff);
        }


        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (this._mwd?.ActiveDisplay != null)
            {
                return this._mwd.ActiveDisplay.DisplayName;
            }

            return $"{this.DisplayName}";
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            if (this._mwd?.ActiveDisplay != null)
            {
                return $"{this._mwd.ActiveDisplay.DisplayPercent}%";
            }

            return "";
        }




        
        protected override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize)
        {
            var activeDisplay = this._mwd?.ActiveDisplay;
            if (activeDisplay == null)
            {
                return null;
            }

            var percent = Math.Clamp(activeDisplay.DisplayPercent, 0, 100);
            var width = imageSize.GetWidth();
            var height = imageSize.GetHeight();
            var margin = Math.Max(6, width / 14);
            var trackHeight = Math.Max(8, height / 8);
            var trackY = (height / 2) - (trackHeight / 2);
            var fillWidth = (Int32)Math.Round((width - (margin * 2)) * (percent / 100.0));

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                bitmapBuilder.FillRectangle(margin, trackY, width - (margin * 2), trackHeight, new BitmapColor(0x30, 0x30, 0x30));
                bitmapBuilder.FillRectangle(margin, trackY, fillWidth, trackHeight, Colors.YELLOW);
                AtemVisuals.DrawText(bitmapBuilder, $"{percent}%");

                return bitmapBuilder.ToImage();
            }

        }
        

    }


}
