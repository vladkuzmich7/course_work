using BankingLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankingLibrary.Services
{
    public class TransactionService
    {
        private readonly CardService _cardService;
        private readonly Dictionary<string, decimal> _exchangeRates = new Dictionary<string, decimal>
        {
            {"RUB/USD", 0.013m},
            {"RUB/EUR", 0.011m},
            {"RUB/BUN", 0.036695m},
            {"USD/RUB", 75.0m},
            {"USD/EUR", 0.85m},
            {"USD/BUN", 3.08m},
            {"EUR/RUB", 90.0m},
            {"EUR/USD", 1.18m},
            {"EUR/BUN", 3.51m},
            {"BUN/RUB", 27.25m},
            {"BUN/EUR", 0.29m},
            {"BUN/USD", 0.32m}
        };

        public TransactionService(CardService cardService)
        {
            _cardService = cardService;
        }

        public Transaction CreateCardTransaction(
            List<User> allUsers,
            string fromCardNumber,
            string toCardNumber,
            decimal amount)
        {
            var fromCard = _cardService.FindCardByNumber(fromCardNumber, allUsers);
            var toCard = _cardService.FindCardByNumber(toCardNumber, allUsers);

            if (fromCard == null || toCard == null)
                throw new Exception("Одна из карт не найдена");

            if (fromCard.Account.Balance < amount)
                throw new Exception("Недостаточно средств на карте");

            decimal convertedAmount = amount;
            if (fromCard.Account.Currency != toCard.Account.Currency)
            {
                string exchangeKey = $"{fromCard.Account.Currency}/{toCard.Account.Currency}";
                if (!_exchangeRates.ContainsKey(exchangeKey))
                    throw new Exception("Конвертация между указанными валютами невозможна");

                convertedAmount = amount * _exchangeRates[exchangeKey];
            }

            fromCard.Account.Balance -= amount;
            toCard.Account.Balance += convertedAmount;

            var transaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                ConvertedAmount = convertedAmount,
                Currency = fromCard.Account.Currency,
                TargetCurrency = toCard.Account.Currency,
                FromCard = fromCardNumber,
                ToCard = toCardNumber,
                Date = DateTime.Now,
                Status = "Completed",
                ExchangeRate = fromCard.Account.Currency == toCard.Account.Currency
                    ? 1m
                    : _exchangeRates[$"{fromCard.Account.Currency}/{toCard.Account.Currency}"]
            };

            var fromUser = allUsers.First(u => u.Cards.Contains(fromCard));
            var toUser = allUsers.First(u => u.Cards.Contains(toCard));

            fromUser.Transactions.Add(transaction);
            toUser.Transactions.Add(transaction);

            return transaction;
        }

        public void DisplayTransactionDetails(Transaction transaction)
        {
            Console.WriteLine($"\nДетали транзакции:");
            Console.WriteLine($"ID: {transaction.Id}");
            Console.WriteLine($"Дата: {transaction.Date:g}");
            Console.WriteLine($"Отправлено с карты: {transaction.FromCard}");
            Console.WriteLine($"Получено на карту: {transaction.ToCard}");
            Console.WriteLine($"Сумма: {transaction.Amount} {transaction.Currency}");

            if (transaction.Currency != transaction.TargetCurrency)
            {
                Console.WriteLine($"Конвертировано в: {transaction.ConvertedAmount} {transaction.TargetCurrency}");
                Console.WriteLine($"Курс: 1 {transaction.Currency} = {transaction.ExchangeRate} {transaction.TargetCurrency}");
            }

            Console.WriteLine($"Статус: {transaction.Status}");
        }
        public Transaction CreateTransaction(User user, decimal amount, string currency, string receiverId, Account senderAccount)
        {
            if (senderAccount.Balance < amount)
            {
                throw new InvalidOperationException("Недостаточно средств на счете");
            }

            var transaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                Currency = currency,
                Amount = amount,
                SenderId = senderAccount.Id,
                ReceiverId = receiverId,
                Date = DateTime.Now
            };

            senderAccount.Balance -= amount;
            user.Transactions.Add(transaction);

            return transaction;
        }
        public void ShowTransactionsHistory(User user)
        {
            Console.WriteLine("История транзакций:");
            foreach (var transaction in user.Transactions)
            {
                Console.WriteLine($"\nID: {transaction.Id}");
                Console.WriteLine($"Дата: {transaction.Date:g}");
                Console.WriteLine($"С карты: {transaction.FromCard}");
                Console.WriteLine($"На карту: {transaction.ToCard}");
                Console.WriteLine($"Сумма: {transaction.Amount} {transaction.Currency}");

                if (transaction.Currency != transaction.TargetCurrency)
                {
                    Console.WriteLine($"Конвертировано в: {transaction.ConvertedAmount} {transaction.TargetCurrency}");
                }

                Console.WriteLine($"Статус: {transaction.Status}");
            }
        }
    }
}