using System;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Controller;
using HotelManager.Utils;

namespace HotelManager.Views
{
    public partial class SettingsWindow
    {
        private readonly IController controller;

        public SettingsWindow(IController controller)
        {
            InitializeComponent();
            this.controller = controller;
            WebAddress.Text = Settings.Instance.WebAddress;
            WebAddressFull.Text = Settings.Instance.WebAddressFull;
            FtpAddress.Text = Settings.Instance.FtpAddress;
            FtpUserName.Text = Settings.Instance.FtpUserName;
            FtpPassword.Text = Settings.Instance.FtpPassword;
            SeasonStartDate.SelectedDate = Settings.Instance.SeasonStartDate;
            SeasonEndDate.SelectedDate = Settings.Instance.SeasonEndDate;
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
            return !string.IsNullOrEmpty(WebAddress.Text) && !string.IsNullOrEmpty(WebAddressFull.Text) &&
                   !string.IsNullOrEmpty(FtpAddress.Text) && !string.IsNullOrEmpty(FtpUserName.Text) &&
                   !string.IsNullOrEmpty(FtpPassword.Text) && hasDates;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Instance.WebAddress = WebAddress.Text;
            Settings.Instance.WebAddressFull = WebAddressFull.Text;
            Settings.Instance.FtpAddress = FtpAddress.Text;
            Settings.Instance.FtpUserName = FtpUserName.Text;
            Settings.Instance.FtpPassword = FtpPassword.Text;
            Settings.Instance.SeasonStartDate = SeasonStartDate.SelectedDate.GetValueOrDefault(DateTime.Today);
            Settings.Instance.SeasonEndDate = SeasonEndDate.SelectedDate.GetValueOrDefault(DateTime.Today.AddDays(365));
            Settings.Instance.LocalUseOnly = LocalUseOnly.IsChecked.GetValueOrDefault();
            controller.SaveSettings();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}