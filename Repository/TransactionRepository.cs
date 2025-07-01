using System;
using System.Collections.Generic;
using System.Linq;
using BankMvc.Models.Entity;
using BankMvc.Contract.Repository;

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
                // Log the exception
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
                // Log the exception
                throw new Exception($"Error retrieving transactions for account ID {accountId}", ex);
            }
        }

        public bool Create(Transaction transaction)
        {
            try
            {
                if (transaction == null)
                    return false;

                // Set transaction date if not already set
                if (transaction.TransactionDate == default(DateTime))
                    transaction.TransactionDate = DateTime.Now;

                _context.Transactions.Add(transaction);
                var result = _context.SaveChanges();

                return result > 0;
            }
            catch (Exception ex)
            {
                // Log the exception
                throw new Exception("Error creating transaction", ex);
            }
        }
    }
}
