namespace Mamey.Auth.DecentralizedIdentifiers.Core;

/// <summary>
/// Represents a W3C-compliant DID URL, including path, query, and fragment.
/// </summary>
public class DidUrl
{
    /// <summary>
    /// The base DID portion.
    /// </summary>
    public Did Did { get; }

    /// <summary>
    /// The path component (optional).
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// The query string component (optional).
    /// </summary>
    public string Query { get; }

    /// <summary>
    /// The fragment identifier (optional, used for referencing keys or services).
    /// </summary>
    public string Fragment { get; }

    /// <summary>
    /// Constructs a DID URL from a full URL string.
    /// </summary>
    public DidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentNullException(nameof(url));

        // Example: did:example:123456/path?service=abc#keys-1
        // Split at the first '#' (fragment)
        var fragmentSplit = url.Split('#');
        var main = fragmentSplit[0];
        Fragment = fragmentSplit.Length > 1 ? fragmentSplit[1] : null;

        // Split at the first '?'
        var querySplit = main.Split('?');
        var mainNoQuery = querySplit[0];
        Query = querySplit.Length > 1 ? querySplit[1] : null;

        // Split at the first '/' after 'did:...'
        var pathSplit = mainNoQuery.Split('/', 2);
        Did = new Did(pathSplit[0]);
        Path = pathSplit.Length > 1 ? "/" + pathSplit[1] : null;
    }

    /// <summary>
    /// Returns the full string representation of the DID URL.
    /// </summary>
    public override string ToString()
    {
        var s = Did.ToString();
        if (!string.IsNullOrEmpty(Path)) s += Path;
        if (!string.IsNullOrEmpty(Query)) s += "?" + Query;
        if (!string.IsNullOrEmpty(Fragment)) s += "#" + Fragment;
        return s;
    }
}