using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManager.Controller;

namespace HotelManager.Views.Templates
{
    public class RoomsTextBox : TextBox
    {
        public RoomsTextBox(MainController controller, int room, bool lastOnFloor)
        {
            Text = $"{controller.Context.Rooms.FirstOrDefault(r => r.FullRoomNumber == room)}";
            IsReadOnly = true;
            Focusable = false;
            FontSize = 20;
            VerticalContentAlignment = VerticalAlignment.Center;
            Margin = new Thickness(0, 0, 0, lastOnFloor ? 20 : 0);
            Cursor = Cursors.Hand;
            MouseDoubleClick += delegate { controller.RequestReservationWindow(room, DateTime.Now); };
        }
    }
}