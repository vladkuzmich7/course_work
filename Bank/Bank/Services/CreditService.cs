using BankingLibrary.Models;
using System;

namespace BankingLibrary.Services
{
    public class CreditService
    {
        public Credit TakeCredit(User user, string targetAccountId, decimal sum, decimal percent, int months)
        {
            var targetAccount = user.Accounts.FirstOrDefault(a => a.Id == targetAccountId);
            if (targetAccount == null)
                throw new ArgumentException("Указанный счет не найден");

            if (sum <= 0)
                throw new ArgumentException("Сумма кредита должна быть положительной");

            if (percent < 0)
                throw new ArgumentException("Процентная ставка не может быть отрицательной");

            if (months <= 0)
                throw new ArgumentException("Срок кредита должен быть положительным");


            decimal totalLoan = sum * (1 + percent / 100 * months / 12);

            var credit = new Credit
            {
                Id = Guid.NewGuid().ToString(),
                Sum = sum,
                Percent = percent,
                Months = months,
                Status = "Active",
                TotalLoan = totalLoan,
                PayedSum = 0,
                Currency = targetAccount.Currency,
                AccountId = targetAccountId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(months)
            };


            targetAccount.Balance += sum;

            user.Credits.Add(credit);
            return credit;
        }

        public void CheckStatus(Credit credit)
        {
            Console.WriteLine($"Статус кредита {credit.Id}:");
            Console.WriteLine($"Общая сумма: {credit.TotalLoan}");
            Console.WriteLine($"Остаток к выплате: {credit.TotalLoan - credit.PayedSum}");
            Console.WriteLine($"Процентная ставка: {credit.Percent}%");
            Console.WriteLine($"Срок: {credit.Months} месяцев");
            Console.WriteLine($"Текущий статус: {credit.Status}");
        }
    }
}