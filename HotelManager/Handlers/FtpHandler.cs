using System;
using System.IO;
using System.Net;
using System.Text;
using HotelManager.Utils;

namespace HotelManager.Handlers
{
    public static class FtpHandler
    {
        public static FtpWebRequest FtpRequest(string ftpPath, string method)
        {
            // #pragma warning disable SYSLIB0014
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpPath);
            string ftpUserName = Settings.Instance.FtpUserName;
            string ftpPassword = Settings.Instance.FtpPassword;
            request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);
            request.Method = method;
            return request;
        }

        public static DateTime? TryGetRemoteFileTime(string fileName)
        {
            try
            {
                string ftpFilePath = Path.Combine(Settings.Instance.FtpAddress, fileName);
                FtpWebRequest request = FtpRequest(ftpFilePath, WebRequestMethods.Ftp.GetDateTimestamp);
                using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                DateTime? remoteFileLastWrite = response.LastModified;
                Logging.Instance.WriteLine($"Remote {fileName} time: {remoteFileLastWrite:dd.MM.yyyy HH:mm:ss}");
                return remoteFileLastWrite;
            }
            catch (Exception e)
            {
                Logging.Instance.WriteLine("Failed to get remote file time. Exception:");
                Logging.Instance.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                return null;
            }
        }

        public static bool TryUploadFileByName(string fileName, bool isLogWriterClosed = false)
        {
            if (Settings.Instance.LocalUseOnly) return true;
            string ftpDirPath = Settings.Instance.FtpAddress;
            if (fileName == Constants.LogFileName && !FtpMakeDir("Logs", out ftpDirPath)) return false;
            string ftpFilePath = Path.Combine(ftpDirPath, fileName);
            string localFilePath = fileName == Constants.LogFileName ? Constants.LogsPath : Constants.LocalPath;
            localFilePath = Path.Combine(localFilePath, fileName);
            return TryUploadFile(fileName, ftpFilePath, localFilePath, isLogWriterClosed);
        }

        public static bool TryUploadBackupFile(string fileName)
        {
            if (Settings.Instance.LocalUseOnly) return true;
            if (!FtpMakeDir("Backups", out string ftpDirPath)) return false;
            string backupFileName = fileName.Insert(fileName.Length - 5, $"-{DateTime.Now:[yyyy-MM-dd][HH-mm-ss]}");
            string ftpFilePath = Path.Combine(ftpDirPath, backupFileName);
            string localFilePath = Path.Combine(Constants.LocalPath, fileName);
            return TryUploadFile(backupFileName, ftpFilePath, localFilePath, false);
        }

        private static bool TryUploadFile(string fileName, string ftpFilePath, string localFilePath, bool isLogWriterClosed)
        {
            try
            {
                FtpWebRequest request = FtpRequest(ftpFilePath, WebRequestMethods.Ftp.UploadFile);
                using StreamReader reader = new StreamReader(localFilePath);
                byte[] fileContents = Encoding.UTF8.GetBytes(reader.ReadToEnd());
                request.ContentLength = fileContents.Length;
                using Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                return LogSuccessAndReturnTrue(fileName, isLogWriterClosed);
            }
            catch
            {
                return LogFailAndReturnFalse(fileName, isLogWriterClosed);
            }
        }

        private static bool LogSuccessAndReturnTrue(string fileName, bool isLogWriterClosed = false)
        {
            if (!isLogWriterClosed) Logging.Instance.WriteLine($"Successfully uploaded \"{fileName}\"!");
            return true;
        }

        private static bool LogFailAndReturnFalse(string fileName, bool isLogWriterClosed = false)
        {
            if (!isLogWriterClosed) Logging.Instance.WriteLine($"Failed to upload \"{fileName}\"!");
            return false;
        }

        private static bool FtpMakeDir(string dirName, out string ftpDirPath)
        {
            ftpDirPath = Path.Combine(Settings.Instance.FtpAddress, dirName);
            FtpWebRequest request = FtpRequest(ftpDirPath, WebRequestMethods.Ftp.MakeDirectory);
            try
            {
                using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                return response.ResponseUri.AbsoluteUri == ftpDirPath;
            }
            catch (WebException e)
            {
                return e.Response.ResponseUri.AbsoluteUri.EndsWith(dirName);
            }
        }
    }
}