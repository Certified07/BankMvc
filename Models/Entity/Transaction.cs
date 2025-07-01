using BankMvc.Models.Enum;

namespace BankMvc.Models.Entity
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public int? ToAccountId { get; set; }  // Nullable for non-transfer transactions
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
