using System;

namespace BankSystem.Core.Models
{
    public class DepositTemplate
    {
        public Guid Id { get; }
        public string Name { get; }
        public int MinMonths { get; }
        public int MaxMonths { get; }
        public decimal InterestRate { get; }
        public string Currency { get; }
        public decimal MinSum { get; }
        public string Description { get; }

        public DepositTemplate(string name, int minMonths, int maxMonths, decimal interestRate, string currency, decimal minSum, string description = "")
        {
            Id = Guid.NewGuid();
            Name = name;
            MinMonths = minMonths;
            MaxMonths = maxMonths;
            InterestRate = interestRate;
            Currency = currency.ToUpper();
            MinSum = minSum;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Name} | {Currency} | {InterestRate}% | {MinMonths}-{MaxMonths} мес. | от {MinSum} | {Description}";
        }
    }
}