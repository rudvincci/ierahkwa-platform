using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Mamey.Auth.Identity.Managers;

/// <summary>
/// Enforces strong password rules and breach checking.
/// </summary>
public class StrongPasswordValidator
{
    private static readonly Regex ComplexityRegex =
        new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$");

    public Task ValidateAsync(string password, CancellationToken ct = default)
    {
        if (!ComplexityRegex.IsMatch(password))
            throw new ValidationException(
                "Password must be ≥8 chars, include upper, lower, digit, and symbol.");

        // TODO: integrate breach-detection API (e.g., HaveIBeenPwned)
        return Task.CompletedTask;
    }
}