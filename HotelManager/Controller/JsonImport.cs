using System;
using System.Collections.Generic;
using System.Linq;
using HotelManager.Data.Models;
using HotelManager.Handlers;
using Reservation = HotelManager.Models.Reservation;

namespace HotelManager.Controller
{
    public static class JsonImport
    {
        public static List<Reservation> ImportReservations(MainController controller, string importFilePath)
        {
            List<Reservation> reservations = JsonHandler.GetFromFile<Reservation>(importFilePath).ToList();
            foreach (Reservation reservation in reservations)
            {
                if (controller.Context.Reservations.Any(r => r.Id == reservation.Id)) continue;
                string[] guestNames = reservation.GuestName.Split();
                string guestLastName = string.Join(" ", guestNames.Skip(1));
                Guest guest = controller.Context.Guests
                                  .Where(g => g.FirstName == guestNames[0])
                                  .FirstOrDefault(g => g.LastName == guestLastName)
                              ?? controller.AddNewGuest(guestNames[0], guestLastName);
                Data.Models.Reservation res = new Data.Models.Reservation
                {
                    State = reservation.State,
                    Source = reservation.Source,
                    Room = controller.Context.Rooms.FirstOrDefault(r => r.FullRoomNumber == reservation.Room),
                    Guest = guest,
                    StartDate = reservation.Period.StartDate,
                    EndDate = reservation.Period.EndDate,
                    TotalSum = reservation.Sums.Total,
                    NumberOfGuests = reservation.GuestsInRoom,
                    Notes = reservation.Notes,
                    DateCreated = DateTime.Today
                };
                controller.Context.Reservations.Add(res);
                if (reservation.Sums.Paid != 0)
                {
                    controller.AddNewTransaction(guest, res, reservation.Sums.Paid, res.DateCreated, "Неизвестен");
                }
                controller.Context.SaveChanges();
            }
            return reservations;
        }

        public static void ImportBuildings(MainController controller, string importFilePath)
        {
            IEnumerable<Building> buildingsImport = JsonHandler.GetFromFile<Building>(importFilePath);
            foreach (Building building in buildingsImport)
            {
                if (controller.Context.Buildings.Any(b => b.Id == building.Id)) continue;
                controller.Context.Buildings.Add(new Building {BuildingName = building.BuildingName});
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
                    Notes = room.Notes,
                    FullRoomNumber = room.FullRoomNumber
                });
                controller.Context.SaveChanges();
            }
        }
    }
}