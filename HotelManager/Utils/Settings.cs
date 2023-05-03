using System;

namespace HotelManager.Utils
{
    public class Settings
    {
        public static event EventHandler OnSettingsChanged;

        private static Settings thisInstance;
        public static Settings Instance => thisInstance ?? new Settings();
        public DateTime SeasonStartDate = DateTime.Today;
        public DateTime SeasonEndDate = DateTime.Today.AddDays(365);
        public string Server { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public Settings() => thisInstance = this;

        public static bool CreateSettings(object settings)
        {
            if (settings.GetType() != typeof(Settings)) return false;
            thisInstance = (Settings)settings;
            Constants.SeasonStartDate = thisInstance.SeasonStartDate;
            Constants.SeasonEndDate = thisInstance.SeasonEndDate;
            return InvokeSettingsChanged();
        }

        public static bool InvokeSettingsChanged()
        {
            OnSettingsChanged?.Invoke(null, EventArgs.Empty);
            return true;
        }
    }
}