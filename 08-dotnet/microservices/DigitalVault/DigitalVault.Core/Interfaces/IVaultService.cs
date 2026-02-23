using DigitalVault.Core.Models;
namespace DigitalVault.Core.Interfaces;

public interface IVaultService
{
    Task<ArchiveItem> ArchiveItemAsync(ArchiveItem item, Stream fileStream);
    Task<ArchiveItem?> GetItemByIdAsync(Guid id);
    Task<IEnumerable<ArchiveItem>> SearchItemsAsync(string? query, ArchiveType? type = null, ClassificationLevel? classification = null, string? department = null);
    Task<ArchiveItem> UpdateItemAsync(ArchiveItem item);
    Task<Stream> DownloadItemAsync(Guid id, Guid userId);
    Task<ArchiveItem> SetLegalHoldAsync(Guid id, bool hold, string? reason);
    Task SoftDeleteItemAsync(Guid id);
    Task PermanentDeleteItemAsync(Guid id);
    Task<ArchiveItem> RestoreItemAsync(Guid id);

    Task<ArchiveFolder> CreateFolderAsync(ArchiveFolder folder);
    Task<ArchiveFolder?> GetFolderByIdAsync(Guid id);
    Task<IEnumerable<ArchiveFolder>> GetFoldersAsync(Guid? parentId = null, string? department = null);
    Task<ArchiveFolder> UpdateFolderAsync(ArchiveFolder folder);
    Task DeleteFolderAsync(Guid id);
    Task<IEnumerable<ArchiveItem>> GetFolderItemsAsync(Guid folderId);

    Task LogAccessAsync(ArchiveAccess access);
    Task<IEnumerable<ArchiveAccess>> GetAccessLogAsync(Guid itemId);

    Task<RetentionPolicy> CreatePolicyAsync(RetentionPolicy policy);
    Task<IEnumerable<RetentionPolicy>> GetPoliciesAsync();
    Task<RetentionPolicy> UpdatePolicyAsync(RetentionPolicy policy);
    Task ApplyRetentionPoliciesAsync();

    Task<ArchiveRequest> CreateRequestAsync(ArchiveRequest request);
    Task<IEnumerable<ArchiveRequest>> GetRequestsAsync(RequestStatus? status = null, Guid? userId = null);
    Task<ArchiveRequest> ProcessRequestAsync(Guid requestId, bool approved, Guid approvedBy, string? notes);

    Task<string> RegisterOnBlockchainAsync(Guid itemId);
    Task<bool> VerifyIntegrityAsync(Guid itemId);
    Task<VaultStatistics> GetStatisticsAsync(string? department = null);
}

public class VaultStatistics
{
    public int TotalItems { get; set; }
    public long TotalStorageBytes { get; set; }
    public int ItemsByClassification { get; set; }
    public int LegalHoldItems { get; set; }
    public int ExpiringThisMonth { get; set; }
    public int PendingRequests { get; set; }
    public Dictionary<string, int> ItemsByType { get; set; } = new();
    public Dictionary<string, long> StorageByDepartment { get; set; } = new();
}
