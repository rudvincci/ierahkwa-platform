namespace Mamey.AI.Government.Models;

public class IntentResult
{
    public string Intent { get; set; } = "Unknown";
    public double Confidence { get; set; }
    public Dictionary<string, string> Entities { get; set; } = new();
}

public class ChatRequest
{
    public string SessionId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Language { get; set; } = "en";
    public List<string> History { get; set; } = new();
}

public class ChatResponse
{
    public string Message { get; set; } = string.Empty;
    public List<string> Suggestions { get; set; } = new();
    public bool ShouldHandoff { get; set; }
    public double Confidence { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}
