namespace Loupedeck.LoupedeckAtemControlerPlugin.Helpers
{
    using System;
    using System.IO;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Processing;

    public static class StillImagePreview
    {
        public static BitmapImage Load(String path, PluginImageSize imageSize)
        {
            var requestedWidth = Math.Max(1, imageSize.GetWidth());
            var requestedHeight = Math.Max(1, imageSize.GetHeight());
            var aspectRatio = requestedWidth / (Double)requestedHeight;
            var resizeMode = aspectRatio > 0.85 && aspectRatio < 1.18
                ? ResizeMode.Crop
                : ResizeMode.Pad;
            return Load(path, requestedWidth, requestedHeight, resizeMode);
        }

        public static BitmapImage LoadScaled(String path, PluginImageSize imageSize, Double scale)
        {
            var requestedWidth = Math.Max(1, imageSize.GetWidth());
            var requestedHeight = Math.Max(1, imageSize.GetHeight());
            var width = Math.Max(requestedWidth, (Int32)Math.Round(requestedWidth * scale));
            var height = Math.Max(requestedHeight, (Int32)Math.Round(requestedHeight * scale));
            return Load(path, width, height, ResizeMode.Crop);
        }

        private static BitmapImage Load(String path, Int32 width, Int32 height, ResizeMode resizeMode)
        {
            PluginLog.Verbose($"[StillImagePreview] rendering {width}x{height}, mode {resizeMode}");

            using (var image = Image.Load(path))
            using (var stream = new MemoryStream())
            {
                image.Mutate(context => context.Resize(new ResizeOptions
                {
                    Size = new Size(width, height),
                    Mode = resizeMode,
                    PadColor = Color.Black,
                }));

                image.SaveAsPng(stream);
                return BitmapImage.FromArray(stream.ToArray());
            }
        }
    }
}
