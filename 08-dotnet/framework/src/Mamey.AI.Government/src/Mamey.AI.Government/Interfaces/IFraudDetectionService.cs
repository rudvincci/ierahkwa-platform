using Mamey.AI.Government.Models;

namespace Mamey.AI.Government.Interfaces;

public interface IFraudDetectionService
{
    Task<FraudScore> AnalyzeApplicationAsync(object applicationData, CancellationToken cancellationToken = default);
    Task<FraudScore> AnalyzeTransactionAsync(object transactionData, CancellationToken cancellationToken = default);
}
