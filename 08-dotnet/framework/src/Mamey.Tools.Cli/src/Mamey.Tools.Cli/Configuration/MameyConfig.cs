using System.Text.Json.Serialization;

namespace Mamey.Tools.Cli.Configuration;

/// <summary>
/// MameyNode CLI configuration
/// </summary>
public class MameyConfig
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";
    
    [JsonPropertyName("network")]
    public NetworkConfig? Network { get; set; }
    
    [JsonPropertyName("credentials")]
    public CredentialConfig? Credentials { get; set; }
    
    [JsonPropertyName("workflows")]
    public WorkflowConfig? Workflows { get; set; }
    
    [JsonPropertyName("testing")]
    public TestingConfig? Testing { get; set; }
    
    [JsonPropertyName("plugins")]
    public List<string>? Plugins { get; set; }
    
    [JsonPropertyName("logging")]
    public LoggingConfig? Logging { get; set; }
}

public class NetworkConfig
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "local";
    
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = "http://localhost:50051";
    
    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 30;
}

public class CredentialConfig
{
    [JsonPropertyName("defaultInstitutionId")]
    public string? DefaultInstitutionId { get; set; }
    
    [JsonPropertyName("credentialSource")]
    public string CredentialSource { get; set; } = "env"; // env, config, keychain, hardware
    
    [JsonPropertyName("keychainName")]
    public string? KeychainName { get; set; }
    
    [JsonPropertyName("credentialFile")]
    public string? CredentialFile { get; set; }
}

public class WorkflowConfig
{
    [JsonPropertyName("directory")]
    public string Directory { get; set; } = "./workflows";
    
    [JsonPropertyName("autoValidate")]
    public bool AutoValidate { get; set; } = true;
}

public class TestingConfig
{
    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 60;
    
    [JsonPropertyName("parallel")]
    public bool Parallel { get; set; } = true;
    
    [JsonPropertyName("coverage")]
    public bool Coverage { get; set; } = false;
}

public class LoggingConfig
{
    [JsonPropertyName("level")]
    public string Level { get; set; } = "info";
    
    [JsonPropertyName("format")]
    public string Format { get; set; } = "text"; // json, text
}
