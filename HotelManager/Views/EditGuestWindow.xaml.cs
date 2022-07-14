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
    public partial class EditGuestWindow
    {
        private readonly ReservationWindow reservationWindow;
        private readonly MainController controller;
        private int guestId;
        List<ReservationInfo> reservationsList;

        public EditGuestWindow(ReservationWindow resWindow, MainController control, string name, string phone, string email)
        {
            InitializeComponent();
            reservationWindow = resWindow;
            controller = control;
            string[] guestNames = name.Split();
            List<string> guests = controller.Context.Guests.Select(x => $"{x.FirstName}|{x.LastName}|{x.Phone}|{x.Email}|{x.Reservations.Count}").ToList();
            FirstName.AutoSuggestionList = guests;
            LastName.AutoSuggestionList = guests;
            FirstName.AutoTextBox.Text = guestNames[0];
            FirstName.AutoListPopup.IsOpen = false;
            LastName.AutoTextBox.Text = string.Join(" ", guestNames.Skip(1));
            LastName.AutoListPopup.IsOpen = false;
            FirstName.AutoTextBox.TextChanged += TextBox_TextChanged;
            LastName.AutoTextBox.TextChanged += TextBox_TextChanged;
            FirstName.OnSelectionChanged += AutoTextBox_OnSelectionChanged;
            LastName.OnSelectionChanged += AutoTextBox_OnSelectionChanged;
            Phone.Text = string.IsNullOrEmpty(phone) ? null : phone;
            Email.Text = string.IsNullOrEmpty(email) ? null : email;
            guestId = controller.GetGuest(FirstName.AutoTextBox.Text, LastName.AutoTextBox.Text, Phone.Text, Email.Text)?.Id ?? 0;
            UpdateReservationsList();
            Save.IsEnabled = IsSaveEnabled();
        }

        private void AutoTextBox_OnSelectionChanged(object sender, string text)
        {
            string[] selectedText = text.Split("|");
            FirstName.AutoTextBox.Text = selectedText[0];
            LastName.AutoTextBox.Text = selectedText[1];
            Phone.Text = selectedText[2];
            Email.Text = selectedText[3];
            guestId = controller.GetGuest(FirstName.AutoTextBox.Text, LastName.AutoTextBox.Text, Phone.Text, Email.Text).Id;
            UpdateReservationsList();
            Save.IsEnabled = true;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) => Save.IsEnabled = IsSaveEnabled();

        private void UpdateReservationsList()
        {
            Reservations.Children.Clear();
            Reservations.RowDefinitions.Clear();
            if (guestId == 0) return;
            reservationsList = controller.GetReservationInfos(GetGuestInfo()).OrderByDescending(r => r.Id).ToList();
            for (int i = 0; i < reservationsList.Count; i++)
            {
                Reservations.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30), MinHeight = 30 });
                string[] resStrings = new string[7];
                resStrings[0] = $"{reservationsList[i].Id}";
                resStrings[1] = $"{reservationsList[i].StartDate:dd.MM.yyyy}";
                resStrings[2] = $"{reservationsList[i].EndDate:dd.MM.yyyy}";
                resStrings[3] = $"{(reservationsList[i].EndDate - reservationsList[i].StartDate).Days}";
                resStrings[4] = $"{reservationsList[i].TotalSum:0.00}";
                resStrings[5] = $"{reservationsList[i].PaidSum:0.00}";
                resStrings[6] = Constants.ReservationStates[reservationsList[i].StateInt];
                for (int j = 0; j < 7; j++)
                {
                    int resId = reservationsList[i].Id;
                    TextBox resTextBox = new TextBox() { Text = resStrings[j] };
                    resTextBox.Cursor = Cursors.Hand;
                    resTextBox.FontSize = 15;
                    resTextBox.IsReadOnly = true;
                    resTextBox.MouseDoubleClick += delegate { controller.RequestReservationWindow(resId); };
                    Grid.SetColumn(resTextBox, j);
                    Grid.SetRow(resTextBox, i);
                    Reservations.Children.Add(resTextBox);
                }
            }
        }

        private bool IsSaveEnabled()
        {
            bool validFirstName = FirstName.Validate();
            bool validLastName = LastName.Validate();
            bool validPhone = Validate(Phone);
            return validFirstName && validLastName && validPhone && GuestHasChanges();
        }

        private static bool Validate(TextBox textBox)
        {
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Background = new SolidColorBrush(Colors.Bisque);
                textBox.Foreground = new SolidColorBrush(Colors.Red);
                return false;
            }
            textBox.Background = new SolidColorBrush(Colors.White);
            textBox.Foreground = new SolidColorBrush(Colors.Black);
            return true;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveGuest();
            if (reservationWindow != null)
            {
                reservationWindow.GuestName.AutoTextBox.Text = $"{FirstName.AutoTextBox.Text} {LastName.AutoTextBox.Text}";
                reservationWindow.GuestName.AutoListPopup.IsOpen = false;
                reservationWindow.Email.Text = Email.Text;
                reservationWindow.Phone.Text = Phone.Text;
                reservationWindow.ResCount.Text = $"{reservationsList.Count}";
            }
            Close();
        }

        public bool SaveGuest()
        {
            GuestInfo guestInfo = GetGuestInfo();
            controller.SaveGuest(guestInfo);
            controller.ChangesMade = true;
            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (IsSaveEnabled() && GuestHasChanges())
            {
                string messageBoxText = Constants.UnsavedChangesText + Constants.DoubleLine + Constants.ConfirmSaveChanges;
                if (ConfirmationBox(messageBoxText, Constants.UnsavedChangesCaption) && !SaveGuest()) return;
            }
            Close();
        }

        private bool GuestHasChanges()
        {
            GuestInfo oldGuestInfo = controller.GetGuestInfo(guestId);
            return oldGuestInfo == null || !oldGuestInfo.Equals(GetGuestInfo());
        }

        public GuestInfo GetGuestInfo()
        {
            GuestInfo guestInfo = new GuestInfo($"{FirstName.AutoTextBox.Text} {LastName.AutoTextBox.Text}", Phone.Text, Email.Text);
            guestInfo.Id = guestId;
            return guestInfo;
        }

        private static bool ConfirmationBox(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
    }
}