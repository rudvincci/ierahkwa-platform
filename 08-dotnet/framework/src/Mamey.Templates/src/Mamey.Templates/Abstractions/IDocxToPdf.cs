namespace Mamey.Templates.Abstractions;

internal interface IDocxToPdf
{
    Task<string> ConvertAsync(string docxPath, string workDir, CancellationToken ct);
}