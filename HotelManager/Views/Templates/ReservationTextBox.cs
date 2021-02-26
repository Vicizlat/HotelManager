using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HotelManager.Controller;
using HotelManager.Utils;

namespace HotelManager.Views.Templates
{
    public class ReservationTextBox : TextBox
    {
        private readonly IController controller;
        private readonly int room;
        private readonly int? id;
        private readonly DateTime startDate;

        public ReservationTextBox(IController controller, int row, DateTime startDate)
        {
            this.controller = controller;
            room = controller.Rooms.TopFloorFirst[row].FullRoomNumber;
            this.startDate = startDate;
            IsReadOnly = true;
            FontSize = 18;
            VerticalContentAlignment = VerticalAlignment.Center;
            Margin = new Thickness(0, 0, 0, controller.Rooms.TopFloorFirst[row].LastOnFloor ? 20 : 0);
            Cursor = Cursors.Hand;
            BorderThickness = new Thickness(1.5);
            if (controller.GetReservationInfo(room, startDate, out string guestName, out int guestsNum, out decimal remainingSum))
            {
                Text = string.Format(Constants.ReservationText, guestName, guestsNum, remainingSum);
                ToolTip = controller.GetTooltipText(room, startDate);
                DateTime resEndDate = controller.ReservationEndDate(room, startDate);
                bool isCheckedIn = controller.IsReservationCheckedIn(room, startDate);
                bool isOverlapping = controller.IsReservationOverlapping(room, resEndDate);
                Background = new SolidColorBrush(isOverlapping ? Colors.Red : isCheckedIn ? Colors.DarkBlue : Colors.AntiqueWhite);
                Foreground = new SolidColorBrush(isCheckedIn ? Colors.AntiqueWhite : Colors.DarkBlue);
            }

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(MenuItems.AddReservation(room, startDate, controller));
            ContextMenu.Items.Add(MenuItems.EditReservation(room, startDate, controller));
            ContextMenu.Items.Add(MenuItems.CheckInReservation(room, startDate, controller));
            MouseDoubleClick += ReservationTextBox_MouseDoubleClick;
        }

        public ReservationTextBox(IController controller, int id)
        {
            this.controller = controller;
            this.id = id;
            IsReadOnly = true;
            FontSize = 14;
            VerticalContentAlignment = VerticalAlignment.Center;
            Cursor = Cursors.Hand;
            Text = $"{controller.Reservations[id - 1]}";
            ToolTip = $"{controller.Reservations[id - 1].ToString().Replace("|", Environment.NewLine)}";
            MouseDoubleClick += ReservationTextBox_MouseDoubleClick;
        }

        private void ReservationTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            controller.RequestReservationWindow(room, startDate, id);
        }
    }
}