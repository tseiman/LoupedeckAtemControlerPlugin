namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System;


    public class ConfigAtemDummyCommand : PluginDynamicCommand
    {


        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;


        public ConfigAtemDummyCommand()
               : base(displayName: "Set ATEM URI", description: "Lets one configure the ATEM IP address or host name", groupName: "Commands")
                    => this.MakeProfileAction("text;Enter ATEM Name or IP:");



        protected override void RunCommand(String actionParameter)
        {
            this._plugin.SetPluginSetting("AtemURI", actionParameter, false);
            this._plugin.stillImageData.AtemURI = actionParameter;

            this._plugin.stillImageData.Save();

          //  this._plugin.atemControlInterface.Reconnect();
        }

    }
}
