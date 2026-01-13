using CashFlow.Application.Interfaces;
using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IKeyWordRepository _keyWordRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILimitRepository _limitRepository;
        private readonly INotificationService _notificationService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrencyService _currencyService;


        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IKeyWordRepository keyWordRepository, IUnitOfWork unitOfWork, ILimitRepository limitRepository, INotificationService notificationService, ICategoryRepository categoryRepository, ICurrencyService currencyService)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _keyWordRepository = keyWordRepository;
            _unitOfWork = unitOfWork;
            _limitRepository = limitRepository;
            _notificationService = notificationService;
            _categoryRepository = categoryRepository;
            _currencyService = currencyService;
        }

        public async Task CreateNewTransactionAsync(int userId, NewTransactionRequest request, bool useTransaction = true)
        {
            using var dbTransaction = useTransaction ? await _unitOfWork.BeginTransactionAsync() : null;
            try
            {
                if (request.CategoryId == null)
                {
                    if (request.Description == null)
                    {
                        request.Description = string.Empty;
                    }
                    request.CategoryId = await _keyWordRepository.GetCategoryIdByDescriptionAsync(userId, request.Description);
                }

                if (request.Amount <= 0 || request.Amount == null)
                {
                    throw new Exception("Transaction must be greater than 0");
                }
                if (request.AccountId == null)
                {
                    throw new Exception("AccountId is required to create a transaction");
                }
                if (request.CategoryId == null)
                {
                    throw new Exception("CategoryId is required to create a transaction");
                }

                var account = await _accountRepository.GetAccountByIdAsync(userId, (int)request.AccountId!);

                if (account == null)
                {
                    throw new Exception("Account not found or access denied.");
                }

                var transactionType = request.Type!.ToLower();

                if (transactionType == "expense")
                {
                    if (account.Balance < request.Amount)
                    {
                        throw new Exception("You can not have less than zero money");
                    }
                    account.Balance -= (decimal)request.Amount!;
                }
                if (transactionType == "income")
                {
                    account.Balance += (decimal)request.Amount!;
                }

                var newTransaction = new Transaction
                {
                    UserId = userId!,
                    AccountId = (int)request.AccountId!,
                    CategoryId = request.CategoryId.Value,
                    Amount = (decimal)request.Amount!,
                    Description = request.Description!,
                    Type = request.Type!
                };

                await _transactionRepository.AddAsync(newTransaction);
                await _accountRepository.UpdateAsync(account);

                if (transactionType == "expense")
                {
                    var categoryId = newTransaction.CategoryId;
                    var limits = await _limitRepository.GetLimitsForCategoryAsync(categoryId);

                    foreach (var limit in limits)
                    {
                        if (DateTime.UtcNow >= limit.StartDate && DateTime.UtcNow <= limit.EndDate)
                        {
                            var category = await _categoryRepository.GetCategoryInfoByIdWithDetailsAsync(userId, limit.CategoryId);

                            if (category == null)
                            {
                                continue;
                            }

                            var spentAmount = await _transactionRepository.GetCategorySpendingsAsync(userId, limit.CategoryId, limit.AccountId, limit.StartDate, limit.EndDate);

                            if (spentAmount > limit.Value)
                            {
                                await _notificationService.SendNotificationAsync(userId, $"Spending limit exceeded for limit: {limit.Name}", $"You reached your limit for category: {category.Name!} on account: {account.Name}! You reached {spentAmount}/{limit.Value}", "info");
                            }
                            else if (spentAmount > (limit.Value * 0.8m))
                            {
                                await _notificationService.SendNotificationAsync(userId, $"You are close to exceeding the limit for the limit: {limit.Name}", $"You nearly reached your limit for category: {category.Name!} on account: {account.Name}! You reached {spentAmount}/{limit.Value}", "info");
                            }
                        }
                    }
                }
                if (dbTransaction != null)
                {
                    await dbTransaction.CommitAsync();
                }
            }
            catch
            {
                if (dbTransaction != null)
                {
                    await dbTransaction.RollbackAsync()!;
                }
                throw;
            }
		}

        public async Task<List<TransactionResponse>> GetAccountTransactionsAsync(int userId, int accountId)
        {
            var transactions = await _transactionRepository.GetAccountTransactionsWithDetailsAsync(userId, accountId);

            return transactions.Select(transaction => new TransactionResponse
            {
                TransactionId = transaction.TransactionId,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Date = transaction.Date,
                Type = transaction.Type,

                Category = transaction.Category == null ? null : new CategoryResponse
                {
                    CategoryId = transaction.Category.CategoryId,
                    Name = transaction.Category.Name,
                    Color = transaction.Category.Color,
                    Type = transaction.Category.Type,
                    Icon = transaction.Category.Icon
                }
            }).ToList();
        }

        public async Task DeleteTransactionAsync(int userId, int transactionId, int accountId)
        {
            using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var transaction = await _transactionRepository.GetTransactionInfoByIdWithDetailsAsync(userId, transactionId);
                var account = await _accountRepository.GetAccountByIdAsync(userId, accountId);

                if (transaction == null || account == null)
                {
                    throw new Exception("Transaction not found or access denied.");
                }

                transaction.DeletedAt = DateTime.UtcNow;

                if (transaction.Type == "income")
                {
                    account.Balance -= transaction.Amount;
                }

                if (transaction.Type == "expense")
                {
                    account.Balance += transaction.Amount;
                }

                await _transactionRepository.UpdateAsync(transaction);
                await _accountRepository.UpdateAsync(account);
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateTransactionAsync(int userId, UpdateTransactionRequest request)
        {
            using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var transaction = await _transactionRepository.GetTransactionInfoByIdWithDetailsAsync(userId, request.TransactionId);
                var account = await _accountRepository.GetAccountByIdAsync(userId, request.AccountId);

                if (transaction == null || account == null)
                {
                    throw new Exception("Transaction not found or access denied.");
                }

                transaction.UpdatedAt = DateTime.UtcNow;

                if (transaction.Type == "expense")
                {
                    account.Balance += transaction.Amount;
                }
                else if (transaction.Type == "income")
                {
                    account.Balance -= transaction.Amount;
                }

                transaction.Amount = request.NewAmount;
                transaction.Type = request.NewType;
                transaction.Description = request.NewDescription;
                transaction.CategoryId = request.NewCategoryId;
                transaction.Date = request.NewDate;

                if (transaction.Type == "expense")
                {
                    account.Balance -= transaction.Amount;
                }
                else if (transaction.Type == "income")
                {
                    account.Balance += transaction.Amount;
                }

                await _transactionRepository.UpdateAsync(transaction);
                await _accountRepository.UpdateAsync(account);
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<CategoryAnalyticsResponse>> GetCategoryAnalyticsAsync(int userId, DateTime startDate, DateTime endDate, string type)
        {
            var transactions = await _transactionRepository.GetTransactionsForAnalyticsAsync(userId, startDate, endDate, type);

            if (!transactions.Any()) return new List<CategoryAnalyticsResponse>();

            var uniqueCurrencies = transactions.Select(t => t.Account.CurrencyCode).Distinct();
            var rates = new Dictionary<string, decimal>();
            foreach (var code in uniqueCurrencies)
            {
                rates[code] = await _currencyService.GetExchangeRateAsync(code, "PLN");
            }

            var normalizedTransactions = transactions.Select(t => new {
                t.Category,
                AmountInPLN = t.Amount * (rates.ContainsKey(t.Account.CurrencyCode) ? rates[t.Account.CurrencyCode] : 1.0m)
            }).ToList();

            decimal totalAmount = normalizedTransactions.Sum(t => t.AmountInPLN);

            var analytics = normalizedTransactions
                .GroupBy(t => t.Category)
                .Select(g => new CategoryAnalyticsResponse
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    Color = g.Key.Color ?? "#808080",
                    TotalValue = g.Sum(t => t.AmountInPLN),
                    Percentage = totalAmount == 0 ? 0 : Math.Round((g.Sum(t => t.AmountInPLN) / totalAmount) * 100, 2)
                })
                .OrderByDescending(x => x.TotalValue)
                .ToList();

            return analytics;
        }

        public async Task<List<MonthlyAnalyticsResponse>> GetMonthlyAnalyticsAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(userId, startDate, endDate);

            if (!transactions.Any()) return new List<MonthlyAnalyticsResponse>();

            var uniqueCurrencies = transactions.Select(t => t.Account.CurrencyCode).Distinct();
            var rates = new Dictionary<string, decimal>();
            foreach (var code in uniqueCurrencies)
            {
                rates[code] = await _currencyService.GetExchangeRateAsync(code, "PLN");
            }

            var analytics = transactions
                .Select(t => new {
                    t.Date,
                    t.Type,
                    AmountInPLN = t.Amount * (rates.ContainsKey(t.Account.CurrencyCode) ? rates[t.Account.CurrencyCode] : 1.0m)
                })
                .GroupBy(t => new { t.Date.Month, t.Date.Year })
                .OrderBy(g => g.Key.Year)
                .ThenBy(g => g.Key.Month)
                .Select(g => new MonthlyAnalyticsResponse
                {
                    MonthNumber = g.Key.Month,
                    TotalIncomeAmount = g.Where(t => t.Type == "income").Sum(t => t.AmountInPLN),
                    TotalExpenseAmount = g.Where(t => t.Type == "expense").Sum(t => t.AmountInPLN),
                    Balance = g.Where(t => t.Type == "income").Sum(t => t.AmountInPLN) - g.Where(t => t.Type == "expense").Sum(t => t.AmountInPLN)
                })
                .ToList();
            return analytics;
        }

        public async Task<List<DailyAnalyticsResponse>> GetDailyAnalyticsAsync(int userId, DateTime startDate, DateTime endDate)
        {
            var transactions = await _transactionRepository.GetTransactionsByDateRangeAsync(userId, startDate, endDate);

            if (!transactions.Any())
            {
                return new List<DailyAnalyticsResponse>();
            }

            var uniqueCurrencies = transactions.Select(t => t.Account.CurrencyCode).Distinct();
            var rates = new Dictionary<string, decimal>();

            foreach (var code in uniqueCurrencies)
            {
                rates[code] = await _currencyService.GetExchangeRateAsync(code, "PLN");
            }

            var analytics = transactions
                .Select(t => new { t.Date, t.Type, AmountInPLN = t.Amount * (rates.ContainsKey(t.Account.CurrencyCode) ? rates[t.Account.CurrencyCode] : 1.0m) })
                .GroupBy(t => t.Date.Date)
                .OrderBy(g => g.Key)
                .Select(g => new DailyAnalyticsResponse
                {
                    Date = g.Key,
                    Income = g.Where(t => t.Type == "income").Sum(t => t.AmountInPLN),
                    Expense = g.Where(t => t.Type == "expense").Sum(t => t.AmountInPLN),
                    Balance = g.Where(t => t.Type == "income").Sum(t => t.AmountInPLN) - g.Where(t => t.Type == "expense").Sum(t => t.AmountInPLN)
                })
                .ToList();
            return analytics;
        }
    }
}