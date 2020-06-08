using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManager
{
    public partial class ReservationWindow : Window
    {
        public ReservationWindow(int id, bool status, int room, string guestName, DateTime startDate, DateTime? endDate, int numGuests = 0, decimal totalPrice = 0, decimal paidSum = 0, string additionalInfo = "Без допълнителна информация")
        {
            InitializeComponent();
            Id.Text = $"{id}";
            Status.IsChecked = status;
            Status.Content = Status.IsChecked.Value ? "Активна резервация" : "Отменена резервация";
            Room.SelectedIndex = GetRoomIndex(room);
            GuestName.Text = guestName;
            StartDate.SelectedDate = startDate;
            if (endDate != null)
            {
                EndDate.SelectedDate = endDate;
                Nights.Text = (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days.ToString();
            }
            else Nights.Text = "0";
            GuestsInRoom.Text = numGuests > 0 ? $"{numGuests}" : "0";
            TotalPrice.Text = totalPrice > 0 ? $"{totalPrice}" : "0";
            PaidSum.Text = paidSum > 0 ? $"{paidSum}" : "0";
            RemainingSum.Text = totalPrice > 0 ? $"{totalPrice - paidSum}" : "0";
            AdditionalInformation.Text = additionalInfo;
            Save.IsEnabled = IsSaveEnabled();
        }

        public ReservationWindow(Reservation reservation)
        {
            InitializeComponent();
            Id.Text = $"{reservation.Id}";
            Status.IsChecked = reservation.Status;
            Status.Content = Status.IsChecked.Value ? "Активна резервация" : "Отменена резервация";
            Room.SelectedIndex = GetRoomIndex(reservation.Room);
            GuestName.Text = reservation.GuestName;
            StartDate.SelectedDate = reservation.StartDate;
            EndDate.SelectedDate = reservation.EndDate;
            Nights.Text = (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days.ToString();
            GuestsInRoom.Text = $"{reservation.GuestsInRoom}";
            TotalPrice.Text = $"{reservation.TotalPrice}";
            PaidSum.Text = $"{reservation.PaidSum}";
            RemainingSum.Text = $"{reservation.TotalPrice - reservation.PaidSum}";
            AdditionalInformation.Text = reservation.AdditionalInformation;
            Save.IsEnabled = IsSaveEnabled();
        }

        private int GetRoomIndex(int room)
        {
            if (room >= 1 && room <= 8) return room;
            else if (room >= 11 && room <= 18) return room - 2;
            else if (room >= 21 && room <= 28) return room - 4;
            else if (room == 32) return 25;
            else return 0;
        }

        private void CheckboxStatus(object sender, RoutedEventArgs e) => Status.Content = Status.IsChecked.Value ? "Активна резервация" : "Отменена резервация";

        private void Room_SelectionChanged(object sender, SelectionChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void GuestName_TextChanged(object sender, TextChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void GuestsInRoom_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GuestsInRoom.Text.Length == 0) GuestsInRoom.Text = "0";
            if (GuestsInRoom.Text.Length > 1) GuestsInRoom.Text = GuestsInRoom.Text.Substring(0, 1);
            Save.IsEnabled = IsSaveEnabled();
        }

        private void Dates_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDate.SelectedDate != null && EndDate.SelectedDate != null)
            {
                Nights.Text = (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days.ToString();
            }
            else Nights.Text = "0";
            Save.IsEnabled = IsSaveEnabled();
        }

        private void Nights_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Nights.Text.Length > 0)
            {
                EndDate.SelectedDate = StartDate.SelectedDate.Value.AddDays(int.Parse(Nights.Text));
            }
            else Nights.Text = "0";
            Save.IsEnabled = IsSaveEnabled();
        }

        private void Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (decimal.TryParse(TotalPrice.Text, out decimal totalPrice) && decimal.TryParse(PaidSum.Text, out decimal paidSum))
            {
                RemainingSum.Text = $"{totalPrice - paidSum}";
            }
            Save.IsEnabled = IsSaveEnabled();
        }

        private void NumbersOnly_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !int.TryParse(e.Text, out int num);

        private bool IsSaveEnabled()
        {
            if (!decimal.TryParse(TotalPrice.Text, out decimal totalPrice) || !decimal.TryParse(PaidSum.Text, out decimal paidSum)) return false;
            else return GuestName.Text.Length > 0 && Room.SelectedIndex > 0 && int.Parse(GuestsInRoom.Text) > 0 && int.Parse(Nights.Text) > 0 && totalPrice > 0;
        }

        private void Room_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> roomsList = new List<string> { "Няма избрана стая" };
            for (int floor = 0; floor < 3; floor++)
            {
                for (int room = 1; room <= 8; room++)
                {
                    roomsList.Add($"Стая {floor}{room}");
                }
            }
            roomsList.Add("Апартамент 32");
            Room.ItemsSource = roomsList;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(Id.Text);
            bool status = Status.IsChecked.Value;
            int room = int.Parse(Room.SelectedItem.ToString().Substring(Room.SelectedItem.ToString().Length - 2, 2));
            string guestName = GuestName.Text;
            DateTime startDate = StartDate.SelectedDate.Value;
            DateTime endDate = EndDate.SelectedDate.Value;
            int guestsInRoom = int.Parse(GuestsInRoom.Text);
            decimal totalPrice = decimal.Parse(TotalPrice.Text);
            decimal paidSum = decimal.Parse(PaidSum.Text);
            string additionalInfo = AdditionalInformation.Text;

            Reservations.Instance.AddReservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo);
            Reservations.Instance.SaveReservations();
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}