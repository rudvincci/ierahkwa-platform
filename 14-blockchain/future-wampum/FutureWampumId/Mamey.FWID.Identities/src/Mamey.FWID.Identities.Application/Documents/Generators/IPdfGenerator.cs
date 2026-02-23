namespace Mamey.FWID.Identities.Application.Documents.Generators;

/// <summary>
/// Service for converting HTML to PDF.
/// </summary>
internal interface IPdfGenerator
{
    /// <summary>
    /// Converts HTML content to PDF bytes.
    /// </summary>
    Task<byte[]> GeneratePdfFromHtmlAsync(
        string html,
        CancellationToken cancellationToken = default);
}
