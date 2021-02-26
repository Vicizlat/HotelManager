using System;
using System.Windows.Controls;
using HotelManager.Controller;
using HotelManager.Views.Templates.Commands;

namespace HotelManager.Views.Templates
{
    public static class MenuItems
    {
        public static MenuItem AddReservation(int room, DateTime startDate, IController controller)
        {
            return new MenuItem
            {
                Header = "Добави резервация",
                Command = new AddReservation(room, startDate, controller)
            };
        }

        public static MenuItem EditReservation(int room, DateTime startDate, IController controller)
        {
            return new MenuItem
            {
                Header = "Редактирай резервация",
                Command = new EditReservation(room, startDate, controller)
            };
        }

        public static MenuItem CheckInReservation(int room, DateTime startDate, IController controller)
        {
            return new MenuItem
            {
                Header = "Настани резервация",
                Command = new CheckInReservation(room, startDate, controller)
            };
        }
    }
}