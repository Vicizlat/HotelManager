using System.IO;
using System.Net;

namespace Handlers
{
    public static class WebHandler
    {
        public static bool TryGetFile(string webAddress, string fileName)
        {
            try
            {
                new WebClient().DownloadFile(Path.Combine(webAddress, fileName), Path.Combine(FileHandler.LocalPath, fileName));
                Logging.Instance.WriteLine($"Successfully downloaded \"{fileName}\"!");
                return true;
            }
            catch
            {
                Logging.Instance.WriteLine($"Failed to download \"{fileName}\"!");
                return false;
            }
        }
    }
}