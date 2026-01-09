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


        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, IKeyWordRepository keyWordRepository, IUnitOfWork unitOfWork, ILimitRepository limitRepository, INotificationService notificationService, ICategoryRepository categoryRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _keyWordRepository = keyWordRepository;
            _unitOfWork = unitOfWork;
            _limitRepository = limitRepository;
            _notificationService = notificationService;
            _categoryRepository = categoryRepository;
        }

        public async Task CreateNewTransactionAsync(int userId, NewTransactionRequest request)
        {
            using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
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

                if (request.CategoryId == null)
                {
                    throw new Exception("CategoryId is required (auto-match failed).");
                }

                if (request.Amount <= 0 || request.Amount == null)
                {
                    throw new Exception("Transaction must be greater than 0");
                }
                if (request.AccountId == null)
                {
                    throw new Exception("AccountId is required to create a transaction");
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
                    var categoryId = request.CategoryId.Value;
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

                            var spentAmount = await _transactionRepository.GetCategorySpendingsAsync(userId, limit.CategoryId, limit.StartDate, limit.EndDate);
                            spentAmount += (decimal)request.Amount!;

                            if (spentAmount > limit.Value)
                            {
                                await _notificationService.SendNotificationAsync(userId, $"Spending limit exceeded for limit: {limit.Name}", $"You reached your limit for category: {category.Name!} on account: {account.Name}! Your reached {spentAmount}/{limit.Value}", "info");
                            }
                            else if (spentAmount > (limit.Value * 0.8m))
                            {
                                await _notificationService.SendNotificationAsync(userId, $"You are close to exceeding the limit for the limit: {limit.Name}", $"You nearly reached your limit for category: {category.Name!} on account: {account.Name}! Your reached {spentAmount}/{limit.Value}", "info");
                            }
                        }
                    }
                }
            }
            catch
            {
                await dbTransaction.RollbackAsync();
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
    }
}