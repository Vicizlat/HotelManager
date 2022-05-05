using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Utils;

namespace HotelManager.Views
{
    public partial class CalendarWindow
    {
        public CalendarWindow()
        {
            InitializeComponent();
            Calendar1.DisplayDateStart = Settings.Instance.SeasonStartDate;
            Calendar1.DisplayDateEnd = Settings.Instance.SeasonEndDate.AddDays(-Settings.Instance.SeasonEndDate.Day);
            Calendar2.DisplayDateStart = new DateTime(Settings.Instance.SeasonStartDate.Year, Settings.Instance.SeasonStartDate.Month + 1, 1);
            Calendar2.DisplayDateEnd = Settings.Instance.SeasonEndDate;
            Calendar2.DisplayDate = DateTime.Now.AddMonths(1);
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            List<DateTime> selectedDates = Calendar1.SelectedDates.ToList();
            selectedDates.AddRange(Calendar2.SelectedDates);
            selectedDates.OrderBy(d => d).ToList();
            if (selectedDates.Count > 1)
            {
                ((MainWindow)Owner).CalendarSelectedStartDate = selectedDates.First();
                ((MainWindow)Owner).CalendarSelectedEndDate = selectedDates.Last();
                DialogResult = true;
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Calendar1_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            Calendar2.DisplayDate = Calendar1.DisplayDate.AddMonths(1);
        }

        private void Calendar2_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            Calendar1.DisplayDate = Calendar2.DisplayDate.AddMonths(-1);
        }
    }
}