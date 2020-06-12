using System;
using System.IO;
using System.Net;
using System.Text;

namespace Handlers
{
    public static class FtpHandler
    {
        public static bool TryGetRemoteFileTime(string fileName, out DateTime? remoteFileLastWrite)
        {
            try
            {
                FtpWebRequest request = FtpRequest(fileName, WebRequestMethods.Ftp.GetDateTimestamp);
                using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                remoteFileLastWrite = response.LastModified;
                Logging.Instance.WriteLine($"Remote {fileName} time: {remoteFileLastWrite:dd.MM.yyyy HH:mm:ss}");
                return true;
            }
            catch
            {
                Logging.Instance.WriteLine("Could not connect to server! Using local file only.");
                remoteFileLastWrite = null;
                return false;
            }
        }

        //public static bool TryGetRemoteFile()
        //{
        //    try
        //    {
        //        FtpWebRequest request = FtpRequest("Reservations", WebRequestMethods.Ftp.DownloadFile);
        //        using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
        //        using StreamReader reader = new StreamReader(response.GetResponseStream());
        //        FileHandler.WriteToFile("temp", reader.ReadToEnd());
        //        Logging.Instance.WriteLine("Successfully downloaded \"Reservations\"!");
        //        return true;
        //    }
        //    catch
        //    {
        //        Logging.Instance.WriteLine("Failed to download \"Reservations\"!");
        //        return false;
        //    }
        //}

        public static void TryUploadFile(string fileName, bool isLogWriterClosed = false)
        {
            try
            {
                FtpWebRequest request = FtpRequest(fileName, WebRequestMethods.Ftp.UploadFile);
                using StreamReader reader = new StreamReader(Path.Combine(FileHandler.LocalPath, fileName));
                byte[] fileContents = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                request.ContentLength = fileContents.Length;
                using Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                if (!isLogWriterClosed) Logging.Instance.WriteLine($"Successfully uploaded \"{fileName}\"!");
            }
            catch
            {
                if (!isLogWriterClosed) Logging.Instance.WriteLine($"Failed to upload \"{fileName}\"!");
            }
        }

        public static FtpWebRequest FtpRequest(string fileName, string method)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(XmlHandler.GetXmlData("FtpAddress"), fileName));
            request.Credentials = new NetworkCredential(XmlHandler.GetXmlData("FtpUserName"), XmlHandler.GetXmlData("FtpPassword"));
            request.Method = method;
            return request;
        }
    }
}