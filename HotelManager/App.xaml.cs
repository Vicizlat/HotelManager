using System;
using System.Text;
using System.Windows;

namespace HotelManager
{
    public partial class App : Application
    {
        private bool LoadMainWindow { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Instance.WriteLine($"Logging started");
            if (FileHandler.CheckFileExists("Config.xml") || WebHandler.TryGetFile(GetWebsiteAddress(), "Config.xml"))
            {
                if (FileHandler.CheckFileExists("Reservations"))
                {
                    if (FileHandler.IsLocalFileNewer(out bool checkLocalFile, out bool checkRemoteFile))
                    {
                        if (checkRemoteFile) LoadMainWindow = true;
                        else if (MessageResponse("Не можах да проверя файла \"Reservations\" на сървъра.")) LoadMainWindow = true;
                        else LoadMainWindow = false;
                    }
                    else if (checkLocalFile)
                    {
                        if (FtpHandler.TryGetRemoteFile()) LoadMainWindow = true;
                        else if (MessageResponse("Файла \"Reservations\" е по-нов на сървъра, но не можах да го сваля.")) LoadMainWindow = true;
                        else LoadMainWindow = false;
                    }
                    else ShowFailMessage("Грешка при отваряне на файл \"Reservations\"");
                }
                else if (WebHandler.TryGetFile(XmlHandler.GetXmlData("WebAddress"), "Reservations")) LoadMainWindow = true;
                else ShowFailMessage("Не успях да сваля \"Reservations\"");
            }
            else ShowFailMessage("Не успях да сваля \"Config.xml\"");

            if (LoadMainWindow)
            {
                new MainWindow().Show();
                MainWindow.Closed += MainWindow_Closed;
            }
            else Shutdown();
        }

        private static bool MessageResponse(string messageBoxText)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(messageBoxText);
            sb.AppendLine("Да продължа ли само с локалния файл?");
            sb.AppendLine("\nВНИМАНИЕ: Може да доведе до загуба на резервации!");
            string messageBoxCaption = "Проблем при връзката със сървъра";
            return MessageBox.Show(sb.ToString(), messageBoxCaption, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private string GetWebsiteAddress()
        {
            SelectWebsiteWindow dialog = new SelectWebsiteWindow();
            dialog.ShowDialog();
            return dialog.Address.Text;
        }

        private void ShowFailMessage(string text)
        {
            MessageBox.Show(text);
            Logging.Instance.WriteLine(text);
            LoadMainWindow = false;
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