namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;
    using Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel;



    // This class implements an example command that counts button presses.
    public class EnableDummyFaderToggleCommand : PluginMultistateDynamicCommand, IBlinkenLightsReceiver, IMultiWheelDispatchable
    {
        private Boolean _blinkState = true;
        private MultiWheelDispatch _mwd;

        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;
        private const Int32 BUTTON_W = 116;
        private const Int32 BUTTON_H = BUTTON_W;





        // Initializes the command class.
        public EnableDummyFaderToggleCommand()
               : base(groupName: "Wheel Select", displayName: "Enable Dummy Fader", description: "This is just to select multi fader with big multi wheel")
        {
            this.IsWidget = true;   // I was looking for this setting for days - it let's the graphics control fully to the Plugin and doesn't rewrite displayText extra
                                    // it prevents as well the user to change the Icons

            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;

   

            this.AddState("OFF", "Dummy Fader\nOFF", "Dummy Fader ON");
            this.AddState("ON", "Dummy Fader\nON", "Dummy Fader ON");



        }

        private class DummAdjustment : IMultiWheelAtemAdjustment {
            public void ApplyAdjustment(Int32 diff) => PluginLog.Verbose($"[DummAdjustment] applyAdjustment {diff}");
        }


        private void OnPluginReady()
        {
            ((BlinkenLightsTimeSource)ServiceDirectory.Get(ServiceDirectory.T_BlinkenLightsTimeSource)).RegisterBlinkenLightReceiver(this);
            this._mwd = (MultiWheelDispatch)ServiceDirectory.Get(ServiceDirectory.T_MultiWheelDispatch);
            this._mwd.RegisterDispatchable(this, new DummAdjustment());
        }


        // This method is called when the user executes the command.
        protected override void RunCommand(String actionParameter)
        {
            PluginLog.Verbose($"[EnableDummyFaderToggleCommand] ToggleCurrentState {this.GetCurrentState().Name}");
            this.ToggleCurrentState();
            if (this.GetCurrentState().Name.Equals("ON"))
            {
                this._blinkState = true;
                this._mwd.InformActive(this);
            }
            else
            {
                this._mwd.InformInActive(this);
            }
        }


        protected override BitmapImage GetCommandImage(String actionParameter, Int32 stateIndex, PluginImageSize imageSize) => this.GetCommandImage( actionParameter,  imageSize);
        
        protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize)
        {

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {

                if (this.GetCurrentState().Name.Equals("ON") && this._blinkState)
                {
                    bitmapBuilder.FillRectangle(0, 0, imageSize.GetWidth(), imageSize.GetHeight(), Colors.YELLOW);
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

        public void Disengage() {
            PluginLog.Verbose($"[EnableDummyFaderToggleCommand] Disangage");
            this.SetCurrentState(0);
            this.ActionImageChanged();
        }
    }
}
