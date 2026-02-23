namespace Mamey.Templates.Abstractions;

internal interface IPdfHardener
{
    Task<string> HardenAsync(string pdfPath, string mode, int dpi, CancellationToken ct);
}