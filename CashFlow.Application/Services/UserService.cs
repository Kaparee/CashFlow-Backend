using CashFlow.Application.Interfaces;
using CashFlow.Application.Repositories;
using CashFlow.Domain.Models;
using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;

namespace CashFlow.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJWTService _jwtService;
        private readonly IEmailService _emailService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IKeyWordRepository _keyWordRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IJWTService jwtService, IEmailService emailService, ICategoryRepository categoryRepository, IKeyWordRepository keyWordRepository, IAccountRepository accountRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _emailService = emailService;
            _categoryRepository = categoryRepository;
            _keyWordRepository = keyWordRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            var isEmailTaken = await _userRepository.IsEmailTakenAsync(request.Email!);
            var isNicknameTaken = await _userRepository.IsNicknameTakenAsync(request.Nickname!);

            if (isEmailTaken == true || isNicknameTaken == true)
            {
                throw new Exception($"Given email or nickname is taken!");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var userGUID = Guid.NewGuid().ToString();

            var newUser = new User
            {
                FirstName = request.FirstName!,
                LastName = request.LastName!,
                Email = request.Email!,
                Nickname = request.Nickname!,
                PasswordHash = passwordHash,
                IsAdmin = false,
                IsActive = true,
                IsVerified = false,
                VerificationToken = userGUID,
                VerifiedAt = null,
            };

            

            await _userRepository.AddAsync(newUser);
            var backendUrl = _configuration["AppUrls:BackendUrl"];
            await _emailService.SendEmailAsync(request.Email!, "Welcome to CashFlow!", $"<h1>Welcome to CashFlow {request.FirstName} {request.LastName}.</h1><br>Please click the link below to activate your account!<br><a href=\"{backendUrl}/api/verify?verificationToken={userGUID}\">VERIFY HERE</a>");
            await SeedUserDataAsync(newUser.UserId);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmailOrNicknameAsync(request.EmailOrNickname!);

            if (user is null) { throw new Exception("User does not exist!"); }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) { throw new Exception("Password is invalid!"); }

            if(!user.IsActive || !user.IsVerified)
            {
                throw new Exception("Account is not active or not verified.");
            }

            var token = _jwtService.GenerateTokenAsync(user);

            LoginResponse response = new LoginResponse();
            response.Token = token;

            return response;
        }

        public async Task<UserResponse> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);

            if(user == null)
            {
                throw new Exception("User was not found");
            }

            return new UserResponse
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Nickname = user.Nickname,
                Email = user.Email,
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin,
                IsVerified = user.IsVerified,
                PhotoUrl = user.PhotoUrl,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
            };
        }

        public async Task VerifyEmailAsync(string verificationToken)
        {
            var user = await _userRepository.GetUserByVerificationTokenAsync(verificationToken);

            if (user == null)
            {
                throw new Exception("Invalid token");
            }

            user.IsVerified = true;
            user.VerifiedAt = DateTime.UtcNow;
            user.VerificationToken = null;

            await _userRepository.UpdateAsync(user);
        }

        public async Task SeedUserDataAsync(int userId)
        {
            var defaultCategories = new List<(string Name, string Icon, string Color, string Type, string[] Keywords)>
            {
                ("Jedzenie", "fastfood", "#FF5733", "expense", new[] { "Biedronka", "Lidl", "KFC", "McDonalds", "¯abka" }),
                ("Transport", "directions_car", "#33B5FF", "expense", new[] { "Shell", "Orlen", "Uber", "Bolt", "ZTM", "PKP" }),
                ("Wynagrodzenie", "payments", "#28B463", "income", new[] { "Przelew", "Salary", "Premia", "Wynagrodzenie" }),
                ("Rozrywka", "theater_comedy", "#AF7AC5", "expense", new[] { "Netflix", "Spotify", "Cinema", "Kino", "Pub" }),
                ("Zdrowie", "medical_services", "#E74C3C", "expense", new[] { "Apteka", "Dentysta", "Lekarz", "Luxmed" })
            };

            var defaultAccounts = new List<(string Name, decimal Balance, string CurrencyCode, bool IsActive, string PhotoUrl)>
            {
                ("G³ówne Konto", 0.00m, "PLN", true, "default_account_url")
            };

            foreach (var accountData in defaultAccounts)
            {
                var newAccount = new Account
                {
                    UserId = userId,
                    Name = accountData.Name,
                    Balance = accountData.Balance,
                    CurrencyCode = accountData.CurrencyCode,
                    IsActive = accountData.IsActive,
                    PhotoUrl = accountData.PhotoUrl,
                };
                await _accountRepository.AddAsync(newAccount);
            }

            foreach(var item in defaultCategories)
            {
                var category = new Category
                {
                    UserId = userId,
                    Name = item.Name,
                    Icon = item.Icon,
                    Color = item.Color,
                    Type = item.Type
                };
                await _categoryRepository.AddAsync(category);

                foreach(var word in item.Keywords)
                {
                    await _keyWordRepository.AddAsync(new KeyWord
                    {
                        UserId = userId,
                        CategoryId = category.CategoryId,
                        Word = word
                    });
                }
            }
        }

        public async Task DeleteUserAsync(int userId)
        {
            using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);

                if (user == null)
                {
                    throw new Exception("User not found or access denied.");
                }

                user.IsActive = false;
                user.DeletedAt = DateTime.UtcNow;

                foreach (var account in user.Accounts)
                {
                    account.DeletedAt = DateTime.UtcNow;
                    account.IsActive = false;
                }
                foreach (var transaction in user.Transactions)
                {
                    transaction.DeletedAt = DateTime.UtcNow;
                }

                await _userRepository.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();
                await dbTransaction.CommitAsync();
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateUserAsync(int userId, UpdateUserRequest request)
        {
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found or access denied.");
            }

            if (user.Nickname != request.NewNickname)
            {
                if (await _userRepository.IsNicknameTakenAsync(request.NewNickname))
                    throw new Exception("This nickname is already taken.");
            }

            user.UpdatedAt = DateTime.UtcNow;

            user.FirstName = request.NewFirstName;
            user.LastName = request.NewLastName;
            user.Nickname = request.NewNickname;
            user.PhotoUrl = request.NewPhotoUrl;

            await _userRepository.UpdateAsync(user);
        }

        public async Task ModifyPasswordAsync(int userId, ModifyPasswordRequest request)
        {
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found or access denied.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                throw new Exception("Current password is invalid.");
            }

            if (request.OldPassword == request.NewPassword)
            {
                throw new Exception("New password cannot be same as the old password.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _emailService.SendEmailAsync(user.Email, "Reset Password - CashFlow", "<h1>Your password has been changed, thank you for supporting our project</h1>");
        }

        public async Task RequestPasswordResetAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailOrNicknameAsync(email);
            if (user == null)
            {
                return;
            }

            var resetGUID = Guid.NewGuid().ToString();
            user.PasswordResetToken = resetGUID;
            user.ResetTokenExpiresAt = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateAsync(user);
            var frontendUrl = _configuration["AppUrls:FrontendUrl"];
            await _emailService.SendEmailAsync(user.Email, "Reset Password - CashFlow", $"<h1>Welcome {user.FirstName} {user.LastName}</h1><br>To reset, click the link below:<br> <a href=\"{frontendUrl}/api/confirm-password-reset?token={resetGUID}\">RESET PASSWORD</a>");
        }

        public async Task ResetPasswordConfirmAsync(ResetPasswordRequest request)
        {
            var user = await _userRepository.GetUserByPasswordResetTokenAsync(request.Token);

            if (user == null || user.ResetTokenExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Invalid or expired token.");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordResetToken = null;
            user.ResetTokenExpiresAt = null;

            await _userRepository.UpdateAsync(user);
            await _emailService.SendEmailAsync(user.Email, "Reset Password - CashFlow", "<h1>Your password has been changed, thank you for supporting our project</h1>");
        }

        public async Task RequestEmailChangeAsync(int userId, string newEmail )
        {
            var user = await _userRepository.GetUserByIdWithDetailsAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found or access denied.");
            }

            if(await _userRepository.IsEmailTakenAsync(newEmail))
            {
                throw new Exception("This email is already taken.");
            }

            var token = Guid.NewGuid().ToString();

            user.NewEmailPending = newEmail;
            user.EmailChangeToken = token;
            user.EmailChangeTokenExpiresAt = DateTime.UtcNow.AddHours(1);

            await _userRepository.UpdateAsync(user);
            var frontendUrl = _configuration["AppUrls:FrontendUrl"];
            await _emailService.SendEmailAsync(newEmail, "Confirm your new email - CashFlow", $"<h1>Welcome {user.FirstName} {user.LastName}</h1><br>To confirm your new Email, click the link below:<br> <a href=\"{frontendUrl}/api/confirm-email-change?token={token}\">CHANGE EMAIL</a>");
        }

        public async Task EmailChangeConfirmAsync(string token)
        {
            var user = await _userRepository.GetUserByEmailChangeTokenAsync(token);

            if (user == null || user.EmailChangeTokenExpiresAt < DateTime.UtcNow)
            {
                throw new Exception("Invalid or expired email change token.");
            }

            user.Email = user.NewEmailPending!;

            user.NewEmailPending = null;
            user.EmailChangeToken = null;
            user.EmailChangeTokenExpiresAt = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _emailService.SendEmailAsync(user.Email, "Email Changed - CashFlow", "<h1>Your Email has been changed, thank you for supporting our project</h1>");
            await _userRepository.UpdateAsync(user);
        }
    }
}