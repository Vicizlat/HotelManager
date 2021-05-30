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
        public ReservationTextBox(MainController controller, ReservationInfo resInfo, bool lastOnFloor)
        {
            IsReadOnly = true;
            FontSize = 18;
            VerticalContentAlignment = VerticalAlignment.Center;
            Margin = new Thickness(0, 0, 0, lastOnFloor ? 20 : 0);
            Cursor = Cursors.Hand;
            BorderThickness = new Thickness(1.5);
            if (!string.IsNullOrEmpty(resInfo.GuestName))
            {
                decimal remainingSum = resInfo.TotalSum - resInfo.PaidSum;
                Text = string.Format(Constants.ReservationText, resInfo.GuestName, resInfo.NumberOfGuests,
                    remainingSum);
                ToolTip = resInfo.ToString();
                bool isCheckedIn = resInfo.IsCheckedIn;
                bool isOverlapping = controller.NextReservationStartDate(resInfo.Room, resInfo.StartDate) <
                                     resInfo.EndDate;
                Color bgColor = isOverlapping ? Colors.Red : isCheckedIn ? Colors.DarkBlue : Colors.AntiqueWhite;
                Background = new SolidColorBrush(bgColor);
                Color fgColor = isCheckedIn ? Colors.AntiqueWhite : Colors.DarkBlue;
                Foreground = new SolidColorBrush(fgColor);
            }

            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(MenuItems.AddEditReservation(resInfo, controller));
            ContextMenu.Items.Add(MenuItems.CheckInReservation(resInfo, controller));
            MouseDoubleClick += delegate { controller.RequestReservationWindow(resInfo.Room, resInfo.StartDate); };
        }

        public ReservationTextBox(MainController controller, int id, string reservationString)
        {
            IsReadOnly = true;
            FontSize = 14;
            VerticalContentAlignment = VerticalAlignment.Center;
            Cursor = Cursors.Hand;
            Text = reservationString;
            ToolTip = Text.Replace("|", Environment.NewLine);
            MouseDoubleClick += delegate { controller.RequestReservationWindow(id); };
        }
    }
}