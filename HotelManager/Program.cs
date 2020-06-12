using System;
using System.Diagnostics;
using System.IO;

namespace HotelManager
{
    class Program
    {
        private static string webAddress;
        private const string ConfigFileName = "Config.xml";
        private const string MainExeFileName = "HotelManagerGui.exe";
        private const string CoreDllFileName = "Core.dll";
        private const string CommandsDllFileName = "Commands.dll";
        private const string HandlersDllFileName = "Handlers.dll";
        private const string TemplatesDllFileName = "Templates.dll";
        private const string DataFileName = "Reservations";

        private static void Main()
        {
            if (!TryUpdateFile(MainExeFileName)) return;
            if (!TryUpdateFile(CoreDllFileName)) return;
            if (!TryUpdateFile(CommandsDllFileName)) return;
            if (!TryUpdateFile(HandlersDllFileName)) return;
            if (!TryUpdateFile(TemplatesDllFileName)) return;
            if (!TryUpdateFile(DataFileName)) return;
            Process.Start(Path.Combine(FileHandler.LocalPath, MainExeFileName));
        }

        public static bool TryUpdateFile(string fileName)
        {
            if (!FileHandler.CheckFileExists(fileName) || !FileHandler.IsLocalFileNewer(fileName))
            {
                if (!FileHandler.TryGetFile(GetWebsiteAddress(), fileName))
                {
                    Console.WriteLine($"Не можах да сваля \"{fileName}\"");
                    Console.ReadLine();
                    return false;
                }
            }
            return true;
        }

        internal static string GetWebsiteAddress()
        {
            if (string.IsNullOrEmpty(webAddress))
            {
                if (!FileHandler.CheckFileExists(ConfigFileName))
                {
                    Console.WriteLine("Напиши адрес за синхронизиране (напр. \"google.com\")");
                    webAddress = $"https://{Console.ReadLine()}/HotelManager/";
                    FileHandler.TryGetFile(webAddress, ConfigFileName);
                }
                else webAddress = $"https://{FileHandler.GetXmlData("WebAddress")}/HotelManager/";
            }
            return webAddress;
        }
    }
}