using Mamey.Portal.Citizenship.Application.Models;

namespace Mamey.Portal.Citizenship.Application.Services;

/// <summary>
/// Service for calculating document validity periods based on citizenship status
/// </summary>
public interface IDocumentValidityCalculator
{
    /// <summary>
    /// Calculates the expiration date for a document based on citizenship status and document type
    /// </summary>
    /// <param name="status">Current citizenship status</param>
    /// <param name="documentKind">Document kind (e.g., "Passport", "IdCard:DriversLicense", "VehicleTag")</param>
    /// <param name="issueDate">Date the document is being issued</param>
    /// <returns>Expiration date for the document</returns>
    DateTimeOffset CalculateExpirationDate(CitizenshipStatus status, string documentKind, DateTimeOffset issueDate);

    /// <summary>
    /// Gets the validity period in years for a document type based on citizenship status
    /// </summary>
    /// <param name="status">Current citizenship status</param>
    /// <param name="documentKind">Document kind</param>
    /// <returns>Validity period in years</returns>
    int GetValidityPeriodYears(CitizenshipStatus status, string documentKind);
}


