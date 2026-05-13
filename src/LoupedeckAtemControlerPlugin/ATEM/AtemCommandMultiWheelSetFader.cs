

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    using LibAtem.Commands;
    using LibAtem.Commands.MixEffects.Transition;

    using Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel;

    public class AtemCommandMultiWheelSetFader : IAtemCommand, IMultiWheelAtemAdjustment
    {
        private const Double MinHandlePosition = 0.0;
        private const Double MaxHandlePosition = 1.0;
        private const Double WheelStep = 0.0125;
        private const Double EndSnapDistance = 0.02;

        private Double _transitionPos;
        private Boolean _inTransition;
        private Int32 _activeWheelDirection;
        private Int32 _completedWheelDirection;

        public AtemCommandMultiWheelSetFader()
        {
            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
            atemControlInterface.RegisterStateChangeListener(this, commands: new List<Type> { IAtemCommand.T_TransitionPositionGetCommand });
        }



        //   public void setAtemClient(AtemControlInterface atemControlInterface) => this._atemControlInterface = atemControlInterface;

        public void ReceiveCommand(Object sender, ICommand command) {
            PluginLog.Verbose($"[AtemCommandSetFader] received command {command.GetType().Name}");

            var tmpCmd = (TransitionPositionGetCommand)command;
            this._transitionPos = SnapEndpoint(ClampHandlePosition(tmpCmd.HandlePosition));
            this._inTransition = tmpCmd.InTransition;
            if (!this._inTransition && this._transitionPos <= MinHandlePosition + EndSnapDistance)
            {
                this._transitionPos = MinHandlePosition;
                this._activeWheelDirection = 0;
            }

            PluginLog.Verbose($"[AtemCommandSetFader] updated Fader Pos {this._transitionPos}, in trans: {tmpCmd.InTransition}, rem frms: {tmpCmd.RemainingFrames}");
        }

        public void ApplyAdjustment(Int32 diff)
        {
            if (diff == 0)
            {
                return;
            }

            PluginLog.Verbose($"[AtemCommandSetFader] applyAdjustment {diff}");

            var wheelDirection = diff > 0 ? 1 : -1;
            if (!this._inTransition && this._transitionPos <= MinHandlePosition + EndSnapDistance)
            {
                if (this._completedWheelDirection == wheelDirection)
                {
                    return;
                }

                this._activeWheelDirection = wheelDirection;
                this._completedWheelDirection = 0;
            }

            var step = diff * WheelStep;
            if (this._activeWheelDirection != 0)
            {
                step = diff * this._activeWheelDirection * WheelStep;
            }

            var nextPos = SnapEndpoint(ClampHandlePosition(this._transitionPos + step));
            if (PositionsEqual(nextPos, this._transitionPos))
            {
                return;
            }

            PluginLog.Verbose($"[AtemCommandSetFader] try to set transPos: {nextPos}, previousPos: {this._transitionPos}, inTransition: {this._inTransition}");


            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
            if (atemControlInterface.SendCommand(new TransitionPositionSetCommand { Index = 0, HandlePosition = nextPos }))
            {
                this._transitionPos = nextPos;
                this._inTransition = nextPos > MinHandlePosition && nextPos < MaxHandlePosition;
                if (nextPos >= MaxHandlePosition - EndSnapDistance)
                {
                    this._completedWheelDirection = this._activeWheelDirection;
                    this._activeWheelDirection = 0;
                }
            }


        }

        public void OnConnect() { }

        private static Double ClampHandlePosition(Double value)
        {
            if (value < MinHandlePosition)
            {
                return MinHandlePosition;
            }

            if (value > MaxHandlePosition)
            {
                return MaxHandlePosition;
            }

            return value;
        }

        private static Boolean PositionsEqual(Double left, Double right) => Math.Abs(left - right) < 0.00001;

        private static Double SnapEndpoint(Double value)
        {
            if (value <= MinHandlePosition + EndSnapDistance)
            {
                return MinHandlePosition;
            }

            if (value >= MaxHandlePosition - EndSnapDistance)
            {
                return MaxHandlePosition;
            }

            return value;
        }
    }
}

