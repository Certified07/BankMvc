using Microsoft.Extensions.Logging;
using BankMvc.Contract.Service;
using BankMvc.Contract.Repository;
using BankMvc.DTO.Request;
using BankMvc.DTO.Response;
using BankMvc.Models;
using BankMvc.Models.Entity;
using BankMvc.Models.Enum; // Add this using statement

namespace Bank.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IAccountRepository accountRepository, ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<AccountDto>> GetByIdAsync(int accountId)
        {
            try
            {
                if (accountId <= 0)
                    return ServiceResult<AccountDto>.FailureResult("Invalid account ID");

                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null)
                    return ServiceResult<AccountDto>.FailureResult("Account not found");

                var accountDto = MapToDto(account);
                return ServiceResult<AccountDto>.SuccessResult(accountDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account by ID: {AccountId}", accountId);
                return ServiceResult<AccountDto>.FailureResult("An error occurred while retrieving the account");
            }
        }

        public async Task<ServiceResult<List<AccountDto>>> GetByUserIdAsync(int userId)
        {
            try
            {
                if (userId <= 0)
                    return ServiceResult<List<AccountDto>>.FailureResult("Invalid user ID");

                var accounts = await _accountRepository.GetByUserIdAsync(userId);
                var accountDtos = accounts.Select(MapToDto).ToList();

                return ServiceResult<List<AccountDto>>.SuccessResult(accountDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting accounts by user ID: {UserId}", userId);
                return ServiceResult<List<AccountDto>>.FailureResult("An error occurred while retrieving accounts");
            }
        }

        public async Task<ServiceResult<AccountDto>> GetByAccountNumberAsync(string accountNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountNumber))
                    return ServiceResult<AccountDto>.FailureResult("Account number is required");

                var account = await _accountRepository.GetByAccountNumberAsync(accountNumber);
                if (account == null)
                    return ServiceResult<AccountDto>.FailureResult("Account not found");

                var accountDto = MapToDto(account);
                return ServiceResult<AccountDto>.SuccessResult(accountDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account by account number: {AccountNumber}", accountNumber);
                return ServiceResult<AccountDto>.FailureResult("An error occurred while retrieving the account");
            }
        }

        public async Task<ServiceResult<AccountDto>> CreateAsync(CreateAccountDto createAccountDto)
        {
            try
            {
                if (createAccountDto == null)
                    return ServiceResult<AccountDto>.FailureResult("Account data is required");

                var existingAccount = await _accountRepository.GetByAccountNumberAsync(createAccountDto.AccountNumber);
                if (existingAccount != null)
                    return ServiceResult<AccountDto>.FailureResult("Account number already exists");

                var account = new Account
                {
                    UserId = createAccountDto.UserId,
                    AccountNumber = createAccountDto.AccountNumber,
                    AccountType = createAccountDto.Type.ToString(), // Fix: Convert AccountType enum to string  
                    Balance = createAccountDto.Balance,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var createdAccount = await _accountRepository.CreateAsync(account);
                var accountDto = MapToDto(createdAccount);

                _logger.LogInformation("Account created successfully: {AccountId}", createdAccount.AccountId);
                return ServiceResult<AccountDto>.SuccessResult(accountDto, "Account created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account");
                return ServiceResult<AccountDto>.FailureResult("An error occurred while creating the account");
            }
        }

        public async Task<ServiceResult<AccountDto>> UpdateAsync(UpdateAccountDto updateAccountDto)
        {
            try
            {
                // Add null check for updateAccountDto
                if (updateAccountDto == null)
                    return ServiceResult<AccountDto>.FailureResult("Update data is required");

                var existingAccount = await _accountRepository.GetByIdAsync(updateAccountDto.AccountId);
                if (existingAccount == null)
                    return ServiceResult<AccountDto>.FailureResult("Account not found");

                existingAccount.Balance = updateAccountDto.Balance;
                existingAccount.UpdatedAt = DateTime.UtcNow; // Update timestamp

                var success = await _accountRepository.UpdateAsync(existingAccount);
                if (!success)
                    return ServiceResult<AccountDto>.FailureResult("Failed to update account");

                var accountDto = MapToDto(existingAccount);
                _logger.LogInformation("Account updated successfully: {AccountId}", updateAccountDto.AccountId);

                return ServiceResult<AccountDto>.SuccessResult(accountDto, "Account updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account: {AccountId}", updateAccountDto?.AccountId);
                return ServiceResult<AccountDto>.FailureResult("An error occurred while updating the account");
            }
        }

        public async Task<ServiceResult> DeleteAsync(int accountId)
        {
            try
            {
                if (accountId <= 0)
                    return ServiceResult.FailureResult("Invalid account ID");

                var exists = await _accountRepository.ExistsAsync(accountId);
                if (!exists)
                    return ServiceResult.FailureResult("Account not found");

                var success = await _accountRepository.DeleteAsync(accountId);
                if (!success)
                    return ServiceResult.FailureResult("Failed to delete account");

                _logger.LogInformation("Account deleted successfully: {AccountId}", accountId);
                return ServiceResult.SuccessResult("Account deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting account: {AccountId}", accountId);
                return ServiceResult.FailureResult("An error occurred while deleting the account");
            }
        }

        public async Task<ServiceResult<List<AccountDto>>> GetAllAsync()
        {
            try
            {
                var accounts = await _accountRepository.GetAllAsync();
                var accountDtos = accounts.Select(MapToDto).ToList();

                return ServiceResult<List<AccountDto>>.SuccessResult(accountDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all accounts");
                return ServiceResult<List<AccountDto>>.FailureResult("An error occurred while retrieving accounts");
            }
        }

        public async Task<ServiceResult<bool>> ExistsAsync(int accountId)
        {
            try
            {
                if (accountId <= 0)
                    return ServiceResult<bool>.FailureResult("Invalid account ID");

                var exists = await _accountRepository.ExistsAsync(accountId);
                return ServiceResult<bool>.SuccessResult(exists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking account existence: {AccountId}", accountId);
                return ServiceResult<bool>.FailureResult("An error occurred while checking account existence");
            }
        }

        public async Task<ServiceResult<decimal>> GetBalanceAsync(int accountId)
        {
            try
            {
                var accountResult = await GetByIdAsync(accountId);
                if (!accountResult.Success)
                    return ServiceResult<decimal>.FailureResult(accountResult.Message);

                return ServiceResult<decimal>.SuccessResult(accountResult.Data!.Balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting account balance: {AccountId}", accountId);
                return ServiceResult<decimal>.FailureResult("An error occurred while retrieving account balance");
            }
        }

        public async Task<ServiceResult<AccountDto>> DepositAsync(int accountId, decimal amount)
        {
            try
            {
                if (amount <= 0)
                    return ServiceResult<AccountDto>.FailureResult("Deposit amount must be greater than zero");

                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null)
                    return ServiceResult<AccountDto>.FailureResult("Account not found");

                account.Balance += amount;
                account.UpdatedAt = DateTime.UtcNow; // Update timestamp
                var success = await _accountRepository.UpdateAsync(account);

                if (!success)
                    return ServiceResult<AccountDto>.FailureResult("Failed to deposit amount");

                var accountDto = MapToDto(account);
                _logger.LogInformation("Deposit successful: {AccountId}, Amount: {Amount}", accountId, amount);

                return ServiceResult<AccountDto>.SuccessResult(accountDto, $"Successfully deposited {amount:C}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error depositing to account: {AccountId}, Amount: {Amount}", accountId, amount);
                return ServiceResult<AccountDto>.FailureResult("An error occurred while processing the deposit");
            }
        }

        public async Task<ServiceResult<AccountDto>> WithdrawAsync(int accountId, decimal amount)
        {
            try
            {
                if (amount <= 0)
                    return ServiceResult<AccountDto>.FailureResult("Withdrawal amount must be greater than zero");

                var account = await _accountRepository.GetByIdAsync(accountId);
                if (account == null)
                    return ServiceResult<AccountDto>.FailureResult("Account not found");

                if (account.Balance < amount)
                    return ServiceResult<AccountDto>.FailureResult("Insufficient funds");

                account.Balance -= amount;
                account.UpdatedAt = DateTime.UtcNow; // Update timestamp
                var success = await _accountRepository.UpdateAsync(account);

                if (!success)
                    return ServiceResult<AccountDto>.FailureResult("Failed to withdraw amount");

                var accountDto = MapToDto(account);
                _logger.LogInformation("Withdrawal successful: {AccountId}, Amount: {Amount}", accountId, amount);

                return ServiceResult<AccountDto>.SuccessResult(accountDto, $"Successfully withdrew {amount:C}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error withdrawing from account: {AccountId}, Amount: {Amount}", accountId, amount);
                return ServiceResult<AccountDto>.FailureResult("An error occurred while processing the withdrawal");
            }
        }

        public async Task<ServiceResult<AccountDto>> TransferAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            try
            {
                if (amount <= 0)
                    return ServiceResult<AccountDto>.FailureResult("Transfer amount must be greater than zero");

                if (fromAccountId == toAccountId)
                    return ServiceResult<AccountDto>.FailureResult("Cannot transfer to the same account");

                var fromAccount = await _accountRepository.GetByIdAsync(fromAccountId);
                var toAccount = await _accountRepository.GetByIdAsync(toAccountId);

                if (fromAccount == null)
                    return ServiceResult<AccountDto>.FailureResult("Source account not found");

                if (toAccount == null)
                    return ServiceResult<AccountDto>.FailureResult("Destination account not found");

                if (fromAccount.Balance < amount)
                    return ServiceResult<AccountDto>.FailureResult("Insufficient funds in source account");

                // Perform transfer
                fromAccount.Balance -= amount;
                fromAccount.UpdatedAt = DateTime.UtcNow; // Update timestamp
                toAccount.Balance += amount;
                toAccount.UpdatedAt = DateTime.UtcNow; // Update timestamp

                var fromSuccess = await _accountRepository.UpdateAsync(fromAccount);
                var toSuccess = await _accountRepository.UpdateAsync(toAccount);

                if (!fromSuccess || !toSuccess)
                    return ServiceResult<AccountDto>.FailureResult("Transfer failed");

                var fromAccountDto = MapToDto(fromAccount);
                _logger.LogInformation("Transfer successful: From {FromAccountId} to {ToAccountId}, Amount: {Amount}",
                    fromAccountId, toAccountId, amount);

                return ServiceResult<AccountDto>.SuccessResult(fromAccountDto,
                    $"Successfully transferred {amount:C} to account {toAccount.AccountNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring between accounts: From {FromAccountId} to {ToAccountId}, Amount: {Amount}",
                    fromAccountId, toAccountId, amount);
                return ServiceResult<AccountDto>.FailureResult("An error occurred while processing the transfer");
            }
        }

        private static AccountDto MapToDto(Account account)
        {
            return new AccountDto
            {
                AccountId = account.AccountId,
                UserId = account.UserId,
                AccountNumber = account.AccountNumber,
                Type = Enum.Parse<AccountType>(account.AccountType), // Fix: Convert string to AccountType enum  
                Balance = account.Balance,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt
            };
        }
    }
}