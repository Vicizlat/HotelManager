using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Core;

namespace HotelManager
{
    public partial class ReservationWindow
    {
        private decimal totalPrice;
        private decimal paidSum;
        private readonly int id;

        public ReservationWindow(int id, int room, DateTime startDate)
        {
            InitializeComponent();
            this.id = id;
            Id.Text = $"{this.id}";
            State.SelectedIndex = 0;
            Room.SelectedIndex = GetRoomIndex(room);
            StartDate.SelectedDate = startDate;
            EndDate.DisplayDateStart = startDate.AddDays(1);
            Reservation res = Reservations.Instance.GetReservation(room, new Period(startDate.AddDays(1), new DateTime(2020, 09, 30)));
            if (res != null) EndDate.DisplayDateEnd = res.Period.StartDate;
        }

        public ReservationWindow(Reservation reservation) : this(reservation.Id, reservation.Room, reservation.Period.StartDate)
        {
            State.SelectedIndex = reservation.ReservationState;
            GuestName.Text = reservation.GuestName;
            EndDate.SelectedDate = reservation.Period.EndDate;
            Nights.Text = $"{reservation.Period.Nights}";
            GuestsInRoom.Text = $"{reservation.GuestsInRoom}";
            TotalPrice.Text = $"{reservation.Sums.Total}";
            PaidSum.Text = $"{reservation.Sums.Paid}";
            RemainingSum.Text = $"{reservation.Sums.Remaining}";
            AdditionalInformation.Text = reservation.AdditionalInformation == "Няма" ? string.Empty : reservation.AdditionalInformation;
        }

        internal int GetRoomIndex(int room)
        {
            if (room >= 1 && room <= 8) return room;
            if (room >= 11 && room <= 18) return room - 2;
            if (room >= 21 && room <= 28) return room - 4;
            if (room == 32) return 25;
            return 0;
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
            if (string.IsNullOrEmpty(TotalPrice.Text)) TotalPrice.Text = "0";
            if (!decimal.TryParse(TotalPrice.Text, out totalPrice)) totalPrice = 0;
            if (string.IsNullOrEmpty(PaidSum.Text)) PaidSum.Text = "0";
            if (!decimal.TryParse(PaidSum.Text, out paidSum)) paidSum = 0;
            RemainingSum.Text = $"{totalPrice - paidSum}";
            Save.IsEnabled = IsSaveEnabled();
        }

        private void IntegersOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        private void DecimalsOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char decimalSeparator = Convert.ToChar(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            e.Handled = !Regex.IsMatch(e.Text, $"^[0-9{decimalSeparator}]+$");
        }

        private bool IsSaveEnabled()
        {
            if (string.IsNullOrEmpty(GuestName.Text) || string.IsNullOrEmpty(GuestsInRoom.Text) || string.IsNullOrEmpty(Nights.Text) ||
                string.IsNullOrEmpty(TotalPrice.Text) || string.IsNullOrEmpty(RemainingSum.Text)) return false;
            if (!decimal.TryParse(RemainingSum.Text, out decimal remainingSum)) return false;
            return Room.SelectedIndex > 0 && int.Parse(GuestsInRoom.Text) > 0 && int.Parse(Nights.Text) > 0 && remainingSum >= 0;
        }

        private void Room_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> roomsList = new List<string> { "Няма избрана стая" };
            for (int floor = 0; floor < 3; floor++)
            {
                for (int room = 1; room <= 8; room++) roomsList.Add($"Стая {floor}{room}");
            }
            roomsList.Add("Апартамент 32");
            Room.ItemsSource = roomsList;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            int reservationState = State.SelectedIndex;
            int room = int.Parse(Room.SelectedItem.ToString().Substring(Room.SelectedItem.ToString().Length - 2, 2));
            Period period = new Period(StartDate.SelectedDate.GetValueOrDefault(), EndDate.SelectedDate.GetValueOrDefault());
            int guestsInRoom = int.Parse(GuestsInRoom.Text);
            Sums sums = new Sums(totalPrice, paidSum);
            string additionalInfo = string.IsNullOrEmpty(AdditionalInformation.Text) ? "Няма" : AdditionalInformation.Text;
            Reservation reservation = new Reservation(id, reservationState, room, GuestName.Text, period, guestsInRoom, sums, additionalInfo);
            Reservations.Instance.SaveReservation(reservation);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void State_Loaded(object sender, RoutedEventArgs e)
        {
            string[] statesList = { "Активна", "Настанена", "Отменена" };
            State.ItemsSource = statesList;
        }
    }
}