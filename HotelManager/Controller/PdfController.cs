using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace HotelManager.Controller
{
    public static class PdfController
    {
        private static readonly PdfTrueTypeFont font1 = new PdfTrueTypeFont(new Font("Arial", 12f), true);
        private static readonly PdfTrueTypeFont font2 = new PdfTrueTypeFont(new Font("Arial", 9f), true);
        private static readonly int pdfGridCellWidth = 78;
        private static readonly int pdfGridRoomsColWidth = 42;
        private static readonly int pdfGridDatesRowHeight = 15;
        private static readonly float pdfGridTableRowHeight = 540;

        public static PdfDocument GeneratePdf(DateTime[] dates, List<string> rooms, List<List<Tuple<string[], DateTime[], bool>>> res)
        {
            int daysToShow = (dates[1] - dates[0]).Days + 1;
            PdfDocument pdfDoc = new PdfDocument();
            PdfPageBase page = CreatePdfPage(pdfDoc, dates[0], dates[1]);
            PdfGrid mainPdfGrid = GetMainGrid(daysToShow);
            PdfGrid datesPdfGrid = GetDatesGrid(daysToShow);
            PdfGrid roomsPdfGrid = GetRoomsGrid();
            PdfGrid tablePdfGrid = GetTableGrid(daysToShow);

            for (int row = 0; row < rooms.Count; row++)
            {
                int skipColumns = 0;
                SetRows(roomsPdfGrid, tablePdfGrid, GetRowHeight(rooms.Count), row, rooms[row]);
                for (int col = 0; col < daysToShow; col++)
                {
                    SetColumns(datesPdfGrid, tablePdfGrid, col, dates[0]);

                    if (skipColumns-- > 0) continue;
                    if (skipColumns < 0) skipColumns = 0;
                    DateTime nextDate = dates[0].AddDays(col);
                    if (!string.IsNullOrEmpty(res[row][col].Item1[0]))
                    {
                        int nights = (res[row][col].Item2[1] - res[row][col].Item2[0]).Days;
                        skipColumns = nights <= 1 ? 0 : dates[0] < res[row][col].Item2[0] ? nights - 1 : (res[row][col].Item2[1] - dates[0]).Days - 1;
                        int columnSpan = skipColumns + 1 < daysToShow - col ? skipColumns + 1 : daysToShow - col;
                        tablePdfGrid.Rows[row].Cells[col].ColumnSpan = columnSpan;
                        tablePdfGrid.Rows[row].Cells[col].Value = GetCellContent(GetRowHeight(rooms.Count), res[row][col].Item1, columnSpan);
                        tablePdfGrid.Rows[row].Cells[col].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Top);
                        tablePdfGrid.Rows[row].Cells[col].Style.BackgroundBrush = res[row][col].Item3 ? PdfBrushes.LightBlue : PdfBrushes.LightYellow;
                    }
                    else tablePdfGrid.Rows[row].Cells[col].Value = string.Empty;
                }
            }

            mainPdfGrid.Rows[0].Cells[1].Value = datesPdfGrid;
            mainPdfGrid.Rows[1].Cells[0].Value = roomsPdfGrid;
            mainPdfGrid.Rows[1].Cells[1].Value = tablePdfGrid;
            mainPdfGrid.Draw(page, 0, 15);
            return pdfDoc;
        }

        public static PdfPageBase CreatePdfPage(PdfDocument pdfDoc, DateTime? startDate, DateTime? endDate)
        {
            PdfPageBase page = pdfDoc.Pages.Add(PdfPageSize.A4, new PdfMargins(10), PdfPageRotateAngle.RotateAngle0, PdfPageOrientation.Landscape);
            page.Canvas.DrawString($"Резервации от {startDate:dd.MM.yyyy} до {endDate:dd.MM.yyyy}", font1, PdfBrushes.Black, 0, 0);
            return page;
        }

        public static PdfGrid GetMainGrid(int daysToShow)
        {
            PdfGrid mainPdfGrid = new PdfGrid();
            mainPdfGrid.Style.CellPadding = new PdfPaddings(0, 0, 0, 0);
            mainPdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
            mainPdfGrid.Columns.Add(2);
            mainPdfGrid.Columns[0].Format.Alignment = PdfTextAlignment.Left;
            mainPdfGrid.Columns[0].Format.LineAlignment = PdfVerticalAlignment.Top;
            mainPdfGrid.Columns[0].Width = pdfGridRoomsColWidth;
            mainPdfGrid.Columns[1].Format.Alignment = PdfTextAlignment.Left;
            mainPdfGrid.Columns[1].Format.LineAlignment = PdfVerticalAlignment.Top;
            mainPdfGrid.Columns[1].Width = daysToShow * pdfGridCellWidth;
            mainPdfGrid.Rows.Add().Height = pdfGridDatesRowHeight;
            mainPdfGrid.Rows.Add().Height = pdfGridTableRowHeight;
            return mainPdfGrid;
        }

        public static PdfGrid GetDatesGrid(int daysToShow)
        {
            PdfGrid datesPdfGrid = new PdfGrid();
            datesPdfGrid.Style.CellPadding = new PdfPaddings(0, 0, 0, 0);
            datesPdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
            datesPdfGrid.Rows.Add().Height = pdfGridDatesRowHeight;
            datesPdfGrid.Columns.Add(daysToShow);
            return datesPdfGrid;
        }

        public static PdfGrid GetRoomsGrid()
        {
            PdfGrid roomsPdfGrid = new PdfGrid();
            roomsPdfGrid.Style.CellPadding = new PdfPaddings(0, 0, 0, 0);
            roomsPdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
            roomsPdfGrid.Columns.Add().Width = pdfGridRoomsColWidth;
            return roomsPdfGrid;
        }

        public static PdfGrid GetTableGrid(int daysToShow)
        {
            PdfGrid tablePdfGrid = new PdfGrid();
            tablePdfGrid.Style.CellPadding = new PdfPaddings(0, 0, 0, 0);
            tablePdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
            tablePdfGrid.Columns.Add(daysToShow);
            return tablePdfGrid;
        }

        public static float GetRowHeight(int roomsCount)
        {
            return pdfGridTableRowHeight / (float)roomsCount;
        }

        public static void SetRows(PdfGrid roomsPdfGrid, PdfGrid tablePdfGrid, float pdfRowHeight, int row, string roomText)
        {
            roomsPdfGrid.Rows.Add().Height = pdfRowHeight;
            roomsPdfGrid.Rows[row].Cells[0].Value = roomText;
            roomsPdfGrid.Rows[row].Cells[0].Style.Font = font2;
            roomsPdfGrid.Rows[row].Cells[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
            tablePdfGrid.Rows.Add().Height = pdfRowHeight;
        }

        public static void SetColumns(PdfGrid datesPdfGrid, PdfGrid tablePdfGrid, int col, DateTime startDate)
        {
            datesPdfGrid.Columns[col].Width = pdfGridCellWidth;
            datesPdfGrid.Rows[0].Cells[col].Value = startDate.AddDays(col).ToShortDateString();
            datesPdfGrid.Rows[0].Cells[col].Style.Font = font2;
            datesPdfGrid.Rows[0].Cells[col].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
            tablePdfGrid.Columns[col].Width = pdfGridCellWidth;
            tablePdfGrid.Columns[col].Format.LineLimit = false;
            tablePdfGrid.Columns[col].Format.WordWrap = PdfWordWrapType.Character;
        }

        public static PdfGrid GetCellContent(float pdfRowHeight, string[] reservationText, int columnSpan)
        {
            PdfGrid cellPdfGrid = new PdfGrid();
            cellPdfGrid.Style.CellPadding = new PdfPaddings(0, 0, 0, 0);
            cellPdfGrid.Style.BorderOverlapStyle = PdfBorderOverlapStyle.Overlap;
            cellPdfGrid.Columns.Add(2);
            cellPdfGrid.Columns[0].Width = pdfRowHeight / 1.3f;
            cellPdfGrid.Columns[1].Width = columnSpan * pdfGridCellWidth - cellPdfGrid.Columns[0].Width;
            cellPdfGrid.Columns[1].Format.Alignment = PdfTextAlignment.Left;
            cellPdfGrid.Columns[1].Format.LineAlignment = PdfVerticalAlignment.Top;
            cellPdfGrid.Columns[1].Format.LineLimit = false;
            cellPdfGrid.Columns[1].Format.WordWrap = PdfWordWrapType.Character;
            cellPdfGrid.Rows.Add().Height = pdfRowHeight;
            cellPdfGrid.Rows[0].Cells[0].Value = GetCellImage(pdfRowHeight, reservationText[1]);
            cellPdfGrid.Rows[0].Cells[1].Value = reservationText[0];
            cellPdfGrid.Rows[0].Cells[1].Style.Font = font2;
            return cellPdfGrid;
        }

        public static PdfGridCellContentList GetCellImage(float pdfRowHeight, string imageUri)
        {
            PdfGridCellContentList contentList = new PdfGridCellContentList();
            PdfGridCellContent imageContent = new PdfGridCellContent();
            imageContent.Image = PdfImage.FromFile(@$"Views\{imageUri}");
            imageContent.ImageSize = new SizeF(pdfRowHeight / 1.3f, pdfRowHeight);
            contentList.List.Add(imageContent);
            return contentList;
        }
    }
}