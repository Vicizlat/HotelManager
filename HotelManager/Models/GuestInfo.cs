using System;
using System.Linq;
using System.Text;
using HotelManager.Controller;
using HotelManager.Data.Models;

namespace HotelManager.Models
{
    public class GuestInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        //public int? ReferrerId { get; set; }
        //public string Referrer { get; set; }
        public int ResCount { get; set; }

        public GuestInfo(string name, string phone, string email)
        {
            string[] guestNames = name.Split();
            FirstName = guestNames[0];
            LastName = string.Join(" ", guestNames.Skip(1));
            Phone = string.IsNullOrEmpty(phone) ? null : phone;
            Email = string.IsNullOrEmpty(email) ? null : email;
        }

        public GuestInfo(Guest guest)
        {
            Id = guest.Id;
            FirstName = guest.FirstName;
            LastName = guest.LastName;
            Phone = guest.Phone;
            Email = guest.Email;
            //ReferrerId = guest.GuestReferrerId;
            //if (Referrer != null) Referrer = $"{guest.GuestReferrer.FirstName} {guest.GuestReferrer.LastName}";
            ResCount = guest.Reservations.Count;
        }

        public Guest ToGuest(MainController controller)
        {
            Guest guest = controller.GetGuest(FirstName, LastName, Phone, Email) ?? new Guest();
            controller.UpdateGuest(guest, this);
            return guest;
        }

        public string GetFullName() => $"{FirstName} {LastName}";

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine($"Име: {FirstName} {LastName}")
                .AppendLine($"Телефон: {Phone}")
                .AppendLine($"Имейл: {Email}")
                .AppendLine($"Брой резервации: {ResCount}");
            //if (Referrer != null) sb.AppendLine($"Препоръчан от: {Referrer}");
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            GuestInfo other = (GuestInfo)obj;
            return other != null
                   && Id == other.Id
                   && FirstName == other.FirstName
                   && LastName == other.LastName
                   && Phone == other.Phone
                   && Email == other.Email;
            //&& ReferrerId == other.ReferrerId
            //&& Referrer == other.Referrer
            //&& ResCount == other.ResCount;
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(Id);
            hashCode.Add(FirstName);
            hashCode.Add(LastName);
            hashCode.Add(Phone);
            hashCode.Add(Email);
            //hashCode.Add(Referrer);
            //hashCode.Add(ResCount);
            return hashCode.ToHashCode();
        }
    }
}