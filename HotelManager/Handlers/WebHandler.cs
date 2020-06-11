﻿using System.IO;
using System.Net;

namespace HotelManager
{
    public static class WebHandler
    {
        public static bool TryGetFile(string webAddress, string fileName)
        {
            try
            {
                WebRequest request = WebRequest.Create($"https://{webAddress}/HotelManager/{fileName}");
                request.Method = WebRequestMethods.File.DownloadFile;
                using (WebResponse response = request.GetResponse())
                {
                    using StreamReader reader = new StreamReader(response.GetResponseStream());
                    FileHandler.WriteToFile(fileName, reader.ReadToEnd());
                }
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