using CashFlow.Application.Interfaces;
using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;
using BCrypt.Net;

namespace CashFlow.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IAccountRepository accountRepository, ITransactionRepository transactionRepository, IUnitOfWork unitOfWork)
        {
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AccountResponse>> GetUserAccountsAsync(int userId)
        {
            var accounts = await _accountRepository.GetUserAccountsWithDetailsAsync(userId);

            if (accounts == null || !accounts.Any())
            {
                throw new Exception("User does not have any accounts");
            }

            return accounts.Select(account => new AccountResponse
            {
                AccountId = account.AccountId!,
                Name = account.Name!,
                Balance = account.Balance!,
                CurrencyCode = account.CurrencyCode!,
                PhotoUrl = account.PhotoUrl!,
            }).ToList();
        }

        public async Task CreateNewAccountAsync(int userId, NewAccountRequest request)
        {
            var isAccountCreated = await _accountRepository.isAccountCreated(userId!, request.Name!);

            if (isAccountCreated == true)
            {
                throw new Exception("Given account name is already created in your profile");
            }

            var newAccount = new Account
            {
                UserId = userId!,
                Name = request.Name!,
                Balance = (decimal)request.Balance!,
                CurrencyCode = request.CurrencyCode!,
                PhotoUrl = request.PhotoUrl!,
                IsActive = true,
            };

            await _accountRepository.AddAsync(newAccount);
        }

        public async Task DeleteAccountAsync(int userId, int accountId)
        {
            using var dbTransaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var account = await _accountRepository.GetAccountByIdAsync(userId, accountId);
                var transactions = await _transactionRepository.GetAccountTransactionsWithDetailsAsync(userId, accountId);

                if (account == null)
                {
                    throw new Exception("Account not found or access denied.");
                }

                account.IsActive = false;
                account.DeletedAt = DateTime.UtcNow;

                foreach (var transaction in transactions)
                {
                    transaction.DeletedAt = DateTime.UtcNow;
                }

                await _accountRepository.UpdateAsync(account);
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAccountAsync(int userId, UpdateAccountRequest request)
        {
            var account = await _accountRepository.GetAccountByIdAsync(userId, request.AccountId);

            if(account == null)
            {
                throw new Exception("Account not found or access denied");
            }

            account.UpdatedAt = DateTime.UtcNow;

            account.Name = request.NewName;
            account.PhotoUrl = request.NewPhotoUrl;

            await _accountRepository.UpdateAsync(account);
        }
    }
}