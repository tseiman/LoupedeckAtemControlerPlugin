
namespace Loupedeck.LoupedeckAtemControlerPlugin.Helpers
{
    using System;

    public class StillImageData
    {



        public String ImagePath { get; set; }  = "";
        public String ActualFullImagePath { get; set; } = "";
        public String AtemURI { get; set; } = "";


        private readonly Plugin _plugin;


        public StillImageData(Plugin plugin)
        {
            this._plugin = plugin;

        }

        public void LoadData() {
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


            if (this._plugin.TryGetPluginSetting("ActualFullImagePath", out var actualFullImagePath))
            {
                this.ActualFullImagePath = actualFullImagePath;
                PluginLog.Info($"[StillImageData] Loading config ImagePath: <{actualFullImagePath}>");
            }
            else
            {
                PluginLog.Warning($"[StillImageData] NOT Loading config ActualFullImagePath");
            }


        }



        public void Save()
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
       

            if (!this.ActualFullImagePath.Equals(""))
            {
                this._plugin.SetPluginSetting("ActualFullImagePath", this.ActualFullImagePath, false);
                PluginLog.Info($"[StillImageData] Storing config ActualFullImagePath: <{this.ActualFullImagePath}>");
            }

        }


    } // end class

} // end NS

