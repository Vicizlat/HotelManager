using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace DotNetInstaller
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                string dotNetDownload = "https://download.visualstudio.microsoft.com/download/pr/";
                if (args[0] == "x86") dotNetDownload += "df7b90d9-b93e-4974-85ef-c1de418bc186/e380e58bbd8505ebaee6c3abb23baade/";
                if (args[0] == "x64") dotNetDownload += "86835fe4-93b5-4f4e-a7ad-c0b0532e407b/f4f2b1239f1203a05b9952028d54fc13/";
                string dotNetFileName = $"windowsdesktop-runtime-3.1.5-win-{args[0]}.exe";
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string localPath = Directory.CreateDirectory(Path.Combine(appDataPath, "HotelManager")).FullName;
                if (!File.Exists(Path.Combine(localPath, dotNetFileName)))
                {
                    Console.WriteLine($"Downloading {dotNetFileName}...");
                    new WebClient().DownloadFile(Path.Combine(dotNetDownload, dotNetFileName), Path.Combine(localPath, dotNetFileName));
                }
                Console.WriteLine($"Installing {dotNetFileName}...");
                Process.Start(Path.Combine(localPath, dotNetFileName), "-q")?.WaitForExit();
            }
            catch
            {
            }
        }
    }
}