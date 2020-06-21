using System;
using System.IO;

namespace Handlers
{
    public static class FileHandler
    {
        public static string LocalPath => Directory.CreateDirectory(Path.Combine(AppDataPath, "HotelManager")).FullName;
        private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

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
                DateTime? localFileLastWrite = File.GetLastWriteTime(Path.Combine(LocalPath, localFileName));
                Logging.Instance.WriteLine($"Local {localFileName} time: {localFileLastWrite:dd.MM.yyyy HH:mm:ss}");
                return localFileLastWrite;
            }
            catch (Exception e)
            {
                Logging.Instance.WriteLine($"Failed to get local file time. Exception:\n{e.Message}\n{e.StackTrace}");
                return null;
            }
        }

        public static string[] ReadFromFile(string fileName)
        {
            return File.ReadAllLines(Path.Combine(LocalPath, fileName));
        }

        public static bool WriteToFile(string fileName, string[] contents)
        {
            try
            {
                File.WriteAllLines(Path.Combine(LocalPath, fileName), contents);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void WriteToFile(string fileName, string contents)
        {
            File.WriteAllText(Path.Combine(LocalPath, fileName), contents);
        }
    }
}