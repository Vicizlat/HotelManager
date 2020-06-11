using System;
using System.Collections.Generic;
using System.Linq;

namespace HotelManager
{
    public delegate void ReservationsEventHandler();

    public class Reservations
    {
        public static Reservations Instance => thisInstance ?? new Reservations();
        private static Reservations thisInstance;
        public event ReservationsEventHandler ReservationsUpdated;
        private List<Reservation> resList;
        public int Count => resList.Count;

        public Reservations()
        {
            thisInstance = this;
            resList = new List<Reservation>();
        }

        public void LoadReservations(string fileName = "Reservations")
        {
            resList.Clear();
            foreach (string line in FileHandler.ReadFromFile(fileName))
            {
                try
                {
                    string[] lineArr = line.Split(new string[] { "|", " |", "| ", " | " }, StringSplitOptions.RemoveEmptyEntries);
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
                    resList.Add(new Reservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo));
                }
                catch
                {
                    Logging.Instance.WriteLine($"Failed to read line: {line}");
                    continue;
                }
            }
        }

        public void AddReservation(int id, bool status, int room, string guestName, DateTime startDate, DateTime endDate, int guestsInRoom, decimal totalPrice, decimal paidSum, string additionalInfo)
        {
            if (GetReservation(id) != null) EditReservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo);
            else resList.Add(new Reservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo));
            OnReservationsUpdated();
            SaveReservations();
        }

        public void EditReservation(int id, bool status, int room, string guestName, DateTime startDate, DateTime endDate, int guestsInRoom, decimal totalPrice, decimal paidSum, string additionalInfo)
        {
            int index = resList.IndexOf(GetReservation(id));
            resList[index].Status = status;
            resList[index].Room = room;
            resList[index].GuestName = guestName;
            resList[index].StartDate = startDate;
            resList[index].EndDate = endDate;
            resList[index].GuestsInRoom = guestsInRoom;
            resList[index].TotalPrice = totalPrice;
            resList[index].PaidSum = paidSum;
            resList[index].AdditionalInformation = additionalInfo;
        }

        public void SaveReservations()
        {
            FileHandler.WriteToFile("Reservations", ReservationsString());
            FtpHandler.TryUploadFile("Reservations");
        }

        protected virtual void OnReservationsUpdated()
        {
            ReservationsUpdated?.Invoke();
        }

        public string[] ReservationsString()
        {
            return resList.Select(r => r.ToString()).ToArray();
        }

        public Reservation GetReservation(int id)
        {
            return resList.Find(r => r.Id == id);
        }

        public Reservation GetReservation(int room, DateTime date)
        {
            return resList.Find(r => r.Room == room && r.StartDate <= date && date < r.EndDate);
        }
    }
}