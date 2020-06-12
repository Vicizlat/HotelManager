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
                    string[] lineArr = line.Split(new [] { "|", " |", "| ", " | " }, StringSplitOptions.RemoveEmptyEntries);
                    int id = int.Parse(lineArr[0].Trim());
                    bool status = bool.Parse(lineArr[1].Trim());
                    int room = int.Parse(lineArr[2].Trim());
                    string guestName = lineArr[3];
                    DateTime startDate = DateTime.Parse(lineArr[4].Trim());
                    DateTime endDate = DateTime.Parse(lineArr[5].Trim());
                    int guestsInRoom = int.Parse(lineArr[6].Trim());
                    decimal totalPrice = decimal.Parse(lineArr[7].Trim());
                    decimal paidSum = decimal.Parse(lineArr[8].Trim());
                    string additionalInfo = lineArr[9];
                    ResList.Add(new Reservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo));
                }
                catch
                {
                    Logging.Instance.WriteLine($"Failed to read line: {line}");
                }
            }
        }

        public void AddReservation(int id, bool status, int room, string guestName, DateTime startDate, DateTime endDate, int guestsInRoom, decimal totalPrice, decimal paidSum, string additionalInfo)
        {
            if (GetReservation(id) != null) EditReservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo);
            else ResList.Add(new Reservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo));
            OnReservationsUpdated();
            SaveReservations();
        }

        protected virtual void OnReservationsUpdated() => ReservationsUpdated?.Invoke();

        public void EditReservation(int id, bool status, int room, string guestName, DateTime startDate, DateTime endDate, int guestsInRoom, decimal totalPrice, decimal paidSum, string additionalInfo)
        {
            int index = ResList.IndexOf(GetReservation(id));
            ResList[index].Status = status;
            ResList[index].Room = room;
            ResList[index].GuestName = guestName;
            ResList[index].StartDate = startDate;
            ResList[index].EndDate = endDate;
            ResList[index].GuestsInRoom = guestsInRoom;
            ResList[index].TotalPrice = totalPrice;
            ResList[index].PaidSum = paidSum;
            ResList[index].AdditionalInformation = additionalInfo;
        }

        public void SaveReservations()
        {
            FileHandler.WriteToFile("Reservations", ReservationsString());
            FtpHandler.TryUploadFile("Reservations");
        }

        public string[] ReservationsString()
        {
            return ResList.Select(r => r.ToString()).ToArray();
        }

        public Reservation GetReservation(int id)
        {
            return ResList.Find(r => r.Id == id);
        }

        public Reservation GetReservation(int room, DateTime date)
        {
            return ResList.Find(r => r.Room == room && r.StartDate <= date && date < r.EndDate);
        }

        public void RequestReservationWindow(int room, DateTime startDate) => OnReservationWindow1Requested(room, startDate);

        public void RequestReservationWindow(Reservation reservation) => OnReservationWindow2Requested(reservation);

        protected virtual void OnReservationWindow1Requested(int room, DateTime startDate) => ReservationWindow1Requested?.Invoke(room, startDate);

        protected virtual void OnReservationWindow2Requested(Reservation reservation) => ReservationWindow2Requested?.Invoke(reservation);
    }
}