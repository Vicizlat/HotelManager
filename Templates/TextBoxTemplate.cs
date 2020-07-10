using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Core;

namespace Templates
{
    public class TextBoxTemplate
    {
        public TextBox ReservationsTextBox(int row, DateTime startDate, Reservation reservation)
        {
            TextBox textBox = new TextBox
            {
                Text = reservation == null ? string.Empty : StaticTemplates.GetReservationText(reservation),
                IsReadOnly = true,
                FontSize = 18,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = StaticTemplates.Margin(row),
                Cursor = Cursors.Hand,
                BorderThickness = new Thickness(1.5),
                ContextMenu = new ContextMenu()
            };
            if (reservation != null)
            {
                textBox.ToolTip = StaticTemplates.GetTooltipText(reservation);
                textBox.Background = IsOverlapingReservation(row, reservation.Period.EndDate) ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.AntiqueWhite);
            }
            textBox.ContextMenu.Items.Add(StaticTemplates.AddReservationMenuItem(row, startDate));
            textBox.ContextMenu.Items.Add(StaticTemplates.EditReservationMenuItem(reservation));
            return textBox;
        }

        private bool IsOverlapingReservation(int row, DateTime date)
        {
            Reservation res = Reservations.Instance.GetReservation(StaticTemplates.GetRoomNumber(row), date);
            return res != null && date != res.Period.StartDate;
        }

        public TextBox RoomsTextBox(int row)
        {
            TextBox textBox = new TextBox
            {
                Text = StaticTemplates.GetRoomText(row),
                IsReadOnly = true,
                FontSize = 20,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = StaticTemplates.Margin(row),
                Cursor = Cursors.Hand,
                ContextMenu = new ContextMenu()
            };
            textBox.ContextMenu.Items.Add(StaticTemplates.AddReservationMenuItem(row, DateTime.Now));
            return textBox;
        }

        public TextBox DatesTextBox(int col, DateTime startDate)
        {
            bool dateIsWeekend = startDate.AddDays(col).DayOfWeek == DayOfWeek.Saturday || startDate.AddDays(col).DayOfWeek == DayOfWeek.Sunday;
            TextBox textBox = new TextBox
            {
                Text = $"{startDate.AddDays(col).ToShortDateString()}",
                IsReadOnly = true,
                FontSize = 20,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Background = dateIsWeekend ? new SolidColorBrush(Colors.LightGreen) : new SolidColorBrush(Colors.LightGray),
                Cursor = Cursors.Arrow,
                Focusable = false,
                IsInactiveSelectionHighlightEnabled = false,
                IsHitTestVisible = false
            };
            return textBox;
        }
    }
}