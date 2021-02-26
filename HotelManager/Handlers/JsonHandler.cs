using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotelManager.Models;

namespace HotelManager.Handlers
{
    public static class JsonHandler
    {
        public static IEnumerable<string> ReservationsJsonStrings(IEnumerable<Reservation> reservations)
        {
            foreach (Reservation reservation in reservations)
            {
                if (reservation == null) continue;
                yield return SerializeToJson(reservation);
            }
        }

        public static IEnumerable<Reservation> GetReservationsFromFile(string fileName)
        {
            foreach (string line in FileHandler.ReadFromFile(fileName))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                yield return DeserializeFromJson(line);
            }
        }

        private static string SerializeToJson(Reservation reservation)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter() }
            };
            return JsonSerializer.Serialize(reservation, options);
        }

        private static Reservation DeserializeFromJson(string line)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            return JsonSerializer.Deserialize<Reservation>(line, options);
        }
    }
}