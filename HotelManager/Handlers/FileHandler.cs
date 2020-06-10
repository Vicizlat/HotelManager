using System;
using System.IO;

namespace HotelManager
{
    public static class FileHandler
    {
        public static string LocalPath => Directory.CreateDirectory(Path.Combine(AppDataPath, "HotelManager")).FullName;
        private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static bool IsLocalFileNewer(out bool checkLocalFile, out bool checkRemoteFile)
        {
            checkLocalFile = TryGetLocalFileTime(out DateTime? localFileLastWrite);
            checkRemoteFile = false;
            if (!checkLocalFile) return false;
            checkRemoteFile = FtpHandler.TryGetRemoteFileTime(out DateTime? remoteFileLastWrite);
            if (!checkRemoteFile) return true;
            bool localFileIsNewer = localFileLastWrite >= remoteFileLastWrite;
            Logging.Instance.WriteLine(localFileIsNewer ? "Local file is newer." : "Remote file is newer. Downloading remote file.");
            return localFileIsNewer;
        }

        private static bool TryGetLocalFileTime(out DateTime? localFileLastWrite)
        {
            localFileLastWrite = File.GetLastWriteTime(Path.Combine(LocalPath, "Reservations"));
            Logging.Instance.WriteLine($"Local file time: {localFileLastWrite:dd.MM.yyyy HH:mm:ss}");
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