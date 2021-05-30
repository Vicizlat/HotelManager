using System.Windows.Controls;
using HotelManager.Controller;
using HotelManager.Data.Models.Enums;
using HotelManager.Views.Templates.Commands;

namespace HotelManager.Views.Templates
{
    public static class MenuItems
    {
        public static MenuItem AddEditReservation(ReservationInfo resInfo, MainController controller)
        {
            if (resInfo.StateInt < 0) return new MenuItem
                {
                    Header = "Добави резервация",
                    CommandParameter = new CommandParameter(resInfo.Room, resInfo.StartDate, controller),
                    Command = new AddEditReservation(),
                    FontSize = 16
                };
            else return new MenuItem
                {
                    Header = "Редактирай резервация",
                    CommandParameter = new CommandParameter(resInfo.Room, resInfo.StartDate, controller),
                    Command = new AddEditReservation(),
                    FontSize = 16
                };
        }

        public static MenuItem CheckInReservation(ReservationInfo resInfo, MainController controller)
        {
            bool canExecute = resInfo.StateInt == (int)State.Active;
            return new MenuItem
            {
                Header = "Настани резервация",
                CommandParameter = new CommandParameter(resInfo.Room, resInfo.StartDate, controller, canExecute),
                Command = new CheckInReservation(),
                FontSize = 16
            };
        }
    }
}