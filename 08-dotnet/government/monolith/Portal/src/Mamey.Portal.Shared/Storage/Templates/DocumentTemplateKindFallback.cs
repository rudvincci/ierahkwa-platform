namespace Mamey.Portal.Shared.Storage.Templates;

public static class DocumentTemplateKindFallback
{
    public static IReadOnlyList<string> GetCandidateKinds(string kind)
    {
        kind = (kind ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(kind))
        {
            return Array.Empty<string>();
        }

        // Common pattern: "BaseKind:Variant" falls back to "BaseKind".
        var idx = kind.IndexOf(':');
        if (idx > 0 && idx < kind.Length - 1)
        {
            var baseKind = kind[..idx].Trim();
            if (!string.IsNullOrWhiteSpace(baseKind))
            {
                return new[] { kind, baseKind };
            }
        }

        return new[] { kind };
    }
}




