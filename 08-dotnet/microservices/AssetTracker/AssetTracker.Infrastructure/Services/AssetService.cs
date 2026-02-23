using AssetTracker.Core.Interfaces;
using AssetTracker.Core.Models;

namespace AssetTracker.Infrastructure.Services;

public class AssetService : IAssetService
{
    private readonly List<Asset> _assets = new();
    private readonly List<AssetMaintenance> _maintenance = new();
    private readonly List<AssetAssignment> _assignments = new();
    private readonly List<AssetAudit> _audits = new();

    public Task<Asset> CreateAssetAsync(Asset asset) { asset.Id = Guid.NewGuid(); asset.AssetTag = $"AST-{DateTime.UtcNow:yyyyMM}-{_assets.Count + 1:D5}"; asset.Status = AssetStatus.Available; asset.CurrentValue = asset.PurchasePrice; asset.CreatedAt = DateTime.UtcNow; _assets.Add(asset); return Task.FromResult(asset); }
    public Task<Asset?> GetAssetByIdAsync(Guid id) => Task.FromResult(_assets.FirstOrDefault(a => a.Id == id));
    public Task<Asset?> GetAssetByTagAsync(string assetTag) => Task.FromResult(_assets.FirstOrDefault(a => a.AssetTag == assetTag));
    public Task<IEnumerable<Asset>> GetAssetsAsync(AssetCategory? category = null, AssetStatus? status = null, string? department = null, string? location = null)
    {
        var q = _assets.AsEnumerable();
        if (category.HasValue) q = q.Where(a => a.Category == category.Value);
        if (status.HasValue) q = q.Where(a => a.Status == status.Value);
        if (!string.IsNullOrEmpty(department)) q = q.Where(a => a.Department == department);
        if (!string.IsNullOrEmpty(location)) q = q.Where(a => a.Location == location);
        return Task.FromResult(q);
    }
    public Task<IEnumerable<Asset>> GetAssetsByAssigneeAsync(Guid userId) => Task.FromResult(_assets.Where(a => a.AssignedTo == userId));
    public Task<Asset> UpdateAssetAsync(Asset asset) { var e = _assets.FirstOrDefault(a => a.Id == asset.Id); if (e != null) { e.Name = asset.Name; e.Location = asset.Location; e.UpdatedAt = DateTime.UtcNow; } return Task.FromResult(e ?? asset); }

    public Task<Asset> AssignAssetAsync(Guid assetId, Guid userId, string userName, string? department, Guid assignedBy)
    {
        var a = _assets.FirstOrDefault(a => a.Id == assetId);
        if (a != null) { a.AssignedTo = userId; a.AssignedToName = userName; a.AssignedDate = DateTime.UtcNow; a.Department = department; a.Status = AssetStatus.InUse; a.UpdatedAt = DateTime.UtcNow;
            _assignments.Add(new AssetAssignment { Id = Guid.NewGuid(), AssetId = assetId, UserId = userId, UserName = userName, Department = department, AssignedDate = DateTime.UtcNow, AssignedBy = assignedBy }); }
        return Task.FromResult(a!);
    }

    public Task<Asset> ReturnAssetAsync(Guid assetId, string? condition, string? notes)
    {
        var a = _assets.FirstOrDefault(a => a.Id == assetId);
        if (a != null) { var assign = _assignments.FirstOrDefault(x => x.AssetId == assetId && x.ReturnedDate == null); if (assign != null) { assign.ReturnedDate = DateTime.UtcNow; assign.Condition = condition; assign.Notes = notes; }
            a.AssignedTo = null; a.AssignedToName = null; a.Status = AssetStatus.Available; a.UpdatedAt = DateTime.UtcNow; }
        return Task.FromResult(a!);
    }

    public Task<Asset> DisposeAssetAsync(Guid assetId, string reason) { var a = _assets.FirstOrDefault(a => a.Id == assetId); if (a != null) { a.Status = AssetStatus.Disposed; a.DisposedAt = DateTime.UtcNow; a.DisposalReason = reason; } return Task.FromResult(a!); }
    public Task DeleteAssetAsync(Guid id) { _assets.RemoveAll(a => a.Id == id); return Task.CompletedTask; }

    public Task<decimal> CalculateDepreciationAsync(Guid assetId)
    {
        var a = _assets.FirstOrDefault(a => a.Id == assetId);
        if (a == null || a.PurchaseDate == null) return Task.FromResult(0m);
        var monthsOwned = (DateTime.UtcNow - a.PurchaseDate.Value).Days / 30;
        var depreciation = a.DepreciationMethod switch {
            DepreciationMethod.StraightLine => (a.PurchasePrice - a.SalvageValue) / a.UsefulLifeMonths * monthsOwned,
            _ => (a.PurchasePrice - a.SalvageValue) / a.UsefulLifeMonths * monthsOwned
        };
        a.CurrentValue = Math.Max(a.PurchasePrice - depreciation, a.SalvageValue);
        return Task.FromResult(depreciation);
    }

