namespace BankingLibrary.Models
{
    public class Transaction
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public string Currency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public string FromCard { get; set; }
        public string ToCard { get; set; }
        public DateTime Date { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Status { get; set; }
    }
}