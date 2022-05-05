using System;
using System.Linq;
using System.Text;
using HotelManager.Controller;
using HotelManager.Data.Models;

namespace HotelManager.Views.Templates
{
    public class ReservationInfo
    {
        public int Id { get; set; }
        public int StateInt { get; set; }
        public int SourceInt { get; set; }
        public int Room { get; set; }
        public string GuestName { get; set; }
        public string GuestReferrer { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalSum { get; set; }
        public decimal PaidSum { get; set; }
        public string Notes { get; set; }
        public int ResCount { get; set; }

        public ReservationInfo() { }

        public ReservationInfo(Reservation reservation)
        {
            Id = reservation.Id;
            StateInt = (int)reservation.State;
            SourceInt = (int)reservation.Source;
            Room = reservation.Room.FullRoomNumber;
            GuestName = $"{reservation.Guest.FirstName} {reservation.Guest.LastName}".Trim();
            string guestReferrer = $"{reservation.Guest.GuestReferrer?.FirstName} {reservation.Guest.GuestReferrer?.LastName}";
            GuestReferrer = guestReferrer.Trim();
            Email = reservation.Guest.Email;
            Phone = reservation.Guest.Phone;
            StartDate = reservation.StartDate;
            EndDate = reservation.EndDate;
            NumberOfGuests = reservation.NumberOfGuests;
            TotalSum = reservation.TotalSum;
            PaidSum = reservation.Transactions.Sum(t => t.PaidSum);
            Notes = reservation.Notes;
            ResCount = reservation.Guest.Reservations.Count;
        }

        public Reservation ToReservation(MainController controller)
        {
            Guest guest = controller.GetGuest(this);
            Reservation reservation = controller.GetReservation(Id) ?? new Reservation();
            controller.UpdateReservation(reservation, guest, this);
            return reservation;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine($"Име: {GuestName}")
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
                   && GuestName == other.GuestName
                   && GuestReferrer == other.GuestReferrer
                   && Email == other.Email
                   && Phone == other.Phone
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
            hashCode.Add(GuestName);
            hashCode.Add(GuestReferrer);
            hashCode.Add(Email);
            hashCode.Add(Phone);
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