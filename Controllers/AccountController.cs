//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using BankMvc.DTO.ViewModels;
//using BankMvc.DTO.Request;
//using BankMvc.Contract.Service;
//using BankMvc.Contract.Repository;
//using BankMvc.Models.Entity;
//using System.Security.Claims;
//using System.ComponentModel.DataAnnotations;
//using BankMvc.DTO.Response;
//using BankMvc.Models.Enum;

//namespace BankMvc.Controllers
//{
//    [Authorize]
//    public class AccountController : Controller
//    {
//        private readonly IAccountService _accountService;
//        private readonly ITransactionRepository _transactionRepository;
//        private readonly ILogger<AccountController> _logger;

//        public AccountController(
//            IAccountService accountService,
//            ITransactionRepository transactionRepository,
//            ILogger<AccountController> logger)
//        {
//            _accountService = accountService;
//            _transactionRepository = transactionRepository;
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Dashboard()
//        {
//            try
//            {
//                // Get current user's ID from claims
//                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
//                if (userIdClaim == null)
//                {
//                    return RedirectToAction("Login", "Auth");
//                }

//                if (int.TryParse(userIdClaim.Value, out int userId))
//                {
//                    // Get user accounts using service layer
//                    var accountsResult = await _accountService.GetByUserIdAsync(userId);
//                    if (!accountsResult.Success)
//                    {
//                        _logger.LogError("Failed to get accounts for user {UserId}: {Error}", userId, accountsResult.Errors);
//                        TempData["ErrorMessage"] = "Unable to load accounts. Please try again.";
//                        return RedirectToAction("Index", "Home");
//                    }

//                    var accounts = accountsResult.Data ?? new List<AccountDto>();

//                    // Get recent transactions for all accounts
//                    var recentTransactions = new List<TransactionSummaryViewModel>();
//                    foreach (var account in accounts)
//                    {
//                        var accountTransactions = _transactionRepository.GetByAccountId(account.AccountId);
//                        recentTransactions.AddRange(accountTransactions.Select(t => new TransactionSummaryViewModel
//                        {
//                            TransactionId = t.TransactionId,
//                            Description = t.Description ?? "Transaction",
//                            Amount = t.Amount,
//                            TransactionType = t.Amount > 0 ? "Credit" : "Debit",
//                            TransactionDate = t.TransactionDate,
//                            AccountId = t.AccountId
//                        }));
//                    }

//                    // Sort by date and take recent ones
//                    recentTransactions = recentTransactions
//                        .OrderByDescending(t => t.TransactionDate)
//                        .Take(10)
//                        .ToList();

//                    // Calculate monthly statistics
//                    var currentMonth = DateTime.Now.Month;
//                    var currentYear = DateTime.Now.Year;
//                    var monthlyTransactions = recentTransactions
//                        .Count(t => t.TransactionDate.Month == currentMonth && t.TransactionDate.Year == currentYear);

//                    var monthlyIncome = recentTransactions
//                        .Where(t => t.TransactionType == "Credit" &&
//                                   t.TransactionDate.Month == currentMonth &&
//                                   t.TransactionDate.Year == currentYear)
//                        .Sum(t => t.Amount);

//                    // Get user name from claims
//                    var userName = User.FindFirst(ClaimTypes.GivenName)?.Value ??
//                                  User.FindFirst(ClaimTypes.Name)?.Value ??
//                                  "User";

//                    // Create the DashboardViewModel
//                    var viewModel = new DashboardViewModel
//                    {
//                        UserName = userName,
//                        LastLoginDate = DateTime.Now, // You might want to track this in the database
//                        TotalBalance = accounts.Sum(a => a.Balance),
//                        MonthlyIncome = monthlyIncome,
//                        MonthlyTransactions = monthlyTransactions,
//                        Accounts = accounts.Select(a => new AccountSummaryViewModel
//                        {
//                            AccountId = a.AccountId,
//                            AccountNumber = a.AccountNumber,
//                            AccountType = a.Type.ToString() ?? "Checking",
//                            Balance = a.Balance,
//                            CreatedAt = a.CreatedAt,
//                            UpdatedAt = a.UpdatedAt
//                        }).ToList(),
//                        RecentTransactions = recentTransactions
//                    };

//                    return View(viewModel);
//                }

//                return RedirectToAction("Login", "Auth");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error loading dashboard for user {UserId}", GetCurrentUserId());
//                TempData["ErrorMessage"] = "An error occurred while loading your dashboard.";
//                return RedirectToAction("Login", "Auth");
//            }
//        }
//        //public async Task<IActionResult> Dashboard()
//        //{
//        //    try
//        //    {
//        //        // Get current user's ID from claims
//        //        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
//        //        if (userIdClaim == null)
//        //        {
//        //            return RedirectToAction("Login", "Auth");
//        //        }

