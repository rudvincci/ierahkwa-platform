using Newtonsoft.Json.Linq;
using JsonLD.Core;

namespace Mamey.Identity.Decentralized.Serialization
{
    /// <summary>
    /// Static utility class wrapping json-ld.net operations for use in the FutureWampumId.Did library.
    /// </summary>
    public static class JsonLdProcessorCore
    {
        /// <summary>
        /// Expands a compacted JSON-LD JObject to its full expanded form.
        /// </summary>
        /// <param name="input">A JObject representing the compacted JSON-LD.</param>
        /// <returns>The expanded JSON-LD (as a JToken: usually JArray or JObject).</returns>
        public static JToken Expand(JObject input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var options = new JsonLdOptions();
            return JToken.FromObject(JsonLD.Core.JsonLdProcessor.Expand(input, options));
        }

        /// <summary>
        /// Compacts an expanded JSON-LD JObject or JArray with a given context.
        /// </summary>
        /// <param name="input">The expanded JSON-LD (JObject or JArray).</param>
        /// <param name="context">The context JObject for compaction.</param>
        /// <returns>The compacted JSON-LD (as a JToken).</returns>
        public static JToken Compact(JToken input, JObject context)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (context == null) throw new ArgumentNullException(nameof(context));
            var options = new JsonLdOptions();
            return JToken.FromObject(JsonLD.Core.JsonLdProcessor.Compact(input, context, options));
        }

        /// <summary>
        /// Converts a JSON-LD document (JObject or JArray) to an RDF dataset (NQuads).
        /// </summary>
        /// <param name="input">The JSON-LD as JToken (usually JObject).</param>
        /// <param name="options">Optional JsonLdOptions.</param>
        /// <returns>RDF dataset as an object (typically Dictionary form).</returns>
        public static object ToRDF(JToken input, JsonLdOptions options = null)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            return JsonLD.Core.JsonLdProcessor.ToRDF(input, options ?? new JsonLdOptions());
        }

        /// <summary>
        /// Converts a JSON string to a JToken for downstream operations.
        /// </summary>
        /// <param name="json">JSON string input.</param>
        /// <returns>Parsed JToken.</returns>
        public static JToken Parse(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
            return JToken.Parse(json);
        }

        /// <summary>
        /// Converts a JToken to a pretty-printed JSON string.
        /// </summary>
        /// <param name="token">The JToken input.</param>
        /// <returns>JSON string.</returns>
        public static string ToJsonString(JToken token)
        {
            if (token == null) throw new ArgumentNullException(nameof(token));
            return token.ToString(Newtonsoft.Json.Formatting.Indented);
        }
    }
}
