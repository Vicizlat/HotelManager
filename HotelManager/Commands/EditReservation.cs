using System;
using System.Windows.Input;

namespace HotelManager
{
    internal class EditReservation : ICommand
    {
        private Reservation Reservation { get; set; }

        public EditReservation(Reservation reservation)
        {
            Reservation = reservation;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return Reservation != null;
        }

        public void Execute(object parameter)
        {
            if (Reservation != null) new ReservationWindow(Reservation).ShowDialog();
        }
    }
}