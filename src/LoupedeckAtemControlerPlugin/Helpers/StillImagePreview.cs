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
            var width = Math.Max(1, imageSize.GetWidth());
            var height = Math.Max(1, imageSize.GetHeight());
            var resizeMode = Math.Max(width, height) > 120
                ? ResizeMode.Crop
                : ResizeMode.Pad;

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
