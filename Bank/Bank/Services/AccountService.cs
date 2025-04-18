using BankingLibrary.Models;
using System;
using System.Collections.Generic;

namespace BankingLibrary.Services
{
    public class AccountService
    {
        public Account CreateAccount(User user, string currency)
        {
            // Проверяем допустимость валюты
            var allowedCurrencies = new[] { "RUB", "USD", "EUR", "BUN" };
            if (!allowedCurrencies.Contains(currency.ToUpper()))
            {
                throw new ArgumentException("Недопустимая валюта счета");
            }

            var account = new Account
            {
                Id = GenerateAccountId(),
                Currency = currency.ToUpper(),
                Balance = 0
            };

            user.Accounts.Add(account);
            return account;
        }

        private string GenerateAccountId()
        {
            return Guid.NewGuid().ToString().Substring(0, 10).ToUpper();
        }

        public void AddCard(Account account, Card card)
        {
            account.Cards.Add(card);
        }
    }
}