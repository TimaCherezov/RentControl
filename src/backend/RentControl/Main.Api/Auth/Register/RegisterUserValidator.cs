using FluentValidation;

namespace Main.Api.Auth.Register;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Имя обязательно.")
            .MaximumLength(200)
            .WithMessage("Имя не может быть длиннее 200 символов.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email обязателен.")
            .EmailAddress()
            .WithMessage("Некорректный email.")
            .MaximumLength(320)
            .WithMessage("Email не может быть длиннее 320 символов.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Пароль обязателен.");
    }
}
