using System;
using System.Linq;
using System.Threading.Tasks;
using BankSystem.Core.Models;
using BankSystem.Core.Models.Users;

namespace BankSystem.Core.Services
{
    public class TransactionService
    {
        private readonly ExchangeRateService _exchangeRateService;

        public TransactionService(ExchangeRateService exchangeRateService)
        {
            _exchangeRateService = exchangeRateService;
        }

        public async Task<Transaction> CreateTransactionAsync(
            UserBase sender,
            string senderCardNumber,
            UserBase receiver,
            string receiverCardNumber,
            decimal amount,
            decimal commission)
        {
            var senderCard = sender.Accounts.SelectMany(a => a.Cards).FirstOrDefault(c => c.Number == senderCardNumber)
                ?? throw new InvalidOperationException("Карта отправителя не найдена");
            var receiverCard = receiver.Accounts.SelectMany(a => a.Cards).FirstOrDefault(c => c.Number == receiverCardNumber)
                ?? throw new InvalidOperationException("Карта получателя не найдена");

            string senderCurrency = senderCard.Account.Currency;
            string receiverCurrency = receiverCard.Account.Currency;

            decimal exchangeRate = 1m;
            decimal receiverAmount = amount;

            if (senderCurrency != receiverCurrency)
            {
                exchangeRate = await _exchangeRateService.GetExchangeRateAsync(senderCurrency, receiverCurrency);
                receiverAmount = Math.Round(amount * exchangeRate, 2);
            }

            var totalToWithdraw = amount + commission;
            if (senderCard.Account.Balance < totalToWithdraw)
                throw new InvalidOperationException("Недостаточно средств с учетом комиссии");

            senderCard.Account.Balance -= totalToWithdraw;
            receiverCard.Account.Balance += receiverAmount;

            return new Transaction(
                senderCurrency,
                amount,
                commission,
                senderCardNumber,
                receiverCardNumber,
                exchangeRate == 1m ? (decimal?)null : exchangeRate
            );
        }
    }
}