using HotelManager.Data;
using HotelManager.Utils;

namespace HotelManager.Controller
{
    public class DbContextGetter
    {
        public static HotelManagerContext GetContext(bool localConnection)
        {
            if (localConnection) return new HotelManagerContext(Constants.LocalConnection);
            //string mySqlConnection = "server=zlpatehomeserver.myqnapcloud.com;port=3306;";
            string server = $"Server={Settings.Instance.Server};";
            //string server = $"server={Settings.Instance.Server};port=3306;";
            string database = $"Database={Settings.Instance.Database};";
            string user = $"User={Settings.Instance.UserName};";
            string password = $"Password={Settings.Instance.Password};";
            string connectionString = $"{server}{database}{user}{password}";
            return new HotelManagerContext(connectionString);
        }
    }
}