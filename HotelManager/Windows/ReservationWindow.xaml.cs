﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HotelManager
{
    public partial class ReservationWindow : Window
    {
        private decimal totalPrice;
        private decimal paidSum;

        public ReservationWindow(int room, DateTime startDate)
        {
            InitializeComponent();
            Id.Text = $"{Reservations.Instance.Count + 1}";
            Room.SelectedIndex = GetRoomIndex(room);
            StartDate.SelectedDate = startDate;
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
            if (reservation.AdditionalInformation == "Няма") AdditionalInformation.Text = null;
            else AdditionalInformation.Text = reservation.AdditionalInformation;
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
                EndDate.SelectedDate = StartDate.SelectedDate.Value.AddDays(result);
            }
            Save.IsEnabled = IsSaveEnabled();
        }

        private void Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(TotalPrice.Text)) TotalPrice.Text = "0";
            if (string.IsNullOrEmpty(PaidSum.Text)) PaidSum.Text = "0";
            if (decimal.TryParse(TotalPrice.Text, out totalPrice) && decimal.TryParse(PaidSum.Text, out paidSum))
            {
                RemainingSum.Text = $"{totalPrice - paidSum}";
            }
            else RemainingSum.Text = "0";
            Save.IsEnabled = IsSaveEnabled();
        }

        private void IntegersOnly_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");

        private void DecimalsOnly_PreviewTextInput(object sender, TextCompositionEventArgs e) => e.Handled = !Regex.IsMatch(e.Text, "^[0-9,]+$");

        private bool IsSaveEnabled()
        {
            if (string.IsNullOrEmpty(GuestName.Text)) return false;
            if (string.IsNullOrEmpty(GuestsInRoom.Text)) return false;
            if (string.IsNullOrEmpty(Nights.Text)) return false;
            if (string.IsNullOrEmpty(TotalPrice.Text)) return false;
            if (string.IsNullOrEmpty(PaidSum.Text)) return false;
            if (!decimal.TryParse(RemainingSum.Text, out decimal remainingSum)) return false;
            else return Room.SelectedIndex > 0 && int.Parse(GuestsInRoom.Text) > 0 && int.Parse(Nights.Text) > 0 && remainingSum > 0;
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
            string additionalInfo = string.IsNullOrEmpty(AdditionalInformation.Text) ? "Няма" : AdditionalInformation.Text;

            Reservations.Instance.AddReservation(id, status, room, guestName, startDate, endDate, guestsInRoom, totalPrice, paidSum, additionalInfo);
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}