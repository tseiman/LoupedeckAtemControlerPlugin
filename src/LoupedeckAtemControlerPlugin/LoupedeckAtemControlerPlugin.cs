
namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;
    using System.Net.Sockets;

    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class LoupedeckAtemControlerPlugin : Plugin
    {
        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;


        public static event Action PluginReady;

      

        // Initializes a new instance of the plugin class.
        public LoupedeckAtemControlerPlugin()
        {
            // Initialize the plugin log.
            PluginLog.Init(this.Log);


            // Initialize the plugin resources.
            PluginResources.Init(this.Assembly);

            ServiceDirectory.Register(new StillImageData(this));
            ServiceDirectory.Register(new BlinkenLightsTimeSource());





        }




        // This method is called when the plugin is loaded.
        public override void Load()
        {

            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);
            stillImageData.LoadData();
            ServiceDirectory.Register(new AtemControlInterface(stillImageData));

            PluginReady?.Invoke();

            ((AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface)).Connect();
        }

        // This method is called when the plugin is unloaded.
        public override void Unload()
        {
            ((AtemControlInterface)ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface)).Dispose();
            ((StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData)).Save();

        }

        



    }
}
