﻿using System;
using System.Collections.Generic;
using System.Linq;
using Handlers;

namespace Core
{
    public class Reservations : List<Reservation>
    {
        public static Reservations Instance => thisInstance ?? new Reservations();
        private static Reservations thisInstance;
        public event ReservationsEventHandler OnReservationsChanged;
        public event AddReservationEventHandler AddReservationWindowRequested;
        public event EditReservationEventHandler EditReservationWindowRequested;

        public Reservations() => thisInstance = this;

        public void SaveReservation(Reservation reservation)
        {
            if (GetReservation(reservation.Id) == null) Add(reservation);
            else this[reservation.Id - 1] = reservation;
            if (FileHandler.WriteToFile("Reservations", this.ToStringArray())) FtpHandler.TryUploadFile("Reservations");
            OnReservationsUpdated();
        }

        public string[] ToStringArray() => this.Select(r => r.ToString()).ToArray();

        public Reservation GetReservation(int id) => this.Find(r => r.Id == id);

        public Reservation GetReservation(int room, DateTime date) => this.Find(r => r.IsMatchingRoomAndDate(room, date));

        public Reservation GetReservation(int room, Period period)
        {
            DateTime maxDate = period.EndDate;
            foreach (Reservation reservation in this.FindAll(r => r.IsMatchingRoomAndPeriod(room, period)))
            {
                if (reservation.Period.StartDate < maxDate) maxDate = reservation.Period.StartDate;
            }
            return this.Find(r => r.IsMatchingRoomAndDate(room, maxDate));
        }

        public void RequestReservationWindow(int room, DateTime startDate) => OnReservationWindowRequested(room, startDate);

        public void RequestReservationWindow(Reservation reservation) => OnReservationWindowRequested(reservation);

        protected virtual void OnReservationsUpdated() => OnReservationsChanged?.Invoke();

        protected virtual void OnReservationWindowRequested(int room, DateTime startDate) => AddReservationWindowRequested?.Invoke(room, startDate);

        protected virtual void OnReservationWindowRequested(Reservation reservation) => EditReservationWindowRequested?.Invoke(reservation);
    }
}