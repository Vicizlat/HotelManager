using System;
using System.Windows.Input;
using Core;

namespace Commands
{
    public class AddReservation : ICommand
    {
        private int Room { get; set; }
        private DateTime StartDate { get; set; }

        public AddReservation(int room, DateTime startDate)
        {
            Room = room;
            StartDate = startDate;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            Reservation reservation = Reservations.Instance.GetReservation(Room, StartDate);
            return reservation == null || !reservation.Status;
        }

        public void Execute(object parameter)
        {
            Reservations.Instance.RequestReservationWindow(Room, StartDate);
        }
    }
}