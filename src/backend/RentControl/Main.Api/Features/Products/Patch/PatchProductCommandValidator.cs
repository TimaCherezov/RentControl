using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Products.Patch;

public class PatchProductCommandValidator : AbstractValidator<PatchProductCommand>
{
    public PatchProductCommandValidator()
    {
        this.AddRequiredIdRules(x => x.Id, "Id");

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

    private static bool HasAtLeastOneField(PatchProductCommand command)
    {
        return command.Name is not null
            || command.MarketValue.HasValue
            || command.DepositOverride.HasValue
            || command.DailyRate.HasValue
            || command.HourlyRate.HasValue
            || command.WeeklyRate.HasValue
            || command.MonthlyRate.HasValue
            || command.Description is not null;
    }
}
