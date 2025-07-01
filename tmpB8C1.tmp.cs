//[HttpGet]
//public async Task<IActionResult> AllTransactions()
//{
//    try
//    {
//        var userId = GetCurrentUserId();
//        if (userId == null)
//        {
//            return RedirectToAction("Login", "Auth");
//        }

//        // Get all user accounts  
//        var accountsResult = await _accountService.GetByUserIdAsync(int.Parse(userId));
//        if (!accountsResult.Success)
//        {
//            _logger.LogError("Failed to get accounts for user {UserId}: {Error}", userId, accountsResult.Errors);
//            TempData["ErrorMessage"] = "Unable to load accounts. Please try again.";
//            return RedirectToAction("Dashboard");
//        }

//        var accounts = accountsResult.Data ?? new List<AccountDto>();

//        // Get all transactions for all user accounts  
//        var allTransactions = new List<TransactionSummaryViewModel>();
//        foreach (var account in accounts)
//        {
//            var accountTransactions = _transactionRepository.GetByAccountId(account.AccountId);
//            allTransactions.AddRange(accountTransactions.Select(t => new TransactionSummaryViewModel
//            {
//                TransactionId = t.TransactionId,
//                Description = t.Description ?? "Transaction",
//                Amount = t.Amount,
//                TransactionType = t.Amount > 0 ? "Credit" : "Debit",
//                TransactionDate = t.TransactionDate,
//                AccountId = t.AccountId
//            }));
//        }

//        // Sort by date (most recent first)  
//        allTransactions = allTransactions
//            .OrderByDescending(t => t.TransactionDate)
//            .ToList();

//        // Fix: Initialize the `Accounts` property correctly  
//        var viewModel = new AllTransactionsViewModel
//        {
//            Transactions = allTransactions,
//            Accounts = accounts.Select(a => new AccountSummaryViewModel
//            {
//                AccountId = a.AccountId,
//                AccountNumber = a.AccountNumber,
//                AccountType = a.Type.ToString(),
//                Balance = a.Balance,
//                CreatedAt = a.CreatedAt,
//                UpdatedAt = a.UpdatedAt
//            }).ToList()
//        };

//        return View(viewModel);
//    }
//    catch (Exception ex)
//    {
//        _logger.LogError(ex, "Error loading all transactions for user {UserId}", GetCurrentUserId());
//        TempData["ErrorMessage"] = "Unable to load transactions. Please try again.";
//        return RedirectToAction("Dashboard");
//    }
//}
