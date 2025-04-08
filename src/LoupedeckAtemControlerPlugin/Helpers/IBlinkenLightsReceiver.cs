using System;
namespace Loupedeck.LoupedeckAtemControlerPlugin.Helpers
{
    public interface IBlinkenLightsReceiver
    {
        public void ReceiveTimeThick(Boolean blinkState);
    }
}

