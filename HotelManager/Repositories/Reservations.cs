using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelManager.Models;
using HotelManager.Utils;

namespace HotelManager.Repositories
{
    public class Reservations : List<Reservation>
    {
        public int NextReservationId => Count + 1;
        public Reservations(IEnumerable<Reservation> reservations)
        {
            foreach (Reservation reservation in reservations)
            {
                Add(reservation);
            }
        }

        public bool IsFromBooking(int id) => this[id - 1].Source == Source.Booking;

        public Reservation GetReservation(int id) => this[id - 1];

        public Reservation GetReservation(int room, DateTime date)
        {
            return Find(r => r.State != State.Canceled && r.Room == room && r.Period.ContainsDate(date));
        }

        public DateTime? GetLastFreeDate(int room, DateTime startDate)
        {
            Period period = new Period(startDate.AddDays(1), Settings.Instance.SeasonEndDate);
            DateTime maxDate = period.EndDate;
            foreach (Reservation reservation in FindAll(r => r.IsMatchingRoomAndPeriod(room, period)))
            {
                if (reservation.Period.StartDate < maxDate) maxDate = reservation.Period.StartDate;
            }
            return GetReservation(room, maxDate)?.Period.StartDate;
        }

        public IEnumerable<int> SearchInGuestName(string searchCriteria, bool excludeCanceled)
        {
            IEnumerable<Reservation> result = this.Where(r => !excludeCanceled || r.State != State.Canceled)
                .Where(r => r.GuestName.Contains(searchCriteria));
            foreach (Reservation reservation in result) yield return reservation.Id;
        }

        public IEnumerable<int> SearchInNotes(string searchCriteria, bool excludeCanceled)
        {
            IEnumerable<Reservation> result = this.Where(r => !excludeCanceled || r.State != State.Canceled)
                .Where(r => r.Notes.Contains(searchCriteria));
            foreach (Reservation reservation in result) yield return reservation.Id;
        }

        public IEnumerable<int> SearchInStartDateIncluded(DateTime? searchStartDate, DateTime? searchEndDate, bool excludeCanceled)
        {
            IEnumerable<Reservation> result = this.Where(r => !excludeCanceled || r.State != State.Canceled)
                .Where(r => r.Period.StartDate >= searchStartDate && r.Period.StartDate <= searchEndDate);
            foreach (Reservation reservation in result) yield return reservation.Id;
        }

        public IEnumerable<int> SearchInEndDateIncluded(DateTime? searchStartDate, DateTime? searchEndDate, bool excludeCanceled)
        {
            IEnumerable<Reservation> result = this.Where(r => !excludeCanceled || r.State != State.Canceled)
                .Where(r => r.Period.EndDate >= searchStartDate && r.Period.EndDate <= searchEndDate);
            foreach (Reservation reservation in result) yield return reservation.Id;
        }

        public IEnumerable<int> SearchInAllDatesIncluded(DateTime? searchStartDate, DateTime? searchEndDate, bool excludeCanceled)
        {
            IEnumerable<Reservation> result = this.Where(r => !excludeCanceled || r.State != State.Canceled)
                .Where(r => r.Period.StartDate >= searchStartDate && r.Period.EndDate <= searchEndDate);
            foreach (Reservation reservation in result) yield return reservation.Id;
        }

        public decimal SumOfReservationsWithIds(IEnumerable<int> ids)
        {
            return ids.Sum(id => this[id - 1].Sums.Total);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Reservation reservation in this)
            {
                sb.AppendLine($"{reservation}");
            }
            return sb.ToString();
        }
    }
}