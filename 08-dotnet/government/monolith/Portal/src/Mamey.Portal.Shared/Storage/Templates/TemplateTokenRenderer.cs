namespace Mamey.Portal.Shared.Storage.Templates;

public static class TemplateTokenRenderer
{
    public static string Apply(string templateHtml, IReadOnlyDictionary<string, string> tokens)
    {
        var html = templateHtml ?? string.Empty;
        if (tokens is null || tokens.Count == 0)
        {
            return html;
        }

        foreach (var kv in tokens)
        {
            html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value ?? string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        return html;
    }
}
