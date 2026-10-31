using FluentValidation;
using System.Linq.Expressions;

namespace Main.Api.Features.Validation;

public static class ValidationRules
{
	/// <summary>
	/// Добавляет правила валидации для обязательной строки.
	/// Значение не может быть null, пустой строкой или состоять только из пробелов,
	/// а также не должно превышать указанную максимальную длину.
	/// </summary>
	/// <typeparam name="T">Тип валидируемого объекта.</typeparam>
	/// <param name="validator">Валидатор, к которому добавляются правила.</param>
	/// <param name="propertyExpression">Выражение, выбирающее строковое свойство объекта.</param>
	/// <param name="fieldName">Название поля, используемое в сообщениях об ошибках.</param>
	/// <param name="maxLength">Максимальная допустимая длина строки.</param>
	/// <returns>Объект для цепочной настройки дополнительных правил валидации.</returns>
	public static IRuleBuilderOptions<T, string?> AddRequiredStringRules<T>(
		this AbstractValidator<T> validator,
		Expression<Func<T, string?>> propertyExpression,
		string fieldName,
		int maxLength)
	{
		return validator
			.RuleFor(propertyExpression)
			.NotEmpty()
			.WithMessage($"{fieldName} is required")
			.MaximumLength(maxLength)
			.WithMessage($"{fieldName} must be at most {maxLength} characters");
	}

	/// <summary>
	/// Добавляет правила валидации для необязательной строки.
	/// Допускает null и пустую строку. Если значение задано, проверяет,
	/// что оно не превышает указанную максимальную длину.
	/// </summary>
	/// <typeparam name="T">Тип валидируемого объекта.</typeparam>
	/// <param name="validator">Валидатор, к которому добавляются правила.</param>
	/// <param name="propertyExpression">Выражение, выбирающее строковое свойство объекта.</param>
	/// <param name="fieldName">Название поля, используемое в сообщениях об ошибках.</param>
	/// <param name="maxLength">Максимальная допустимая длина строки.</param>
	/// <returns>Объект для цепочной настройки дополнительных правил валидации.</returns>
	public static IRuleBuilderOptions<T, string?> AddOptionalStringRules<T>(
		this AbstractValidator<T> validator,
		Expression<Func<T, string?>> propertyExpression,
		string fieldName,
		int maxLength)
	{
		return validator
			.RuleFor(propertyExpression)
			.MaximumLength(maxLength)
			.WithMessage($"{fieldName} must be at most {maxLength} characters");
	}

	/// <summary>
	/// Добавляет правила валидации для необязательной непустой строки.
	/// Допускает null, но если значение задано, оно не может быть пустым или состоять только из пробелов,
	/// а также не должно превышать указанную максимальную длину.
	/// </summary>
	/// <typeparam name="T">Тип валидируемого объекта.</typeparam>
	/// <param name="validator">Валидатор, к которому добавляются правила.</param>
	/// <param name="propertyExpression">Выражение, выбирающее строковое свойство объекта.</param>
	/// <param name="fieldName">Название поля, используемое в сообщениях об ошибках.</param>
	/// <param name="maxLength">Максимальная допустимая длина строки.</param>
	/// <returns>Объект для цепочной настройки дополнительных правил валидации.</returns>
	public static IRuleBuilderOptions<T, string?> AddOptionalNonEmptyStringRules<T>(
		this AbstractValidator<T> validator,
		Expression<Func<T, string?>> propertyExpression,
		string fieldName,
		int maxLength)
	{
		var compiled = propertyExpression.Compile();

		return validator
			.RuleFor(propertyExpression)
			.NotEmpty()
			.WithMessage($"{fieldName} cannot be empty")
			.When(x => compiled(x) is not null, ApplyConditionTo.CurrentValidator)
			.MaximumLength(maxLength)
			.WithMessage($"{fieldName} must be at most {maxLength} characters");
	}

