


namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{

    using System;

    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;

    public class AtemHelpers
    {
        public static Byte[] GetRgbaByteArray(Image<Rgba32> image)
        {
            var width = image.Width;
            var height = image.Height;
            var rgbaBytes = new Byte[width * height * 4];

            image.ProcessPixelRows(accessor =>
            {
                for (var y = 0; y < height; y++)
                {
                    Span<Rgba32> row = accessor.GetRowSpan(y);
                    for (var x = 0; x < width; x++)
                    {
                        var index = (y * width + x) * 4;
                        Rgba32 pixel = row[x];
                        rgbaBytes[index + 0] = pixel.R;
                        rgbaBytes[index + 1] = pixel.G;
                        rgbaBytes[index + 2] = pixel.B;
                        rgbaBytes[index + 3] = pixel.A;
                    }
                }
            });

            return rgbaBytes;
        }

    }
}

