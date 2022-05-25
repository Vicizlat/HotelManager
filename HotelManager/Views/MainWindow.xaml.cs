using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HotelManager.Controller;
using HotelManager.Views.Templates;
using HotelManager.Utils;
using HotelManager.Handlers;
using HotelManager.Views.Images;
using HotelManager.Data.Models.Enums;
using System.Drawing;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using Microsoft.Win32;

namespace HotelManager.Views
{
    public partial class MainWindow
    {
        public DateTime CalendarSelectedStartDate { get; set; }
        public DateTime CalendarSelectedEndDate { get; set; }
        internal int DaysToShow;
        private readonly MainController controller;
        private DateTime?[] selectedDates = Array.Empty<DateTime?>();
        private PdfDocument pdfDoc;

        public MainWindow(MainController controller)
        {
            Logging.Instance.WriteLine("Start initializing MainWindow...");
            InitializeComponent();
            this.controller = controller;
            StartDate.SelectedDate = Constants.SeasonStartDate;
            //StartDate.SelectedDate = DateTime.Today;
            EndDate.SelectedDate = StartDate.SelectedDate.Value.AddDays(13);
            controller.OnReservationsChanged += CreateReservationsTable;
            controller.OnRoomsChanged += CreateReservationsTable;
            controller.OnReservationAdd += ReservationWindowRequested;
            controller.OnReservationEdit += ReservationWindowRequested;
            Logging.Instance.WriteLine("End initializing MainWindow...");
        }

        private void SettingsImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new SettingsWindow().ShowDialog();

        private void HotelImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new HotelSetupWindow(controller).ShowDialog();

        private void ImportExportImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new ImportExportWindow(controller).ShowDialog();

        private void TransactionImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new TransactionsWindow(controller).ShowDialog();

        private void PriceRangeImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            new PriceRangesWindow(controller).ShowDialog();
        }

        private void SearchImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => new SearchWindow(controller).Show();

