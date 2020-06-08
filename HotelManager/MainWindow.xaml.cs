using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace HotelManager
{
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; set; }
        private readonly int Rows = 25;

        public MainWindow()
        {
            InitializeComponent();
            LogWriter.Instance.WriteLine($"Logging started");
            if (!File.Exists(Path.Combine(FileHandler.LocalPath, "Config.xml"))) FileHandler.TryGetConfigFile();
            Instance = this;
            StartDate.SelectedDate = DateTime.Now;
            EndDate.SelectedDate = DateTime.Now.AddDays(9);
            CreateRoomRows();
        }

        public void CreateReservationsTable()
        {
            int datesToShow = (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days + 1;
            Table.Children.Clear();
            for (int row = 0; row < Rows; row++)
            {
                int skipColumns = 0;
                for (int col = 0; col < datesToShow; col++)
                {
                    DateTime date = StartDate.SelectedDate.Value.AddDays(col);
                    Reservation reservation = Reservations.Instance.FindReservationByRoomAndDate(GetRoom(row), date);
                    TextBox tableTextBox = new TextBoxTemplate().ReservationsTextBox(row, date, reservation != null && reservation.Status ? reservation : null);
                    if (skipColumns-- > 0) continue;
                    Grid.SetRow(tableTextBox, row);
                    Grid.SetColumn(tableTextBox, col);
                    Grid.SetColumnSpan(tableTextBox, reservation != null && reservation.Status ? reservation.Nights : 1);
                    if (reservation == null || !reservation.Status) skipColumns = 0;
                    else if (StartDate.SelectedDate.Value < reservation.StartDate) skipColumns = reservation.Nights - 1;
                    else skipColumns = (reservation.EndDate - StartDate.SelectedDate.Value).Days - 1;
                    Table.Children.Add(tableTextBox);
                }
            }
        }

        private int GetRoom(int row)
        {
            if (row == 0) return 32;
            else if (row >= 1 && row <= 8) return 20 + row;
            else if (row >= 9 && row <= 16) return 2 + row;
            else return row - 16;
        }

        private void CreateDatesColumns()
        {
            int datesToShow = (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days + 1;
            Dates.ColumnDefinitions.Clear();
            Table.ColumnDefinitions.Clear();
            for (int col = 0; col < datesToShow; col++)
            {
                Dates.ColumnDefinitions.Add(ColumnDef(150, 150));
                Table.ColumnDefinitions.Add(ColumnDef(150, 150));

                TextBox dateTextBox = new TextBoxTemplate().DatesTextBox(col, StartDate.SelectedDate.Value);
                Grid.SetColumn(dateTextBox, col);
                Dates.Children.Add(dateTextBox);
            }
        }

        private void CreateRoomRows()
        {
            for (int row = 0; row < Rows; row++)
            {
                int rowHeight = row == 0 || row == 8 || row == 16 ? 50 : 30;
                Rooms.RowDefinitions.Add(RowDef(rowHeight, 30));
                Table.RowDefinitions.Add(RowDef(rowHeight, 30));

                TextBox roomTextBox = new TextBoxTemplate().RoomsTextBox(row);
                Grid.SetRow(roomTextBox, row);
                Rooms.Children.Add(roomTextBox);
            }
        }

        private ColumnDefinition ColumnDef(double minWidth, double maxWidth) => new ColumnDefinition() { MinWidth = minWidth, MaxWidth = maxWidth };

        private RowDefinition RowDef(double rowHeight, double minHeight)
        {
            return new RowDefinition() { Height = new GridLength(rowHeight, GridUnitType.Pixel), MinHeight = minHeight };
        }

        internal void AddReservation(int room, DateTime startDate)
        {
            new ReservationWindow(Reservations.Instance.Count + 1, true, room, "", startDate, null).ShowDialog();
        }

        internal void EditReservation(Reservation reservation)
        {
            if (reservation != null) new ReservationWindow(reservation).ShowDialog();
        }

        private void StartEndDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDate.SelectedDate != null && EndDate.SelectedDate != null && EndDate.SelectedDate > StartDate.SelectedDate)
            {
                CreateDatesColumns();
                CreateReservationsTable();
            }
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e) => new CalendarWindow(this).ShowDialog();

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => LogWriter.Instance.Close();
    }
}