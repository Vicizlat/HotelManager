using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HotelManager.Controller;
using HotelManager.Models;
using HotelManager.Utils;

namespace HotelManager.Views
{
    public partial class ReservationWindow
    {
        private readonly MainController controller;
        private readonly int id;
        private bool totalPriceManualMode;

        public ReservationWindow(MainController controller, int stateInt, int sourceInt, int room, DateTime startDate)
        {
            InitializeComponent();
            this.controller = controller;
            List<string> guests = controller.Context.Guests.Select(x => $"{x.FirstName}|{x.LastName}|{x.Phone}|{x.Email}|{x.Reservations.Count}").ToList();
            GuestName.AutoSuggestionList = guests;
            GuestName.AutoTextBox.TextChanged += GuestInfo_TextChanged;
            GuestName.OnSelectionChanged += AutoTextBox_OnSelectionChanged;
            State.SelectedIndex = stateInt;
            Source.SelectedIndex = sourceInt;
            Room.SelectedIndex = controller.Context.Rooms.FirstOrDefault(r => r.FullRoomNumber == room)?.Id ?? 0;
            StartDate.SelectedDate = startDate;
            EndDate.DisplayDateStart = StartDate.SelectedDate?.AddDays(1);
            EndDate.DisplayDateEnd = controller.NextReservationStartDate(room, startDate);
        }

        public ReservationWindow(int room, DateTime startDate, MainController controller)
            : this(controller, 0, 2, room, startDate)
        {
            id = controller.Context.Reservations.Count() + 1;
            Title = $"Добавяне на резервация номер: {id}";
            PaidSum.DecimalBox.Text = "0";
            GuestName.AutoTextBox.IsReadOnly = false;
            Email.IsReadOnly = false;
            Phone.IsReadOnly = false;
            EditGuestImage.IsEnabled = false;
            EditGuestImage.Visibility = Visibility.Hidden;
        }

        public ReservationWindow(ReservationInfo resInfo, MainController controller)
            : this(controller, resInfo.StateInt, resInfo.SourceInt, resInfo.Room, resInfo.StartDate)
        {
            id = resInfo.Id;
            Title = $"Редактиране на резервация номер: {id}";
            totalPriceManualMode = true;
            TotalPrice.ReadOnly = false;
            GuestName.AutoTextBox.Text = resInfo.Guest.GetFullName();
            GuestName.AutoListPopup.IsOpen = false;
            Email.Text = resInfo.Guest.Email;
            Phone.Text = resInfo.Guest.Phone;
            ResCount.Text = $"{resInfo.Guest.ResCount}";
            EndDate.SelectedDate = resInfo.EndDate;
            Nights.IntBox.Text = $"{(resInfo.EndDate - resInfo.StartDate).Days}";
            GuestsInRoom.IntBox.Text = $"{resInfo.NumberOfGuests}";
            TotalPrice.DecimalBox.Text = $"{resInfo.TotalSum}";
            PaidSum.DecimalBox.Text = $"{resInfo.PaidSum}";
            RemainingSum.DecimalBox.Text = $"{resInfo.TotalSum - resInfo.PaidSum}";
            Notes.Text = resInfo.Notes;
            Save.IsEnabled = IsSaveEnabled();
        }