//        //        if (int.TryParse(userIdClaim.Value, out int userId))
//        //        {
//        //            // Use your custom service to get user data
//        //            var userAccounts = await _accountService.GetByUserIdAsync(userId);

//        //            // Pass data to view
//        //            ViewBag.UserName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? "User";
//        //            ViewBag.LastName = User.FindFirst(ClaimTypes.Surname)?.Value ?? "";

//        //            return View(userAccounts);
//        //        }

//        //        return RedirectToAction("Login", "Auth");
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        // Log the exception
//        //        TempData["ErrorMessage"] = "An error occurred while loading your dashboard.";
//        //        return RedirectToAction("Login", "Auth");
//        //    }
//        //}

//        //[HttpGet]
//        //public async Task<IActionResult> Dashboard()
//        //{
//        //    try
//        //    {
//        //        var userId = GetCurrentUserId();
//        //        if (userId == null)
//        //        {
//        //            return RedirectToAction("Login", "Auth");
//        //        }

//        //        var user = await _userManager.FindByIdAsync(userId);
//        //        if (user == null)
//        //        {
//        //            return RedirectToAction("Login", "Auth");
//        //        }

//        //        // Get user accounts using service layer
//        //        var accountsResult = await _accountService.GetByUserIdAsync(int.Parse(userId));
//        //        if (!accountsResult.Success)
//        //        {
//        //            _logger.LogError("Failed to get accounts for user {UserId}: {Error}", userId, accountsResult.Errors);
//        //            TempData["ErrorMessage"] = "Unable to load accounts. Please try again.";
//        //            return RedirectToAction("Index", "Home");
//        //        }

//        //        var accounts = accountsResult.Data ?? new List<AccountDto>();

//        //        // Get recent transactions for all accounts
//        //        var recentTransactions = new List<TransactionSummaryViewModel>();
//        //        foreach (var account in accounts)
//        //        {
//        //            var accountTransactions = _transactionRepository.GetByAccountId(account.AccountId);
//        //            recentTransactions.AddRange(accountTransactions.Select(t => new TransactionSummaryViewModel
//        //            {
//        //                TransactionId = t.TransactionId,
//        //                Description = t.Description ?? "Transaction",
//        //                Amount = t.Amount,
//        //                TransactionType = t.Amount > 0 ? "Credit" : "Debit",
//        //                TransactionDate = t.TransactionDate,
//        //                AccountId = t.AccountId
//        //            }));
//        //        }

//        //        // Sort by date and take recent ones
//        //        recentTransactions = recentTransactions
//        //            .OrderByDescending(t => t.TransactionDate)
//        //            .Take(10)
//        //            .ToList();

//        //        // Calculate monthly statistics
//        //        var currentMonth = DateTime.Now.Month;
//        //        var currentYear = DateTime.Now.Year;
//        //        var monthlyTransactions = recentTransactions
//        //            .Count(t => t.TransactionDate.Month == currentMonth && t.TransactionDate.Year == currentYear);

//        //        var monthlyIncome = recentTransactions
//        //            .Where(t => t.TransactionType == "Credit" &&
//        //                       t.TransactionDate.Month == currentMonth &&
//        //                       t.TransactionDate.Year == currentYear)
//        //            .Sum(t => t.Amount);

//        //        var viewModel = new DashboardViewModel
//        //        {
//        //            UserName = user.UserName ?? "User",
//        //            LastLoginDate = DateTime.Now, // You might want to track this in the database
//        //            TotalBalance = accounts.Sum(a => a.Balance),
//        //            MonthlyIncome = monthlyIncome,
//        //            MonthlyTransactions = monthlyTransactions,
//        //            Accounts = accounts.Select(a => new AccountSummaryViewModel
//        //            {
//        //                AccountId = a.AccountId,
//        //                AccountNumber = a.AccountNumber,
//        //                AccountType = a.Type.ToString() ?? "Checking",

//        //                Balance = a.Balance,
//        //                CreatedAt = a.CreatedAt,
//        //                UpdatedAt = a.UpdatedAt
//        //            }).ToList(),
//        //            RecentTransactions = recentTransactions
//        //        };

//        //        return View(viewModel);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError(ex, "Error loading dashboard for user {UserId}", GetCurrentUserId());
//        //        TempData["ErrorMessage"] = "Unable to load dashboard. Please try again.";
//        //        return RedirectToAction("Index", "Home");
//        //    }
//        //}

//        [HttpPost]
//        public async Task<IActionResult> Transfer(TransferViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return Json(new { success = false, message = "Invalid transfer details" });
//            }

