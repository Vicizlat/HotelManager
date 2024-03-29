﻿using System;
using System.IO;
using System.Text;
using System.Windows;
using HotelManager.Handlers;
using HotelManager.Controller;
using HotelManager.Utils;
using HotelManager.Views;
using Squirrel;
using System.Threading.Tasks;

namespace HotelManager
{
    public partial class App
    {
        private MainController controller;
        private UpdateManager manager;
        private UpdateInfo updateInfo;
        private string installedVersion;

        private async void Application_StartupAsync(object sender, StartupEventArgs e)
        {
            Logging.Instance.WriteLine("Logging started");
            ManageLogFiles(Constants.LogsPath);
            await CheckForUpdate();
            controller = new MainController();
            if (!ReadSettingsFile() || !controller.Initialize())
            {
                ShowFailMessage("Can't connect to database!");
                CallShutdown();
                return;
            }
            Settings.OnSettingsChanged += WriteSettingsFile;
            MainWindow mainWindow = new MainWindow(controller) { Title = $"Hotel Manager v.{installedVersion}" };
            mainWindow.Show();
            mainWindow.Closing += delegate { CallShutdown(); };
        }

        private async Task CheckForUpdate()
        {
            manager = await UpdateManager.GitHubUpdateManager(@"https://github.com/Vicizlat/HotelManager");
            installedVersion = manager.CurrentlyInstalledVersion()?.ToString() ?? "Debug";
            if (installedVersion == "Debug") return;
            updateInfo = await manager.CheckForUpdate();
            string newVersion = updateInfo.FutureReleaseEntry.Version.ToString();
            if (updateInfo.ReleasesToApply.Count > 0 && UpdateMessageResponse(newVersion))
            {
                await manager.UpdateApp();
                Logging.Instance.WriteLine($"Succesfuly Updated from v.{installedVersion} to v.{newVersion}!");
                UpdateManager.RestartApp("HotelManager.exe");
            }
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

        private bool ShowFailMessage(string text)
        {
            Logging.Instance.WriteLine(text);
            return MessageBox.Show(text) == MessageBoxResult.None;
        }

        private bool UpdateMessageResponse(string newVersion)
        {
            StringBuilder sb = new StringBuilder($"There is an update for this program (v.{newVersion}).");
            sb.AppendLine($"The currently installed version is v.{installedVersion}.");
            sb.AppendLine().AppendLine("Do you want to restart and update this program?");
            Logging.Instance.WriteLine(sb.ToString());
            bool response = MessageBox.Show(sb.ToString(), "Update found...", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            Logging.Instance.WriteLine($"Response: {(response ? "Yes" : "No")}");
            return response;
        }

        private void CallShutdown()
        {
            if (controller != null && controller.ChangesMade)
            {
                foreach (string item in Constants.ImportExportSources)
                {
                    string path = Path.Combine(Constants.LocalPath, $"{item}.json");
                    controller.ExportCollectionToJson(path, controller.GetCollectionByName(item), item, false);
                }
            }
            Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logging.Instance.Close();
        }
    }
}