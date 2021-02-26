using System.Collections.Generic;
using System.Linq;
using HotelManager.Models;
using HotelManager.Utils;

namespace HotelManager.Repositories
{
    public class Rooms : List<Room>
    {
        public int RealCount => Count - 1;
        public List<Room> TopFloorFirst { get; set; }

        //TODO:
        //Set up for selectable floors and rooms per floor.
        public Rooms(int floors, int roomsPerFloor, int startFloor = 0)
        {
            Add(new Room(Count, -1, 0, Constants.NoRoomSelected, false, false));
            for (int i = startFloor; i <= floors + (startFloor - 1); i++)
            {
                for (int j = 1; j <= roomsPerFloor; j++)
                {
                    Add(new Room(Count, i, j, "Стая", j == 1, j == roomsPerFloor));
                }
            }
            Add(new Room(Count, 3, 2, "Апартамент", true, true));
            TopFloorFirst = this.OrderByDescending(r => r.Floor).ThenBy(r => r.Number).ToList();
        }

        public int GetRoomIndex(int fullRoomNumber)
        {
            return FindIndex(r => r.FullRoomNumber == fullRoomNumber);
        }
    }
}