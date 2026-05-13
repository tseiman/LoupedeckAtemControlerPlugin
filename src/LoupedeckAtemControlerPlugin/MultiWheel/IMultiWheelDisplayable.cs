namespace Loupedeck.LoupedeckAtemControlerPlugin.MultiWheel
{
    public interface IMultiWheelDisplayable
    {
        public event Action DisplayChanged;

        public String DisplayName { get; }

        public Int32 DisplayPercent { get; }
    }
}
