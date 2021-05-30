using System;
using System.Collections.Generic;
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
            if (!ReadSettingsFile() || !TryUpdateFile(Constants.ReservationsFileName))
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
            mainWindow.Closed += CallShutDown;
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

        private bool ExportCollectionsToJson(string filePath, IEnumerable<object> collection)
        {
            if (FileHandler.WriteAllLines(filePath, JsonHandler.GetJsonStrings(collection)))
            {
                string messageText = $"Successfully exported Reservations to \"{filePath}\"!";
                Logging.Instance.WriteLine(messageText);
                MessageBox.Show(messageText, "Export success!", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            }
            else
            {
                string messageText = $"Failed to export Reservations to \"{filePath}\"!";
                Logging.Instance.WriteLine(messageText);
                MessageBox.Show(messageText, "Export fail!", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private void CallShutDown(object sender, EventArgs e)
        {
            //if (!FtpHandler.TryUploadBackupFile(Constants.ReservationsFileName))
            //{
            //    ShowFailMessage(string.Format(Constants.ErrorRemoteFileUpload, Constants.ReservationsFileName));
            //}
            //string filePath = Path.Combine(Constants.LocalPath, Constants.ReservationsFileName);
            //if (!ExportCollectionsToJson(filePath, controller.Reservations)) return;
            //if (!FtpHandler.TryUploadFileByName(Constants.ReservationsFileName))
            //{
            //    ShowFailMessage(string.Format(Constants.ErrorRemoteFileUpload, Constants.ReservationsFileName));
            //}
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