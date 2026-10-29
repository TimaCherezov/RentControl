using FluentValidation;

namespace Main.Api.Features.Products.Patch;

public class PatchProductValidator : AbstractValidator<PatchProductRequest>
{
    public PatchProductValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50).When(x => x.Name is not null)
            .WithMessage("Name must be at most 50 characters");

        RuleFor(x => x.MarketValue)
            .GreaterThanOrEqualTo(0).When(x => x.MarketValue.HasValue)
            .WithMessage("MarketValue must be non-negative");

        RuleFor(x => x.DailyRate)
            .GreaterThanOrEqualTo(0).When(x => x.DailyRate.HasValue)
            .WithMessage("DailyRate must be non-negative");

        RuleFor(x => x.HourlyRate)
            .GreaterThanOrEqualTo(0).When(x => x.HourlyRate.HasValue)
            .WithMessage("HourlyRate must be non-negative");

        RuleFor(x => x.WeeklyRate)
            .GreaterThanOrEqualTo(0).When(x => x.WeeklyRate.HasValue)
            .WithMessage("WeeklyRate must be non-negative");

        RuleFor(x => x.MonthlyRate)
            .GreaterThanOrEqualTo(0).When(x => x.MonthlyRate.HasValue)
            .WithMessage("MonthlyRate must be non-negative");
    }
}
