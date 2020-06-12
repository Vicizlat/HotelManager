using System;
using System.Text;
using System.Windows;
using Core;
using Handlers;

namespace Gui
{
    public partial class App
    {
        //private bool loadMainWindow;
        //private string webAddress = $"https://{XmlHandler.GetXmlData("WebAddress")}/HotelManager/";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Instance.WriteLine("Logging started");
            //if (FileHandler.CheckFileExists("Reservations"))
            //{
            //    if (FileHandler.IsLocalFileNewer("Reservations", out bool checkLocalFile, out bool checkRemoteFile))
            //    {
            //        if (checkRemoteFile) loadMainWindow = true;
            //        else if (MessageResponse("Не можах да проверя файла \"Reservations\" на сървъра.")) loadMainWindow = true;
            //        else loadMainWindow = false;
            //    }
            //    else if (checkLocalFile)
            //    {
            //        if (WebHandler.TryGetFile(webAddress, "temp"))
            //        {
            //            Reservations.Instance.LoadReservations("temp");
            //            FileHandler.WriteToFile("Reservations", Reservations.Instance.ReservationsString());
            //            loadMainWindow = true;
            //        }
            //        else if (MessageResponse("Файла \"Reservations\" е по-нов на сървъра, но не можах да го сваля.")) loadMainWindow = true;
            //        else loadMainWindow = false;
            //    }
            //    else ShowFailMessage("Грешка при отваряне на файл \"Reservations\"");
            //}
            //else if (WebHandler.TryGetFile(webAddress, "Reservations")) loadMainWindow = true;
            //else ShowFailMessage("Не успях да сваля \"Reservations\"");

            //if (loadMainWindow)
            //{
                new MainWindow().Show();
                if (MainWindow != null) MainWindow.Closed += MainWindow_Closed;
            //}
            //else Shutdown();
        }

        //internal bool MessageResponse(string messageBoxText)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine(messageBoxText);
        //    sb.AppendLine("Да продължа ли само с локалния файл?");
        //    sb.AppendLine("\nВНИМАНИЕ: Може да доведе до загуба на резервации!");
        //    string messageBoxCaption = "Проблем при връзката със сървъра";
        //    return MessageBox.Show(sb.ToString(), messageBoxCaption, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        //}

        //internal string GetWebsiteAddress()
        //{
        //    if (string.IsNullOrEmpty(webAddress))
        //    {
        //        SelectWebsiteWindow dialog = new SelectWebsiteWindow();
        //        dialog.ShowDialog();
        //        webAddress = $"https://{dialog.Address.Text}/HotelManager/";
        //    }
        //    return webAddress;
        //}

        //private void ShowFailMessage(string text)
        //{
        //    MessageBox.Show(text);
        //    Logging.Instance.WriteLine(text);
        //    loadMainWindow = false;
        //}

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