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
        public Sums Sums { get; set; }
        public int GuestsInRoom { get; set; }
        public string AdditionalInformation { get; set; }

        public Reservation(int id, bool status, int room, string guestName, Period period, int guestsInRoom, Sums sums, string additionalInfo)
        {
            Id = id;
            Status = status;
            Room = room;
            GuestName = guestName;
            Period = period;
            GuestsInRoom = guestsInRoom;
            Sums = sums;
            AdditionalInformation = additionalInfo;
        }

        public bool IsMatchingRoomAndDate(int room, DateTime date)
        {
            return Status && Room == room && Period.ContainsDate(date);
        }

        public override string ToString()
        {
            return $"{Id}|{Status}|{Room}|{GuestName}|{Period}|{GuestsInRoom}|{Sums}|{AdditionalInformation}";
        }
    }
}