using IERAHKWA.Platform.Models;

namespace IERAHKWA.Platform.Services;

public interface IPlatformService
{
    Task<PlatformOverview> GetOverviewAsync();
    Task<Dictionary<string, ServiceInfo>> GetServicesStatusAsync();
    Task<ServiceInfo?> GetServiceHealthAsync(string serviceId);
    Task<List<ServiceInfo>> GetAllServicesAsync();
    Task<List<DepartmentInfo>> GetAllDepartmentsAsync();
    Task<List<TokenInfo>> GetAllTokensAsync();
    Task<PlatformConfig> GetConfigAsync();
    Task<List<ServiceInfo>> GetModulesAsync();
}
