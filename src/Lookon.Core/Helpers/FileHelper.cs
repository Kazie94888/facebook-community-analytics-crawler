using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace LookOn.Core.Helpers
{
    public class FileHelper
    {
        public static void SaveByteArrayAsImage(string fullOutputPath, string base64String)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64String);

                Image image;
                using (var ms = new MemoryStream(bytes))
                {
                    image = Image.FromStream(ms);
                }

                image.Save(fullOutputPath, ImageFormat.Jpeg);
            }
            catch (Exception)
            {
                // ignore
            }
        }
    }
}