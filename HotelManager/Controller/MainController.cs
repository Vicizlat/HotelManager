using System;
using System.Collections.Generic;
using System.Linq;
using HotelManager.Handlers;
using HotelManager.Utils;
using HotelManager.Data;
using HotelManager.Data.Models;
using HotelManager.Data.Models.Enums;
using HotelManager.Views;
using HotelManager.Views.Templates;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using Microsoft.Win32;
using System.IO;

namespace HotelManager.Controller
{
    public class MainController
    {
        public event EventHandler OnReservationsChanged;
        public event EventHandler OnRoomsChanged;
        public event EventHandler<DateTime> OnReservationAdd;
        public event EventHandler<int> OnReservationEdit;
        public HotelManagerContext Context { get; set; }
        public List<ReservationInfo> ReservationInfos { get; set; }
        public List<GuestInfo> GuestInfos { get; set; }
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
            RoomsList.AddRange(Context.Rooms.ToList().Select(r => r.ToString()));
            return true;
        }

        private void ResetDatabase()
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            MessageBox.Show("Database has been reset successfully!", "Database reset successful!", MessageBoxButton.OK, MessageBoxImage.Information);
            var result = MessageBox.Show("Do you want to import from backups?", "Import backup?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                List<string> filePaths = new List<string>();
                foreach (string backup in Constants.ImportExportSources)
                {
                    OpenFileDialog dlg = new OpenFileDialog
                    {
                        Title = $"Load backup file for {backup}",
                        DefaultExt = ".json",
                        Filter = "JSON Files (*.json)|*.json"
                    };
                    if (dlg.ShowDialog() == true) filePaths.Add(dlg.FileName);
                }
                ImportBuildings(filePaths[0]);
                ImportFloors(filePaths[1]);
                ImportRooms(filePaths[2]);
                ImportGuests(filePaths[3]);
                ImportReservations(filePaths[4]);
                ImportTransactions(filePaths[5]);
                //JsonImport.ImportOldReservations(this, Path.Combine(Constants.LocalPath, "oldReservations.json"));
            }
        }

        public void ImportBuildings(string importFilePath)
        {
            JsonImport.ImportBuildings(this, importFilePath);
            OnRoomsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ImportFloors(string importFilePath)
        {
            JsonImport.ImportFloors(this, importFilePath);
            OnRoomsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ImportRooms(string importFilePath)
        {
            JsonImport.ImportRooms(this, importFilePath);
            OnRoomsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ImportGuests(string importFilePath)
        {
            JsonImport.ImportGuests(this, importFilePath);
            OnReservationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ImportReservations(string importFilePath)
        {
            JsonImport.ImportReservations(this, importFilePath);
            OnReservationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ImportTransactions(string importFilePath)
        {
            JsonImport.ImportTransactions(this, importFilePath);
            OnReservationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ExportCollectionToJson(string filePath, IEnumerable<object> collection, string collectionName, bool showSuccessMessage = true)
        {
            if (FileHandler.WriteAllLines(filePath, JsonHandler.GetJsonStrings(collection)))
            {
                string messageText = $"Successfully exported {collectionName} to \"{filePath}\"!";
                Logging.Instance.WriteLine(messageText);
                if (showSuccessMessage) MessageBox.Show(messageText, "Export success!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string messageText = $"Failed to export {collectionName} to \"{filePath}\"!";
                Logging.Instance.WriteLine(messageText);
                MessageBox.Show(messageText, "Export fail!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public IEnumerable<object> GetCollectionByName(string name)
        {
            switch (name)
            {
                case "Buildings":
                    return Context.Buildings.OrderBy(b => b.Id);
                case "Floors":
                    return Context.Floors.Include(f => f.Building).OrderBy(f => f.Id);
                case "Rooms":
                    return Context.Rooms.Include(r => r.Floor.Building).OrderBy(r => r.Id);
                case "Guests":
                    return Context.Guests.OrderBy(g => g.Id);
                case "Reservations":
                    return Context.Reservations.Include(r => r.Room.Floor.Building).Include(r => r.Guest).OrderBy(r => r.Id);
                case "Transactions":
                    return Context.Transactions.Include(t => t.Guest).Include(t => t.Reservation).OrderBy(t => t.Id);
                default:
                    return new List<object>();
            }
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
            Reservation reservation = GetReservation(resInfo.Id);
            if (reservation == null)
            {
                Logging.Instance.WriteLine("Add reservation:");
                reservation = resInfo.ToReservation(this);
                Context.Reservations.Add(reservation);
            }
            else
            {
                Logging.Instance.WriteLine("Edit reservation:");
                Logging.Instance.WriteLine($"Old: {reservation}", true);
                UpdateReservation(reservation, resInfo.Guest.ToGuest(this), resInfo);
            }
            Logging.Instance.WriteLine($"New: {reservation}", true);
            Context.SaveChanges();
            OnReservationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public Guest GetGuest(int id)
        {
            return Context.Guests.Include(g => g.GuestReferrer).FirstOrDefault(g => g.Id == id);
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

        public Guest FindGuest(string name, string phone, string email)
        {
            string[] guestNames = name.Split();
            string firstName = guestNames[0];
            string lastName = string.Join(" ", guestNames.Skip(1));
            if (string.IsNullOrEmpty(phone)) phone = null;
            if (string.IsNullOrEmpty(email)) email = null;
            return Context.Guests
                .Include(g => g.GuestReferrer)
                .Where(g => g.Phone == phone)
                .Where(g => g.Email == email)
                .FirstOrDefault(g => g.FirstName == firstName && g.LastName == lastName);
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
            reservation.LastVersionJson = JsonHandler.SerializeToJson(reservation);
            reservation.State = State.CheckedIn;
            reservation.DateModified = DateTime.Now;
            Logging.Instance.WriteLine($"New: {reservation}", true);
            Context.SaveChanges();
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

        public void AddPriceRange(DateTime startDate, DateTime endDate, decimal basePrice, int baseGuests, decimal priceChange)
        {
            Context.PriceRanges.Add(new PriceRange
            {
                StartDate = startDate,
                EndDate = endDate,
                BasePrice = basePrice,
                BasePriceGuests = baseGuests,
                PriceChangePerGuest = priceChange,
                IsActive = true
            });
            Context.SaveChanges();
        }

        public void UpdatePriceRangeState(int id, bool newState)
        {
            Context.PriceRanges.FirstOrDefault(pr => pr.Id == id).IsActive = newState;
            Context.SaveChanges();
        }

        public List<string> GetPriceRangeStrings()
        {
            return Context.PriceRanges.Select(pr => pr.ToString()).ToList();
        }

        public bool GetPriceRangeState(int id)
        {
            return Context.PriceRanges.FirstOrDefault(pr => pr.Id == id).IsActive;
        }

        public bool ValidatePriceRangeByDate(DateTime date)
        {
            return Context.PriceRanges.Where(pr => pr.IsActive).Any(pr => pr.StartDate <= date && date <= pr.EndDate);
        }

        public decimal[] GetBasePriceForDate(DateTime date, out int baseGuests, out DateTime[] priceRangeDates)
        {
            PriceRange pr = Context.PriceRanges.Where(pr => pr.IsActive).FirstOrDefault(pr => pr.StartDate <= date && date <= pr.EndDate);
            if (pr == null)
            {
                baseGuests = 0;
                priceRangeDates = new DateTime[] { date, date };
                return new decimal[] { 0, 0 };
            }
            baseGuests = pr.BasePriceGuests;
            priceRangeDates = new DateTime[] { pr.StartDate, pr.EndDate };
            return new decimal[] { pr.BasePrice, pr.PriceChangePerGuest };
        }

        public bool ValidatePriceRangeById(int id)
        {
            PriceRange priceRange = Context.PriceRanges.FirstOrDefault(pr => pr.Id == id);
            return !ValidatePriceRangeByDate(priceRange.StartDate) && !ValidatePriceRangeByDate(priceRange.EndDate);
        }
    }
}