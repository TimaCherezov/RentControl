using FluentValidation;

namespace Main.Api.Features.Products.Patch;

public class PatchProductRequestValidator : AbstractValidator<PatchProductRequest>
{
    public PatchProductRequestValidator()
    {
        RuleFor(x => x)
            .Must(HasAtLeastOneField)
            .WithMessage("At least one field must be provided");

        RuleFor(x => x.Name)
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .When(x => x.Name is not null)
            .WithMessage("Name cannot be empty");

        RuleFor(x => x.Name)
            .MaximumLength(50)
            .When(x => x.Name is not null)
            .WithMessage("Name must be at most 50 characters");

        RuleFor(x => x.MarketValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MarketValue.HasValue)
            .WithMessage("MarketValue must be non-negative");

        RuleFor(x => x.DepositOverride)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DepositOverride.HasValue)
            .WithMessage("DepositOverride must be non-negative");

        RuleFor(x => x.DailyRate)
            .GreaterThanOrEqualTo(0)
            .When(x => x.DailyRate.HasValue)
            .WithMessage("DailyRate must be non-negative");

        RuleFor(x => x.HourlyRate)
            .GreaterThanOrEqualTo(0)
            .When(x => x.HourlyRate.HasValue)
            .WithMessage("HourlyRate must be non-negative");

        RuleFor(x => x.WeeklyRate)
            .GreaterThanOrEqualTo(0)
            .When(x => x.WeeklyRate.HasValue)
            .WithMessage("WeeklyRate must be non-negative");

        RuleFor(x => x.MonthlyRate)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MonthlyRate.HasValue)
            .WithMessage("MonthlyRate must be non-negative");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null)
            .WithMessage("Description must be at most 1000 characters");
    }

    private static bool HasAtLeastOneField(PatchProductRequest request)
    {
        return request.Name is not null
            || request.MarketValue.HasValue
            || request.DepositOverride.HasValue
            || request.DailyRate.HasValue
            || request.HourlyRate.HasValue
            || request.WeeklyRate.HasValue
            || request.MonthlyRate.HasValue
            || request.Description is not null;
    }
}