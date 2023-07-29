using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LookOn.Core.Helpers
{
    public static class ImageHelper
    {
        public static byte[] ResizeImage(byte[] imgBytes, int width = 150)
        {
            try
            {
                using var inputMemoryStream   = new MemoryStream(imgBytes);
                using var originalImage       = Image.FromStream(inputMemoryStream, false, false);
                var       originalImageFormat = originalImage.RawFormat;
                var       originalImageWidth  = originalImage.Width;
                var       originalImageHeight = originalImage.Height;
                var       resizedImageWidth   = width; // Type here the width you want
                var       resizedImageHeight  = Convert.ToInt32(resizedImageWidth * originalImageHeight / originalImageWidth);
                using var bitmapResized       = new Bitmap(originalImage, resizedImageWidth, resizedImageHeight);
                using var streamResized       = new MemoryStream();
                bitmapResized.Save(streamResized, originalImageFormat);
                var resizedImage = streamResized.ToArray();

                return resizedImage;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}