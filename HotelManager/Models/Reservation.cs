using HotelManager.Data.Models.Enums;
using HotelManager.Views.Templates;

namespace HotelManager.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public State State { get; set; }
        public Source Source { get; set; }
        public int Room { get; set; }
        public string GuestName { get; set; }
        public Period Period { get; set; }
        public Sums Sums { get; set; }
        public int GuestsInRoom { get; set; }
        public string Notes { get; set; }

        public Reservation() { }

        public Reservation(Reservation reservation)
        {
            Id = reservation.Id;
            State = reservation.State;
            Source = reservation.Source;
            Room = reservation.Room;
            GuestName = reservation.GuestName;
            Period = reservation.Period;
            GuestsInRoom = reservation.GuestsInRoom;
            Sums = reservation.Sums;
            Notes = reservation.Notes;
        }

        public Reservation(int id, State state, Source source, int room, string name, Period period, int guests, Sums sums, string notes)
        {
            Id = id;
            State = state;
            Source = source;
            Room = room;
            GuestName = name;
            Period = period;
            GuestsInRoom = guests;
            Sums = sums;
            Notes = notes;
        }

        public Reservation(ReservationInfo resInfo)
        {
            Period period = new Period(resInfo.StartDate, resInfo.EndDate);
            Sums sums = new Sums(resInfo.TotalSum, resInfo.PaidSum);
            Id = resInfo.Id;
            State = (State)resInfo.StateInt;
            Source = (Source)resInfo.SourceInt;
            Room = resInfo.Room;
            GuestName = resInfo.GuestName;
            Period = period;
            GuestsInRoom = resInfo.NumberOfGuests;
            Sums = sums;
            Notes = resInfo.Notes;
        }

        public override string ToString()
        {
            return $"Номер: {Id} | Състояние: {State} | Източник: {Source} | Стая: {Room} | Гост: {GuestName} | Период: {Period} | Гости: {GuestsInRoom} | Суми: {Sums} | Бележки: {Notes}";
        }
    }
}