using System;
using HotelManager.Controller;

namespace HotelManager.Views.Templates
{
    public class CommandParameter
    {
        public int Room;
        public DateTime StartDate;
        public MainController Controller;
        public bool CanExecute;

        public CommandParameter(int room, DateTime startDate, MainController controller)
        {
            Room = room;
            StartDate = startDate;
            Controller = controller;
        }

        public CommandParameter(int room, DateTime startDate, MainController controller, bool canExecute)
        {
            Room = room;
            StartDate = startDate;
            Controller = controller;
            CanExecute = canExecute;
        }
    }
}