namespace IERAHKWA.Platform.Models;

public class ServiceInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Status { get; set; } = "checking";
    public string Category { get; set; } = "general";
    public string Type { get; set; } = "service";
}

public class DepartmentInfo
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Value { get; set; }
    public string Status { get; set; } = "ACTIVE";
    public string? Link { get; set; }
    public string? PlatformKey { get; set; }
    public string Category { get; set; } = "general";
    public string Type { get; set; } = "department";
}

public class TokenInfo
{
    public string Id { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}

public class PlatformConfig
{
    public Dictionary<string, string>? Services { get; set; }
    public List<DepartmentInfo>? Departments { get; set; }
    public List<TokenInfo>? Tokens { get; set; }
}

public class PlatformOverview
{
    public string Name { get; set; } = "IERAHKWA Platform";
    public string Version { get; set; } = "2.0.0";
    public string Status { get; set; } = "operational";
    public int Modules { get; set; }
    public int ModulesOnline { get; set; }
    public int Tokens { get; set; }
    public int Departments { get; set; }
    public int Services { get; set; }
    public string Uptime { get; set; } = "0%";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Error { get; set; }
}
