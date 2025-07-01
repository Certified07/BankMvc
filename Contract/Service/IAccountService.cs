using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankMvc.DTO.Request;
using BankMvc.DTO.Response;

namespace BankMvc.Contract.Service
{
    public interface IAccountService
    {
     
        Task<ServiceResult<AccountDto>> GetByIdAsync(int accountId);
        Task<ServiceResult<List<AccountDto>>> GetByUserIdAsync(int userId);
        Task<ServiceResult<AccountDto>> GetByAccountNumberAsync(string accountNumber);
        Task<ServiceResult<AccountDto>> CreateAsync(CreateAccountDto createAccountDto);
        Task<ServiceResult<AccountDto>> UpdateAsync(UpdateAccountDto updateAccountDto);
        Task<ServiceResult> DeleteAsync(int accountId);
        Task<ServiceResult<List<AccountDto>>> GetAllAsync();
        Task<ServiceResult<bool>> ExistsAsync(int accountId);
        Task<ServiceResult<decimal>> GetBalanceAsync(int accountId);
        Task<ServiceResult<AccountDto>> TransferAsync(int fromAccountId, int toAccountId, decimal amount);
        Task<ServiceResult<AccountDto>> DepositAsync(int accountId, decimal amount);
        Task<ServiceResult<AccountDto>> WithdrawAsync(int accountId, decimal amount);
    }
   
}