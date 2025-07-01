using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BankMvc.DTO.Request
{
   public class UpdateAccountDto
    {
        [Required(ErrorMessage = "Account ID is required")]
        public int AccountId { get; set; }
        
        [Required(ErrorMessage = "Balance is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Balance must be non-negative")]
        public decimal Balance { get; set; }
    }
}