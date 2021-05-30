using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public static IEnumerable<T> GetFromFile<T>(string fileName)
        {
            foreach (string line in FileHandler.ReadAllLines(fileName))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                yield return DeserializeFromJson<T>(line);
            }
        }

        public static string SerializeToJson(object entity)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                Converters = { new JsonStringEnumConverter() }
            };
            string result = JsonSerializer.Serialize(entity, options);
            return result;
        }

        private static T DeserializeFromJson<T>(string line)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            };
            return JsonSerializer.Deserialize<T>(line, options);
        }
    }
}