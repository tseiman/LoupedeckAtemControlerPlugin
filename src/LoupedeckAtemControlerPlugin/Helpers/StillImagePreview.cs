namespace Loupedeck.LoupedeckAtemControlerPlugin.Helpers
{
    using System;
    using System.IO;

    using SixLabors.Fonts;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.Drawing.Processing;
    using SixLabors.ImageSharp.PixelFormats;
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
            return LoadScaled(path, imageSize, scale, null);
        }

        public static BitmapImage LoadScaledWithText(String path, PluginImageSize imageSize, Double scale, String text)
        {
            return LoadScaled(path, imageSize, scale, text);
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

        private static BitmapImage LoadScaled(String path, PluginImageSize imageSize, Double scale, String text)
        {
            using (var original = Image.Load<Rgba32>(path))
            using (var stream = new MemoryStream())
            {
                var requestedWidth = Math.Max(1, imageSize.GetWidth());
                var requestedHeight = Math.Max(1, imageSize.GetHeight());
                var sourceAspectRatio = original.Width / (Double)original.Height;
                var width = Math.Max(1, (Int32)Math.Round(requestedWidth * scale));
                var height = Math.Max(1, (Int32)Math.Round(width / sourceAspectRatio));

                if (height < requestedHeight * scale * 0.5)
                {
                    height = Math.Max(1, (Int32)Math.Round(requestedHeight * scale));
                    width = Math.Max(1, (Int32)Math.Round(height * sourceAspectRatio));
                }

                PluginLog.Verbose($"[StillImagePreview] rendering original-scaled {width}x{height}, scale {scale}");
                original.Mutate(context => context.Resize(width, height));

                if (!String.IsNullOrEmpty(text))
                {
                    DrawTextOverlay(original, text);
                }

                original.SaveAsPng(stream);
                return BitmapImage.FromArray(stream.ToArray());
            }
        }

        private static void DrawTextOverlay(Image<Rgba32> image, String text)
        {
            var overlayHeight = Math.Max(42, image.Height / 5);
            image.Mutate(context =>
            {
                context.Fill(
                    new Color(new Rgba32(0, 0, 0, 180)),
                    new RectangleF(0, image.Height - overlayHeight, image.Width, overlayHeight));

                var fontSize = Math.Max(18, overlayHeight / 2);
                var point = new PointF(Math.Max(8, image.Width / 24), image.Height - overlayHeight + Math.Max(6, overlayHeight / 6));
                context.DrawText(text.Replace("\n", " "), SystemFonts.CreateFont("Arial", fontSize), Color.White, point);
            });
        }
    }
}
