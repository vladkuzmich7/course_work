using System;

namespace BankSystem.Core.Models
{
    public class Transaction
    {
        public Guid Id { get; }
        public string Currency { get; }
        public decimal Amount { get; }
        public decimal Commission { get; }
        public string SenderCardNumber { get; }
        public string ReceiverCardNumber { get; }
        public DateTime Date { get; }
        public decimal? ExchangeRate { get; }

        public Transaction(
            string currency,
            decimal amount,
            decimal commission,
            string senderCard,
            string receiverCard,
            decimal? exchangeRate = null)
        {
            Id = Guid.NewGuid();
            Currency = currency;
            Amount = amount;
            Commission = commission;
            SenderCardNumber = senderCard;
            ReceiverCardNumber = receiverCard;
            ExchangeRate = exchangeRate;
            Date = DateTime.Now;
        }

        public void ShowInfo()
        {
            Console.WriteLine($"Транзакция: {Id}");
            Console.WriteLine($"Сумма: {Amount} {Currency}");
            Console.WriteLine($"Комиссия: {Commission} {Currency}");
            Console.WriteLine($"Отправитель (карта): {SenderCardNumber}");
            Console.WriteLine($"Получатель (карта): {ReceiverCardNumber}");
            Console.WriteLine($"Дата: {Date:dd.MM.yyyy HH:mm}");
            if (ExchangeRate != null)
                Console.WriteLine($"Курс конвертации: {ExchangeRate:F4}");
        }
    }
}