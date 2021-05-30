using System;
using System.Linq;
using System.Text;
using HotelManager.Controller;
using HotelManager.Data.Models;
using HotelManager.Data.Models.Enums;

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
        public bool IsCheckedIn => StateInt == (int)State.CheckedIn;
        public bool IsFromBooking => SourceInt == (int)Source.Booking;

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
    }
}