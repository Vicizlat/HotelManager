using System;
using System.IO;
using System.Windows;
using HotelManager.Controller;
using HotelManager.Handlers;
using HotelManager.Utils;
using HotelManager.Views;

namespace HotelManager
{
    public partial class App
    {
        private IController controller;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Instance.WriteLine("Logging started");
            string[] files = Directory.GetFiles(Constants.LogsPath);
            while (files.Length > 50)
            {
                File.Delete(files[0]);
                files = Directory.GetFiles(Constants.LogsPath);
            }
            controller = new Controller.Controller(out bool controllerInitialized);
            if (!controllerInitialized)
            {
                CallShutDown(this, EventArgs.Empty);
                return;
            }
            MainWindow = new MainWindow(controller);
            MainWindow.Show();
            MainWindow.Closed += CallShutDown;
        }

        private void CallShutDown(object sender, EventArgs e)
        {
            Logging.Instance.Close();
            Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (Settings.Instance.LocalUseOnly || FtpHandler.TryUploadFileByName(Constants.LogFileName, true)) return;
            new Logging();
            controller.ShowFailMessage(string.Format(Constants.ErrorRemoteFileUpload, Constants.LogFileName));
            Logging.Instance.Close();
        }
    }
}