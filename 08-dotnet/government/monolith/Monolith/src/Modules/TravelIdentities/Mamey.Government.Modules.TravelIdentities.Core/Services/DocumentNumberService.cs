using Mamey.Government.Modules.TravelIdentities.Core.Domain.Repositories;
using Mamey.Types;

namespace Mamey.Government.Modules.TravelIdentities.Core.Services;

/// <summary>
/// Generates document numbers with Luhn check digit for travel identity documents.
/// </summary>
internal sealed class DocumentNumberService : IDocumentNumberService
{
    private readonly ITravelIdentityRepository _repository;

    public DocumentNumberService(ITravelIdentityRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> GenerateAsync(Guid tenantId, string prefix, CancellationToken cancellationToken = default)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _repository.GetCountByYearAsync(new TenantId(tenantId), year, cancellationToken);
        var sequence = count + 1;
        
        // Format: PREFIX-YYYY-NNNNNN-C (where C is Luhn check digit)
        var baseNumber = $"{prefix}{year}{sequence:D6}";
        var checkDigit = CalculateLuhnCheckDigit(baseNumber);
        
        return $"{prefix}-{year}-{sequence:D6}-{checkDigit}";
    }

    public bool Validate(string documentNumber)
    {
        // Remove dashes and extract the number
        var cleanNumber = documentNumber.Replace("-", "");
        
        if (string.IsNullOrEmpty(cleanNumber) || cleanNumber.Length < 2)
            return false;
        
        // Extract check digit (last character)
        var providedCheck = cleanNumber[^1];
        var baseNumber = cleanNumber[..^1];
        
        // Calculate expected check digit
        var expectedCheck = CalculateLuhnCheckDigit(baseNumber);
        
        return providedCheck == expectedCheck;
    }

    private static char CalculateLuhnCheckDigit(string number)
    {
        // Convert letters to numbers (A=10, B=11, etc.)
        var digits = new System.Collections.Generic.List<int>();
        foreach (var c in number.ToUpperInvariant())
        {
            if (char.IsDigit(c))
            {
                digits.Add(c - '0');
            }
            else if (char.IsLetter(c))
            {
                // A=10, B=11, ... Z=35
                var value = c - 'A' + 10;
                digits.Add(value / 10);
                digits.Add(value % 10);
            }
        }
        
        // Luhn algorithm
        var sum = 0;
        var isSecond = true;
        
        for (var i = digits.Count - 1; i >= 0; i--)
        {
            var d = digits[i];
            
            if (isSecond)
            {
                d *= 2;
                if (d > 9)
                    d -= 9;
            }
            
            sum += d;
            isSecond = !isSecond;
        }
        
        var checkDigit = (10 - (sum % 10)) % 10;
        return (char)('0' + checkDigit);
    }
}
