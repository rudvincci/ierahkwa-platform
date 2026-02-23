using IDOFactory.Core.Models;

namespace IDOFactory.Core.Interfaces;

public interface IIDOService
{
    // Pools
    Task<IEnumerable<IDOPool>> GetAllPoolsAsync();
    Task<IEnumerable<IDOPool>> GetPoolsByStatusAsync(IDOPoolStatus status);
    Task<IDOPool?> GetPoolByIdAsync(string poolId);
    Task<IDOPool> CreatePoolAsync(IDOPool pool);
    Task<IDOPool> UpdatePoolAsync(IDOPool pool);
    Task<bool> DeletePoolAsync(string poolId);
    
    // Pool Status Management
    Task<IDOPool> StartRegistrationAsync(string poolId);
    Task<IDOPool> StartSaleAsync(string poolId);
    Task<IDOPool> EndSaleAsync(string poolId);
    Task<IDOPool> FinalizePoolAsync(string poolId);
    Task<IDOPool> CancelPoolAsync(string poolId);
    
    // Contributions
    Task<IDOContribution> ContributeAsync(string poolId, string userAddress, decimal amount);
    Task<IEnumerable<IDOContribution>> GetUserContributionsAsync(string userAddress);
    Task<IEnumerable<IDOContribution>> GetPoolContributionsAsync(string poolId);
    Task<IDOContribution> ClaimTokensAsync(string contributionId, string userAddress);
    Task<IDOContribution> RefundAsync(string contributionId, string userAddress);
    
    // Whitelist
    Task<bool> AddToWhitelistAsync(string poolId, IEnumerable<string> addresses);
    Task<bool> RemoveFromWhitelistAsync(string poolId, IEnumerable<string> addresses);
    Task<bool> IsWhitelistedAsync(string poolId, string address);
    
    // Statistics
    Task<IDOStatistics> GetPlatformStatisticsAsync();
    Task<IDOPoolStatistics> GetPoolStatisticsAsync(string poolId);
}

public interface ITokenLockerService
{
    Task<IEnumerable<TokenLocker>> GetAllLockersAsync();
    Task<IEnumerable<TokenLocker>> GetLockersByOwnerAsync(string ownerAddress);
    Task<IEnumerable<TokenLocker>> GetLockersByTokenAsync(string tokenAddress);
    Task<TokenLocker?> GetLockerByIdAsync(string lockerId);
    
    Task<TokenLocker> CreateLockAsync(TokenLocker locker);
    Task<TokenLocker> UnlockTokensAsync(string lockerId, string ownerAddress, decimal amount);
    Task<TokenLocker> TransferOwnershipAsync(string lockerId, string currentOwner, string newOwner);
    Task<TokenLocker> EmergencyWithdrawAsync(string lockerId, string ownerAddress);
    
    Task<decimal> GetUnlockableAmountAsync(string lockerId);
    Task<DateTime> GetNextUnlockDateAsync(string lockerId);
    
    Task<TokenLockerStatistics> GetStatisticsAsync();
}

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllProjectsAsync();
    Task<IEnumerable<Project>> GetApprovedProjectsAsync();
    Task<Project?> GetProjectByIdAsync(string projectId);
    Task<Project?> GetProjectByOwnerAsync(string ownerAddress);
    
    Task<Project> CreateProjectAsync(Project project);
    Task<Project> UpdateProjectAsync(Project project);
    Task<bool> DeleteProjectAsync(string projectId);
    
    Task<Project> ApproveProjectAsync(string projectId, string adminAddress);
    Task<Project> RejectProjectAsync(string projectId, string adminAddress, string reason);
    Task<Project> SuspendProjectAsync(string projectId, string adminAddress, string reason);
}

public interface IPlatformConfigService
{
    Task<PlatformConfig> GetConfigAsync();
    Task<PlatformConfig> UpdateConfigAsync(PlatformConfig config, string adminAddress);
    
    Task<bool> IsAdminAsync(string address);
    Task<bool> AddAdminAsync(string address, string currentAdmin);
    Task<bool> RemoveAdminAsync(string address, string currentAdmin);
    
    Task<IEnumerable<SupportedNetwork>> GetSupportedNetworksAsync();
    Task<bool> AddNetworkAsync(SupportedNetwork network, string adminAddress);
    Task<bool> RemoveNetworkAsync(int chainId, string adminAddress);
}

// Statistics DTOs
public class IDOStatistics
{
    public int TotalPools { get; set; }
    public int ActivePools { get; set; }
    public int SuccessfulPools { get; set; }
    public decimal TotalFundsRaised { get; set; }
    public int TotalParticipants { get; set; }
    public int TotalProjects { get; set; }
}

public class IDOPoolStatistics
{
    public string PoolId { get; set; } = string.Empty;
    public decimal FundsRaised { get; set; }
    public decimal PercentageFilled { get; set; }
    public int Participants { get; set; }
    public decimal AverageContribution { get; set; }
    public TimeSpan TimeRemaining { get; set; }
}

public class TokenLockerStatistics
{
    public int TotalLocks { get; set; }
    public decimal TotalValueLocked { get; set; }
    public int ActiveLocks { get; set; }
    public int UniqueTokensLocked { get; set; }
}
