using Mamey.Identity.Decentralized.Abstractions;
using Newtonsoft.Json.Linq;
using JsonLD.Core;

namespace Mamey.Identity.Decentralized.Services;

/// <summary>
/// Provides URDNA2015 normalization (canonicalization) for JSON-LD and RDF datasets.
/// </summary>
public class CanonicalizationService : ICanonicalizationService
{
    private readonly IJsonLdProcessor _jsonLdProcessor;

    /// <summary>
    /// Creates a new CanonicalizationService using the given JSON-LD processor.
    /// </summary>
    public CanonicalizationService(IJsonLdProcessor jsonLdProcessor)
    {
        _jsonLdProcessor = jsonLdProcessor ?? throw new ArgumentNullException(nameof(jsonLdProcessor));
    }

    /// <inheritdoc />
    public async Task<string> CanonicalizeAsync(string nquads, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(nquads))
            throw new ArgumentNullException(nameof(nquads), "Input N-Quads string must not be empty.");

        // json-ld.net Normalize expects JSON-LD input, not N-Quads.
        // If input is already canonical N-Quads, just return (or implement N-Quads minification if desired).
        // If you want to parse N-Quads → JSON-LD, consider implementing as future work.
        return await Task.FromResult(nquads);
    }

    /// <inheritdoc />
    public async Task<string> CanonicalizeJsonLdAsync(string jsonLd, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(jsonLd))
            throw new ArgumentNullException(nameof(jsonLd), "JSON-LD input must not be empty.");

        // Parse as JToken for json-ld.net
        var jToken = JToken.Parse(jsonLd);

        // The Normalize API runs URDNA2015 and outputs N-Quads.
        var options = new JsonLdOptions();

        var canonicalized = await Task.Run(() =>
        {
            // Mode: "URDNA2015", output: "N-Quads"
            return JsonLD.Core.JsonLdProcessor.Normalize(jToken, options);
        }, cancellationToken);

        if (canonicalized == null)
            throw new InvalidOperationException("Canonicalization (URDNA2015) failed; result is null.");

        return canonicalized.ToString(); // N-Quads canonical string for digital signature
    }
}