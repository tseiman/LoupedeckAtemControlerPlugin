
namespace Loupedeck.LoupedeckAtemControlerPlugin.Helpers
{
    using System;

    public class StillImageData
    {



        private String ImagePath { get; set; }  = "";
        private String AtemURI { get; set; } = "";


        private readonly Plugin _plugin;


        public StillImageData(Plugin plugin)
        {
            this._plugin = plugin;

            PluginLog.Info($"[StillImageData] Loading data for StillImage");

            if (this._plugin.TryGetPluginSetting("AtemURI", out var atemURI))
            {
                PluginLog.Info($"[StillImageData] Loading config AtemURI: <{atemURI}>");
                this.AtemURI = atemURI;
            }
            else
            {
                PluginLog.Warning($"[StillImageData] NOT Loading config AtemURI");
            }


            if (this._plugin.TryGetPluginSetting("ImagePath", out var imagePath))
            {
                this.ImagePath = imagePath;
                PluginLog.Info($"[StillImageData] Loading config ImagePath: <{imagePath}>");
            }
            else
            {
                PluginLog.Warning($"[StillImageData] NOT Loading config ImagePath");
            }

        }



        public void Dispose()
        {

            if (!this.AtemURI.Equals(""))
            {
                this._plugin.SetPluginSetting("AtemURI", this.AtemURI, false);
                PluginLog.Info($"[StillImageData] Storing config AtemURI: <{this.AtemURI}>");

            }

            if (!this.ImagePath.Equals(""))
            {
                this._plugin.SetPluginSetting("ImagePath", this.ImagePath, false);
                PluginLog.Info($"[StillImageData] Storing config ImagePath: <{this.ImagePath}>");
            }
        }


    } // end class

} // end NS

