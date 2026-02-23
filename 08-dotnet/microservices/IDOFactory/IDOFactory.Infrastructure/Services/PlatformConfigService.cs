using IDOFactory.Core.Interfaces;
using IDOFactory.Core.Models;

namespace IDOFactory.Infrastructure.Services;

public class PlatformConfigService : IPlatformConfigService
{
    private PlatformConfig _config;

    public PlatformConfigService()
    {
        _config = new PlatformConfig();
    }

    public Task<PlatformConfig> GetConfigAsync() => Task.FromResult(_config);

    public Task<PlatformConfig> UpdateConfigAsync(PlatformConfig config, string adminAddress)
    {
        if (!_config.AdminAddresses.Contains(adminAddress))
            throw new UnauthorizedAccessException("Only admins can update configuration");
        
        config.UpdatedAt = DateTime.UtcNow;
        _config = config;
        return Task.FromResult(_config);
    }

    public Task<bool> IsAdminAsync(string address) =>
        Task.FromResult(_config.AdminAddresses.Contains(address));

    public Task<bool> AddAdminAsync(string address, string currentAdmin)
    {
        if (!_config.AdminAddresses.Contains(currentAdmin))
            throw new UnauthorizedAccessException("Only admins can add new admins");
        
        if (!_config.AdminAddresses.Contains(address))
        {
            _config.AdminAddresses.Add(address);
            _config.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(true);
    }

    public Task<bool> RemoveAdminAsync(string address, string currentAdmin)
    {
        if (!_config.AdminAddresses.Contains(currentAdmin))
            throw new UnauthorizedAccessException("Only admins can remove admins");
        
        if (_config.AdminAddresses.Count <= 1)
            throw new InvalidOperationException("Cannot remove the last admin");
        
        _config.AdminAddresses.Remove(address);
        _config.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }

    public Task<IEnumerable<SupportedNetwork>> GetSupportedNetworksAsync() =>
        Task.FromResult<IEnumerable<SupportedNetwork>>(_config.Networks.Where(n => n.IsEnabled).ToList());

    public Task<bool> AddNetworkAsync(SupportedNetwork network, string adminAddress)
    {
        if (!_config.AdminAddresses.Contains(adminAddress))
            throw new UnauthorizedAccessException("Only admins can add networks");
        
        if (_config.Networks.Any(n => n.ChainId == network.ChainId))
            throw new InvalidOperationException("Network with this chain ID already exists");
        
        _config.Networks.Add(network);
        _config.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(true);
    }

    public Task<bool> RemoveNetworkAsync(int chainId, string adminAddress)
    {
        if (!_config.AdminAddresses.Contains(adminAddress))
            throw new UnauthorizedAccessException("Only admins can remove networks");
        
        var network = _config.Networks.FirstOrDefault(n => n.ChainId == chainId);
        if (network != null)
        {
            _config.Networks.Remove(network);
            _config.UpdatedAt = DateTime.UtcNow;
        }
        return Task.FromResult(true);
    }
}
