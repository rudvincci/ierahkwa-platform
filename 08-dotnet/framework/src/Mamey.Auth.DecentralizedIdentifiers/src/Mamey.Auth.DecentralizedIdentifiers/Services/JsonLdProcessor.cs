using Mamey.Auth.DecentralizedIdentifiers.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonLD.Core;

namespace Mamey.Auth.DecentralizedIdentifiers.Services;

/// <summary>
/// Provides W3C JSON-LD processing (expand, compact, RDF, parse/serialize) using json-ld.net.
/// </summary>
public class JsonLdProcessor : IJsonLdProcessor
{
    /// <inheritdoc />
    public async Task<object> ExpandAsync(string json, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(json))
            throw new ArgumentNullException(nameof(json));

        var jToken = JToken.Parse(json);
        try
        {
            var expanded = await Task.Run(() =>
            {
                // Use null context for expansion, as per W3C spec.
                return JsonLdProcessorNet.Expand(jToken, new JsonLdOptions());
            }, cancellationToken);

            return expanded;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("JSON-LD Expansion failed.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<object> CompactAsync(string json, string context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
        if (string.IsNullOrWhiteSpace(context)) throw new ArgumentNullException(nameof(context));

        var jToken = JToken.Parse(json);
        var jContext = JToken.Parse(context);

        try
        {
            var compacted = await Task.Run(() =>
            {
                return JsonLdProcessorNet.Compact(jToken, jContext, new JsonLdOptions());
            }, cancellationToken);

            return compacted;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("JSON-LD Compaction failed.", ex);
        }
    }

    /// <inheritdoc />
    public async Task<string> ToRdfAsync(string json, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));

        var jToken = JToken.Parse(json);

        try
        {
            var nquads = await Task.Run(() =>
            {
                var opts = new JsonLdOptions();
                var rdfDataset = JsonLdProcessorNet.ToRDF(jToken, opts);
                return RDFDatasetUtils.ToNQuads((RDFDataset)rdfDataset);
            }, cancellationToken);

            return nquads;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("JSON-LD to RDF (N-Quads) conversion failed.", ex);
        }
    }

    /// <inheritdoc />
    public T Parse<T>(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));
        return JsonConvert.SerializeObject(obj, Formatting.None);
    }
}

/// <summary>
/// Internal bridge to disambiguate between our interface and json-ld.net.
/// </summary>
internal static class JsonLdProcessorNet
{
    public static object Expand(JToken input, JsonLdOptions opts)
        => JsonLD.Core.JsonLdProcessor.Expand(input, opts);

    public static object Compact(JToken input, JToken context, JsonLdOptions opts)
        => JsonLD.Core.JsonLdProcessor.Compact(input, context, opts);

    public static object ToRDF(JToken input, JsonLdOptions opts)
        => JsonLD.Core.JsonLdProcessor.ToRDF(input, opts);
}