using System;

using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    public class AtemControlInterface
    {

        private readonly StillImageData _imageData;

        public AtemControlInterface(StillImageData imageData)
        {
            this._imageData = imageData;
        }

        public Boolean setStillImage(String uri, String fileName) {
            PluginLog.Info($">>>> [AtemControlInterface.setStillImage()] Sending Image <{fileName}> to <{uri}>");
            return true;
        }
    }
}

