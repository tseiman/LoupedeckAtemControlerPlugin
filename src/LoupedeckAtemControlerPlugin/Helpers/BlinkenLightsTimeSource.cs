

namespace Loupedeck.LoupedeckAtemControlerPlugin.Helpers
{

    using System;

    using LibAtem.Commands;

    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;
    using System.Timers;

    public class BlinkenLightsTimeSource
    {

        private readonly List<IBlinkenLightsReceiver> _blinkenLightsReceivers = new();
        private readonly static Timer _timer = new Timer(1000);
        private Boolean _blinkState;

        public BlinkenLightsTimeSource() {


            _timer.Elapsed += this.OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }


        public void OnTimedEvent(Object source, ElapsedEventArgs e) {
            this._blinkState = !this._blinkState;
            foreach (var blinkenLightsReceiver in this._blinkenLightsReceivers)
            {
                blinkenLightsReceiver.ReceiveTimeThick(this._blinkState);
            }

        }

        public void RegisterBlinkenLightReceiver(IBlinkenLightsReceiver receiver) {
            if (receiver != null) {
                this._blinkenLightsReceivers.Add(receiver);
            }

        }
    }
}

