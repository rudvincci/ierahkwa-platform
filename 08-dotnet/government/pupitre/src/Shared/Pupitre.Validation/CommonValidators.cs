using FluentValidation;

namespace Pupitre.Validation;

/// <summary>
/// Common validation rules for Pupitre.
/// </summary>
public static class CommonValidators
{
    /// <summary>
    /// Validates that a GUID is not empty.
    /// </summary>
    public static IRuleBuilderOptions<T, Guid> NotEmptyGuid<T>(this IRuleBuilder<T, Guid> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("'{PropertyName}' must not be empty.");
    }

    /// <summary>
    /// Validates email format.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .WithMessage("'{PropertyName}' must be a valid email address.");
    }

    /// <summary>
    /// Validates a name (first name, last name, etc.).
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(1)
            .MaximumLength(100)
            .Matches(@"^[\p{L}\s'-]+$")
            .WithMessage("'{PropertyName}' must contain only letters, spaces, hyphens, and apostrophes.");
    }

    /// <summary>
    /// Validates grade level (typically 1-12 for K-12).
    /// </summary>
    public static IRuleBuilderOptions<T, int> ValidGradeLevel<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        int minGrade = 1,
        int maxGrade = 12)
    {
        return ruleBuilder
            .InclusiveBetween(minGrade, maxGrade)
            .WithMessage($"'{{PropertyName}}' must be between {minGrade} and {maxGrade}.");
    }

    /// <summary>
    /// Validates a score percentage (0-100).
    /// </summary>
    public static IRuleBuilderOptions<T, decimal> ValidScorePercentage<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder
            .InclusiveBetween(0, 100)
            .WithMessage("'{PropertyName}' must be between 0 and 100.");
    }

    /// <summary>
    /// Validates pagination page number.
    /// </summary>
    public static IRuleBuilderOptions<T, int> ValidPage<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(1)
            .WithMessage("'{PropertyName}' must be at least 1.");
    }

    /// <summary>
    /// Validates pagination page size.
    /// </summary>
    public static IRuleBuilderOptions<T, int> ValidPageSize<T>(
        this IRuleBuilder<T, int> ruleBuilder,
        int maxPageSize = 100)
    {
        return ruleBuilder
            .InclusiveBetween(1, maxPageSize)
            .WithMessage($"'{{PropertyName}}' must be between 1 and {maxPageSize}.");
    }

    /// <summary>
    /// Validates a URL.
    /// </summary>
    public static IRuleBuilderOptions<T, string> ValidUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .When(x => !string.IsNullOrEmpty(x as string))
            .WithMessage("'{PropertyName}' must be a valid URL.");
    }

    /// <summary>
    /// Validates date is not in the future.
    /// </summary>
    public static IRuleBuilderOptions<T, DateTime> NotInFuture<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("'{PropertyName}' cannot be in the future.");
    }

    /// <summary>
    /// Validates date is not in the past.
    /// </summary>
    public static IRuleBuilderOptions<T, DateTime> NotInPast<T>(this IRuleBuilder<T, DateTime> ruleBuilder)
    {
        return ruleBuilder
            .GreaterThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("'{PropertyName}' cannot be in the past.");
    }
}