	/// <summary>
	/// Добавляет правила валидации для обязательного неотрицательного десятичного числа.
	/// Значение должно быть больше или равно нулю.
	/// </summary>
	/// <typeparam name="T">Тип валидируемого объекта.</typeparam>
	/// <param name="validator">Валидатор, к которому добавляются правила.</param>
	/// <param name="propertyExpression">Выражение, выбирающее свойство типа decimal объекта.</param>
	/// <param name="fieldName">Название поля, используемое в сообщениях об ошибках.</param>
	/// <returns>Объект для цепочной настройки дополнительных правил валидации.</returns>
	public static IRuleBuilderOptions<T, decimal> AddRequiredDecimalRules<T>(
		this AbstractValidator<T> validator,
		Expression<Func<T, decimal>> propertyExpression,
		string fieldName)
	{
		return validator
			.RuleFor(propertyExpression)
			.GreaterThanOrEqualTo(0)
			.WithMessage($"{fieldName} must be non-negative");
	}

	/// <summary>
	/// Добавляет правила валидации для обязательного неотрицательного десятичного числа (decimal?).
	/// Перегрузка для nullable decimal (decimal?).
	/// Значение не может быть null и должно быть больше или равно нулю.
	/// </summary>
	/// <typeparam name="T">Тип валидируемого объекта.</typeparam>
	/// <param name="validator">Валидатор, к которому добавляются правила.</param>
	/// <param name="propertyExpression">Выражение, выбирающее свойство типа decimal? объекта.</param>
	/// <param name="fieldName">Название поля, используемое в сообщениях об ошибках.</param>
	/// <returns>Объект для цепочной настройки дополнительных правил валидации.</returns>
	public static IRuleBuilderOptions<T, decimal?> AddRequiredDecimalRules<T>(
		this AbstractValidator<T> validator,
		Expression<Func<T, decimal?>> propertyExpression,
		string fieldName)
	{
		return validator
			.RuleFor(propertyExpression)
			.NotNull()
			.WithMessage($"{fieldName} is required")
			.GreaterThanOrEqualTo(0)
			.WithMessage($"{fieldName} must be non-negative");
	}

	/// <summary>
	/// Добавляет правила валидации для необязательного неотрицательного десятичного числа (decimal?).
	/// Допускает null. Если значение задано, оно должно быть больше или равно нулю.
	/// </summary>
	/// <typeparam name="T">Тип валидируемого объекта.</typeparam>
	/// <param name="validator">Валидатор, к которому добавляются правила.</param>
	/// <param name="propertyExpression">Выражение, выбирающее свойство типа decimal? объекта.</param>
	/// <param name="fieldName">Название поля, используемое в сообщениях об ошибках.</param>
	/// <returns>Объект для цепочной настройки дополнительных правил валидации.</returns>
	public static IRuleBuilderOptions<T, decimal?> AddOptionalDecimalRules<T>(
		this AbstractValidator<T> validator,
		Expression<Func<T, decimal?>> propertyExpression,
		string fieldName)
	{
		return validator
			.RuleFor(propertyExpression)
			.GreaterThanOrEqualTo(0)
			.WithMessage($"{fieldName} must be non-negative");
	}

	/// <summary>
	/// Добавляет правило валидации для обязательного идентификатора.
	/// Значение не может быть пустым идентификатором (Guid.Empty).
	/// </summary>
	/// <typeparam name="T">Тип валидируемого объекта.</typeparam>
	/// <param name="validator">Валидатор, к которому добавляются правила.</param>
	/// <param name="propertyExpression">Выражение, выбирающее свойство типа Guid объекта.</param>
	/// <param name="fieldName">Название поля, используемое в сообщениях об ошибках.</param>
	/// <returns>Объект для цепочной настройки дополнительных правил валидации.</returns>
	public static IRuleBuilderOptions<T, Guid> AddRequiredIdRules<T>(
		this AbstractValidator<T> validator,
		Expression<Func<T, Guid>> propertyExpression,
		string fieldName)
	{
		return validator
			.RuleFor(propertyExpression)
			.NotEmpty()
			.WithMessage($"{fieldName} is required");
	}
}
