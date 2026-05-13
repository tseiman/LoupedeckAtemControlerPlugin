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
        // Initializes the command class.
        public MacroPlayCommand()
            : base(groupName: "Misc", displayName: "Run Macro", description: "Runs a macro which was predefined in the ATEM GUI")
        {


            this.IsWidget = true;


            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;

            this.AddState("ON", "In Macro", "Macro is Running");
            this.AddState("OFF", "Out Macro" , "Run Macro");
            this._currentState = "OFF";

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



            var currentStateName = this.GetCurrentMacroStateName(actionParameter);
            var currentStateDisplayName = this.GetCurrentMacroStateDisplayName(actionParameter);

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {

                if (currentStateName.Equals("ON"))// && this._blinkState 
                {
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Green);
                    AtemVisuals.DrawText(bitmapBuilder, currentStateDisplayName);
                }
                else
                {
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                    AtemVisuals.DrawText(bitmapBuilder, currentStateDisplayName);
                }

                return bitmapBuilder.ToImage();
            }

        }


        protected override String GetCommandDisplayName(String actionParameter,  PluginImageSize imageSize)
        {
            return this.GetCurrentMacroStateDisplayName(actionParameter);

        }

        private String GetCurrentMacroStateName(String actionParameter)
        {
            if (this._currentState.Equals("ON") || this._currentState.Equals("OFF"))
            {
                return this.GetCurrentState(this._currentState).Name;
            }

            var state = this.GetCurrentState(actionParameter);
            this._currentState = state?.Name ?? "OFF";
            return this._currentState;
        }

        private String GetCurrentMacroStateDisplayName(String actionParameter)
        {
            var stateName = this.GetCurrentMacroStateName(actionParameter);
            return this.GetCurrentState(stateName).DisplayName;
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
