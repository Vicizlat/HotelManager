using System;
using System.Linq;
using System.Text;
using HotelManager.Data.Models;
using HotelManager.Controller;
using HotelManager.Utils;

namespace HotelManager.Models
{
    public class ReservationInfo
    {
        public int Id { get; set; }
        public int StateInt { get; set; }
        public int SourceInt { get; set; }
        public int Room { get; set; }
        public string RoomShortName { get; set; }
        public GuestInfo Guest { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalSum { get; set; }
        public decimal PaidSum { get; set; }
        public string Notes { get; set; }

        public ReservationInfo() { }

        public ReservationInfo(Reservation reservation)
        {
            Id = reservation.Id;
            StateInt = (int)reservation.State;
            SourceInt = (int)reservation.Source;
            Room = reservation.Room.FullRoomNumber;
            RoomShortName = reservation.Room.RoomTypeShort;
            Guest = new GuestInfo(reservation.Guest);
            StartDate = reservation.StartDate;
            EndDate = reservation.EndDate;
            NumberOfGuests = reservation.NumberOfGuests;
            TotalSum = reservation.TotalSum;
            PaidSum = reservation.Transactions.Sum(t => t.PaidSum);
            Notes = reservation.Notes;
        }

        public Reservation ToReservation(MainController controller)
        {
            Reservation reservation = controller.GetReservation(Id) ?? new Reservation();
            controller.UpdateReservation(reservation, this);
            return reservation;
        }

        public string[] ToSearchArray()
        {
            return new string[]
            {
                $"{Id}",
                Constants.ReservationStates[StateInt],
                Constants.ReservationSources[SourceInt],
                $"{RoomShortName} {Room}",
                Guest.GetFullName(),
                $"{StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}",
                $"{TotalSum} - {PaidSum} = {TotalSum - PaidSum}",
                Notes
            };
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine($"Гост: {Guest.GetFullName()}")
                .AppendLine($"Брой гости: {NumberOfGuests}")
                .AppendLine($"Период: {StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}")
                .AppendLine($"Обща цена: {TotalSum}")
                .AppendLine($"Предплатена сума: {PaidSum}")
                .AppendLine($"Оставаща сума: {TotalSum - PaidSum}")
                .Append($"Допълнителна информация: {Notes ?? "Няма"}");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            ReservationInfo other = (ReservationInfo)obj;
            return other != null
                   && Id == other.Id
                   && StateInt == other.StateInt
                   && SourceInt == other.SourceInt
                   && Room == other.Room
                   && Guest.Equals(other.Guest)
                   && StartDate.Equals(other.StartDate)
                   && EndDate.Equals(other.EndDate)
                   && NumberOfGuests == other.NumberOfGuests
                   && TotalSum == other.TotalSum
                   && PaidSum == other.PaidSum
                   && Notes == other.Notes;
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(Id);
            hashCode.Add(StateInt);
            hashCode.Add(SourceInt);
            hashCode.Add(Room);
            hashCode.Add(Guest.GetHashCode());
            hashCode.Add(StartDate);
            hashCode.Add(EndDate);
            hashCode.Add(NumberOfGuests);
            hashCode.Add(TotalSum);
            hashCode.Add(PaidSum);
            hashCode.Add(Notes);
            return hashCode.ToHashCode();
        }
    }
}