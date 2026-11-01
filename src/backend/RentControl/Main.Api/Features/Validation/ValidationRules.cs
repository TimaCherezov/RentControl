using FluentValidation;

namespace Main.Api.Features.Validation;

public static class ValidationRules
{
    extension<T>(IRuleBuilder<T, string?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, string?> RequiredString(int maxLength)
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("{PropertyName} is required")
                .MaximumLength(maxLength)
                .WithMessage($"{{PropertyName}} must be at most {maxLength} characters");
        }

        public IRuleBuilderOptions<T, string?> OptionalString(int maxLength)
        {
            return ruleBuilder
                .MaximumLength(maxLength)
                .WithMessage($"{{PropertyName}} must be at most {maxLength} characters");
        }

        public IRuleBuilderOptions<T, string?> OptionalNonEmptyString(int maxLength)
        {
            return ruleBuilder
                .Must(value => value is null || !string.IsNullOrWhiteSpace(value))
                .WithMessage("{PropertyName} cannot be empty")
                .MaximumLength(maxLength)
                .WithMessage($"{{PropertyName}} must be at most {maxLength} characters");
        }
    }

    extension<T>(IRuleBuilder<T, decimal> ruleBuilder)
    {
        public IRuleBuilderOptions<T, decimal> NonNegative()
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0)
                .WithMessage("{PropertyName} must be non-negative");
        }
    }

    extension<T>(IRuleBuilder<T, decimal?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, decimal?> RequiredNonNegative()
        {
            return ruleBuilder
                .NotNull()
                .WithMessage("{PropertyName} is required")
                .GreaterThanOrEqualTo(0)
                .WithMessage("{PropertyName} must be non-negative");
        }

        public IRuleBuilderOptions<T, decimal?> OptionalNonNegative()
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0)
                .WithMessage("{PropertyName} must be non-negative");
        }
    }

    extension<T>(IRuleBuilder<T, Guid> ruleBuilder)
    {
        public IRuleBuilderOptions<T, Guid> RequiredId()
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("{PropertyName} is required");
        }
    }
}