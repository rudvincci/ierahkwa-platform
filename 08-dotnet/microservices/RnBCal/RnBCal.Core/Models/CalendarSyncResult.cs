namespace RnBCal.Core.Models;

/// <summary>
/// Result of calendar sync operation
/// </summary>
public class CalendarSyncResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? IcsFileContent { get; set; }
    public string? IcsFileName { get; set; }
    public Dictionary<CalendarProvider, CalendarLink> CalendarLinks { get; set; } = new();
    public List<string> Errors { get; set; } = new();
    public DateTime SyncedAt { get; set; } = DateTime.UtcNow;
}

public class CalendarLink
{
    public string Provider { get; set; } = string.Empty;
    public string DirectLink { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string IconClass { get; set; } = string.Empty;
}

public enum CalendarProvider
{
    Google,
    Apple,
    Yahoo,
    Outlook,
    Office365,
    AOL
}
