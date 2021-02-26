using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManager.Controller;
using HotelManager.Utils;

namespace HotelManager.Views
{
    public partial class ReservationWindow
    {
        private readonly IController controller;
        private decimal totalPrice;
        private decimal paidSum;

        public ReservationWindow(int room, DateTime startDate, IController controller)
        {
            InitializeComponent();
            this.controller = controller;
            Id.Text = $"{controller.Reservations.NextReservationId}";
            State.SelectedIndex = 0;
            Source.SelectedIndex = 0;
            Room.SelectedIndex = controller.Rooms.GetRoomIndex(room);
            StartDate.SelectedDate = startDate;
            EndDate.DisplayDateStart = startDate.AddDays(1);
            EndDate.DisplayDateEnd = controller.Reservations.GetLastFreeDate(room, startDate) ?? Settings.Instance.SeasonEndDate;
        }

        public ReservationWindow(int id, IController controller)
        {
            InitializeComponent();
            this.controller = controller;
            Id.Text = $"{id}";
            State.SelectedIndex = (int)controller.Reservations.GetReservation(id).State;
            Source.SelectedIndex = (int)controller.Reservations.GetReservation(id).Source;
            Room.SelectedIndex = controller.Rooms.GetRoomIndex(controller.Reservations.GetReservation(id).Room);
            GuestName.Text = controller.Reservations.GetReservation(id).GuestName;
            StartDate.SelectedDate = controller.Reservations.GetReservation(id).Period.StartDate;
            EndDate.SelectedDate = controller.Reservations.GetReservation(id).Period.EndDate;
            Nights.Text = $"{controller.Reservations.GetReservation(id).Period.Nights}";
            GuestsInRoom.Text = $"{controller.Reservations.GetReservation(id).GuestsInRoom}";
            TotalPrice.Text = $"{controller.Reservations.GetReservation(id).Sums.Total}";
            PaidSum.Text = $"{controller.Reservations.GetReservation(id).Sums.Paid}";
            RemainingSum.Text = $"{controller.Reservations.GetReservation(id).Sums.Remaining}";
            Notes.Text = controller.Reservations.GetReservation(id).Notes == "Няма"
                ? string.Empty
                : controller.Reservations.GetReservation(id).Notes;
        }

        private void Room_SelectionChanged(object sender, SelectionChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void GenericText_TextChanged(object sender, TextChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void Dates_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDate.SelectedDate != null && EndDate.SelectedDate != null)
            {
                Nights.Text = (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days.ToString();
            }
            Save.IsEnabled = IsSaveEnabled();
        }

        private void Nights_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Nights.Text.Length > 0 && int.TryParse(Nights.Text, out int result))
            {
                EndDate.SelectedDate = StartDate.SelectedDate?.AddDays(result);
            }
            Save.IsEnabled = IsSaveEnabled();
        }

        private void Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textSender)
            {
                textSender.Text = textSender.Text.Replace(',', '.');
                if (textSender == TotalPrice) totalPrice = ParseDecimal(textSender.Text);
                if (textSender == PaidSum) paidSum = ParseDecimal(textSender.Text);
                RemainingSum.Text = $"{(totalPrice - paidSum):f2}";
            }
            Save.IsEnabled = IsSaveEnabled();
        }

        private decimal ParseDecimal(string text)
        {
            return decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result) ? result : 0;
        }

        private void Room_Loaded(object sender, RoutedEventArgs e)
        {
            Room.ItemsSource = controller.Rooms;
        }

        private void State_Loaded(object sender, RoutedEventArgs e)
        {
            State.ItemsSource = new[] { "Активна", "Настанена", "Отменена" };
        }

        private void Source_Loaded(object sender, RoutedEventArgs e)
        {
            Source.ItemsSource = new[] { "По телефон", "По имейл", "Booking.com", "На място" };
        }

        private void Numbers_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (e.Key != Key.D0 && e.Key != Key.D1 && e.Key != Key.D2 && e.Key != Key.D3 && e.Key != Key.D4 &&
                e.Key != Key.D5 && e.Key != Key.D6 && e.Key != Key.D7 && e.Key != Key.D8 && e.Key != Key.D9 &&
                e.Key != Key.NumPad0 && e.Key != Key.NumPad1 && e.Key != Key.NumPad2 && e.Key != Key.NumPad3 &&
                e.Key != Key.NumPad4 && e.Key != Key.NumPad5 && e.Key != Key.NumPad6 && e.Key != Key.NumPad7 &&
                e.Key != Key.NumPad8 && e.Key != Key.NumPad9) e.Handled = true;
            if (textBox == GuestsInRoom || textBox == Nights) return;
            if (e.Key != Key.OemPeriod && e.Key != Key.OemComma && e.Key != Key.Decimal) return;
            e.Handled = true;
            if (textBox == null || textBox.Text.Contains('.')) return;
            int caretPosition = textBox.SelectionStart;
            textBox.Text = textBox.Text.Insert(caretPosition, ".");
            textBox.SelectionStart = caretPosition + 1;
        }

        private bool IsSaveEnabled()
        {
            if (string.IsNullOrEmpty(GuestName.Text) || string.IsNullOrEmpty(GuestsInRoom.Text) || string.IsNullOrEmpty(Nights.Text) ||
                string.IsNullOrEmpty(TotalPrice.Text) || string.IsNullOrEmpty(RemainingSum.Text)) return false;
            if (!decimal.TryParse(RemainingSum.Text, out decimal remainingSum)) return false;
            return Room.SelectedIndex > 0 && int.Parse(GuestsInRoom.Text) > 0 && int.Parse(Nights.Text) > 0 && remainingSum >= 0;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(Id.Text);
            int state = State.SelectedIndex;
            int source = Source.SelectedIndex;
            int room = controller.Rooms[Room.SelectedIndex].FullRoomNumber;
            string guestName = GuestName.Text;
            DateTime startDate = StartDate.SelectedDate.GetValueOrDefault();
            int nights = int.Parse(Nights.Text);
            int guestsInRoom = int.Parse(GuestsInRoom.Text);
            string notes = string.IsNullOrEmpty(Notes.Text) ? "Няма" : Notes.Text;
            controller.SaveReservation(id, state, source, room, guestName, startDate, nights, guestsInRoom, totalPrice, paidSum, notes);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}