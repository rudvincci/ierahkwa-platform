using Mamey.Government.Modules.Citizens.Core.Domain.Events;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Types;

namespace Mamey.Government.Modules.Citizens.Core.Domain.Entities;

/// <summary>
/// Citizen aggregate root - represents a citizen record.
/// </summary>
public class Citizen : AggregateRoot<CitizenId>
{
    private readonly List<CitizenshipStatusHistory> _statusHistory = new();

    private Citizen() { }

    public Citizen(
        CitizenId id,
        TenantId tenantId,
        Name citizenName,
        Email? email = null,
        Phone? phone = null,
        Address? address = null,
        DateTime? dateOfBirth = null,
        CitizenshipStatus status = CitizenshipStatus.Probationary,
        Guid applicationId = default,
        int version = 0)
        : base(id, version)
    {
        TenantId = tenantId;
        CitizenName = citizenName;
        Email = email;
        Phone = phone;
        Address = address;
        DateOfBirth = dateOfBirth;
        Status = status;
        ApplicationId = applicationId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        
        // Add initial status to history
        _statusHistory.Add(new CitizenshipStatusHistory(status, DateTime.UtcNow, "Initial status"));
        
        AddEvent(new CitizenCreated(Id, TenantId, CitizenName, Status, applicationId));
    }

    public TenantId TenantId { get; private set; }
    public Name CitizenName { get; private set; } = null!;
    public Email? Email { get; private set; }
    public Phone? Phone { get; private set; }
    public Address? Address { get; private set; }
    public DateTime? DateOfBirth { get; private set; }
    public CitizenshipStatus Status { get; private set; }
    public string? PhotoPath { get; private set; } // MinIO path
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid ApplicationId { get; private set; }
    public IReadOnlyList<CitizenshipStatusHistory> StatusHistory => _statusHistory.AsReadOnly();
    
    // Convenience properties
    public string FirstName => CitizenName.FirstName;
    public string LastName => CitizenName.LastName;
    public string FullName => CitizenName.FullName;

    public void UpdatePersonalDetails(Name citizenName, DateTime? dateOfBirth)
    {
        CitizenName = citizenName;
        DateOfBirth = dateOfBirth;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new CitizenModified(this));
    }

    public void UpdateContact(Email? email, Phone? phone, Address? address)
    {
        Email = email;
        Phone = phone;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new CitizenModified(this));
    }

    public void UpdatePhoto(string photoPath)
    {
        PhotoPath = photoPath;
        UpdatedAt = DateTime.UtcNow;
        
        AddEvent(new CitizenModified(this));
    }

    public void ChangeStatus(CitizenshipStatus newStatus, string? reason = null)
    {
        if (Status == newStatus) return;
        
        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;
        
        _statusHistory.Add(new CitizenshipStatusHistory(newStatus, DateTime.UtcNow, reason ?? $"Status changed from {oldStatus}"));
        
        AddEvent(new CitizenStatusChanged(Id, oldStatus, newStatus, reason));
    }

    public void Deactivate(string reason)
    {
        UpdatedAt = DateTime.UtcNow;
        AddEvent(new CitizenDeactivated(Id, reason));
    }
}

/// <summary>
/// Status history entry for tracking citizenship status changes.
/// </summary>
public class CitizenshipStatusHistory
{
    public CitizenshipStatusHistory(
        CitizenshipStatus status,
        DateTime changedAt,
        string? reason = null)
    {
        Status = status;
        ChangedAt = changedAt;
        Reason = reason;
    }

    public CitizenshipStatus Status { get; }
    public DateTime ChangedAt { get; }
    public string? Reason { get; }
}
