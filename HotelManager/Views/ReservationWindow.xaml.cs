using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HotelManager.Controller;
using HotelManager.Utils;
using HotelManager.Views.Templates;

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
            State.SelectedIndex = stateInt;
            Source.SelectedIndex = sourceInt;
            Room.SelectedIndex = controller.Context.Rooms.FirstOrDefault(r => r.FullRoomNumber == room)?.Id ?? 0;
            StartDate.SelectedDate = startDate;
            List<string[]> guests = controller.Context.Guests.Select(x => new[]
            {
                $"{x.FirstName} {x.LastName}".Trim(),
                x.Phone,
                x.Email,
                $"{x.Reservations.Count}"
            }).ToList();
            GuestName.AutoSuggestionList = guests;
            GuestName.AutoTextBox.TextChanged += GuestName_TextChanged;
            GuestReferrer.AutoSuggestionList = guests;
            GuestReferrer.AutoTextBox.TextChanged += GuestName_TextChanged;
            EndDate.DisplayDateStart = StartDate.SelectedDate?.AddDays(1);
            EndDate.DisplayDateEnd = controller.NextReservationStartDate(room, startDate);
        }

        public ReservationWindow(int room, DateTime startDate, MainController controller)
            : this(controller, 0, 2, room, startDate)
        {
            id = controller.Context.Reservations.Count() + 1;
            Title = $"Добавяне на резервация номер: {id}";
            PaidSum.DecimalBox.Text = "0";
        }

        public ReservationWindow(ReservationInfo resInfo, MainController controller)
            : this(controller, resInfo.StateInt, resInfo.SourceInt, resInfo.Room, resInfo.StartDate)
        {
            id = resInfo.Id;
            Title = $"Редактиране на резервация номер: {id}";
            GuestName.AutoTextBox.Text = resInfo.Guest.GetName();
            GuestReferrer.AutoTextBox.Text = resInfo.Guest.Referrer;
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
            PaidSum.DecimalBox.Text = $"{controller.Context.Transactions.Where(t => t.ReservationId == id).Sum(t => t.PaidSum)}";
            RemainingSum.DecimalBox.Text = $"{TotalPrice.DecimalValue - PaidSum.DecimalValue}";
            if (ReservationHasChanges()) SaveReservation();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void GuestsInRoom_TextChanged(object sender, TextChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void GuestName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (GuestName.Validate())
            {
                string[] guestInfo = GuestName.AutoSuggestionList.FirstOrDefault(g => g[0] == GuestName.AutoTextBox.Text);
                Phone.Text = guestInfo?[1] ?? string.Empty;
                Email.Text = guestInfo?[2] ?? string.Empty;
                ResCount.Text = guestInfo?[3] ?? "0";
            }
            if (GuestReferrer.AutoTextBox.Text == GuestName.AutoTextBox.Text)
            {
                Save.IsEnabled = false;
                GuestReferrer.AutoTextBox.Background = new SolidColorBrush(Colors.Bisque);
                return;
            }
            GuestReferrer.AutoTextBox.Background = new SolidColorBrush(Colors.White);
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
                RemainingSum.DecimalBox.Text = $"{remainingSum:f2}";
            }
            Save.IsEnabled = IsSaveEnabled();
        }

        public bool IsSaveEnabled()
        {
            bool validRoom = Room.SelectedIndex > 0;
            bool validGuest = GuestName.Validate();
            bool validGuests = GuestsInRoom.Validate();
            bool validNights = Nights.Validate();
            bool validTotal = TotalPrice.Validate();
            bool validPaid = PaidSum.Validate();
            bool validRemaining = RemainingSum.Validate();
            if (validGuests && validNights && !totalPriceManualMode)
            {
                CalculateTotalPrice(StartDate.SelectedDate.Value, EndDate.SelectedDate.Value, GuestsInRoom.IntValue);
            }
            return validRoom && validGuest && validGuests && validNights && validTotal && validPaid && validRemaining && ReservationHasChanges();
        }

        private void CalculateTotalPrice(DateTime startDate, DateTime endDate, int guests)
        {
            decimal[] prices = controller.GetBasePriceForDate(startDate, out int baseGuests, out DateTime[] prDates);
            if (prices[0] == 0 || prices[1] == 0)
            {
                totalPriceManualMode = true;
                TotalPrice.ReadOnly = false;
                return;
            }
            decimal totalPrice;
            if (prDates[1] < endDate)
            {
                decimal[] prices2 = controller.GetBasePriceForDate(endDate, out int baseGuests2, out DateTime[] prDates2);
                decimal firstPrice = (prices[0] + ((guests - baseGuests) * prices[1])) * (prDates[1] - startDate).Days;
                decimal secondPrice = (prices2[0] + ((guests - baseGuests2) * prices2[1])) * (endDate - prDates2[1]).Days;
                totalPrice = firstPrice + secondPrice;
            }
            else
            {
                totalPrice = (prices[0] + ((guests - baseGuests) * prices[1])) * (endDate - startDate).Days;
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
            controller.SaveReservation(GetReservationInfo());
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
                Guest = new GuestInfo(controller.FindGuest(GuestName.AutoTextBox.Text, Phone.Text, Email.Text)),
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
            if (IsSaveEnabled() && ReservationHasChanges())
            {
                string messageBoxText = Constants.UnsavedChangesText + Constants.DoubleLine + Constants.ConfirmSaveChanges;
                if (ConfirmationBox(messageBoxText, Constants.UnsavedChangesCaption) && !SaveReservation()) return;
            }
            Close();
        }

        private bool ReservationHasChanges()
        {
            ReservationInfo oldReservationInfo = controller.GetReservationInfo(id);
            return oldReservationInfo == null || !oldReservationInfo.Equals(GetReservationInfo());
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