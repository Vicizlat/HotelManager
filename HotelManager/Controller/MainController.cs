using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HotelManager.Handlers;
using HotelManager.Utils;
using HotelManager.Data;
using HotelManager.Data.Models;
using HotelManager.Data.Models.Enums;
using HotelManager.Models;
using HotelManager.Views;
using HotelManager.Views.Templates;
using Microsoft.EntityFrameworkCore;
using Reservation = HotelManager.Data.Models.Reservation;

namespace HotelManager.Controller
{
    public class MainController
    {
        public event EventHandler OnReservationsChanged;
        public event EventHandler OnRoomsChanged;
        public event EventHandler<DateTime> OnReservationAdd;
        public event EventHandler<int> OnReservationEdit;
        public HotelManagerContext Context { get; set; }
        public List<Models.Reservation> Reservations { get; set; }
        public List<ReservationInfo> ReservationInfos { get; set; }
        public static List<string> RoomsList = new List<string> { Constants.NoRoomSelected };
        private readonly bool resetDb = false;

        public bool Initialize()
        {
            //bool localConnection = MessageBox.Show("Use local connection?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            //Context = DbContextGetter.GetContext(localConnection: localConnection);
            Context = DbContextGetter.GetContext(localConnection: false);
            if (!Context.Database.CanConnect()) return false;
            if (resetDb) ResetDatabase();
            if (!Context.Buildings.Any() || !Context.Floors.Any() || !Context.Rooms.Any())
            {
                new HotelSetupWindow(this).ShowDialog();
            }
            //RoomsArray = new List<string> { Constants.NoRoomSelected };
            RoomsList.AddRange(Context.Rooms.ToList().Select(r => r.ToString()));

            //WaitWindow waitWindow = new WaitWindow("Сваляне на резервации от сървъра...");
            //Reservations = JsonImport.ImportReservations(this, Path.Combine(Constants.LocalPath, Constants.ReservationsFileName));
            //Reservations = GetReservations().ToList();
            //UpdateResInfos(waitWindow);
            //waitWindow.Close();
            return true;
        }

        private void ResetDatabase()
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            JsonImport.ImportBuildings(this, Path.Combine(Constants.LocalPath, "Buildings.json"));
            JsonImport.ImportFloors(this, Path.Combine(Constants.LocalPath, "Floors.json"));
            JsonImport.ImportRooms(this, Path.Combine(Constants.LocalPath, "Rooms.json"));
        }

        private void UpdateResInfos(WaitWindow waitWindow)
        {
            ReservationInfos = new List<ReservationInfo>(Context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .Include(r => r.Transactions)
                .Select(x => new ReservationInfo(x)));
            waitWindow.Close();
        }

        private IEnumerable<Models.Reservation> GetReservations()
        {
            for (int i = 1; i <= Context.Reservations.Count(); i++)
            {
                yield return Context.Reservations.Where(r => r.Id == i).Select(x =>
                new Models.Reservation(
                    x.Id,
                    x.State,
                    x.Source,
                    x.Room.FullRoomNumber,
                    $"{x.Guest.FirstName} {x.Guest.LastName}",
                    new Period(x.StartDate, x.EndDate), x.NumberOfGuests,
                    new Sums(x.TotalSum, x.Transactions.Sum(t => t.PaidSum)),
                    x.Notes
                )).FirstOrDefault();
            }
        }

