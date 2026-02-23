namespace Mamey.FWID.Identities.Application.AI.Models;

/// <summary>
/// Behavioral profile for an identity.
/// </summary>
public class BehavioralProfile
{
    public Guid ProfileId { get; set; } = Guid.NewGuid();
    public Guid IdentityId { get; set; }
    public LoginPatternProfile LoginPattern { get; set; } = new();
    public SessionProfile SessionProfile { get; set; } = new();
    public KeystrokeProfile? KeystrokeProfile { get; set; }
    public DeviceProfile DeviceProfile { get; set; } = new();
    public double BaselineConfidence { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int DataPointCount { get; set; }
    public bool IsEstablished => DataPointCount >= 10;
}

public class LoginPatternProfile
{
    public List<int> PreferredHoursOfDay { get; set; } = new();
    public List<DayOfWeek> PreferredDaysOfWeek { get; set; } = new();
    public List<string> CommonLocations { get; set; } = new();
    public List<string> CommonIPRanges { get; set; } = new();
    public double AverageLoginFrequencyPerDay { get; set; }
    public double AverageSessionDurationMinutes { get; set; }
}

public class SessionProfile
{
    public double AveragePageViewsPerSession { get; set; }
    public double AverageSessionDuration { get; set; }
    public List<string> CommonNavigationPaths { get; set; } = new();
    public List<string> FrequentlyAccessedFeatures { get; set; } = new();
    public double TypicalIdleTimeSeconds { get; set; }
}

public class KeystrokeProfile
{
    public double AverageTypingSpeed { get; set; }
    public double TypingSpeedStdDev { get; set; }
    public double AverageKeyHoldTime { get; set; }
    public double AverageKeyInterval { get; set; }
    public Dictionary<string, double> DigramLatencies { get; set; } = new();
    public double ProfileConfidence { get; set; }
}

public class DeviceProfile
{
    public List<string> KnownDevices { get; set; } = new();
    public List<string> KnownBrowsers { get; set; } = new();
    public List<string> KnownOperatingSystems { get; set; } = new();
    public string? PrimaryDeviceId { get; set; }
}
