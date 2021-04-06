using System;
using HotelManager.Repositories;

namespace HotelManager.Controller
{
    public interface IController
    {
        event EventHandler<string> OnReservationsChanged;
        event EventHandler<DateTime> OnReservationAdd;
        event EventHandler<int> OnReservationEdit;
        Reservations Reservations { get; }
        Rooms Rooms { get; }
        void RequestReservationWindow(int room, DateTime startDate, int? id = null);
        void SaveReservation(int id, int state, int source, int room, string name, DateTime startDate, int nights, int guests, decimal total, decimal paid, string notes);
        bool GetReservationInfo(int room, DateTime startDate, out string guestName, out int guestsNum, out decimal remainingSum);
        DateTime ReservationStartDate(int room, DateTime date);
        DateTime ReservationEndDate(int room, DateTime date);
        int ReservationNights(int room, DateTime date);
        bool IsValidReservation(int room, DateTime startDate);
        bool IsReservationCheckedIn(int room, DateTime startDate);
        bool IsReservationOverlapping(int room, DateTime startDate);
        bool CanExecuteCheckIn(int room, DateTime startDate);
        void CheckInReservation(int room, DateTime startDate);
        string GetTooltipText(int room, DateTime startDate);
    }
}