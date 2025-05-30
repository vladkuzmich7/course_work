namespace BankSystem.Core.Models
{
    public class Card
    {
        public string Number { get; }
        public string Cvv { get; }
        public DateTime ExpirationDate { get; }
        public Account Account { get; private set; }

        public Card(string number, string cvv, DateTime expirationDate, Account account)
        {
            Number = number;
            Cvv = cvv;
            ExpirationDate = expirationDate;
            Account = account;
        }

        public void ChangeAccount(Account newAccount)
        {
            if (newAccount == null)
                throw new ArgumentNullException(nameof(newAccount));

            Account = newAccount;
        }

        public override string ToString()
        {
            return $"Карта {Number} (Срок: {ExpirationDate:MM/yy}, CVV: {Cvv})";
        }
    }
}