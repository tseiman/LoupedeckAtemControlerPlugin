

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{

    using LibAtem.Commands.Media;
    using LibAtem.Common;

    public class AtemCommandSetStillMedia : IAtemCommand
    {

        private AtemControlInterface _atemControlInterface;


        public void SetStillImageMediaToPlayer(UInt32 slot)
        {

            PluginLog.Verbose($"[AtemCommandSetStillMedia] setStillImageMediaToPlayer");

            this._atemControlInterface.SendCommand(new MediaPlayerSourceSetCommand { Mask = MediaPlayerSourceSetCommand.MaskFlags.StillIndex, Index = (MediaPlayerId)0, StillIndex = slot });

        }


        public void setAtemClient(AtemControlInterface atemControlInterface) => this._atemControlInterface = atemControlInterface;
    }
}

