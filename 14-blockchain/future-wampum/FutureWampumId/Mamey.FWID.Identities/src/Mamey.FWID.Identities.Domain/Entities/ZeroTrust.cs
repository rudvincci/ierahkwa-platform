using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a zero-trust security aggregate root for policy-based access control.
/// </summary>
internal class ZeroTrust : AggregateRoot<ZeroTrustId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private ZeroTrust()
    {
        Policies = new List<SecurityPolicy>();
        AccessRequests = new List<AccessRequest>();
        TrustSignals = new List<TrustSignal>();
        IsolationZones = new List<IsolationZone>();
        AuditLog = new List<SecurityEvent>();
    }

    /// <summary>
    /// Initializes a new instance of the ZeroTrust aggregate root.
    /// </summary>
    /// <param name="id">The zero-trust identifier.</param>
    /// <param name="entityId">The entity this policy applies to.</param>
    /// <param name="entityType">The type of entity.</param>
    /// <param name="trustLevel">The initial trust level.</param>
    public ZeroTrust(
        ZeroTrustId id,
        string entityId,
        string entityType,
        TrustLevel trustLevel)
        : base(id)
    {
        EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId));
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        TrustLevel = trustLevel;
        Status = ZeroTrustStatus.Active;
        CreatedAt = DateTime.UtcNow;
        LastAssessmentAt = CreatedAt;
        Policies = new List<SecurityPolicy>();
        AccessRequests = new List<AccessRequest>();
        TrustSignals = new List<TrustSignal>();
        IsolationZones = new List<IsolationZone>();
        AuditLog = new List<SecurityEvent>();
        Version = 1;

        AddEvent(new ZeroTrustPolicyCreated(Id, EntityId, EntityType, TrustLevel, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The entity this zero-trust policy applies to.
    /// </summary>
    public string EntityId { get; private set; }

    /// <summary>
    /// The type of entity.
    /// </summary>
    public string EntityType { get; private set; }

    /// <summary>
    /// The current trust level.
    /// </summary>
    public TrustLevel TrustLevel { get; private set; }

    /// <summary>
    /// The status of the zero-trust policy.
    /// </summary>
    public ZeroTrustStatus Status { get; private set; }

    /// <summary>
    /// When the policy was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the trust level was last assessed.
    /// </summary>
    public DateTime LastAssessmentAt { get; private set; }

    /// <summary>
    /// The risk score (0-100).
    /// </summary>
    public int RiskScore { get; private set; }

    /// <summary>
    /// The isolation zone assignment.
    /// </summary>
    public string? IsolationZone { get; private set; }

    /// <summary>
    /// The security policies applied.
    /// </summary>
    public List<SecurityPolicy> Policies { get; private set; }

    /// <summary>
    /// The access requests made.
    /// </summary>
    public List<AccessRequest> AccessRequests { get; private set; }

    /// <summary>
    /// The trust signals collected.
    /// </summary>
    public List<TrustSignal> TrustSignals { get; private set; }

    /// <summary>
    /// The isolation zones defined.
    /// </summary>
    public List<IsolationZone> IsolationZones { get; private set; }

    /// <summary>
    /// The security audit log.
    /// </summary>
    public List<SecurityEvent> AuditLog { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Adds a security policy.
    /// </summary>
    /// <param name="policyId">The policy identifier.</param>
    /// <param name="policyType">The type of policy.</param>
    /// <param name="rules">The policy rules.</param>
    /// <param name="priority">The policy priority.</param>
    public void AddSecurityPolicy(
        string policyId,
        PolicyType policyType,
        Dictionary<string, object> rules,
        int priority)
    {
        var policy = new SecurityPolicy(
            policyId,
            policyType,
            rules,
            priority,
            DateTime.UtcNow);

        Policies.Add(policy);
        IncrementVersion();

        AddEvent(new SecurityPolicyAdded(Id, policyId, policyType, priority, DateTime.UtcNow));
    }

    /// <summary>
    /// Evaluates an access request.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="userId">The user making the request.</param>
    /// <param name="resource">The resource being accessed.</param>
    /// <param name="action">The action being performed.</param>
    /// <param name="context">The access context.</param>
    /// <returns>The access decision.</returns>
    public AccessDecision EvaluateAccessRequest(
        string requestId,
        string userId,
        string resource,
        string action,
        Dictionary<string, object> context)
    {
        var request = new AccessRequest(
            requestId,
            userId,
            resource,
            action,
            context,
            DateTime.UtcNow);

        AccessRequests.Add(request);

        // Evaluate against all policies
        var decision = EvaluatePolicies(request);

        request.Decision = decision.Result;
        request.DecisionReason = decision.Reason;
        request.EvaluatedAt = DateTime.UtcNow;

        // Update trust level based on access patterns
        UpdateTrustLevelBasedOnAccess(decision);

        IncrementVersion();

        AddEvent(new AccessRequestEvaluated(Id, requestId, decision.Result, decision.Reason, DateTime.UtcNow));

        return decision;
    }

    /// <summary>
    /// Records a trust signal.
    /// </summary>
    /// <param name="signalType">The type of signal.</param>
    /// <param name="value">The signal value.</param>
    /// <param name="confidence">The confidence score.</param>
    /// <param name="source">The source of the signal.</param>
    public void RecordTrustSignal(
        TrustSignalType signalType,
        object value,
        int confidence,
        string source)
    {
        var signal = new TrustSignal(
            signalType,
            value,
            confidence,
            source,
            DateTime.UtcNow);

        TrustSignals.Add(signal);

        // Recalculate trust level
        RecalculateTrustLevel();
        LastAssessmentAt = DateTime.UtcNow;

        IncrementVersion();

        AddEvent(new TrustSignalRecorded(Id, signalType, confidence, source, DateTime.UtcNow));
    }

    /// <summary>
    /// Assigns an isolation zone.
    /// </summary>
    /// <param name="zoneId">The zone identifier.</param>
    /// <param name="zoneName">The zone name.</param>
    /// <param name="securityLevel">The security level of the zone.</param>
    /// <param name="networkSegment">The network segment.</param>
    public void AssignIsolationZone(
        string zoneId,
        string zoneName,
        SecurityLevel securityLevel,
        string networkSegment)
    {
        var zone = new IsolationZone(
            zoneId,
            zoneName,
            securityLevel,
            networkSegment,
            DateTime.UtcNow);

        IsolationZones.Add(zone);
        IsolationZone = zoneId;

        IncrementVersion();

        AddEvent(new IsolationZoneAssigned(Id, zoneId, zoneName, securityLevel, DateTime.UtcNow));
    }

    /// <summary>
    /// Updates the risk score.
    /// </summary>
    /// <param name="newScore">The new risk score (0-100).</param>
    /// <param name="reason">The reason for the change.</param>
    public void UpdateRiskScore(int newScore, string reason)
    {
        if (newScore < 0 || newScore > 100)
            throw new ArgumentException("Risk score must be between 0 and 100", nameof(newScore));

        var oldScore = RiskScore;
        RiskScore = newScore;

        // Adjust trust level based on risk score
        if (newScore > 70)
            TrustLevel = TrustLevel.Low;
        else if (newScore > 30)
            TrustLevel = TrustLevel.Medium;
        else
            TrustLevel = TrustLevel.High;

        LastAssessmentAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new RiskScoreUpdated(Id, oldScore, newScore, reason, LastAssessmentAt));
    }

    /// <summary>
    /// Records a security event.
    /// </summary>
    /// <param name="eventType">The type of security event.</param>
    /// <param name="description">The event description.</param>
    /// <param name="severity">The event severity.</param>
    public void RecordSecurityEvent(
        SecurityEventType eventType,
        string description,
        SecuritySeverity severity)
    {
        var securityEvent = new SecurityEvent(
            eventType,
            description,
            severity,
            DateTime.UtcNow);

        AuditLog.Add(securityEvent);

        // Adjust trust level based on security events
        if (severity == SecuritySeverity.Critical)
            TrustLevel = TrustLevel.Low;
        else if (severity == SecuritySeverity.High && TrustLevel > TrustLevel.Low)
            TrustLevel = TrustLevel - 1;

        IncrementVersion();

        AddEvent(new SecurityEventRecorded(Id, eventType, severity, description, DateTime.UtcNow));
    }

    /// <summary>
    /// Deactivates the zero-trust policy.
    /// </summary>
    /// <param name="reason">The reason for deactivation.</param>
    public void Deactivate(string reason)
    {
        if (Status == ZeroTrustStatus.Inactive)
            return;

        Status = ZeroTrustStatus.Inactive;
        IncrementVersion();

        AddEvent(new ZeroTrustPolicyDeactivated(Id, reason, DateTime.UtcNow));
    }

    #endregion

    #region Private Methods

    private AccessDecision EvaluatePolicies(AccessRequest request)
    {
        // Evaluate all policies in priority order
        var applicablePolicies = Policies
            .OrderByDescending(p => p.Priority)
            .Where(p => IsPolicyApplicable(p, request))
            .ToList();

        foreach (var policy in applicablePolicies)
        {
            var decision = EvaluatePolicy(policy, request);
            if (decision.Result != AccessResult.Undecided)
            {
                return decision;
            }
        }

        // Default deny if no policy applies
        return new AccessDecision(
            AccessResult.Denied,
            "No applicable policy found",
            TrustLevel,
            RiskScore);
    }

    private bool IsPolicyApplicable(SecurityPolicy policy, AccessRequest request)
    {
        // Check if policy applies to this resource/action
        return policy.Rules.TryGetValue("resources", out var resources) &&
               resources is List<string> resourceList &&
               resourceList.Contains(request.Resource);
    }

    private AccessDecision EvaluatePolicy(SecurityPolicy policy, AccessRequest request)
    {
        // Simple policy evaluation logic
        switch (policy.PolicyType)
        {
            case PolicyType.AllowList:
                return EvaluateAllowListPolicy(policy, request);
            case PolicyType.DenyList:
                return EvaluateDenyListPolicy(policy, request);
            case PolicyType.AttributeBased:
                return EvaluateAttributeBasedPolicy(policy, request);
            case PolicyType.RoleBased:
                return EvaluateRoleBasedPolicy(policy, request);
            default:
                return new AccessDecision(
                    AccessResult.Undecided,
                    "Unknown policy type",
                    TrustLevel,
                    RiskScore);
        }
    }

    private AccessDecision EvaluateAllowListPolicy(SecurityPolicy policy, AccessRequest request)
    {
        if (policy.Rules.TryGetValue("allowedUsers", out var allowedUsers) &&
            allowedUsers is List<string> userList &&
            userList.Contains(request.UserId))
        {
            return new AccessDecision(
                AccessResult.Allowed,
                "User is in allow list",
                TrustLevel,
                RiskScore);
        }

        return new AccessDecision(
            AccessResult.Undecided,
            "User not in allow list",
            TrustLevel,
            RiskScore);
    }

    private AccessDecision EvaluateDenyListPolicy(SecurityPolicy policy, AccessRequest request)
    {
        if (policy.Rules.TryGetValue("deniedUsers", out var deniedUsers) &&
            deniedUsers is List<string> userList &&
            userList.Contains(request.UserId))
        {
            return new AccessDecision(
                AccessResult.Denied,
                "User is in deny list",
                TrustLevel,
                RiskScore);
        }

        return new AccessDecision(
            AccessResult.Undecided,
            "User not in deny list",
            TrustLevel,
            RiskScore);
    }

    private AccessDecision EvaluateAttributeBasedPolicy(SecurityPolicy policy, AccessRequest request)
    {
        // Check trust level requirements
        if (policy.Rules.TryGetValue("minTrustLevel", out var minTrust) &&
            minTrust is string minTrustStr &&
            Enum.TryParse<TrustLevel>(minTrustStr, out var requiredTrust))
        {
            if (TrustLevel < requiredTrust)
            {
                return new AccessDecision(
                    AccessResult.Denied,
                    $"Trust level {TrustLevel} below required {requiredTrust}",
                    TrustLevel,
                    RiskScore);
            }
        }

        // Check risk score limits
        if (policy.Rules.TryGetValue("maxRiskScore", out var maxRisk) &&
            maxRisk is int maxRiskInt &&
            RiskScore > maxRiskInt)
        {
            return new AccessDecision(
                AccessResult.Denied,
                $"Risk score {RiskScore} exceeds maximum {maxRiskInt}",
                TrustLevel,
                RiskScore);
            }

        return new AccessDecision(
            AccessResult.Allowed,
            "Attribute-based access granted",
            TrustLevel,
            RiskScore);
    }

    private AccessDecision EvaluateRoleBasedPolicy(SecurityPolicy policy, AccessRequest request)
    {
        // Check if user has required roles
        if (policy.Rules.TryGetValue("requiredRoles", out var requiredRoles) &&
            requiredRoles is List<string> roleList)
        {
            // Note: In a real implementation, this would check user's actual roles
            // For now, assume role checking logic
            return new AccessDecision(
                AccessResult.Undecided,
                "Role-based evaluation requires user role verification",
                TrustLevel,
                RiskScore);
        }

        return new AccessDecision(
            AccessResult.Undecided,
            "No role requirements specified",
            TrustLevel,
            RiskScore);
    }

    private void UpdateTrustLevelBasedOnAccess(AccessDecision decision)
    {
        // Adjust trust level based on access patterns
        if (decision.Result == AccessResult.Denied)
        {
            // Multiple denials might indicate suspicious behavior
            var recentDenials = AccessRequests
                .Where(r => r.Decision == AccessResult.Denied)
                .Count(r => (DateTime.UtcNow - r.RequestedAt) < TimeSpan.FromHours(1));

            if (recentDenials > 5)
            {
                TrustLevel = TrustLevel.Low;
                RecordSecurityEvent(
                    SecurityEventType.SuspiciousActivity,
                    $"Multiple access denials: {recentDenials} in last hour",
                    SecuritySeverity.Medium);
            }
        }
    }

    private void RecalculateTrustLevel()
    {
        // Calculate trust level based on recent signals
        var recentSignals = TrustSignals
            .Where(s => (DateTime.UtcNow - s.RecordedAt) < TimeSpan.FromDays(7))
            .ToList();

        if (!recentSignals.Any())
            return;

        var averageConfidence = recentSignals.Average(s => s.Confidence);

        // Adjust trust level based on signal confidence
        if (averageConfidence > 80)
            TrustLevel = TrustLevel.High;
        else if (averageConfidence > 60)
            TrustLevel = TrustLevel.Medium;
        else
            TrustLevel = TrustLevel.Low;
    }

    #endregion
}

/// <summary>
/// Represents the trust level.
/// </summary>
internal enum TrustLevel
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Represents the status of zero-trust policy.
/// </summary>
internal enum ZeroTrustStatus
{
    Active,
    Inactive,
    Suspended
}

/// <summary>
/// Represents the result of an access decision.
/// </summary>
internal enum AccessResult
{
    Allowed,
    Denied,
    Undecided,
    RequiresApproval
}

/// <summary>
/// Represents the type of security policy.
/// </summary>
internal enum PolicyType
{
    AllowList,
    DenyList,
    AttributeBased,
    RoleBased,
    TimeBased,
    LocationBased
}

/// <summary>
/// Represents the type of trust signal.
/// </summary>
internal enum TrustSignalType
{
    AuthenticationSuccess,
    AuthenticationFailure,
    DeviceFingerprint,
    LocationChange,
    TimeAnomaly,
    BehavioralPattern
}

/// <summary>
/// Represents the security level.
/// </summary>
internal enum SecurityLevel
{
    Public,
    Internal,
    Confidential,
    Restricted,
    Classified
}

/// <summary>
/// Represents the type of security event.
/// </summary>
internal enum SecurityEventType
{
    AccessGranted,
    AccessDenied,
    AuthenticationSuccess,
    AuthenticationFailure,
    SuspiciousActivity,
    PolicyViolation,
    TrustLevelChange
}

/// <summary>
/// Represents the severity of a security event.
/// </summary>
internal enum SecuritySeverity
{
    Low,
    Medium,
    High,
    Critical
}

/// <summary>
/// Represents an access decision.
/// </summary>
internal class AccessDecision
{
    public AccessResult Result { get; set; }
    public string Reason { get; set; }
    public TrustLevel TrustLevel { get; set; }
    public int RiskScore { get; set; }

    public AccessDecision(
        AccessResult result,
        string reason,
        TrustLevel trustLevel,
        int riskScore)
    {
        Result = result;
        Reason = reason;
        TrustLevel = trustLevel;
        RiskScore = riskScore;
    }
}

/// <summary>
/// Represents a security policy.
/// </summary>
internal class SecurityPolicy
{
    public string PolicyId { get; set; }
    public PolicyType PolicyType { get; set; }
    public Dictionary<string, object> Rules { get; set; } = new();
    public int Priority { get; set; }
    public DateTime CreatedAt { get; set; }

    public SecurityPolicy(
        string policyId,
        PolicyType policyType,
        Dictionary<string, object> rules,
        int priority,
        DateTime createdAt)
    {
        PolicyId = policyId;
        PolicyType = policyType;
        Rules = rules;
        Priority = priority;
        CreatedAt = createdAt;
    }
}

/// <summary>
/// Represents an access request.
/// </summary>
internal class AccessRequest
{
    public string RequestId { get; set; }
    public string UserId { get; set; }
    public string Resource { get; set; }
    public string Action { get; set; }
    public Dictionary<string, object> Context { get; set; } = new();
    public DateTime RequestedAt { get; set; }
    public AccessResult? Decision { get; set; }
    public string? DecisionReason { get; set; }
    public DateTime? EvaluatedAt { get; set; }

    public AccessRequest(
        string requestId,
        string userId,
        string resource,
        string action,
        Dictionary<string, object> context,
        DateTime requestedAt)
    {
        RequestId = requestId;
        UserId = userId;
        Resource = resource;
        Action = action;
        Context = context;
        RequestedAt = requestedAt;
    }
}

/// <summary>
/// Represents a trust signal.
/// </summary>
internal class TrustSignal
{
    public TrustSignalType SignalType { get; set; }
    public object Value { get; set; }
    public int Confidence { get; set; }
    public string Source { get; set; }
    public DateTime RecordedAt { get; set; }

    public TrustSignal(
        TrustSignalType signalType,
        object value,
        int confidence,
        string source,
        DateTime recordedAt)
    {
        SignalType = signalType;
        Value = value;
        Confidence = confidence;
        Source = source;
        RecordedAt = recordedAt;
    }
}

/// <summary>
/// Represents an isolation zone.
/// </summary>
internal class IsolationZone
{
    public string ZoneId { get; set; }
    public string ZoneName { get; set; }
    public SecurityLevel SecurityLevel { get; set; }
    public string NetworkSegment { get; set; }
    public DateTime AssignedAt { get; set; }

    public IsolationZone(
        string zoneId,
        string zoneName,
        SecurityLevel securityLevel,
        string networkSegment,
        DateTime assignedAt)
    {
        ZoneId = zoneId;
        ZoneName = zoneName;
        SecurityLevel = securityLevel;
        NetworkSegment = networkSegment;
        AssignedAt = assignedAt;
    }
}

/// <summary>
/// Represents a security event.
/// </summary>
internal class SecurityEvent
{
    public SecurityEventType EventType { get; set; }
    public string Description { get; set; }
    public SecuritySeverity Severity { get; set; }
    public DateTime OccurredAt { get; set; }

    public SecurityEvent(
        SecurityEventType eventType,
        string description,
        SecuritySeverity severity,
        DateTime occurredAt)
    {
        EventType = eventType;
        Description = description;
        Severity = severity;
        OccurredAt = occurredAt;
    }
}
