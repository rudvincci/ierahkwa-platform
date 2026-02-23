using System.Text.RegularExpressions;

namespace Mamey.Portal.Cms.Application.Services;

public sealed class CmsHtmlSanitizer : ICmsHtmlSanitizer
{
    public string SanitizeNewsBody(string html)
    {
        return SanitizeCore(html, allowHeadings: false);
    }

    public string SanitizePageBody(string html)
    {
        // Pages commonly need headings/sections.
        return SanitizeCore(html, allowHeadings: true);
    }

    private static string SanitizeCore(string? html, bool allowHeadings)
    {
        html ??= string.Empty;
        if (html.Length == 0) return string.Empty;

        // 1) Remove comments
        html = Regex.Replace(html, "<!--[\\s\\S]*?-->", string.Empty, RegexOptions.Compiled);

        // 2) Drop obviously dangerous blocks (script/style/iframe/object/embed/link/meta)
        html = StripBlock(html, "script");
        html = StripBlock(html, "style");
        html = StripBlock(html, "iframe");
        html = StripBlock(html, "object");
        html = StripBlock(html, "embed");
        html = StripSelfClosing(html, "link");
        html = StripSelfClosing(html, "meta");

        // 3) Drop all tags except allowlist (keep their inner text)
        // Allowed (news): p, br, strong, b, em, i, u, ul, ol, li, a
        // Allowed (pages): + h1,h2,h3,blockquote
        var allowed = allowHeadings
            ? "p\\b|br\\b|strong\\b|b\\b|em\\b|i\\b|u\\b|ul\\b|ol\\b|li\\b|a\\b|h1\\b|h2\\b|h3\\b|blockquote\\b"
            : "p\\b|br\\b|strong\\b|b\\b|em\\b|i\\b|u\\b|ul\\b|ol\\b|li\\b|a\\b";

        html = Regex.Replace(
            html,
            $"</?(?!{allowed})[a-zA-Z][^>]*>",
            string.Empty,
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // 4) Strip attributes from non-<a> allowed tags
        html = Regex.Replace(html, "<(p|strong|b|em|i|u|ul|ol|li|h1|h2|h3|blockquote)\\b[^>]*>", "<$1>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        html = Regex.Replace(html, "<br\\b[^>]*>", "<br>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // 5) Sanitize <a> tags: keep only safe href/title/target, enforce rel when target present
        html = Regex.Replace(html, "<a\\b[^>]*>", m => SanitizeAnchorOpenTag(m.Value), RegexOptions.Compiled | RegexOptions.IgnoreCase);

        // 6) Trim excessive whitespace
        return html.Trim();
    }

    private static string StripBlock(string html, string tag)
        => Regex.Replace(html, $"<{tag}\\b[\\s\\S]*?</{tag}>", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static string StripSelfClosing(string html, string tag)
        => Regex.Replace(html, $"<{tag}\\b[^>]*>", string.Empty, RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static string? GetAttr(string tag, string name)
    {
        var m = Regex.Match(tag, $"{name}\\s*=\\s*(\"([^\"]*)\"|'([^']*)'|([^\\s>]+))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        if (!m.Success) return null;
        return m.Groups[2].Success ? m.Groups[2].Value
            : m.Groups[3].Success ? m.Groups[3].Value
            : m.Groups[4].Success ? m.Groups[4].Value
            : null;
    }

    private static string SanitizeAnchorOpenTag(string original)
    {
        // Start with bare <a>
        var href = GetAttr(original, "href");
        var title = GetAttr(original, "title");
        var target = GetAttr(original, "target");

        href = SanitizeHref(href);

        var attrs = new List<string>();
        if (!string.IsNullOrWhiteSpace(href))
        {
            attrs.Add($"href=\"{HtmlAttrEncode(href)}\"");
        }
        if (!string.IsNullOrWhiteSpace(title))
        {
            attrs.Add($"title=\"{HtmlAttrEncode(title)}\"");
        }
        if (!string.IsNullOrWhiteSpace(target))
        {
            // Allow only _blank for MVP
            if (string.Equals(target, "_blank", StringComparison.OrdinalIgnoreCase))
            {
                attrs.Add("target=\"_blank\"");
                attrs.Add("rel=\"noopener noreferrer\"");
            }
        }

        return attrs.Count == 0 ? "<a>" : "<a " + string.Join(' ', attrs) + ">";
    }

    private static string? SanitizeHref(string? href)
    {
        if (string.IsNullOrWhiteSpace(href)) return null;
        href = href.Trim();

        // allow tenant-relative links like /somewhere
        if (href.StartsWith("/", StringComparison.Ordinal)) return href;

        if (!Uri.TryCreate(href, UriKind.Absolute, out var uri)) return null;
        if (string.Equals(uri.Scheme, "http", StringComparison.OrdinalIgnoreCase)) return href;
        if (string.Equals(uri.Scheme, "https", StringComparison.OrdinalIgnoreCase)) return href;
        if (string.Equals(uri.Scheme, "mailto", StringComparison.OrdinalIgnoreCase)) return href;
        return null;
    }

    private static string HtmlAttrEncode(string value)
        => value.Replace("&", "&amp;", StringComparison.Ordinal)
                .Replace("\"", "&quot;", StringComparison.Ordinal)
                .Replace("<", "&lt;", StringComparison.Ordinal)
                .Replace(">", "&gt;", StringComparison.Ordinal);
}
