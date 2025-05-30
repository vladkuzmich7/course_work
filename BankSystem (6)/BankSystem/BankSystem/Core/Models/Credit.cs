using System;

namespace BankSystem.Core.Models
{
    public enum CreditStatus
    {
        Active,
        Paid,
        Overdue
    }

    public class Credit
    {
        public decimal Balance { get; set; }
        public decimal Amount { get; set; }
        public string AccountNumber { get; }
        public string Currency { get; set; } = string.Empty;
        public decimal Sum { get; }
        public decimal Percent { get; }
        public int Months { get; }
        public CreditStatus Status { get; private set; }
        public decimal TotalLoan { get; }
        public decimal PayedSum { get; private set; }

        public DateTime StartDate { get; set; }
        public DateTime? LastPaymentDate { get; private set; }

        public Credit(string accountNumber, decimal sum, decimal percent, int months)
        {
            if (sum <= 0) throw new ArgumentException("Сумма должна быть больше 0");
            if (percent <= 0) throw new ArgumentException("Процент должен быть больше 0");
            if (months <= 0) throw new ArgumentException("Срок должен быть больше 0");

            AccountNumber = accountNumber;
            Sum = sum;
            Percent = percent;
            Months = months;
            Status = CreditStatus.Active;
            PayedSum = 0;

            TotalLoan = Sum + (Sum * Percent / 100) * Months / 12;
            Balance = TotalLoan;
            StartDate = DateTime.Now;
            LastPaymentDate = null;
        }

        public void Repay(decimal payment)
        {
            if (payment <= 0)
                throw new ArgumentException("Сумма должна быть положительной");
            if (Balance <= 0)
                throw new InvalidOperationException("Кредит уже погашен");
            if (payment > Balance)
                payment = Balance;
            Balance -= payment;
            PayedSum += payment;
            LastPaymentDate = DateTime.Now;
            CheckStatus();
        }

        public void Pay(decimal payment)
        {
            if (Status != CreditStatus.Active)
                throw new InvalidOperationException("Кредит не активен");

            PayedSum += payment;
            LastPaymentDate = DateTime.Now;
            CheckStatus();
        }

        public void CheckStatus()
        {
            if (PayedSum >= TotalLoan || Balance <= 0)
            {
                Status = CreditStatus.Paid;
                PayedSum = TotalLoan;
                Balance = 0;
            }
            // Можно добавить условия для Overdue
        }

        public decimal GetMonthlyPayment()
        {
            if (Months <= 0)
                return 0;
            if (Status != CreditStatus.Active)
                return 0;
            var monthsLeft = GetMonthsLeft();
            if (monthsLeft <= 1)
                return Balance;
            return Math.Round(TotalLoan / Months, 2, MidpointRounding.AwayFromZero);
        }

        public int GetMonthsLeft()
        {
            var monthly = Months > 0 ? Math.Round(TotalLoan / Months, 2, MidpointRounding.AwayFromZero) : 0;
            if (monthly <= 0)
                return 0;
            return (int)Math.Ceiling(Balance / monthly);
        }

        // Новый метод для проверки просрочки
        public bool IsPaymentOverdue()
        {
            if (Status != CreditStatus.Active)
                return false;

            DateTime now = DateTime.Now;
            int monthsPassed = ((now.Year - StartDate.Year) * 12 + now.Month - StartDate.Month);
            if (monthsPassed >= Months)
                monthsPassed = Months - 1; // после последнего платежа

            // Ожидаемая дата последнего платежа за этот месяц
            DateTime expectedPaymentDate = StartDate.AddMonths(monthsPassed + 1);

            // Если последний платеж не был в этом месяце и уже истек срок платежа за текущий месяц
            if ((LastPaymentDate == null || LastPaymentDate.Value < StartDate.AddMonths(monthsPassed + 1 - 1)) &&
                now > expectedPaymentDate)
            {
                return true;
            }
            return false;
        }
    }
}