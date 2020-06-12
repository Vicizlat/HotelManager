using System;
using System.IO;

namespace Handlers
{
    public static class FileHandler
    {
        public static string LocalPath => Directory.CreateDirectory(Path.Combine(AppDataPath, "HotelManager")).FullName;
        private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static bool IsLocalFileNewer(string fileName, out bool checkLocalFile, out bool checkRemoteFile)
        {
            checkLocalFile = TryGetLocalFileTime(fileName, out DateTime? localFileLastWrite);
            checkRemoteFile = FtpHandler.TryGetRemoteFileTime(fileName, out DateTime? remoteFileLastWrite);
            if (!checkLocalFile) return false;
            if (!checkRemoteFile) return true;
            return localFileLastWrite >= remoteFileLastWrite;
        }

        private static bool TryGetLocalFileTime(string localFileName, out DateTime? localFileLastWrite)
        {
            localFileLastWrite = File.GetLastWriteTime(Path.Combine(LocalPath, localFileName));
            Logging.Instance.WriteLine($"Local {localFileName} time: {localFileLastWrite:dd.MM.yyyy HH:mm:ss}");
            return localFileLastWrite != null;
        }

        public static string[] ReadFromFile(string fileName)
        {
            return File.ReadAllLines(Path.Combine(LocalPath, fileName));
        }

        public static void WriteToFile(string fileName, string[] contents)
        {
            File.WriteAllLines(Path.Combine(LocalPath, fileName), contents);
        }

        public static void WriteToFile(string fileName, string contents)
        {
            File.WriteAllText(Path.Combine(LocalPath, fileName), contents);
        }

        public static bool CheckFileExists(string fileName)
        {
            return File.Exists(Path.Combine(LocalPath, fileName));
        }
    }
}