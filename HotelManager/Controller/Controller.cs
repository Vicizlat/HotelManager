using System;
using System.IO;
using System.Text;
using HotelManager.Handlers;
using HotelManager.Models;
using HotelManager.Repositories;
using HotelManager.Utils;
using HotelManager.Views;
using System.Windows;
using System.Xml.Serialization;

namespace HotelManager.Controller
{
    public class Controller : IController
    {
        public event EventHandler<string> OnReservationsChanged;
        public event EventHandler<DateTime> OnReservationAdd;
        public event EventHandler<int> OnReservationEdit;
        public Reservations Reservations { get; }
        public Rooms Rooms { get; }
        //public Guests Guests { get; }

        public Controller(out bool controllerInitialized)
        {
            controllerInitialized = ReadSettingsFile();
            if (controllerInitialized && !Settings.Instance.LocalUseOnly)
            {
                controllerInitialized = TryUpdateFile(Constants.ReservationsFileName);
            }
            if (!controllerInitialized) return;
            Rooms = new Rooms(3, 8);
            Reservations = new Reservations(JsonHandler.GetReservationsFromFile(Constants.ReservationsFileName));
            //Guests = new Guests();
            //foreach (Reservation reservation in Reservations)
            //{
            //    Guest guest = new Guest(reservation.GuestName, null, null);
            //    if (Guests.FirstOrDefault(g => g.Name == guest.Name) == null)
            //    {
            //        Guests.AddGuests(guest);
            //    }
            //    reservation.Guest = Guests.FirstOrDefault(g => g.Name == guest.Name);
            //}
            OnReservationsChanged += Reservations_OnReservationsChanged;
        }

        private bool TryUpdateFile(string fileName)
        {
            bool fileExists = FileHandler.FileExists(fileName);
            bool localFileNewer = FileHandler.IsLocalFileNewer(fileName, out bool checkedRemoteFile);
            if (fileExists && localFileNewer) return checkedRemoteFile || MessageResponse(fileName);
            return WebHandler.TryGetFile(fileName) || ShowFailMessage(string.Format(Constants.ErrorRemoteFileDownload, fileName));
        }

        public bool ShowFailMessage(string text)
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

        public void RequestReservationWindow(int room, DateTime startDate, int? id = null)
        {
            if (id == null && Reservations.GetReservation(room, startDate) == null)
            {
                OnReservationAdd?.Invoke(room, startDate);
            }
            else
            {
                id ??= Reservations.GetReservation(room, startDate).Id;
                if (id > 0 && id <= Reservations.Count)
                {
                    OnReservationEdit?.Invoke(this, id.Value);
                }
            }
        }

        public void SaveReservation(int id, int state, int source, int room, string name, DateTime startDate, int nights, int guests, decimal total, decimal paid, string notes)
        {
            Period period = new Period(startDate, startDate.AddDays(nights));
            Sums sums = new Sums(total, paid);
            Reservation oldReservation = Reservations[id - 1];
            Reservation newReservation = new Reservation(id, (State)state, (Source)source, room, name, period, guests, sums, notes);
            if (oldReservation == null) Reservations.Add(newReservation);
            else Reservations[id - 1] = newReservation;
            OnReservationsChanged?.Invoke(new[] { oldReservation, newReservation }, Constants.ReservationsFileName);
        }

        public bool GetReservationInfo(int room, DateTime startDate, out string guestName, out int guestsNum, out decimal remainingSum)
        {
            Reservation reservation = Reservations.GetReservation(room, startDate);
            guestName = reservation?.GuestName ?? string.Empty;
            guestsNum = reservation?.GuestsInRoom ?? 0;
            remainingSum = reservation?.Sums.Remaining ?? 0;
            return reservation != null;
        }

        public DateTime ReservationStartDate(int room, DateTime date)
        {
            return Reservations.GetReservation(room, date).Period.StartDate;
        }

        public DateTime ReservationEndDate(int room, DateTime date)
        {
            return Reservations.GetReservation(room, date).Period.EndDate;
        }

        public int ReservationNights(int room, DateTime date)
        {
            return Reservations.GetReservation(room, date)?.Period.Nights ?? 1;
        }

