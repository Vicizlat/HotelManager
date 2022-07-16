using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManager.Controller;

namespace HotelManager.Views.Templates
{
    public class RoomsTextBox : TextBox
    {
        public RoomsTextBox(MainController controller, Tuple<string, int, bool> room)
        {
            Text = room.Item1;
            IsReadOnly = true;
            Focusable = false;
            FontSize = 20;
            VerticalContentAlignment = VerticalAlignment.Center;
            Margin = new Thickness(0, 0, 0, room.Item3 ? 20 : 0);
            Cursor = Cursors.Hand;
            MouseDoubleClick += delegate { controller.RequestReservationWindow(room.Item2, DateTime.Now); };
        }
    }
}