using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BankMvc.Models.Entity;

namespace BankMvc.DTO.ViewModels
{
    public class DashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public DateTime LastLoginDate { get; set; }
        public decimal TotalBalance { get; set; }
        public decimal MonthlyIncome { get; set; }
        public int MonthlyTransactions { get; set; }
        public List<AccountSummaryViewModel> Accounts { get; set; } = new List<AccountSummaryViewModel>();
        public List<TransactionSummaryViewModel> RecentTransactions { get; set; } = new List<TransactionSummaryViewModel>();
    }

    public class AccountSummaryViewModel
    {
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TransactionSummaryViewModel
    {
        public int TransactionId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty; // Credit or Debit
        public DateTime TransactionDate { get; set; }
        public int AccountId { get; set; }
    }

    public class TransferViewModel
    {
        [Required(ErrorMessage = "Source account is required")]
        public int FromAccountId { get; set; }

        [Required(ErrorMessage = "Destination account number is required")]
        [Display(Name = "To Account Number")]
        public string ToAccountNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Transfer Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
    }

    public class DepositViewModel
    {
        [Required(ErrorMessage = "Account is required")]
        public int AccountId { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Deposit Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Description")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; } = string.Empty;
    }
    //public class AllTransactionsViewModel
    //{
    //    public List<TransactionSummaryViewModel> Transactions { get; set; } = new List<TransactionSummaryViewModel>();
    //    public int TotalTransactions { get; set; }
    //    public List<Account> Accounts { get; set; } = new List<Account>();
    //    public decimal TotalAmount { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //}
}