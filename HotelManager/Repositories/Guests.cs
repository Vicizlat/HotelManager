using System.Collections.Generic;
using HotelManager.Models;

namespace HotelManager.Repositories
{
    public class Guests : List<Guest>
    {
        public int RealCount => Count - 1;

        public Guests()
        {
            Add(new Guest("Служебен гост", null, null) { Id = 0 });
        }

        public void AddGuests(Guest guest)
        {
            guest.Id = Count;
            Add(guest);
        }
    }
}