namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;



    // This class implements an example command that counts button presses.
    public class MacroPlayCommand : PluginMultistateDynamicCommand // , IBlinkenLightsReceiver
    {
        //       private AtemCommandMacroPlay _atemCommandMacroPlay;
        //     private Boolean _blinkState;

        //   private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;

        private String _currentState = "";
        private BitmapImage _image;

        // Initializes the command class.
        public MacroPlayCommand()
            : base(groupName: "Misc", displayName: "Run Macro", description: "Runs a macro which was predefined in the ATEM GUI")
        {


            this.IsWidget = true;


            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;

            this.AddState("ON", "In Macro", "Macro is Running");
            this.AddState("OFF", "Out Macro" , "Run Macro");

            this.MakeProfileAction("list;aaaa:");


        }

        protected override Boolean OnLoad() {
            var x = base.OnLoad();
        
            return x;
        }



        private void OnPluginReady()
        {
            PluginLog.Verbose($"[MacroPlayCommand] OnPluginReady");
            AtemVisuals.RegisterConnectionRefresh(this.ActionImageChanged);


            //       this._atemCommandMacroPlay = new();

            //         ((BlinkenLightsTimeSource)ServiceDirectory.Get(ServiceDirectory.T_BlinkenLightsTimeSource)).RegisterBlinkenLightReceiver(this);

        }

        /*

        private void GetToggleEvent(Boolean state)
        {
            if (state)
            { this.SetCurrentState(0); } else
            { this.SetCurrentState(1); }
            
        }
        */


        // This method is called when the user executes the command.
        protected override void RunCommand(String actionParameter)
        {
            if (!AtemVisuals.IsAtemConnected())
            {
                return;
            }


            //   this.ToggleCurrentState(actionParameter);

            PluginLog.Verbose($"[MacroPlayCommand] >>>>> ToggleCurrentState {actionParameter}//{this.GetCurrentState(actionParameter).Name}");

            this.ToggleCurrentState(actionParameter);
            this._currentState = this.GetCurrentState(actionParameter).Name;
            //   this._atemCommandTogglePreview.TogglePreview(this.GetCurrentState().Name.Equals("ON"));

            this.ActionImageChanged();

        }


        protected override PluginActionParameter[] GetParameters() {

            var x = new List<PluginActionParameter> { new PluginActionParameter($"actionParamA", "aaaaaa", String.Empty), new PluginActionParameter($"actionParamB", "bbbbb", String.Empty) };
            return x.ToArray();
        }


        protected override BitmapImage GetCommandImage(String actionParameter, Int32 stateIndex, PluginImageSize imageSize)
        {
            // protected override BitmapImage GetCommandImage(String actionParameter, PluginImageSize imageSize) {



            PluginLog.Verbose($"[MacroPlayCommand] GetCommandImage {this._currentState}////{actionParameter}//{stateIndex}// ,  {this.GetCurrentState(this._currentState).Name}");

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {

                if (this._currentState.Equals("ON"))// && this._blinkState 
                {
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Green);
                    AtemVisuals.DrawText(bitmapBuilder, this.GetCurrentState(this._currentState).DisplayName);
                }
                else if (this._currentState.Equals("OFF"))
                {
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                    AtemVisuals.DrawText(bitmapBuilder, this.GetCurrentState(this._currentState).DisplayName);
                }
                else
                {
                    return this._image;
                }

                //      bitmapBuilder.FillRectangle(0, 0, imageSize.GetWidth(), imageSize.GetHeight(), BitmapColor.Black);

                //   bitmapBuilder.DrawText(this.GetCurrentState(this._currentState).DisplayName);

                this._image = bitmapBuilder.ToImage();
                return this._image;
            }

        }


        protected override String GetCommandDisplayName(String actionParameter,  PluginImageSize imageSize)
        {
                     PluginLog.Verbose($"[MacroPlayCommand] GetCommandDisplayName {this.GetCurrentState(actionParameter).Name},  {this.GetCurrentState(actionParameter).Description},  {this.GetCurrentState(actionParameter).DisplayName}");
                       return this.GetCurrentState(this._currentState).DisplayName;

            // return "aaaaa";

        }
        

        /*
        public void ReceiveTimeThick(Boolean blinkState)
        {
            this._blinkState = blinkState;

            this.ActionImageChanged();
        }
        */
    }
}
