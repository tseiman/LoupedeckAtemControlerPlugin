

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    using LibAtem.Commands;
    using LibAtem.Commands.MixEffects.Transition;

    using Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel;

    public class AtemCommandMultiWheelSetFader : IAtemCommand, IMultiWheelAtemAdjustment
    {
        private Int32 _transitionPos;
        private Int32 _previousPos;

        private Int32 prefix = 1;

        public AtemCommandMultiWheelSetFader()
        {
            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
            atemControlInterface.RegisterStateChangeListener(this, commands: new List<Type> { IAtemCommand.T_TransitionPositionGetCommand });
        }



        //   public void setAtemClient(AtemControlInterface atemControlInterface) => this._atemControlInterface = atemControlInterface;

        public void ReceiveCommand(Object sender, ICommand command) {
            PluginLog.Verbose($"[AtemCommandSetFader] received command {command.GetType().Name}");

            var tmpCmd = (TransitionPositionGetCommand)command; 
            this._transitionPos = (Int32)(tmpCmd.HandlePosition * 1000);
            if (tmpCmd.HandlePosition == 0 && (!tmpCmd.InTransition))
            {
                this.prefix *= -1;
            }

            PluginLog.Verbose($"[AtemCommandSetFader] updated Fader Pos {this._transitionPos}, in trans: {tmpCmd.InTransition}, rem frms: {tmpCmd.RemainingFrames}");
        }

        public void ApplyAdjustment(Int32 diff)
        {
            var isUp = diff > 0 ? 1 : -1;



            var tmpPos = this._transitionPos + (isUp )  * 1;


            PluginLog.Verbose($"[AtemCommandSetFader] applyAdjustment {diff}");




            var transPos = (Double)tmpPos / 1000;

            if (tmpPos < 0)
            {
                transPos = 0;
           
            }

            if (tmpPos >= 1000)
            {
                transPos = 1;

            }

         /*   if (this._previousPos > 1000 && isUp > 0)
            {
                transPos = 0;
               
            }
         */


            PluginLog.Verbose($"[AtemCommandSetFader] try to set tmpPos: {tmpPos}, transPos: {transPos}, isUp: {isUp}, _previousPos: {this._previousPos}");


            this._previousPos = this._transitionPos;

            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
            atemControlInterface.SendCommand(new TransitionPositionSetCommand { Index = 0, HandlePosition = transPos });


        }

        public void OnConnect() { }
    }
}

