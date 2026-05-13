namespace Loupedeck.LoupedeckAtemControlerPlugin.Helpers
{
    using System;

    public static class StillImageChangedEvent
    {
        public static event Action OnChanged;

        public static void Raise() => OnChanged?.Invoke();
    }
}
