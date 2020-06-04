using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManager
{
    public class TextBoxTemplate
    {
        public TextBox ReservationsTextBox(int row, DateTime startDate, Reservation reservation)
        {
            TextBox textBox = new TextBox()
            {
                Text = string.Empty,
                IsReadOnly = true,
                FontSize = 18,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = row == 0 || row == 8 || row == 16 ? new Thickness(0, 0, 0, 20) : new Thickness(0),
                Cursor = Cursors.Hand,
                BorderThickness = new Thickness(1.5),
                ContextMenu = new ContextMenu()
            };
            if (reservation != null)
            {
                textBox.Text = $"{reservation.GuestName} - {reservation.GuestsInRoom} гости - За плащане: {reservation.RemainingSum} лв.";
                textBox.ToolTip = $"Име: {reservation.GuestName}\n";
                textBox.ToolTip += $"Брой гости: {reservation.GuestsInRoom}\n";
                textBox.ToolTip += $"Период: {reservation.StartDate.ToShortDateString()} - {reservation.EndDate.ToShortDateString()}\n";
                textBox.ToolTip += $"Обща цена: {reservation.TotalPrice}\n";
                textBox.ToolTip += $"Предплатена сума: {reservation.PaidSum}\n";
                textBox.ToolTip += $"Оставаща сума: {reservation.RemainingSum}\n";
                textBox.ToolTip += $"Допълнителна информация: {reservation.AdditionalInformation}";
                textBox.Background = new SolidColorBrush(Colors.AntiqueWhite);
            }
            textBox.ContextMenu.Items.Add(AddReservationMenuItem(GetRoom(row), startDate));
            textBox.ContextMenu.Items.Add(EditReservationMenuItem(reservation));
            return textBox;
        }

        public TextBox RoomsTextBox(int row)
        {
            string text = "";
            if (row == 0) text = $"Апартамент 32";
            if (row >= 1 && row <= 8) text = $"Стая 2{row}";
            if (row >= 9 && row <= 16) text = $"Стая 1{row - 8}";
            if (row >= 17 && row <= 24) text = $"Стая 0{row - 16}";
            TextBox textBox = new TextBox()
            {
                Text = text,
                IsReadOnly = true,
                FontSize = 20,
                VerticalContentAlignment = VerticalAlignment.Center,
                Margin = row == 0 || row == 8 || row == 16 ? new Thickness(0, 0, 0, 20) : new Thickness(0),
                Cursor = Cursors.Hand,
                ContextMenu = new ContextMenu()
            };
            textBox.ContextMenu.Items.Add(AddReservationMenuItem(GetRoom(row), DateTime.Now));
            return textBox;
        }

        public TextBox DatesTextBox(int col, DateTime startDate)
        {
            bool dateIsWeekend = startDate.AddDays(col).DayOfWeek == DayOfWeek.Saturday || startDate.AddDays(col).DayOfWeek == DayOfWeek.Sunday;
            TextBox textBox = new TextBox()
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

        private int GetRoom(int row)
        {
            if (row == 0) return 32;
            else if (row >= 1 && row <= 8) return 20 + row;
            else if (row >= 9 && row <= 16) return 2 + row;
            else return row - 16;
        }

        private MenuItem AddReservationMenuItem(int room, DateTime startDate)
        {
            return new MenuItem()
            {
                Header = "Добави резервация",
                Command = new AddReservation(room, startDate)
            };
        }

        private MenuItem EditReservationMenuItem(Reservation reservation)
        {
            return new MenuItem()
            {
                Header = "Редактирай резервация",
                Command = new EditReservation(reservation)
            };
        }
    }
}