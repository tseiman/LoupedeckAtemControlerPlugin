

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    using LibAtem.Commands;
    using LibAtem.Commands.Media;
    using LibAtem.Commands.MixEffects.Transition;
    using LibAtem.Common;

    public class AtemCommandTogglePreview : IAtemCommand
    {

        private AtemControlInterface _atemControlInterface;
        private readonly Action<Boolean> _toggleCallback;

        public AtemCommandTogglePreview(Action<Boolean> toggleCallback) {
            this._toggleCallback = toggleCallback;
        }


        public void TogglePreview(Boolean preview)
        {
            PluginLog.Verbose($"[AtemCommandTogglePreview] TogglePreview");
            this._atemControlInterface.SendCommand(new TransitionPreviewSetCommand {  Index = 0, PreviewTransition = preview });

        }


        public void SetAtemClient(AtemControlInterface atemControlInterface)
        {
            this._atemControlInterface = atemControlInterface;
     //       this._atemControlInterface.RegisterOnConnectCallback(OnConnect);
            this._atemControlInterface.RegisterStateChangeListener(this, commands: new List<Type> { IAtemCommand.TransitionPreviewGetCommand });
        }
        

        public void ReceiveCommand(Object sender, ICommand command)
        {
            PluginLog.Verbose($"[AtemCommandTogglePreview] received command {command.GetType().Name}");
            if (command is TransitionPreviewGetCommand c)
            {
                this._toggleCallback?.Invoke(c.PreviewTransition);
            }
        }


        public void OnConnect() { }
    }
}

