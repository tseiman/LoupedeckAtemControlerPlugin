
namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;

    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class LoupedeckAtemControlerPlugin : Plugin
    {
        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;


        public readonly StillImageData stillImageData;// = new(this.Plugin);

        public AtemControlInterface atemControlInterface;

        public static event Action PluginReady;


        // Initializes a new instance of the plugin class.
        public LoupedeckAtemControlerPlugin()
        {
            // Initialize the plugin log.
            PluginLog.Init(this.Log);

            // Initialize the plugin resources.
            PluginResources.Init(this.Assembly);

            this.stillImageData = new StillImageData(this);

        }


   

        // This method is called when the plugin is loaded.
        public override void Load()
        {
            //       Environment.SetEnvironmentVariable("DOTNET_SYSTEM_DRAWING_ENABLE_UNIX_SUPPORT", "1", EnvironmentVariableTarget.Process);
                      
                    // this.AddDynamicAction(new SetStillImageCommand(ata));

            this.stillImageData.LoadData();
            this.atemControlInterface = new AtemControlInterface(this.stillImageData);

            //   this.AddDynamicAction(new ImageScrollAdjustment(true));

            PluginReady?.Invoke();

            this.atemControlInterface.Connect();

        }

        // This method is called when the plugin is unloaded.
        public override void Unload()
        {
            this.atemControlInterface.Dispose();
            this.stillImageData.Save();

        }

        public Object initAtemCommand(IAtemCommand command) {

            PluginLog.Verbose($"[LoupedeckAtemControlerPlugin] initialize ATEM command {command.GetType()}");

            if (this.atemControlInterface == null)
            {
                PluginLog.Error($"[LoupedeckAtemControlerPlugin] AtemControlInterface not initialized !");
                return null;
            }

            command.SetAtemClient(this.atemControlInterface);
            return command;
        }




    }
}
