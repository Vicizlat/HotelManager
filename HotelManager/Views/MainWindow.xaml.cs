using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Controller;
using HotelManager.Views.Templates;
using HotelManager.Utils;
using System.Linq;

namespace HotelManager.Views
{
    public partial class MainWindow
    {
        internal int DaysToShow;
        private readonly IController controller;

        public MainWindow(IController controller)
        {
            InitializeComponent();
            this.controller = controller;
            IconsDockPanel.Children.Add(new SettingsImage(controller));
            CreateRoomRows();
            StartDate.SelectedDate = DateTime.Now;
            StartDate.DisplayDateStart = Settings.Instance.SeasonStartDate;
            StartDate.DisplayDateEnd = Settings.Instance.SeasonEndDate;
            EndDate.SelectedDate = DateTime.Now.AddDays(13);
            EndDate.DisplayDateStart = Settings.Instance.SeasonStartDate;
            EndDate.DisplayDateEnd = Settings.Instance.SeasonEndDate;
            SearchIn.SelectedIndex = 0;
            controller.OnReservationsChanged += CreateReservationsTable;
            controller.OnReservationAdd += ReservationWindowRequested;
            controller.OnReservationEdit += ReservationWindowRequested;
        }

        internal void ReservationWindowRequested(object room, DateTime startDate)
        {
            new ReservationWindow((int)room, startDate, controller).ShowDialog();
        }

        internal void ReservationWindowRequested(object sender, int id)
        {
            new ReservationWindow(id, controller).ShowDialog();
        }

        public void CreateReservationsTable(object sender, string fileName)
        {
            DateTime startDate = StartDate.SelectedDate.GetValueOrDefault(DateTime.Now);
            Table.Children.Clear();
            for (int row = 0; row < controller.Rooms.RealCount; row++)
            {
                int room = controller.Rooms.TopFloorFirst[row].FullRoomNumber;
                int skipColumns = 0;
                for (int col = 0; col <= DaysToShow; col++)
                {
                    DateTime nextDate = startDate.AddDays(col);
                    DockPanel dockPanel = new DockPanel();
                    TextBox tableTextBox = new ReservationTextBox(controller, row, nextDate);
                    int? id = controller.Reservations.GetReservation(room, nextDate)?.Id;
                    if (id != null && controller.Reservations.IsFromBooking(id.Value))
                    {
                        dockPanel.Children.Add(new BookingImage());
                    }
                    dockPanel.Children.Add(tableTextBox);
                    if (skipColumns-- > 0) continue;
                    Grid.SetRow(dockPanel, row);
                    Grid.SetColumn(dockPanel, col);
                    int nights = controller.ReservationNights(room, nextDate);
                    Grid.SetColumnSpan(dockPanel, nights);
                    skipColumns = nights > 1 ? GetColumnsToSkip(room, startDate, nextDate, nights) : 0;
                    Table.Children.Add(dockPanel);
                }
            }
        }

        private int GetColumnsToSkip(int room, DateTime startDate, DateTime nextDate, int nights)
        {
            if (startDate < controller.ReservationStartDate(room, nextDate)) return nights - 1;
            return (controller.ReservationEndDate(room, nextDate) - startDate).Days - 1;
        }

        private void CreateDatesColumns(DateTime startDate)
        {
            Dates.Children.Clear();
            Dates.ColumnDefinitions.Clear();
            Table.ColumnDefinitions.Clear();
            for (int col = 0; col <= DaysToShow; col++)
            {
                Dates.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });
                Table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });

                TextBox dateTextBox = new DatesTextBox(startDate.AddDays(col));
                Grid.SetColumn(dateTextBox, col);
                Dates.Children.Add(dateTextBox);
            }
        }

        private void CreateRoomRows()
        {
            for (int row = 0; row < controller.Rooms.RealCount; row++)
            {
                int rowHeight = controller.Rooms.TopFloorFirst[row].LastOnFloor ? 50 : 30;
                Rooms.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight), MinHeight = 30 });
                Table.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight), MinHeight = 30 });

                TextBox roomTextBox = new RoomsTextBox(controller, row);
                Grid.SetRow(roomTextBox, row);
                Rooms.Children.Add(roomTextBox);
            }
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e) => new CalendarWindow(this).ShowDialog();

        private void SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!StartDate.SelectedDate.HasValue || !EndDate.SelectedDate.HasValue) return;
            if (StartDate.SelectedDate > EndDate.SelectedDate) return;
            DaysToShow = (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days;
            CreateDatesColumns(StartDate.SelectedDate.Value);
            CreateReservationsTable(this, string.Empty);
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsTable.Children.Clear();
            ResultsTable.RowDefinitions.Clear();
            string searchCriteria = SearchBox.Text;
            bool excludeCanceled = !IncludeCanceled.IsChecked.HasValue || !IncludeCanceled.IsChecked.Value;
            DateTime? searchStartDate = SearchStartDate.SelectedDate;
            DateTime? searchEndDate = SearchEndDate.SelectedDate;
            IEnumerable<int> results = SearchIn.SelectedIndex switch
            {
                0 => controller.Reservations.SearchInGuestName(searchCriteria, excludeCanceled),
                1 => controller.Reservations.SearchInNotes(searchCriteria, excludeCanceled),
                2 => controller.Reservations.SearchInStartDateIncluded(searchStartDate, searchEndDate, excludeCanceled),
                3 => controller.Reservations.SearchInEndDateIncluded(searchStartDate, searchEndDate, excludeCanceled),
                4 => controller.Reservations.SearchInAllDatesIncluded(searchStartDate, searchEndDate, excludeCanceled),
                _ => new List<int>()
            };
            SearchResults.Text = $"Обща сума на резервациите: {controller.Reservations.SumOfReservationsWithIds(results)} | ";
            SearchResults.Text += $"Намерени резултати: {results.Count()}";
            int index = 0;
            foreach (int id in results)
            {
                ResultsTable.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30), MinHeight = 30 });
                TextBox tableTextBox = new ReservationTextBox(controller, id);
                Grid.SetRow(tableTextBox, index++);
                ResultsTable.Children.Add(tableTextBox);
            }
        }

        private void SearchIn_Loaded(object sender, RoutedEventArgs e)
        {
            SearchIn.ItemsSource = new[]
            {
                "Име на госта", "Допълнителна информация", "Начална дата в периода", "Крайна дата в периода", "Изцяло в периода"
            };
        }

        private void SearchIn_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchIn.SelectedIndex == 0 || SearchIn.SelectedIndex == 1)
            {
                SearchBox.Visibility = Visibility.Visible;
                SearchStartDateText.Visibility = Visibility.Hidden;
                SearchStartDate.Visibility = Visibility.Hidden;
                SearchEndDateText.Visibility = Visibility.Hidden;
                SearchEndDate.Visibility = Visibility.Hidden;
            }
            else
            {
                SearchBox.Visibility = Visibility.Hidden;
                SearchStartDateText.Visibility = Visibility.Visible;
                SearchStartDate.Visibility = Visibility.Visible;
                SearchEndDateText.Visibility = Visibility.Visible;
                SearchEndDate.Visibility = Visibility.Visible;
            }
        }
    }
}