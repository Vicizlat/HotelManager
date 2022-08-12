using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManager.Controller;
using HotelManager.Models;

namespace HotelManager.Views.Templates
{
    public class RoomsTextBox : TextBox
    {
        public RoomsTextBox(MainController controller, RoomInfo room)
        {
            Text = room.DisplayName;
            IsReadOnly = true;
            Focusable = false;
            FontSize = 20;
            VerticalContentAlignment = VerticalAlignment.Center;
            Margin = new Thickness(0, 0, 0, room.LastOnFloor ? 20 : 0);
            Cursor = Cursors.Hand;
            MouseDoubleClick += delegate { controller.RequestReservationWindow(room.FullRoomNumber, DateTime.Now); };
        }
    }
}