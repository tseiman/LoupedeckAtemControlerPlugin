namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;
    using log4net.Plugin;

    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    // This class implements an example command that counts button presses.
    public class TransitionPreviewToggleCommand : PluginMultistateDynamicCommand, IBlinkenLightsReceiver
    {
        private AtemCommandTogglePreview _atemCommandTogglePreview;
        private Boolean _blinkState;

        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;

       

        // Initializes the command class.
        public TransitionPreviewToggleCommand()
               : base(groupName: "Misc", displayName: "Toggle Preview Transition", description: "The transition can be previewed in ATEM preview screen")
        {
            this.IsWidget = true;

            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;

            this.AddState("OFF", "Preview Transition OFF", "Preview Transition OFF");
            this.AddState("ON", "Preview Transition ON", "Preview Transition ON");

        }


        private void OnPluginReady()
        {
            this._atemCommandTogglePreview = new(this.GetToggleEvent);

            ((BlinkenLightsTimeSource)ServiceDirectory.Get(ServiceDirectory.T_BlinkenLightsTimeSource)).RegisterBlinkenLightReceiver(this);
            AtemVisuals.RegisterConnectionRefresh(this.ActionImageChanged);

        }


        private void GetToggleEvent(Boolean state)
        {
            if (state)
            { this.SetCurrentState(1); } else
            { this.SetCurrentState(0); }
            
        }




        // This method is called when the user executes the command.
        protected override void RunCommand(String actionParameter)
        {
            this.ToggleCurrentState();
            PluginLog.Verbose($"[TransitionPreviewToggleCommand] ToggleCurrentState {this.GetCurrentState().Name}");

            this._atemCommandTogglePreview.TogglePreview(this.GetCurrentState().Name.Equals("ON"));


            this.ActionImageChanged();
        }


        protected override BitmapImage GetCommandImage(String actionParameter, Int32 stateIndex, PluginImageSize imageSize) {

           

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {

                if (this.GetCurrentState().Name.Equals("ON") && this._blinkState)
                {
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Red);
                }
                else
                {
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                }

                AtemVisuals.DrawText(bitmapBuilder, this.GetCurrentState().DisplayName);

                return bitmapBuilder.ToImage();
            }

        }


        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            return this.GetCurrentState().DisplayName; 

        }
        protected override String GetCommandDisplayName(String actionParameter, Int32 stateIndex, PluginImageSize imageSize) => this.GetCommandDisplayName(actionParameter, imageSize);



        public void ReceiveTimeThick(Boolean blinkState)
        {
            this._blinkState = blinkState;

            this.ActionImageChanged();
        }

    }
}
