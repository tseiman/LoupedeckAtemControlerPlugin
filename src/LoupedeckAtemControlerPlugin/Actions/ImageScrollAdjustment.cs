
namespace Loupedeck.LoupedeckAtemControlerPlugin
{


    using System.IO;

    using Loupedeck.LoupedeckAtemControlerPlugin.Helpers;

    using SkiaSharp;


    public class ImageScrollAdjustment : PluginDynamicAdjustment
    {
        private String[] _imageFiles = Array.Empty<String>();
        private Int32 _currentIndex = 0;
        private String _imageFolder;

        protected StillImageData _stillImageData;
        
        public ImageScrollAdjustment(StillImageData stillImageData)
             : base(displayName: "Still Image Select", description: "scrolls through the still images in the still_image folder", groupName: "Adjustments", hasReset: false)
        {
            _stillImageData = stillImageData;
        }
        
        /*
        public ImageScrollAdjustment()
        : base(displayName: "Still Image Select", description: "scrolls through the still images in the still_image folder", groupName: "Adjustments", hasReset: false)
        {      
        }

        */
        public void StillImageData(StillImageData stillImageData) => this._stillImageData = stillImageData;

        protected override Boolean OnLoad()
        {
            /*       var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                   _imageFolder = Path.Combine(appData, "Logi", "LogiPluginService", "PluginData", "LoupedeckAtemControlerPlugin", "still_images");

                   if (!Directory.Exists(_imageFolder))
                       Directory.CreateDirectory(_imageFolder);

                   _imageFiles = Directory.GetFiles(_imageFolder, "*.jpg");
                   this.Log.Info($"[ImageScroll] Found {_imageFiles.Length} images in {_imageFolder}");
            */


            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            this._imageFolder = Path.Combine(appData, "Logi", "LogiPluginService", "PluginData", "LoupedeckAtemControlerPlugin", "still_images");

            if (!Directory.Exists(this._imageFolder))
            {
                Directory.CreateDirectory(this._imageFolder);
            }
                

            var watcher = new FileSystemWatcher();
            watcher.Path = this._imageFolder;
            watcher.NotifyFilter = NotifyFilters.Attributes
                                 | NotifyFilters.CreationTime
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.FileName
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite
                                 | NotifyFilters.Security
                                 | NotifyFilters.Size;

            watcher.Filter = "*.*";

            var fileSystemEventHandler = new FileSystemEventHandler(this.OnChanged);

            watcher.Changed += fileSystemEventHandler;
            watcher.Created += fileSystemEventHandler;
            watcher.Deleted += fileSystemEventHandler;
        //    watcher.Renamed += fileSystemEventHandler;

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;

            this.OnChanged(null,null);

            return true;
        }


        private void OnChanged(Object source, FileSystemEventArgs e) {
            this._imageFiles = Directory.GetFiles(this._imageFolder, "*.jpg");
            this.Log.Info($"[ImageScroll] Found {this._imageFiles.Length} images in {this._imageFolder}");

        }

        protected override void ApplyAdjustment(String actionParameter, Int32 diff)
        {
            if (this._imageFiles.Length == 0)
            {
                this.OnChanged(null, null);
                return;
            }


            this._currentIndex += diff;

            // Wrap-around
            if (this._currentIndex >= this._imageFiles.Length)
            {
                this.OnChanged(null, null);
                this._currentIndex = 0;
            }


            if (this._currentIndex < 0)
            {
                this.OnChanged(null, null);
                this._currentIndex = this._imageFiles.Length - 1;
            }
            

            this.Log.Info($"[ImageScroll] ApplyAdjustment → index: {this._currentIndex}, actionParam: {actionParameter}");

            if (this._stillImageData != null)
            {
                this._stillImageData.ImagePath = this._imageFiles[this._currentIndex];
            }
        //    this._stillImageData.ImagePath = this._imageFiles[this._currentIndex];

            this.ActionImageChanged(null);
            this.ActionImageChanged(actionParameter);
            this.AdjustmentValueChanged(actionParameter);
            
        }

        
        protected override String GetAdjustmentDisplayName(String actionParameter, PluginImageSize imageSize)
        {
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
                    var (width, height) = this.GetSize(imageSize);
                    this.Log.Info($"[ImageScroll] GetAdjustmentImage → size: {width}x{height}, param: {actionParameter}");

                    if (this._imageFiles.Length == 0)
                    {
                        this.Log.Warning("No image files available.");
                        return this.RenderButtonImage("No Images", width, height);
                    }

                    if (this._currentIndex < 0 || this._currentIndex >= this._imageFiles.Length)
                    {
                        this.Log.Warning($"Invalid image index: {this._currentIndex}");
                        return this.RenderButtonImage("Invalid Index", width, height);
                    }

                    var path = this._imageFiles[this._currentIndex];

                    if (!File.Exists(path))
                    {
                        this.Log.Warning($"Image file not found: {path}");
                        return this.RenderButtonImage("Not Found", width, height);
                    }

                    try
                    {
                        using var stream = File.OpenRead(path);
                        using var original = SKBitmap.Decode(stream);

                        using var resized = new SKBitmap(width, height, original.ColorType, original.AlphaType);
                        using var canvas = new SKCanvas(resized);

                        using var paint = new SKPaint
                        {
                            FilterQuality = SKFilterQuality.Medium,
                            IsAntialias = true
                        };

                        var destRect = new SKRect(0, 0, width, height);
                        canvas.DrawBitmap(original, destRect, paint);

                        using var image = SKImage.FromBitmap(resized);
                        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

                        return BitmapImage.FromArray(data.ToArray());
                    }
                    catch (Exception ex)
                    {
                        this.Log.Warning("Image loading failed:\n" + ex.ToString());
                        return this.RenderButtonImage("Error", width, height);
                    }
                }
        

       

        private BitmapImage RenderButtonImage(String text, Int32 width, Int32 height)
        {
            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.DarkSlateBlue);

            using var paint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 24,
                IsAntialias = true
            };

            canvas.DrawText(text, 10, height / 2, paint);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return BitmapImage.FromArray(data.ToArray());
        }

        private (Int32 Width, Int32 Height) GetSize(PluginImageSize size)
        {
            this.Log.Info($"[ImageScroll] GetSize with height" + size.ToString());
            ;

            Int32 w, h;
            switch (size)
            {
                case PluginImageSize.Width60:
                    w = 60;
                    h = 60;
                    break;
                case PluginImageSize.Width90:
                    w = 90;
                    h = 90;
                    break;
                case PluginImageSize.Width116:
                    w = 116;
                    h = 116;
                    break;
                default:
                    w = 80;
                    h = 80;
                    break;
            }

            return (w,h);

        }
    }

 
}