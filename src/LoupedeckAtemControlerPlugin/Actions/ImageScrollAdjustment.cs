
namespace Loupedeck.LoupedeckAtemControlerPlugin
{


    using System.IO;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    using Loupedeck.Devices.Loupedeck2Devices;


    public class ImageScrollAdjustment : PluginDynamicAdjustment
    {
        private String[] _imageFiles = Array.Empty<String>();
        private Int32 _currentIndex = 0;
   //     private String _imageFolder;


        private FileSystemWatcher _fsWatcher;


        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;

        public ImageScrollAdjustment()
               : base(groupName: "Still Image Selection", displayName: "Still Image Select", description: "scrolls through the still images in the still_image folder", hasReset: false)
        {

            this.IsWidget = true;

            LoupedeckAtemControlerPlugin.PluginReady += this.OnPluginReady;

            this.MakeProfileAction("text;Enter Folder to find JPEG Images:");
        }




        private void OnPluginReady()
        {

            PluginLog.Verbose($"[ImageScroll] Initializing ...");
            AtemVisuals.RegisterConnectionRefresh(this.RefreshAdjustmentDisplay);

            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);

            if (!stillImageData.ImagePath.Equals(""))
            {

                this.SetupFsWatcher();
                this.OnChanged(null, null);
                PluginLog.Verbose($"[ImageScroll] Loading images from stored config path {stillImageData.ImagePath}");
                if (this._imageFiles.Length > 0)
                {
                    this._currentIndex = Math.Clamp(this._currentIndex, 0, this._imageFiles.Length - 1);
                    stillImageData.ActualFullImagePath = this._imageFiles[this._currentIndex];
                }
                this.RefreshAdjustmentDisplay();
            }



        }



        private void OnChanged(Object source, FileSystemEventArgs e)
        {
            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);
            this._imageFiles = Directory.GetFiles(stillImageData.ImagePath, "*.jpg").Union(Directory.GetFiles(stillImageData.ImagePath, "*.jpeg")).ToArray();
            PluginLog.Verbose($"[ImageScroll] Found {this._imageFiles.Length} images in {stillImageData.ImagePath}");

        }


        private void SetupFsWatcher()
        {

            PluginLog.Verbose($"[ImageScroll] Setting up new FSWatcher s {String.Join(", ", this._plugin.ListPluginSettings())}");
            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);

            this._fsWatcher = new FileSystemWatcher();
            this._fsWatcher.Path = stillImageData.ImagePath;
            this._fsWatcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            this._fsWatcher.Filter = "*.*";

            var fileSystemEventHandler = new FileSystemEventHandler(this.OnChanged);
            var fileRenamedSystemEventHandler = new RenamedEventHandler(this.OnChanged);

            this._fsWatcher.Changed += fileSystemEventHandler;
            this._fsWatcher.Created += fileSystemEventHandler;
            this._fsWatcher.Deleted += fileSystemEventHandler;
            this._fsWatcher.Renamed += fileRenamedSystemEventHandler;

            this._fsWatcher.IncludeSubdirectories = true;
            this._fsWatcher.EnableRaisingEvents = true;

        }



        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            if (!AtemVisuals.IsAtemConnected())
            {
                return;
            }

            //    PluginLog.Verbose($"[ImageScroll] >>>>>>>>> PLUGIn Settings {String.Join(", ", this._plugin.ListPluginSettings())}");

            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);

            if (!stillImageData.ImagePath.Equals(actionParameter) || this._imageFiles == null || this._imageFiles.Length == 0)
            {

                PluginLog.Verbose($"[ImageScroll]  ImagePath not equal actionParam setting with with {actionParameter}");

                this._fsWatcher?.Dispose();
                                
                stillImageData.ImagePath = actionParameter;
                this.SetupFsWatcher();
                this.OnChanged(null, null);
                stillImageData.Save();
            }
           /* else
            {
                PluginLog.Info($"[ImageScroll]  overwrite ImagePath  with {actionParameter}");
                this._plugin.stillImageData.ImagePath = actionParameter;
                this._plugin.stillImageData.Save();
            }
           */
            if (this._imageFiles.Length == 0)
            {
                return;
            }


            this._currentIndex += diff;


            if (this._currentIndex >= this._imageFiles.Length)
            {
                this._currentIndex = this._imageFiles.Length - 1;
            }


            if (this._currentIndex < 0)
            {
                this._currentIndex = 0;
            }


            PluginLog.Verbose($"[ImageScroll] ApplyAdjustment → index: {this._currentIndex}, actionParam: {actionParameter}");

            if (stillImageData != null)
            {
                stillImageData.ActualFullImagePath = this._imageFiles[this._currentIndex];

        //        this._plugin.stillImageData.ImagePath = actionParameter;

            }

            this.RefreshAdjustmentDisplay();

        }


        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            if (this._imageFiles == null || this._imageFiles.Length == 0)
            {
                return "No Images";
            }


            var path = this._imageFiles[this._currentIndex];

            if (!File.Exists(path))
            {
                PluginLog.Warning($"Image file not found: {path}");
                return "Img not Found";
            }


            return this._imageFiles.Length > 0
                ? Path.GetFileName(this._imageFiles[this._currentIndex])
                : "No Images";
        }

        protected override String GetAdjustmentValue(String actionParameter)
        {
            return this._imageFiles.Length > 0
                ? $"{this._currentIndex + 1}/{this._imageFiles.Length}"
                : "0/0";
        }





        protected override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize)
        {
            if (this._imageFiles == null || this._imageFiles.Length == 0)
            {
                using (var bitmapBuilder = new BitmapBuilder(imageSize))
                {
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                    AtemVisuals.DrawText(bitmapBuilder, "No\nImages");
                    return bitmapBuilder.ToImage();
                }
            }

            this._currentIndex = Math.Clamp(this._currentIndex, 0, this._imageFiles.Length - 1);
            var path = this._imageFiles[this._currentIndex];

            if (!File.Exists(path))
            {
                using (var bitmapBuilder = new BitmapBuilder(imageSize))
                {
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                    AtemVisuals.DrawText(bitmapBuilder, "Img not\nFound");
                    return bitmapBuilder.ToImage();
                }
            }

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                try
                {
                    PluginLog.Verbose($"[ImageScroll] Rendering image preview {imageSize.GetWidth()}x{imageSize.GetHeight()} from {path}");
                    bitmapBuilder.SetBackgroundImage(StillImagePreview.Load(path, imageSize));
                }
                catch (Exception e)
                {
                    PluginLog.Warning($"[ImageScroll] Could not render image preview {path}: {e}");
                    AtemVisuals.FillBackground(bitmapBuilder, imageSize, BitmapColor.Black);
                    AtemVisuals.DrawText(bitmapBuilder, "Img\nError");
                }

                AtemVisuals.ApplyOfflineOverlay(bitmapBuilder, imageSize);
                return bitmapBuilder.ToImage();
            }
         
        }

        private void RefreshAdjustmentDisplay()
        {
            this.ActionImageChanged();
            this.AdjustmentValueChanged("");
        }



    }


}
