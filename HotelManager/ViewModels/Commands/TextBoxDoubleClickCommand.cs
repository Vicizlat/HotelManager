using HotelManager.Controller;
using System;
using System.Windows.Input;

namespace HotelManager.ViewModels.Commands
{
    internal class TextBoxDoubleClickCommand : ICommand
    {
        private MainController controller;
        public int Id;

        public TextBoxDoubleClickCommand(MainController controller, int id)
        {
            this.controller = controller;
            Id = id;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => controller.RequestReservationWindow(Id);
    }
}