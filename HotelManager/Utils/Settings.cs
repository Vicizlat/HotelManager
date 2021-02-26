using System;

namespace HotelManager.Utils
{
    public class Settings
    {
        private static Settings thisInstance;
        public static Settings Instance => thisInstance ?? new Settings();
        public string WebAddress { get; set; }
        public string WebAddressFull { get; set; }
        public string FtpAddress { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public DateTime SeasonStartDate = DateTime.Today;
        public DateTime SeasonEndDate = DateTime.Today.AddDays(365);
        public bool LocalUseOnly { get; set; }

        public Settings() => thisInstance = this;

        public static void CreateSettings(Settings settings) => thisInstance = settings;
    }
}