        public void ImportReservations(string importFilePath)
        {
            //Reservations = JsonImport.ImportReservations(this, importFilePath);
            OnReservationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ImportRooms(string importFilePath)
        {
            JsonImport.ImportRooms(this, importFilePath);
            OnRoomsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void RequestReservationWindow(int id)
        {
            if (id > 0 && id <= Context.Reservations.Count())
            {
                OnReservationEdit?.Invoke(this, id);
            }
        }

        public void RequestReservationWindow(int room, DateTime startDate)
        {
            int? resId = Context.Reservations.Where(r => r.Room.FullRoomNumber == room)
                .Where(r => r.StartDate <= startDate && startDate < r.EndDate)
                .FirstOrDefault(r => r.State != State.Canceled)?.Id;
            if (resId == null)
            {
                OnReservationAdd?.Invoke(room, startDate);
            }
            else RequestReservationWindow(resId.Value);
        }

        public void SaveReservation(ReservationInfo resInfo)
        {
            Guest guest = GetGuest(resInfo);
            Reservation reservation = GetReservation(resInfo.Id);
            if (reservation == null)
            {
                //Reservations.Add(new Models.Reservation(resInfo));
                Logging.Instance.WriteLine("Add reservation:");

                reservation = resInfo.ToReservation(this);
                Context.Reservations.Add(reservation);
            }
            else
            {
                //Reservations[resInfo.Id - 1] = new Models.Reservation(resInfo);
                Logging.Instance.WriteLine("Edit reservation:");
                Logging.Instance.WriteLine($"Old: {reservation}", true);

                UpdateReservation(reservation, guest, resInfo);
            }
            Logging.Instance.WriteLine($"New: {reservation}", true);
            Context.SaveChanges();
            //UpdateResInfos(new WaitWindow("Обновяване на резервации..."));
            OnReservationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public Guest GetGuest(ReservationInfo resInfo)
        {
            Guest[] guests = FindGuestsByNames(resInfo.GuestName, out string guestFirstName, out string guestLastName);
            Guest guest = guests.Length switch
            {
                0 => AddNewGuest(guestFirstName, guestLastName),
                1 => guests[0],
                _ => null
            };
            if (guest == null) return null;
            guest.Email = resInfo.Email;
            guest.Phone = resInfo.Phone;
            if (string.IsNullOrEmpty(resInfo.GuestReferrer)) return guest;
            Guest[] guestRefs = FindGuestsByNames(resInfo.GuestReferrer, out string _, out string _);
            guest.GuestReferrer = guestRefs.Length == 1 ? guestRefs[0] : null;
            return guest;
        }

        public Reservation GetReservation(int id)
        {
            return Context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Transactions)
                .Include(r => r.Room)
                .FirstOrDefault(r => r.Id == id);
        }

        public Reservation GetReservation(int room, DateTime startDate)
        {
            return Context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Transactions)
                .Where(r => r.State != State.Canceled)
                .Where(r => r.StartDate <= startDate)
                .Where(r => startDate < r.EndDate)
                .FirstOrDefault(r => r.Room.FullRoomNumber == room);
        }

        //TODO:Handle guests with the same name!
        public Guest[] FindGuestsByNames(string guestName, out string guestFirstName, out string guestLastName)
        {
            string[] guestNames = guestName.Split();
            guestFirstName = guestNames[0];
            guestLastName = string.Join(" ", guestNames.Skip(1));
            string firstName = guestFirstName;
            string lastName = guestLastName;
            Guest[] guests = Context.Guests
                .Where(g => g.FirstName == firstName)
                .Where(g => g.LastName == lastName).ToArray();
            return guests;
        }

        public Guest AddNewGuest(string guestFirstName, string guestLastName)
        {
            Guest guest = new Guest
            {
                FirstName = guestFirstName,
                LastName = guestLastName
            };
            Context.Guests.Add(guest);
            return guest;
        }

        public void UpdateReservation(Reservation reservation, Guest guest, ReservationInfo resInfo)
        {
            reservation.LastVersionJson = JsonHandler.SerializeToJson(reservation);
            reservation.State = (State)resInfo.StateInt;
            reservation.Source = (Source)resInfo.SourceInt;
            reservation.Room = Context.Rooms.FirstOrDefault(r => r.FullRoomNumber == resInfo.Room);
            reservation.Guest = guest;
            reservation.StartDate = resInfo.StartDate;
            reservation.EndDate = resInfo.EndDate;
            reservation.TotalSum = resInfo.TotalSum;
            reservation.NumberOfGuests = resInfo.NumberOfGuests;
            reservation.Notes = resInfo.Notes;
            reservation.DateModified = DateTime.Now;
        }

        public void AddNewTransaction(Guest guest, Reservation reservation, decimal paidSum, DateTime transDate, string tarnsMethod)
        {
            reservation.Transactions.Add(new Transaction
            {
                Guest = guest,
                Reservation = reservation,
                PaidSum = paidSum - reservation.Transactions.Sum(t => t.PaidSum),
                PaymentDate = transDate,
                PaymentMethod = tarnsMethod
            });
        }

        public void CheckInReservation(int room, DateTime startDate)
        {
            Reservation reservation = Context.Reservations
                .Include(r => r.Guest).Include(r => r.Transactions)
                .Where(r => r.Room.FullRoomNumber == room)
                .Where(r => r.StartDate <= startDate)
                .Where(r => startDate < r.EndDate)
                .FirstOrDefault(r => r.State != State.Canceled);
            if (reservation == null) return;
            Logging.Instance.WriteLine("Edit reservation:");
            Logging.Instance.WriteLine($"Old: {reservation}", true);
            //Reservations[reservation.Id - 1].State = State.CheckedIn;
            reservation.LastVersionJson = JsonHandler.SerializeToJson(reservation);
            reservation.State = State.CheckedIn;
            reservation.DateModified = DateTime.Now;
            Logging.Instance.WriteLine($"New: {reservation}", true);
            Context.SaveChanges();
            //UpdateResInfos(new WaitWindow("Обновяване на резервации..."));
            OnReservationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public ReservationInfo GetReservationInfo(int id)
        {
            Reservation reservation = GetReservation(id);
            return reservation == null ? null : new ReservationInfo(reservation);
        }

        public ReservationInfo GetReservationInfo(int room, DateTime date)
        {
            Reservation reservation = GetReservation(room, date);
            return reservation == null ? null : new ReservationInfo(reservation);
        }

        public List<ReservationInfo> GetReservationInfos(DateTime startDate, DateTime endDate)
        {
            return new List<ReservationInfo>(Context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .Include(r => r.Transactions)
                .Where(r => r.EndDate >= startDate && r.StartDate <= endDate)
                .Select(x => new ReservationInfo(x)));
        }

        public DateTime NextReservationStartDate(int room, DateTime startDate)
        {
            return Context.Reservations
                       .Where(r => r.Room.FullRoomNumber == room)
                       .Where(r => r.StartDate >= startDate.AddDays(1))
                       .OrderBy(r => r.StartDate)
                       .FirstOrDefault(r => r.State != State.Canceled)?.StartDate
                   ?? Settings.Instance.SeasonEndDate;
        }

        public Dictionary<int, bool> GetRoomsDictionary()
        {
            return Context.Rooms
                .OrderByDescending(r => r.Floor.FloorNumber)
                .ThenBy(r => r.RoomNumber)
                .Select(x => new { x.FullRoomNumber, x.LastOnFloor })
                .ToDictionary(key => key.FullRoomNumber, value => value.LastOnFloor);
        }
    }
}