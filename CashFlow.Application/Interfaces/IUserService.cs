using CashFlow.Application.DTO.Requests;
using CashFlow.Application.DTO.Responses;

namespace CashFlow.Application.Interfaces
{
	public interface IUserService
	{
		Task RegisterAsync(RegisterRequest request);
		Task<LoginResponse> LoginAsync(LoginRequest request);
		Task<UserResponse> GetUserByIdAsync(int userId);
		Task VerifyEmailAsync(string verificationToken);
		Task DeleteUserAsync(int userId);
		Task UpdateUserAsync(int userId, UpdateUserRequest request);
		Task ModifyPasswordAsync(int userId, ModifyPasswordRequest request);
		Task RequestPasswordResetAsync(string email);
        Task ResetPasswordConfirmAsync(ResetPasswordRequest request);
		Task RequestEmailChangeAsync(int userId, string newEmail);
        Task EmailChangeConfirmAsync(string token);
    }
}