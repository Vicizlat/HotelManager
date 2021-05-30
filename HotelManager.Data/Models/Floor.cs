using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HotelManager.Data.Models
{
    public class Floor
    {
        public int Id { get; set; }
        public int FloorNumber { get; set; }
        [JsonIgnore]
        public int BuildingId { get; set; }
        public Building Building { get; set; }
        [JsonIgnore]
        public ICollection<Room> Rooms { get; set; }

        public Floor()
        {
            Rooms = new HashSet<Room>();
        }

        public override string ToString()
        {
            return $"Етаж {FloorNumber} от Сграда {Building.BuildingName} - {Rooms.Count} стаи";
        }
    }
}