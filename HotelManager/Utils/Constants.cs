using System;
using System.IO;

namespace HotelManager.Utils
{
    public static class Constants
    {
        private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string LocalPath = Directory.CreateDirectory(Path.Combine(AppDataPath, "HotelManager")).FullName;
        public static readonly string LogsPath = Directory.CreateDirectory(Path.Combine(LocalPath, "Logs")).FullName;
        public static string LogFileName = $"Log-{DateTime.Now:[yyyy-MM-dd][HH-mm-ss]}.txt";
        public static string LocalConnection = @"Server=.\SQLEXPRESS;Database=HotelManager;Integrated Security=True";
        public static string DoubleLine = Environment.NewLine + Environment.NewLine;

        public const string ReservationsFileName = "Reservations.json";
        public const string SettingsFilename = "Settings.xml";
        public const string ErrorRemoteFileDownload = "Не успях да сваля \"{0}\"";
        public const string ErrorRemoteFileUpload = "Не успях да кача \"{0}\"";
        public const string ErrorWriteFile = "Не успях да запиша \"{0}\"";
        public const string ErrorRemoteFileCheck = "Не успях да проверя файла \"{0}\" на сървъра.";
        public const string ContinueLocal = "Да продължа ли само с локалния файл?";
        public const string WarningLoss = "ВНИМАНИЕ: Може да доведе до загуба на резервации!";
        public const string NetworkError = "Проблем при връзката със сървъра";
        public const string CalendarButtonText = "Избор на период за показване";
        public const string StartDateText = "Начална дата: ";
        public const string EndDateText = "Крайна дата: ";
        public const string ReservationText = "{0} - {1} гости - За плащане: {2} лв.";
        public const string NoRoomSelected = "Няма избрана стая";
        public const string UnsavedChangesCaption = "Незапазени промени в резервацията";
        public const string UnsavedChangesText = "По тази резервация има незапазени промени.";
        public const string UnsavedChangesTextPayment = "За добавяне на плащане, резервацията трябва първо да бъде актуализирана.";
        public const string OverCapacityCaption = "Превишен капацитет на стаята";
        public const string OverCapacityText = "Броя гости е повече от капацитета на стаята.";
        public const string ConfirmSaveChanges = "Да запазя ли резервацията?";
        public static readonly string[] SearchOptions =
        {
            "Име на госта",
            "Допълнителна информация",
            "Начална дата в периода",
            "Крайна дата в периода",
            "Изцяло в периода"
        };
        public static readonly string[] ReservationStates =
        {
            "Активна",
            "Настанена",
            "Отменена"
        };
        public static readonly string[] ReservationSources =
        {
            "По телефон",
            "По имейл",
            "Booking.com",
            "На място",
            "Познат/Приятел"
        };
        public static readonly string[] ImportExportSources =
        {
            "Guests",
            "Reservations",
            "Transactions",
            "Buildings",
            "Floors",
            "Rooms"
        };
    }
}