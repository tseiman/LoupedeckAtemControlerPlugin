namespace Loupedeck.LoupedeckAtemControlerPlugin
{
    using System.IO;
    using System.Reflection;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    using Loupedeck.Devices.Loupedeck2Devices;


    public class ImageScrollAdjustment : PluginDynamicAdjustment
    {
        private String[] _imageFiles = Array.Empty<String>();
        private Int32 _currentIndex = 0;
        private String _lastActionParameter = "";

        private FileSystemWatcher _fsWatcher;

        private LoupedeckAtemControlerPlugin _plugin => (LoupedeckAtemControlerPlugin)this.Plugin;

        public ImageScrollAdjustment()
               : base(groupName: "Still Image Selection", displayName: "Still Image Select", description: "scrolls through the still images in the still_image folder", hasReset: false)
        {
            this.IsWidget = true;
            this.GroupName = "Still Image Selection";
            this.DisplayName = "Still Image Select";
            this.Description = "scrolls through the still images in the still_image folder";

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
                this.RefreshAdjustmentDisplay(stillImageData.ImagePath);
            }
        }


        private void OnChanged(Object source, FileSystemEventArgs e)
        {
            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);
            this._imageFiles = Directory.GetFiles(stillImageData.ImagePath, "*.jpg")
                                        .Union(Directory.GetFiles(stillImageData.ImagePath, "*.jpeg"))
                                        .ToArray();
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
            this._lastActionParameter = actionParameter ?? "";

            if (!AtemVisuals.IsAtemConnected())
            {
                return;
            }

            var stillImageData = (StillImageData)ServiceDirectory.Get(ServiceDirectory.T_StillImageData);

            if (!stillImageData.ImagePath.Equals(actionParameter) || this._imageFiles == null || this._imageFiles.Length == 0)
            {
                PluginLog.Verbose($"[ImageScroll]  ImagePath not equal actionParam setting with {actionParameter}");

                this._fsWatcher?.Dispose();

                stillImageData.ImagePath = actionParameter;
                this.SetupFsWatcher();
                this.OnChanged(null, null);
                stillImageData.Save();
            }

            if (this._imageFiles.Length == 0)
            {
                this.RefreshAdjustmentDisplay(actionParameter);
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

                // Notify SetStillImageCommand that the selected image changed so its
                // MultiWheel / button display also redraws immediately.
                StillImageChangedEvent.Raise();
            }

            this.RefreshAdjustmentDisplay(actionParameter);
        }


        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {
            this._lastActionParameter = actionParameter ?? "";

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

            return Path.GetFileName(this._imageFiles[this._currentIndex]);
        }


        protected override String GetAdjustmentValue(String actionParameter)
        {
            this._lastActionParameter = actionParameter ?? "";

            return this._imageFiles.Length > 0
                ? $"{this._currentIndex + 1}/{this._imageFiles.Length}"
                : "0/0";
        }


        protected override BitmapImage GetAdjustmentImage(String actionParameter, PluginImageSize imageSize)
        {
            this._lastActionParameter = actionParameter ?? "";

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


        // Parameterless overload used by AtemVisuals connection-refresh callback.
        private void RefreshAdjustmentDisplay() => this.RefreshAdjustmentDisplay(this._lastActionParameter);

        private void RefreshAdjustmentDisplay(String actionParameter)
        {
            var parameter = actionParameter ?? "";

            // AdjustmentValueChanged(parameter) tells the Plugin Service that the
            // value text (shown on dial strips) has changed for this specific parameter.
            this.AdjustmentValueChanged(parameter);

            // ActionImageChanged(parameter) is the key call for the MultiWheel central
            // display: it tells the Plugin Service to re-call GetAdjustmentImage() for
            // this parameter and push the new bitmap to the wheel screen.
            // Calling without a parameter refreshes ALL currently visible instances.
            this.ActionImageChanged(parameter);
            this.ActionImageChanged();
        }
    }
}

