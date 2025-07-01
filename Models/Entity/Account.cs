using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BankMvc.Models.Enum;

namespace BankMvc.Models.Entity
{
    public class Account
    {
        public int AccountId { get; set; }
        public int UserId { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; } // Added this property to fix the error  
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public virtual User User { get; set; }
    }
}
