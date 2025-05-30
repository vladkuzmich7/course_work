namespace BankSystem.Core.Models
{

    public class CreditType
    {
        public string Name { get; }
        public decimal DefaultPercent { get; }
        public int MinMonths { get; }
        public int MaxMonths { get; }
        public decimal MinSum { get; }
        public decimal MaxSum { get; }
        public string Currency { get; } // <--- Новое поле

        public CreditType(string name, decimal defaultPercent, int minMonths, int maxMonths, decimal minSum, decimal maxSum, string currency)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentNullException(nameof(currency));
            Name = name;
            DefaultPercent = defaultPercent;
            MinMonths = minMonths;
            MaxMonths = maxMonths;
            MinSum = minSum;
            MaxSum = maxSum;
            Currency = currency.ToUpper();
        }

        public override string ToString()
        {
            return $"{Name} (валюта: {Currency}, ставка: {DefaultPercent}%, срок: {MinMonths}-{MaxMonths} мес., сумма: {MinSum}-{MaxSum})";
        }
    }
}