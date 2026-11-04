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
        public IRuleBuilderOptions<T, Guid> Id()
        {
            return ruleBuilder
                .NotEmpty()
                .WithMessage("{PropertyName} is not empty");
        }
    }

    extension<T>(IRuleBuilder<T, Guid?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, Guid?> OptionalId()
        {
            return ruleBuilder
                .Must(x => !x.HasValue || x.Value != Guid.Empty)
                .WithMessage("{PropertyName} is not empty");
        }
    }

    extension<T>(IRuleBuilder<T, DateTimeOffset> ruleBuilder)
    {
        public IRuleBuilderOptions<T, DateTimeOffset> NotInPast()
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(DateTimeOffset.UtcNow)
                .WithMessage("{PropertyName} cannot be in the past");
        }
    }

    extension<T>(IRuleBuilder<T, DateTimeOffset?> ruleBuilder)
    {
        public IRuleBuilderOptions<T, DateTimeOffset?> OptionalNotInPast()
        {
            return ruleBuilder
                .Must(x => !x.HasValue || x.Value >= DateTimeOffset.UtcNow)
                .WithMessage("{PropertyName} cannot be in the past");
        }
    }

    extension<T, TProperty>(IRuleBuilder<T, TProperty> ruleBuilder)
        where TProperty : notnull
    {
        public IRuleBuilderOptions<T, TProperty> EqualToExpected(TProperty expected)
        {
            return ruleBuilder.Equal(expected).WithMessage($"{{PropertyName}} must be {expected}");
        }
    }

    extension<T, TProperty>(IRuleBuilder<T, TProperty?> ruleBuilder)
        where TProperty : struct, Enum
    {
        public IRuleBuilderOptions<T, TProperty?> OptionalIsInEnum()
        {
            return ruleBuilder
                .Must(x => !x.HasValue || Enum.IsDefined(typeof(TProperty), x.Value))
                .WithMessage("{PropertyName} is not in enum");
        }
    }

    extension<T>(IRuleBuilder<T, int> ruleBuilder)
    {
        public IRuleBuilderOptions<T, int> NonNegative()
        {
            return ruleBuilder
                .GreaterThanOrEqualTo(0)
                .WithMessage("{PropertyName} must be non-negative");
        }
    }

    extension<T>(IRuleBuilder<T, DateOnly> ruleBuilder)
    {
        public IRuleBuilderOptions<T, DateOnly> NotInFuture()
        {
            return ruleBuilder
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                .WithMessage("{PropertyName} cannot be in the future");
        }
    }
}
