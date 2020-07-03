using System;
using System.Windows;
using System.Windows.Controls;
using Core;
using Handlers;
using Templates;

namespace HotelManager
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
            StartDate.SelectedDate = DateTime.Now;
            EndDate.SelectedDate = DateTime.Now.AddDays(13);
            Reservations.Instance.OnReservationsChanged += CreateReservationsTable;
            Reservations.Instance.AddReservationWindowRequested += ReservationWindowRequested;
            Reservations.Instance.EditReservationWindowRequested += ReservationWindowRequested;
        }

        internal void ReservationWindowRequested(Reservation reservation) => new ReservationWindow(reservation).ShowDialog();

        internal void ReservationWindowRequested(int room, DateTime startDate)
        {
            int id = Reservations.Instance.Count + 1;
            new ReservationWindow(id, room, startDate).ShowDialog();
        }

        public void CreateReservationsTable()
        {
            if (FileHandler.WriteToFile("Reservations", Reservations.Instance.ToStringArray())) FtpHandler.TryUploadFile("Reservations");
            Table.Children.Clear();
            for (int row = 0; row < 25; row++)
            {
                int skipColumns = 0;
                for (int col = 0; col <= DaysToShow; col++)
                {
                    DateTime date = StartDateSelectedDate.AddDays(col);
                    Reservation reservation = Reservations.Instance.GetReservation(StaticTemplates.GetRoomNumber(row), date);
                    TextBox tableTextBox = new TextBoxTemplate().ReservationsTextBox(row, date, reservation != null && reservation.Status ? reservation : null);
                    if (skipColumns-- > 0) continue;
                    Grid.SetRow(tableTextBox, row);
                    Grid.SetColumn(tableTextBox, col);
                    Grid.SetColumnSpan(tableTextBox, reservation != null && reservation.Status ? reservation.Period.Nights : 1);
                    if (reservation == null || !reservation.Status) skipColumns = 0;
                    else if (StartDateSelectedDate < reservation.Period.StartDate) skipColumns = reservation.Period.Nights - 1;
                    else skipColumns = (reservation.Period.EndDate - StartDateSelectedDate).Days - 1;
                    Table.Children.Add(tableTextBox);
                }
            }
        }

        private void CreateDatesColumns()
        {
            Dates.Children.Clear();
            Dates.ColumnDefinitions.Clear();
            Table.ColumnDefinitions.Clear();
            for (int col = 0; col <= DaysToShow; col++)
            {
                Dates.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });
                Table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });

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
                Rooms.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight), MinHeight = 30 });
                Table.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight), MinHeight = 30 });

                TextBox roomTextBox = new TextBoxTemplate().RoomsTextBox(row);
                Grid.SetRow(roomTextBox, row);
                Rooms.Children.Add(roomTextBox);
            }
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e) => new CalendarWindow(this).ShowDialog();

        private void SelectedDatesChanged()
        {
            if (StartDateSelectedDate > EndDateSelectedDate) return;
            DaysToShow = (EndDateSelectedDate - StartDateSelectedDate).Days;
            CreateDatesColumns();
            CreateReservationsTable();
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