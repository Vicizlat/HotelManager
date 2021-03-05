using System.Collections.Generic;

namespace HotelManager.Models
{
    public class Guest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<int> ReservationIds { get; set; }

        public Guest()
        {
            ReservationIds = new List<int>();
        }

        public Guest(string name, string phone, string email) : this()
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