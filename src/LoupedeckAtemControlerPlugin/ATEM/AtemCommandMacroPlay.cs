

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    using LibAtem.Commands;
    using LibAtem.Commands.Macro;
    using LibAtem.Commands.MixEffects.Transition;


    public class MacroStor {

        public Int32 Index { get; }
        public String Name { get; }
        public String Description { get; }

        public MacroStor(Int32 index, String name, String description) {
            this.Index = index;
            this.Name = name;
            this.Description = description;
        }
    }


    public class AtemCommandMacroPlay : IAtemCommand
    {

     //   private AtemControlInterface _atemControlInterface;
//        private readonly Action<Boolean> _toggleCallback;


        public AtemCommandMacroPlay() {
            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
            atemControlInterface.RegisterStateChangeListener(this, commands: new List<Type> { IAtemCommand.T_MacroPropertiesGetCommand });

           
        }


/*        public void TogglePreview(Boolean preview)
        {
            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
            PluginLog.Verbose($"[AtemCommandTogglePreview] TogglePreview");
            atemControlInterface.SendCommand(new TransitionPreviewSetCommand { Index = 0, PreviewTransition = preview });

        }
*/

    

        public void ReceiveCommand(Object sender, ICommand command)
        {
            PluginLog.Verbose($"[AtemCommandMacroPlay] received command {command.GetType().Name}");
            if (command is MacroPropertiesGetCommand)
            {
                var c = (MacroPropertiesGetCommand)command;

                if (c.IsUsed && (!c.HasUnsupportedOps))
                {
                  //  PluginLog.Verbose($"[AtemCommandMacroPlay] command eith parameters: \nindex: {c.Index}, \nisUsed: {c.IsUsed} \nname: {c.Name} \ndesc: {c.Description}, \nunsupportedOps: {c.HasUnsupportedOps} \n");

                }

            }
        }


        public void OnConnect() { }
    }
}

