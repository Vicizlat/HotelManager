using System;

namespace Core
{
    public class Reservation
    {
        public int Id { get; set; }
        public bool Status { get; set; }
        public int Room { get; set; }
        public string GuestName { get; set; }
        public Period Period { get; set; }
        public int GuestsInRoom { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PaidSum { get; set; }
        public decimal RemainingSum => TotalPrice - PaidSum;
        public string AdditionalInformation { get; set; }

        public Reservation(int id, bool status, int room, string guestName, DateTime[] dates, int guestsInRoom, decimal totalPrice, decimal paidSum, string additionalInfo)
        {
            Id = id;
            Status = status;
            Room = room;
            GuestName = guestName;
            Period = new Period(dates[0], dates[1]);
            GuestsInRoom = guestsInRoom;
            TotalPrice = totalPrice;
            PaidSum = paidSum;
            AdditionalInformation = additionalInfo;
        }

        public bool IsMatchingRoomAndDate(int room, DateTime date)
        {
            return Room == room && Period.ContainsDate(date);
        }

        public override string ToString()
        {
            return $"{Id}|{Status}|{Room}|{GuestName}|{Period}|{GuestsInRoom}|{TotalPrice}|{PaidSum}|{AdditionalInformation}";
        }
    }
}