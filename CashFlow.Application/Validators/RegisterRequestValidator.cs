using FluentValidation;
using CashFlow.Application.DTO.Requests;

namespace CashFlow.Application.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(request => request.Password).NotEmpty();
            RuleFor(request => request.Password).MinimumLength(8);
            RuleFor(request => request.Password)
                .Matches("(?=.*\\d)")
                .WithMessage("Password must contain at least one number.");
            RuleFor(request => request.Password)
                .Matches("(?=.*[a-z])")
                .WithMessage("Password must contain at least one small letter.");
            RuleFor(request => request.Password)
                .Matches("(?=.*[A-Z])")
                .WithMessage("Password must contain at least one big letter.");
            RuleFor(request => request.Password)
                .Matches(@"(?=.*[!@#$%^&*_\-])")
                .WithMessage("Password must contain at least one special character.");

            RuleFor(request => request.Email).NotEmpty().EmailAddress();

            RuleFor(request => request.FirstName).NotEmpty();
            RuleFor(request => request.LastName).NotEmpty();
            RuleFor(request => request.Nickname).NotEmpty();
        }
    }
}