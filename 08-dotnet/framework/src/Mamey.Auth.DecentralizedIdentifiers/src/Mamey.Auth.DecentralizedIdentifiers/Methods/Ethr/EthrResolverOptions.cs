using System.Collections.Generic;

namespace Mamey.Auth.DecentralizedIdentifiers.Methods.Ethr;

/// <summary>
/// Configuration options for DID Ethr resolver
/// </summary>
public class EthrResolverOptions
{
    /// <summary>
    /// Enable caching of resolved DID documents
    /// </summary>
    public bool EnableCaching { get; set; } = true;
    
    /// <summary>
    /// Cache TTL in minutes
    /// </summary>
    public int CacheTtlMinutes { get; set; } = 60;
    
    /// <summary>
    /// Ethereum network (mainnet, goerli, sepolia, etc.)
    /// </summary>
    public string Network { get; set; } = "mainnet";
    
    /// <summary>
    /// Gas price in Gwei
    /// </summary>
    public decimal GasPriceGwei { get; set; } = 20;
    
    /// <summary>
    /// Gas limit for transactions
    /// </summary>
    public long GasLimit { get; set; } = 600000;
    
    /// <summary>
    /// HTTP timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
    
    /// <summary>
    /// Maximum number of retries for failed requests
    /// </summary>
    public int MaxRetries { get; set; } = 3;
    
    /// <summary>
    /// Retry delay in milliseconds
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;
    
    /// <summary>
    /// Enable transaction confirmation waiting
    /// </summary>
    public bool WaitForConfirmation { get; set; } = true;
    
    /// <summary>
    /// Number of confirmations to wait for
    /// </summary>
    public int Confirmations { get; set; } = 1;
    
    /// <summary>
    /// Allowed Ethereum addresses (if empty, all addresses are allowed)
    /// </summary>
    public List<string> AllowedAddresses { get; set; } = new List<string>();
    
    /// <summary>
    /// Blocked Ethereum addresses
    /// </summary>
    public List<string> BlockedAddresses { get; set; } = new List<string>();
}





