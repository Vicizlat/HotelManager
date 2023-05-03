using System.Collections.Generic;
using System.Linq;
using HotelManager.Data.Models;
using HotelManager.Handlers;

namespace HotelManager.Controller
{
    public static class JsonImport
    {
        public static void ImportBuildings(MainController controller, string importFilePath)
        {
            IEnumerable<Building> buildingsImport = JsonHandler.GetFromFile<Building>(importFilePath);
            foreach (Building building in buildingsImport)
            {
                if (controller.Context.Buildings.Any(b => b.Id == building.Id)) continue;
                controller.Context.Buildings.Add(new Building { BuildingName = building.BuildingName });
                controller.Context.SaveChanges();
            }
        }

        public static void ImportFloors(MainController controller, string importFilePath)
        {
            IEnumerable<Floor> floorsImport = JsonHandler.GetFromFile<Floor>(importFilePath);
            foreach (Floor floor in floorsImport.OrderBy(f => f.Building.BuildingName).ThenBy(f => f.FloorNumber))
            {
                if (controller.Context.Floors.Any(f => f.Id == floor.Id)) continue;
                controller.Context.Floors.Add(new Floor
                {
                    FloorNumber = floor.FloorNumber,
                    Building = controller.Context.Buildings.FirstOrDefault(b => b.Id == floor.Building.Id)
                });
                controller.Context.SaveChanges();
            }
        }

        public static void ImportRooms(MainController controller, string importFilePath)
        {
            IEnumerable<Room> roomsImport = JsonHandler.GetFromFile<Room>(importFilePath);
            foreach (Room room in roomsImport.OrderBy(r => r.Floor.FloorNumber).ThenBy(r => r.RoomNumber))
            {
                if (controller.Context.Rooms.Any(r => r.Id == room.Id)) continue;
                controller.Context.Rooms.Add(new Room
                {
                    Floor = controller.Context.Floors.FirstOrDefault(f => f.Id == room.Floor.Id),
                    RoomNumber = room.RoomNumber,
                    MaxGuests = room.MaxGuests,
                    FirstOnFloor = room.FirstOnFloor,
                    LastOnFloor = room.LastOnFloor,
                    RoomType = room.RoomType,
                    RoomTypeShort = room.RoomTypeShort,
                    Notes = room.Notes,
                    FullRoomNumber = room.FullRoomNumber
                });
                controller.Context.SaveChanges();
            }
        }

        public static void ImportGuests(MainController controller, string importFilePath)
        {
            IEnumerable<Guest> guestsImport = JsonHandler.GetFromFile<Guest>(importFilePath);
            foreach (Guest guest in guestsImport)
            {
                if (controller.Context.Guests.Any(g => g.Id == guest.Id)) continue;
                controller.Context.Guests.Add(new Guest
                {
                    FirstName = guest.FirstName,
                    LastName = guest.LastName,
                    Phone = guest.Phone,
                    Email = guest.Email,
                    GuestReferrer = controller.Context.Guests.FirstOrDefault(g => g.Id == guest.GuestReferrerId)
                });
                controller.Context.SaveChanges();
            }
        }

        public static void ImportReservations(MainController controller, string importFilePath)
        {
            IEnumerable<Reservation> reservations = JsonHandler.GetFromFile<Reservation>(importFilePath);
            foreach (Reservation reservation in reservations)
            {
                if (controller.Context.Reservations.Any(r => r.Id == reservation.Id)) continue;
                controller.Context.Reservations.Add(new Reservation
                {
                    State = reservation.State,
                    Source = reservation.Source,
                    Room = controller.Context.Rooms.FirstOrDefault(r => r.Id == reservation.Room.Id),
                    Guest = controller.Context.Guests.FirstOrDefault(g => g.Id == reservation.Guest.Id),
                    NumberOfGuests = reservation.NumberOfGuests,
                    StartDate = reservation.StartDate,
                    EndDate = reservation.EndDate,
                    TotalSum = reservation.TotalSum,
                    Notes = reservation.Notes,
                    DateCreated = reservation.DateCreated,
                    DateModified = reservation.DateModified
                });
                controller.Context.SaveChanges();
            }
        }

        public static void ImportTransactions(MainController controller, string importFilePath)
        {
            IEnumerable<Transaction> transactionsImport = JsonHandler.GetFromFile<Transaction>(importFilePath);
            foreach (Transaction transaction in transactionsImport)
            {
                if (controller.Context.Transactions.Any(t => t.Id == transaction.Id)) continue;
                controller.Context.Transactions.Add(new Transaction
                {
                    Guest = controller.Context.Guests.FirstOrDefault(g => g.Id == transaction.Guest.Id),
                    Reservation = controller.Context.Reservations.FirstOrDefault(r => r.Id == transaction.Reservation.Id),
                    PaidSum = transaction.PaidSum,
                    PaymentDate = transaction.PaymentDate,
                    PaymentMethod = transaction.PaymentMethod
                });
                controller.Context.SaveChanges();
            }
        }

        public static void ImportPriceRanges(MainController controller, string importFilePath)
        {
            IEnumerable<PriceRange> priceRangesImport = JsonHandler.GetFromFile<PriceRange>(importFilePath);
            foreach (PriceRange priceRange in priceRangesImport)
            {
                if (controller.Context.PriceRanges.Any(pr => pr.Id == priceRange.Id)) continue;
                controller.Context.PriceRanges.Add(new PriceRange
                {
                    StartDate = priceRange.StartDate,
                    EndDate = priceRange.EndDate,
                    BasePrice = priceRange.BasePrice,
                    BasePriceGuests = priceRange.BasePriceGuests,
                    PriceChangePerGuest = priceRange.PriceChangePerGuest,
                    IsActive = priceRange.IsActive
                });
                controller.Context.SaveChanges();
            }
        }
    }
}