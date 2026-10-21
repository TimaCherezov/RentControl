using FluentValidation;

namespace Main.Api.Auth.Login;

public class LoginUserValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email обязателен.")
            .EmailAddress()
            .WithMessage("Некорректный email.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Пароль обязателен.");
    }
}