        private void SavePdfImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                DefaultExt = ".pdf",
                AddExtension = true,
                OverwritePrompt = true,
                Filter = "PDF Files (*.pdf)|*.pdf"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                pdfDoc.SaveToFile(saveFileDialog.FileName);
            }
        }

        private void PrintImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => pdfDoc.Print();

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
            Rooms.Children.Clear();
            Table.Children.Clear();
            Dates.Children.Clear();
            Rooms.RowDefinitions.Clear();
            Table.RowDefinitions.Clear();
            Dates.ColumnDefinitions.Clear();
            Table.ColumnDefinitions.Clear();

            pdfDoc = new PdfDocument();
            PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Arial", 12f), true);
            PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("Arial", 9f), true);
            PdfPageBase page = pdfDoc.Pages.Add(PdfPageSize.A4, new PdfMargins(10), PdfPageRotateAngle.RotateAngle0, PdfPageOrientation.Landscape);
            page.Canvas.DrawString($"Резервации от {StartDate.SelectedDate:dd.MM.yyyy} до {EndDate.SelectedDate:dd.MM.yyyy}", font1, PdfBrushes.Black, 0, 0);
            PdfGrid mainPdfGrid = new PdfGrid();
            mainPdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Inside;
            PdfGridRow datesRow = mainPdfGrid.Rows.Add();
            datesRow.Height = 15;
            PdfGridRow tableRow = mainPdfGrid.Rows.Add();
            tableRow.Height = 540;
            mainPdfGrid.Columns.Add(2);
            mainPdfGrid.Columns[0].Width = 65;
            mainPdfGrid.Columns[1].Width = (DaysToShow + 1) * 70;
            PdfGrid datesPdfGrid = new PdfGrid();
            datesPdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
            datesPdfGrid.Rows.Add();
            datesPdfGrid.Rows[0].Height = 15;
            datesPdfGrid.Columns.Add(DaysToShow + 1);
            PdfGrid roomsPdfGrid = new PdfGrid();
            roomsPdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
            roomsPdfGrid.Columns.Add(1);
            roomsPdfGrid.Columns[0].Width = 65;
            PdfGrid tablePdfGrid = new PdfGrid();
            tablePdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
            tablePdfGrid.Rows.Add();
            tablePdfGrid.Columns.Add(DaysToShow + 1);

            for (int col = 0; col <= DaysToShow; col++)
            {
                Dates.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });
                Table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150), MinWidth = 150 });
                datesPdfGrid.Columns[col].Width = 70;
                datesPdfGrid.Columns[col].Format.Alignment = PdfTextAlignment.Center;
                datesPdfGrid.Columns[col].Format.LineAlignment = PdfVerticalAlignment.Middle;
                tablePdfGrid.Columns[col].Width = 70;
                tablePdfGrid.Columns[col].Format.Alignment = PdfTextAlignment.Left;
                tablePdfGrid.Columns[col].Format.LineAlignment = PdfVerticalAlignment.Top;
                tablePdfGrid.Columns[col].Format.LineLimit = false;
                tablePdfGrid.Columns[col].Format.WordWrap = PdfWordWrapType.Character;
            }
            Dictionary<int, bool> rooms = controller.GetRoomsDictionary();
            DateTime startDate = StartDate.SelectedDate.GetValueOrDefault(DateTime.Today);
            List<ReservationInfo> resInfos = controller.GetReservationInfos(startDate, startDate.AddDays(DaysToShow));
            for (int row = 0; row < rooms.Count; row++)
            {
                int room = rooms.ElementAt(row).Key;
                bool roomIsLastOnFloor = rooms.ElementAt(row).Value;
                int rowHeight = roomIsLastOnFloor ? 50 : 30;
                int skipColumns = 0;
                Rooms.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight), MinHeight = 30 });
                Table.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight), MinHeight = 30 });
                TextBox roomTextBox = new RoomsTextBox(controller, room, roomIsLastOnFloor);
                Grid.SetRow(roomTextBox, row);
                Rooms.Children.Add(roomTextBox);

                roomsPdfGrid.Rows.Add();
                roomsPdfGrid.Rows[row].Height = tableRow.Height / rooms.Count;
                roomsPdfGrid.Rows[row].Cells[0].Value = roomTextBox.Text;
                roomsPdfGrid.Rows[row].Cells[0].Style.Font = font2;
                roomsPdfGrid.Rows[row].Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
                tablePdfGrid.Rows.Add();
                tablePdfGrid.Rows[row].Height = tableRow.Height / rooms.Count;

                for (int col = 0; col <= DaysToShow; col++)
                {
                    TextBox dateTextBox = new DatesTextBox(StartDate.SelectedDate.GetValueOrDefault().AddDays(col));
                    Grid.SetColumn(dateTextBox, col);
                    Dates.Children.Add(dateTextBox);

                    datesPdfGrid.Rows[0].Cells[col].Value = dateTextBox.Text;
                    datesPdfGrid.Rows[0].Cells[col].Style.Font = font2;

                    if (skipColumns-- > 0) continue;
                    if (skipColumns < 0) skipColumns = 0;
                    DateTime nextDate = startDate.AddDays(col);
                    DockPanel dockPanel = new DockPanel();
                    ReservationInfo resInfo = resInfos.Where(r => r.StartDate <= nextDate && nextDate < r.EndDate && r.Room == room)
                        .FirstOrDefault(r => r.StateInt != (int)State.Canceled);
                    //ReservationInfo resInfo = controller.GetReservationInfo(room, nextDate);
                    if (resInfo != null)
                    {
                        dockPanel.Children.Add(new ReservationIcon(resInfo.SourceInt));
                        int nights = (resInfo.EndDate - resInfo.StartDate).Days;
                        Grid.SetColumnSpan(dockPanel, nights);
                        skipColumns = nights <= 1 ? 0 : startDate < resInfo.StartDate ? nights - 1 : (resInfo.EndDate - startDate).Days - 1;
                    }
                    else resInfo = new ReservationInfo { Room = room, StartDate = nextDate, StateInt = -1 };
                    ReservationTextBox reservationTextBox = new ReservationTextBox(controller, resInfo, roomIsLastOnFloor);
                    dockPanel.Children.Add(reservationTextBox);
                    Grid.SetRow(dockPanel, row);
                    Grid.SetColumn(dockPanel, col);
                    Table.Children.Add(dockPanel);

                    int columnSpan = skipColumns + 1 < tablePdfGrid.Columns.Count - col ? skipColumns + 1 : tablePdfGrid.Columns.Count - col;
                    tablePdfGrid.Rows[row].Cells[col].Value = reservationTextBox.Text;
                    tablePdfGrid.Rows[row].Cells[col].ColumnSpan = columnSpan;
                    tablePdfGrid.Rows[row].Cells[col].Style.Font = font2;
                    if (resInfo.StateInt == 1)
                    {
                        tablePdfGrid.Rows[row].Cells[col].Style.BackgroundBrush = PdfBrushes.LightBlue;
                    }
                }
            }
            datesRow.Cells[1].Value = datesPdfGrid;
            tableRow.Cells[0].Value = roomsPdfGrid;
            tableRow.Cells[1].Value = tablePdfGrid;
            mainPdfGrid.Draw(page, 0, 15);
            selectedDates = new DateTime?[]
            {
                StartDate.SelectedDate,
                EndDate.SelectedDate
            };
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
            CreateReservationsTable(this, EventArgs.Empty);
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