        private void TransactionImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ReservationHasChanges())
            {
                string messageBoxText = Constants.UnsavedChangesText + Environment.NewLine + Constants.UnsavedChangesTextPayment +
                                        Constants.DoubleLine + Constants.ConfirmSaveChanges;
                if (!ConfirmationBox(messageBoxText, Constants.UnsavedChangesCaption) || !SaveReservation()) return;
            }
            new TransactionsWindow(controller, id).ShowDialog();
            decimal paidSum = controller.Context.Transactions.Where(t => t.ReservationId == id).Sum(t => t.PaidSum);
            PaidSum.DecimalBox.Text = $"{paidSum}";
            RemainingSum.DecimalBox.Text = $"{TotalPrice.DecimalValue - PaidSum.DecimalValue}";
            Save.IsEnabled = IsSaveEnabled();
            //if (ReservationHasChanges()) SaveReservation();
        }

        private void EditGuestImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new EditGuestWindow(this, controller, GuestName.AutoTextBox.Text, Phone.Text, Email.Text).ShowDialog();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void GuestsInRoom_TextChanged(object sender, TextChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void GuestInfo_TextChanged(object sender, TextChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void AutoTextBox_OnSelectionChanged(object sender, string text)
        {
            string[] selectedText = text.Split("|");
            GuestName.AutoTextBox.Text = $"{selectedText[0]} {selectedText[1]}";
            Phone.Text = selectedText[2];
            Email.Text = selectedText[3];
            ResCount.Text = selectedText[4];
            GuestName.AutoTextBox.IsReadOnly = true;
            Phone.IsReadOnly = true;
            Email.IsReadOnly = true;
            EditGuestImage.IsEnabled = true;
            EditGuestImage.Visibility = Visibility.Visible;
            Save.IsEnabled = IsSaveEnabled();
        }

        private void Dates_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDate.SelectedDate != null && EndDate.SelectedDate != null)
            {
                Nights.IntBox.Text = $"{(EndDate.SelectedDate - StartDate.SelectedDate).Value.Days}";
            }
        }

        private void Nights_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Nights.Validate())
            {
                EndDate.SelectedDate = StartDate.SelectedDate?.AddDays(Nights.IntValue);
            }
            Save.IsEnabled = IsSaveEnabled();
        }

        private void Price_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TotalPrice.Validate() && PaidSum.Validate())
            {
                decimal remainingSum = TotalPrice.DecimalValue - PaidSum.DecimalValue;
                RemainingSum.DecimalBox.Text = $"{remainingSum}";
            }
            if (totalPriceManualMode)
            {
                Save.IsEnabled = IsSaveEnabled();
            }
        }

        public bool IsSaveEnabled()
        {
            bool validRoom = Room.SelectedIndex > 0;
            bool validGuests = GuestsInRoom.Validate();
            bool validGuest = GuestName.Validate();
            bool validPhone = ValidateTextBox(Phone);
            bool validNights = Nights.Validate();
            if (!totalPriceManualMode && validGuests && validNights)
            {
                CalculateTotalPrice(StartDate.SelectedDate.Value, EndDate.SelectedDate.Value, GuestsInRoom.IntValue);
            }
            bool validTotal = TotalPrice.Validate();
            bool validPaid = PaidSum.Validate();
            bool reservationChanges = ReservationHasChanges();
            return validRoom && validGuests && validGuest && validPhone && validNights && validTotal && validPaid && reservationChanges;
        }

        private bool ValidateTextBox(TextBox textBox)
        {
            bool isValid = !string.IsNullOrEmpty(textBox.Text);
            textBox.Background = isValid ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Bisque);
            textBox.Foreground = isValid ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Red);
            return isValid;
        }

        private void CalculateTotalPrice(DateTime startDate, DateTime endDate, int guests)
        {
            decimal totalPrice = 0;
            for (int i = 0; i < Nights.IntValue; i++)
            {
                decimal priceForDay = controller.GetPriceForDate(startDate.AddDays(i), guests);
                if (priceForDay == 0)
                {
                    totalPriceManualMode = true;
                    TotalPrice.ReadOnly = false;
                    return;
                }
                totalPrice += priceForDay;
            }
            TotalPrice.DecimalBox.Text = $"{totalPrice}";
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveReservation();
            Close();
        }

        public bool SaveReservation()
        {
            if (!IsSaveEnabled())
            {
                MessageBox.Show("Резервацията има невалидни задължителни полета.", "Непълна информация за резервация");
                return false;
            }
            if (GuestsInRoom.IntValue > controller.Context.Rooms.First(r => r.Id == Room.SelectedIndex).MaxGuests)
            {
                string messageBoxText = Constants.OverCapacityText + Constants.DoubleLine + Constants.ConfirmSaveChanges;
                if (!ConfirmationBox(messageBoxText, Constants.OverCapacityCaption)) return false;
            }

            ReservationInfo resInfo = GetReservationInfo();
            controller.SaveReservation(resInfo);
            controller.ChangesMade = true;
            return true;
        }

        public ReservationInfo GetReservationInfo()
        {
            ReservationInfo reservationInfo = new ReservationInfo
            {
                Id = id,
                StateInt = State.SelectedIndex,
                SourceInt = Source.SelectedIndex,
                Room = controller.Context.Rooms.First(r => r.Id == Room.SelectedIndex).FullRoomNumber,
                Guest = controller.GetGuestInfo(GuestName.AutoTextBox.Text, Phone.Text, Email.Text),
                StartDate = StartDate.SelectedDate.GetValueOrDefault(),
                EndDate = EndDate.SelectedDate.GetValueOrDefault(),
                NumberOfGuests = GuestsInRoom.IntValue,
                TotalSum = TotalPrice.DecimalValue,
                PaidSum = PaidSum.DecimalValue,
                Notes = Notes.Text
            };
            return reservationInfo;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaveEnabled())
            {
                string messageBoxText = Constants.UnsavedChangesText + Constants.DoubleLine + Constants.ConfirmSaveChanges;
                if (ConfirmationBox(messageBoxText, Constants.UnsavedChangesCaption) && !SaveReservation()) return;
            }
            Close();
        }

        private bool ReservationHasChanges()
        {
            ReservationInfo oldReservationInfo = controller.GetReservationInfo(id);
            bool equalReservations = true;
            if (oldReservationInfo != null)
            {
                ReservationInfo newReservationInfo = GetReservationInfo();
                equalReservations = oldReservationInfo.Equals(newReservationInfo);
            }
            return oldReservationInfo == null || !equalReservations;
        }

        private static bool ConfirmationBox(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }

        private void TotalPrice_KeyUp(object sender, KeyEventArgs e)
        {
            totalPriceManualMode = true;
        }
    }
}