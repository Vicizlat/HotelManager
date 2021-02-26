using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Utils;

namespace HotelManager.Views
{
    public partial class CalendarWindow
    {
        private MainWindow MainWindow { get; }

        public CalendarWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            MainWindow = mainWindow;
            Calendar.DisplayDateStart = Settings.Instance.SeasonStartDate;
            Calendar.DisplayDateEnd = Settings.Instance.SeasonEndDate;
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            Calendar calendar = sender as Calendar;
            if (calendar?.SelectedDates.Count > 1)
            {
                MainWindow.StartDate.SelectedDate = calendar.SelectedDates.OrderBy(d => d).First();
                MainWindow.EndDate.SelectedDate = calendar.SelectedDates.OrderBy(d => d).Last();
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}