namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{
    
    using LibAtem.Net;

    public interface IAtemCommand
    {
        public void setAtemClient(AtemControlInterface atemControlInterface);

    }
}

