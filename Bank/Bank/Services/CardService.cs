using BankingLibrary.Models;
using System;
using System.Collections.Generic;

namespace BankingLibrary.Services
{
    public class CardService
    {
        private readonly Random _random = new Random();

        public Card CreateCard(User user, Account account)
        {
            var card = new Card
            {
                Number = GenerateCardNumber(),
                Cvv = GenerateCvv(),
                Account = account,
                ExpiryDate = DateTime.Now.AddYears(3)
            };
            user.Cards.Add(card);

            Console.WriteLine($"Карта успешно создана!");
            Console.WriteLine($"Номер: {card.GetMaskedNumber()}");
            Console.WriteLine($"Срок действия: {card.ExpiryDate:MM/yy}");

            return card;
        }
        public Card FindCardByNumber(string cardNumber, List<User> allUsers)
        {
            return allUsers.SelectMany(u => u.Cards)
                          .FirstOrDefault(c => c.Number == cardNumber);
        }
        public void ChangeAccount(Card card, Account newAccount)
        {
            card.Account.Cards.Remove(card);
            card.Account = newAccount;
            newAccount.Cards.Add(card);
        }

        private string GenerateCardNumber()
        {
            return $"{_random.Next(1000, 9999)} {_random.Next(1000, 9999)} {_random.Next(1000, 9999)} {_random.Next(1000, 9999)}";
        }

        private string GenerateCvv()
        {
            return _random.Next(100, 999).ToString();
        }
        public void DisplayUserCards(User user)
        {
            if (user.Cards.Count == 0)
            {
                Console.WriteLine("У вас нет созданных карт.");
                return;
            }

            Console.WriteLine("=== ВАШИ КАРТЫ ===");
            foreach (var card in user.Cards)
            {
                Console.WriteLine($"\nНомер карты: {card.Number}");
                Console.WriteLine($"CVV: {card.Cvv}");
                Console.WriteLine($"Привязанный счет: {card.Account.Id}");
                Console.WriteLine($"Валюта счета: {card.Account.Currency}");
                Console.WriteLine($"Баланс: {card.Account.Balance}");
            }
        }
    }
}