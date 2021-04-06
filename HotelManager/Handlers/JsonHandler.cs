using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using HotelManager.Models;

namespace HotelManager.Handlers
{
    public static class JsonHandler
    {
        public static IEnumerable<string> GetJsonStrings(IEnumerable<object> entities)
        {
            foreach (object entity in entities)
            {
                if (entity == null) continue;
                yield return SerializeToJson(entity);
            }
        }

        public static IEnumerable<Reservation> GetReservationsFromFile(string fileName)
        {
            foreach (string line in FileHandler.ReadAllLines(fileName))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                yield return DeserializeFromJson(line);
            }
        }

        private static string SerializeToJson(object entity)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter() }
            };
            return JsonSerializer.Serialize(entity, options);
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