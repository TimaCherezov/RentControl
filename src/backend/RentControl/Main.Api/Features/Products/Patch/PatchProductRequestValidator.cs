using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Products.Patch;

public class PatchProductRequestValidator : AbstractValidator<PatchProductRequest>
{
    public PatchProductRequestValidator()
    {
        RuleFor(x => x)
            .Must(HasAtLeastOneField)
            .WithMessage("At least one field must be provided");

        this.AddOptionalNonEmptyStringRules(x => x.Name, "Name", 50);

        this.AddOptionalDecimalRules(x => x.MarketValue, "MarketValue");

        this.AddOptionalDecimalRules(x => x.DepositOverride, "DepositOverride");

        this.AddOptionalDecimalRules(x => x.DailyRate, "DailyRate");

        this.AddOptionalDecimalRules(x => x.HourlyRate, "HourlyRate");

        this.AddOptionalDecimalRules(x => x.WeeklyRate, "WeeklyRate");

        this.AddOptionalDecimalRules(x => x.MonthlyRate, "MonthlyRate");

        this.AddOptionalStringRules(x => x.Description, "Description", 1000);
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
