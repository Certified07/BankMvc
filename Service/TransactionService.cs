//namespace BankMvc.Service
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Threading.Tasks;
//    using global::BankMvc.Contract.Repository;
//    using global::BankMvc.Contract.Service;
//    using global::BankMvc.Models.Entity;

//    namespace BankMvc.Service
//    {
//        public class TransactionService : ITransactionService
//        {
//            private readonly ITransactionRepository _transactionRepository;

//            public TransactionService(ITransactionRepository transactionRepository)
//            {
//                _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
//            }

//            public Transaction GetTransactionById(int transactionId)
//            {
//                if (transactionId <= 0)
//                    throw new ArgumentException("Transaction ID must be greater than zero", nameof(transactionId));

//                try
//                {
//                    var transaction = _transactionRepository.GetById(transactionId);
//                    if (transaction == null)
//                        throw new InvalidOperationException($"Transaction with ID {transactionId} not found");

//                    return transaction;
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception($"Error retrieving transaction with ID {transactionId}", ex);
//                }
//            }

//            public List<Transaction> GetTransactionsByAccountId(int accountId)
//            {
//                if (accountId <= 0)
//                    throw new ArgumentException("Account ID must be greater than zero", nameof(accountId));

//                try
//                {
//                    return _transactionRepository.GetByAccountId(accountId);
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception($"Error retrieving transactions for account ID {accountId}", ex);
//                }
//            }

//            public bool CreateTransaction(Transaction transaction)
//            {
//                if (transaction == null)
//                    throw new ArgumentNullException(nameof(transaction));

//                // Business logic validation
//                ValidateTransaction(transaction);

//                try
//                {
//                    return _transactionRepository.Create(transaction);
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception("Error creating transaction", ex);
//                }
//            }

//            public bool ProcessDeposit(int accountId, decimal amount, string description = "Deposit")
//            {
//                if (accountId <= 0)
//                    throw new ArgumentException("Account ID must be greater than zero", nameof(accountId));

//                if (amount <= 0)
//                    throw new ArgumentException("Deposit amount must be greater than zero", nameof(amount));

//                var transaction = new Transaction
//                {
//                    AccountId = accountId,
//                    Amount = amount,
//                    TransactionType = "Deposit",
//                    Description = description,
//                    TransactionDate = DateTime.Now
//                };

//                return CreateTransaction(transaction);
//            }

//            public bool ProcessWithdrawal(int accountId, decimal amount, string description = "Withdrawal")
//            {
//                if (accountId <= 0)
//                    throw new ArgumentException("Account ID must be greater than zero", nameof(accountId));

//                if (amount <= 0)
//                    throw new ArgumentException("Withdrawal amount must be greater than zero", nameof(amount));

//                var transaction = new Transaction
//                {
//                    AccountId = accountId,
//                    Amount = -amount, // Negative for withdrawal
//                    TransactionType = "Withdrawal",
//                    Description = description,
//                    TransactionDate = DateTime.Now
//                };

//                return CreateTransaction(transaction);
//            }

//            public bool ProcessTransfer(int fromAccountId, int toAccountId, decimal amount, string description = "Transfer")
//            {
//                if (fromAccountId <= 0)
//                    throw new ArgumentException("From Account ID must be greater than zero", nameof(fromAccountId));

//                if (toAccountId <= 0)
//                    throw new ArgumentException("To Account ID must be greater than zero", nameof(toAccountId));

//                if (fromAccountId == toAccountId)
//                    throw new ArgumentException("Cannot transfer to the same account");

//                if (amount <= 0)
//                    throw new ArgumentException("Transfer amount must be greater than zero", nameof(amount));

//                try
//                {
//                    // Create withdrawal transaction from source account
//                    var withdrawalTransaction = new Transaction
//                    {
//                        AccountId = fromAccountId,
//                        Amount = -amount,
//                        TransactionType = "Transfer Out",
//                        Description = $"{description} - To Account {toAccountId}",
//                        TransactionDate = DateTime.Now
//                    };

//                    // Create deposit transaction to destination account
//                    var depositTransaction = new Transaction
//                    {
//                        AccountId = toAccountId,
//                        Amount = amount,
//                        TransactionType = "Transfer In",
//                        Description = $"{description} - From Account {fromAccountId}",
//                        TransactionDate = DateTime.Now
//                    };

//                    // Process both transactions
//                    bool withdrawalSuccess = CreateTransaction(withdrawalTransaction);
//                    bool depositSuccess = CreateTransaction(depositTransaction);

//                    return withdrawalSuccess && depositSuccess;
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception("Error processing transfer", ex);
//                }
//            }

//            public decimal GetAccountBalance(int accountId)
//            {
//                if (accountId <= 0)
//                    throw new ArgumentException("Account ID must be greater than zero", nameof(accountId));

//                try
//                {
//                    var transactions = GetTransactionsByAccountId(accountId);
//                    return transactions.Sum(t => t.Amount);
//                }
//                catch (Exception ex)
//                {
//                    throw new Exception($"Error calculating balance for account ID {accountId}", ex);
//                }
//            }

//            private void ValidateTransaction(Transaction transaction)
//            {
//                if (transaction.AccountId <= 0)
//                    throw new ArgumentException("Account ID must be greater than zero");

//                if (transaction.Amount == 0)
//                    throw new ArgumentException("Transaction amount cannot be zero");

//                if (string.IsNullOrWhiteSpace(transaction.TransactionType))
//                    throw new ArgumentException("Transaction type is required");
//            }
//        }
//    }
//}
