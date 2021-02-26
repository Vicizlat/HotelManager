using System.Collections.Generic;

namespace HotelManager.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int Floor { get; set; }
        public int Number { get; set; }
        public string RoomType { get; set; }
        public bool FirstOnFloor { get; set; }
        public bool LastOnFloor { get; set; }
        public int FullRoomNumber => int.Parse($"{Floor}{Number}");
        public IEnumerable<Reservation> Reservations { get; set; }

        public Room()
        {
            
        }

        public Room(int id, int floor, int number, string roomType, bool firstOnFloor, bool lastOnFloor)
        {
            Id = id;
            Floor = floor;
            Number = number;
            RoomType = roomType;
            FirstOnFloor = firstOnFloor;
            LastOnFloor = lastOnFloor;
        }

        public override string ToString()
        {
            return $"{RoomType}{(Number == 0 ? string.Empty : $" {FullRoomNumber}")}";
        }
    }
}