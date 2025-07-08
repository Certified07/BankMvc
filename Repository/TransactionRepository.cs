//using System;
//using System.Collections.Generic;
//using System.Linq;
//using BankMvc.Models.Entity;
//using BankMvc.Contract.Repository;
//using BankMvc.Data;

//using Microsoft.EntityFrameworkCore;

//namespace BankMvc.Repository
//{
//    public class TransactionRepository : ITransactionRepository
//    {
//        private readonly BankApplicationDbContext _context;

//        public TransactionRepository(BankApplicationDbContext context)
//        {
//            _context = context;
//        }

//        public Transaction GetById(int transactionId)
//        {
//            try
//            {
//                return _context.Transactions
//                    .FirstOrDefault(t => t.TransactionId == transactionId);
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                throw new Exception($"Error retrieving transaction with ID {transactionId}", ex);
//            }
//        }

//        public List<Transaction> GetByAccountId(int accountId)
//        {
//            try
//            {
//                return _context.Transactions
//                    .Where(t => t.AccountId == accountId)
//                    .OrderByDescending(t => t.TransactionDate)
//                    .ToList();
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                throw new Exception($"Error retrieving transactions for account ID {accountId}", ex);
//            }
//        }

//        public bool Create(Transaction transaction)
//        {
//            try
//            {
//                if (transaction == null)
//                    return false;

//                // Set transaction date if not already set
//                if (transaction.TransactionDate == default(DateTime))
//                    transaction.TransactionDate = DateTime.Now;

//                _context.Transactions.Add(transaction);
//                var result = _context.SaveChanges();

//                return result > 0;
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                throw new Exception("Error creating transaction", ex);
//            }
//        }
//}
//}

using System;
using System.Collections.Generic;
using System.Linq;
using BankMvc.Models.Entity;
using BankMvc.Models.Enum;
using BankMvc.Contract.Repository;
using BankMvc.Data;
using Microsoft.EntityFrameworkCore;

namespace BankMvc.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly BankApplicationDbContext _context;

        public TransactionRepository(BankApplicationDbContext context)
        {
            _context = context;
        }

        public Transaction GetById(int transactionId)
        {
            try
            {
                return _context.Transactions
                    .FirstOrDefault(t => t.TransactionId == transactionId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving transaction with ID {transactionId}", ex);
            }
        }

        public List<Transaction> GetByAccountId(int accountId)
        {
            try
            {
                return _context.Transactions
                    .Where(t => t.AccountId == accountId)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving transactions for account ID {accountId}", ex);
            }
        }

        public bool Create(Transaction transaction)
        {
            try
            {
                if (transaction == null)
                    return false;

                // Validate transaction type
                if (!Enum.IsDefined(typeof(TransactionType), transaction.TransactionType))
                {
                    throw new ArgumentException("Invalid transaction type");
                }

                // Set transaction date if not already set
                if (transaction.TransactionDate == default(DateTime))
                    transaction.TransactionDate = DateTime.Now;

                // Generate reference number if not provided
                if (string.IsNullOrEmpty(transaction.ReferenceNumber))
                {
                    transaction.ReferenceNumber = $"TXN{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
                }

                _context.Transactions.Add(transaction);
                var result = _context.SaveChanges();

                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating transaction", ex);
            }
        }

        // Additional helper methods for working with transaction types
        public List<Transaction> GetByAccountIdAndType(int accountId, TransactionType transactionType)
        {
            try
            {
                return _context.Transactions
                    .Where(t => t.AccountId == accountId && t.TransactionType == transactionType)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving transactions for account ID {accountId} and type {transactionType}", ex);
            }
        }

        public decimal GetBalanceByAccountId(int accountId)
        {
            try
            {
                var transactions = _context.Transactions
                    .Where(t => t.AccountId == accountId)
                    .ToList();

                decimal balance = 0;
                foreach (var transaction in transactions)
                {
                    switch (transaction.TransactionType)
                    {
                        case TransactionType.Deposit:
                            balance += transaction.Amount;
                            break;
                        case TransactionType.Withdrawal:
                            balance -= transaction.Amount;
                            break;
                        case TransactionType.Transfer:
                            // For transfers, you might need additional logic
                            // depending on whether it's incoming or outgoing
                            break;
                    }
                }

                return balance;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error calculating balance for account ID {accountId}", ex);
            }
        }
    }
}