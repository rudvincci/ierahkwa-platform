using System;
using System.Threading;
using System.Threading.Tasks;
using Mamey.Types;

namespace Mamey.Government.Modules.TravelIdentities.Core.Services;

/// <summary>
/// Service for generating document numbers with check digits using Luhn algorithm.
/// </summary>
public interface IDocumentNumberService
{
    /// <summary>
    /// Generates a new document number with Luhn check digit.
    /// </summary>
    Task<string> GenerateAsync(Guid tenantId, string prefix, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Validates a document number's check digit.
    /// </summary>
    bool Validate(string documentNumber);
}
