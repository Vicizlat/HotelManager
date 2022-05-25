using System;
using System.IO;
using System.Text;
using System.Windows;
using HotelManager.Controller;
using HotelManager.Handlers;
using HotelManager.Utils;
using HotelManager.Views;

namespace HotelManager
{
    public partial class App
    {
        private MainController controller;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Instance.WriteLine("Logging started");
            ManageLogFiles(Constants.LogsPath);
            if (!ReadSettingsFile())
            {
                CallShutDown(this, EventArgs.Empty);
                return;
            }
            Settings.OnSettingsChanged += WriteSettingsFile;
            controller = new MainController();
            if (!controller.Initialize())
            {
                ShowFailMessage("Can't connect to database!");
                CallShutDown(this, EventArgs.Empty);
                return;
            }
            MainWindow mainWindow = new MainWindow(controller);
            mainWindow.Show();
            mainWindow.Closing += CallShutDown;
        }

        private void ManageLogFiles(string logsPath)
        {
            string[] files = Directory.GetFiles(logsPath);
            while (files.Length > 50)
            {
                File.Delete(files[0]);
                files = Directory.GetFiles(logsPath);
            }
        }

        private bool ReadSettingsFile()
        {
            if (!FileHandler.FileExists(Constants.SettingsFilename))
            {
                return new SettingsWindow().ShowDialog() ?? false;
            }
            StringReader reader = new StringReader(FileHandler.ReadAllText(Constants.SettingsFilename));
            return Settings.CreateSettings(XmlHandler.Serializer(Settings.Instance).Deserialize(reader));
        }

        private void WriteSettingsFile(object sender, EventArgs e)
        {
            StringWriter writer = new StringWriter();
            XmlHandler.Serializer(Settings.Instance).Serialize(writer, Settings.Instance, XmlHandler.Namespaces());
            FileHandler.WriteAllText(Constants.SettingsFilename, writer.ToString());
        }

        private bool TryUpdateFile(string fileName)
        {
            if (Settings.Instance.LocalUseOnly) return true;
            bool fileExists = FileHandler.FileExists(fileName);
            bool localFileNewer = FileHandler.IsLocalFileNewer(fileName, out bool checkedRemoteFile);
            if (fileExists && localFileNewer) return checkedRemoteFile || MessageResponse(fileName);
            return WebHandler.TryGetFile(fileName) || ShowFailMessage(string.Format(Constants.ErrorRemoteFileDownload, fileName));
        }

        private bool ShowFailMessage(string text)
        {
            Logging.Instance.WriteLine(text);
            return MessageBox.Show(text) == MessageBoxResult.None;
        }

        private bool MessageResponse(string fileName)
        {
            StringBuilder sb = new StringBuilder(string.Format(Constants.ErrorRemoteFileCheck, fileName));
            sb.AppendLine().AppendLine(Constants.ContinueLocal);
            Logging.Instance.WriteLine(sb.ToString());
            sb.AppendLine().AppendLine().AppendLine(Constants.WarningLoss);
            bool response = MessageBox.Show(sb.ToString(), Constants.NetworkError, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            Logging.Instance.WriteLine($"Response: {(response ? "Yes" : "No")}");
            return response;
        }

        private void CallShutDown(object sender, EventArgs e)
        {
            foreach (string item in Constants.ImportExportSources)
            {
                string path = Path.Combine(Constants.LocalPath, $"{item}.json");
                controller.ExportCollectionToJson(path, controller.GetCollectionByName(item), item, false);
            }
            Logging.Instance.Close();
            Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Settings.Instance.LocalUseOnly || FtpHandler.TryUploadFileByName(Constants.LogFileName, true)) return;
            string text = string.Format(Constants.ErrorRemoteFileUpload, Constants.LogFileName);
            new Logging().WriteLine(text);
            MessageBox.Show(text);
            Logging.Instance.Close();
        }
    }
}