using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Core;

namespace Templates
{
    public static class StaticTemplates
    {
        public static string GetTooltipText(Reservation reservation)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Име: {reservation.GuestName}");
            sb.AppendLine($"Брой гости: {reservation.GuestsInRoom}");
            sb.AppendLine($"Период: {reservation.Period.StartDate:dd.MM.yyyy} - {reservation.Period.EndDate:dd.MM.yyyy}");
            sb.AppendLine($"Обща цена: {reservation.Sums.Total}");
            sb.AppendLine($"Предплатена сума: {reservation.Sums.Paid}");
            sb.AppendLine($"Оставаща сума: {reservation.Sums.Remaining}");
            sb.Append($"Допълнителна информация: {reservation.AdditionalInformation}");
            return sb.ToString();
        }

        public static int GetRoomNumber(int row)
        {
            return row == 0 ? 32 : row >= 1 && row <= 8 ? 20 + row : row >= 9 && row <= 16 ? 2 + row : row - 16;
        }

        public static string GetReservationText(Reservation reservation)
        {
            return $"{reservation.GuestName} - {reservation.GuestsInRoom} гости - За плащане: {reservation.Sums.Remaining} лв.";
        }


        public static string GetRoomText(int row)
        {
            return 1 <= row && row <= 8 ? $"Стая 2{row}" : 9 <= row && row <= 16 ? $"Стая 1{row - 8}" : 17 <= row && row <= 24 ? $"Стая 0{row - 16}" : "Апартамент 32";
        }

        public static Thickness Margin(int row)
        {
            return row == 0 || row == 8 || row == 16 ? new Thickness(0, 0, 0, 20) : new Thickness(0);
        }
        
        public static MenuItem AddReservationMenuItem(int row, DateTime startDate)
        {
            return new MenuItem
            {
                Header = "Добави резервация",
                Command = new Commands.AddReservation(GetRoomNumber(row), startDate)
            };
        }

        public static MenuItem EditReservationMenuItem(Reservation reservation)
        {
            return new MenuItem
            {
                Header = "Редактирай резервация",
                Command = new Commands.EditReservation(reservation)
            };
        }
    }
}