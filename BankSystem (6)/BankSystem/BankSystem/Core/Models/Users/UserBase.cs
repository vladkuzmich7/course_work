using BankSystem.Core.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace BankSystem.Core.Models.Users
{
    public abstract class UserBase : IRole
    {
        public List<Deposit> Deposits { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public string Login { get; set; }
        protected string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public List<Account> Accounts { get; set; } = new List<Account>();

        protected UserBase(string login, string password, string firstName, string lastName,
                         string email, string phone, DateTime birthDate,
                         string passportSeries, string passportNumber)
        {
            Login = login;
            PasswordHash = HashPassword(password);
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            BirthDate = birthDate;
            PassportSeries = passportSeries;
            PassportNumber = passportNumber;
        }
        public List<Credit> Credits { get; } = new List<Credit>();

        public bool VerifyPassword(string inputPassword)
        {
            return HashPassword(inputPassword) == PasswordHash;
        }

        protected static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        public abstract string RoleName { get; }
        public abstract void DisplayPermissions();
    }
}