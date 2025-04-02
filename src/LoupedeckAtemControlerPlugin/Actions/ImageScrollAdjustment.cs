
namespace Loupedeck.LoupedeckAtemControlerPlugin
{


    using System.IO;
    using System.Xml.Linq;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    using SkiaSharp;


   
   public class ImageScrollAdjustment : PluginDynamicAdjustment
    {
        private String[] _imageFiles = Array.Empty<String>();
        private Int32 _currentIndex = 0;
        private String _imageFolder;

        protected StillImageData _stillImageData;

        private FileSystemWatcher _fsWatcher;

        public ImageScrollAdjustment(StillImageData stillImageData)
               : base(displayName: "Still Image Select", description: "scrolls through the still images in the still_image folder", groupName: "Adjustments", hasReset: false)
        {
            this.MakeProfileAction("text;Enter Folder to find JPEG Images:");
            this._stillImageData = stillImageData;

        }

        protected override Boolean OnLoad()
        {
            this._imageFolder = null; 
            return true;
        }


        private void OnChanged(Object source, FileSystemEventArgs e) {
            this._imageFiles = Directory.GetFiles(this._imageFolder, "*.jpg").Union(Directory.GetFiles(this._imageFolder, "*.jpeg")).ToArray();
            this.Log.Info($"[ImageScroll] Found {this._imageFiles.Length} images in {this._imageFolder}");

        }


        private void setupFsWatcher() {

            this._fsWatcher = new FileSystemWatcher();
            this._fsWatcher.Path = this._imageFolder;
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


            if (this._imageFolder == null)
            {
                this._imageFolder = actionParameter;
                this.setupFsWatcher();
            }
            else
            {
                this._imageFolder = actionParameter;
            }



            if (this._imageFiles.Length == 0)
            {
                return;
            }
       

            this._currentIndex += diff;

            // Wrap-around
            if (this._currentIndex >= this._imageFiles.Length)
            {
                this._currentIndex = 0;
            }


            if (this._currentIndex < 0)
            {
                this._currentIndex = this._imageFiles.Length - 1;
            }
            

            this.Log.Info($"[ImageScroll] ApplyAdjustment → index: {this._currentIndex}, actionParam: {actionParameter}");

            if (this._stillImageData != null)
            {
                this._stillImageData.ImagePath = this._imageFiles[this._currentIndex];
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


            return BitmapImage.FromFile(path);


         
        }
        

       
       
    }

 
}