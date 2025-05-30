using BankSystem.Core.Models.Users;
using System;
using System.Text.RegularExpressions;

namespace BankSystem.Core.Validations
{
    public static class UserValidations
    {
        public static void ValidateUser(UserBase user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            ValidateEmail(user.Email);
            ValidatePhone(user.Phone);
            ValidateAge(user.BirthDate);
            ValidatePassport(user.PassportSeries, user.PassportNumber);
        }

        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                throw new ArgumentException("Email должен содержать @");
        }

        public static void ValidatePhone(string phone)
        {
            var regex = new Regex(@"^\+375\s?(29|44|25|33)\d{7}$");
            if (!regex.IsMatch(phone))
                throw new ArgumentException("Телефон должен быть в формате +375 (29|44|25|33)XXXXXXX");
        }

        public static void ValidateAge(DateTime birthDate)
        {
            var age = DateTime.Today.Year - birthDate.Year;
            if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;

            if (age < 18)
                throw new ArgumentException("Пользователь должен быть старше 18 лет");
        }

        public static void ValidatePassport(string series, string number)
        {
            // Белорусский паспорт: 2 буквы и 7 цифр (AB1234567)
            var regex = new Regex(@"^[A-Za-z]{2}\d{7}$");
            if (!regex.IsMatch(series + number))
                throw new ArgumentException("Паспорт должен быть в формате: 2 буквы и 7 цифр (AB1234567)");
        }
    }
}