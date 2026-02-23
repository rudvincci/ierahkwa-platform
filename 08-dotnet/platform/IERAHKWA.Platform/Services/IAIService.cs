namespace IERAHKWA.Platform.Services;

public interface IAIService
{
    Task<string> ChatAsync(string message, string? context = null);
    Task<string> GenerateCodeAsync(string prompt, string language = "javascript");
    Task<string> AnalyzeCodeAsync(string code);
}
