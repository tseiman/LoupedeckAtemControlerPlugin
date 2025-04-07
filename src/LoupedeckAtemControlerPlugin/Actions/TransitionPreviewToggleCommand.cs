namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;
    using log4net.Plugin;

    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;

    // This class implements an example command that counts button presses.
    public class TransitionPreviewToggleCommand : PluginMultistateDynamicCommand
    {
        private AtemCommandTogglePreview _atemCommandTogglePreview;

        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;

       

        // Initializes the command class.
        public TransitionPreviewToggleCommand()
               : base(displayName: "Toggle Preview Transition", description: "The transition can be previewed in ATEM preview screen", groupName: "Commands")
        {

            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;

            this.AddState("ON", "Preview Transition ON", "Preview Transition ON");
            this.AddState("OFF", "Preview Transition OFF", "Preview Transition OFF");
        }

        private void OnPluginReady()
        {
            this._atemCommandTogglePreview = new();
            this._plugin.initAtemCommand(this._atemCommandTogglePreview);
            this._atemCommandTogglePreview.TogglePreview(false); // HACK - please implement a way how to get state
        }






        // This method is called when the user executes the command.
        protected override void RunCommand(String actionParameter)
        {
            this.ToggleCurrentState();
            PluginLog.Verbose($"[TransitionPreviewToggleCommand] ToggleCurrentState {this.GetCurrentState().Name}");

            this._atemCommandTogglePreview.TogglePreview(this.GetCurrentState().Name.Equals("ON"));

            

        }


        protected override BitmapImage GetCommandImage(String actionParameter, Int32 stateIndex, PluginImageSize imageSize) {

           

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                //  bitmapBuilder.SetBackgroundImage(EmbeddedResources.ReadImage("MyPlugin.EmbeddedResources.MyImage.png"));

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
            protected override String GetCommandDisplayName(String actionParameter, PluginImageSize imageSize) =>
                $"";

        
    }
}
