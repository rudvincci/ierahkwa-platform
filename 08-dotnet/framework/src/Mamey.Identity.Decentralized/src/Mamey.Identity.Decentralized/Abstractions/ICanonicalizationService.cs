namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Defines a contract for canonicalizing RDF datasets or JSON-LD documents according to
/// W3C URDNA2015 (Universal RDF Dataset Normalization Algorithm 2015).
/// </summary>
public interface ICanonicalizationService
{
    /// <summary>
    /// Canonicalizes the input RDF dataset (in N-Quads form) using URDNA2015.
    /// </summary>
    /// <param name="nquads">The RDF dataset as N-Quads.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The canonicalized N-Quads string.</returns>
    Task<string> CanonicalizeAsync(string nquads, CancellationToken cancellationToken = default);

    /// <summary>
    /// Canonicalizes a JSON-LD document by expanding and converting it to a normalized RDF dataset.
    /// </summary>
    /// <param name="jsonLd">The JSON-LD input document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The canonicalized N-Quads string.</returns>
    Task<string> CanonicalizeJsonLdAsync(string jsonLd, CancellationToken cancellationToken = default);
}