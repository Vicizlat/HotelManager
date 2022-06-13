using System;
using System.Text;
using HotelManager.Controller;
using HotelManager.Data.Models;

namespace HotelManager.Views.Templates
{
    public class GuestInfo
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int? ReferrerId { get; set; }
        public string Referrer { get; set; }
        public int ResCount { get; set; }

        public GuestInfo() { }

        public GuestInfo(Guest guest)
        {
            Id = guest.Id;
            FirstName = guest.FirstName;
            LastName = guest.LastName;
            Phone = guest.Phone;
            Email = guest.Email;
            ReferrerId = guest.GuestReferrerId;
            if (Referrer != null) Referrer = $"{guest.GuestReferrer.FirstName} {guest.GuestReferrer.LastName}";
            ResCount = guest.Reservations.Count;
        }

        public Guest ToGuest(MainController controller)
        {
            return controller.GetGuest(Id);
        }

        public string GetName()
        {
            string pref = string.Empty;
            if (ResCount > 1) pref = $"({ResCount})";
            return $"{pref}{FirstName} {LastName}";
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder()
                .AppendLine($"Име: {FirstName} {LastName}")
                .AppendLine($"Телефон: {Phone}")
                .AppendLine($"Имейл: {Email}");
            if (Referrer != null) sb.AppendLine($"Препоръчан от: {Referrer}");
            sb.AppendLine($"Брой резервации: {ResCount}");
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
                   && Email == other.Email
                   && ReferrerId == other.ReferrerId
                   && Referrer == other.Referrer
                   && ResCount == other.ResCount;
        }

        public override int GetHashCode()
        {
            HashCode hashCode = new HashCode();
            hashCode.Add(Id);
            hashCode.Add(FirstName);
            hashCode.Add(LastName);
            hashCode.Add(Phone);
            hashCode.Add(Email);
            hashCode.Add(Referrer);
            hashCode.Add(ResCount);
            return hashCode.ToHashCode();
        }
    }
}