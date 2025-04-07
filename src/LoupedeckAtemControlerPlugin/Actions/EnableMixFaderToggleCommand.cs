namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;
    using log4net.Plugin;

    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;

    // This class implements an example command that counts button presses.
    public class EnableMixFaderToggleCommand : PluginMultistateDynamicCommand
    {

        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;



        // Initializes the command class.
        public EnableMixFaderToggleCommand()
               : base(displayName: "Enable Mix Fader", description: "This enables a knob as Fader with the Mix Fader Adjustment", groupName: "Commands")
        {

            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;

            this.AddState("ON", "Mix Fader ON", "Mix Fader ON");
            this.AddState("OFF", "Mix Fader OFF", "Mix Fader OFF");
        }


        private void OnPluginReady()
        {
        }


        // This method is called when the user executes the command.
        protected override void RunCommand(String actionParameter)
        {
            PluginLog.Verbose($"[EnableMixFaderToggleCommand] ToggleCurrentState {this.GetCurrentState().Name}");
            this.ToggleCurrentState();

        }


        protected override BitmapImage GetCommandImage(String actionParameter, Int32 stateIndex, PluginImageSize imageSize) {



            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {

                if (this.GetCurrentState().Name.Equals("ON"))
                {
                    bitmapBuilder.FillRectangle(0, 0, imageSize.GetButtonWidth(), imageSize.GetButtonHeight(), BitmapColor.Red);
                }
                else
                {
                    bitmapBuilder.FillRectangle(0, 0, imageSize.GetButtonWidth(), imageSize.GetButtonHeight(), BitmapColor.Black);
                }

                bitmapBuilder.DrawText(this.GetCurrentState().DisplayName);

                return bitmapBuilder.ToImage();
            }

        }


        // This method is called when Loupedeck needs to show the command on the console or the UI.
        protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            return this.GetCurrentState().DisplayName;
        }
        
    }
}
