using System;
using System.IO;
using System.Net;
using System.Text;

namespace Handlers
{
    public static class FtpHandler
    {
        public static DateTime? TryGetRemoteFileTime(string fileName)
        {
            try
            {
                FtpWebRequest request = FtpRequest(fileName, WebRequestMethods.Ftp.GetDateTimestamp);
                using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                DateTime? remoteFileLastWrite = response.LastModified;
                Logging.Instance.WriteLine($"Remote {fileName} time: {remoteFileLastWrite:dd.MM.yyyy HH:mm:ss}");
                return remoteFileLastWrite;
            }
            catch (Exception e)
            {
                Logging.Instance.WriteLine($"Failed to get remote file time. Exception:\n{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

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