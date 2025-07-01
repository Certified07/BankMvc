using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using BankMvc.Models.Enum;

namespace BankMvc.DTO.Request
{
    public class CreateAccountDto
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }
        
        [Required(ErrorMessage = "Account number is required")]
        [StringLength(20, ErrorMessage = "Account number cannot exceed 20 characters")]
        public string AccountNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Account type is required")]
        public string Type { get; set; }
        
        [Required(ErrorMessage = "Initial balance is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be non-negative")]
        public decimal InitialBalance { get; set; }
        public decimal Balance { get; set; }
    }
}