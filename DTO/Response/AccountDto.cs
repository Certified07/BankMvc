using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BankMvc.Models.Enum;

namespace BankMvc.DTO.Response
{
    public class AccountDto
    {
        public int AccountId { get; set; }
        
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Account number is required")]
        [StringLength(20, ErrorMessage = "Account number cannot exceed 20 characters")]
        public string AccountNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Account type is required")]
        public AccountType Type { get; set; }
        
        [Required(ErrorMessage = "Balance is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be non-negative")]
        public decimal Balance { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}