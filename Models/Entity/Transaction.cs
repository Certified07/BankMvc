//using BankMvc.Models.Enum;

//namespace BankMvc.Models.Entity
//{
//    public class Transaction
//    {
//        public int TransactionId { get; set; }
//        public int AccountId { get; set; }
//        public int? ToAccountId { get; set; }  // Nullable for non-transfer transactions
//        public string TransactionType  { get; set; }
//        public decimal Amount { get; set; }
//        public string Description { get; set; }
//        public DateTime TransactionDate { get; set; }
//    }
//}

using System;
using System.ComponentModel.DataAnnotations;
using BankMvc.Models.Enum;

namespace BankMvc.Models.Entity
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        public DateTime TransactionDate { get; set; }

        public string Description { get; set; }

        public string ReferenceNumber { get; set; }

        public  Account Account { get; set; }
    }
}