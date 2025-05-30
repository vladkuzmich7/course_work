using BankSystem.Core.Models.Users;
using BankSystem.Core.Services.User;
using BankSystem.Core.Services;
using BankSystem.Core.Models;

namespace BankSystem.ConsoleApp.Menus
{
    public class AdminMenu
    {
        private Dictionary<UserBase, UserMenu> _userMenus;
        private readonly DepositService _depositService;
        private readonly DepositTemplateService _depositTemplateService;
        private readonly IUserService _userService;
        private readonly UserService _userService1;
        private readonly ICreditTypeService _creditTypeService;
        private readonly ICreditApplicationService _creditApplicationService;
        private readonly UserBase _employee;

        public AdminMenu(
         IUserService userService,
         ICreditTypeService creditTypeService,
         ICreditApplicationService creditApplicationService,
         UserBase employee,
         DepositTemplateService depositTemplateService // <-- обязательно добавить
     )
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _creditTypeService = creditTypeService ?? throw new ArgumentNullException(nameof(creditTypeService));
            _creditApplicationService = creditApplicationService ?? throw new ArgumentNullException(nameof(creditApplicationService));
            _employee = employee ?? throw new ArgumentNullException(nameof(employee));
            _depositTemplateService = depositTemplateService ?? throw new ArgumentNullException(nameof(depositTemplateService));
        }
        private void ShowDepositTemplateMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление шаблонами вкладов ===");
                Console.WriteLine("1. Создать шаблон");
                Console.WriteLine("2. Список шаблонов");
                Console.WriteLine("0. Назад");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateDepositTemplateMenu();
                        break;
                    case "2":
                        ShowDepositTemplates();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        Thread.Sleep(1000);
                        break;
                }
            }
        }

        private void CreateDepositTemplateMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Создать шаблон вклада ===");
            Console.Write("Название: ");
            var name = Console.ReadLine();
            Console.Write("Мин. срок (мес.): ");
            var minMonths = int.Parse(Console.ReadLine());
            Console.Write("Макс. срок (мес.): ");
            var maxMonths = int.Parse(Console.ReadLine());
            Console.Write("Ставка (%): ");
            var rate = decimal.Parse(Console.ReadLine());
            Console.Write("Валюта: ");
            var currency = Console.ReadLine();
            Console.Write("Мин. сумма: ");
            var minSum = decimal.Parse(Console.ReadLine());
            Console.Write("Описание (опционально): ");
            var description = Console.ReadLine();

            var template = new DepositTemplate(name, minMonths, maxMonths, rate, currency, minSum, description ?? "");
            _depositTemplateService.AddTemplate(template);

            Console.WriteLine("Шаблон создан!");
            Console.ReadKey();
        }

        private void ShowDepositTemplates()
        {
            Console.Clear();
            Console.WriteLine("=== Список шаблонов вкладов ===");
            var templates = _depositTemplateService.GetAllTemplates().ToList();
            if (templates.Count == 0)
                Console.WriteLine("Нет шаблонов.");
            else
                for (var i = 0; i < templates.Count; i++)
                    Console.WriteLine($"{i + 1}. {templates[i]}");
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        public void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Меню администратора ===");
                Console.WriteLine("1. Управление пользователями");
                Console.WriteLine("2. Список пользователей");
                Console.WriteLine("3. Создать вид кредита");
                Console.WriteLine("4. Список видов кредитов");
                Console.WriteLine("5. Заявки на кредит");
                Console.WriteLine("6. Показать уведомления");
                Console.WriteLine("7. Выход в главное меню");
                Console.WriteLine("8. Управление шаблонами вкладов");
                Console.WriteLine("9. Просмотреть уведомления о новых вкладах");
                Console.Write("Выберите действие: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        UserManagementMenu();
                        break;
                    case "2":
                        ShowUsersList();
                        break;
                    case "3":
                        CreateCreditTypeMenu();
                        break;
                    case "4":
                        ShowCreditTypes();
                        break;
                    case "5":
                        ShowCreditApplicationsMenu(); 
                        break;
                    case "6":
                        if (_employee is AdminUser admin)
                            admin.ShowNotifications();
                        else
                            Console.WriteLine("Нет доступа!");
                        break;
                    case "7":
                        return;
                    case "8":
                        ShowDepositTemplateMenu();
                        break;
                    case "9":
                        ShowDepositNotificationsMenu();
                        break;

                    default:
                        Console.WriteLine("Неверный выбор!");
                        Thread.Sleep(1000);
                        break;
                }
            }
        }
        private void ShowDepositNotificationsMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Уведомления о новых вкладах ===");

            // Получаем все вклады, ожидающие подтверждения
            var pendingDeposits = _depositService.GetAll().Where(d => d.Status == DepositStatus.Active).ToList();
            if (pendingDeposits.Count == 0)
            {
                Console.WriteLine("Нет новых уведомлений о вкладах.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < pendingDeposits.Count; i++)
            {
                var d = pendingDeposits[i];
                Console.WriteLine($"   Счет: {d.AccountNumber}");
                Console.WriteLine($"   Сумма: {d.Principal} {d.Currency}");
                Console.WriteLine($"   Ставка: {d.InterestRate}%");
                Console.WriteLine($"   Дата открытия: {d.OpenDate:dd.MM.yyyy}");
                Console.WriteLine(new string('-', 40));
            }

            Console.Write("Введите номер вклада для обработки или 0 для возврата: ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 0 || idx > pendingDeposits.Count)
                return;
            if (idx == 0) return;

            var deposit = pendingDeposits[idx - 1];

            Console.WriteLine("1. Подтвердить вклад");
            Console.WriteLine("2. Отклонить вклад");
            var action = Console.ReadLine();
            if (action == "1")
            {
                deposit.Status = DepositStatus.Approved;
                Console.WriteLine("Вклад подтвержден!");
                // Здесь можно уведомить пользователя
            }
            else if (action == "2")
            {
                deposit.Status = DepositStatus.Declined;
                Console.WriteLine("Вклад отклонен.");
                // Здесь можно уведомить пользователя
            }
            Console.ReadKey();
        }
        private void CreateCreditTypeMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Создать вид кредита ===");
            Console.Write("Название кредита: ");
            var name = Console.ReadLine();

            Console.Write("Валюта (RUB, USD, EUR, BYN): ");
            var allowedCurrencies = new[] { "RUB", "USD", "EUR", "BYN" };
            var currency = Console.ReadLine()?.ToUpper();
            if (!allowedCurrencies.Contains(currency))
            {
                Console.WriteLine("Некорректная валюта!");
                Console.ReadKey();
                return;
            }

            Console.Write("Ставка (%): ");
            if (!decimal.TryParse(Console.ReadLine(), out var percent) || percent <= 0)
            {
                Console.WriteLine("Некорректная ставка!");
                Console.ReadKey();
                return;
            }

            Console.Write("Мин. срок (мес.): ");
            if (!int.TryParse(Console.ReadLine(), out var minMonths) || minMonths <= 0)
            {
                Console.WriteLine("Некорректный срок!");
                Console.ReadKey();
                return;
            }

            Console.Write("Макс. срок (мес.): ");
            if (!int.TryParse(Console.ReadLine(), out var maxMonths) || maxMonths < minMonths)
            {
                Console.WriteLine("Некорректный срок!");
                Console.ReadKey();
                return;
            }

            Console.Write("Мин. сумма: ");
            if (!decimal.TryParse(Console.ReadLine(), out var minSum) || minSum <= 0)
            {
                Console.WriteLine("Некорректная сумма!");
                Console.ReadKey();
                return;
            }

            Console.Write("Макс. сумма: ");
            if (!decimal.TryParse(Console.ReadLine(), out var maxSum) || maxSum < minSum)
            {
                Console.WriteLine("Некорректная сумма!");
                Console.ReadKey();
                return;
            }

            var type = new CreditType(name, percent, minMonths, maxMonths, minSum, maxSum, currency);
            _creditTypeService.AddCreditType(type);

            Console.WriteLine("Вид кредита создан!");
            Console.ReadKey();
        }

        private void ShowCreditTypes()
        {
            Console.Clear();
            Console.WriteLine("=== Список видов кредитов ===");
            var types = _creditTypeService.GetAllCreditTypes().ToList();
            if (types.Count == 0)
                Console.WriteLine("Нет доступных видов кредитов.");
            else
            {
                int i = 1;
                foreach (var t in types)
                {
                    Console.WriteLine($"{i++}. {t}");
                }
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void UserManagementMenu()
        {
            while (true) // Добавляем цикл для обработки возврата
            {
                Console.Clear();
                Console.WriteLine("=== Управление пользователями ===");

                var users = _userService.GetAllUsers()
                    .OrderBy(u => u is AdminUser ? 0 : u is EmployeeUser ? 1 : 2)
                    .ThenBy(u => u.LastName)
                    .ToList();

                PrintUsersList(users);

                Console.Write("\nВведите номер пользователя или логин (0 - назад): ");
                var input = Console.ReadLine();

                if (input == "0")
                    break; // Выходим из цикла и возвращаемся в ShowMenu

                var selectedUser = ResolveUserSelection(input, users);

                if (selectedUser == null)
                {
                    Console.WriteLine("Пользователь не найден!");
                    Console.ReadKey();
                    continue; // Продолжаем цикл
                }

                ShowUserActionsMenu(selectedUser);
            }
        }

        private void ShowUserActionsMenu(UserBase user)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== Управление пользователем: {user.Login} ===");
                PrintUserDetails(user);

                Console.WriteLine("\n1. Изменить роль");
                Console.WriteLine("2. Редактировать данные");
                Console.WriteLine("3. Назад");
                Console.Write("Выберите действие: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        ChangeUserRole(user);
                        return; // Выходим из метода и возвращаемся в UserManagementMenu
                    case "2":
                        EditUserData(user);
                        return; // Выходим из метода и возвращаемся в UserManagementMenu
                    case "3":
                        return; // Выходим из метода и возвращаемся в UserManagementMenu
                    default:
                        Console.WriteLine("Неверный выбор!");
                        Thread.Sleep(1000);
                        break;
                }
            }
        }

        private void ChangeUserRole(UserBase user)
        {
            if (user is AdminUser)
            {
                Console.WriteLine("Нельзя изменить роль администратора!");
                Console.ReadKey();
                return;
            }

            string currentRole = user is EmployeeUser ? "Сотрудник" : "Пользователь";
            string newRole = user is EmployeeUser ? "Пользователь" : "Сотрудник";

            Console.WriteLine($"\nТекущая роль: {currentRole}");
            Console.WriteLine($"Новая роль: {newRole}");
            Console.Write("Подтвердите изменение (y/n): ");

            if (Console.ReadLine()?.ToLower() == "y")
            {
                try
                {
                    _userService.ChangeUserRole(user.Login);
                    Console.WriteLine($"\nРоль пользователя {user.Login} изменена на {newRole}!");
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine($"\nОшибка: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("\nИзменение отменено");
            }
            Console.ReadKey();
        }

        private void EditUserData(UserBase user)
        {
            Console.Clear();
            Console.WriteLine($"=== Редактирование пользователя: {user.Login} ===");

            Console.WriteLine("\nТекущие данные:");
            PrintUserDetails(user);

            Console.WriteLine("\nВведите новые данные (оставьте пустым чтобы не изменять):");

            Console.Write($"Имя [{user.FirstName}]: ");
            var firstName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(firstName))
                user.FirstName = firstName;

            Console.Write($"Фамилия [{user.LastName}]: ");
            var lastName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(lastName))
                user.LastName = lastName;

            Console.Write($"Email [{user.Email}]: ");
            var email = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(email))
                user.Email = email;

            Console.Write($"Телефон [{user.Phone}]: ");
            var phone = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(phone))
                user.Phone = phone;

            Console.Write($"Дата рождения [{user.BirthDate:dd.MM.yyyy}]: ");
            var birthDateInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(birthDateInput) && DateTime.TryParse(birthDateInput, out var birthDate))
                user.BirthDate = birthDate;

            Console.Write($"Серия паспорта [{user.PassportSeries}]: ");
            var passportSeries = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(passportSeries))
                user.PassportSeries = passportSeries;

            Console.Write($"Номер паспорта [{user.PassportNumber}]: ");
            var passportNumber = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(passportNumber))
                user.PassportNumber = passportNumber;

            Console.WriteLine("\nДанные успешно обновлены!");
            Console.ReadKey();
        }
        private void ShowCreditApplicationsMenu()
        {
            while (true)
            {
                Console.Clear();
                var applications = _creditApplicationService.GetAllApplications()
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();

                if (applications.Count == 0)
                {
                    Console.WriteLine("Нет заявок на кредиты.");
                    Console.WriteLine("Нажмите любую клавишу для возврата...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("=== Заявки на кредиты ===");
                for (int i = 0; i < applications.Count; i++)
                {
                    var app = applications[i];
                    Console.WriteLine($"{i + 1}. [{app.Status}] {app.Applicant.LastName} {app.Applicant.FirstName} | {app.CreditType.Name} | {app.Sum} | {app.Months} мес. | Счет: {app.AccountNumber}");
                }
                Console.WriteLine("0. Назад");

                Console.Write("Выберите номер заявки для просмотра деталей: ");
                if (!int.TryParse(Console.ReadLine(), out int selected) || selected < 0 || selected > applications.Count)
                {
                    Console.WriteLine("Неверный выбор!");
                    Thread.Sleep(1000);
                    continue;
                }
                if (selected == 0)
                    return;

                var appDetails = applications[selected - 1];
                ShowCreditApplicationDetails(appDetails);
            }
        }

        private void ShowCreditApplicationDetails(CreditApplication app)
        {
            Console.Clear();
            var user = app.Applicant;
            Console.WriteLine("=== Детали заявки на кредит ===");
            Console.WriteLine($"Заявитель: {user.LastName} {user.FirstName} ({user.Login})");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Телефон: {user.Phone}");
            Console.WriteLine($"Дата рождения: {user.BirthDate:dd.MM.yyyy}");
            Console.WriteLine($"Паспорт: {user.PassportSeries} {user.PassportNumber}");
            Console.WriteLine($"Дата регистрации: {user.CreatedDate:dd.MM.yyyy HH:mm}");
            Console.WriteLine("-----");
            Console.WriteLine($"Вид кредита: {app.CreditType.Name}");
            Console.WriteLine($"Сумма: {app.Sum}");
            Console.WriteLine($"Срок: {app.Months} мес.");
            Console.WriteLine($"Счет зачисления: {app.AccountNumber}");
            Console.WriteLine($"Статус: {app.Status}");
            if (app.Status != CreditApplicationStatus.Pending)
            {
                Console.WriteLine($"Кем рассмотрено: {app.ApprovedBy?.Login ?? "-"}");
                Console.WriteLine($"Дата решения: {app.DecisionAt:dd.MM.yyyy HH:mm}");
            }

            if (app.Status == CreditApplicationStatus.Pending)
            {
                Console.Write("Одобрить (y), Отклонить (n), Назад (другое): ");
                var action = Console.ReadLine();
                if (action == "y")
                {
                    _creditApplicationService.ApproveApplication(app.Id, _employee);
                    Console.WriteLine("Заявка одобрена и кредит выдан.");
                }
                else if (action == "n")
                {
                    _creditApplicationService.RejectApplication(app.Id, _employee);
                    Console.WriteLine("Заявка отклонена.");
                }
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
        private void ShowFullUserInfo(UserBase user)
        {
            Console.Clear();
            Console.WriteLine($"=== Полная информация о пользователе ===");
            Console.WriteLine($"Логин: {user.Login}");
            Console.WriteLine($"Имя: {user.FirstName} {user.LastName}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Телефон: {user.Phone}");
            Console.WriteLine($"Дата рождения: {user.BirthDate:dd.MM.yyyy}");
            Console.WriteLine($"Паспорт: {user.PassportSeries} {user.PassportNumber}");
            Console.WriteLine($"Роль: {GetRoleName(user)}");
            Console.WriteLine($"Дата регистрации: {user.CreatedDate:dd.MM.yyyy HH:mm}");

            // Счета
            Console.WriteLine("\n--- Счета пользователя ---");
            if (user.Accounts != null && user.Accounts.Count > 0)
            {
                foreach (var acc in user.Accounts)
                {
                    Console.WriteLine($"Счет: {acc.AccountNumber} | Валюта: {acc.Currency} | Тип: {acc.Type} | Баланс: {acc.Balance}");
                    // Карты по счету
                    if (acc.Cards != null && acc.Cards.Count > 0)
                    {
                        foreach (var card in acc.Cards)
                        {
                            Console.WriteLine($"    Карта: {card.Number} | CVV: {card.Cvv} | Срок: {card.ExpirationDate:MM/yy}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Нет счетов.");
            }

            // Кредиты
            Console.WriteLine("\n--- Кредиты пользователя ---");
            if (user.Credits != null && user.Credits.Count > 0)
            {
                foreach (var credit in user.Credits)
                {
                    Console.WriteLine($"Кредит: {credit.AccountNumber} | Сумма: {credit.Sum} | Остаток: {credit.Balance} | Ставка: {credit.Percent}% | Месяцев: {credit.Months} | Статус: {credit.Status}");
                }
            }
            else
            {
                Console.WriteLine("Нет кредитов.");
            }

            // Вклады
            Console.WriteLine("\n--- Вклады пользователя ---");
            if (user.Deposits != null && user.Deposits.Count > 0)
            {
                foreach (var deposit in user.Deposits)
                {
                    Console.WriteLine($"Вклад: {deposit.DepositAccountNumber} | Сумма: {deposit.Principal} | Ставка: {deposit.InterestRate}% | Статус: {deposit.Status} | Открыт: {deposit.OpenDate:dd.MM.yyyy}");
                }
            }
            else
            {
                Console.WriteLine("Нет вкладов.");
            }
        }
        // Пример внутри AdminMenu:
        private void ShowUserTransactions(UserBase user)
        {
            UserMenu userMenu = _userMenus[user];
            var transactions = userMenu.GetUserTransactions();

            Console.WriteLine($"Транзакции пользователя {user.Login}:");
            foreach (var t in transactions)
            {
                t.ShowInfo();
                Console.WriteLine(new string('-', 40));
            }
        }

        private void ShowUsersList()
        {
            Console.Clear();
            Console.WriteLine("=== Список всех пользователей ===");

            var users = _userService.GetAllUsers()
                .OrderBy(u => u is AdminUser ? 0 : u is EmployeeUser ? 1 : 2)
                .ThenBy(u => u.LastName);

            PrintUsersList(users);

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        #region Helper Methods

        private UserBase? ResolveUserSelection(string? input, List<UserBase> users)
        {
            if (string.IsNullOrEmpty(input)) return null;

            if (int.TryParse(input, out int number) && number > 0 && number <= users.Count)
            {
                return users[number - 1];
            }
            return users.FirstOrDefault(u => u.Login.Equals(input, StringComparison.OrdinalIgnoreCase));
        }

        private void PrintUsersList(IEnumerable<UserBase> users)
        {
            Console.WriteLine("\n{0,-5} {1,-15} {2,-15} {3,-15} {4,-25}",
                "#", "Логин", "Имя", "Фамилия", "Роль");
            Console.WriteLine(new string('-', 80));

            int index = 1;
            foreach (var user in users)
            {
                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-15} {4,-25}",
                    index++,
                    user.Login,
                    user.FirstName,
                    user.LastName,
                    GetRoleName(user));
            }
        }

        private void PrintUserDetails(UserBase user)
        {
            Console.WriteLine($"Логин: {user.Login}");
            Console.WriteLine($"Имя: {user.FirstName} {user.LastName}");
            Console.WriteLine($"Email: {user.Email}");
            Console.WriteLine($"Телефон: {user.Phone}");
            Console.WriteLine($"Роль: {GetRoleName(user)}");
        }

        private string GetRoleName(UserBase user)
        {
            return user switch
            {
                AdminUser => "Администратор",
                EmployeeUser => "Сотрудник",
                _ => "Пользователь"
            };
        }

        #endregion
    }
}