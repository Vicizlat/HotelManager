using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HotelManager.Controller;
using HotelManager.Data.Models;
using HotelManager.Views.Templates;
using Microsoft.EntityFrameworkCore;

namespace HotelManager.Views
{
    public partial class TransactionsWindow
    {
        private readonly MainController controller;
        private bool searchLocked = false;

        public TransactionsWindow(MainController controller, int? reservationId = null)
        {
            InitializeComponent();
            this.controller = controller;
            if (reservationId != null)
            {
                ReservationId.IntBox.Text = $"{reservationId}";
                ReservationId.IntBox.IsReadOnly = true;
                ReservationId.IntBox.IsHitTestVisible = false;
                ReservationId.IntBox.Focusable = false;
                searchLocked = true;
            }
            TransactionDate.SelectedDate = DateTime.Now;
            SearchImage_MouseLeftButtonUp(this, null);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            OpenReservation.IsEnabled = !searchLocked && ReservationId.Validate();
            AddTransaction.IsEnabled = IsSaveEnabled();
        }

        private void Sum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TransactionSum.Validate(true))
            {
                if (decimal.TryParse(RemainingSum.Text, out decimal remainingSum))
                {
                    decimal totalSum = controller.Context.Reservations.FirstOrDefault(r => r.Id == ReservationId.IntValue).TotalSum;
                    decimal paidSum = controller.Context.Transactions.Where(t => t.ReservationId == ReservationId.IntValue).Sum(t => t.PaidSum);
                    RemainingSum.Text = $"{totalSum - paidSum - TransactionSum.DecimalValue}";
                }
            }
            AddTransaction.IsEnabled = IsSaveEnabled();
        }

        private void SearchImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Transactions.Children.Clear();
            Transactions.RowDefinitions.Clear();
            List<string> transactionsList;
            if (string.IsNullOrEmpty(ReservationId.IntBox.Text))
            {
                transactionsList = controller.Context.Transactions
                    .Include(t => t.Guest)
                    .Select(t => t.ToString()).ToList();
            }
            else
            {
                transactionsList = controller.Context.Transactions
                    .Where(t => t.ReservationId == ReservationId.IntValue)
                    .Include(t => t.Guest)
                    .Select(t => t.ToString()).ToList();
                decimal totalSum = controller.Context.Reservations.FirstOrDefault(r => r.Id == ReservationId.IntValue).TotalSum;
                decimal paidSum = controller.Context.Transactions.Where(t => t.ReservationId == ReservationId.IntValue).Sum(t => t.PaidSum);
                RemainingSum.Text = $"{totalSum - paidSum}";
            }
            for (int i = 0; i < transactionsList.Count; i++)
            {
                Transactions.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30), MinHeight = 30 });
                string[] transStrings = transactionsList[i].Split(" | ", StringSplitOptions.TrimEntries);
                for (int j = 0; j < 6; j++)
                {
                    TransactionTextBox transTextBox = new TransactionTextBox(j == 1 && !searchLocked ? this : null) { Text = transStrings[j] };
                    Grid.SetColumn(transTextBox, j);
                    Grid.SetRow(transTextBox, i);
                    Transactions.Children.Add(transTextBox);
                }
            }
        }

        private void Dates_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            AddTransaction.IsEnabled = IsSaveEnabled();
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            Reservation reservation = controller.Context.Reservations.Include(r => r.Guest).FirstOrDefault(r => r.Id == ReservationId.IntValue);
            if (reservation != null)
            {
                controller.Context.Transactions.Add(new Transaction
                {
                    Reservation = reservation,
                    Guest = reservation.Guest,
                    PaidSum = TransactionSum.DecimalValue,
                    PaymentMethod = TransactionMethod.Text,
                    PaymentDate = TransactionDate.SelectedDate
                });
                controller.Context.SaveChanges();
                decimal totalSum = controller.Context.Reservations.FirstOrDefault(r => r.Id == ReservationId.IntValue).TotalSum;
                decimal paidSum = controller.Context.Transactions.Where(t => t.ReservationId == ReservationId.IntValue).Sum(t => t.PaidSum);
                RemainingSum.Text = $"{totalSum - paidSum}";
            }
            TransactionSum.DecimalBox.Text = "0";
            TransactionMethod.Text = string.Empty;
            TransactionDate.SelectedDate = DateTime.Now;
            SearchImage_MouseLeftButtonUp(sender, null);
        }

        private bool IsSaveEnabled()
        {
            bool validReservation = ReservationId.Validate();
            bool validSum = TransactionSum.Validate(true);
            bool validDate = TransactionDate.SelectedDate != null;
            bool validMethod = Validate(TransactionMethod);
            bool validRemaining = Validate(RemainingSum);
            return validReservation && validSum && validDate && validMethod && validRemaining;
        }

        private static bool Validate(TextBox textBox)
        {
            decimal decimalValue = 0;
            bool isEmpty = string.IsNullOrEmpty(textBox.Text);
            bool validateDecimal = textBox.Name == nameof(RemainingSum);
            bool isNotDecimal = validateDecimal && !decimal.TryParse(textBox.Text, out decimalValue);
            if (isEmpty || isNotDecimal || decimalValue < 0)
            {
                textBox.Background = new SolidColorBrush(Colors.Bisque);
                textBox.Foreground = new SolidColorBrush(Colors.Red);
                return false;
            }
            textBox.Background = new SolidColorBrush(Colors.White);
            textBox.Foreground = new SolidColorBrush(Colors.Black);
            return true;
        }

        private void OpenReservation_Click(object sender, RoutedEventArgs e)
        {
            controller.RequestReservationWindow(ReservationId.IntValue);
        }
    }
}