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
            WebAddress.Text = Settings.Instance.WebAddress;
            FtpAddress.Text = Settings.Instance.FtpAddress;
            FtpUserName.Text = Settings.Instance.FtpUserName;
            FtpPassword.Text = Settings.Instance.FtpPassword;
            SeasonStartDate.SelectedDate = Settings.Instance.SeasonStartDate;
            SeasonEndDate.SelectedDate = Settings.Instance.SeasonEndDate;
            Server.Text = Settings.Instance.Server;
            Database.Text = Settings.Instance.Database;
            UserName.Text = Settings.Instance.UserName;
            Password.Text = Settings.Instance.Password;
            LocalUseOnly.IsChecked = Settings.Instance.LocalUseOnly;
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
            if (LocalUseOnly.IsChecked.GetValueOrDefault()) return hasDates;
            return !string.IsNullOrEmpty(WebAddress.Text) && !string.IsNullOrEmpty(FtpAddress.Text) &&
                   !string.IsNullOrEmpty(FtpUserName.Text) && !string.IsNullOrEmpty(FtpPassword.Text) && hasDates;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.WebAddress = WebAddress.Text;
            Settings.Instance.FtpAddress = FtpAddress.Text;
            Settings.Instance.FtpUserName = FtpUserName.Text;
            Settings.Instance.FtpPassword = FtpPassword.Text;
            Settings.Instance.SeasonStartDate = SeasonStartDate.SelectedDate.GetValueOrDefault(DateTime.Today);
            Settings.Instance.SeasonEndDate = SeasonEndDate.SelectedDate.GetValueOrDefault(DateTime.Today.AddDays(365));
            Settings.Instance.Server = Server.Text;
            Settings.Instance.Database = Database.Text;
            Settings.Instance.UserName = UserName.Text;
            Settings.Instance.Password = Password.Text;
            Settings.Instance.LocalUseOnly = LocalUseOnly.IsChecked.GetValueOrDefault();
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