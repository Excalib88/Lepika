using System;
using System.IO;

namespace Grand.Web.Extensions
{
    public static class PictureExtensions
    {
        public static byte[] ToBytes(this string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var bytes = File.ReadAllBytes(path);
                fs.Read(bytes, 0, Convert.ToInt32(fs.Length));

                return bytes; 
            }
        }

        public static string ToMimeType(this string fileExtension)
        {
            var contentType = "";
            
            switch (fileExtension)
            {
                case ".bmp":
                    contentType = "image/bmp";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                case ".jpeg":
                case ".jpg":
                case ".jpe":
                case ".jfif":
                case ".pjpeg":
                case ".pjp":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".tiff":
                case ".tif":
                    contentType = "image/tiff";
                    break;
                default:
                    break;
            }

            return contentType;
        }
            
    }
}