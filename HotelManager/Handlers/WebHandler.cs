using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using HotelManager.Utils;

namespace HotelManager.Handlers
{
    public static class WebHandler
    {
        public static async Task<bool> TryGetFileAsync(string webAddress, string fileName)
        {
            string remoteFilePath = Path.Combine(webAddress, fileName);
            string localFilePath = Path.Combine(Constants.LocalPath, fileName);
            try
            {
                using HttpClient client = new HttpClient();
                await DownloadFileTaskAsync(client, remoteFilePath, localFilePath);
                Logging.Instance.WriteLine($"Successfully downloaded \"{fileName}\"!");
                return true;
            }
            catch
            {
                Logging.Instance.WriteLine($"Failed to download \"{fileName}\"!");
                return false;
            }
        }

        public static async Task DownloadFileTaskAsync(HttpClient client, string uri, string FileName)
        {
            using Stream s = await client.GetStreamAsync(uri);
            using FileStream fs = new FileStream(FileName, FileMode.CreateNew);
            await s.CopyToAsync(fs);
        }
    }
}