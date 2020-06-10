using System;
using System.Windows;
using System.Windows.Controls;

namespace HotelManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CreateRoomRows();
            Reservations.Instance.LoadReservations();
            StartDate.SelectedDate = DateTime.Now;
            EndDate.SelectedDate = DateTime.Now.AddDays(13);
            Reservations.Instance.ReservationsUpdated += CreateReservationsTable;
        }

        public void CreateReservationsTable()
        {
            Table.Children.Clear();
            for (int row = 0; row < 25; row++)
            {
                int skipColumns = 0;
                for (int col = 0; col <= (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days; col++)
                {
                    DateTime date = StartDate.SelectedDate.Value.AddDays(col);
                    Reservation reservation = Reservations.Instance.GetReservation(GetRoom(row), date);
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
            Dates.Children.Clear();
            Dates.ColumnDefinitions.Clear();
            Table.ColumnDefinitions.Clear();
            for (int col = 0; col <= (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days; col++)
            {
                Dates.ColumnDefinitions.Add(ColumnDef(150));
                Table.ColumnDefinitions.Add(ColumnDef(150));

                TextBox dateTextBox = new TextBoxTemplate().DatesTextBox(col, StartDate.SelectedDate.Value);
                Grid.SetColumn(dateTextBox, col);
                Dates.Children.Add(dateTextBox);
            }
        }

        private void CreateRoomRows()
        {
            for (int row = 0; row < 25; row++)
            {
                int rowHeight = row == 0 || row == 8 || row == 16 ? 50 : 30;
                Rooms.RowDefinitions.Add(RowDef(rowHeight, 30));
                Table.RowDefinitions.Add(RowDef(rowHeight, 30));

                TextBox roomTextBox = new TextBoxTemplate().RoomsTextBox(row);
                Grid.SetRow(roomTextBox, row);
                Rooms.Children.Add(roomTextBox);
            }
        }

        private ColumnDefinition ColumnDef(double width)
        {
            return new ColumnDefinition() { MinWidth = width, Width = new GridLength(width), MaxWidth = width };
        }

        private RowDefinition RowDef(double rowHeight, double minHeight)
        {
            return new RowDefinition() { Height = new GridLength(rowHeight), MinHeight = minHeight };
        }

        private void SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDate.SelectedDate != null && EndDate.SelectedDate != null && EndDate.SelectedDate > StartDate.SelectedDate)
            {
                CreateDatesColumns();
                CreateReservationsTable();
            }
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            new CalendarWindow(this).ShowDialog();
        }
    }
}