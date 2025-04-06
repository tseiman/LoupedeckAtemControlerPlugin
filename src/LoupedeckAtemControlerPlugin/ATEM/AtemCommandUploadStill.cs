﻿

namespace Loupedeck.LoupedeckAtemControlerPlugin.ATEM
{

    using System;

    using LibAtem.Commands.Media;
    using LibAtem.Common;
    using LibAtem.Net.DataTransfer;
    using LibAtem.Util.Media;

    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp;
    // using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using LibAtem.Net;

    public class AtemCommandUploadStill : IAtemCommand
    {

        private AtemControlInterface _atemControlInterface;





        public async Task<Boolean> SetStillImageAsync(String fileName, UInt32 stillId)
        {



            PluginLog.Verbose($"[AtemCommandUploadStill] Loading filename to ATEM media Slot {fileName}");


            if (this._atemControlInterface == null)
            {
                PluginLog.Error($"[AtemCommandUploadStill] error ATEM controller Interface not initialized");
                return false;
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

                var rgbaBytes = AtemHelpers.GetRgbaByteArray(image);

                var frame = AtemFrame.FromRGBA(Path.GetFileNameWithoutExtension(fileName), rgbaBytes, ColourSpace.BT709); // TODO - colorspace



                var completion = new TaskCompletionSource<bool>();
               
                var job = new UploadMediaStillJob(stillId, frame,
                    (success) =>
                    {
                        PluginLog.Verbose($"[AtemControlInterface] Still upload {stillId} completed with {success}");
                        completion.SetResult(success);
                        // new MediaPlayerSourceSetCommand { Mask = MediaPlayerSourceSetCommand.MaskFlags.StillIndex, Index = (MediaPlayerId)PlayerIndex, StillIndex = StillIndex }

                    });

                PluginLog.Warning($"[AtemControlInterface] Still upload {stillId} queued");




                this._atemControlInterface.QueueDataTransferJob(job);

                // Wait for the upload before returning
                await completion.Task;




            }
            catch (Exception e)
            {
                PluginLog.Warning($"[AtemControlInterface] ERROR talking to ATEM {e}");
                return false;
            }
            return true;
        }

        public void setAtemClient(AtemControlInterface atemControlInterface) => this._atemControlInterface = atemControlInterface;


    }
}

