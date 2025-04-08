
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
    using LibAtem.Commands.Macro;
    using LibAtem.Commands.CameraControl;
    using LibAtem.Commands.DataTransfer;
    using System.Threading;

    public class AtemControlInterface
    {

        private readonly StillImageData _imageData;

        private Boolean _connected = false;

        public AtemClient AtemClient { get; set; } = null;

        private readonly Dictionary<Type, List<IAtemCommand>> _receiveCommandSubscribers = new();

        private readonly List<Action> _onConnectEvent = new();


        public AtemControlInterface(StillImageData imageData)
        {
            this._imageData = imageData;
            PluginLog.Info($"[AtemControlInterface] Trying to connect to ATEM {this._imageData.AtemURI}");

        }


        public void Connect() {



            this.AtemClient = new AtemClient(this._imageData.AtemURI, false);
            
            /*
                        this.AtemClient.OnConnection += (atemClient) =>
                        {
                            PluginLog.Info($"[AtemControlInterface] connected {this._imageData.AtemURI}");
                            this._connected = true;

                        };
            */
            /*    this.AtemClient.OnDisconnect += (atemClient) =>
                {
                    PluginLog.Info($"[AtemControlInterface] disconnected {this._imageData.AtemURI}");
                    this._connected = false;
                };
            */

            this.AtemClient.OnConnection += this.OnConnect;
            this.AtemClient.OnDisconnect += this.OnDisconnect;
            this.AtemClient.OnReceive += this.OnCommand; 



            try
            {
                this.AtemClient.Connect();

            }
            catch (Exception e) {
                PluginLog.Warning($"[AtemControlInterface] connection to {this._imageData.AtemURI} failed because {e}");
            }
        }


        private void OnDisconnect(Object sender)
        {
            PluginLog.Info($"[AtemControlInterface] disconnected {this._imageData.AtemURI}");
            this._connected = false;
        }


        private void OnConnect(Object  sender)
        {
            PluginLog.Info($"[AtemControlInterface] connected {this._imageData.AtemURI}");
            this._connected = true;

            foreach (var cb in  this._onConnectEvent)
            {
                PluginLog.Verbose($"[AtemControlInterface] triggering callback {cb}");
                cb();
            }
        }


        private void OnCommand(Object sender, IReadOnlyList<ICommand> commands)
        {

            foreach (var cmd in commands)
            {
            /*    if (! (cmd is TimeCodeCommand or CameraControlGetCommand or DataTransferCompleteCommand or MacroPropertiesGetCommand))
                {
                    PluginLog.Verbose($"[AtemControlInterface] Received Command {cmd.GetType().Name}");
                }
            */

                if (! this._receiveCommandSubscribers.ContainsKey(cmd.GetType()))
                {
                    continue;
                }

                PluginLog.Verbose($"[AtemControlInterface] Processing Command {cmd.GetType().Name}");

                foreach (var eventReceiver in this._receiveCommandSubscribers[cmd.GetType()])
                {
                    eventReceiver.ReceiveCommand(sender, cmd);

                }
              
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

            PluginLog.Verbose($"[AtemControlInterface] Try SendCommand {command.GetType().Name} ...");

            if (!this._connected)
            {
                PluginLog.Warning($"[AtemControlInterface] not connecterd to ATEM cannot SendCommand");
                return false;
            }

            try
            {

                PluginLog.Verbose($"[AtemControlInterface] issuing SendCommand {command.GetType().Name}");

                this.AtemClient.SendCommand(command);
                
            }
            catch (Exception e)
            {
                PluginLog.Error($"[AtemControlInterface] ERROR ATEM cannot SendCommand \n{e}");
                return false;
            }

            
            return true;
        }

        /*
         * this lets a IAtemCommand implementing class register to an incomming ATEM event
         * */
        public void RegisterStateChangeListener(IAtemCommand atemCommand, List<Type> commands) {

            foreach (var cmd in commands)
            {
                PluginLog.Verbose($"[AtemControlInterface] Trying to register {cmd.Name}");

                if (!this._receiveCommandSubscribers.ContainsKey(cmd))
                {
                    PluginLog.Verbose($"[AtemControlInterface] Creating new list for {cmd.Name}");
                    this._receiveCommandSubscribers[cmd] = new List<IAtemCommand>();

                }
                this._receiveCommandSubscribers[cmd].Add(atemCommand);
            }
        }


        public void RegisterOnConnectCallback(Action connectCallback)
        {
            PluginLog.Verbose($"[AtemControlInterface] registering onConnect callback {connectCallback}");
            this._onConnectEvent.Add(connectCallback);
        }



        public void Dispose()
        {
            this.AtemClient.Dispose();
        }

        public static implicit operator AtemControlInterface(AtemClient v) => throw new NotImplementedException();
    }
}

