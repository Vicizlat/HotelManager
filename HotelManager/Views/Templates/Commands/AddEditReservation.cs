using System;
using System.Windows.Input;

namespace HotelManager.Views.Templates.Commands
{
    public class AddEditReservation : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            CommandParameter par = (CommandParameter)parameter;
            par.Controller.RequestReservationWindow(par.Room, par.StartDate);
        }
    }
}