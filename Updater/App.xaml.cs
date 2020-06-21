using IWshRuntimeLibrary;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml;
using File = System.IO.File;

namespace Updater
{
    public partial class App
    {
        private static readonly string ProgramFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        private static string InstallPath => Directory.CreateDirectory(Path.Combine(ProgramFilesPath, "HotelManager")).FullName;
        private string webAddress;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            webAddress = WebAddress();
            if (!IsDotNetPresent() || string.IsNullOrEmpty(webAddress))
            {
                if (string.IsNullOrEmpty(webAddress) || !TryGetFile(InstallPath, "DotNetInstaller.exe"))
                {
                    Shutdown();
                    return;
                }
                Process.Start(Path.Combine(InstallPath, "DotNetInstaller.exe"), "x86")?.WaitForExit();
            }
            MainWindow mainWindow = new MainWindow(webAddress, GetXmlData("FtpAddress"), GetXmlData("FtpUserName"), GetXmlData("FtpPassword"));
            mainWindow.Closed += MainWindow_Closed;
            mainWindow.Show();
        }

        internal bool IsDotNetPresent()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("dotnet", "--list-runtimes")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                using Process proc = Process.Start(psi);
                while (proc != null && !proc.StandardOutput.EndOfStream)
                {
                    string input = proc.StandardOutput.ReadLine();
                    if (input == null || !input.StartsWith("Microsoft.")) continue;
                    string[] line = input.Split();
                    int[] version = line[1].Split('.').Select(int.Parse).ToArray();
                    if (line[0].Split('.')[1] != "WindowsDesktop") continue;
                    if (version[0] < 3) continue;
                    if (version[0] == 3 && version[1] < 1) continue;
                    if (version[0] == 3 && version[1] == 1 && version[2] < 5) continue;
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool TryGetFile(string path, string fileName)
        {
            try
            {
                using WebClient webClient = new WebClient();
                webClient.DownloadFile(Path.Combine(webAddress, fileName), Path.Combine(path, fileName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal void CreateShortcut(string desktopPath)
        {
            IWshShortcut shortcut = new WshShell().CreateShortcut(Path.Combine(desktopPath, "Hotel Manager.lnk"));
            shortcut.Description = "Hotel Manager";
            shortcut.WorkingDirectory = InstallPath;
            shortcut.TargetPath = Path.Combine(InstallPath, "HotelManager.exe");
            shortcut.Save();
        }

        private string WebAddress()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userPath = Directory.CreateDirectory(Path.Combine(appDataPath, "HotelManager")).FullName;
            if (File.Exists(Path.Combine(userPath, "Config.xml"))) return GetXmlData("WebAddressFull");
            SelectWebsiteWindow webAddressWindow = new SelectWebsiteWindow();
            webAddressWindow.ShowDialog();
            string newWebAddress = webAddressWindow.Address.Text;
            if (string.IsNullOrEmpty(newWebAddress) || !TryGetFile(userPath, "Config.xml")) return string.Empty;
            return $"https://{newWebAddress}/HotelManager/";
        }

        internal string GetXmlData(string element)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string userPath = Directory.CreateDirectory(Path.Combine(appDataPath, "HotelManager")).FullName;
            using XmlReader reader = XmlReader.Create(Path.Combine(userPath, "Config.xml"));
            while (reader.Read())
            {
                if (reader.IsStartElement() && reader.Name == element) return reader.ReadElementContentAsString();
            }
            return element;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            CreateShortcut(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            Process.Start(Path.Combine(InstallPath, "HotelManager.exe"));
            Shutdown();
        }
    }
}