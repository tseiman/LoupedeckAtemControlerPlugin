
namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;
    using Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel;

    public class MultiWheelAdjustment : PluginDynamicAdjustment
    {
        private MultiWheelDispatch _mwd;
        private String _lastActionParameter = "";

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
            this._mwd.DisplayChanged += this.RefreshAdjustmentDisplay;
            AtemVisuals.RegisterConnectionRefresh(this.RefreshAdjustmentDisplay);

        }


        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            this._lastActionParameter = actionParameter ?? "";

            if (!AtemVisuals.IsAtemConnected())
            {
                return;
            }

            this._mwd.ApplyAdjustment(diff);
            this.RefreshAdjustmentDisplay();
        }


        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            this._lastActionParameter = actionParameter ?? "";

            if (this._mwd?.ActiveDisplay != null)
            {
                return "";
            }

            return $"{this.DisplayName}";
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            this._lastActionParameter = actionParameter ?? "";

            if (this._mwd?.ActiveDisplay != null)
            {
                return $"{this._mwd.ActiveDisplay.DisplayPercent}%";
            }

            return "";
        }




        
        protected override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize)
        {
            PluginLog.Verbose($"[MultiWheelAdjustment] GetAdjustmentImage imageSize={imageSize.GetWidth()}x{imageSize.GetHeight()} actionParameter='{actionParameter ?? "<null>"}'");

            var activeDisplay = this._mwd?.ActiveDisplay;
            if (activeDisplay == null)
            {
                return null;
            }

            var percent = Math.Clamp(activeDisplay.DisplayPercent, 0, 100);
            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                AtemVisuals.DrawText(bitmapBuilder, $"{percent}%");

                return bitmapBuilder.ToImage();
            }

        }

        private void RefreshAdjustmentDisplay()
        {
            this.ActionImageChanged();
            this.AdjustmentValueChanged(this._lastActionParameter);
        }
        

    }


}
