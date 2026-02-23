using DigitalVault.Core.Interfaces;
using DigitalVault.Core.Models;
using System.Security.Cryptography;
namespace DigitalVault.Infrastructure.Services;

public class VaultService : IVaultService
{
    private readonly List<ArchiveItem> _items = new();
    private readonly List<ArchiveFolder> _folders = new();
    private readonly List<ArchiveAccess> _accessLog = new();
    private readonly List<RetentionPolicy> _policies = new();
    private readonly List<ArchiveRequest> _requests = new();

    public async Task<ArchiveItem> ArchiveItemAsync(ArchiveItem item, Stream fileStream) { item.Id = Guid.NewGuid(); item.ArchiveCode = $"ARC-{DateTime.UtcNow:yyyyMMdd}-{_items.Count + 1:D6}"; item.HashSHA256 = await ComputeHashAsync(fileStream); item.StoragePath = $"/vault/{item.Id}/{item.FileName}"; item.CreatedAt = DateTime.UtcNow; _items.Add(item); return item; }
    public Task<ArchiveItem?> GetItemByIdAsync(Guid id) => Task.FromResult(_items.FirstOrDefault(i => i.Id == id && !i.IsDeleted));
    public Task<IEnumerable<ArchiveItem>> SearchItemsAsync(string? query, ArchiveType? type = null, ClassificationLevel? classification = null, string? department = null) { var q = _items.Where(i => !i.IsDeleted); if (!string.IsNullOrEmpty(query)) q = q.Where(i => i.Name.Contains(query, StringComparison.OrdinalIgnoreCase)); if (type.HasValue) q = q.Where(i => i.Type == type.Value); if (classification.HasValue) q = q.Where(i => i.Classification == classification.Value); if (!string.IsNullOrEmpty(department)) q = q.Where(i => i.Department == department); return Task.FromResult(q); }
    public Task<ArchiveItem> UpdateItemAsync(ArchiveItem item) { var e = _items.FirstOrDefault(i => i.Id == item.Id); if (e != null) { e.Name = item.Name; e.Tags = item.Tags; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? item); }
    public async Task<Stream> DownloadItemAsync(Guid id, Guid userId) { await LogAccessAsync(new ArchiveAccess { ArchiveItemId = id, UserId = userId, Action = AccessAction.Download }); return new MemoryStream(); }
    public Task<ArchiveItem> SetLegalHoldAsync(Guid id, bool hold, string? reason) { var i = _items.FirstOrDefault(i => i.Id == id); if (i != null) { i.IsLegalHold = hold; i.LegalHoldReason = reason; i.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(i!); }
    public Task SoftDeleteItemAsync(Guid id) { var i = _items.FirstOrDefault(i => i.Id == id); if (i != null && !i.IsLegalHold) { i.IsDeleted = true; i.DeletedAt = DateTime.UtcNow; } return Task.CompletedTask; }
    public Task PermanentDeleteItemAsync(Guid id) { _items.RemoveAll(i => i.Id == id && !i.IsLegalHold); return Task.CompletedTask; }
    public Task<ArchiveItem> RestoreItemAsync(Guid id) { var i = _items.FirstOrDefault(i => i.Id == id); if (i != null) { i.IsDeleted = false; i.DeletedAt = null; } return Task.FromResult(i!); }

    public Task<ArchiveFolder> CreateFolderAsync(ArchiveFolder folder) { folder.Id = Guid.NewGuid(); folder.CreatedAt = DateTime.UtcNow; _folders.Add(folder); return Task.FromResult(folder); }
    public Task<ArchiveFolder?> GetFolderByIdAsync(Guid id) => Task.FromResult(_folders.FirstOrDefault(f => f.Id == id));
    public Task<IEnumerable<ArchiveFolder>> GetFoldersAsync(Guid? parentId = null, string? department = null) { var q = _folders.AsEnumerable(); if (parentId.HasValue) q = q.Where(f => f.ParentFolderId == parentId.Value); else q = q.Where(f => f.ParentFolderId == null); if (!string.IsNullOrEmpty(department)) q = q.Where(f => f.Department == department); return Task.FromResult(q); }
    public Task<ArchiveFolder> UpdateFolderAsync(ArchiveFolder folder) { var e = _folders.FirstOrDefault(f => f.Id == folder.Id); if (e != null) e.Name = folder.Name; return Task.FromResult(e ?? folder); }
    public Task DeleteFolderAsync(Guid id) { _folders.RemoveAll(f => f.Id == id); return Task.CompletedTask; }
    public Task<IEnumerable<ArchiveItem>> GetFolderItemsAsync(Guid folderId) => Task.FromResult(_items.Where(i => i.ParentFolderId == folderId && !i.IsDeleted));

    public Task LogAccessAsync(ArchiveAccess access) { access.Id = Guid.NewGuid(); access.AccessedAt = DateTime.UtcNow; _accessLog.Add(access); return Task.CompletedTask; }
    public Task<IEnumerable<ArchiveAccess>> GetAccessLogAsync(Guid itemId) => Task.FromResult(_accessLog.Where(a => a.ArchiveItemId == itemId).OrderByDescending(a => a.AccessedAt));

    public Task<RetentionPolicy> CreatePolicyAsync(RetentionPolicy policy) { policy.Id = Guid.NewGuid(); policy.CreatedAt = DateTime.UtcNow; _policies.Add(policy); return Task.FromResult(policy); }
    public Task<IEnumerable<RetentionPolicy>> GetPoliciesAsync() => Task.FromResult(_policies.Where(p => p.IsActive));
    public Task<RetentionPolicy> UpdatePolicyAsync(RetentionPolicy policy) { var e = _policies.FirstOrDefault(p => p.Id == policy.Id); if (e != null) { e.Name = policy.Name; e.RetentionYears = policy.RetentionYears; } return Task.FromResult(e ?? policy); }
    public Task ApplyRetentionPoliciesAsync() => Task.CompletedTask;

    public Task<ArchiveRequest> CreateRequestAsync(ArchiveRequest request) { request.Id = Guid.NewGuid(); request.RequestNumber = $"REQ-{_requests.Count + 1:D5}"; request.Status = RequestStatus.Pending; request.RequestedAt = DateTime.UtcNow; _requests.Add(request); return Task.FromResult(request); }
    public Task<IEnumerable<ArchiveRequest>> GetRequestsAsync(RequestStatus? status = null, Guid? userId = null) { var q = _requests.AsEnumerable(); if (status.HasValue) q = q.Where(r => r.Status == status.Value); if (userId.HasValue) q = q.Where(r => r.RequestedBy == userId.Value); return Task.FromResult(q); }
    public Task<ArchiveRequest> ProcessRequestAsync(Guid requestId, bool approved, Guid approvedBy, string? notes) { var r = _requests.FirstOrDefault(r => r.Id == requestId); if (r != null) { r.Status = approved ? RequestStatus.Approved : RequestStatus.Rejected; r.ApprovedBy = approvedBy; r.ApprovedAt = DateTime.UtcNow; r.ApprovalNotes = notes; } return Task.FromResult(r!); }

    public Task<string> RegisterOnBlockchainAsync(Guid itemId) { var i = _items.FirstOrDefault(i => i.Id == itemId); if (i != null) i.BlockchainHash = $"0x{Guid.NewGuid():N}"; return Task.FromResult(i?.BlockchainHash ?? ""); }
    public Task<bool> VerifyIntegrityAsync(Guid itemId) => Task.FromResult(true);

    public Task<VaultStatistics> GetStatisticsAsync(string? department = null)
    {
        var items = string.IsNullOrEmpty(department) ? _items.Where(i => !i.IsDeleted) : _items.Where(i => !i.IsDeleted && i.Department == department);
        var list = items.ToList();
        return Task.FromResult(new VaultStatistics { TotalItems = list.Count, TotalStorageBytes = list.Sum(i => i.FileSize), LegalHoldItems = list.Count(i => i.IsLegalHold), ExpiringThisMonth = list.Count(i => i.RetentionUntil?.Month == DateTime.UtcNow.Month), PendingRequests = _requests.Count(r => r.Status == RequestStatus.Pending), ItemsByType = list.GroupBy(i => i.Type.ToString()).ToDictionary(g => g.Key, g => g.Count()), StorageByDepartment = list.Where(i => i.Department != null).GroupBy(i => i.Department!).ToDictionary(g => g.Key, g => g.Sum(i => i.FileSize)) });
    }

    private async Task<string> ComputeHashAsync(Stream stream) { using var sha256 = SHA256.Create(); var hash = await sha256.ComputeHashAsync(stream); stream.Position = 0; return Convert.ToHexString(hash); }
}
