using System;

namespace BankSystem.Core.Models
{

    public enum DepositStatus
    {
        Approved,   // Подтвержден
        Declined,   // Отклонен
        Active,     // Активен (если есть переходы)
        Closed
    }

    public class Deposit
    {
        public Guid Id { get; }
        public string AccountNumber { get; }
        public string DepositAccountNumber { get; } // <-- Номер депозитного счета
        public decimal Principal { get; private set; }
        public decimal InterestRate { get; }
        public DateTime OpenDate { get; }
        public DateTime? CloseDate { get; private set; }
        public DepositStatus Status { get; set; } = DepositStatus.Approved;
        public string Currency { get; }
        public decimal AccumulatedInterest { get; private set; }

        public bool Capitalization { get; } // Капитализация процентов
        public string InterestPayoutAccountNumber { get; } // Счет для выплаты процентов

        public Deposit(string depositAccountNumber, decimal principal, decimal interestRate, string currency, bool capitalization, string payoutAccountNumber = null)
        {
            Id = Guid.NewGuid();
            DepositAccountNumber = depositAccountNumber;
            Principal = principal;
            InterestRate = interestRate;
            Currency = currency.ToUpper();
            OpenDate = DateTime.Now;
            Status = DepositStatus.Active;
            AccumulatedInterest = 0;
            Capitalization = capitalization;
            InterestPayoutAccountNumber = payoutAccountNumber;
        }

        // Проценты начисляются раз в месяц
        public void AccumulateInterest(Dictionary<string, Account> userAccounts)
        {
            if (Status != DepositStatus.Active) throw new InvalidOperationException("Вклад не активен");
            var interest = Principal * (InterestRate / 100m) / 12m;
            if (Capitalization)
            {
                Principal += interest;
            }
            else
            {
                if (!string.IsNullOrEmpty(InterestPayoutAccountNumber) && userAccounts.TryGetValue(InterestPayoutAccountNumber, out var payoutAccount))
                {
                    payoutAccount.Balance += interest;
                }
                else
                {
                    throw new InvalidOperationException("Счет для выплаты процентов не найден");
                }
            }
            AccumulatedInterest += interest;
        }
        public void Close()
        {
            Status = DepositStatus.Closed;
            CloseDate = DateTime.Now;
        }

        // Начисление процентов (например, раз в месяц)
        public void AccumulateInterest(int months = 1)
        {
            if (Status != DepositStatus.Active)
                throw new InvalidOperationException("Вклад не активен");
            var interest = Principal * (InterestRate / 100) * months / 12;
            AccumulatedInterest += interest;
        }

        // Снятие (полное или частичное)
        public void Withdraw(decimal amount)
        {
            if (Status != DepositStatus.Active)
                throw new InvalidOperationException("Вклад не активен");
            if (amount > Principal)
                throw new InvalidOperationException("Недостаточно средств на вкладе");
            Principal -= amount;
        }

        // Перевод средств на другой счет (снимает и закрывает вклад)
        public decimal TransferAndClose()
        {
            if (Status != DepositStatus.Active)
                throw new InvalidOperationException("Вклад не активен");
            CloseDate = DateTime.Now;
            Status = DepositStatus.Closed;
            var total = Principal + AccumulatedInterest;
            Principal = 0;
            AccumulatedInterest = 0;
            return total;
        }
    }
}