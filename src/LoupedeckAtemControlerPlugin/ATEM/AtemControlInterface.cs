
namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{


    using System;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;
    using LibAtem.Net;
 
    using LibAtem.Common;
    using SixLabors.ImageSharp.Processing;
    using LibAtem.Util.Media;
    using LibAtem.Net.DataTransfer;
    using LibAtem.Commands.Media;
    using LibAtem.Commands;

    public class AtemControlInterface
    {

        private readonly StillImageData _imageData;

        private Boolean _connected = false;

        public AtemClient AtemClient { get; set; } = null;

        public AtemControlInterface(StillImageData imageData)
        {
            this._imageData = imageData;
            PluginLog.Info($"[AtemControlInterface] Trying to connect to ATEM {this._imageData.AtemURI}");

            this.Connect();

        }






        public void Connect() {



            this.AtemClient = new AtemClient(this._imageData.AtemURI, true);


            this.AtemClient.OnConnection += (atemClient) =>
            {
                PluginLog.Info($"[AtemControlInterface] connected {this._imageData.AtemURI}");
                this._connected = true;
            };

            this.AtemClient.OnDisconnect += (atemClient) =>
            {
                PluginLog.Info($"[AtemControlInterface] disconnected {this._imageData.AtemURI}");
                this._connected = false;
            };

            try
            {
                this.AtemClient.Connect();

            }
            catch (Exception e) {
                PluginLog.Warning($"[AtemControlInterface] connection to {this._imageData.AtemURI} failed because {e}");
            }
        }




        public Boolean QueueDataTransferJob(DataTransferJob job)
        {

            if (!this._connected)
            {
                PluginLog.Warning($"[AtemControlInterface] not connecterd to ATEM cannot QueueDataTransferJob");
                return false;
            }

            try
            {
                this.AtemClient.DataTransfer.QueueJob(job);
            }
            catch (Exception e) {
                PluginLog.Error($"[AtemControlInterface] ERROR ATEM cannot QueueDataTransferJob \n{e}");
                return false;
            }
            return true;
        }



        public Boolean SendCommand(ICommand command)
        {

            if (!this._connected)
            {
                PluginLog.Warning($"[AtemControlInterface] not connecterd to ATEM cannot SendCommand");
                return false;
            }

            try
            {
                this.AtemClient.SendCommand(command);
            }
            catch (Exception e)
            {
                PluginLog.Error($"[AtemControlInterface] ERROR ATEM cannot SendCommand \n{e}");
                return false;
            }

            
            return true;
        }




        public void Dispose()
        {
            this.AtemClient.Dispose();
        }

        public static implicit operator AtemControlInterface(AtemClient v) => throw new NotImplementedException();
    }
}

