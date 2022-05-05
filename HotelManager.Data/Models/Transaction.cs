using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManager.Data.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int GuestId { get; set; }
        public Guest Guest { get; set; }
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        [Column(TypeName = "decimal(6, 2)")]
        public decimal PaidSum { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string PaymentMethod { get; set; }

        public override string ToString()
        {
            return
                $"{Id} | {ReservationId} | {Guest.FirstName} {Guest.LastName} | {PaymentMethod} | {PaymentDate:dd.MM.yyyy} | {PaidSum}";
        }
    }
}