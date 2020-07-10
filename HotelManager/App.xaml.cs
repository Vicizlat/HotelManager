using System;
using System.IO;
using System.Windows;
using Core;
using Handlers;

namespace HotelManager
{
    public partial class App
    {
        private const string ReservationsFileName = "Reservations";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logging.Instance.WriteLine("Logging started");
            if (TryUpdateFile(ReservationsFileName))
            {
                LoadReservations(ReservationsFileName);
                new MainWindow().Show();
                if (MainWindow != null) MainWindow.Closed += MainWindow_Closed;
            }
            else Shutdown();
        }

        private static bool TryUpdateFile(string fileName)
        {
            try
            {
                return File.Exists(Path.Combine(FileHandler.LocalPath, fileName)) && FileHandler.IsLocalFileNewer(fileName, out bool checkedRemoteFile)
                    ? checkedRemoteFile || MessageResponse($"Не можах да проверя файла \"{fileName}\" на сървъра.")
                    : WebHandler.TryGetFile(XmlHandler.GetXmlData("WebAddressFull"), fileName) || ShowFailMessage($"Не успях да сваля \"{fileName}\"");
            }
            catch(Exception e)
            {
                return ShowFailMessage(e.Message);
            }
        }

        private static bool ShowFailMessage(string text)
        {
            MessageBox.Show(text);
            Logging.Instance.WriteLine(text);
            return false;
        }

        private static bool MessageResponse(string messageBoxText)
        {
            Logging.Instance.WriteLine(messageBoxText);
            messageBoxText += Environment.NewLine + "Да продължа ли само с локалния файл?" + Environment.NewLine;
            messageBoxText += Environment.NewLine + "ВНИМАНИЕ: Може да доведе до загуба на резервации!";
            string messageBoxCaption = "Проблем при връзката със сървъра";
            bool response = MessageBox.Show(messageBoxText, messageBoxCaption, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            Logging.Instance.WriteLine($"Response: {response}");
            return response;
        }

        private static void LoadReservations(string fileName)
        {
            foreach (string line in FileHandler.ReadFromFile(fileName))
            {
                Reservation reservation = ParseReservationLine(line);
                if (reservation != null) Reservations.Instance.Add(reservation);
            }
        }

        private static Reservation ParseReservationLine(string line)
        {
            try
            {
                string[] lineArr = line.Split(new[] { "|", " |", "| ", " | " }, StringSplitOptions.RemoveEmptyEntries);
                int id = int.Parse(lineArr[0].Trim());
                int status = int.Parse(lineArr[1].Trim());
                int room = int.Parse(lineArr[2].Trim());
                string guestName = lineArr[3].Trim();
                Period period = new Period(DateTime.Parse(lineArr[4].Trim()), DateTime.Parse(lineArr[5].Trim()));
                int guestsInRoom = int.Parse(lineArr[6].Trim());
                Sums sums = new Sums(decimal.Parse(lineArr[7].Trim()), decimal.Parse(lineArr[8].Trim()));
                string additionalInfo = lineArr[9].Trim();
                return new Reservation(id, status, room, guestName, period, guestsInRoom, sums, additionalInfo);
            }
            catch
            {
                Logging.Instance.WriteLine($"Failed to read line: {line}");
                return null;
            }
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