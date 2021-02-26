using System;
using System.Collections.Generic;
using System.IO;
using HotelManager.Utils;

namespace HotelManager.Handlers
{
    public static class FileHandler
    {
        public static bool FileExists(string fileName)
        {
            return File.Exists(Path.Combine(Constants.LocalPath, fileName));
        }

        public static bool IsLocalFileNewer(string fileName, out bool checkedRemoteFile)
        {
            DateTime? localFileLastWrite = TryGetLocalFileTime(fileName);
            DateTime? remoteFileLastWrite = FtpHandler.TryGetRemoteFileTime(fileName);
            checkedRemoteFile = remoteFileLastWrite.HasValue;
            if (!localFileLastWrite.HasValue) return false;
            if (!remoteFileLastWrite.HasValue) return true;
            return localFileLastWrite.Value >= remoteFileLastWrite.Value;
        }

        private static DateTime? TryGetLocalFileTime(string localFileName)
        {
            try
            {
                DateTime? localFileLastWrite = File.GetLastWriteTime(Path.Combine(Constants.LocalPath, localFileName));
                Logging.Instance.WriteLine($"Local {localFileName} time: {localFileLastWrite:dd.MM.yyyy HH:mm:ss}");
                return localFileLastWrite;
            }
            catch (Exception e)
            {
                Logging.Instance.WriteLine("Failed to get local file time. Exception:");
                Logging.Instance.WriteLine(e.Message + Environment.NewLine + e.StackTrace);
                return null;
            }
        }

        public static string[] ReadFromFile(string fileName)
        {
            return File.ReadAllLines(Path.Combine(Constants.LocalPath, fileName));
        }

        public static bool WriteToFile(string fileName, IEnumerable<string> contents)
        {
            try
            {
                File.WriteAllLines(Path.Combine(Constants.LocalPath, fileName), contents);
                Logging.Instance.WriteLine($"Successfully saved \"{fileName}\"!");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}