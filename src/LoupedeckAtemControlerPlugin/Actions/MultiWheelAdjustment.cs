
namespace Loupedeck.LoupedeckAtemControlerPlugin
{




    public class MultiWheelAdjustment : PluginDynamicAdjustment
    {
        private Int32 _currentFaderPos;

        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;

        public MultiWheelAdjustment()
               : base(groupName: "Wheel Select", displayName: "Multi wheel Adjustment", description: "Can do Cross Mix, TBD",hasReset: false)
        {


            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;

        }




        private void OnPluginReady()
        {
            PluginLog.Verbose($"[MultiWheelAdjustment] OnPluginReady");


        }


        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
          


            this._currentFaderPos += diff;


            if (this._currentFaderPos >= 9999)
            {
                this._currentFaderPos = 9999;
            }


            if (this._currentFaderPos < 0)
            {
                this._currentFaderPos = 0;
            }


            PluginLog.Verbose($"[ImageScroll] ApplyAdjustment → index: {this._currentFaderPos}, actionParam: {actionParameter}");


        }


        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            return $"{this.DisplayName}";
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            return $"{this._currentFaderPos}";
        }




        
        protected override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize)
        {

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {

                    bitmapBuilder.FillRectangle(0, 0, imageSize.GetWidth(), imageSize.GetHeight(), BitmapColor.Blue);

                bitmapBuilder.DrawText(this.DisplayName);
                //  bitmapBuilder.SetBackgroundImage(bitmapBuilder.ToImage());

                return bitmapBuilder.ToImage();
            }

        }
        

    }


}