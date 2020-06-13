using System;
using System.Windows;
using Handlers;

namespace Gui
{
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Instance.WriteLine("Logging started");
            new MainWindow().Show();
            if (MainWindow != null) MainWindow.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Logging.Instance.Close();
            Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            FtpHandler.TryUploadFile("Log.txt", true);
        }
    }
}