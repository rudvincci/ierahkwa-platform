using Mamey.FWID.Identities.Application.ClanRegistrar.Events;
using Mamey.FWID.Identities.Application.ClanRegistrar.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.ClanRegistrar.Services;

/// <summary>
/// Clan registrar service implementation.
/// Manages clan registrar workflows and identity approvals.
/// </summary>
public class ClanRegistrarService : IClanRegistrarService
{
    private readonly ILogger<ClanRegistrarService> _logger;
    
    // In-memory storage for demo - use repository in production
    private readonly Dictionary<Guid, Models.ClanRegistrar> _registrars = new();
    private readonly Dictionary<Guid, RegistrationApproval> _approvals = new();
    private readonly object _lock = new();
    
    public ClanRegistrarService(ILogger<ClanRegistrarService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        // Initialize with default registrar for demo
        InitializeDefaultRegistrars();
    }
    
    #region Registrar Management
    
    /// <inheritdoc />
    public Task<Models.ClanRegistrar?> GetRegistrarAsync(Guid registrarId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _registrars.TryGetValue(registrarId, out var registrar);
            return Task.FromResult(registrar);
        }
    }
    
    /// <inheritdoc />
    public Task<Models.ClanRegistrar?> GetRegistrarByIdentityAsync(Guid identityId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var registrar = _registrars.Values.FirstOrDefault(r => r.IdentityId == identityId && r.IsActive);
            return Task.FromResult(registrar);
        }
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<Models.ClanRegistrar>> GetRegistrarsByZoneAsync(string zone, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var registrars = _registrars.Values
                .Where(r => r.Zone.Equals(zone, StringComparison.OrdinalIgnoreCase) && r.IsActive)
                .ToList();
            return Task.FromResult<IReadOnlyList<Models.ClanRegistrar>>(registrars);
        }
    }
    
    /// <inheritdoc />
    public async Task<Models.ClanRegistrar?> FindRegistrarForIdentityAsync(
        Guid identityId,
        string zone,
        string? clanName = null,
        CancellationToken cancellationToken = default)
    {
        var zoneRegistrars = await GetRegistrarsByZoneAsync(zone, cancellationToken);
        
        // Priority: Clan Matriarch > Clan Elder > Institutional > Delegate
        Models.ClanRegistrar? registrar = null;
        
        // First, try to find a clan matriarch for the specific clan
        if (!string.IsNullOrEmpty(clanName))
        {
            registrar = zoneRegistrars.FirstOrDefault(r => 
                r.Type == RegistrarType.ClanMatriarch && 
                r.ClanName?.Equals(clanName, StringComparison.OrdinalIgnoreCase) == true);
        }
        
        // Fallback to any clan matriarch in the zone
        registrar ??= zoneRegistrars.FirstOrDefault(r => r.Type == RegistrarType.ClanMatriarch);
        
        // Fallback to clan elder
        registrar ??= zoneRegistrars.FirstOrDefault(r => r.Type == RegistrarType.ClanElder);
        
        // Fallback to institutional
        registrar ??= zoneRegistrars.FirstOrDefault(r => r.Type == RegistrarType.Institutional);
        
        // Fallback to delegate
        registrar ??= zoneRegistrars.FirstOrDefault(r => r.Type == RegistrarType.Delegate);
        
        _logger.LogDebug("Found registrar {RegistrarId} for zone {Zone}, clan {Clan}", 
            registrar?.Id, zone, clanName);
            
        return registrar;
    }
    
    /// <inheritdoc />
    public Task<Models.ClanRegistrar> RegisterRegistrarAsync(
        RegisterRegistrarRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Registering new registrar for zone {Zone}, type {Type}", 
            request.Zone, request.Type);
            
        var registrar = new Models.ClanRegistrar
        {
            IdentityId = request.IdentityId,
            DID = request.DID,
            Type = request.Type,
            Zone = request.Zone.ToLowerInvariant(),
            ClanName = request.ClanName,
            InstitutionName = request.InstitutionName,
            Title = request.Title,
            Scope = request.Scope ?? new RegistrarScope(),
            AppointedBy = request.AppointedBy,
            ExpiresAt = request.ExpiresAt
        };
        
        lock (_lock)
        {
            _registrars[registrar.Id] = registrar;
        }
        
        _logger.LogInformation("Registrar {RegistrarId} registered successfully", registrar.Id);
        return Task.FromResult(registrar);
    }
    
    /// <inheritdoc />
    public Task<bool> DeactivateRegistrarAsync(Guid registrarId, string reason, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_registrars.TryGetValue(registrarId, out var registrar))
            {
                registrar.IsActive = false;
                _logger.LogInformation("Deactivated registrar {RegistrarId}. Reason: {Reason}", registrarId, reason);
                return Task.FromResult(true);
            }
        }
        return Task.FromResult(false);
    }
    
    /// <inheritdoc />
    public Task<bool> DelegateAuthorityAsync(
        Guid fromRegistrarId,
        Guid toRegistrarId,
        RegistrarScope scope,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_registrars.TryGetValue(fromRegistrarId, out var fromRegistrar) || !fromRegistrar.Scope.CanDelegate)
            {
                _logger.LogWarning("Registrar {FromId} cannot delegate authority", fromRegistrarId);
                return Task.FromResult(false);
            }
            
            if (!_registrars.TryGetValue(toRegistrarId, out var toRegistrar))
            {
                _logger.LogWarning("Target registrar {ToId} not found", toRegistrarId);
                return Task.FromResult(false);
            }
            
            fromRegistrar.DelegateRegistrarIds.Add(toRegistrarId);
            _logger.LogInformation("Authority delegated from {FromId} to {ToId}", fromRegistrarId, toRegistrarId);
            return Task.FromResult(true);
        }
    }
    
    #endregion
    
    #region Approval Workflow
    
    /// <inheritdoc />
    public async Task<RegistrationApproval> SubmitForApprovalAsync(
        SubmitApprovalRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Submitting identity {IdentityId} for approval in zone {Zone}",
            request.IdentityId, request.Zone);
            
        // Find appropriate registrar
        var registrar = await FindRegistrarForIdentityAsync(
            request.IdentityId, request.Zone, request.ClanName, cancellationToken);
            
        if (registrar == null)
        {
            throw new InvalidOperationException($"No registrar available for zone {request.Zone}");
        }
        
        var approval = new RegistrationApproval
        {
            IdentityId = request.IdentityId,
            RegistrarId = registrar.Id,
            Priority = request.Priority,
            RequiredDocuments = request.DocumentTypes.Select(dt => new RequiredDocument
            {
                DocumentType = dt,
                IsProvided = false,
                IsVerified = false
            }).ToList()
        };
        
        approval.History.Add(new ApprovalHistoryEntry
        {
            Action = "Submitted",
            FromStatus = ApprovalStatus.Pending,
            ToStatus = ApprovalStatus.Pending,
            Notes = $"Submitted to registrar {registrar.Id}"
        });
        
        lock (_lock)
        {
            _approvals[approval.Id] = approval;
        }
        
        // TODO: Send notification to registrar
        // await _notificationService.NotifyRegistrarAsync(registrar.Id, approval);
        
        _logger.LogInformation("Approval {ApprovalId} created for identity {IdentityId}, assigned to registrar {RegistrarId}",
            approval.Id, request.IdentityId, registrar.Id);
            
        return approval;
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<RegistrationApproval>> GetPendingApprovalsAsync(
        Guid registrarId,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var pending = _approvals.Values
                .Where(a => a.RegistrarId == registrarId && 
                           (a.Status == ApprovalStatus.Pending || 
                            a.Status == ApprovalStatus.InReview ||
                            a.Status == ApprovalStatus.AdditionalInfoRequired))
                .OrderByDescending(a => a.Priority)
                .ThenBy(a => a.SubmittedAt)
                .ToList();
            return Task.FromResult<IReadOnlyList<RegistrationApproval>>(pending);
        }
    }
    
    /// <inheritdoc />
    public Task<RegistrationApproval?> GetApprovalAsync(Guid approvalId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _approvals.TryGetValue(approvalId, out var approval);
            return Task.FromResult(approval);
        }
    }
    
    /// <inheritdoc />
    public async Task<RegistrationApproval> ReviewApprovalAsync(
        ReviewApprovalRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Reviewing approval {ApprovalId} by registrar {RegistrarId}",
            request.ApprovalId, request.RegistrarId);
            
        var approval = await GetApprovalAsync(request.ApprovalId, cancellationToken)
            ?? throw new InvalidOperationException($"Approval {request.ApprovalId} not found");
            
        var registrar = await GetRegistrarAsync(request.RegistrarId, cancellationToken)
            ?? throw new InvalidOperationException($"Registrar {request.RegistrarId} not found");
            
        if (approval.RegistrarId != request.RegistrarId && 
            !registrar.DelegateRegistrarIds.Contains(approval.RegistrarId))
        {
            throw new UnauthorizedAccessException("Registrar not authorized for this approval");
        }
        
        var previousStatus = approval.Status;
        
        approval.ReviewedAt = DateTime.UtcNow;
        approval.ReviewedBy = request.RegistrarId;
        approval.Decision = request.Decision;
        approval.DecisionReason = request.Reason;
        approval.RegistrarNotes = request.Notes;
        
        approval.Status = request.Decision switch
        {
            ApprovalDecision.Approved => ApprovalStatus.Approved,
            ApprovalDecision.Rejected => ApprovalStatus.Rejected,
            ApprovalDecision.NeedsMoreInfo => ApprovalStatus.AdditionalInfoRequired,
            ApprovalDecision.Escalate => ApprovalStatus.Escalated,
            _ => approval.Status
        };
        
        approval.History.Add(new ApprovalHistoryEntry
        {
            Action = $"Reviewed: {request.Decision}",
            ActorId = request.RegistrarId,
            FromStatus = previousStatus,
            ToStatus = approval.Status,
            Notes = request.Reason
        });
        
        if (approval.Status == ApprovalStatus.Approved)
        {
            _logger.LogInformation("Identity {IdentityId} approved by registrar {RegistrarId}",
                approval.IdentityId, request.RegistrarId);
                
            // Publish event for downstream processing
            // await _eventDispatcher.PublishAsync(new IdentityApprovedByClanEvent(...));
        }
        
        return approval;
    }
    
    /// <inheritdoc />
    public async Task<RegistrationApproval> RequestAdditionalInfoAsync(
        Guid approvalId,
        Guid registrarId,
        string requestedInfo,
        CancellationToken cancellationToken = default)
    {
        return await ReviewApprovalAsync(new ReviewApprovalRequest
        {
            ApprovalId = approvalId,
            RegistrarId = registrarId,
            Decision = ApprovalDecision.NeedsMoreInfo,
            Reason = requestedInfo
        }, cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<RegistrationApproval> EscalateToCouncilAsync(
        Guid approvalId,
        Guid registrarId,
        string reason,
        CancellationToken cancellationToken = default)
    {
        var approval = await GetApprovalAsync(approvalId, cancellationToken)
            ?? throw new InvalidOperationException($"Approval {approvalId} not found");
            
        approval.EscalatedToCouncilId = Guid.NewGuid(); // Council ID would come from configuration
        
        return await ReviewApprovalAsync(new ReviewApprovalRequest
        {
            ApprovalId = approvalId,
            RegistrarId = registrarId,
            Decision = ApprovalDecision.Escalate,
            Reason = reason
        }, cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<RegistrationApproval> ProvideDocumentAsync(
        Guid approvalId,
        string documentType,
        string documentId,
        CancellationToken cancellationToken = default)
    {
        var approval = await GetApprovalAsync(approvalId, cancellationToken)
            ?? throw new InvalidOperationException($"Approval {approvalId} not found");
            
        var doc = approval.RequiredDocuments.FirstOrDefault(d => 
            d.DocumentType.Equals(documentType, StringComparison.OrdinalIgnoreCase));
            
        if (doc != null)
        {
            doc.DocumentId = documentId;
            doc.IsProvided = true;
            
            approval.History.Add(new ApprovalHistoryEntry
            {
                Action = $"Document provided: {documentType}",
                FromStatus = approval.Status,
                ToStatus = approval.Status,
                Notes = $"Document ID: {documentId}"
            });
        }
        
        return approval;
    }
    
    #endregion
    
    #region Statistics
    
    /// <inheritdoc />
    public Task<RegistrarStatistics> GetStatisticsAsync(
        Guid registrarId,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var registrarApprovals = _approvals.Values.Where(a => a.RegistrarId == registrarId).ToList();
            
            var stats = new RegistrarStatistics
            {
                RegistrarId = registrarId,
                TotalApprovals = registrarApprovals.Count,
                ApprovedCount = registrarApprovals.Count(a => a.Status == ApprovalStatus.Approved),
                RejectedCount = registrarApprovals.Count(a => a.Status == ApprovalStatus.Rejected),
                PendingCount = registrarApprovals.Count(a => 
                    a.Status == ApprovalStatus.Pending || a.Status == ApprovalStatus.InReview),
                EscalatedCount = registrarApprovals.Count(a => a.Status == ApprovalStatus.Escalated),
                LastApprovalAt = registrarApprovals
                    .Where(a => a.Status == ApprovalStatus.Approved)
                    .OrderByDescending(a => a.ReviewedAt)
                    .FirstOrDefault()?.ReviewedAt
            };
            
            var completed = registrarApprovals.Where(a => a.ReviewedAt.HasValue).ToList();
            if (completed.Any())
            {
                stats.AverageProcessingTimeHours = completed
                    .Average(a => (a.ReviewedAt!.Value - a.SubmittedAt).TotalHours);
            }
            
            return Task.FromResult(stats);
        }
    }
    
    /// <inheritdoc />
    public Task<ZoneStatistics> GetZoneStatisticsAsync(
        string zone,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var zoneRegistrars = _registrars.Values
                .Where(r => r.Zone.Equals(zone, StringComparison.OrdinalIgnoreCase))
                .ToList();
                
            var zoneRegistrarIds = zoneRegistrars.Select(r => r.Id).ToHashSet();
            var zoneApprovals = _approvals.Values
                .Where(a => zoneRegistrarIds.Contains(a.RegistrarId))
                .ToList();
            
            var stats = new ZoneStatistics
            {
                Zone = zone,
                TotalRegistrars = zoneRegistrars.Count,
                ActiveRegistrars = zoneRegistrars.Count(r => r.IsActive),
                TotalPendingApprovals = zoneApprovals.Count(a => 
                    a.Status == ApprovalStatus.Pending || a.Status == ApprovalStatus.InReview),
                ApprovedToday = zoneApprovals.Count(a => 
                    a.Status == ApprovalStatus.Approved && 
                    a.ReviewedAt?.Date == DateTime.UtcNow.Date)
            };
            
            var completed = zoneApprovals.Where(a => a.ReviewedAt.HasValue).ToList();
            if (completed.Any())
            {
                stats.AverageProcessingTimeHours = completed
                    .Average(a => (a.ReviewedAt!.Value - a.SubmittedAt).TotalHours);
            }
            
            return Task.FromResult(stats);
        }
    }
    
    #endregion
    
    /// <summary>
    /// Initializes default registrars for demonstration.
    /// </summary>
    private void InitializeDefaultRegistrars()
    {
        var defaultRegistrar = new Models.ClanRegistrar
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            IdentityId = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            DID = "did:futurewampum:mohawk:registrar-1",
            Type = RegistrarType.ClanMatriarch,
            Zone = "mohawk",
            ClanName = "Bear Clan",
            Title = "Clan Mother",
            Scope = new RegistrarScope
            {
                CanApproveRegistrations = true,
                CanIssueCredentials = true,
                CanRevokeCredentials = true,
                CanApproveGuardianship = true,
                CanDelegate = true
            }
        };
        
        _registrars[defaultRegistrar.Id] = defaultRegistrar;
        
        // Add general zone registrar
        var generalRegistrar = new Models.ClanRegistrar
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            IdentityId = Guid.Parse("00000000-0000-0000-0000-000000000002"),
            DID = "did:futurewampum:general:registrar-1",
            Type = RegistrarType.Institutional,
            Zone = "general",
            InstitutionName = "FutureWampum Identity Authority",
            Title = "Identity Administrator"
        };
        
        _registrars[generalRegistrar.Id] = generalRegistrar;
    }
}
