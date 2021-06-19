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
            GuestName.AutoTextBox.Text = resInfo.GuestName;
            GuestReferrer.AutoTextBox.Text = resInfo.GuestReferrer;
            EndDate.SelectedDate = resInfo.EndDate;
            Nights.IntBox.Text = $"{(resInfo.EndDate - resInfo.StartDate).Days}";
            GuestsInRoom.IntBox.Text = $"{resInfo.NumberOfGuests}";
            TotalPrice.DecimalBox.Text = $"{resInfo.TotalSum}";
            PaidSum.DecimalBox.Text = $"{resInfo.PaidSum}";
            RemainingSum.DecimalBox.Text = $"{resInfo.TotalSum - resInfo.PaidSum}";
            Notes.Text = resInfo.Notes;
        }

        private void TransactionImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ReservationInfo oldReservationInfo = controller.GetReservationInfo(id);
            ReservationInfo newReservationInfo = GetReservationInfo();
            if (oldReservationInfo == null || !oldReservationInfo.Equals(newReservationInfo))
            {
                MessageBoxResult messageBoxResult = MessageBox.Show(
                    "По тази резервация има незапазени промени.\r\n" +
                    "За добавяне на плащане, резервацията трябва първо да бъде актуализирана.\r\n" +
                    "\r\nДа запазя ли резервацията?",
                    "Незапазени промени в резервацията",
                    MessageBoxButton.YesNo);
                if (messageBoxResult != MessageBoxResult.Yes) return;
            }
            new TransactionsWindow(controller, id, this).ShowDialog();
        }

        private void Room_SelectionChanged(object sender, SelectionChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

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

        private void Room_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> rooms = new List<string> { Constants.NoRoomSelected };
            rooms.AddRange(controller.Context.Rooms.ToList().Select(r => r.ToString()));
            Room.ItemsSource = rooms;
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
            return validRoom && validGuest && validGuests && validNights && validTotal && validPaid && validRemaining;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveReservation();
            Close();
        }

        public void SaveReservation()
        {
            if (!IsSaveEnabled())
            {
                MessageBox.Show(
                    "Резервацията има невалидни задължителни полета и не може да бъде запазена.\r\n" +
                    "Моля попълнете всички задължителни полета преди да добавите плащане.",
                    "Непълна информация за резервация");
                return;
            }
            if (GuestsInRoom.IntValue > controller.Context.Rooms.First(r => r.Id == Room.SelectedIndex).MaxGuests)
            {
                if (MessageBox.Show(
                    "Броя гости е повече от капацитета на стаята.\r\n" +
                    "\r\nДа продължа ли със записа?",
                    "Превишен капацитет на стаята",
                    MessageBoxButton.YesNo) == MessageBoxResult.No) return;
            }
            controller.SaveReservation(GetReservationInfo());
        }

        public ReservationInfo GetReservationInfo()
        {
            return new ReservationInfo
            {
                Id = id,
                StateInt = State.SelectedIndex,
                SourceInt = Source.SelectedIndex,
                Room = controller.Context.Rooms.First(r => r.Id == Room.SelectedIndex).FullRoomNumber,
                GuestName = GuestName.AutoTextBox.Text,
                GuestReferrer = GuestReferrer.AutoTextBox.Text,
                Email = string.IsNullOrEmpty(Email.Text) ? null : Email.Text,
                Phone = string.IsNullOrEmpty(Phone.Text) ? null : Phone.Text,
                StartDate = StartDate.SelectedDate.GetValueOrDefault(),
                EndDate = EndDate.SelectedDate.GetValueOrDefault(),
                NumberOfGuests = GuestsInRoom.IntValue,
                TotalSum = TotalPrice.DecimalValue,
                PaidSum = PaidSum.DecimalValue,
                Notes = Notes.Text
            };
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private bool CompareReservations()
        {
            ReservationInfo oldReservationInfo = controller.GetReservationInfo(id);
            return oldReservationInfo == null || !oldReservationInfo.Equals(GetReservationInfo());
        }
    }
}