using System.Collections.Generic;
using System.Linq;
using System.Text;
using HotelManager.Models;
using HotelManager.Utils;

namespace HotelManager.Repositories
{
    public class Guests : HashSet<Guest>
    {
        public int RealCount => Count - 1;

        public void AddGuests(Guest guest)
        {
            guest.Id = Count;
            Add(guest);
        }
    }
}