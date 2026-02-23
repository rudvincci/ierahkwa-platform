using System.Text.Json.Serialization;

namespace Mamey.Tracing.Jaeger.Tests.Helpers;

public class JaegerServicesResponse
{
    [JsonPropertyName("data")]
    public List<string> Data { get; set; } = new();
}

public class JaegerTracesResponse
{
    [JsonPropertyName("data")]
    public List<JaegerTrace> Data { get; set; } = new();
}

public class JaegerTrace
{
    [JsonPropertyName("traceID")]
    public string TraceID { get; set; } = string.Empty;

    [JsonPropertyName("spans")]
    public List<JaegerSpan> Spans { get; set; } = new();
}

public class JaegerSpan
{
    [JsonPropertyName("operationName")]
    public string OperationName { get; set; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<JaegerTag> Tags { get; set; } = new();

    [JsonPropertyName("logs")]
    public List<JaegerLog> Logs { get; set; } = new();

    [JsonPropertyName("duration")]
    public long Duration { get; set; }

    [JsonPropertyName("startTime")]
    public long StartTime { get; set; }

    [JsonPropertyName("spanID")]
    public string SpanID { get; set; } = string.Empty;

    [JsonPropertyName("parentSpanID")]
    public string? ParentSpanID { get; set; }
}

public class JaegerTag
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public object Value { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class JaegerLog
{
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("fields")]
    public List<JaegerTag> Fields { get; set; } = new();
}
