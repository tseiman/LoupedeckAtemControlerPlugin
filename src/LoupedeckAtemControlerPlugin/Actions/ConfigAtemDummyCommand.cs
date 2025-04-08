namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;
    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    public class ConfigAtemDummyCommand : PluginDynamicCommand
    {


        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;


        public ConfigAtemDummyCommand()
               : base(groupName: "Configurations", displayName: "Set ATEM URI", description: "Lets one configure the ATEM IP address or host name")
        {
            this.MakeProfileAction("text;Enter ATEM Name or IP:");
        }


        protected override void RunCommand(String actionParameter)
        {
            this._plugin.SetPluginSetting("AtemURI", actionParameter, false);

            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);

            stillImageData.AtemURI = actionParameter;

            stillImageData.Save();

//            this._plugin.stillImageData.Save();

          //  this._plugin.atemControlInterface.Reconnect();
        }

    }
}
