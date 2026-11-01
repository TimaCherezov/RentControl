using FluentValidation;
using FluentValidation.Results;
using MassTransit;

namespace Main.Api.Features.Validation;

public sealed class CommandValidationFilter<T>(IEnumerable<IValidator<T>> validators) : IFilter<SendContext<T>>
	where T : class
{
	private readonly IReadOnlyCollection<IValidator<T>> _validators = validators.ToArray();

	public async Task Send(
		SendContext<T> context,
		IPipe<SendContext<T>> next)
	{
		var failures = new List<ValidationFailure>();

		foreach (var validator in _validators)
		{
			var validationContext = new ValidationContext<T>(context.Message);

			var result = await validator.ValidateAsync(
				validationContext,
				context.CancellationToken);

			failures.AddRange(
				result.Errors.Where(error => error is not null));
		}

		if (failures.Count > 0)
		{
			throw new ValidationException(failures);
		}

		await next.Send(context);
	}

	public void Probe(ProbeContext context)
	{
		context.CreateFilterScope("commandValidation");
	}
}