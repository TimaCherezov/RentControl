using FluentValidation;
using Main.Api.Features.Validation;

namespace Main.Api.Features.Products.Update;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
	public UpdateProductCommandValidator()
	{
		this.AddRequiredIdRules(x => x.Id, "Id");

		this.AddRequiredStringRules(x => x.Name, "Name", 50);

		this.AddRequiredDecimalRules(x => x.MarketValue, "MarketValue");

		this.AddOptionalDecimalRules(x => x.DepositOverride, "DepositOverride");

		this.AddRequiredDecimalRules(x => x.DailyRate, "DailyRate");

		this.AddRequiredDecimalRules(x => x.HourlyRate, "HourlyRate");

		this.AddOptionalDecimalRules(x => x.WeeklyRate, "WeeklyRate");

		this.AddOptionalDecimalRules(x => x.MonthlyRate, "MonthlyRate");

		this.AddOptionalStringRules(x => x.Description, "Description", 1000);
	}
}
