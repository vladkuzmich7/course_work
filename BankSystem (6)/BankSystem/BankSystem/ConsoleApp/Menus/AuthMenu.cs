using BankSystem.Core.Models.Users;
using BankSystem.Core.Services.Auth;
using BankSystem.Core.Services.User;
using System;

namespace BankSystem.ConsoleApp.Menus
{
    // Класс LiteDbUser и репозиторий LiteDbUserRepository можно оставить только для миграции/импорта!
    // После импорта использовать их в AuthMenu не требуется и НЕ следует!

    public class AuthMenu
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AuthMenu(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        public UserBase? ShowLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Вход ===");

            Console.Write("Логин: ");
            var login = Console.ReadLine() ?? string.Empty;

            Console.Write("Пароль: ");
            var password = Console.ReadLine() ?? string.Empty;

            // Только через старый репозиторий!
            var user = _authService.Login(login, password);
            if (user == null)
            {
                Console.WriteLine("Ошибка входа! Неверный логин или пароль.");
                Console.ReadKey();
                return null;
            }

            Console.WriteLine($"\nДобро пожаловать, {user.FirstName}!");
            user.DisplayPermissions();
            Console.ReadKey();
            return user;
        }

        public void ShowRegistrationMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Регистрация нового пользователя ===");

            Console.Write("Имя: ");
            var firstName = Console.ReadLine() ?? string.Empty;

            Console.Write("Фамилия: ");
            var lastName = Console.ReadLine() ?? string.Empty;

            Console.Write("Email(@): ");
            var email = Console.ReadLine() ?? string.Empty;

            Console.Write("Телефон (формат: +375(29)XXX...): ");
            var phone = Console.ReadLine() ?? string.Empty;

            Console.Write("Дата рождения (дд.мм.гггг): ");
            DateTime birthDate;
            while (!DateTime.TryParse(Console.ReadLine(), out birthDate))
            {
                Console.Write("Некорректная дата. Введите снова (дд.мм.гггг): ");
            }

            Console.Write("Серия паспорта (2 буквы): ");
            var passportSeries = Console.ReadLine() ?? string.Empty;

            Console.Write("Номер паспорта (7 цифр): ");
            var passportNumber = Console.ReadLine() ?? string.Empty;

            Console.Write("Логин: ");
            var login = Console.ReadLine() ?? string.Empty;

            Console.Write("Пароль: ");
            var password = Console.ReadLine() ?? string.Empty;

            try
            {
                // Создаём пользователя только в старом репозитории!
                var classicUser = new RegularUser(
                    login: login,
                    password: password,
                    firstName: firstName,
                    lastName: lastName,
                    email: email,
                    phone: phone,
                    birthDate: birthDate,
                    passportSeries: passportSeries,
                    passportNumber: passportNumber
                );
                _userService.AddUser(classicUser);

                Console.WriteLine("\nРегистрация прошла успешно!");
                Console.WriteLine($"Логин: {login}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка регистрации: {ex.Message}");
            }

            Console.ReadKey();
        }
    }
}