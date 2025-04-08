namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;
    using System.Drawing;
    using System.Reflection.Metadata;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;




    // This class implements an example command that counts button presses.
    public class EnableMixFaderToggleCommand : PluginMultistateDynamicCommand, IBlinkenLightsReceiver
    {
        private Boolean _blinkState = true;

        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;
        private const Int32 BUTTON_W = 116;
        private const Int32 BUTTON_H = BUTTON_W;




      

        // Initializes the command class.
        public EnableMixFaderToggleCommand()
               : base(groupName: "Wheel Select", displayName: "Enable Mix Fader", description: "This enables a knob as Fader with the Mix Fader Adjustment")
        {
            this.IsWidget = true;   // I was looking for this setting for days - it let's the graphics control fully to the Plugin and doesn't rewrite displayText extra
                                    // it prevents as well the user to change the Icons

            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;


            this.AddState("OFF", "Mix Fader\nOFF", "Mix Fader ON");
            this.AddState("ON", "Mix Fader\nON", "Mix Fader ON");



        }



        private void OnPluginReady()
        {
            ((BlinkenLightsTimeSource)ServiceDirectory.Get(ServiceDirectory.T_BlinkenLightsTimeSource)).RegisterBlinkenLightReceiver(this);

        }


        // This method is called when the user executes the command.
        protected override void RunCommand(String actionParameter)
        {
            PluginLog.Verbose($"[EnableMixFaderToggleCommand] ToggleCurrentState {this.GetCurrentState().Name}");
            this.ToggleCurrentState();
            if (this.GetCurrentState().Name.Equals("ON"))
            {
                this._blinkState = true;
            }
        }


        protected override BitmapImage GetCommandImage(String actionParameter, Int32 stateIndex, PluginImageSize imageSize) => this.GetCommandImage( actionParameter,  imageSize);
        
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {

                if (this.GetCurrentState().Name.Equals("ON") && this._blinkState)
                {
                    bitmapBuilder.FillRectangle(0, 0, imageSize.GetWidth(), imageSize.GetHeight(), BitmapColor.Red);
                }
                else
                {
                    bitmapBuilder.FillRectangle(0, 0, imageSize.GetWidth(), imageSize.GetHeight(), BitmapColor.Black);
                }

                 bitmapBuilder.DrawText(this.GetCurrentState().DisplayName);
              //  bitmapBuilder.SetBackgroundImage(bitmapBuilder.ToImage());

                return bitmapBuilder.ToImage();
            }
            
        }


        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
         //   this.SetPropertyValue("Layout", "Centered");
            return this.GetCurrentState().DisplayName; // "\u200B";

        }
        protected override String GetCommandDisplayName(String actionParameter, Int32 stateIndex, PluginImageSize imageSize) => this.GetCommandDisplayName(actionParameter, imageSize);

        

        public void ReceiveTimeThick(Boolean blinkState) {
            this._blinkState = blinkState;
            
            this.ActionImageChanged();
        }
    }
}
