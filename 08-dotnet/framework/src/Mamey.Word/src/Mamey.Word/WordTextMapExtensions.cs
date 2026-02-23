using Spire.Doc;

namespace Mamey.Word;

/// <summary>
/// Runtime, map-driven replacement: replaces {{Key}} with map[Key].ToString().
/// Coexists with ReplaceText&lt;T&gt;.
/// </summary>
public static class WordTextMapExtensions
{
    public static Document ReplaceText(this Document document, IDictionary<string, object> map)
    {
        if (document is null) throw new ArgumentNullException(nameof(document));
        if (map is null) throw new ArgumentNullException(nameof(map));

        foreach (var (key, value) in map)
        {
            var placeholder = $"{{{{{key}}}}}";
            var text = value?.ToString() ?? string.Empty;
            document.Replace(placeholder, text, true, true);
        }
        return document;
    }
}