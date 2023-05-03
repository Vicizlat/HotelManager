using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManager.Views.Templates;
using HotelManager.Handlers;
using HotelManager.Views.Images;
using HotelManager.Controller;
using HotelManager.Models;

namespace HotelManager.Views
{
    public partial class MainWindow
    {
        public DateTime CalendarSelectedStartDate { get; set; }
        public DateTime CalendarSelectedEndDate { get; set; }
        internal int DaysToShow;
        private readonly MainController controller;
        private DateTime[] selectedDates = Array.Empty<DateTime>();
        private List<List<Tuple<string[], DateTime[], bool>>> dataForPdf;

        public MainWindow(MainController controller)
        {
            Logging.Instance.WriteLine("Start initializing MainWindow...");
            InitializeComponent();
            this.controller = controller;
            StartDate.SelectedDate = DateTime.Today;
            EndDate.SelectedDate = StartDate.SelectedDate.Value.AddDays(12);
            controller.OnReservationsChanged += delegate { CreateReservationsTable(); };
            controller.OnRoomsChanged += delegate { CreateReservationsTable(); };
            controller.OnReservationAdd += ReservationWindowRequested;
            controller.OnReservationEdit += ReservationWindowRequested;
            Logging.Instance.WriteLine("End initializing MainWindow...");
        }

        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new SettingsWindow().ShowDialog();

        private void HotelImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new HotelSetupWindow(controller).ShowDialog();

        private void ImportExportImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new ImportExportWindow(controller).ShowDialog();

        private void TransactionImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new TransactionsWindow(controller).ShowDialog();

        private void PriceRangeImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new PriceRangesWindow(controller).ShowDialog();

        private void SearchImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new SearchWindow(controller).Show();

        private void SavePdfImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (FileHandler.TryGetSaveFilePath(".pdf", out string filePath))
            {
                List<string> roomsTexts = MainController.RoomInfos.Skip(1).Select(r => r.DisplayName).ToList();
                PdfController.GeneratePdf(selectedDates, roomsTexts, dataForPdf).SaveToFile(filePath);
            }
        }

