using CashFlow.Application.Interfaces;
using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Services
{
    public class RecTransactionService : IRecTransactionService
    {
        private readonly IRecTransactionRepository _recTransactionRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IKeyWordRepository _keyWordRepository;
        private readonly ITransactionService _transactionService;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public RecTransactionService(IRecTransactionRepository recTransactionRepository, ITransactionRepository transactionRepository, IAccountRepository accountRepository, IEmailService emailService, IUnitOfWork unitOfWork, IKeyWordRepository keyWordRepository, ITransactionService transactionService, IUserRepository userRepository, INotificationService notificationService)
        {
            _recTransactionRepository = recTransactionRepository;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _keyWordRepository = keyWordRepository;
            _transactionService = transactionService;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        private DateTime CalculateNextDate(DateTime nextPaymentDate, string frequency)
        {
            var cleanedFrequency = frequency.ToLower().Trim();

            switch (cleanedFrequency)
            {
                case "daily":
                    nextPaymentDate = nextPaymentDate.AddDays(1);
                    break;
                case "weekly":
                    nextPaymentDate = nextPaymentDate.AddDays(7);
                    break;
                case "monthly":
                    nextPaymentDate = nextPaymentDate.AddMonths(1);
                    break;
                case "yearly":
                    nextPaymentDate = nextPaymentDate.AddYears(1);
                    break;
                default:
                    break;
            }
            return nextPaymentDate;
        }

        public async Task<List<RecTransactionResponse>> GetAccountRecTransactionsAsync(int userId, int accountId)
        {
            var recTransactions = await _recTransactionRepository.GetAccountRecTransactionsWithDetailsAsync(userId, accountId);

            return recTransactions.Select(recTransaction => new RecTransactionResponse
            {
                RecTransactionId = recTransaction.RecTransactionId,
                Name = recTransaction.Name,
                Amount = recTransaction.Amount,
                Frequency = recTransaction.Frequency,
                StartDate = recTransaction.StartDate,
                EndDate = recTransaction.EndDate,
                Type = recTransaction.Type,
                NextPaymentDate = recTransaction.NextPaymentDate,

                AccountId = recTransaction.Account.AccountId,
                AccountName = recTransaction.Account.Name,

                CategoryId = recTransaction.Category.CategoryId,
                CategoryName = recTransaction.Category.Name
            }).ToList();
        }

        public async Task CreateRecTransactionAsync(int userId, NewRecTransactionRequest request)
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

                var newRecTransaction = new RecTransaction
                {
                    Type = request.Type,
                    Name = request.Name,
                    Frequency = request.Frequency,
                    IsTrue = request.isTrue,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    UserId = userId,
                    AccountId = (int)request.AccountId!,
                    CategoryId = (int)request.CategoryId!,
                    Amount = (decimal)request.Amount!,
                    Description = request.Description,
                    NextPaymentDate = request.NextPaymentDate
                };

                await _recTransactionRepository.AddAsync(newRecTransaction);
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task ProcessPendingTransactionsAsync()
        {
            var recTransactionsPending = await _recTransactionRepository.GetPendingRecurringTransactionsWithDetailsAsync(DateTime.UtcNow);

            var userSummaries = new Dictionary<int, List<string>>();

            foreach (var recTransactionPending in recTransactionsPending)
            {
                using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
                try
                {
                    if (recTransactionPending.User == null || recTransactionPending.Account == null) continue;

                    if (!userSummaries.ContainsKey(recTransactionPending.UserId))
                    {
                        userSummaries[recTransactionPending.UserId] = new List<string>();
                    }

                    if (recTransactionPending.IsTrue == true)
                    {
                        var transactionRequest = new NewTransactionRequest()
                        {
                            AccountId = recTransactionPending.AccountId,
                            CategoryId = recTransactionPending.CategoryId,
                            Amount = recTransactionPending.Amount,
                            Description = "(Automat) " + recTransactionPending.Description,
                            Type = recTransactionPending.Type
                        };

                        await _transactionService.CreateNewTransactionAsync(recTransactionPending.UserId, transactionRequest, false);

                        userSummaries[recTransactionPending.UserId].Add($"Added: {recTransactionPending.Name} ({recTransactionPending.Amount})");
                        //await _notificationService.SendNotificationAsync(recTransactionPending.UserId, "Recurring transaction has been added", $"We have added new transaction automatically based on your recurring transaction to account: {recTransactionPending.Account.Namee}! Reccurring transaction name: {recTransactionPending.Name}", "info");
                    }
                    else if (recTransactionPending.IsTrue == false)
                    {
                        userSummaries[recTransactionPending.UserId].Add($"To add: {recTransactionPending.Name} ({recTransactionPending.Amount})");
                        //await _notificationService.SendNotificationAsync(recTransactionPending.UserId, $"You have recurring transaction to add", $"Hello, {recTransactionPending.User.FirstName} {recTransactionPending.User.LastName} You have a recurring transaction to add today: {DateTime.UtcNow.Date} in account: {recTransactionPending.Account.Name}. Reccurring transaction name: {recTransactionPending.Name} Remember to add it manually!", "info");
                    }

                    recTransactionPending.NextPaymentDate = CalculateNextDate(recTransactionPending.NextPaymentDate, recTransactionPending.Frequency);
                    recTransactionPending.UpdatedAt = DateTime.UtcNow;

                    await _recTransactionRepository.UpdateAsync(recTransactionPending);
                    await dbTransaction.CommitAsync();
                }
                catch(Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    Console.WriteLine(ex);
                }

            }

            foreach (var summary in userSummaries)
            {
                var userId = summary.Key;
                var messages = summary.Value;

                string title = $"Today recurring transaction raport for: ({messages.Count}) transaction/s";
                string body = string.Join("\n", messages);

                await _notificationService.SendNotificationAsync(userId, title, $"There is list for recurring transactions on your account" + "\n" + body, "info");
            }

            await ProcessUpcomingRemindersAsync();
        }

        public async Task ProcessUpcomingRemindersAsync()
        {
            var recTransactionsUpcoming = await _recTransactionRepository.GetUpcomingRecurringTransactionsWithDetailsAsync(DateTime.UtcNow.AddDays(1));

            var userSummaries = new Dictionary<int, List<string>>();

            foreach (var recTransactionUpcoming in recTransactionsUpcoming)
            {
                if (recTransactionUpcoming.User == null || recTransactionUpcoming.Account == null) continue;

                if (!userSummaries.ContainsKey(recTransactionUpcoming.UserId))
                {
                    userSummaries[recTransactionUpcoming.UserId] = new List<string>();
                }

                userSummaries[recTransactionUpcoming.UserId].Add($"Tomorrow: {recTransactionUpcoming.Name} ({recTransactionUpcoming.Amount})");
                //await _notificationService.SendNotificationAsync(recTransactionUpcoming.UserId, "There is upcoming recurring transaction", $"Hello, {recTransactionUpcoming.User.FirstName} {recTransactionUpcoming.User.LastName} There will be a recurring transaction tomorrow for account: {recTransactionUpcoming.Account.Name} Reccurring transaction name: {recTransactionUpcoming.Name}", "info");
            }

            foreach (var summary in userSummaries)
            {
                var userId = summary.Key;
                var messages = summary.Value;

                string title = $"Upcoming recurring transaction raport for: ({messages.Count}) transaction/s";
                string body = string.Join("\n", messages);

                await _notificationService.SendNotificationAsync(userId, title, "There is list for upcoming recurring transactions on your account" + "\n" + body, "info");
            }
        }
    }
}