//            try
//            {
//                var userId = GetCurrentUserId();
//                if (userId == null)
//                {
//                    return Json(new { success = false, message = "User not authenticated" });
//                }

//                // Verify source account ownership using service layer
//                var sourceAccountResult = await _accountService.GetByIdAsync(model.FromAccountId);
//                if (!sourceAccountResult.Success || sourceAccountResult.Data == null)
//                {
//                    return Json(new { success = false, message = "Source account not found" });
//                }

//                var sourceAccount = sourceAccountResult.Data;
//                if (sourceAccount.UserId != int.Parse(userId))
//                {
//                    return Json(new { success = false, message = "Access denied to source account" });
//                }

//                // Verify destination account exists
//                var destinationAccountResult = await _accountService.GetByAccountNumberAsync(model.ToAccountNumber);
//                if (!destinationAccountResult.Success || destinationAccountResult.Data == null)
//                {
//                    return Json(new { success = false, message = "Destination account not found" });
//                }

//                // Perform transfer using service layer
//                var transferResult = await _accountService.TransferAsync(model.FromAccountId, destinationAccountResult.Data.AccountId, model.Amount);
//                if (!transferResult.Success)
//                {
//                    return Json(new { success = false, message = transferResult.Errors != null && transferResult.Errors.Any() ? string.Join(", ", transferResult.Errors) : "Transfer failed" });
//                }

