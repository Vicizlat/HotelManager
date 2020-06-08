using System;
using System.Collections.Generic;
using System.IO;

namespace HotelManager
{
    public class Reservations
    {
        private static Reservations ThisInstance { get; set; }
        public static Reservations Instance => ThisInstance ?? new Reservations();
        public List<Reservation> List { get; set; }
        public int Count => List.Count;

        public Reservations()
        {
            List = new List<Reservation>();
            ThisInstance = this;
            if (FileHandler.IsLocalFileNewer()) LoadReservations();
            else if (FileHandler.TryGetRemoteFile()) SaveReservations(false);
            else File.WriteAllLines(Path.Combine(FileHandler.LocalPath, "Reservations"), ReservationsStringLines());
        }

        public void LoadReservations(string fileName = "Reservations")
        {
            List.Clear();
            foreach (string line in File.ReadAllLines(Path.Combine(FileHandler.LocalPath, fileName)))
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
                    List.Add(new Reservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo));
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
            else List.Add(new Reservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo));
        }

        public void EditReservation(int id, bool status, int room, string guestName, DateTime startDate, DateTime endDate, int guestsInRoom, decimal totalPrice, decimal paidSum, string additionalInfo)
        {
            int index = List.IndexOf(GetReservation(id));
            List[index].Status = status;
            List[index].Room = room;
            List[index].GuestName = guestName;
            List[index].StartDate = startDate;
            List[index].EndDate = endDate;
            List[index].GuestsInRoom = guestsInRoom;
            List[index].TotalPrice = totalPrice;
            List[index].PaidSum = paidSum;
            List[index].AdditionalInformation = additionalInfo;
        }

        public void SaveReservations(bool uploadLocal = true)
        {
            File.WriteAllLines(Path.Combine(FileHandler.LocalPath, "Reservations"), ReservationsStringLines());
            if (uploadLocal) FileHandler.TryUploadFile("Reservations");
            MainWindow.Instance.CreateReservationsTable();
        }

        public string[] ReservationsStringLines()
        {
            string[] lines = new string[Count];
            for (int i = 0; i < Count; i++) lines[i] = List[i].ToString();
            return lines;
        }

        public Reservation GetReservation(int id)
        {
            return List.Find(r => r.Id == id);
        }

        public Reservation GetReservation(int room, DateTime date)
        {
            return List.Find(r => r.Room == room && r.StartDate <= date && date < r.EndDate);
        }
    }
}