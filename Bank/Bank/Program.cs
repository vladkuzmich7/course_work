using BankingLibrary.Models;
using BankingLibrary.Services;
using System;
using System.Collections.Generic;

namespace BankingApp
{
    class Program
    {
        private static User _currentUser;
        private static List<User> _users = new List<User>();
        private static AccountService _accountService = new AccountService();
        private static CardService _cardService = new CardService();
        private static CreditService _creditService = new CreditService();
        private static TransactionService _transactionService = new TransactionService(_cardService); 
        private static AuthService _authService = new AuthService();

        static void Main(string[] args)
        {
            InitializeSampleData();

            bool exit = false;
            while (!exit)
            {
                Console.Clear();

                if (_currentUser == null)
                {
                    DisplayAuthMenu(ref exit);
                }
                else
                {
                    DisplayMainMenu(ref exit);
                }
            }
        }
        static void CreateCardTransactionMenu(List<User> allUsers)
        {
            Console.Clear();
            Console.WriteLine("=== ПЕРЕВОД С КАРТЫ НА КАРТУ ===");

            if (_currentUser.Cards.Count == 0)
            {
                Console.WriteLine("У вас нет карт для перевода.");
                Console.ReadKey();
                return;
            }

            // Выбор карты отправителя
            Console.WriteLine("\nВаши карты:");
            for (int i = 0; i < _currentUser.Cards.Count; i++)
            {
                var card = _currentUser.Cards[i];
                Console.WriteLine($"{i + 1}. {card.Number} ({card.Account.Currency} {card.Account.Balance})");
            }

            Console.Write("Выберите карту для списания: ");
            if (!int.TryParse(Console.ReadLine(), out int fromIndex) || fromIndex < 1 || fromIndex > _currentUser.Cards.Count)
            {
                Console.WriteLine("Неверный выбор карты!");
                Console.ReadKey();
                return;
            }

            var fromCard = _currentUser.Cards[fromIndex - 1];

            // Ввод данных получателя
            Console.Write("\nВведите номер карты получателя: ");
            string toCardNumber = Console.ReadLine().Trim();

            Console.Write("Введите сумму: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal amount) || amount <= 0)
            {
                Console.WriteLine("Неверная сумма!");
                Console.ReadKey();
                return;
            }

            try
            {
                var transaction = _transactionService.CreateCardTransaction(
                    allUsers,
                    fromCard.Number,
                    toCardNumber,
                    amount);

                Console.WriteLine("\nПеревод выполнен успешно!");
                _transactionService.DisplayTransactionDetails(transaction);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка перевода: {ex.Message}");
            }

            Console.ReadKey();
        }
        static void InitializeSampleData()
        {
            var sampleUser = new User
            {
                FirstName = "Иван",
                LastName = "Иванов",
                Email = "ivan@example.com",
                Login = "ivanov",
                Password = "securepassword"
            };
            _users.Add(sampleUser);
            _accountService.CreateAccount(sampleUser, "RUB");
        }

        static void DisplayAuthMenu(ref bool exit)
        {
            Console.WriteLine("=== БАНКОВСКАЯ СИСТЕМА ===");
            Console.WriteLine("1. Вход");
            Console.WriteLine("2. Регистрация");
            Console.WriteLine("3. Выход");
            Console.Write("Выберите действие: ");

            switch (Console.ReadLine())
            {
                case "1": Login(); break;
                case "2": Register(); break;
                case "3": exit = true; break;
                default:
                    Console.WriteLine("Неверный ввод!");
                    Console.ReadKey();
                    break;
            }
        }

        static void DisplayMainMenu(ref bool exit)
        {
            Console.WriteLine($"=== БАНКОВСКАЯ СИСТЕМА ===");
            Console.WriteLine($"Пользователь: {_currentUser.FirstName} {_currentUser.LastName}");
            Console.WriteLine("\n1. Создать счет\n2. Создать карту\n3. Взять кредит\n4. Перевод\n5. История операций\n6. Мои счета\n7. Мои кредиты\n8. Мой профиль\n9. Мои карты\n10. Выйти");
            Console.Write("Выберите действие: ");

            switch (Console.ReadLine())
            {
                case "1": CreateAccountMenu(); break;
                case "2": CreateCardMenu(); break;
                case "3": TakeCreditMenu(); break;
                case "4": CreateCardTransactionMenu(_users); break;
                case "5": ShowTransactionsHistory(); break;
                case "6": ShowAccountInfo(); break;
                case "7": CheckCreditStatus(); break;
                case "8":
                    DisplayUserMenu();
                    break;
                case "9":
                    ShowUserCards();
                    break;
                case "10":
                    _currentUser = null;
                    break;
                default:
                    Console.WriteLine("Неверный ввод!");
                    Console.ReadKey();
                    break;
            }
        }

        static void Login()
        {
            Console.Clear();
            Console.Write("Логин: ");
            string login = Console.ReadLine();

            Console.Write("Пароль: ");
            string password = Console.ReadLine();

            _currentUser = _authService.Authenticate(login, password, _users);

            if (_currentUser == null)
            {
                Console.WriteLine("\nОшибка авторизации!");
                Console.ReadKey();
            }
        }

        static void Register()
        {
            Console.Clear();
            var newUser = new User();

            Console.Write("Имя: ");
            newUser.FirstName = Console.ReadLine();

            Console.Write("Фамилия: ");
            newUser.LastName = Console.ReadLine();

            Console.Write("Email: ");
            newUser.Email = Console.ReadLine();

            Console.Write("Логин: ");
            newUser.Login = Console.ReadLine();

            Console.Write("Пароль: ");
            newUser.Password = Console.ReadLine();

            if (_authService.IsLoginExists(newUser.Login, _users))
            {
                Console.WriteLine("\nЛогин занят!");
                Console.ReadKey();
                return;
            }

            _users.Add(newUser);
            _currentUser = newUser;
            _accountService.CreateAccount(newUser, "RUB");

            Console.WriteLine("\nРегистрация успешна!");
            Console.ReadKey();
        }

        static void CreateAccountMenu()
        {
            Console.Clear();
            Console.WriteLine("=== СОЗДАНИЕ НОВОГО СЧЕТА ===");
            Console.WriteLine("Доступные валюты: RUB, USD, EUR, BUN");

            string currency;
            bool isValidCurrency;

            do
            {
                Console.Write("Введите валюту счета: ");
                currency = Console.ReadLine().ToUpper();

                isValidCurrency = currency == "RUB" || currency == "USD" ||
                                 currency == "EUR" || currency == "BUN";

                if (!isValidCurrency)
                {
                    Console.WriteLine("Ошибка: допустимые валюты - RUB, USD, EUR, BUN");
                    Console.WriteLine("Пожалуйста, введите корректную валюту.");
                }
            }
            while (!isValidCurrency);

            try
            {
                var account = _accountService.CreateAccount(_currentUser, currency);
                Console.WriteLine($"\nСчет успешно создан!");
                Console.WriteLine($"ID: {account.Id}");
                Console.WriteLine($"Валюта: {account.Currency}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка при создании счета: {ex.Message}");
            }

            Console.ReadKey();
        }

        static void CreateCardMenu()
        {
            Console.Clear();
            if (_currentUser.Accounts.Count == 0)
            {
                Console.WriteLine("Нет доступных счетов!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Выберите счет:");
            for (int i = 0; i < _currentUser.Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {_currentUser.Accounts[i].Id} ({_currentUser.Accounts[i].Currency})");

            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= _currentUser.Accounts.Count)
            {
                var card = _cardService.CreateCard(_currentUser, _currentUser.Accounts[index - 1]);
                Console.WriteLine($"\nКарта {card.Number} создана!");
            }
            else
            {
                Console.WriteLine("Неверный выбор!");
            }
            Console.ReadKey();
        }

        static void TakeCreditMenu()
        {
            Console.Clear();
            Console.WriteLine("=== ОФОРМЛЕНИЕ КРЕДИТА ===");

            if (_currentUser.Accounts.Count == 0)
            {
                Console.WriteLine("У вас нет открытых счетов. Сначала создайте счет.");
                Console.ReadKey();
                return;
            }

            // Показываем список счетов с подробной информацией
            Console.WriteLine("Доступные счета:");
            Console.WriteLine("ID счета".PadRight(15) + "Валюта".PadRight(10) + "Баланс");
            foreach (var account in _currentUser.Accounts)
            {
                Console.WriteLine($"{account.Id.PadRight(15)}{account.Currency.PadRight(10)}{account.Balance}");
            }

            Console.Write("\nВведите ID счета для зачисления средств: ");
            string accountId = Console.ReadLine();

            Console.Write("Введите сумму кредита: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal sum) || sum <= 0)
            {
                Console.WriteLine("Неверная сумма кредита!");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите процентную ставку (годовых): ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal percent) || percent < 0)
            {
                Console.WriteLine("Неверная процентная ставка!");
                Console.ReadKey();
                return;
            }

            Console.Write("Введите срок кредита (месяцев): ");
            if (!int.TryParse(Console.ReadLine(), out int months) || months <= 0)
            {
                Console.WriteLine("Неверный срок кредита!");
                Console.ReadKey();
                return;
            }

            try
            {
                var credit = _creditService.TakeCredit(_currentUser, accountId, sum, percent, months);

                Console.WriteLine("\nКредит успешно оформлен!");
                Console.WriteLine($"Сумма: {sum} {credit.Currency}");
                Console.WriteLine($"Счет зачисления: {accountId}");
                Console.WriteLine($"Процентная ставка: {percent}% годовых");
                Console.WriteLine($"Срок: {months} месяцев");
                Console.WriteLine($"Дата погашения: {credit.EndDate:dd.MM.yyyy}");
                Console.WriteLine($"Общая сумма к возврату: {credit.TotalLoan:F2} {credit.Currency}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            Console.ReadKey();
        }

        static void CreateTransactionMenu()
        {
            Console.Clear();
            if (_currentUser.Accounts.Count == 0)
            {
                Console.WriteLine("Нет доступных счетов!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Выберите счет списания:");
            for (int i = 0; i < _currentUser.Accounts.Count; i++)
                Console.WriteLine($"{i + 1}. {_currentUser.Accounts[i].Id} ({_currentUser.Accounts[i].Balance} {_currentUser.Accounts[i].Currency})");

            if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > _currentUser.Accounts.Count)
            {
                Console.WriteLine("Неверный выбор!");
                Console.ReadKey();
                return;
            }

            var account = _currentUser.Accounts[index - 1];

            Console.Write("Сумма: ");
            decimal amount = decimal.Parse(Console.ReadLine());

            Console.Write("ID получателя: ");
            string receiverId = Console.ReadLine();

            try
            {
                var transaction = _transactionService.CreateTransaction(_currentUser, amount, account.Currency, receiverId, account);
                Console.WriteLine($"\nПеревод {transaction.Id} выполнен!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nОшибка: {ex.Message}");
            }
            Console.ReadKey();
        }

        static void ShowTransactionsHistory()
        {
            Console.Clear();
            if (_currentUser.Transactions.Count == 0)
            {
                Console.WriteLine("История операций пуста");
            }
            else
            {
                foreach (var t in _currentUser.Transactions)
                {
                    Console.WriteLine($"{t.Date} | {t.Amount} {t.Currency} | Счет получателя: {t.ReceiverId}");
                }
            }
            Console.ReadKey();
        }

        static void ShowAccountInfo()
        {
            Console.Clear();
            if (_currentUser.Accounts.Count == 0)
            {
                Console.WriteLine("Нет счетов");
            }
            else
            {
                foreach (var acc in _currentUser.Accounts)
                {
                    Console.WriteLine($"\nСчет: {acc.Id} | Баланс: {acc.Balance} {acc.Currency}");
                    if (acc.Cards.Count > 0)
                    {
                        Console.WriteLine("Привязанные карты:");
                        foreach (var card in acc.Cards)
                            Console.WriteLine($"* {card.Number} (CVV: {card.Cvv})");
                    }
                }
            }
            Console.ReadKey();
        }

        static void CheckCreditStatus()
        {
            Console.Clear();
            if (_currentUser.Credits.Count == 0)
            {
                Console.WriteLine("Нет активных кредитов");
            }
            else
            {
                foreach (var cr in _currentUser.Credits)
                {
                    Console.WriteLine($"\nКредит {cr.Id}:");
                    Console.WriteLine($"Сумма: {cr.Sum} {cr.Currency}");
                    Console.WriteLine($"Остаток: {cr.TotalLoan - cr.PayedSum} {cr.Currency}");
                    Console.WriteLine($"Ставка: {cr.Percent}% | Срок: {cr.Months} мес.");
                }
            }
            Console.ReadKey();
        }

        static void DisplayUserMenu()
        {
            Console.Clear();
            Console.WriteLine("=== МОЙ ПРОФИЛЬ ===");
            Console.WriteLine($"1. Имя: {_currentUser.FirstName}");
            Console.WriteLine($"2. Фамилия: {_currentUser.LastName}");
            Console.WriteLine($"3. Email: {_currentUser.Email}");
            Console.WriteLine($"4. Сменить пароль");
            Console.WriteLine($"5. Назад в главное меню");
            Console.Write("Выберите действие: ");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Write("Новое имя: ");
                    _currentUser.FirstName = Console.ReadLine();
                    Console.WriteLine("Имя изменено!");
                    break;
                case "2":
                    Console.Write("Новая фамилия: ");
                    _currentUser.LastName = Console.ReadLine();
                    Console.WriteLine("Фамилия изменена!");
                    break;
                case "3":
                    Console.Write("Новый email: ");
                    _currentUser.Email = Console.ReadLine();
                    Console.WriteLine("Email изменен!");
                    break;
                case "4":
                    ChangePasswordMenu();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Неверный ввод!");
                    break;
            }
            Console.ReadKey();
        }
        static void ShowUserCards()
        {
            Console.Clear();
            _cardService.DisplayUserCards(_currentUser);
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        static void ChangePasswordMenu()
        {
            Console.Clear();
            Console.WriteLine("=== СМЕНА ПАРОЛЯ ===");

            Console.Write("Текущий пароль: ");
            string current = Console.ReadLine();

            Console.Write("Новый пароль: ");
            string newPass = Console.ReadLine();

            if (_authService.ChangePassword(_currentUser, current, newPass))
            {
                Console.WriteLine("Пароль успешно изменен!");
            }
            else
            {
                Console.WriteLine("Неверный текущий пароль!");
            }
        }
    }
}