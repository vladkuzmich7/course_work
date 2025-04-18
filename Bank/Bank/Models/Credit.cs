
namespace BankingLibrary.Models
{
    public class Credit
    {
        public string Id { get; set; }
        public decimal Sum { get; set; }
        public decimal Percent { get; set; }
        public int Months { get; set; }
        public string Status { get; set; }
        public decimal TotalLoan { get; set; }
        public decimal PayedSum { get; set; }
        public string Currency { get; set; }
        public string AccountId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}