using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HotelManager.Controller;
using HotelManager.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelManager.Views
{
    public partial class TransactionsWindow
    {
        private readonly ReservationWindow owner;
        private readonly MainController controller;

        public TransactionsWindow(MainController controller, int reservationId, ReservationWindow owner)
        {
            InitializeComponent();
            this.controller = controller;
            this.owner = owner;
            ReservationId.IntBox.Text = $"{reservationId}";
            ReservationId.IntBox.IsReadOnly = true;
            ReservationId.IntBox.IsHitTestVisible = false;
            ReservationId.IntBox.Focusable = false;
            TransactionDate.SelectedDate = DateTime.Now;
            RemainingSum.Text = $"{owner.RemainingSum.DecimalValue}";
            Search_Click(this, null);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddTransaction.IsEnabled = IsSaveEnabled();
        }

        private void Sum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TransactionSum.Validate(true))
            {
                decimal remainingSum = owner.RemainingSum.DecimalValue - TransactionSum.DecimalValue;
                RemainingSum.Text = $"{remainingSum:f2}";
            }
            AddTransaction.IsEnabled = IsSaveEnabled();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
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
            }
            for (int row = 0; row < transactionsList.Count; row++)
            {
                Transactions.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30), MinHeight = 30 });
                TextBox transactionTextBox = new TextBox { Text = transactionsList[row] };
                Grid.SetRow(transactionTextBox, row);
                Transactions.Children.Add(transactionTextBox);
            }
        }

        private void Dates_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            AddTransaction.IsEnabled = IsSaveEnabled();
        }

        private void AddTransaction_Click(object sender, RoutedEventArgs e)
        {
            Reservation reservation = controller.Context.Reservations
                .Include(r => r.Guest).FirstOrDefault(r => r.Id == ReservationId.IntValue);
            if (reservation == null)
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Тази резервация липсва в базата данни.\r\n" +
                    "За добавяне на плащане към нова резервация, тя трябва първо да бъде запазена.\r\n" +
                    "\r\nДа запазя ли резервацията?", "Няма такава резервация", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    owner.SaveReservation();
                    if (!owner.IsSaveEnabled())
                    {
                        Close();
                        return;
                    }
                }
                else return;
            }
            reservation = controller.Context.Reservations.Include(r => r.Guest).FirstOrDefault(r => r.Id == ReservationId.IntValue);
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
                decimal sum = controller.Context.Transactions.Where(t => t.ReservationId == reservation.Id).Sum(t => t.PaidSum);
                owner.PaidSum.DecimalBox.Text = $"{sum}";
                RemainingSum.Text = $"{owner.RemainingSum.DecimalValue}";
                owner.SaveReservation();
            }
            TransactionSum.DecimalBox.Text = "0";
            TransactionMethod.Text = string.Empty;
            TransactionDate.SelectedDate = DateTime.Now;
            Search_Click(sender, e);
        }

        private bool IsSaveEnabled()
        {
            bool validReservation = ReservationId.Validate();
            bool validSum = TransactionSum.Validate(true);
            bool validDate = TransactionDate.SelectedDate != null;
            bool validType = Validate(TransactionMethod);
            bool validRemaining = Validate(RemainingSum);
            return validReservation && validSum && validDate && validType && validRemaining;
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
    }
}