using Mamey.Templates.Abstractions;
using Mamey.Word;

namespace Mamey.Templates;

public sealed class MameyWordTemplateEngine : ITemplateEngine
{
    private readonly ITemplateRepository _templates;
    private readonly IWordService _word;

    public MameyWordTemplateEngine(ITemplateRepository templates, IWordService word)
    { _templates = templates; _word = word; }

    public async Task<string> FillAsync(string templateName, string workDir, string modelJson, CancellationToken ct)
    {
        // templateName format: "Form-27@latest" or "Form-27@3"
        var (id, ver) = Parse(templateName);
        if (ver == 0) ver = await _templates.GetLatestVersionAsync(id, ct);
        var blob = await _templates.GetAsync(id, ver, ct);

        var path = Path.Combine(workDir, "template.docx");
        await File.WriteAllBytesAsync(path, blob.Data, ct);

        // Then reuse your WordService map-based overload to produce filled.docx
        // (as shown earlier). No library rewrite needed.
        // ...
        
        
        
        return Path.Combine(workDir, "filled.docx");
    }

    private static (string id, int version) Parse(string s)
    {
        var parts = s.Split('@');
        return parts.Length == 2 && int.TryParse(parts[1], out var v) ? (parts[0], v) : (s, 0);
    }
}