namespace Mamey.Templates.Abstractions;

internal interface ITemplateEngine
{
    // Returns path to filled DOCX
    Task<string> FillAsync(string templateName, string workDir, string modelJson, CancellationToken ct);
}