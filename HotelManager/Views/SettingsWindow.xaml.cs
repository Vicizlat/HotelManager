using System;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Utils;

namespace HotelManager.Views
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
            SeasonStartDate.SelectedDate = Settings.Instance.SeasonStartDate;
            SeasonEndDate.SelectedDate = Settings.Instance.SeasonEndDate;
            Server.Text = Settings.Instance.Server;
            Port.Text = $"{Settings.Instance.Port}";
            Database.Text = Settings.Instance.Database;
            Username.Text = Settings.Instance.Username;
            Password.Text = Settings.Instance.Password;
        }

        private void GenericText_TextChanged(object sender, TextChangedEventArgs e)
        {
            Save.IsEnabled = IsSaveEnabled();
        }

        private void Dates_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Save.IsEnabled = IsSaveEnabled();
        }

        private void LocalUseOnly_OnChecked(object sender, RoutedEventArgs e)
        {
            Save.IsEnabled = IsSaveEnabled();
        }

        private bool IsSaveEnabled()
        {
            bool hasDates = SeasonStartDate.SelectedDate.HasValue && SeasonEndDate.SelectedDate.HasValue;
            return !string.IsNullOrEmpty(Server.Text) && !string.IsNullOrEmpty(Port.Text) && !string.IsNullOrEmpty(Username.Text) &&
                   !string.IsNullOrEmpty(Password.Text) && !string.IsNullOrEmpty(Database.Text) && hasDates;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.SeasonStartDate = SeasonStartDate.SelectedDate.GetValueOrDefault(DateTime.Today);
            Settings.Instance.SeasonEndDate = SeasonEndDate.SelectedDate.GetValueOrDefault(DateTime.Today.AddDays(365));
            Settings.Instance.Server = Server.Text;
            Settings.Instance.Port = int.TryParse(Port.Text, out int port) ? port : 3306;
            Settings.Instance.Database = Database.Text;
            Settings.Instance.Username = Username.Text;
            Settings.Instance.Password = Password.Text;
            CloseWindow(Settings.InvokeSettingsChanged());
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) => CloseWindow(false);

        private void CloseWindow(bool result)
        {
            DialogResult = result;
            Close();
        }
    }
}