        private void PrintImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            List<string> roomsTexts = MainController.RoomInfos.Skip(1).Select(r => r.DisplayName).ToList();
            PdfController.GeneratePdf(selectedDates, roomsTexts, dataForPdf).Print();
        }

        private void AddGuest_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new EditGuestWindow(null, controller, "", "", "").ShowDialog();
        }

        private void ReservationWindowRequested(object room, DateTime startDate)
        {
            new ReservationWindow((int)room, startDate, controller).ShowDialog();
        }

        private void ReservationWindowRequested(object sender, int id)
        {
            new ReservationWindow(controller.GetReservationInfo(id), controller).ShowDialog();
        }

        public void CreateReservationsTable()
        {
            Logging.Instance.WriteLine("Start adding Table cells...");
            Rooms.Children.Clear();
            Table.Children.Clear();
            Dates.Children.Clear();
            Rooms.RowDefinitions.Clear();
            Table.RowDefinitions.Clear();
            Dates.ColumnDefinitions.Clear();
            Table.ColumnDefinitions.Clear();
            for (int col = 0; col < DaysToShow; col++)
            {
                Table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });
            }
            DateTime startDate = StartDate.SelectedDate.GetValueOrDefault(DateTime.Today);
            List<ReservationInfo> resInfos = controller.GetReservationInfos(startDate, startDate.AddDays(DaysToShow - 1));
            dataForPdf = new List<List<Tuple<string[], DateTime[], bool>>>();
            for (int row = 0; row < MainController.RoomInfos.Skip(1).Count(); row++)
            {
                dataForPdf.Add(new List<Tuple<string[], DateTime[], bool>>());
                int skipColumns = 0;
                RoomInfo roomInfo = MainController.RoomInfos.Skip(1).ToList()[row];
                Rooms.RowDefinitions.Add(new RowDefinition { Height = new GridLength(roomInfo.LastOnFloor ? 50 : 30), MinHeight = 30 });
                Table.RowDefinitions.Add(new RowDefinition { Height = new GridLength(roomInfo.LastOnFloor ? 50 : 30), MinHeight = 30 });
                TextBox roomTextBox = new RoomsTextBox(controller, roomInfo);
                Grid.SetRow(roomTextBox, row);
                Rooms.Children.Add(roomTextBox);
                for (int col = 0; col < DaysToShow; col++)
                {
                    Dates.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });
                    TextBox dateTextBox = new DatesTextBox(startDate.AddDays(col));
                    Grid.SetColumn(dateTextBox, col);
                    Dates.Children.Add(dateTextBox);
                    dataForPdf[row].Add(new Tuple<string[], DateTime[], bool>(Array.Empty<string>(), Array.Empty<DateTime>(), false));
                    if (skipColumns-- > 0) continue;
                    if (skipColumns < 0) skipColumns = 0;
                    DateTime nextDate = startDate.AddDays(col);
                    DockPanel dockPanel = new DockPanel();
                    ReservationInfo resInfo = resInfos.Where(r => r.StartDate <= nextDate && nextDate < r.EndDate)
                        .FirstOrDefault(r => r.Room == roomInfo.FullRoomNumber);
                    ReservationIcon image = null;
                    if (resInfo != null)
                    {
                        image = new ReservationIcon(resInfo.SourceInt);
                        dockPanel.Children.Add(image);
                        int nights = (resInfo.EndDate - resInfo.StartDate).Days;
                        Grid.SetColumnSpan(dockPanel, nights);
                        skipColumns = nights <= 1 ? 0 : startDate < resInfo.StartDate ? nights - 1 : (resInfo.EndDate - startDate).Days - 1;
                    }
                    else resInfo = new ReservationInfo { Room = roomInfo.FullRoomNumber, StartDate = nextDate, StateInt = -1 };
                    ReservationTextBox resTextBox = new ReservationTextBox(controller, resInfo, roomInfo.LastOnFloor);
                    dockPanel.Children.Add(resTextBox);
                    Grid.SetRow(dockPanel, row);
                    Grid.SetColumn(dockPanel, col);
                    Table.Children.Add(dockPanel);
                    dataForPdf[row][col] = new Tuple<string[], DateTime[], bool>
                    (
                        new string[] { resTextBox.Text, image?.ImageUri ?? string.Empty },
                        new DateTime[] { resInfo.StartDate, resInfo.EndDate },
                        resInfo.StateInt == 1
                    );
                }
            }
            selectedDates = new DateTime[] { StartDate.SelectedDate.GetValueOrDefault(), EndDate.SelectedDate.GetValueOrDefault() };
            Logging.Instance.WriteLine("End adding Table cells...");
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            if (new CalendarWindow() { Owner = this }.ShowDialog() == true)
            {
                StartDate.SelectedDate = CalendarSelectedStartDate;
                EndDate.SelectedDate = CalendarSelectedEndDate;
                Logging.Instance.WriteLine("Dates changed from Calendar.");
            }
        }

        private void SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedDates.Length != 2)
            {
                selectedDates = new DateTime[] { StartDate.SelectedDate.GetValueOrDefault(), EndDate.SelectedDate.GetValueOrDefault() };
            }
            if (selectedDates[0] == StartDate.SelectedDate && selectedDates[1] == EndDate.SelectedDate) return;
            if (!StartDate.SelectedDate.HasValue || !EndDate.SelectedDate.HasValue) return;
            if (StartDate.SelectedDate >= EndDate.SelectedDate) return;
            DaysToShow = (EndDate.SelectedDate - StartDate.SelectedDate).Value.Days + 1;
            Logging.Instance.WriteLine($"SelectedDatesChanged: DaysToShow - {DaysToShow}");
            CreateReservationsTable();
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