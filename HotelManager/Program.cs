using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;

namespace HotelManager
{
    class Program
    {
        public static string LocalPath => Directory.CreateDirectory(Path.Combine(AppDataPath, "HotelManager")).FullName;
        private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        private static void Main()
        {
            if (!File.Exists(Path.Combine(LocalPath, "Config.xml")))
            {
                Console.WriteLine("Напиши адрес за синхронизиране (напр. \"google.com\")");
                if (!TryGetFile(Console.ReadLine(), "Config.xml")) return;
            }
            string[] fileNames = { "HotelManagerGui.exe", "Core.dll", "Commands.dll", "Handlers.dll", "Templates.dll", "Reservations" };
            foreach (string fileName in fileNames)
            {
                if (!TryUpdateFile(fileName)) return;
            }
            Process.Start(Path.Combine(LocalPath, fileNames[0]));
        }

        public static bool TryUpdateFile(string fileName)
        {
            if (File.Exists(Path.Combine(LocalPath, fileName)) && IsLocalFileNewer(fileName)) return true;
            if (TryGetFile(GetXmlData("WebAddress"), fileName)) return true;
            Console.WriteLine($"Не можах да сваля \"{fileName}\"");
            return false;

        }

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
                new WebClient().DownloadFile($"https://{webAddress}/HotelManager/{fileName}", Path.Combine(LocalPath, fileName));
                Console.WriteLine($"Successfully downloaded \"{fileName}\".");
                return true;
            }
            catch
            {
                Console.WriteLine($"Failed to download\"{fileName}\".");
                return false;
            }
        }

        public static string GetXmlData(string element)
        {
            using XmlReader reader = XmlReader.Create(Path.Combine(LocalPath, "Config.xml"));
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.Name == element) return reader.ReadElementContentAsString();
            }
            return element;
        }
    }
}