//                return Json(new { success = true, message = "Transfer completed successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error processing transfer for user {UserId}", GetCurrentUserId());
//                return Json(new { success = false, message = "Transfer failed. Please try again." });
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> Deposit(DepositViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return Json(new { success = false, message = "Invalid deposit details" });
//            }

//            try
//            {
//                var userId = GetCurrentUserId();
//                if (userId == null)
//                {
//                    return Json(new { success = false, message = "User not authenticated" });
//                }

//                // Verify account ownership using service layer
//                var accountResult = await _accountService.GetByIdAsync(model.AccountId);
//                if (!accountResult.Success || accountResult.Data == null)
//                {
//                    return Json(new { success = false, message = "Account not found" });
//                }

//                var account = accountResult.Data;
//                if (account.UserId != int.Parse(userId))
//                {
//                    return Json(new { success = false, message = "Access denied to account" });
//                }

//                // Process deposit using service layer
//                var depositResult = await _accountService.DepositAsync(model.AccountId, model.Amount);
//                if (!depositResult.Success)
//                {
//                    return Json(new { success = false, message = depositResult.Errors != null && depositResult.Errors.Any() ? string.Join(", ", depositResult.Errors) : "Deposit failed" });
//                }

//                return Json(new { success = true, message = "Deposit processed successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error processing deposit for user {UserId}", GetCurrentUserId());
//                return Json(new { success = false, message = "Deposit failed. Please try again." });
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> Transactions(int id)
//        {
//            try
//            {
//                var userId = GetCurrentUserId();
//                if (userId == null)
//                {
//                    return RedirectToAction("Login", "Auth");
//                }

//                // Verify account ownership using service layer
//                var accountResult = await _accountService.GetByIdAsync(id);
//                if (!accountResult.Success || accountResult.Data == null)
//                {
//                    TempData["ErrorMessage"] = "Account not found";
//                    return RedirectToAction("Dashboard");
//                }

//                var account = accountResult.Data;
//                if (account.UserId != int.Parse(userId))
//                {
//                    TempData["ErrorMessage"] = "Access denied";
//                    return RedirectToAction("Dashboard");
//                }

//                var transactions = _transactionRepository.GetByAccountId(id);
//                var viewModel = new AccountTransactionsViewModel
//                {
//                    Account = new AccountSummaryViewModel
//                    {
//                        AccountId = account.AccountId,
//                        AccountNumber = account.AccountNumber,
//                        AccountType = account.Type.ToString(),
//                        Balance = account.Balance,
//                        CreatedAt = account.CreatedAt,
//                        UpdatedAt = account.UpdatedAt
//                    },
//                    Transactions = transactions.Select(t => new TransactionSummaryViewModel
//                    {
//                        TransactionId = t.TransactionId,
//                        Description = t.Description ?? "Transaction",
//                        Amount = t.Amount,
//                        TransactionType = t.Amount > 0 ? "Credit" : "Debit",
//                        TransactionDate = t.TransactionDate,
//                        AccountId = t.AccountId
//                    }).ToList()
//                };

//                return View(viewModel);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error loading transactions for account {AccountId}", id);
//                TempData["ErrorMessage"] = "Unable to load transactions. Please try again.";
//                return RedirectToAction("Dashboard");
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return Json(new { success = false, message = "Invalid account details" });
//            }

//            try
//            {
//                var userId = GetCurrentUserId();
//                if (userId == null)
//                {
//                    return Json(new { success = false, message = "User not authenticated" });
//                }

//                // Create account using service layer
//                var createAccountDto = new CreateAccountDto
//                {
//                    UserId = int.Parse(userId),
//                    Type = model.AccountType.ToString(),
//                    InitialBalance = model.InitialDeposit
//                };

//                var createResult = await _accountService.CreateAsync(createAccountDto);
//                if (!createResult.Success)
//                {
//                    return Json(new { success = false, message = createResult.Errors != null && createResult.Errors.Any() ? string.Join(", ", createResult.Errors) : "Account creation failed" });
//                }

//                var newAccount = createResult.Data;
//                return Json(new
//                {
//                    success = true,
//                    message = "Account created successfully",
//                    accountNumber = newAccount?.AccountNumber
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating account for user {UserId}", GetCurrentUserId());
//                return Json(new { success = false, message = "Account creation failed. Please try again." });
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> Statement(int id)
//        {
//            try
//            {
//                var userId = GetCurrentUserId();
//                if (userId == null)
//                {
//                    return RedirectToAction("Login", "Auth");
//                }

//                // Verify account ownership using service layer
//                var accountResult = await _accountService.GetByIdAsync(id);
//                if (!accountResult.Success || accountResult.Data == null)
//                {
//                    return NotFound();
//                }

//                var account = accountResult.Data;
//                if (account.UserId != int.Parse(userId))
//                {
//                    return NotFound();
//                }

//                var transactions = _transactionRepository.GetByAccountId(id);

//                var accountEntity = new Account
//                {
//                    AccountId = account.AccountId,
//                    UserId = account.UserId,
//                    AccountNumber = account.AccountNumber,
//                    AccountType = account.Type.ToString(),
//                    Balance = account.Balance,
//                    CreatedAt = account.CreatedAt,
//                    UpdatedAt = account.UpdatedAt
//                };

//                return View("Statement", new AccountStatementViewModel
//                {
//                    Account = accountEntity,
//                    Transactions = transactions,
//                    StatementDate = DateTime.Now,
//                    StatementPeriod = "Last 30 days"
//                });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error generating statement for account {AccountId}", id);
//                return BadRequest("Unable to generate statement");
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> Withdraw(WithdrawViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return Json(new { success = false, message = "Invalid withdrawal details" });
//            }

//            try
//            {
//                var userId = GetCurrentUserId();
//                if (userId == null)
//                {
//                    return Json(new { success = false, message = "User not authenticated" });
//                }

//                // Verify account ownership using service layer
//                var accountResult = await _accountService.GetByIdAsync(model.AccountId);
//                if (!accountResult.Success || accountResult.Data == null)
//                {
//                    return Json(new { success = false, message = "Account not found" });
//                }

//                var account = accountResult.Data;
//                if (account.UserId != int.Parse(userId))
//                {
//                    return Json(new { success = false, message = "Access denied to account" });
//                }

//                // Process withdrawal using service layer
//                var withdrawResult = await _accountService.WithdrawAsync(model.AccountId, model.Amount);
//                if (!withdrawResult.Success)
//                {
//                    return Json(new { success = false, message = withdrawResult.Errors != null && withdrawResult.Errors.Any() ? string.Join(", ", withdrawResult.Errors) : "Withdrawal failed" });
//                }

//                return Json(new { success = true, message = "Withdrawal processed successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error processing withdrawal for user {UserId}", GetCurrentUserId());
//                return Json(new { success = false, message = "Withdrawal failed. Please try again." });
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetBalance(int accountId)
//        {
//            try
//            {
//                var userId = GetCurrentUserId();
//                if (userId == null)
//                {
//                    return Json(new { success = false, message = "User not authenticated" });
//                }

//                // Verify account ownership
//                var accountResult = await _accountService.GetByIdAsync(accountId);
//                if (!accountResult.Success || accountResult.Data == null)
//                {
//                    return Json(new { success = false, message = "Account not found" });
//                }

//                var account = accountResult.Data;
//                if (account.UserId != int.Parse(userId))
//                {
//                    return Json(new { success = false, message = "Access denied" });
//                }

//                // Get balance using service layer
//                var balanceResult = await _accountService.GetBalanceAsync(accountId);
//                if (!balanceResult.Success)
//                {
//                    return Json(new { success = false, message = balanceResult.Errors != null && balanceResult.Errors.Any() ? string.Join(", ", balanceResult.Errors) : "Unable to get balance" });
//                }



//                return Json(new { success = true, balance = balanceResult.Data });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting balance for account {AccountId}", accountId);
//                return Json(new { success = false, message = "Unable to get balance" });
//            }
//        }

//        private string? GetCurrentUserId()
//        {
//            return User.FindFirstValue(ClaimTypes.NameIdentifier);
//        }
//    }



//    // Additional ViewModels
//    public class AccountTransactionsViewModel
//    {
//        public AccountSummaryViewModel Account { get; set; } = new AccountSummaryViewModel();
//        public List<TransactionSummaryViewModel> Transactions { get; set; } = new List<TransactionSummaryViewModel>();
//    }

//    public class CreateAccountViewModel
//    {
//        [Required(ErrorMessage = "Account type is required")]
//        [Display(Name = "Account Type")]
//        public string AccountType { get; set; } = string.Empty;

//        [Range(0, double.MaxValue, ErrorMessage = "Initial deposit must be 0 or greater")]
//        [Display(Name = "Initial Deposit")]
//        public decimal InitialDeposit { get; set; }
//    }

//    public class DepositViewModel
//    {
//        [Required]
//        public int AccountId { get; set; }

//        [Required]
//        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
//        public decimal Amount { get; set; }

//        public string? Description { get; set; }
//    }

//    public class WithdrawViewModel
//    {
//        [Required]
//        public int AccountId { get; set; }

//        [Required]
//        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
//        public decimal Amount { get; set; }

//        public string? Description { get; set; }
//    }

//    public class TransferViewModel
//    {
//        [Required]
//        public int FromAccountId { get; set; }

//        [Required]
//        [Display(Name = "To Account Number")]
//        public string ToAccountNumber { get; set; } = string.Empty;

//        [Required]
//        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
//        public decimal Amount { get; set; }

//        public string? Description { get; set; }
//    }

//    public class AccountStatementViewModel
//    {
//        public Account Account { get; set; } = new Account();
//        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
//        public DateTime StatementDate { get; set; }
//        public string StatementPeriod { get; set; } = string.Empty;
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BankMvc.DTO.ViewModels;
using BankMvc.DTO.Request;
using BankMvc.Contract.Service;
using BankMvc.Contract.Repository;
using BankMvc.Models.Entity;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using BankMvc.DTO.Response;
using BankMvc.Models.Enum;

namespace BankMvc.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ITransactionService _transactionService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAccountService accountService,
            ITransactionService transactionService,
            ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // Get current user's ID from claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                if (int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Get user accounts using service layer
                    var accountsResult = await _accountService.GetByUserIdAsync(userId);
                    if (!accountsResult.Success)
                    {
                        _logger.LogError("Failed to get accounts for user {UserId}: {Error}", userId, accountsResult.Errors);
                        TempData["ErrorMessage"] = "Unable to load accounts. Please try again.";
                        return RedirectToAction("Index", "Home");
                    }

                    var accounts = accountsResult.Data ?? new List<AccountDto>();

                    // Get recent transactions for all accounts
                    var recentTransactions = new List<TransactionSummaryViewModel>();
                    foreach (var account in accounts)
                    {
                        var accountTransactions = _transactionService.GetTransactionsByAccountId(account.AccountId);
                        recentTransactions.AddRange(accountTransactions.Select(t => new TransactionSummaryViewModel
                        {
                            TransactionId = t.TransactionId,
                            Description = t.Description ?? "Transaction",
                            Amount = t.Amount,
                            TransactionType = t.Amount > 0 ? "Credit" : "Debit",
                            TransactionDate = t.TransactionDate,
                            AccountId = t.AccountId
                        }));
                    }

                    // Sort by date and take recent ones
                    recentTransactions = recentTransactions
                        .OrderByDescending(t => t.TransactionDate)
                        .Take(10)
                        .ToList();

                    // Calculate monthly statistics
                    var currentMonth = DateTime.Now.Month;
                    var currentYear = DateTime.Now.Year;
                    var monthlyTransactions = recentTransactions
                        .Count(t => t.TransactionDate.Month == currentMonth && t.TransactionDate.Year == currentYear);

                    var monthlyIncome = recentTransactions
                        .Where(t => t.TransactionType == "Credit" &&
                                   t.TransactionDate.Month == currentMonth &&
                                   t.TransactionDate.Year == currentYear)
                        .Sum(t => t.Amount);

                    // Get user name from claims
                    var userName = User.FindFirst(ClaimTypes.GivenName)?.Value ??
                                  User.FindFirst(ClaimTypes.Name)?.Value ??
                                  "User";

                    // Create the DashboardViewModel
                    var viewModel = new DashboardViewModel
                    {
                        UserName = userName,
                        LastLoginDate = DateTime.Now, // You might want to track this in the database
                        TotalBalance = accounts.Sum(a => a.Balance),
                        MonthlyIncome = monthlyIncome,
                        MonthlyTransactions = monthlyTransactions,
                        Accounts = accounts.Select(a => new AccountSummaryViewModel
                        {
                            AccountId = a.AccountId,
                            AccountNumber = a.AccountNumber,
                            AccountType = a.Type.ToString() ?? "Checking",
                            Balance = a.Balance,
                            CreatedAt = a.CreatedAt,
                            UpdatedAt = a.UpdatedAt
                        }).ToList(),
                        RecentTransactions = recentTransactions
                    };

                    return View(viewModel);
                }

                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "An error occurred while loading your dashboard.";
                return RedirectToAction("Login", "Auth");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Transfer(TransferViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid transfer details" });
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Verify source account ownership using service layer
                var sourceAccountResult = await _accountService.GetByIdAsync(model.FromAccountId);
                if (!sourceAccountResult.Success || sourceAccountResult.Data == null)
                {
                    return Json(new { success = false, message = "Source account not found" });
                }

                var sourceAccount = sourceAccountResult.Data;
                if (sourceAccount.UserId != int.Parse(userId))
                {
                    return Json(new { success = false, message = "Access denied to source account" });
                }

                // Verify destination account exists
                var destinationAccountResult = await _accountService.GetByAccountNumberAsync(model.ToAccountNumber);
                if (!destinationAccountResult.Success || destinationAccountResult.Data == null)
                {
                    return Json(new { success = false, message = "Destination account not found" });
                }

                // Perform transfer using service layer
                var transferResult = await _accountService.TransferAsync(model.FromAccountId, destinationAccountResult.Data.AccountId, model.Amount);
                var addTransaction = _transactionService.ProcessTransfer(model.FromAccountId,destinationAccountResult.Data.AccountId, model.Amount, model.Description );
                if (!transferResult.Success&&addTransaction==true)
                {
                    return Json(new { success = false, message = transferResult.Errors != null && transferResult.Errors.Any() ? string.Join(", ", transferResult.Errors) : "Transfer failed" });
                }

                return Json(new { success = true, message = "Transfer completed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing transfer for user {UserId}", GetCurrentUserId());
                return Json(new { success = false, message = "Transfer failed. Please try again." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid deposit details" });
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Verify account ownership using service layer
                var accountResult = await _accountService.GetByIdAsync(model.AccountId);
                if (!accountResult.Success || accountResult.Data == null)
                {
                    return Json(new { success = false, message = "Account not found" });
                }

                var account = accountResult.Data;
                if (account.UserId != int.Parse(userId))
                {
                    return Json(new { success = false, message = "Access denied to account" });
                }

                // Process deposit using service layer
                var depositResult = await _accountService.DepositAsync(model.AccountId, model.Amount);
                var addTransaction =  _transactionService.ProcessDeposit(model.AccountId, model.Amount, model.Description);
                if (!depositResult.Success && addTransaction==true)
                {
                    return Json(new { success = false, message = depositResult.Errors != null && depositResult.Errors.Any() ? string.Join(", ", depositResult.Errors) : "Deposit failed" });
                }

                return Json(new { success = true, message = "Deposit processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing deposit for user {UserId}", GetCurrentUserId());
                return Json(new { success = false, message = "Deposit failed. Please try again." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Transactions(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Verify account ownership using service layer
                var accountResult = await _accountService.GetByIdAsync(id);
                if (!accountResult.Success || accountResult.Data == null)
                {
                    TempData["ErrorMessage"] = "Account not found";
                    return RedirectToAction("Dashboard");
                }

                var account = accountResult.Data;
                if (account.UserId != int.Parse(userId))
                {
                    TempData["ErrorMessage"] = "Access denied";
                    return RedirectToAction("Dashboard");
                }

                var transactions = _transactionService.GetTransactionsByAccountId(id);
                var viewModel = new AccountTransactionsViewModel
                {
                    Account = new AccountSummaryViewModel
                    {
                        AccountId = account.AccountId,
                        AccountNumber = account.AccountNumber,
                        AccountType = account.Type.ToString(),
                        Balance = account.Balance,
                        CreatedAt = account.CreatedAt,
                        UpdatedAt = account.UpdatedAt
                    },
                    Transactions = transactions.Select(t => new TransactionSummaryViewModel
                    {
                        TransactionId = t.TransactionId,
                        Description = t.Description ?? "Transaction",
                        Amount = t.Amount,
                        TransactionType = t.Amount > 0 ? "Credit" : "Debit",
                        TransactionDate = t.TransactionDate,
                        AccountId = t.AccountId
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading transactions for account {AccountId}", id);
                TempData["ErrorMessage"] = "Unable to load transactions. Please try again.";
                return RedirectToAction("Dashboard");
            }
        }

        // NEW METHOD: Get all transactions for the current user
        [HttpGet]
        public async Task<IActionResult> AllTransactions()
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Get all user accounts
                var accountsResult = await _accountService.GetByUserIdAsync(int.Parse(userId));
                if (!accountsResult.Success)
                {
                    _logger.LogError("Failed to get accounts for user {UserId}: {Error}", userId, accountsResult.Errors);
                    TempData["ErrorMessage"] = "Unable to load accounts. Please try again.";
                    return RedirectToAction("Dashboard");
                }

                var accounts = accountsResult.Data ?? new List<AccountDto>();
                
                //Accounts = accounts.Select(a => new AccountSummaryViewModel
                //{
                //    AccountId = a.AccountId,
                //    AccountNumber = a.AccountNumber,
                //    AccountType = a.Type.ToString(),
                //    Balance = a.Balance,
                //    CreatedAt = a.CreatedAt,
                //    UpdatedAt = a.UpdatedAt
                //}).ToList(),  
                
                // Get all transactions for all user accounts
                var allTransactions = new List<TransactionSummaryViewModel>();
                foreach (var account in accounts)
                {
                    var accountTransactions = _transactionService.GetTransactionsByAccountId(account.AccountId);
                    allTransactions.AddRange(accountTransactions.Select(t => new TransactionSummaryViewModel
                    {
                        TransactionId = t.TransactionId,
                        Description = t.Description ?? "Transaction",
                        Amount = t.Amount,
                        TransactionType = t.Amount > 0 ? "Credit" : "Debit",
                        TransactionDate = t.TransactionDate,
                        AccountId = t.AccountId
                    }));
                }

                // Sort by date (most recent first)
                allTransactions = allTransactions
                    .OrderByDescending(t => t.TransactionDate)
                    .ToList();

                var viewModel = new AllTransactionsViewModel
                {
                    Transactions = allTransactions,
                    Accounts = accounts.Select(a => new AccountSummaryViewModel
                    {
                        AccountId = a.AccountId,
                        AccountNumber = a.AccountNumber,
                        AccountType = a.Type.ToString(),
                        Balance = a.Balance,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading all transactions for user {UserId}", GetCurrentUserId());
                TempData["ErrorMessage"] = "Unable to load transactions. Please try again.";
                return RedirectToAction("Dashboard");
            }
        }

        // NEW METHOD: Get transaction details
        [HttpGet]
        public async Task<IActionResult> GetTransactionDetails(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Get transaction details
                var transaction = _transactionService.GetTransactionById(id);
                if (transaction == null)
                {
                    return Json(new { success = false, message = "Transaction not found" });
                }

                // Verify user owns the account associated with this transaction
                var accountResult = await _accountService.GetByIdAsync(transaction.AccountId);
                if (!accountResult.Success || accountResult.Data == null)
                {
                    return Json(new { success = false, message = "Account not found" });
                }

                var account = accountResult.Data;
                if (account.UserId != int.Parse(userId))
                {
                    return Json(new { success = false, message = "Access denied" });
                }

                var transactionDetails = new TransactionDetailsViewModel
                {
                    TransactionId = transaction.TransactionId,
                    Description = transaction.Description ?? "Transaction",
                    Amount = transaction.Amount,
                    TransactionType = transaction.Amount > 0 ? "Credit" : "Debit",
                    TransactionDate = transaction.TransactionDate,
                    AccountId = transaction.AccountId,
                    AccountNumber = account.AccountNumber,
                    AccountType = account.Type.ToString(),
                    BalanceAfterTransaction = account.Balance
                };

                return Json(new { success = true, data = transactionDetails });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction details for transaction {TransactionId}", id);
                return Json(new { success = false, message = "Unable to get transaction details" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid account details" });
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Create account using service layer
                //if (CreateAccountViewModel. == null)
                //{
                    
                //}
                var createAccountDto = new CreateAccountDto
                {
                    UserId = int.Parse(userId),
                    Type = model.AccountType.ToString(),
                    Balance = model.InitialDeposit
                };

                var createResult = await _accountService.CreateAsync(createAccountDto);
                if (!createResult.Success)
                {
                    return Json(new { success = false, message = createResult.Errors != null && createResult.Errors.Any() ? string.Join(", ", createResult.Errors) : "Account creation failed" });
                }

                var newAccount = createResult.Data;
                return Json(new
                {
                    success = true,
                    message = "Account created successfully",
                    accountNumber = newAccount?.AccountNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account for user {UserId}", GetCurrentUserId());
                return Json(new { success = false, message = "Account creation failed. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Statement(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                // Verify account ownership using service layer
                var accountResult = await _accountService.GetByIdAsync(id);
                if (!accountResult.Success || accountResult.Data == null)
                {
                    return NotFound();
                }

                var account = accountResult.Data;
                if (account.UserId != int.Parse(userId))
                {
                    return NotFound();
                }

                var transactions = _transactionService.GetTransactionsByAccountId(id);

                var accountEntity = new Account
                {
                    AccountId = account.AccountId,
                    UserId = account.UserId,
                    AccountNumber = account.AccountNumber,
                    AccountType = account.Type.ToString(),
                    Balance = account.Balance,
                    CreatedAt = account.CreatedAt,
                    UpdatedAt = account.UpdatedAt
                };

                return View("Statement", new AccountStatementViewModel
                {
                    Account = accountEntity,
                    Transactions = transactions,
                    StatementDate = DateTime.Now,
                    StatementPeriod = "Last 30 days"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating statement for account {AccountId}", id);
                return BadRequest("Unable to generate statement");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Withdraw(WithdrawViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid withdrawal details" });
            }

            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Verify account ownership using service layer
                var accountResult = await _accountService.GetByIdAsync(model.AccountId);
                if (!accountResult.Success || accountResult.Data == null)
                {
                    return Json(new { success = false, message = "Account not found" });
                }

                var account = accountResult.Data;
                if (account.UserId != int.Parse(userId))
                {
                    return Json(new { success = false, message = "Access denied to account" });
                }

                // Process withdrawal using service layer
                var withdrawResult = await _accountService.WithdrawAsync(model.AccountId, model.Amount);
                var addTransaction = _transactionService.ProcessWithdrawal(model.AccountId, model.Amount, model.Description);
                if (!withdrawResult.Success)
                {
                    return Json(new { success = false, message = withdrawResult.Errors != null && withdrawResult.Errors.Any() ? string.Join(", ", withdrawResult.Errors) : "Withdrawal failed" });
                }

                return Json(new { success = true, message = "Withdrawal processed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing withdrawal for user {UserId}", GetCurrentUserId());
                return Json(new { success = false, message = "Withdrawal failed. Please try again." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBalance(int accountId)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                // Verify account ownership
                var accountResult = await _accountService.GetByIdAsync(accountId);
                if (!accountResult.Success || accountResult.Data == null)
                {
                    return Json(new { success = false, message = "Account not found" });
                }

                var account = accountResult.Data;
                if (account.UserId != int.Parse(userId))
                {
                    return Json(new { success = false, message = "Access denied" });
                }

                // Get balance using service layer
                var balanceResult = await _accountService.GetBalanceAsync(accountId);
                if (!balanceResult.Success)
                {
                    return Json(new { success = false, message = balanceResult.Errors != null && balanceResult.Errors.Any() ? string.Join(", ", balanceResult.Errors) : "Unable to get balance" });
                }

                return Json(new { success = true, balance = balanceResult.Data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting balance for account {AccountId}", accountId);
                return Json(new { success = false, message = "Unable to get balance" });
            }
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

    // Additional ViewModels
    public class AccountTransactionsViewModel
    {
        public AccountSummaryViewModel Account { get; set; } = new AccountSummaryViewModel();
        public List<TransactionSummaryViewModel> Transactions { get; set; } = new List<TransactionSummaryViewModel>();
    }

    public class AllTransactionsViewModel
    {
        public List<TransactionSummaryViewModel> Transactions { get; set; } = new List<TransactionSummaryViewModel>();
        public List<AccountSummaryViewModel> Accounts { get; set; } = new List<AccountSummaryViewModel>();
    }

    public class TransactionDetailsViewModel
    {
        public int TransactionId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public int AccountId { get; set; }
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public decimal BalanceAfterTransaction { get; set; }
    }

    public class CreateAccountViewModel
    {
        [Required(ErrorMessage = "Account type is required")]
        [Display(Name = "Account Type")]
        public string AccountType { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Initial deposit must be 0 or greater")]
        [Display(Name = "Initial Deposit")]
        public decimal InitialDeposit { get; set; }
    }

    public class DepositViewModel
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }

    public class WithdrawViewModel
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }

    public class TransferViewModel
    {
        [Required]
        public int FromAccountId { get; set; }

        [Required]
        [Display(Name = "To Account Number")]
        public string ToAccountNumber { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        public string? Description { get; set; }
    }

    public class AccountStatementViewModel
    {
        public Account Account { get; set; } = new Account();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public DateTime StatementDate { get; set; }
        public string StatementPeriod { get; set; } = string.Empty;
    }
}