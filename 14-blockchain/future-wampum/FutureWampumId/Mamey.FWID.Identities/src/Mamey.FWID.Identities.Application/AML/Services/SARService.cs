using System.Text;
using Mamey.FWID.Identities.Application.AML.Models;
using Mamey.MessageBrokers;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.AML.Services;

/// <summary>
/// SAR service implementation.
/// </summary>
public class SARService : ISARService
{
    private readonly ILogger<SARService> _logger;
    private readonly IBusPublisher _publisher;
    
    // In-memory storage
    private readonly Dictionary<Guid, SuspiciousActivityReport> _sars = new();
    private readonly Dictionary<string, Guid> _referenceToId = new();
    private readonly object _lock = new();
    
    private static int _sarCounter = 1000;
    private const int FilingDeadlineDays = 30;
    
    public SARService(
        ILogger<SARService> logger,
        IBusPublisher publisher)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
    }
    
    /// <inheritdoc />
    public async Task<SuspiciousActivityReport> CreateSARAsync(
        CreateSARRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating SAR for identity {IdentityId}, Trigger: {Trigger}",
            request.SubjectIdentityId, request.Trigger);
        
        var referenceNumber = GenerateReferenceNumber();
        
        var sar = new SuspiciousActivityReport
        {
            ReferenceNumber = referenceNumber,
            SubjectIdentityId = request.SubjectIdentityId,
            SubjectName = request.SubjectName,
            SubjectDID = request.SubjectDID,
            Trigger = request.Trigger,
            Type = request.Type,
            Priority = request.Priority,
            Narrative = request.Narrative,
            ActivityDetectedAt = request.ActivityDetectedAt,
            CreatedBy = request.CreatedBy,
            Evidence = request.InitialEvidence ?? new(),
            RelatedTransactions = request.RelatedTransactions ?? new(),
            DueDate = DateTime.UtcNow.AddDays(FilingDeadlineDays),
            Status = SARStatus.Draft
        };
        
        sar.WorkflowHistory.Add(new SARWorkflowStep
        {
            FromStatus = SARStatus.Draft,
            ToStatus = SARStatus.Draft,
            Timestamp = DateTime.UtcNow,
            PerformedBy = request.CreatedBy,
            Comments = "SAR created"
        });
        
        lock (_lock)
        {
            _sars[sar.Id] = sar;
            _referenceToId[referenceNumber] = sar.Id;
        }
        
        await _publisher.PublishAsync(new SARCreatedEvent
        {
            SARId = sar.Id,
            ReferenceNumber = sar.ReferenceNumber,
            SubjectIdentityId = sar.SubjectIdentityId,
            Trigger = sar.Trigger,
            Priority = sar.Priority,
            CreatedAt = sar.CreatedAt
        });
        
        _logger.LogInformation("SAR created: {ReferenceNumber} ({SARId})", referenceNumber, sar.Id);
        
        return sar;
    }
    
    /// <inheritdoc />
    public Task<SuspiciousActivityReport?> GetSARAsync(Guid sarId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            _sars.TryGetValue(sarId, out var sar);
            return Task.FromResult(sar);
        }
    }
    
    /// <inheritdoc />
    public Task<SuspiciousActivityReport?> GetSARByReferenceAsync(string referenceNumber, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_referenceToId.TryGetValue(referenceNumber, out var sarId))
            {
                _sars.TryGetValue(sarId, out var sar);
                return Task.FromResult(sar);
            }
            return Task.FromResult<SuspiciousActivityReport?>(null);
        }
    }
    
    /// <inheritdoc />
    public async Task<SuspiciousActivityReport> UpdateStatusAsync(
        Guid sarId,
        SARStatus newStatus,
        Guid performedBy,
        string? comments = null,
        CancellationToken cancellationToken = default)
    {
        var sar = await GetSARAsync(sarId, cancellationToken)
            ?? throw new InvalidOperationException($"SAR {sarId} not found");
        
        var previousStatus = sar.Status;
        sar.Status = newStatus;
        
        sar.WorkflowHistory.Add(new SARWorkflowStep
        {
            FromStatus = previousStatus,
            ToStatus = newStatus,
            Timestamp = DateTime.UtcNow,
            PerformedBy = performedBy,
            Comments = comments
        });
        
        _logger.LogInformation("SAR {ReferenceNumber} status updated: {From} -> {To}",
            sar.ReferenceNumber, previousStatus, newStatus);
        
        return sar;
    }
    
    /// <inheritdoc />
    public Task<bool> AssignSARAsync(Guid sarId, Guid assigneeId, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_sars.TryGetValue(sarId, out var sar))
                return Task.FromResult(false);
            
            sar.AssignedTo = assigneeId;
            return Task.FromResult(true);
        }
    }
    
    /// <inheritdoc />
    public Task<bool> AddEvidenceAsync(Guid sarId, SAREvidence evidence, CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (!_sars.TryGetValue(sarId, out var sar))
                return Task.FromResult(false);
            
            sar.Evidence.Add(evidence);
            return Task.FromResult(true);
        }
    }
    
    /// <inheritdoc />
    public Task<SuspiciousActivityReport> SubmitForApprovalAsync(
        Guid sarId,
        Guid submittedBy,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatusAsync(sarId, SARStatus.ApprovalRequired, submittedBy,
            "Submitted for supervisor approval", cancellationToken);
    }
    
    /// <inheritdoc />
    public Task<SuspiciousActivityReport> ApproveSARAsync(
        Guid sarId,
        Guid approvedBy,
        string? comments = null,
        CancellationToken cancellationToken = default)
    {
        return UpdateStatusAsync(sarId, SARStatus.Approved, approvedBy,
            comments ?? "Approved for filing", cancellationToken);
    }
    
    /// <inheritdoc />
    public async Task<SARFilingResult> FileSARAsync(
        Guid sarId,
        Guid filedBy,
        CancellationToken cancellationToken = default)
    {
        var sar = await GetSARAsync(sarId, cancellationToken);
        if (sar == null)
        {
            return new SARFilingResult
            {
                SARId = sarId,
                Success = false,
                ErrorMessage = "SAR not found"
            };
        }
        
        if (sar.Status != SARStatus.Approved)
        {
            return new SARFilingResult
            {
                SARId = sarId,
                Success = false,
                ErrorMessage = "SAR must be approved before filing"
            };
        }
        
        _logger.LogInformation("Filing SAR {ReferenceNumber} with regulatory body", sar.ReferenceNumber);
        
        // Simulate filing with regulatory body
        var confirmationNumber = $"SCOB-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(10000, 99999)}";
        
        sar.FiledAt = DateTime.UtcNow;
        sar.FilingConfirmation = confirmationNumber;
        sar.FiledWith = "SCOB (Sovereign Community Oversight Board)";
        sar.DeadlineMet = sar.FiledAt <= sar.DueDate;
        
        await UpdateStatusAsync(sarId, SARStatus.Filed, filedBy,
            $"Filed with {sar.FiledWith}. Confirmation: {confirmationNumber}", cancellationToken);
        
        await _publisher.PublishAsync(new SARFiledEvent
        {
            SARId = sar.Id,
            ReferenceNumber = sar.ReferenceNumber,
            ConfirmationNumber = confirmationNumber,
            FiledWith = sar.FiledWith,
            FiledAt = sar.FiledAt.Value,
            DeadlineMet = sar.DeadlineMet.Value
        });
        
        return new SARFilingResult
        {
            SARId = sarId,
            Success = true,
            ConfirmationNumber = confirmationNumber,
            RegulatoryBody = sar.FiledWith,
            FiledAt = sar.FiledAt
        };
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<SuspiciousActivityReport>> GetPendingSARsAsync(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var pending = _sars.Values
                .Where(s => s.Status is SARStatus.Draft or SARStatus.PendingReview or SARStatus.UnderReview or SARStatus.ApprovalRequired)
                .OrderBy(s => s.DueDate)
                .ToList();
            return Task.FromResult<IReadOnlyList<SuspiciousActivityReport>>(pending);
        }
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<SuspiciousActivityReport>> GetSARsApproachingDeadlineAsync(
        int daysUntilDeadline = 3,
        CancellationToken cancellationToken = default)
    {
        var cutoff = DateTime.UtcNow.AddDays(daysUntilDeadline);
        
        lock (_lock)
        {
            var approaching = _sars.Values
                .Where(s => s.Status != SARStatus.Filed && s.Status != SARStatus.Closed && s.DueDate <= cutoff)
                .OrderBy(s => s.DueDate)
                .ToList();
            return Task.FromResult<IReadOnlyList<SuspiciousActivityReport>>(approaching);
        }
    }
    
    /// <inheritdoc />
    public Task<IReadOnlyList<SuspiciousActivityReport>> GetSARsForIdentityAsync(
        Guid identityId,
        CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            var sars = _sars.Values
                .Where(s => s.SubjectIdentityId == identityId)
                .OrderByDescending(s => s.CreatedAt)
                .ToList();
            return Task.FromResult<IReadOnlyList<SuspiciousActivityReport>>(sars);
        }
    }
    
    /// <inheritdoc />
    public async Task<byte[]> GenerateSARReportAsync(Guid sarId, CancellationToken cancellationToken = default)
    {
        var sar = await GetSARAsync(sarId, cancellationToken)
            ?? throw new InvalidOperationException($"SAR {sarId} not found");
        
        // Generate simple text report (in production, use proper PDF generation)
        var sb = new StringBuilder();
        sb.AppendLine("=".PadRight(60, '='));
        sb.AppendLine("SUSPICIOUS ACTIVITY REPORT");
        sb.AppendLine("=".PadRight(60, '='));
        sb.AppendLine();
        sb.AppendLine($"Reference Number: {sar.ReferenceNumber}");
        sb.AppendLine($"Status: {sar.Status}");
        sb.AppendLine($"Priority: {sar.Priority}");
        sb.AppendLine($"Created: {sar.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"Due Date: {sar.DueDate:yyyy-MM-dd}");
        sb.AppendLine();
        sb.AppendLine("-".PadRight(60, '-'));
        sb.AppendLine("SUBJECT INFORMATION");
        sb.AppendLine("-".PadRight(60, '-'));
        sb.AppendLine($"Name: {sar.SubjectName}");
        sb.AppendLine($"Identity ID: {sar.SubjectIdentityId}");
        sb.AppendLine($"DID: {sar.SubjectDID ?? "N/A"}");
        sb.AppendLine();
        sb.AppendLine("-".PadRight(60, '-'));
        sb.AppendLine("ACTIVITY DETAILS");
        sb.AppendLine("-".PadRight(60, '-'));
        sb.AppendLine($"Trigger: {sar.Trigger}");
        sb.AppendLine($"Type: {sar.Type}");
        sb.AppendLine($"Activity Detected: {sar.ActivityDetectedAt:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine();
        sb.AppendLine("Narrative:");
        sb.AppendLine(sar.Narrative);
        sb.AppendLine();
        
        if (sar.FiledAt.HasValue)
        {
            sb.AppendLine("-".PadRight(60, '-'));
            sb.AppendLine("FILING INFORMATION");
            sb.AppendLine("-".PadRight(60, '-'));
            sb.AppendLine($"Filed: {sar.FiledAt:yyyy-MM-dd HH:mm:ss} UTC");
            sb.AppendLine($"Filed With: {sar.FiledWith}");
            sb.AppendLine($"Confirmation: {sar.FilingConfirmation}");
            sb.AppendLine($"Deadline Met: {(sar.DeadlineMet == true ? "Yes" : "No")}");
        }
        
        return Encoding.UTF8.GetBytes(sb.ToString());
    }
    
    #region Private Methods
    
    private static string GenerateReferenceNumber()
    {
        var counter = Interlocked.Increment(ref _sarCounter);
        return $"SAR-{DateTime.UtcNow:yyyyMMdd}-{counter:D5}";
    }
    
    #endregion
}

/// <summary>
/// Event when SAR is created.
/// </summary>
public record SARCreatedEvent
{
    public Guid SARId { get; init; }
    public string ReferenceNumber { get; init; } = null!;
    public Guid SubjectIdentityId { get; init; }
    public SARTrigger Trigger { get; init; }
    public SARPriority Priority { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Event when SAR is filed.
/// </summary>
public record SARFiledEvent
{
    public Guid SARId { get; init; }
    public string ReferenceNumber { get; init; } = null!;
    public string ConfirmationNumber { get; init; } = null!;
    public string FiledWith { get; init; } = null!;
    public DateTime FiledAt { get; init; }
    public bool DeadlineMet { get; init; }
}
