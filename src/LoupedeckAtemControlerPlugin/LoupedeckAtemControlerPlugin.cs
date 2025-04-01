
namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    // This class contains the plugin-level logic of the Loupedeck plugin.

    public class LoupedeckAtemControlerPlugin : Plugin
    {
        // Gets a value indicating whether this is an API-only plugin.
        public override Boolean UsesApplicationApiOnly => true;

        // Gets a value indicating whether this is a Universal plugin or an Application plugin.
        public override Boolean HasNoApplication => true;


        private readonly StillImageData _stillImageData = new();

        // Initializes a new instance of the plugin class.
        public LoupedeckAtemControlerPlugin()
        {
            // Initialize the plugin log.
            PluginLog.Init(this.Log);

            // Initialize the plugin resources.
            PluginResources.Init(this.Assembly);



        }


   

        // This method is called when the plugin is loaded.
        public override void Load()
        {
            Environment.SetEnvironmentVariable("DOTNET_SYSTEM_DRAWING_ENABLE_UNIX_SUPPORT", "1", EnvironmentVariableTarget.Process);

            //   this.AddAdjustment.Add(new Loupedeck.LoupedeckAtemControlerPlugin.Adjustments.ImageDialAdjustment());
            // this.Actions.Add(new Loupedeck.LoupedeckAtemControlerPlugin.Adjustments.ImageScrollAdjustment());
            this.AddDynamicAction(new ImageScrollAdjustment(this._stillImageData));
           this.AddDynamicAction(new SetStillImageCommand(this._stillImageData));

       /*     var imageScroll = new ImageScrollAdjustment();
            imageScroll.StillImageData(this._stillImageData);
            var stillImageCommand = new SetStillImageCommand();
            stillImageCommand.StillImageData(this._stillImageData);

            this.AddAction(imageScroll, true);
            this.AddAction(stillImageCommand, true);
       */
       
        }

        // This method is called when the plugin is unloaded.
        public override void Unload()
        {
        }
    }
}
