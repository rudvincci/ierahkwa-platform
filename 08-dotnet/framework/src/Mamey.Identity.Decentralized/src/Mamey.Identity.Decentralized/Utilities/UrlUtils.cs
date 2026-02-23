namespace Mamey.Identity.Decentralized.Utilities;

/// <summary>
/// Utilities for working with DID URLs and fragments.
/// </summary>
public static class UrlUtils
{
    /// <summary>
    /// Returns the fragment part (without '#'), or null if none.
    /// </summary>
    public static string GetFragment(string didUrl)
    {
        if (didUrl == null) return null;
        var idx = didUrl.IndexOf('#');
        return idx >= 0 && idx < didUrl.Length - 1 ? didUrl[(idx + 1)..] : null;
    }

    /// <summary>
    /// Removes the fragment from a DID URL, returning the base DID or URL.
    /// </summary>
    public static string WithoutFragment(string didUrl)
    {
        if (didUrl == null) return null;
        var idx = didUrl.IndexOf('#');
        return idx >= 0 ? didUrl[..idx] : didUrl;
    }
}