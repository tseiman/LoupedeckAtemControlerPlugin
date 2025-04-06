
namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{


    using System;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;
    using LibAtem.Net;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using LibAtem.Common;
    using SixLabors.ImageSharp.Processing;
    using LibAtem.Util.Media;
    using System.Runtime.InteropServices;
    using LibAtem.Net.DataTransfer;
    using LibAtem.Commands.Media;

    public class AtemControlInterface
    {

        private readonly StillImageData _imageData;

        private Boolean _connected = false;

        private AtemClient _atemClient;

        public AtemControlInterface(StillImageData imageData)
        {
            this._imageData = imageData;
            PluginLog.Info($"[AtemControlInterface] Trying to connect to ATEM {this._imageData.AtemURI}");

            this.Connect();

        }






        public void Connect() {



            this._atemClient = new AtemClient(this._imageData.AtemURI, true);


            this._atemClient.OnConnection += (atemClient) =>
            {
                PluginLog.Info($"[AtemControlInterface] connected {this._imageData.AtemURI}");
                this._connected = true;
            };

            this._atemClient.OnDisconnect += (atemClient) =>
            {
                PluginLog.Info($"[AtemControlInterface] disconnected {this._imageData.AtemURI}");
                this._connected = false;
            };

            try
            {
                this._atemClient.Connect();

            }
            catch (Exception e) {
                PluginLog.Warning($"[AtemControlInterface] connection to {this._imageData.AtemURI} failed because {e}");
            }
        }




        public static byte[] GetRgbaByteArray(Image<Rgba32> image)
        {
            int width = image.Width;
            int height = image.Height;
            byte[] rgbaBytes = new byte[width * height * 4];

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < height; y++)
                {
                    Span<Rgba32> row = accessor.GetRowSpan(y);
                    for (int x = 0; x < width; x++)
                    {
                        int index = (y * width + x) * 4;
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




        public async Task<Boolean> setStillImageAsync(String fileName) {



            PluginLog.Verbose($"[AtemControlInterface] Loading filename to ATEM media Slot {fileName}");

            if (!this._connected)
            {
                PluginLog.Warning($"[AtemControlInterface] not connecterd to ATEM cannot upload {fileName}");
            }

            try
            {

                var resolution = VideoModeResolution._1080;
                var resolutionSize = resolution.GetSize();

                using var image = Image.Load<Rgba32>(fileName);

                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size((Int32)resolutionSize.Item1, (Int32)resolutionSize.Item2),
                    Mode = ResizeMode.Pad
                }));

                //    byte[] rgbaBytes = GetBytes(image);

                var rgbaBytes =  GetRgbaByteArray(image);

                var frame = AtemFrame.FromRGBA("LoupdeckImage", rgbaBytes, ColourSpace.BT709); // TODO - colorspace



                var completion = new TaskCompletionSource<bool>();
                UInt32 stillId = 19;
                var job = new UploadMediaStillJob(stillId, frame,
                    (success) =>
                    {
                        Console.WriteLine("Still upload {0} completed with {1}", stillId, success);
                        completion.SetResult(success);
                    });

                PluginLog.Warning($"[AtemControlInterface] \"Still upload {stillId} queued");




                this._atemClient.DataTransfer.QueueJob(job);

                // Wait for the upload before returning
                await completion.Task;

                //   this._atemClient.


                var playClip = new MediaPoolClipDescriptionCommand
                {
                    Index = stillId,
                    IsUsed = true,
                    Name = "LoupdeckImage",
                    FrameCount = 1
                };




            }
            catch (Exception e) {
                PluginLog.Warning($"[AtemControlInterface] ERROR talking to ATEM {e}");
                return false;
            }
            return true;
        }

        public void Dispose()
        {
            this._atemClient.Dispose();
        }





    }
}

