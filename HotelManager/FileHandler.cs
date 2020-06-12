using System;
using System.IO;
using System.Net;
using System.Xml;

namespace HotelManager
{
    public static class FileHandler
    {
        public static string LocalPath => Directory.CreateDirectory(Path.Combine(AppDataPath, "HotelManager")).FullName;
        private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static bool CheckFileExists(string fileName) => File.Exists(Path.Combine(LocalPath, fileName));

        public static bool IsLocalFileNewer(string fileName)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(GetXmlData("FtpAddress"), fileName));
                request.Credentials = new NetworkCredential(GetXmlData("FtpUserName"), GetXmlData("FtpPassword"));
                request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return File.GetLastWriteTime(Path.Combine(LocalPath, fileName)) >= response.LastModified;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryGetFile(string webAddress, string fileName)
        {
            try
            {
                new WebClient().DownloadFile($"{webAddress}{fileName}", Path.Combine(LocalPath, fileName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string GetXmlData(string xmlData)
        {
            using XmlReader reader = XmlReader.Create(Path.Combine(LocalPath, "Config.xml"));
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.Name == xmlData) return reader.ReadElementContentAsString();
            }
            return xmlData;
        }
    }
}