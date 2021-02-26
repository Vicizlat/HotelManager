using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManager.Controller;

namespace HotelManager.Views.Templates
{
    public class RoomsTextBox : TextBox
    {
        private readonly IController controller;
        private readonly int room;

        public RoomsTextBox(IController controller, int row)
        {
            this.controller = controller;
            room = controller.Rooms.TopFloorFirst[row].FullRoomNumber;
            Text = $"{controller.Rooms.TopFloorFirst[row]}";
            IsReadOnly = true;
            Focusable = false;
            FontSize = 20;
            VerticalContentAlignment = VerticalAlignment.Center;
            Margin = new Thickness(0, 0, 0, controller.Rooms.TopFloorFirst[row].LastOnFloor ? 20 : 0);
            Cursor = Cursors.Hand;
            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(MenuItems.AddReservation(room, DateTime.Now, controller));
            MouseDoubleClick += RoomsTextBox_MouseDoubleClick;
        }

        private void RoomsTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            controller.RequestReservationWindow(room, DateTime.Now);
        }
    }
}