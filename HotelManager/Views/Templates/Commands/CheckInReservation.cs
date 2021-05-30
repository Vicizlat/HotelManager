using System;
using System.Windows.Input;

namespace HotelManager.Views.Templates.Commands
{
    public class CheckInReservation : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object parameter)
        {
            CommandParameter par = (CommandParameter)parameter;
            return par.CanExecute;
        }

        public void Execute(object parameter)
        {
            CommandParameter par = (CommandParameter)parameter;
            par.Controller.CheckInReservation(par.Room, par.StartDate);
        }
    }
}