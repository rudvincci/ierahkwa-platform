using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Calculates document validity periods based on citizenship status
/// </summary>
public sealed class DocumentValidityCalculator : IDocumentValidityCalculator
{
    public DateTimeOffset CalculateExpirationDate(CitizenshipStatus status, string documentKind, DateTimeOffset issueDate)
    {
        var years = GetValidityPeriodYears(status, documentKind);
        return issueDate.AddYears(years);
    }

    public int GetValidityPeriodYears(CitizenshipStatus status, string documentKind)
    {
        return status switch
        {
            CitizenshipStatus.Probationary => 2, // All documents valid for 2 years
            CitizenshipStatus.Resident => 3, // All documents valid for 3 years
            CitizenshipStatus.Citizen => GetCitizenValidityPeriod(documentKind), // Varies by document type
            _ => 2 // Default to 2 years for unknown status
        };
    }

    private static int GetCitizenValidityPeriod(string documentKind)
    {
        // Normalize document kind for comparison
        var kind = documentKind.ToLowerInvariant();

        // Passport: 10 years
        if (kind == "passport")
        {
            return 10;
        }

        // Driver's License: 7 years
        if (kind.Contains("driverslicense") || kind.Contains("driverlicense") || kind == "idcard:driverslicense")
        {
            return 7;
        }

        // Other documents: 1-5 years depending on type
        if (kind.StartsWith("idcard:"))
        {
            // ID card variants: 5 years
            return 5;
        }

        if (kind.StartsWith("vehicletag:"))
        {
            // Vehicle tags: 1 year
            return 1;
        }

        if (kind == "citizenshipcertificate")
        {
            // Citizenship certificate: 5 years (renewable)
            return 5;
        }

        // Default for other documents: 3 years
        return 3;
    }
}


