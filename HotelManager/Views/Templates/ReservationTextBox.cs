using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HotelManager.Controller;
using HotelManager.Models;
using HotelManager.Utils;

namespace HotelManager.Views.Templates
{
    public class ReservationTextBox : TextBox
    {
        public ReservationTextBox(MainController controller, ReservationInfo resInfo, bool lastOnFloor)
        {
            IsReadOnly = true;
            Focusable = false;
            FontSize = 18;
            VerticalContentAlignment = VerticalAlignment.Center;
            Margin = new Thickness(0, 0, 0, lastOnFloor ? 20 : 0);
            Cursor = Cursors.Hand;
            BorderThickness = new Thickness(1.5);
            if (resInfo.Guest != null)
            {
                decimal remainingSum = resInfo.TotalSum - resInfo.PaidSum;
                string pref = string.Empty;
                if (resInfo.Guest.ResCount > 1) pref = $"({resInfo.Guest.ResCount})";
                Text = string.Format(Constants.ReservationText, pref + resInfo.Guest.GetFullName(), resInfo.NumberOfGuests, remainingSum);
                ToolTip = resInfo.ToString();
                bool isCheckedIn = resInfo.StateInt == 1;
                bool isOverlapping = controller.NextReservationStartDate(resInfo.Room, resInfo.StartDate) < resInfo.EndDate;
                Color bgColor = isOverlapping ? Colors.Red : isCheckedIn ? Colors.LightBlue : Colors.AntiqueWhite;
                Background = new SolidColorBrush(bgColor);
            }
            ContextMenu = new ContextMenu();
            ContextMenu.Items.Add(MenuItems.AddEditReservation(resInfo, controller));
            ContextMenu.Items.Add(MenuItems.CheckInReservation(resInfo, controller));
            MouseDoubleClick += delegate { controller.RequestReservationWindow(resInfo.Room, resInfo.StartDate); };
        }
    }
}