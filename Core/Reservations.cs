using System;
using System.Collections.Generic;
using System.Linq;
using Handlers;

namespace Core
{
    public class Reservations
    {
        public static Reservations Instance => thisInstance ?? new Reservations();
        private static Reservations thisInstance;
        private protected List<Reservation> ResList;
        public int Count => ResList.Count;
        public event ReservationsEventHandler ReservationsUpdated;
        public event ReservationWindow1EventHandler ReservationWindow1Requested;
        public event ReservationWindow2EventHandler ReservationWindow2Requested;

        public Reservations()
        {
            thisInstance = this;
            ResList = new List<Reservation>();
        }

        public void LoadReservations(string fileName = "Reservations")
        {
            ResList.Clear();
            foreach (string line in FileHandler.ReadFromFile(fileName))
            {
                try
                {
                    string[] lineArr = line.Split(new[] { "|", " |", "| ", " | " }, StringSplitOptions.RemoveEmptyEntries);
                    int id = int.Parse(lineArr[0].Trim());
                    bool status = bool.Parse(lineArr[1].Trim());
                    int room = int.Parse(lineArr[2].Trim());
                    string guestName = lineArr[3].Trim();
                    DateTime[] dates = { DateTime.Parse(lineArr[4].Trim()), DateTime.Parse(lineArr[5].Trim()) };
                    int guestsInRoom = int.Parse(lineArr[6].Trim());
                    decimal totalPrice = decimal.Parse(lineArr[7].Trim());
                    decimal paidSum = decimal.Parse(lineArr[8].Trim());
                    string additionalInfo = lineArr[9].Trim();
                    ResList.Add(new Reservation(id, status, room, guestName, dates, guestsInRoom, totalPrice, paidSum, additionalInfo));
                }
                catch
                {
                    Logging.Instance.WriteLine($"Failed to read line: {line}");
                }
            }
        }

        public void AddReservation(int id, bool status, int room, string guestName, DateTime[] dates, int guestsInRoom, decimal[] sums, string additionalInfo)
        {
            if (GetReservation(id) != null)
            {
                int index = ResList.IndexOf(GetReservation(id));
                ResList[index].Status = status;
                ResList[index].Room = room;
                ResList[index].GuestName = guestName;
                ResList[index].Period.StartDate = dates[0];
                ResList[index].Period.EndDate = dates[1];
                ResList[index].GuestsInRoom = guestsInRoom;
                ResList[index].TotalPrice = sums[0];
                ResList[index].PaidSum = sums[1];
                ResList[index].AdditionalInformation = additionalInfo;
            }
            else ResList.Add(new Reservation(id, status, room, guestName, dates, guestsInRoom, sums[0], sums[1], additionalInfo));
            OnReservationsUpdated();
            if (FileHandler.WriteToFile("Reservations", ReservationsString())) FtpHandler.TryUploadFile("Reservations");
        }

        protected virtual void OnReservationsUpdated() => ReservationsUpdated?.Invoke();

        public string[] ReservationsString() => ResList.Select(r => r.ToString()).ToArray();

        public Reservation GetReservation(int id) => ResList.Find(r => r.Id == id);

        public Reservation GetReservation(int room, DateTime date) => ResList.Find(r => r.IsMatchingRoomAndDate(room, date));

        public void RequestReservationWindow(int room, DateTime startDate) => OnReservationWindowRequested(room, startDate);

        public void RequestReservationWindow(Reservation reservation) => OnReservationWindowRequested(reservation);

        protected virtual void OnReservationWindowRequested(int room, DateTime startDate) => ReservationWindow1Requested?.Invoke(room, startDate);

        protected virtual void OnReservationWindowRequested(Reservation reservation) => ReservationWindow2Requested?.Invoke(reservation);
    }
}