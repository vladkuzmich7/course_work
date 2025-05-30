using BankSystem.Core.Models;
using BankSystem.Core.Models.Users;
using BankSystem.Core.Repositories;
using BankSystem.Core.Validations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankSystem.Core.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public UserBase? GetUser(string login)
        {
            return _userRepository.GetByLogin(login);
        }

        public void AddUser(UserBase user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            UserValidations.ValidateUser(user);

            if (_userRepository.GetByLogin(user.Login) != null)
                throw new InvalidOperationException("Пользователь с таким логином уже существует");

            var existingUser = _userRepository.GetAllUsers()
                .FirstOrDefault(u => u.PassportSeries == user.PassportSeries &&
                                   u.PassportNumber == user.PassportNumber);

            if (existingUser != null)
                throw new InvalidOperationException("Пользователь с такими паспортными данными уже существует");

            _userRepository.AddUser(user);
        }

        public void ChangeUserRole(string login)
        {
            var user = _userRepository.GetByLogin(login);
            if (user == null)
                throw new KeyNotFoundException("Пользователь не найден");

            UserBase newUser;

            if (user is RegularUser)
            {
                newUser = new EmployeeUser(
                    user.Login,
                    "temporary_password",
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Phone,
                    user.BirthDate,
                    user.PassportSeries,
                    user.PassportNumber);
            }
            else if (user is EmployeeUser)
            {
                newUser = new RegularUser(
                    user.Login,
                    "temporary_password",
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.Phone,
                    user.BirthDate,
                    user.PassportSeries,
                    user.PassportNumber);
            }
            else
            {
                throw new InvalidOperationException("Нельзя изменить роль администратора");
            }

            _userRepository.UpdateUser(login, newUser);
        }

        public IEnumerable<UserBase> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public Card CreateCard(UserBase user, string accountNumber, DateTime expirationDate)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var account = user.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new ArgumentException("Счет не найден");

            // Генерация номера карты (16 цифр)
            var cardNumber = GenerateCardNumber();

            // Генерация CVV (3 цифры)
            var cvv = new Random().Next(100, 999).ToString();

            var card = new Card(cardNumber, cvv, expirationDate, account);
            account.AddCard(card);

            return card;
        }

        private string GenerateCardNumber()
        {
            var random = new Random();
            var cardNumber = "4"; // Начинаем с 4 (Visa)
            for (int i = 0; i < 15; i++)
            {
                cardNumber += random.Next(0, 9);
            }
            return cardNumber;
        }
        public Account CreateAccount(UserBase user, string currency, AccountType type)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Валюта обязательна");

            // Проверка валюты
            var allowedCurrencies = new[] { "RUB", "USD", "EUR", "BYN" };
            if (!allowedCurrencies.Contains(currency.ToUpper()))
                throw new ArgumentException("Данная валюта не поддерживается");

            string accountNumber = GenerateAccountNumber();
            var account = new Account(accountNumber, currency.ToUpper(), type);
            user.Accounts.Add(account);

            return account;
        }

        private string GenerateAccountNumber()
        {
            var random = new Random();
            string accountNumber = "";
            for (int i = 0; i < 12; i++)
                accountNumber += random.Next(0, 10);
            return accountNumber;
        }

        public Card CreateCard(UserBase user, string accountNumber)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var account = user.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);
            if (account == null)
                throw new ArgumentException("Счет не найден");

            // Генерация номера карты (16 цифр)
            var cardNumber = GenerateCardNumber();

            // Генерация CVV (3 цифры)
            var cvv = new Random().Next(100, 1000).ToString();

            // Срок действия: 3 года с текущей даты
            var expirationDate = DateTime.Now.AddYears(3);

            var card = new Card(cardNumber, cvv, expirationDate, account);
            account.AddCard(card);

            return card;
        }


    }

}