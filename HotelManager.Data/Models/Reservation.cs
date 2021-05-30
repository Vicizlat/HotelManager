using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using HotelManager.Data.Models.Enums;

namespace HotelManager.Data.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public State State { get; set; }
        public Source Source { get; set; }
        [JsonIgnore]
        public int RoomId { get; set; }
        public Room Room { get; set; }
        [JsonIgnore]
        public int GuestId { get; set; }
        public Guest Guest { get; set; }
        public int NumberOfGuests { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal TotalSum { get; set; }
        public string Notes { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        [JsonIgnore]
        public string LastVersionJson { get; set; }
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }

        public Reservation()
        {
            DateCreated = DateTime.Now;
            DateModified = DateCreated;
            Transactions = new HashSet<Transaction>();
        }

        public override string ToString()
        {
            string guestName = $"{Guest.FirstName} {Guest.LastName}";
            decimal paidSum = Transactions.Sum(t => t.PaidSum);
            return $"Номер: {Id} | Състояние: {State} | Източник: {Source} | Стая: {Room} | Гост: {guestName} | Период: {StartDate:dd.MM.yyyy}-{EndDate:dd.MM.yyyy} | Гости: {NumberOfGuests} | Суми: {TotalSum} - {paidSum} = {TotalSum - paidSum} | Бележки: {Notes.Replace(Environment.NewLine, " ")}";
        }
    }
}