using System;
using System.Windows.Input;
using HotelManager.Controller;

namespace HotelManager.Views.Templates.Commands
{
    public class AddReservation : ICommand
    {
        private readonly IController controller;
        private int Room { get; }
        private DateTime StartDate { get; }

        public AddReservation(int room, DateTime startDate, IController controller)
        {
            Room = room;
            StartDate = startDate;
            this.controller = controller;
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter) => !controller.IsValidReservation(Room, StartDate);

        public void Execute(object parameter) => controller.RequestReservationWindow(Room, StartDate);
    }
}