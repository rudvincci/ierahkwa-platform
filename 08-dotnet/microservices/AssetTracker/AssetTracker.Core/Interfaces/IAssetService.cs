using AssetTracker.Core.Models;

namespace AssetTracker.Core.Interfaces;

public interface IAssetService
{
    Task<Asset> CreateAssetAsync(Asset asset);
    Task<Asset?> GetAssetByIdAsync(Guid id);
    Task<Asset?> GetAssetByTagAsync(string assetTag);
    Task<IEnumerable<Asset>> GetAssetsAsync(AssetCategory? category = null, AssetStatus? status = null, string? department = null, string? location = null);
    Task<IEnumerable<Asset>> GetAssetsByAssigneeAsync(Guid userId);
    Task<Asset> UpdateAssetAsync(Asset asset);
    Task<Asset> AssignAssetAsync(Guid assetId, Guid userId, string userName, string? department, Guid assignedBy);
    Task<Asset> ReturnAssetAsync(Guid assetId, string? condition, string? notes);
    Task<Asset> DisposeAssetAsync(Guid assetId, string reason);
    Task DeleteAssetAsync(Guid id);
    Task<decimal> CalculateDepreciationAsync(Guid assetId);

    Task<AssetMaintenance> ScheduleMaintenanceAsync(AssetMaintenance maintenance);
    Task<IEnumerable<AssetMaintenance>> GetMaintenanceHistoryAsync(Guid assetId);
    Task<IEnumerable<AssetMaintenance>> GetUpcomingMaintenanceAsync(int daysAhead = 30);
    Task<AssetMaintenance> CompleteMaintenanceAsync(Guid maintenanceId, decimal? cost, string? notes);

    Task<IEnumerable<AssetAssignment>> GetAssignmentHistoryAsync(Guid assetId);

    Task<AssetAudit> CreateAuditAsync(AssetAudit audit);
    Task<AssetAudit?> GetAuditByIdAsync(Guid id);
    Task<IEnumerable<AssetAudit>> GetAuditsAsync(AuditStatus? status = null);
    Task<AssetAudit> CompleteAuditAsync(Guid auditId, int found, int missing, int discrepancy, string? notes);

    Task<string> GenerateQrCodeAsync(Guid assetId);
    Task<AssetStatistics> GetStatisticsAsync(string? department = null);
}

public class AssetStatistics
{
    public int TotalAssets { get; set; }
    public int AssetsInUse { get; set; }
    public int AvailableAssets { get; set; }
    public int UnderMaintenance { get; set; }
    public decimal TotalPurchaseValue { get; set; }
    public decimal TotalCurrentValue { get; set; }
    public decimal TotalDepreciation { get; set; }
    public int WarrantyExpiringSoon { get; set; }
    public int MaintenanceDue { get; set; }
    public Dictionary<string, int> AssetsByCategory { get; set; } = new();
    public Dictionary<string, int> AssetsByStatus { get; set; } = new();
    public Dictionary<string, int> AssetsByDepartment { get; set; } = new();
}
