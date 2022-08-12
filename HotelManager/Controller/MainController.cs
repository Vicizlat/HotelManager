using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using HotelManager.Handlers;
using HotelManager.Data;
using HotelManager.Data.Models;
using HotelManager.Data.Models.Enums;
using HotelManager.Models;
using HotelManager.Utils;
using HotelManager.Views;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HotelManager.Controller
{
    public class MainController
    {
        public event EventHandler OnReservationsChanged;
        public event EventHandler OnRoomsChanged;
        public event EventHandler<DateTime> OnReservationAdd;
        public event EventHandler<int> OnReservationEdit;

        public WaitWindow WaitWindow { get; set; }
        public HotelManagerContext Context { get; set; }
        public List<ReservationInfo> ReservationInfos { get; set; }
        public List<GuestInfo> GuestInfos { get; set; }
        public static List<RoomInfo> RoomInfos { get; set; }
        public bool ChangesMade = false;
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
            RoomInfos = GetRoomInfos();
            return true;
        }

        private void ResetDatabase()
        {
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            string messageText = "Database has been reset successfully! Do you want to import from backups?";
            MessageBoxResult result = MessageBox.Show(messageText, "Database reset", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                string title = "Load backup file for ";
                if (!FileHandler.TryGetOpenFilePath(".json", title + Constants.ImportExportSources[0], out string buildingsPath)) return;
                ImportBuildings(buildingsPath);
                if (!FileHandler.TryGetOpenFilePath(".json", title + Constants.ImportExportSources[1], out string floorsPath)) return;
                ImportFloors(floorsPath);
                if (!FileHandler.TryGetOpenFilePath(".json", title + Constants.ImportExportSources[2], out string roomsPath)) return;
                ImportRooms(roomsPath);
                if (!FileHandler.TryGetOpenFilePath(".json", title + Constants.ImportExportSources[3], out string guestsPath)) return;
                ImportGuests(guestsPath);
                if (!FileHandler.TryGetOpenFilePath(".json", title + Constants.ImportExportSources[4], out string reservationsPath)) return;
                ImportReservations(reservationsPath);
                if (!FileHandler.TryGetOpenFilePath(".json", title + Constants.ImportExportSources[5], out string transactionsPath)) return;
                ImportTransactions(transactionsPath);
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

        public void SaveGuest(GuestInfo guestInfo)
        {
            Guest guest = GetGuest(guestInfo.Id);
            if (guest == null)
            {
                Logging.Instance.WriteLine("Add guest:");
                guest = guestInfo.ToGuest(this);
                Context.Guests.Add(guest);
            }
            else
            {
                Logging.Instance.WriteLine("Edit guest:");
                Logging.Instance.WriteLine($"Old: {guest}", true);
                UpdateGuest(guest, guestInfo);
            }
            Logging.Instance.WriteLine($"New: {guest}", true);
            Context.SaveChanges();
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
                UpdateReservation(reservation, resInfo);
            }
            Logging.Instance.WriteLine($"New: {reservation}", true);
            Context.SaveChanges();
            OnReservationsChanged?.Invoke(this, EventArgs.Empty);
        }

        public Guest GetGuest(int id)
        {
            return Context.Guests.Include(g => g.Reservations).FirstOrDefault(g => g.Id == id);
        }

        public Guest GetGuest(string firstName, string lastName, string phone, string email)
        {
            if (string.IsNullOrEmpty(phone)) phone = null;
            if (string.IsNullOrEmpty(email)) email = null;
            return Context.Guests
                .Where(g => g.Phone == phone)
                .Where(g => g.Email == email)
                .FirstOrDefault(g => g.FirstName == firstName && g.LastName == lastName);
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

        public void UpdateGuest(Guest guest, GuestInfo guestInfo)
        {
            guest.FirstName = guestInfo.FirstName;
            guest.LastName = guestInfo.LastName;
            guest.Phone = guestInfo.Phone;
            guest.Email = guestInfo.Email;
        }

        public void UpdateReservation(Reservation reservation, ReservationInfo resInfo)
        {
            reservation.LastVersionJson = JsonHandler.SerializeToJson(reservation);
            reservation.State = (State)resInfo.StateInt;
            reservation.Source = (Source)resInfo.SourceInt;
            reservation.Room = Context.Rooms.FirstOrDefault(r => r.FullRoomNumber == resInfo.Room);
            if (GetGuestInfo(resInfo.Guest.Id) == null)
            {
                SaveGuest(resInfo.Guest);
            }
            reservation.Guest = resInfo.Guest.ToGuest(this);
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

        public GuestInfo GetGuestInfo(int id)
        {
            Guest guest = GetGuest(id);
            return guest == null ? null : new GuestInfo(guest);
        }

        public GuestInfo GetGuestInfo(string name, string phone, string email)
        {
            string[] guestNames = name.Split();
            string guestPhone = string.IsNullOrEmpty(phone) ? null : phone;
            string guestEmail = string.IsNullOrEmpty(email) ? null : email;
            Guest guest = GetGuest(guestNames[0], string.Join(" ", guestNames.Skip(1)), guestPhone, guestEmail);
            return guest == null ? new GuestInfo(name, phone, email) : new GuestInfo(guest);
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

        private List<RoomInfo> GetRoomInfos()
        {
            List<RoomInfo> roomInfos = new List<RoomInfo> { new RoomInfo() };
            roomInfos.AddRange(Context.Rooms.Select(r => new RoomInfo(r)));
            return roomInfos;
        }

        public async Task<List<ReservationInfo>> GetAllReservationInfos(bool includeCanceled)
        {
            return await Context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .Include(r => r.Transactions)
                .Where(r => includeCanceled || r.State != State.Canceled)
                .Select(x => new ReservationInfo(x)).ToListAsync();
        }

        public List<ReservationInfo> GetReservationInfos(DateTime startDate, DateTime endDate)
        {
            return Context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Guest)
                .Include(r => r.Guest.Reservations)
                .Include(r => r.Transactions)
                .Where(r => r.EndDate >= startDate && r.StartDate <= endDate)
                .Where(r => r.State != State.Canceled)
                .Select(x => new ReservationInfo(x)).ToList();
        }

        public List<ReservationInfo> GetReservationInfos(GuestInfo guest)
        {
            return GetGuest(guest.Id).Reservations.Select(x => new ReservationInfo(x)).ToList();
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

        public decimal GetPriceForDate(DateTime date, int guests)
        {
            if (guests < 2) guests = 2;
            PriceRange pr = Context.PriceRanges.Where(pr => pr.IsActive).Where(pr => date >= pr.StartDate).FirstOrDefault(pr => date <= pr.EndDate);
            return pr?.BasePrice + pr?.PriceChangePerGuest * (guests - pr?.BasePriceGuests) ?? 0;
        }

        public bool ValidatePriceRangeById(int id)
        {
            PriceRange priceRange = Context.PriceRanges.FirstOrDefault(pr => pr.Id == id);
            return !ValidatePriceRangeByDate(priceRange.StartDate) && !ValidatePriceRangeByDate(priceRange.EndDate);
        }

        public void ShowWaitWindow(string text = null)
        {
            WaitWindow = new WaitWindow(text);
            WaitWindow.Show();
        }

        public void CloseWaitWindow()
        {
            if (WaitWindow != null) WaitWindow.Close();
        }
    }
}