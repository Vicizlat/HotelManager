using System;

namespace Core
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ReservationState { get; set; }
        public int Room { get; set; }
        public string GuestName { get; set; }
        public Period Period { get; set; }
        public Sums Sums { get; set; }
        public int GuestsInRoom { get; set; }
        public string AdditionalInformation { get; set; }

        public Reservation(int id, int reservationState, int room, string guestName, Period period, int guestsInRoom, Sums sums, string additionalInfo)
        {
            Id = id;
            ReservationState = reservationState;
            Room = room;
            GuestName = guestName;
            Period = period;
            GuestsInRoom = guestsInRoom;
            Sums = sums;
            AdditionalInformation = additionalInfo;
        }

        public bool IsMatchingRoomAndDate(int room, DateTime date)
        {
            return ReservationState != (int)State.Canceled && Room == room && Period.ContainsDate(date);
        }

        public bool IsMatchingRoomAndPeriod(int room, Period period)
        {
            return ReservationState != (int)State.Canceled && Room == room && period.ContainsDate(Period.StartDate);
        }

        public override string ToString()
        {
            return $"{Id}|{ReservationState}|{Room}|{GuestName}|{Period}|{GuestsInRoom}|{Sums}|{AdditionalInformation}";
        }
    }
}