namespace BankingLibrary.Models
{
    public class Account
    {
        public string Id { get; set; }
        public User User { get; set; }
        public string Currency { get; set; }
        public decimal Balance { get; set; }
        public List<Card> Cards { get; set; } = new List<Card>();
    }
}