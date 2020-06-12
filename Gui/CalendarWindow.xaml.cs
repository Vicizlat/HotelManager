using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Gui
{
    public partial class CalendarWindow
    {
        private MainWindow MainWindow { get; set; }

        public CalendarWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            MainWindow = mainWindow;
        }

        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as Calendar).SelectedDates.Count > 1)
            {
                MainWindow.StartDate.SelectedDate = (sender as Calendar).SelectedDates.OrderBy(d => d).First();
                MainWindow.EndDate.SelectedDate = (sender as Calendar).SelectedDates.OrderBy(d => d).Last();
                Close();
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();
    }
}