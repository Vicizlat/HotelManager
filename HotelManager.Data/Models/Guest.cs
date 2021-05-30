using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelManager.Data.Models
{
    public class Guest
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }
        [MaxLength(20)]
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [JsonIgnore]
        public int? GuestReferrerId { get; set; }
        public Guest GuestReferrer { get; set; }
        [JsonIgnore]
        public ICollection<Reservation> Reservations { get; set; }
        [JsonIgnore]
        public ICollection<Transaction> Transactions { get; set; }
        [JsonIgnore]
        public ICollection<Guest> GuestReferrals { get; set; }

        public Guest()
        {
            Transactions = new HashSet<Transaction>();
            Reservations = new HashSet<Reservation>();
        }

        public Guest(string firstName, string lastName, string phone, string email) : this()
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            Email = email;
        }

        public override string ToString()
        {
            return $"Име: {FirstName} {LastName} | Телефон: {Phone} | Имейл: {Email}";
        }
    }
}