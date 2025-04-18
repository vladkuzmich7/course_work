namespace BankingLibrary.Models
{
    public class User
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassportId { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<Account> Accounts { get; set; } = new List<Account>();
        public List<Card> Cards { get; set; } = new List<Card>();
        public List<Credit> Credits { get; set; } = new List<Credit>();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}