        public bool IsValidReservation(int room, DateTime startDate)
        {
            return Reservations.GetReservation(room, startDate) != null;
        }

        public bool IsReservationCheckedIn(int room, DateTime startDate)
        {
            Reservation reservation = Reservations.GetReservation(room, startDate);
            return reservation != null && reservation.State == State.CheckedIn;
        }

        public bool IsReservationOverlapping(int room, DateTime startDate)
        {
            Reservation reservation = Reservations.GetReservation(room, startDate);
            return reservation != null && startDate != reservation.Period.StartDate;
        }

        public bool CanExecuteCheckIn(int room, DateTime startDate)
        {
            Reservation reservation = Reservations.GetReservation(room, startDate);
            return reservation != null && reservation.State != State.CheckedIn;
        }

        public void CheckInReservation(int room, DateTime startDate)
        {
            Reservation oldReservation = new Reservation(Reservations.GetReservation(room, startDate));
            Reservations[oldReservation.Id - 1].State = State.CheckedIn;
            Reservation newReservation = Reservations[oldReservation.Id - 1];
            OnReservationsChanged?.Invoke(new[] { oldReservation, newReservation }, Constants.ReservationsFileName);
        }

        public string GetTooltipText(int room, DateTime startDate)
        {
            Reservation reservation = Reservations.GetReservation(room, startDate);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Име: {reservation.GuestName}");
            sb.AppendLine($"Брой гости: {reservation.GuestsInRoom}");
            sb.AppendLine($"Период: {reservation.Period.StartDate:dd.MM.yyyy} - {reservation.Period.EndDate:dd.MM.yyyy}");
            sb.AppendLine($"Обща цена: {reservation.Sums.Total}");
            sb.AppendLine($"Предплатена сума: {reservation.Sums.Paid}");
            sb.AppendLine($"Оставаща сума: {reservation.Sums.Remaining}");
            sb.Append($"Допълнителна информация: {reservation.Notes}");
            return sb.ToString();
        }

        public bool ShowSettingsWindow()
        {
            return new SettingsWindow(this).ShowDialog() ?? false;
        }

        public void SaveSettings()
        {
            WriteSettingsFile();
        }

        private void Reservations_OnReservationsChanged(object sender, string fileName)
        {
            if (!Settings.Instance.LocalUseOnly && !FtpHandler.TryUploadBackupFile(fileName))
            {
                ShowFailMessage(string.Format(Constants.ErrorRemoteFileUpload, fileName));
            }
            if (!FileHandler.WriteToFile(fileName, JsonHandler.ReservationsJsonStrings(Reservations)))
            {
                ShowFailMessage(string.Format(Constants.ErrorWriteFile, fileName));
                return;
            }
            if (sender is Reservation[] reservations)
            {
                if (reservations[0] == null)
                {
                    Logging.Instance.WriteLine("Add reservation:");
                    Logging.Instance.WriteLine($"{reservations[1]}", true);
                }
                else
                {
                    Logging.Instance.WriteLine("Edit reservation:");
                    Logging.Instance.WriteLine($"Old: {reservations[0]}", true);
                    Logging.Instance.WriteLine($"New: {reservations[1]}", true);
                }
            }
            if (!Settings.Instance.LocalUseOnly && !FtpHandler.TryUploadFileByName(fileName))
            {
                ShowFailMessage(string.Format(Constants.ErrorRemoteFileUpload, fileName));
            }
        }

        private bool ReadSettingsFile()
        {
            string configFileName = Path.Combine(Constants.LocalPath, Constants.ConfigFileName);
            if (!FileHandler.FileExists(configFileName)) return ShowSettingsWindow();
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using Stream reader = new FileStream(configFileName, FileMode.Open);
            Settings.CreateSettings((Settings)serializer.Deserialize(reader));
            return true;
        }

        private void WriteSettingsFile()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            using Stream writer = new FileStream(Path.Combine(Constants.LocalPath, Constants.ConfigFileName), FileMode.Create);
            serializer.Serialize(writer, Settings.Instance);
        }
    }
}