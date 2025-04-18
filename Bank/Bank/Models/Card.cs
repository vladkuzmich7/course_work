namespace BankingLibrary.Models
{
    public class Card
    {
        public string Number { get; set; }
        public string Cvv { get; set; }
        public Account Account { get; set; }
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddYears(3); 

        public string GetMaskedNumber()
        {
            return $"**** **** **** {Number.Substring(15)}";
        }
    }
}