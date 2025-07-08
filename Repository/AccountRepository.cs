
using Microsoft.EntityFrameworkCore;

using BankMvc.Contract.Repository;
using BankMvc.Models.Entity;
using BankMvc.DTO.Request;
using BankMvc.Data;

namespace Bank.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankApplicationDbContext _context;

        public AccountRepository(BankApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> GetByIdAsync(int accountId)
        {
            try
            {
                return await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == accountId);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error retrieving account: {ex.Message}");
                return null;
            }
        }

        public async Task<List<Account>> GetByUserIdAsync(int userId)
        {
            try
            {
                return await _context.Accounts
                    .Where(a => a.UserId == userId)
                    .OrderBy(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving accounts: {ex.Message}");
                return new List<Account>();
            }
        }

        public async Task<Account?> GetByAccountNumberAsync(string accountNumber)
        {
            try
            {
                return await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving account by account number: {ex.Message}");
                return null;
            }
        }

        public async Task<Account> CreateAsync(Account account)
        {
              
             
            try
            {
                account.CreatedAt = DateTime.Now;
                account.UpdatedAt = DateTime.Now;

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating account: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Account account)
        {
            try
            {
                var existingAccount = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == account.AccountId);

                if (existingAccount == null)
                    return false;

                existingAccount.Balance = account.Balance;
                existingAccount.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating account: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int accountId)
        {
            try
            {
                var account = await _context.Accounts
                    .FirstOrDefaultAsync(a => a.AccountId == accountId);

                if (account == null)
                    return false;

                _context.Accounts.Remove(account);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting account: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Account>> GetAllAsync()
        {
            try
            {
                return await _context.Accounts
                    .OrderBy(a => a.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving all accounts: {ex.Message}");
                return new List<Account>();
            }
        }

        public async Task<bool> ExistsAsync(int accountId)
        {
            try
            {
                return await _context.Accounts
                    .AnyAsync(a => a.AccountId == accountId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking account existence: {ex.Message}");
                return false;
            }
        }
    }
}
