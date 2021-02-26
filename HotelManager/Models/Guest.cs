using System.Collections.Generic;

namespace HotelManager.Models
{
    public class Guest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public IEnumerable<Reservation> Reservations { get; set; }

        public Guest() { }

        public Guest(string name, string phone, string email)
        {
            Name = name;
            Phone = phone;
            Email = email;
        }

        public override string ToString()
        {
            return $"Име: {Name} | Телефон: {Phone} | Имейл: {Email}";
        }
    }
}