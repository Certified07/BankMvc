using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMvc.Models.Entity;

namespace BankMvc.Contract.Repository
{
    public interface IAccountRepository
    {
        Task<Account?> GetByIdAsync(int accountId);
        Task<List<Account>> GetByUserIdAsync(int userId);
        Task<Account?> GetByAccountNumberAsync(string accountNumber);
        Task<Account> CreateAsync(Account account);
        Task<bool> UpdateAsync(Account account);
        Task<bool> DeleteAsync(int accountId);
        Task<List<Account>> GetAllAsync();
        Task<bool> ExistsAsync(int accountId);
    }
}