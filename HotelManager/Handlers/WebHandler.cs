using System.IO;
using System.Net;
using HotelManager.Utils;

namespace HotelManager.Handlers
{
    public static class WebHandler
    {
        public static bool TryGetFile(string fileName)
        {
            string remoteFilePath = Path.Combine(Settings.Instance.WebAddress, fileName);
            string localFilePath = Path.Combine(Constants.LocalPath, fileName);
            try
            {
                using WebClient webClient = new WebClient();
                webClient.DownloadFile(remoteFilePath, localFilePath);
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