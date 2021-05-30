using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Handlers;
using HotelManager.Utils;

namespace HotelManager.Views
{
    public partial class CalendarWindow
    {

        public CalendarWindow()
        {
            InitializeComponent();
            Calendar.DisplayDateStart = Settings.Instance.SeasonStartDate;
            Calendar.DisplayDateEnd = Settings.Instance.SeasonEndDate;
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            List<DateTime> selectedDates = Calendar.SelectedDates.OrderBy(d => d).ToList();
            if (selectedDates.Count > 1)
            {
                MainWindow mainWindow = (MainWindow)Owner;
                DateTime calendarSelectedStartDate = selectedDates.First();
                if (calendarSelectedStartDate != mainWindow.StartDate.SelectedDate)
                {
                    mainWindow.StartDate.SelectedDate = calendarSelectedStartDate;
                    Logging.Instance.WriteLine("StartDate changed from Calendar.");
                }
                DateTime calendarSelectedEndDate = selectedDates.Last();
                if (calendarSelectedEndDate != mainWindow.EndDate.SelectedDate)
                {
                    mainWindow.EndDate.SelectedDate = calendarSelectedEndDate;
                    Logging.Instance.WriteLine("EndDate changed from Calendar.");
                }
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}