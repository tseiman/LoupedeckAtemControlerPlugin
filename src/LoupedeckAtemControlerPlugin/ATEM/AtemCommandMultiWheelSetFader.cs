

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    using LibAtem.Commands;

    using Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel;

    public class AtemCommandMultiWheelSetFader : IAtemCommand, IMultiWheelAtemAdjustment
    {




     //   public void setAtemClient(AtemControlInterface atemControlInterface) => this._atemControlInterface = atemControlInterface;

        public void ReceiveCommand(Object sender, ICommand command) {
            PluginLog.Verbose($"[AtemCommandSetFader] received command {command.GetType().Name}");
        }

        public void ApplyAdjustment(Int32 diff)
        {
            PluginLog.Verbose($"[AtemCommandSetFader] applyAdjustment {diff}");
        }

        public void OnConnect() { }
    }
}

