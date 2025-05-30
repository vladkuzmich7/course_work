using LiteDB;
using System.Collections.Generic;

namespace BankSystem.Core.Models
{
    public enum AccountType
    {
        Checking,      // Текущий
        Savings,       // Накопительный
        Deposit,       // Депозитный
        Currency,      // Валютный
        Settlement     // Расчетный
    }

    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
        public AccountType Type { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
        public Account() { }
        public Account(string accountNumber, string currency, AccountType type)
        {
            AccountNumber = accountNumber;
            Currency = currency;
            Type = type;
            Balance = 0m;
        }


        public void AddCard(Card card)
        {
            Cards.Add(card);
        }

        public void RemoveCard(string cardNumber)
        {
            Cards.RemoveAll(c => c.Number == cardNumber);
        }
        // Запрет на снятие и перевод для депозитного счета
        public bool CanWithdrawOrTransfer()
        {
            return Type != AccountType.Deposit;
        }

    }
}