    public Task<AssetMaintenance> ScheduleMaintenanceAsync(AssetMaintenance maintenance) { maintenance.Id = Guid.NewGuid(); maintenance.Status = MaintenanceStatus.Scheduled; _maintenance.Add(maintenance); return Task.FromResult(maintenance); }
    public Task<IEnumerable<AssetMaintenance>> GetMaintenanceHistoryAsync(Guid assetId) => Task.FromResult(_maintenance.Where(m => m.AssetId == assetId));
    public Task<IEnumerable<AssetMaintenance>> GetUpcomingMaintenanceAsync(int daysAhead = 30) => Task.FromResult(_maintenance.Where(m => m.Status == MaintenanceStatus.Scheduled && m.ScheduledDate <= DateTime.UtcNow.AddDays(daysAhead)));
    public Task<AssetMaintenance> CompleteMaintenanceAsync(Guid maintenanceId, decimal? cost, string? notes) { var m = _maintenance.FirstOrDefault(m => m.Id == maintenanceId); if (m != null) { m.Status = MaintenanceStatus.Completed; m.CompletedDate = DateTime.UtcNow; m.Cost = cost; m.Notes = notes; } return Task.FromResult(m!); }

    public Task<IEnumerable<AssetAssignment>> GetAssignmentHistoryAsync(Guid assetId) => Task.FromResult(_assignments.Where(a => a.AssetId == assetId).OrderByDescending(a => a.AssignedDate));

    public Task<AssetAudit> CreateAuditAsync(AssetAudit audit) { audit.Id = Guid.NewGuid(); audit.AuditNumber = $"AUD-{DateTime.UtcNow:yyyyMMdd}-{_audits.Count + 1:D4}"; audit.Status = AuditStatus.Planned; _audits.Add(audit); return Task.FromResult(audit); }
    public Task<AssetAudit?> GetAuditByIdAsync(Guid id) => Task.FromResult(_audits.FirstOrDefault(a => a.Id == id));
    public Task<IEnumerable<AssetAudit>> GetAuditsAsync(AuditStatus? status = null) => Task.FromResult(status.HasValue ? _audits.Where(a => a.Status == status.Value) : _audits.AsEnumerable());
    public Task<AssetAudit> CompleteAuditAsync(Guid auditId, int found, int missing, int discrepancy, string? notes) { var a = _audits.FirstOrDefault(a => a.Id == auditId); if (a != null) { a.Status = AuditStatus.Completed; a.EndDate = DateTime.UtcNow; a.FoundAssets = found; a.MissingAssets = missing; a.DiscrepancyAssets = discrepancy; a.Notes = notes; } return Task.FromResult(a!); }

    public Task<string> GenerateQrCodeAsync(Guid assetId) => Task.FromResult($"https://assets.ierahkwa.gov/qr/{assetId}");

    public Task<AssetStatistics> GetStatisticsAsync(string? department = null)
    {
        var assets = string.IsNullOrEmpty(department) ? _assets : _assets.Where(a => a.Department == department).ToList();
        return Task.FromResult(new AssetStatistics {
            TotalAssets = assets.Count, AssetsInUse = assets.Count(a => a.Status == AssetStatus.InUse), AvailableAssets = assets.Count(a => a.Status == AssetStatus.Available),
            UnderMaintenance = assets.Count(a => a.Status == AssetStatus.UnderMaintenance), TotalPurchaseValue = assets.Sum(a => a.PurchasePrice), TotalCurrentValue = assets.Sum(a => a.CurrentValue),
            TotalDepreciation = assets.Sum(a => a.PurchasePrice - a.CurrentValue), WarrantyExpiringSoon = assets.Count(a => a.WarrantyExpiry <= DateTime.UtcNow.AddDays(30)),
            MaintenanceDue = _maintenance.Count(m => m.Status == MaintenanceStatus.Scheduled && m.ScheduledDate <= DateTime.UtcNow.AddDays(7)),
            AssetsByCategory = assets.GroupBy(a => a.Category.ToString()).ToDictionary(g => g.Key, g => g.Count()),
            AssetsByStatus = assets.GroupBy(a => a.Status.ToString()).ToDictionary(g => g.Key, g => g.Count())
        });
    }
}
