using System;

namespace HotelManager
{
    public class Reservation
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public int Room { get; set; }
        public string GuestName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Nights => (EndDate - StartDate).Days;
        public int GuestsInRoom { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaidSum { get; set; }
        public decimal RemainingSum => TotalPrice - PaidSum;
        public string AdditionalInformation { get; set; }

        public Reservation(int id, bool status, int room, string guestName, DateTime startDate, DateTime endDate, int guestsInRoom, decimal totalPrice, decimal paidSum = 0, string additionalInfo = "")
        {
            Id = id;
            Status = status;
            Room = room;
            GuestName = guestName;
            StartDate = startDate;
            EndDate = endDate;
            GuestsInRoom = guestsInRoom;
            TotalPrice = totalPrice;
            PaidSum = paidSum;
            AdditionalInformation = additionalInfo;
        }

        public override string ToString()
        {
            return $"{Id}|{Status}|{Room}|{GuestName}|{StartDate.ToString()}|{EndDate.ToString()}|{GuestsInRoom}|{TotalPrice}|{PaidSum}|{AdditionalInformation}";
        }
    }
}