using System;
using System.Windows.Input;
using Core;

namespace Templates.Commands
{
    public class CheckInReservation : ICommand
    {
        private Reservation Reservation { get; }

        public CheckInReservation(Reservation reservation)
        {
            Reservation = reservation;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            return Reservation != null && Reservation.ReservationState != (int)State.CheckedIn;
        }

        public void Execute(object parameter)
        {
            Reservation.ReservationState = (int)State.CheckedIn;
            Reservations.Instance.SaveReservation(Reservation);
        }
    }
}