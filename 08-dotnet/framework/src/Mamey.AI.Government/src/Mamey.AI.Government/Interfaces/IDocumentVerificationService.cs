using Mamey.AI.Government.Models;

namespace Mamey.AI.Government.Interfaces;

public interface IDocumentVerificationService
{
    Task<DocumentVerificationResult> VerifyDocumentAsync(Stream documentStream, string documentType, CancellationToken cancellationToken = default);
    Task<DocumentVerificationResult> AnalyzeDocumentAsync(Stream documentStream, CancellationToken cancellationToken = default);
}
