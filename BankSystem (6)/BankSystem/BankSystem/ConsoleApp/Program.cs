using BankSystem.Core.Models.Users;
using BankSystem.Core.Repositories;
using BankSystem.Core.Services.User;
using BankSystem.Core.Services;
using BankSystem.ConsoleApp.Menus;
using BankSystem.Core.Services.Auth;
using System;
using System.Threading.Tasks;
using BankSystem.Core.Models;

class Program
{
    static async Task Main(string[] args)
    {
        var depositTemplateRepository = new DepositTemplateRepository();
        var depositTemplateService = new DepositTemplateService(depositTemplateRepository);
        // Инициализация сервисов и репозиториев
        var exchangeRateService = new ExchangeRateService();
        var transactionService = new TransactionService(exchangeRateService);

        var userRepository = new UserRepository();
        var authService = new AuthService(userRepository);
        var userService = new UserService(userRepository);

        var creditTypeRepository = new CreditTypeRepository();
        var creditTypeService = new CreditTypeService(creditTypeRepository);

        var creditAppRepository = new CreditApplicationRepository();
        var creditAppService = new CreditApplicationService(creditAppRepository);

        var authMenu = new AuthMenu(authService, userService);
        var adminUser = userService.GetAllUsers().OfType<AdminUser>().FirstOrDefault();
        if (adminUser != null)
        {
            var adminObserver = new AdminNotificationObserver(adminUser);
            creditAppService.RegisterObserver(adminObserver);
        }
        UserBase? loggedInUser = null;
        var depositRepository = new DepositRepository();
        var depositService = new DepositService(depositRepository);

        // Создание тестового пользователя с картой и балансом
        try
        {
            var testUser = new RegularUser(
                "123",
                "123",
                "Иван",
                "Иванов",
                "ivan@example.com",
                "+375291234567",
                new DateTime(1990, 5, 15),
                "AB",
                "1234567");

            userService.AddUser(testUser);

            // Создаём счет и карту, зачисляем деньги
            var testAccount = userService.CreateAccount(testUser, "BYN", AccountType.Checking);
            testAccount.Balance = 1000m;
            var testCard = userService.CreateCard(testUser, testAccount.AccountNumber);

            Console.WriteLine($"Тестовый пользователь создан: {testUser.Login}");
            Console.WriteLine($"Номер счета: {testAccount.AccountNumber}, Баланс: {testAccount.Balance} {testAccount.Currency}");
            Console.WriteLine($"Карта: {testCard.Number}, CVV: {testCard.Cvv}, Срок: {testCard.ExpirationDate:MM/yy}");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }
        catch
        {
            // Игнорируем ошибку, если пользователь уже существует
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== Банковская система ===");
            Console.WriteLine("1. Вход");
            Console.WriteLine("2. Регистрация");
            Console.WriteLine("3. Выход");
            Console.Write("Выберите действие: ");

            var choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    loggedInUser = authMenu.ShowLoginMenu();
                    if (loggedInUser is AdminUser)
                    {
                        var adminMenu = new AdminMenu(userService, creditTypeService, creditAppService, loggedInUser, depositTemplateService);
                        adminMenu.ShowMenu();
                    }
                    else if (loggedInUser is EmployeeUser)
                    {
                        var employeeMenu = new AdminMenu(userService, creditTypeService, creditAppService, loggedInUser, depositTemplateService);
                        employeeMenu.ShowMenu();
                    }
                    else if (loggedInUser != null)
                    {
                        var userMenu = new UserMenu(
                            depositService,
                            depositTemplateService,
                            userService,
                            creditTypeService,
                            creditAppService,
                            transactionService,
                            loggedInUser
                        );
                        await userMenu.ShowMenuAsync();
                    }
                    break;
                case "2":
                    authMenu.ShowRegistrationMenu();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Неверный выбор!");
                    await Task.Delay(1000);
                    break;
            }
        }
    }
}