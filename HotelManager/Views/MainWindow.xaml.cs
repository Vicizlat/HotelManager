using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HotelManager.Controller;
using HotelManager.Views.Templates;
using HotelManager.Utils;
using System.Linq;
using HotelManager.Handlers;
using HotelManager.Views.Images;

namespace HotelManager.Views
{
    public partial class MainWindow
    {
        internal int DaysToShow;
        private readonly MainController controller;
        private Dictionary<int, bool> rooms;
        private DateTime?[] selectedDates = Array.Empty<DateTime?>();

        public MainWindow(MainController controller)
        {
            Logging.Instance.WriteLine("Start initializing MainWindow...");
            InitializeComponent();
            this.controller = controller;
            ImportExportImage.MouseLeftButtonDown += delegate { new ImportExportWindow(controller).ShowDialog(); };
            HotelImage.MouseLeftButtonDown += delegate { new HotelSetupWindow(controller).ShowDialog(); };
            CreateRoomRows(this, EventArgs.Empty);
            //StartDate.SelectedDate = new DateTime(2020, 8, 12);
            StartDate.SelectedDate = DateTime.Today;
            StartDate.DisplayDateStart = Settings.Instance.SeasonStartDate;
            StartDate.DisplayDateEnd = Settings.Instance.SeasonEndDate;
            EndDate.SelectedDate = StartDate.SelectedDate.Value.AddDays(13);
            EndDate.DisplayDateStart = Settings.Instance.SeasonStartDate;
            EndDate.DisplayDateEnd = Settings.Instance.SeasonEndDate;
            controller.OnReservationsChanged += CreateReservationsTable;
            controller.OnRoomsChanged += CreateRoomRows;
            controller.OnReservationAdd += ReservationWindowRequested;
            controller.OnReservationEdit += ReservationWindowRequested;
            Logging.Instance.WriteLine("End initializing MainWindow...");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            new SearchWindow(controller).Show();
        }

        private void ReservationWindowRequested(object room, DateTime startDate)
        {
            new ReservationWindow((int)room, startDate, controller).ShowDialog();
        }

        private void ReservationWindowRequested(object sender, int id)
        {
            new ReservationWindow(controller.GetReservationInfo(id), controller).ShowDialog();
        }

        public void CreateReservationsTable(object sender, EventArgs e)
        {
            Logging.Instance.WriteLine("Start adding Table cells...");
            Table.Children.Clear();
            DateTime startDate = StartDate.SelectedDate.GetValueOrDefault(DateTime.Today);
            for (int row = 0; row < rooms.Count; row++)
            {
                int room = rooms.ElementAt(row).Key;
                int skipColumns = 0;
                for (int col = 0; col <= DaysToShow; col++)
                {
                    if (skipColumns-- > 0) continue;
                    DateTime nextDate = startDate.AddDays(col);
                    DockPanel dockPanel = new DockPanel();
                    ReservationInfo resInfo = controller.GetReservationInfo(room, nextDate);
                    if (resInfo != null)
                    {
                        dockPanel.Children.Add(resInfo.SourceInt switch
                        {
                            0 => new ReservationIcon("Phone"),
                            1 => new ReservationIcon("Mail"),
                            2 => new ReservationIcon("Booking"),
                            3 => new ReservationIcon("HotelIconC"),
                            4 => new ReservationIcon("Friend"),
                            _ => new ReservationIcon("Phone")
                        });
                        int nights = (resInfo.EndDate - resInfo.StartDate).Days;
                        Grid.SetColumnSpan(dockPanel, nights);
                        skipColumns = nights <= 1 ? 0 : startDate < resInfo.StartDate
                                ? nights - 1 : (resInfo.EndDate - startDate).Days - 1;
                    }
                    else resInfo = new ReservationInfo { Room = room, StartDate = nextDate, StateInt = -1 };
                    dockPanel.Children.Add(new ReservationTextBox(controller, resInfo, rooms.ElementAt(row).Value));
                    Grid.SetRow(dockPanel, row);
                    Grid.SetColumn(dockPanel, col);
                    Table.Children.Add(dockPanel);
                }
            }
            selectedDates = new DateTime?[]
            {
                StartDate.SelectedDate,
                EndDate.SelectedDate
            };
            Logging.Instance.WriteLine("End adding Table cells...");
        }

        private void CreateRoomRows(object sender, EventArgs e)
        {
            Logging.Instance.WriteLine("Start adding Rooms...");
            rooms = controller.Context.Rooms
                .OrderByDescending(r => r.Floor.FloorNumber)
                .ThenBy(r => r.RoomNumber)
                .Select(x => new { x.FullRoomNumber, x.LastOnFloor })
                .ToDictionary(key => key.FullRoomNumber, value => value.LastOnFloor);
            Rooms.Children.Clear();
            Rooms.RowDefinitions.Clear();
            Table.RowDefinitions.Clear();
            for (int row = 0; row < rooms.Count; row++)
            {
                int room = rooms.ElementAt(row).Key;
                int rowHeight = rooms.ElementAt(row).Value ? 50 : 30;
                Rooms.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight), MinHeight = 30 });
                Table.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight), MinHeight = 30 });

                TextBox roomTextBox = new RoomsTextBox(controller, room, rooms.ElementAt(row).Value);
                Grid.SetRow(roomTextBox, row);
                Rooms.Children.Add(roomTextBox);
            }
            Logging.Instance.WriteLine("End adding Rooms...");
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            CalendarWindow calendarWindow = new CalendarWindow { Owner = this };
            calendarWindow.ShowDialog();
        }

        private void SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedDates.Length != 2)
            {
                selectedDates = new DateTime?[]
                {
                    StartDate.SelectedDate,
                    EndDate.SelectedDate
                };
            }
            if (selectedDates[0] == StartDate.SelectedDate && selectedDates[1] == EndDate.SelectedDate) return;
            if (!StartDate.SelectedDate.HasValue || !EndDate.SelectedDate.HasValue) return;
            if (StartDate.SelectedDate >= EndDate.SelectedDate) return;

            DaysToShow = (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days;
            Logging.Instance.WriteLine($"SelectedDatesChanged: DaysToShow - {DaysToShow}");
            CreateDatesColumns();
            CreateReservationsTable(this, EventArgs.Empty);
        }

        private void CreateDatesColumns()
        {
            Logging.Instance.WriteLine("Start adding Dates...");
            Dates.Children.Clear();
            Dates.ColumnDefinitions.Clear();
            Table.ColumnDefinitions.Clear();
            for (int col = 0; col <= DaysToShow; col++)
            {
                Dates.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });
                Table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });

                TextBox dateTextBox = new DatesTextBox(StartDate.SelectedDate.GetValueOrDefault().AddDays(col));
                Grid.SetColumn(dateTextBox, col);
                Dates.Children.Add(dateTextBox);
            }
            Logging.Instance.WriteLine("End adding Dates...");
        }

        private void ScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (Equals(sender, SvTable))
            {
                SvRooms.ScrollToVerticalOffset(e.VerticalOffset);
                SvDates.ScrollToHorizontalOffset(e.HorizontalOffset);
            }
            else if (Equals(sender, SvRooms))
            {
                SvTable.ScrollToVerticalOffset(e.VerticalOffset);
            }
        }
    }
}