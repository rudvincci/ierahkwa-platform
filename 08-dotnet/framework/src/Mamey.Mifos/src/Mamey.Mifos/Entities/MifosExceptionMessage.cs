using System.Net;
using System.Text.Json.Serialization;

namespace Mamey.Mifos.Entities
{
    public class MifosExceptionMessage
    {
        public string? DeveloperMessage { get; set; }
        public string? DeveloperDocLink { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string? DefaultUserMessage { get; set; }
        public string? UserMessageGlobalisationCode { get; set; }
        public string? ParameterName { get; set; }
        public string? Value { get; set; } = null;

        [JsonPropertyName("args")]
        public IEnumerable<string> Arguments { get; set; } = Enumerable.Empty<string>();

    }
}

