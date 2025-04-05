
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
               : base(false)
        {
            base.GroupName = "Adjustments";
            base.DisplayName = "Still Image Select";
            base.Description = "scrolls through the still images in the still_image folder";

            this.MakeProfileAction("text;Enter Folder to find JPEG Images:");
        }

        protected override Boolean OnLoad()
        {
            if (!this._plugin.stillImageData.ImagePath.Equals(""))
            {
                this.SetupFsWatcher();
                this.OnChanged(null, null);
                this.Log.Info($"[ImageScroll] Loading images from stored config path");
                this.ActionImageChanged();
            }
                  


                    // this._plugin.stillImageData.ImagePath = null;


            return true;
        }


        private void OnChanged(Object source, FileSystemEventArgs e)
        {
            this._imageFiles = Directory.GetFiles(this._plugin.stillImageData.ImagePath, "*.jpg").Union(Directory.GetFiles(this._plugin.stillImageData.ImagePath, "*.jpeg")).ToArray();
            this.Log.Info($"[ImageScroll] Found {this._imageFiles.Length} images in {this._plugin.stillImageData.ImagePath}");

        }


        private void SetupFsWatcher()
        {

            this._fsWatcher = new FileSystemWatcher();
            this._fsWatcher.Path = this._plugin.stillImageData.ImagePath;
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


            if (this._plugin.stillImageData.ImagePath == null || (!this._plugin.stillImageData.ImagePath.Equals(actionParameter)))
            {
                this._fsWatcher?.Dispose();
                                
                this._plugin.stillImageData.ImagePath = actionParameter;
                this.SetupFsWatcher();
                this.OnChanged(null, null);
            }
            else
            {
                this._plugin.stillImageData.ImagePath = actionParameter;
            }

            if (this._imageFiles.Length == 0)
            {
                return;
            }


            this._currentIndex += diff;


            if (this._currentIndex >= this._imageFiles.Length)
            {
                this._currentIndex = this._imageFiles.Length;
            }


            if (this._currentIndex < 0)
            {
                this._currentIndex = 0;
            }


            this.Log.Info($"[ImageScroll] ApplyAdjustment → index: {this._currentIndex}, actionParam: {actionParameter}");

            if (this._plugin.stillImageData != null)
            {
                this._plugin.stillImageData.ImagePath = this._imageFiles[this._currentIndex];
            }

            this.ActionImageChanged();

        }


        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {


            var path = this._imageFiles[this._currentIndex];

            if (!File.Exists(path))
            {
                this.Log.Warning($"Image file not found: {path}");
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
            var path = this._imageFiles[this._currentIndex];

            using (var bitmapBuilder = new BitmapBuilder(imageSize))
            {
                bitmapBuilder.SetBackgroundImage(BitmapImage.FromFile(path));
                return bitmapBuilder.ToImage();
            }
         
        }



    }


}