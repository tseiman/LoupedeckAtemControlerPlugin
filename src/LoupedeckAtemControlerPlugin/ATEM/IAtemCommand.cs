namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    using LibAtem.Commands;
    using LibAtem.Commands.MixEffects.Transition;
    using LibAtem.Net;

    public interface IAtemCommand
    {

        public static readonly Type TransitionPreviewGetCommand = typeof(TransitionPreviewGetCommand);


        public void SetAtemClient(AtemControlInterface atemControlInterface);
        public void ReceiveCommand(Object sender, ICommand command);
        public void OnConnect();
    }
}

