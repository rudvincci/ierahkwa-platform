using Mamey.Templates.Configuration;

namespace Mamey.Templates.Abstractions;

internal interface IPdfSigner
{
    Task<string> SignAsync(string pdfPath, SignOptions sign, CancellationToken ct);
}