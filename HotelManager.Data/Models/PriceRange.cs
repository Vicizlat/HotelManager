using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManager.Data.Models
{
    public class PriceRange
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal BasePrice { get; set; }
        public int BasePriceGuests { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal PriceChangePerGuest { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public ICollection<Room> Rooms { get; set; }

        public PriceRange()
        {
            Rooms = new HashSet<Room>();
        }

        public override string ToString()
        {
            return $"{Id} | {StartDate:dd.MM.yyyy} | {EndDate:dd.MM.yyyy} | {BasePrice} | {BasePriceGuests} | {PriceChangePerGuest} | {IsActive}";
        }
    }
}