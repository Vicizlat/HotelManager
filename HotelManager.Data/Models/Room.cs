using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HotelManager.Data.Models
{
    public class Room
    {
        public int Id { get; set; }
        [JsonIgnore]
        public int FloorId { get; set; }
        public Floor Floor { get; set; }
        public int RoomNumber { get; set; }
        public string RoomType { get; set; }
        public string RoomTypeShort { get; set; }
        public int MaxGuests { get; set; }
        public bool FirstOnFloor { get; set; }
        public bool LastOnFloor { get; set; }
        public string Notes { get; set; }
        public int FullRoomNumber { get; set; }
        [JsonIgnore]
        public ICollection<Reservation> Reservations { get; set; }

        public Room()
        {
            Reservations = new HashSet<Reservation>();
        }

        public override string ToString()
        {
            return $"{RoomTypeShort}{(RoomNumber == 0 ? string.Empty : $" {FullRoomNumber}")}";
        }
    }
}