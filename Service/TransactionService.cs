using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMvc.Models.Entity;
using BankMvc.Models.Enum;
using BankMvc.Contract.Repository;
using BankMvc.Contract.Service;

namespace BankMvc.Service
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
        }

        public Transaction GetTransactionById(int transactionId)
        {
            if (transactionId <= 0)
                throw new ArgumentException("Transaction ID must be greater than zero.", nameof(transactionId));

            return _transactionRepository.GetById(transactionId);
        }

        public List<Transaction> GetTransactionsByAccountId(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than zero.", nameof(accountId));

            return _transactionRepository.GetByAccountId(accountId);
        }

        public List<Transaction> GetTransactionsByAccountIdAndType(int accountId, TransactionType transactionType)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than zero.", nameof(accountId));

            return _transactionRepository.GetByAccountIdAndType(accountId, transactionType);
        }

        public List<Transaction> GetTransactionsByDateRange(int accountId, DateTime startDate, DateTime endDate)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than zero.", nameof(accountId));

            if (startDate > endDate)
                throw new ArgumentException("Start date must be before or equal to end date.");

            var transactions = _transactionRepository.GetByAccountId(accountId);
            return transactions.Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate).ToList();
        }

        public bool ProcessDeposit(int accountId, decimal amount, string description = null)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than zero.", nameof(accountId));

            if (amount <= 0)
                throw new ArgumentException("Deposit amount must be greater than zero.", nameof(amount));

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                TransactionType = TransactionType.Deposit,
                TransactionDate = DateTime.Now,
                Description = description ?? "Deposit"
            };

            if (!ValidateTransaction(transaction))
                return false;

            return _transactionRepository.Create(transaction);
        }

        public bool ProcessWithdrawal(int accountId, decimal amount, string description = null)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than zero.", nameof(accountId));

            if (amount <= 0)
                throw new ArgumentException("Withdrawal amount must be greater than zero.", nameof(amount));

            // Check if account has sufficient balance
            var currentBalance = GetAccountBalance(accountId);
            if (amount > currentBalance)
                throw new InvalidOperationException("Insufficient funds for withdrawal.");

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                TransactionType = TransactionType.Withdrawal,
                TransactionDate = DateTime.Now,
                Description = description ?? "Withdrawal"
            };

            if (!ValidateTransaction(transaction))
                return false;

            return _transactionRepository.Create(transaction);
        }

        public bool ProcessTransfer(int fromAccountId, int toAccountId, decimal amount, string description = null)
        {
            if (fromAccountId <= 0)
                throw new ArgumentException("From Account ID must be greater than zero.", nameof(fromAccountId));

            if (toAccountId <= 0)
                throw new ArgumentException("To Account ID must be greater than zero.", nameof(toAccountId));

            if (fromAccountId == toAccountId)
                throw new ArgumentException("Cannot transfer to the same account.");

            if (amount <= 0)
                throw new ArgumentException("Transfer amount must be greater than zero.", nameof(amount));

            // Check if source account has sufficient balance
            var fromAccountBalance = GetAccountBalance(fromAccountId);
            if (amount > fromAccountBalance)
                throw new InvalidOperationException("Insufficient funds for transfer.");

            // Create withdrawal transaction for source account
            var withdrawalTransaction = new Transaction
            {
                AccountId = fromAccountId,
                Amount = amount,
                TransactionType = TransactionType.Transfer,
                TransactionDate = DateTime.Now,
                Description = description ?? $"Transfer to Account {toAccountId}"
            };

            // Create deposit transaction for destination account
            var depositTransaction = new Transaction
            {
                AccountId = toAccountId,
                Amount = amount,
                TransactionType = TransactionType.Transfer,
                TransactionDate = DateTime.Now,
                Description = description ?? $"Transfer from Account {fromAccountId}"
            };

            // Validate both transactions
            if (!ValidateTransaction(withdrawalTransaction) || !ValidateTransaction(depositTransaction))
                return false;

            // Process both transactions (in a real scenario, this should be wrapped in a database transaction)
            var withdrawalSuccess = _transactionRepository.Create(withdrawalTransaction);
            if (!withdrawalSuccess)
                return false;

            var depositSuccess = _transactionRepository.Create(depositTransaction);
            if (!depositSuccess)
            {
                // In a real scenario, you would rollback the withdrawal transaction here
                throw new InvalidOperationException("Transfer failed. Please contact support.");
            }

            return true;
        }

        public decimal GetAccountBalance(int accountId)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than zero.", nameof(accountId));

            return _transactionRepository.GetBalanceByAccountId(accountId);
        }

        public bool ValidateTransaction(Transaction transaction)
        {
            if (transaction == null)
                return false;

            if (transaction.AccountId <= 0)
                return false;

            if (transaction.Amount <= 0)
                return false;

            if (transaction.TransactionDate == default(DateTime))
                return false;

            if (string.IsNullOrWhiteSpace(transaction.Description))
                return false;

            return true;
        }

        public List<Transaction> GetRecentTransactions(int accountId, int count = 10)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than zero.", nameof(accountId));

            if (count <= 0)
                throw new ArgumentException("Count must be greater than zero.", nameof(count));

            var transactions = _transactionRepository.GetByAccountId(accountId);
            return transactions.OrderByDescending(t => t.TransactionDate).Take(count).ToList();
        }

        public decimal GetTotalTransactionAmount(int accountId, TransactionType transactionType, DateTime? startDate = null, DateTime? endDate = null)
        {
            if (accountId <= 0)
                throw new ArgumentException("Account ID must be greater than zero.", nameof(accountId));

            var transactions = _transactionRepository.GetByAccountIdAndType(accountId, transactionType);

            if (startDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate >= startDate.Value).ToList();

            if (endDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate <= endDate.Value).ToList();

            return transactions.Sum(t => t.Amount);
        }
    }
}