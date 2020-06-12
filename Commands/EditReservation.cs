using System;
using System.Windows.Input;
using Core;

namespace Commands
{
    public class EditReservation : ICommand
    {
        private Reservation Reservation { get; set; }

        public EditReservation(Reservation reservation)
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
            return Reservation != null;
        }

        public void Execute(object parameter)
        {
            if (Reservation != null) Reservations.Instance.RequestReservationWindow(Reservation);
        }
    }
}