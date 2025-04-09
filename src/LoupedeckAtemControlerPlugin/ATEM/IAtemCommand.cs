namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    using LibAtem.Commands;
    using LibAtem.Commands.MixEffects.Transition;


    public interface IAtemCommand
    {

        public static readonly Type T_TransitionPreviewGetCommand = typeof(TransitionPreviewGetCommand);
        public static readonly Type T_TimeCodeCommand = typeof(TimeCodeCommand);
        public static readonly Type T_TransitionPositionGetCommand = typeof(TransitionPositionGetCommand);


        public void ReceiveCommand(Object sender, ICommand command);
        public void OnConnect();
    }
}

