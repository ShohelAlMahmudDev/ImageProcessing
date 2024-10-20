using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace FastImageCombine.Helpers
{
    public static class ImageConversionHelper
    {
      
        public static BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {
            //In memory readwrite for performance and best quality image processing 
            using MemoryStream memoryStream = new();
            bitmap.Save(memoryStream, ImageFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);

            PngBitmapDecoder decoder = new(
                memoryStream,
                BitmapCreateOptions.None,
                BitmapCacheOption.OnLoad);

            return decoder.Frames[0];
        }

        public static Bitmap BitmapSourceToBitmap(BitmapSource bitmapSource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new ())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(outStream);

                bitmap = new Bitmap(outStream);
            }

            return bitmap;
        }

        public static ImageFormat GetImageFormatByExtension(string filePath)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".jpg" or ".jpeg" => ImageFormat.Jpeg,
                ".png" => ImageFormat.Png,
                ".gif" => ImageFormat.Gif,
                _ => ImageFormat.Bmp,
            };
        }
    }
}
