using BankSystem.Core.Models;
using BankSystem.Core.Models.Users;
using BankSystem.Core.Services.User;
using BankSystem.Core.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace BankSystem.ConsoleApp.Menus
{
    public class UserMenu
    {

        private readonly List<Deposit> _userDeposits = new();
        private readonly List<string> _notifications = new(); // Новый список уведомлений
        private readonly DepositService _depositService;
        private readonly DepositTemplateService _depositTemplateService;
        private readonly TransactionService _transactionService;
        private readonly IUserService _userService;
        private readonly ICreditTypeService _creditTypeService;
        private readonly ICreditApplicationService _creditApplicationService;
        private readonly UserBase _user;

        // История транзакций пользователя (можно заменить на получение из БД)
        private readonly List<Transaction> _transactions = new();

        public UserMenu(
            DepositService depositService,
            DepositTemplateService depositTemplateService,
            IUserService userService,
            ICreditTypeService creditTypeService,
            ICreditApplicationService creditApplicationService,
            TransactionService transactionService,
            UserBase user)
        {
            _depositService = depositService ?? throw new ArgumentNullException(nameof(depositService));
            _depositTemplateService = depositTemplateService ?? throw new ArgumentNullException(nameof(depositTemplateService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _creditTypeService = creditTypeService ?? throw new ArgumentNullException(nameof(creditTypeService));
            _creditApplicationService = creditApplicationService ?? throw new ArgumentNullException(nameof(creditApplicationService));
            _transactionService = transactionService ?? throw new ArgumentNullException(nameof(transactionService));
            _user = user ?? throw new ArgumentNullException(nameof(user));
        }

        public async Task ShowMenuAsync()
        {
            while (true)
            {
                UpdateCreditNotifications(); // Проверяем уведомления перед каждым показом меню

                Console.Clear();
                ShowNotifications(); // Показываем уведомления
                Console.Clear();
                Console.WriteLine($"=== Меню пользователя: {_user.Login} ===");
                Console.WriteLine("1. Создать счет");
                Console.WriteLine("2. Просмотреть мои счета");
                Console.WriteLine("3. Создать карту");
                Console.WriteLine("4. Просмотреть мои карты");
                Console.WriteLine("5. Взять кредит");
                Console.WriteLine("6. Просмотреть кредиты");
                Console.WriteLine("7. Создать перевод");
                Console.WriteLine("8. История переводов");
                Console.WriteLine("9. Создать вклад");
                Console.WriteLine("10. Выход");
                Console.WriteLine("11. Мои вклады");
                Console.WriteLine("12. Погасить кредит"); // Новый пункт
                Console.WriteLine("13. Удалить счет");      // <- Новый пункт
                Console.WriteLine("14. Удалить карту");     // <- Новый пункт
                Console.WriteLine("15. Просмотреть уведомления"); // Новый пункт
                Console.Write("Выберите действие: ");

                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        CreateAccountMenu();
                        break;
                    case "2":
                        ShowAccounts();
                        break;
                    case "3":
                        CreateCardMenu();
                        break;
                    case "4":
                        ShowCards();
                        break;
                    case "5":
                        TakeCreditMenu();
                        break;
                    case "6":
                        ShowCreditsMenu();
                        break;
                    case "7":
                        await CreateTransactionMenuAsync();
                        break;
                    case "8":
                        ShowTransactions();
                        break;
                    case "9":
                        CreateDepositMenu();
                        break;
                    case "10":
                        return;
                    case "11":
                        ShowDepositsMenu();
                        break;
                    case "12":
                        PayCreditMenu(); // Новый метод
                        break;
                    case "13":
                        DeleteAccountMenu();
                        break;
                    case "14":
                        DeleteCardMenu();
                        break;
                    case "15":
                        ViewNotificationsMenu();
                        break;
                    default:
                        Console.WriteLine("Неверный выбор!");
                        System.Threading.Thread.Sleep(1000);
                        break;
                }
            }
        }
        private void DeleteCardMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Удаление карты ===");
            var allCards = _user.Accounts.SelectMany(a => a.Cards).ToList();
            if (allCards.Count == 0)
            {
                Console.WriteLine("У вас нет карт.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Ваши карты:");
            for (int i = 0; i < allCards.Count; i++)
            {
                var card = allCards[i];
                Console.WriteLine($"{i + 1}. {card.Number} (Счет: {card.Account.AccountNumber}, Валюта: {card.Account.Currency})");
            }
            Console.Write("Выберите номер карты для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int cardIdx) || cardIdx < 1 || cardIdx > allCards.Count)
            {
                Console.WriteLine("Неверный выбор!");
                Console.ReadKey();
                return;
            }
            var cardToDelete = allCards[cardIdx - 1];
            Console.Write("Вы уверены, что хотите удалить эту карту? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() != "y")
            {
                Console.WriteLine("Удаление отменено.");
                Console.ReadKey();
                return;
            }

            cardToDelete.Account.Cards.Remove(cardToDelete);
            AddNotification($"Карта {cardToDelete.Number} удалена.");
            Console.WriteLine("Карта успешно удалена!");
            Console.ReadKey();
        }

        private void UpdateCreditNotifications()
        {
            // Не очищаем список, чтобы не терять старые уведомления
            if (_user?.Credits == null)
                return;

            foreach (var credit in _user.Credits)
            {
                if (credit.IsPaymentOverdue())
                {
                    decimal monthly = credit.GetMonthlyPayment();
                    string note = $"Внимание! Платеж по кредиту {credit.AccountNumber} ({credit.Balance:F2} {credit.Currency}) просрочен. " +
                                  $"Погасите хотя бы ежемесячный платеж: {monthly:F2} {credit.Currency}.";
                    if (!_notifications.Contains(note))
                        _notifications.Add(note);
                }
            }
        }
        private void ViewNotificationsMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Ваши уведомления ===");
            if (_notifications.Count == 0)
            {
                Console.WriteLine("Нет новых уведомлений.");
            }
            else
            {
                foreach (var note in _notifications)
                    Console.WriteLine("- " + note);
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void AddNotification(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                _notifications.Add($"{DateTime.Now:dd.MM.yyyy HH:mm:ss}: {message}");
            }
        }

        private void ShowNotifications()
        {
            if (_notifications.Count == 0)
                return;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== Уведомления ===");
            foreach (var note in _notifications.TakeLast(3))
                Console.WriteLine(note);
            Console.ResetColor();
            Console.WriteLine();
        }
        private void PayCreditMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Погашение кредита ===");

            if (_user.Credits == null || _user.Credits.Count == 0)
            {
                Console.WriteLine("У вас нет активных кредитов.");
                Console.ReadKey();
                return;
            }

            var activeCredits = _user.Credits.Where(c => c.Balance > 0 && c.Status == CreditStatus.Active).ToList();
            if (activeCredits.Count == 0)
            {
                Console.WriteLine("У вас нет кредитов, требующих погашения.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Ваши кредиты для погашения:");
            for (int i = 0; i < activeCredits.Count; i++)
            {
                var c = activeCredits[i];
                Console.WriteLine($"{i + 1}. Кредит ID: {c.AccountNumber} | Остаток к погашению: {c.Balance:F2} | Счет: {c.AccountNumber}");
            }

            Console.Write("Выберите кредит для погашения (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int creditIndex) || creditIndex < 1 || creditIndex > activeCredits.Count)
            {
                Console.WriteLine("Неверный выбор.");
                Console.ReadKey();
                return;
            }
            var selectedCredit = activeCredits[creditIndex - 1];

            var userAccounts = _user.Accounts.Where(a => a.Currency == selectedCredit.Currency).ToList();
            if (userAccounts.Count == 0) userAccounts = _user.Accounts.ToList();
            Console.WriteLine("С ваших счетов:");
            for (int i = 0; i < userAccounts.Count; i++)
            {
                var a = userAccounts[i];
                Console.WriteLine($"{i + 1}. Счет: {a.AccountNumber} | Баланс: {a.Balance} {a.Currency}");
            }
            Console.Write("Выберите счет для списания (номер): ");
            if (!int.TryParse(Console.ReadLine(), out int accountIdx) || accountIdx < 1 || accountIdx > userAccounts.Count)
            {
                Console.WriteLine("Неверный выбор.");
                Console.ReadKey();
                return;
            }
            var selectedAccount = userAccounts[accountIdx - 1];

            decimal monthlyPayment = selectedCredit.GetMonthlyPayment();
            decimal totalPaymentLeft = selectedCredit.Balance;

            Console.WriteLine($"\nОбщая сумма к погашению по кредиту: {totalPaymentLeft:F2} {selectedCredit.Currency}");
            Console.WriteLine($"Платеж за этот месяц: {monthlyPayment:F2} {selectedCredit.Currency}");

            Console.Write($"Введите сумму платежа (от {monthlyPayment:F2} до {totalPaymentLeft:F2}): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal payment) || payment < monthlyPayment || payment > selectedAccount.Balance || payment > totalPaymentLeft)
            {
                Console.WriteLine("Некорректная сумма для платежа.");
                Console.ReadKey();
                return;
            }

            try
            {
                TransferToCredit(selectedAccount, selectedCredit, payment);
                AddNotification($"Погашен кредит {selectedCredit.AccountNumber} на сумму {payment:F2} {selectedCredit.Currency}");
                Console.WriteLine("Платеж успешно совершен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("Нажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
        private void DeleteAccountMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Удаление счета ===");
            if (_user.Accounts.Count == 0)
            {
                Console.WriteLine("У вас нет счетов.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Ваши счета:");
            for (int i = 0; i < _user.Accounts.Count; i++)
            {
                var acc = _user.Accounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} | Баланс: {acc.Balance} {acc.Currency} | Карт: {acc.Cards.Count}");
            }
            Console.Write("Выберите номер счета для удаления: ");
            if (!int.TryParse(Console.ReadLine(), out int accIdx) || accIdx < 1 || accIdx > _user.Accounts.Count)
            {
                Console.WriteLine("Неверный выбор!");
                Console.ReadKey();
                return;
            }
            var account = _user.Accounts[accIdx - 1];

            if (account.Balance != 0)
            {
                Console.WriteLine("Нельзя удалить счет с ненулевым балансом!");
                Console.ReadKey();
                return;
            }
            if (account.Cards.Count > 0)
            {
                Console.WriteLine("Удалите все карты, привязанные к этому счету, прежде чем удалять счет.");
                Console.ReadKey();
                return;
            }

            Console.Write("Вы уверены, что хотите удалить этот счет? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() != "y")
            {
                Console.WriteLine("Удаление отменено.");
                Console.ReadKey();
                return;
            }

            _user.Accounts.Remove(account);
            AddNotification($"Счет {account.AccountNumber} удален.");
            Console.WriteLine("Счет успешно удален!");
            Console.ReadKey();
        }
        public void AddDepositForUser(Deposit deposit)
        {
            _userDeposits.Add(deposit);

            // Если у пользователя есть коллекция Deposits — добавляем и туда (если нужно)
            if (_user.Deposits != null)
                _user.Deposits.Add(deposit);
        }

        // Вся логика перевода денег на кредит внутри UserMenu (без отдельного сервиса)
        private void TransferToCredit(Account fromAccount, Credit credit, decimal amount)
        {
            if (fromAccount == null)
                throw new ArgumentNullException(nameof(fromAccount));
            if (credit == null)
                throw new ArgumentNullException(nameof(credit));
            if (amount <= 0)
                throw new ArgumentException("Сумма должна быть положительной");
            if (fromAccount.Balance < amount)
                throw new InvalidOperationException("Недостаточно средств на счете");

            fromAccount.Balance -= amount;
            credit.Repay(amount);
        }
        // ... внутри UserMenu ...
        private void CreateDepositMenu()
{
    Console.Clear();
    Console.WriteLine("=== Открытие вклада ===");
    var templates = _depositTemplateService.GetAllTemplates().ToList();
    if (templates.Count == 0)
    {
        Console.WriteLine("Нет доступных шаблонов вкладов. Обратитесь к сотруднику банка.");
        Console.ReadKey();
        return;
    }

    Console.WriteLine("Доступные шаблоны вкладов:");
    for (int i = 0; i < templates.Count; i++)
        Console.WriteLine($"{i + 1}. {templates[i]}");
    Console.Write("Выберите номер шаблона: ");
    if (!int.TryParse(Console.ReadLine(), out int templateIndex) || templateIndex < 1 || templateIndex > templates.Count)
    {
        Console.WriteLine("Неверный выбор шаблона!");
        Console.ReadKey();
        return;
    }
    var selectedTemplate = templates[templateIndex - 1];

    // 1. Выбор счета для списания
    var fromAccounts = _user.Accounts
        .Where(a => a.Currency == selectedTemplate.Currency && a.Type != AccountType.Deposit)
        .ToList();
    if (fromAccounts.Count == 0)
    {
        Console.WriteLine($"У вас нет счетов в валюте {selectedTemplate.Currency} для списания.");
        Console.ReadKey();
        return;
    }
    Console.WriteLine("Выберите счет для списания суммы вклада:");
    for (int i = 0; i < fromAccounts.Count; i++)
        Console.WriteLine($"{i + 1}. {fromAccounts[i].AccountNumber} (Баланс: {fromAccounts[i].Balance})");
    Console.Write("Введите номер счета: ");
    if (!int.TryParse(Console.ReadLine(), out int fromIdx) || fromIdx < 1 || fromIdx > fromAccounts.Count)
    {
        Console.WriteLine("Неверный выбор!");
        Console.ReadKey();
        return;
    }
    var fromAccount = fromAccounts[fromIdx - 1];

    // 2. Ввод суммы и проверка баланса
    Console.Write($"Введите сумму вклада (от {selectedTemplate.MinSum}): ");
    if (!decimal.TryParse(Console.ReadLine(), out decimal sum) || sum < selectedTemplate.MinSum || sum > fromAccount.Balance)
    {
        Console.WriteLine("Некорректная сумма или недостаточно средств!");
        Console.ReadKey();
        return;
    }

    // 3. Ввод срока вклада
    Console.Write($"Введите срок вклада в месяцах ({selectedTemplate.MinMonths}-{selectedTemplate.MaxMonths}): ");
    if (!int.TryParse(Console.ReadLine(), out int months) ||
        months < selectedTemplate.MinMonths || months > selectedTemplate.MaxMonths)
    {
        Console.WriteLine("Некорректный срок!");
        Console.ReadKey();
        return;
    }

    // 4. Открытие депозитного счета
    var depositAccount = _userService.CreateAccount(_user, selectedTemplate.Currency, AccountType.Deposit);
    depositAccount.Balance = sum;

    // 5. Списание суммы со счета пользователя
    fromAccount.Balance -= sum;

    // 6. Капитализация или отдельный счет для процентов
    Console.Write("Начислять проценты с капитализацией (y) или на отдельный счет (n)? (y/n): ");
    bool capitalization = Console.ReadLine()?.Trim().ToLower() == "y";
    string payoutAccountNumber = null;
    if (!capitalization)
    {
        var payoutAccounts = _user.Accounts
            .Where(a => a.Currency == selectedTemplate.Currency && a.Type != AccountType.Deposit)
            .ToList();
        if (payoutAccounts.Count == 0)
        {
            Console.WriteLine("Нет подходящих счетов для выплаты процентов! Вклад будет с капитализацией.");
            capitalization = true;
        }
        else
        {
            Console.WriteLine("Выберите счет для выплаты процентов:");
            for (int i = 0; i < payoutAccounts.Count; i++)
                Console.WriteLine($"{i + 1}. {payoutAccounts[i].AccountNumber} (Баланс: {payoutAccounts[i].Balance})");
            Console.Write("Введите номер счета: ");
            if (!int.TryParse(Console.ReadLine(), out int payoutIdx) || payoutIdx < 1 || payoutIdx > payoutAccounts.Count)
            {
                Console.WriteLine("Неверный выбор! Вклад будет с капитализацией.");
                capitalization = true;
            }
            else
            {
                payoutAccountNumber = payoutAccounts[payoutIdx - 1].AccountNumber;
            }
        }
    }

    // 7. Создание объекта Deposit и сохранение вклада
    var deposit = new Deposit(depositAccount.AccountNumber, sum, selectedTemplate.InterestRate, selectedTemplate.Currency, capitalization, payoutAccountNumber);
    AddDepositForUser(deposit);

    Console.WriteLine($"Вклад открыт! Id: {deposit.Id}, депозитный счет: {depositAccount.AccountNumber}");
    Console.WriteLine("Нажмите любую клавишу для возврата...");
    Console.ReadKey();
}
        private void ShowDepositsMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Мои вклады ===");

            var userDeposits = _depositService.GetDepositsForUser(_user);

            if (!userDeposits.Any())
            {
                Console.WriteLine("У вас нет открытых вкладов.");
            }
            else
            {
                foreach (var deposit in userDeposits)
                {
                    Console.WriteLine($"Вклад #{deposit.Id}");
                    Console.WriteLine($"Счет: {deposit.AccountNumber}");
                    Console.WriteLine($"Сумма: {deposit.Principal} {deposit.Currency}");
                    Console.WriteLine($"Начислено процентов: {deposit.AccumulatedInterest:F2}");
                    Console.WriteLine($"Ставка: {deposit.InterestRate}%");
                    Console.WriteLine($"Дата открытия: {deposit.OpenDate:dd.MM.yyyy}");
                    Console.WriteLine($"Статус: {deposit.Status}");
                    Console.WriteLine(new string('-', 30));
                }
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private async Task CreateTransactionMenuAsync()
        {
            Console.Clear();
            Console.WriteLine("=== Создать перевод ===");
            if (_user.Accounts.All(a => a.Cards.Count == 0))
            {
                Console.WriteLine("У вас нет ни одной карты для отправки средств.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Ваши карты:");
            var allCards = _user.Accounts.SelectMany(a => a.Cards).ToList();
            for (int i = 0; i < allCards.Count; i++)
                Console.WriteLine($"{i + 1}. {allCards[i].Number} ({allCards[i].Account.Currency}, баланс: {allCards[i].Account.Balance})");

            Console.Write("Выберите карту для списания: ");
            if (!int.TryParse(Console.ReadLine(), out int cardIdx) || cardIdx < 1 || cardIdx > allCards.Count)
            {
                Console.WriteLine("Неверный выбор!");
                Console.ReadKey();
                return;
            }
            var senderCard = allCards[cardIdx - 1];

            Console.Write("Введите номер карты получателя: ");
            var receiverCardNumber = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(receiverCardNumber))
            {
                Console.WriteLine("Некорректный номер карты!");
                Console.ReadKey();
                return;
            }

            if (receiverCardNumber == senderCard.Number)
            {
                Console.WriteLine("Нельзя переводить деньги на ту же карту!");
                Console.ReadKey();
                return;
            }

            var receiverUser = _userService.GetAllUsers()
                .FirstOrDefault(u => u.Accounts.SelectMany(a => a.Cards).Any(c => c.Number == receiverCardNumber));

            if (receiverUser == null)
            {
                Console.WriteLine("Карта получателя не найдена!");
                Console.ReadKey();
                return;
            }
            var receiverCard = receiverUser.Accounts.SelectMany(a => a.Cards)
                .First(c => c.Number == receiverCardNumber);

            Console.Write("Введите сумму: ");
            if (!decimal.TryParse(Console.ReadLine(), out var amount) || amount <= 0)
            {
                Console.WriteLine("Некорректная сумма!");
                Console.ReadKey();
                return;
            }

            var commission = Math.Round(amount * 0.01m, 2, MidpointRounding.AwayFromZero);
            var total = amount + commission;
            Console.WriteLine($"Будет списано {total} (с учетом комиссии 1% = {commission})");

            Console.Write("Подтвердите перевод (y/n): ");
            if (Console.ReadLine()?.ToLower() != "y")
            {
                Console.WriteLine("Перевод отменен.");
                Console.ReadKey();
                return;
            }

            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(
                    _user, senderCard.Number, receiverUser, receiverCard.Number, amount, commission);

                _transactions.Add(transaction);
                AddNotification($"Выполнен перевод {amount:F2} с карты {senderCard.Number} на карту {receiverCardNumber}");
                Console.WriteLine("Перевод успешно выполнен:");
                transaction.ShowInfo();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void ShowTransactions()
        {
            Console.Clear();
            Console.WriteLine("=== История переводов ===");
            if (_transactions.Count == 0)
            {
                Console.WriteLine("Нет совершённых переводов.");
            }
            else
            {
                foreach (var t in _transactions.OrderByDescending(t => t.Date))
                {
                    t.ShowInfo();
                    Console.WriteLine(new string('-', 40));
                }
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void CreateAccountMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Создание нового счета ===");
            var selectedType = AccountType.Checking;
            var allowedCurrencies = new[] { "BYN", "USD", "EUR", "RUB" };
            Console.Write("Доступные валюты: ");
            Console.WriteLine(string.Join(", ", allowedCurrencies));
            Console.Write("Введите валюту счета: ");
            string currency = Console.ReadLine().ToUpper();

            if (!allowedCurrencies.Contains(currency))
            {
                Console.WriteLine("Неверная валюта!");
                Console.ReadKey();
                return;
            }

            try
            {
                var account = _userService.CreateAccount(_user, currency, selectedType);
                AddNotification($"Создан новый счет: {account.AccountNumber} ({currency})");
                Console.WriteLine($"\nСчет успешно создан!");
                Console.WriteLine($"Номер счета: {account.AccountNumber}");
                Console.WriteLine($"Валюта: {account.Currency}");
                Console.WriteLine($"Начальный баланс: {account.Balance}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при создании счета: {ex.Message}");
            }


            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void CreateCardMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Создание карты ===");

            if (_user.Accounts.Count == 0)
            {
                Console.WriteLine("У вас нет счетов для привязки карты!");
                Console.WriteLine("Нажмите любую клавишу для возврата...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Выберите счет для привязки карты:");
            for (int i = 0; i < _user.Accounts.Count; i++)
            {
                var acc = _user.Accounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} ({acc.Balance} {acc.Currency}, {acc.Type})");
            }

            Console.Write("Введите номер счета: ");
            if (!int.TryParse(Console.ReadLine(), out int accountIndex) || accountIndex < 1 || accountIndex > _user.Accounts.Count)
            {
                Console.WriteLine("Неверный выбор счета!");
                Console.ReadKey();
                return;
            }

            var account = _user.Accounts[accountIndex - 1];

            try
            {
                var card = _userService.CreateCard(_user, account.AccountNumber);
                AddNotification($"Создана новая карта: {card.Number} для счета {account.AccountNumber}");
                Console.WriteLine($"\nКарта успешно создана!");
                Console.WriteLine($"Номер карты: {card.Number}");
                Console.WriteLine($"Срок действия: {card.ExpirationDate:MM/yy}");
                Console.WriteLine($"CVV: {card.Cvv} (запомните его, он больше не будет показываться)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при создании карты: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void ShowAccounts()
        {
            Console.Clear();
            Console.WriteLine("=== Мои счета ===");

            if (_user.Accounts.Count == 0)
            {
                Console.WriteLine("У вас нет счетов.");
            }
            else
            {
                foreach (var acc in _user.Accounts)
                {
                    Console.WriteLine($"Номер: {acc.AccountNumber}");
                    Console.WriteLine($"Тип: {acc.Type}");
                    Console.WriteLine($"Валюта: {acc.Currency}");
                    Console.WriteLine($"Баланс: {acc.Balance}");
                    Console.WriteLine(new string('-', 30));
                }
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void ShowCards()
        {
            Console.Clear();
            Console.WriteLine("=== Мои карты ===");

            bool hasCards = false;
            foreach (var account in _user.Accounts)
            {
                if (account.Cards.Count > 0)
                {
                    hasCards = true;
                    Console.WriteLine($"\nСчет: {account.AccountNumber} ({account.Currency}, {account.Type})");
                    foreach (var card in account.Cards)
                    {
                        Console.WriteLine($"- Карта: {card.Number}");
                        Console.WriteLine($"  CVV: {card.Cvv}");
                        Console.WriteLine($"  Срок действия: {card.ExpirationDate:MM/yy}");
                    }
                }
            }

            if (!hasCards)
            {
                Console.WriteLine("У вас нет ни одной карты.");
            }

            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }

        private void TakeCreditMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Оформление заявки на кредит ===");

            if (_user.Accounts.Count == 0)
            {
                Console.WriteLine("Сначала создайте счет для зачисления кредита!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Выберите счет для зачисления кредита:");
            for (int i = 0; i < _user.Accounts.Count; i++)
            {
                var acc = _user.Accounts[i];
                Console.WriteLine($"{i + 1}. {acc.AccountNumber} ({acc.Currency}, {acc.Balance})");
            }
            Console.Write("Введите номер счета: ");
            if (!int.TryParse(Console.ReadLine(), out int accountIndex) || accountIndex < 1 || accountIndex > _user.Accounts.Count)
            {
                Console.WriteLine("Неверный выбор счета!");
                Console.ReadKey();
                return;
            }

            var account = _user.Accounts[accountIndex - 1];

            var availableCreditTypes = _creditTypeService.GetAllCreditTypes()
                .Where(c => c.Currency == account.Currency)
                .ToList();

            if (availableCreditTypes.Count == 0)
            {
                Console.WriteLine($"Нет доступных видов кредитов для валюты {account.Currency}. Обратитесь к администратору.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Выберите вид кредита:");
            for (int i = 0; i < availableCreditTypes.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableCreditTypes[i]}");
            }
            Console.Write("Введите номер вида кредита: ");
            if (!int.TryParse(Console.ReadLine(), out int typeIndex) || typeIndex < 1 || typeIndex > availableCreditTypes.Count)
            {
                Console.WriteLine("Неверный выбор!");
                Console.ReadKey();
                return;
            }

            var selectedType = availableCreditTypes[typeIndex - 1];

            Console.Write($"Сумма кредита ({selectedType.MinSum}-{selectedType.MaxSum}): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal sum) ||
                sum < selectedType.MinSum || sum > selectedType.MaxSum)
            {
                Console.WriteLine("Некорректная сумма!");
                Console.ReadKey();
                return;
            }

            Console.Write($"Срок кредита в месяцах ({selectedType.MinMonths}-{selectedType.MaxMonths}): ");
            if (!int.TryParse(Console.ReadLine(), out int months) ||
                months < selectedType.MinMonths || months > selectedType.MaxMonths)
            {
                Console.WriteLine("Некорректный срок!");
                Console.ReadKey();
                return;
            }

            var app = new CreditApplication(_user, selectedType, account.AccountNumber, sum, months);
            _creditApplicationService.SubmitApplication(app);

            Console.WriteLine($"\nЗаявка на кредит отправлена! Ожидайте одобрения сотрудником или администратором.");
            Console.ReadKey();
        }

        public List<Transaction> GetUserTransactions()
        {
            // Возвращаем копию списка, чтобы его нельзя было изменить извне
            return new List<Transaction>(_transactions);
        }
        private void ShowCreditsMenu()
        {
            Console.Clear();
            Console.WriteLine("=== Мои кредиты ===");
            if (_user.Credits.Count == 0)
            {
                Console.WriteLine("Кредитов нет.");
            }
            else
            {
                foreach (var credit in _user.Credits)
                {
                    Console.WriteLine($"Счет: {credit.AccountNumber}");
                    Console.WriteLine($"Сумма: {credit.Sum}");
                    Console.WriteLine($"Процент: {credit.Percent}%");
                    Console.WriteLine($"Срок: {credit.Months} мес.");
                    Console.WriteLine($"Статус: {credit.Status}");
                    Console.WriteLine($"К возврату: {credit.TotalLoan:F2}");
                    Console.WriteLine($"Уже погашено: {credit.PayedSum:F2}");
                    Console.WriteLine(new string('-', 30));
                }
            }
            Console.WriteLine("\nНажмите любую клавишу для возврата...");
            Console.ReadKey();
        }
    }
}