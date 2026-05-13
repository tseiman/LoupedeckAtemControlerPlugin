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
            var isSquareDisplay = aspectRatio > 0.85 && aspectRatio < 1.18;
            var width = isSquareDisplay ? Math.Max(requestedWidth, 240) : requestedWidth;
            var height = isSquareDisplay ? Math.Max(requestedHeight, 240) : requestedHeight;
            var resizeMode = aspectRatio > 0.85 && aspectRatio < 1.18
                ? ResizeMode.Crop
                : ResizeMode.Pad;
            PluginLog.Verbose($"[StillImagePreview] requested {requestedWidth}x{requestedHeight}, rendering {width}x{height}, mode {resizeMode}");

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
