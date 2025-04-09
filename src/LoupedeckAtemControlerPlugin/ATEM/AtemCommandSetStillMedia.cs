

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    using LibAtem.Commands;
    using LibAtem.Commands.Media;
    using LibAtem.Common;

    public class AtemCommandSetStillMedia : IAtemCommand
    {

        public void SetStillImageMediaToPlayer(UInt32 slot)
        {

            PluginLog.Verbose($"[AtemCommandSetStillMedia] setStillImageMediaToPlayer");

            var atemControlInterface = (AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface);
            atemControlInterface.SendCommand(new MediaPlayerSourceSetCommand { Mask = MediaPlayerSourceSetCommand.MaskFlags.StillIndex, Index = (MediaPlayerId)0, StillIndex = slot });

        }


        public void ReceiveCommand(Object sender, ICommand command) {
            PluginLog.Verbose($"[AtemCommandSetStillMedia] received command {command.GetType().Name}");
        }

        
        public void OnConnect() { }


    }
}

