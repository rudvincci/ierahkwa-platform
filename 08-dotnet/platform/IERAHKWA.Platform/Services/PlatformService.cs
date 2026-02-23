using System.Net.Http;
using System.Text.Json;
using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Services;

public class PlatformService : IPlatformService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PlatformService> _logger;
    private PlatformConfig? _config;
    private readonly Dictionary<string, ServiceInfo> _services = new();

    public PlatformService(IHttpClientFactory httpClientFactory, ILogger<PlatformService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        LoadConfig();
    }

    private class ConfigJson
    {
        public Dictionary<string, string>? services { get; set; }
        public List<ConfigDepartment>? departments { get; set; }
        public List<TokenInfo>? tokens { get; set; }
    }
    
    private class ConfigDepartment
    {
        public string? id { get; set; }
        public string? icon { get; set; }
        public string? title { get; set; }
        public string? value { get; set; }
        public string? status { get; set; }
        public string? link { get; set; }
        public string? platformKey { get; set; }
        public bool visible { get; set; }
    }

    private void LoadConfig()
    {
        try
        {
            var configPath = "/Users/ruddie/Sovereign Akwesasne Government - Office of the Prime Minister - Photos/soberanos natives/platform/config.json";
            _logger.LogInformation($"Loading config from: {configPath}");
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                var configJson = JsonSerializer.Deserialize<ConfigJson>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                if (configJson != null)
                {
                    // Convert ConfigDepartment to DepartmentInfo
                    var depts = configJson.departments?.Select(d => new DepartmentInfo
                    {
                        Id = d.id ?? "",
                        Name = d.title ?? "",
                        Icon = d.icon,
                        Value = d.value,
                        Status = d.status ?? "ACTIVE",
                        Link = d.link,
                        PlatformKey = d.platformKey,
                        Category = "platform",
                        Type = "department"
                    }).ToList();
                    
                    _config = new PlatformConfig
                    {
                        Services = configJson.services,
                        Departments = depts,
                        Tokens = configJson.tokens
                    };
                    
                    _logger.LogInformation($"Loaded {depts?.Count ?? 0} departments and {configJson.services?.Count ?? 0} services");
                }

                // Load services from config
                if (_config?.Services != null)
                {
                    foreach (var (key, url) in _config.Services)
                    {
                        var port = ExtractPortFromUrl(url);
                        _services[key] = new ServiceInfo
                        {
                            Id = key,
                            Name = FormatServiceName(key),
                            Url = url,
                            Port = port,
                            Status = "checking",
                            Category = GetCategoryFromKey(key),
                            Type = "service"
                        };
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading config.json");
        }
    }

    private int ExtractPortFromUrl(string url)
    {
        var match = System.Text.RegularExpressions.Regex.Match(url, @":(\d+)");
        return match.Success ? int.Parse(match.Groups[1].Value) : 3000;
    }

    private string FormatServiceName(string key)
    {
        return System.Text.RegularExpressions.Regex.Replace(key, @"([A-Z])", " $1")
            .Trim()
            .Replace(key[0].ToString(), key[0].ToString().ToUpper());
    }

    private string GetCategoryFromKey(string key)
    {
        var categories = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "gov", "government" }, { "admin", "administration" }, { "bank", "banking" },
            { "bdet", "banking" }, { "blockchain", "blockchain" }, { "tradex", "trading" },
            { "net10", "defi" }, { "farmfactory", "defi" }, { "gaming", "gaming" },
            { "casino", "gaming" }, { "lotto", "gaming" }, { "social", "social" },
            { "ai", "ai" }, { "quantum", "quantum" }, { "education", "education" },
            { "school", "education" }, { "health", "health" }, { "insurance", "insurance" },
            { "services", "services" }, { "appstudio", "development" }, { "appbuilder", "development" },
            { "security", "security" }, { "shop", "commerce" }, { "pos", "commerce" },
            { "inventory", "commerce" }, { "spikeoffice", "business" }, { "rnbcal", "business" },
            { "crm", "business" }, { "documentflow", "government" }, { "esignature", "government" },
            { "voting", "government" }, { "citizencrm", "government" }, { "hrm", "business" },
            { "advocate", "legal" }
        };
        return categories.GetValueOrDefault(key, "general");
    }

    public async Task<PlatformOverview> GetOverviewAsync()
    {
        var services = await GetServicesStatusAsync();
        var onlineCount = services.Values.Count(s => s.Status == "online");
        var totalServices = services.Count;

        return new PlatformOverview
        {
            Modules = totalServices,
            ModulesOnline = onlineCount,
            Tokens = _config?.Tokens?.Count ?? 103,
            Departments = _config?.Departments?.Count ?? 12,
            Services = totalServices,
            Uptime = totalServices > 0 ? $"{((double)onlineCount / totalServices * 100):F1}%" : "0%"
        };
    }

    public async Task<Dictionary<string, ServiceInfo>> GetServicesStatusAsync()
    {
        var result = new Dictionary<string, ServiceInfo>();

        foreach (var (key, service) in _services)
        {
            var status = await CheckServiceHealthAsync(service);
            result[key] = new ServiceInfo
            {
                Id = service.Id,
                Name = service.Name,
                Url = service.Url,
                Port = service.Port,
                Status = status,
                Category = service.Category,
                Type = service.Type
            };
        }

        return result;
    }

    private async Task<string> CheckServiceHealthAsync(ServiceInfo service)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(2);

            var endpoints = new[] { "/health", "/api/health", "/status", "/" };
            foreach (var endpoint in endpoints)
            {
                try
                {
                    var url = service.Url.TrimEnd('/') + endpoint;
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode || (int)response.StatusCode < 500)
                    {
                        return "online";
                    }
                }
                catch
                {
                    continue;
                }
            }
        }
        catch
        {
            // Ignore errors
        }

        return "offline";
    }

    public async Task<ServiceInfo?> GetServiceHealthAsync(string serviceId)
    {
        if (!_services.TryGetValue(serviceId, out var service))
            return null;

        var status = await CheckServiceHealthAsync(service);
        return new ServiceInfo
        {
            Id = service.Id,
            Name = service.Name,
            Url = service.Url,
            Port = service.Port,
            Status = status,
            Category = service.Category,
            Type = service.Type
        };
    }

    public Task<List<ServiceInfo>> GetAllServicesAsync()
    {
        return Task.FromResult(_services.Values.ToList());
    }

    public Task<List<DepartmentInfo>> GetAllDepartmentsAsync()
    {
        return Task.FromResult(_config?.Departments ?? new List<DepartmentInfo>());
    }

    public Task<List<TokenInfo>> GetAllTokensAsync()
    {
        return Task.FromResult(_config?.Tokens ?? new List<TokenInfo>());
    }

    public Task<PlatformConfig> GetConfigAsync()
    {
        return Task.FromResult(_config ?? new PlatformConfig());
    }

    public async Task<List<ServiceInfo>> GetModulesAsync()
    {
        var services = await GetAllServicesAsync();
        return services;
    }
}
