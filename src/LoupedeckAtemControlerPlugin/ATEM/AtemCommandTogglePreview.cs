

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    using LibAtem.Commands;
    using LibAtem.Commands.MixEffects.Transition;

    public class AtemCommandTogglePreview : IAtemCommand
    {

     //   private AtemControlInterface _atemControlInterface;
        private readonly Action<Boolean> _toggleCallback;

        public AtemCommandTogglePreview(Action<Boolean> toggleCallback) {
            this._toggleCallback = toggleCallback;
            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
            atemControlInterface.RegisterStateChangeListener(this, commands: new List<Type> { IAtemCommand.T_TransitionPreviewGetCommand });
        }


        public void TogglePreview(Boolean preview)
        {
            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
            PluginLog.Verbose($"[AtemCommandTogglePreview] TogglePreview");
            atemControlInterface.SendCommand(new TransitionPreviewSetCommand { Index = 0, PreviewTransition = preview });

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

