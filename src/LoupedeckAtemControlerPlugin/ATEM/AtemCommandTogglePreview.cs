

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{

    using LibAtem.Commands.Media;
    using LibAtem.Commands.MixEffects.Transition;
    using LibAtem.Common;

    public class AtemCommandTogglePreview : IAtemCommand
    {

        private AtemControlInterface _atemControlInterface;


        public void TogglePreview(Boolean preview)
        {
            PluginLog.Verbose($"[AtemCommandTogglePreview] TogglePreview");
            this._atemControlInterface.SendCommand(new TransitionPreviewSetCommand {  Index = 0, PreviewTransition = preview });
        }

        /*   public Boolean GetPreviewState() {
               PluginLog.Verbose($"[AtemCommandTogglePreview] TogglePreview");
               this._atemControlInterface.SendCommand(new TransitionPreviewGetCommand { Index = 0 });
           }
        */
        public void setAtemClient(AtemControlInterface atemControlInterface) => this._atemControlInterface = atemControlInterface;
    }
}

