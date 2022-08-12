using HotelManager.Data.Models;
using HotelManager.Utils;

namespace HotelManager.Models
{
    public class RoomInfo
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public int RoomNumber { get; set; }
        public int FullRoomNumber { get; set; }
        public bool FirstOnFloor { get; set; }
        public bool LastOnFloor { get; set; }
        public int ReservationsCount { get; set; }

        public RoomInfo()
        {
            Id = 0;
            DisplayName = Constants.NoRoomSelected;
        }

        public RoomInfo(Room room)
        {
            Id = room.Id;
            RoomNumber = room.RoomNumber;
            FullRoomNumber = room.FullRoomNumber;
            FirstOnFloor = room.FirstOnFloor;
            LastOnFloor = room.LastOnFloor;
            DisplayName = $"{room.RoomTypeShort}{(RoomNumber == 0 ? string.Empty : $" {FullRoomNumber}")}";
            ReservationsCount = room.Reservations.Count;
        }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}