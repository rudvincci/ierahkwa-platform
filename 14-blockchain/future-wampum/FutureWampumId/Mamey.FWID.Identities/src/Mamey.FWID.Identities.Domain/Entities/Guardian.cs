using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a Guardian aggregate root for managing dependent access delegation.
/// </summary>
internal class Guardian : AggregateRoot<GuardianId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private Guardian()
    {
        Delegations = new List<Delegation>();
    }

    /// <summary>
    /// Initializes a new instance of the Guardian aggregate root.
    /// </summary>
    /// <param name="id">The guardian identifier.</param>
    /// <param name="guardianIdentityId">The identity ID of the guardian.</param>
    /// <param name="dependentIdentityId">The identity ID of the dependent.</param>
    /// <param name="relationshipType">The type of relationship (parent, legal_guardian, etc.).</param>
    public Guardian(
        GuardianId id,
        IdentityId guardianIdentityId,
        IdentityId dependentIdentityId,
        string relationshipType)
        : base(id)
    {
        GuardianIdentityId = guardianIdentityId ?? throw new ArgumentNullException(nameof(guardianIdentityId));
        DependentIdentityId = dependentIdentityId ?? throw new ArgumentNullException(nameof(dependentIdentityId));
        RelationshipType = relationshipType ?? throw new ArgumentNullException(nameof(relationshipType));
        Status = GuardianStatus.Active;
        CreatedAt = DateTime.UtcNow;
        Delegations = new List<Delegation>();
        Version = 1;

        AddEvent(new GuardianCreated(Id, GuardianIdentityId, DependentIdentityId, RelationshipType, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The identity ID of the guardian.
    /// </summary>
    public IdentityId GuardianIdentityId { get; private set; }

    /// <summary>
    /// The identity ID of the dependent.
    /// </summary>
    public IdentityId DependentIdentityId { get; private set; }

    /// <summary>
    /// The type of relationship between guardian and dependent.
    /// </summary>
    public string RelationshipType { get; private set; }

    /// <summary>
    /// The status of the guardian relationship.
    /// </summary>
    public GuardianStatus Status { get; private set; }

    /// <summary>
    /// Date and time the guardian relationship was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Date and time the guardian relationship was terminated.
    /// </summary>
    public DateTime? TerminatedAt { get; private set; }

    /// <summary>
    /// The reason for termination.
    /// </summary>
    public string? TerminationReason { get; private set; }

    /// <summary>
    /// The delegations granted by this guardian.
    /// </summary>
    public List<Delegation> Delegations { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Terminates the guardian relationship.
    /// </summary>
    /// <param name="reason">The reason for termination.</param>
    public void Terminate(string reason)
    {
        if (Status == GuardianStatus.Terminated)
            return;

        Status = GuardianStatus.Terminated;
        TerminatedAt = DateTime.UtcNow;
        TerminationReason = reason;
        IncrementVersion();

        AddEvent(new GuardianTerminated(Id, reason, TerminatedAt.Value));
    }

    /// <summary>
    /// Grants a delegation to the dependent.
    /// </summary>
    /// <param name="delegationId">The delegation identifier.</param>
    /// <param name="scope">The scope being delegated.</param>
    /// <param name="expiresAt">Optional expiration date.</param>
    /// <param name="conditions">Optional conditions for the delegation.</param>
    public void GrantDelegation(
        DelegationId delegationId,
        string scope,
        DateTime? expiresAt = null,
        string? conditions = null)
    {
        if (Status != GuardianStatus.Active)
            throw new InvalidOperationException("Cannot grant delegations for inactive guardians");

        var delegation = new Delegation(
            delegationId,
            scope,
            expiresAt,
            conditions,
            DateTime.UtcNow);

        Delegations.Add(delegation);
        IncrementVersion();

        AddEvent(new DelegationGranted(Id, delegationId, scope, expiresAt, conditions));
    }

    /// <summary>
    /// Revokes a delegation.
    /// </summary>
    /// <param name="delegationId">The delegation identifier to revoke.</param>
    /// <param name="reason">The reason for revocation.</param>
    public void RevokeDelegation(DelegationId delegationId, string reason)
    {
        var delegation = Delegations.FirstOrDefault(d => d.Id == delegationId);
        if (delegation == null)
            throw new InvalidOperationException("Delegation not found");

        delegation.Revoke(reason);
        IncrementVersion();

        AddEvent(new DelegationRevoked(Id, delegationId, reason));
    }

    /// <summary>
    /// Checks if the guardian can act on behalf of the dependent for a given scope.
    /// </summary>
    /// <param name="scope">The scope to check.</param>
    /// <returns>True if the guardian has active delegation for the scope.</returns>
    public bool CanActForScope(string scope)
    {
        return Status == GuardianStatus.Active &&
               Delegations.Any(d => d.Scope == scope &&
                                  d.Status == DelegationStatus.Active &&
                                  (d.ExpiresAt == null || d.ExpiresAt > DateTime.UtcNow));
    }

    #endregion
}

/// <summary>
/// Represents the status of a guardian relationship.
/// </summary>
internal enum GuardianStatus
{
    Active,
    Suspended,
    Terminated
}

/// <summary>
/// Represents a delegation granted by a guardian.
/// </summary>
internal class Delegation : IEquatable<Delegation>
{
    public DelegationId Id { get; private set; }
    public string Scope { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? Conditions { get; private set; }
    public DateTime GrantedAt { get; private set; }
    public DelegationStatus Status { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevocationReason { get; private set; }

    public Delegation(
        DelegationId id,
        string scope,
        DateTime? expiresAt,
        string? conditions,
        DateTime grantedAt)
    {
        Id = id;
        Scope = scope;
        ExpiresAt = expiresAt;
        Conditions = conditions;
        GrantedAt = grantedAt;
        Status = DelegationStatus.Active;
    }

    public void Revoke(string reason)
    {
        if (Status == DelegationStatus.Revoked)
            return;

        Status = DelegationStatus.Revoked;
        RevokedAt = DateTime.UtcNow;
        RevocationReason = reason;
    }

    public bool Equals(Delegation? other)
    {
        return other != null && Id.Equals(other.Id);
    }

    public override bool Equals(object? obj) => Equals(obj as Delegation);
    public override int GetHashCode() => Id.GetHashCode();
}

/// <summary>
/// Represents the status of a delegation.
/// </summary>
internal enum DelegationStatus
{
    Active,
    Revoked,
    Expired
}
