namespace Mamey.Identity.Decentralized.Abstractions;

/// <summary>
/// Defines a contract for processing JSON-LD documents, including expansion, compaction, framing,
/// and RDF dataset conversion, as required by W3C DID and Verifiable Credentials specifications.
/// </summary>
public interface IJsonLdProcessor
{
    /// <summary>
    /// Expands a JSON-LD input to its full form using provided contexts.
    /// </summary>
    /// <param name="json">The compacted JSON-LD document.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Expanded JSON-LD as an object (could be a dictionary or array).</returns>
    Task<object> ExpandAsync(string json, CancellationToken cancellationToken = default);

    /// <summary>
    /// Compacts a JSON-LD input using the provided context.
    /// </summary>
    /// <param name="json">The expanded JSON-LD document.</param>
    /// <param name="context">The context to use for compaction.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Compacted JSON-LD as an object.</returns>
    Task<object> CompactAsync(string json, string context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Converts a JSON-LD document to N-Quads RDF dataset.
    /// </summary>
    /// <param name="json">The JSON-LD input.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>N-Quads string representing the RDF dataset.</returns>
    Task<string> ToRdfAsync(string json, CancellationToken cancellationToken = default);

    /// <summary>
    /// Parses a JSON-LD document from string to a strongly-typed object.
    /// </summary>
    /// <typeparam name="T">The type to parse to.</typeparam>
    /// <param name="json">The JSON-LD string.</param>
    /// <returns>The parsed object.</returns>
    T Parse<T>(string json);

    /// <summary>
    /// Serializes a strongly-typed object to JSON-LD string.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>The JSON-LD string.</returns>
    string Serialize(object obj);
}