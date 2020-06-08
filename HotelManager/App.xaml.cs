using System;
using System.IO;
using System.Windows;

namespace HotelManager
{
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Instance.WriteLine($"Logging started");
            if (File.Exists(Path.Combine(FileHandler.LocalPath, "Config.xml")) || FileHandler.TryGetConfigFile())
            {
                new MainWindow().Show();
                MainWindow.Closed += MainWindow_Closed;
            }
            else
            {
                MessageBox.Show("Не успях да сваля \"Config.xml\"");
                Shutdown();
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Logging.Instance.Close();
            Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            FileHandler.TryUploadFile("Log.txt", true);
        }
    }
}