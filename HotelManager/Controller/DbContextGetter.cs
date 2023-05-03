using HotelManager.Data;
using HotelManager.Utils;

namespace HotelManager.Controller
{
    public class DbContextGetter
    {
        public static HotelManagerContext GetContext()
        {
            string server = $"Server={Settings.Instance.Server};";
            string port = $"Port={Settings.Instance.Port};";
            string database = $"Database={Settings.Instance.Database};";
            string user = $"User={Settings.Instance.Username};";
            string password = $"Password={Settings.Instance.Password};";
            string connectionString = $"{server}{port}{database}{user}{password}";
            return new HotelManagerContext(connectionString);
        }
    }
}