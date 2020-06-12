using System;
using System.Windows;
using System.Windows.Controls;
using Core;
using Templates;

namespace Gui
{
    public partial class MainWindow
    {
        internal DateTime StartDateSelectedDate;
        internal DateTime EndDateSelectedDate;
        internal int DaysToShow;

        public MainWindow()
        {
            InitializeComponent();
            CreateRoomRows();
            Reservations.Instance.LoadReservations();
            StartDate.SelectedDate = DateTime.Now;
            EndDate.SelectedDate = DateTime.Now.AddDays(13);
            Reservations.Instance.ReservationsUpdated += CreateReservationsTable;
            Reservations.Instance.ReservationWindow1Requested += ReservationWindowRequested;
            Reservations.Instance.ReservationWindow2Requested += ReservationWindowRequested;
        }

        private void ReservationWindowRequested(Reservation reservation) => new ReservationWindow(reservation).ShowDialog();

        private void ReservationWindowRequested(int room, DateTime startDate) => new ReservationWindow(room, startDate).ShowDialog();

        public void CreateReservationsTable()
        {
            Table.Children.Clear();
            for (int row = 0; row < 25; row++)
            {
                int skipColumns = 0;
                for (int col = 0; col <= DaysToShow; col++)
                {
                    DateTime date = StartDateSelectedDate.AddDays(col);
                    Reservation reservation = Reservations.Instance.GetReservation(GetRoom(row), date);
                    TextBox tableTextBox = new TextBoxTemplate().ReservationsTextBox(row, date, reservation != null && reservation.Status ? reservation : null);
                    if (skipColumns-- > 0) continue;
                    Grid.SetRow(tableTextBox, row);
                    Grid.SetColumn(tableTextBox, col);
                    Grid.SetColumnSpan(tableTextBox, reservation != null && reservation.Status ? reservation.Nights : 1);
                    if (reservation == null || !reservation.Status) skipColumns = 0;
                    else if (StartDateSelectedDate < reservation.StartDate) skipColumns = reservation.Nights - 1;
                    else skipColumns = DaysToShow - 1;
                    Table.Children.Add(tableTextBox);
                }
            }
        }

        internal int GetRoom(int row)
        {
            if (row == 0) return 32;
            if (row >= 1 && row <= 8) return 20 + row;
            if (row >= 9 && row <= 16) return 2 + row;
            return row - 16;
        }

        private void CreateDatesColumns()
        {
            Dates.Children.Clear();
            Dates.ColumnDefinitions.Clear();
            Table.ColumnDefinitions.Clear();
            for (int col = 0; col <= DaysToShow; col++)
            {
                Dates.ColumnDefinitions.Add(ColumnDef(150));
                Table.ColumnDefinitions.Add(ColumnDef(150));

                TextBox dateTextBox = new TextBoxTemplate().DatesTextBox(col, StartDateSelectedDate);
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

        internal ColumnDefinition ColumnDef(double width)
        {
            return new ColumnDefinition() { MinWidth = width, Width = new GridLength(width), MaxWidth = width };
        }

        internal RowDefinition RowDef(double rowHeight, double minHeight)
        {
            return new RowDefinition() { Height = new GridLength(rowHeight), MinHeight = minHeight };
        }

        private void SelectedDatesChanged()
        {
            if (StartDateSelectedDate > EndDateSelectedDate) return;
            DaysToShow = (EndDateSelectedDate - StartDateSelectedDate).Days;
            CreateDatesColumns();
            CreateReservationsTable();
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            new CalendarWindow(this).ShowDialog();
        }

        private void StartDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDate.SelectedDate == null) return;
            StartDateSelectedDate = StartDate.SelectedDate.GetValueOrDefault(DateTime.Now);
            SelectedDatesChanged();
        }

        private void EndDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EndDate.SelectedDate == null) return;
            EndDateSelectedDate = EndDate.SelectedDate.GetValueOrDefault(StartDateSelectedDate.AddDays(13));
            SelectedDatesChanged();
        }
    }
}