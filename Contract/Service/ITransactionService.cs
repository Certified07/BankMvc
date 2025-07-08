using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMvc.Models.Entity;
using BankMvc.Models.Enum;
using BankMvc.Contract.Repository;

namespace BankMvc.Contract.Service
{
    public interface ITransactionService
    {
        Transaction GetTransactionById(int transactionId);
        List<Transaction> GetTransactionsByAccountId(int accountId);
        List<Transaction> GetTransactionsByAccountIdAndType(int accountId, TransactionType transactionType);
        List<Transaction> GetTransactionsByDateRange(int accountId, DateTime startDate, DateTime endDate);
        bool ProcessDeposit(int accountId, decimal amount, string description = null);
        bool ProcessWithdrawal(int accountId, decimal amount, string description = null);
        bool ProcessTransfer(int fromAccountId, int toAccountId, decimal amount, string description = null);
        decimal GetAccountBalance(int accountId);
        bool ValidateTransaction(Transaction transaction);
        List<Transaction> GetRecentTransactions(int accountId, int count = 10);
        decimal GetTotalTransactionAmount(int accountId, TransactionType transactionType, DateTime? startDate = null, DateTime? endDate = null);
    }
}