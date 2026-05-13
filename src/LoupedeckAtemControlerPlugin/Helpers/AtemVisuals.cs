namespace Loupedeck.LoupedeckAtemControlerPlugin.Helpers
{
    using Loupedeck.LoupedeckAtemControlerPlugin.ATEM;

    public static class AtemVisuals
    {
        private static readonly BitmapColor OfflineBackground = new(0x40, 0x40, 0x40);
        private static readonly BitmapColor OfflineText = new(0x00, 0x00, 0x00);

        public static Boolean IsAtemConnected()
        {
            var atemControlInterface = ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface) as AtemControlInterface;
            return atemControlInterface?.IsConnected == true;
        }

        public static void RegisterConnectionRefresh(Action refresh)
        {
            var atemControlInterface = ServiceDirectory.Get(ServiceDirectory.T_AtemControlInterface) as AtemControlInterface;
            if (atemControlInterface == null)
            {
                return;
            }

            atemControlInterface.ConnectionStateChanged += _ => refresh();
        }

        public static void FillBackground(BitmapBuilder bitmapBuilder, PluginImageSize imageSize, BitmapColor onlineColor)
        {
            var color = IsAtemConnected() ? onlineColor : OfflineBackground;
            bitmapBuilder.FillRectangle(0, 0, imageSize.GetWidth(), imageSize.GetHeight(), color);
        }

        public static void ApplyOfflineOverlay(BitmapBuilder bitmapBuilder, PluginImageSize imageSize)
        {
            if (IsAtemConnected())
            {
                return;
            }

            for (var y = 0; y < imageSize.GetHeight(); y += 2)
            {
                bitmapBuilder.FillRectangle(0, y, imageSize.GetWidth(), 1, OfflineBackground);
            }
        }

        public static void DrawText(BitmapBuilder bitmapBuilder, String text)
        {
            if (IsAtemConnected())
            {
                bitmapBuilder.DrawText(text);
                return;
            }

            bitmapBuilder.DrawText(text, color: OfflineText);
        }
    }
}
