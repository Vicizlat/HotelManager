using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManager.Data.Models
{
    public class Building
    {
        public int Id { get; set; }
        [Required]
        public string BuildingName { get; set; }
        [JsonIgnore]
        public ICollection<Floor> Floors { get; set; }

        public Building()
        {
            Floors = new HashSet<Floor>();
        }

        public override string ToString()
        {
            return $"Сграда {BuildingName} - {Floors.Count} етажа";
        }
    }
}