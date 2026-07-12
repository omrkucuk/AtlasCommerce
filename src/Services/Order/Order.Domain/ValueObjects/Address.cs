using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.ValueObjects
{
    public sealed record Address(
        string FirstName,
        string LastName,
        string Phone,
        string City,
        string District,
        string FullAddress,
        string ZipCode,
        string Country = "TR")
    {
        public static Address Of(string firstName, string lastName, string phone, string city, string district, string fullAddress, string zipCode, string country = "TR")
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("Ad boş olamaz.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Soyad  boş olamaz.");
            if (string.IsNullOrWhiteSpace(phone)) throw new ArgumentException("Telefon  boş olamaz.");
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("Şehir  boş olamaz.");
            if (string.IsNullOrWhiteSpace(district)) throw new ArgumentException("İlçe boş olamaz.");
            if (string.IsNullOrWhiteSpace(fullAddress)) throw new ArgumentException("Adres boş olamaz.");

            return new Address(firstName, lastName, phone, city, district, fullAddress, zipCode, country);
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}
