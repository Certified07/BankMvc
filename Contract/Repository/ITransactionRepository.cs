using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMvc.Models.Entity;

namespace BankMvc.Contract.Repository
{
    public interface ITransactionRepository
    {
        Transaction GetById(int transactionId);
        List<Transaction> GetByAccountId(int accountId);
        bool Create(Transaction transaction);
    }

}