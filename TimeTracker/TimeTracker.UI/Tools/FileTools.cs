using System.IO;
using System.Web;

namespace TimeTracker.UI.Tools
{
    public class FileTools
    {
        public static byte[] ConvertImageToBytes(HttpPostedFileBase postedFile)
        {
            if (postedFile != null)
            {
                byte[] imageBuffer = new byte[postedFile.ContentLength];
                Stream imageStream = postedFile.InputStream;
                imageStream.Read(imageBuffer, 0, imageBuffer.Length);
                return imageBuffer;
            }

            return null;
        }
    }
}