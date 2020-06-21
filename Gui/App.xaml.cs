using System;
using System.IO;
using System.Text;
using System.Windows;
using Handlers;

namespace Gui
{
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Instance.WriteLine("Logging started");
            if (TryUpdateFile("Reservations"))
            {
                new MainWindow().Show();
                if (MainWindow != null) MainWindow.Closed += MainWindow_Closed;
            }
            else Shutdown();
            //new MainWindow().Show();
            //if (MainWindow != null) MainWindow.Closed += MainWindow_Closed;
        }

        private bool TryUpdateFile(string fileName)
        {
            try
            {
                if (File.Exists(Path.Combine(FileHandler.LocalPath, fileName)))
                {
                    if (FileHandler.IsLocalFileNewer(fileName, out bool checkedRemoteFile))
                    {
                        return checkedRemoteFile || MessageResponse($"Не можах да проверя файла \"{fileName}\" на сървъра.");
                    }
                }
                if (WebHandler.TryGetFile(XmlHandler.GetXmlData("WebAddressFull"), fileName)) return true;
                ShowFailMessage("Не успях да сваля \"Reservations\"");
                return false;
            }
            catch(Exception e)
            {
                ShowFailMessage(e.Message);
                return false;
            }
        }

        internal void ShowFailMessage(string text)
        {
            MessageBox.Show(text);
            Logging.Instance.WriteLine(text);
        }

        internal bool MessageResponse(string messageBoxText)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(messageBoxText);
            sb.AppendLine("Да продължа ли само с локалния файл?");
            sb.AppendLine("\nВНИМАНИЕ: Може да доведе до загуба на резервации!");
            string messageBoxCaption = "Проблем при връзката със сървъра";
            return MessageBox.Show(sb.ToString(), messageBoxCaption, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
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