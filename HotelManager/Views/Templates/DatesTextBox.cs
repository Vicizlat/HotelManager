using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManager.Views.Templates
{
    public class DatesTextBox : TextBox
    {
        public DatesTextBox(DateTime date)
        {
            Text = $"{date.ToShortDateString()}";
            IsReadOnly = true;
            FontSize = 20;
            VerticalContentAlignment = VerticalAlignment.Center;
            HorizontalContentAlignment = HorizontalAlignment.Center;
            bool dateIsWeekend = date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
            Background = dateIsWeekend ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray);
            Cursor = Cursors.Arrow;
            Focusable = false;
            IsInactiveSelectionHighlightEnabled = false;
            IsHitTestVisible = false;
        }
    }
}