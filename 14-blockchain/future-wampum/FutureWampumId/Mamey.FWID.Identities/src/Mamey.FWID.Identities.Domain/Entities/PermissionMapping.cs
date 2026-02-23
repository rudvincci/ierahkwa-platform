using Mamey.Persistence.SQL;

namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Domain entity representing a permission mapping for a service.
/// </summary>
internal class PermissionMapping : EFEntity<Guid>
{
    /// <summary>
    /// Gets or sets the service name (e.g., "dids-service").
    /// </summary>
    public string ServiceName { get; private set; } = null!;

    /// <summary>
    /// Gets or sets the list of permissions granted to this service.
    /// </summary>
    public List<string> Permissions { get; private set; } = new();

    /// <summary>
    /// Gets or sets the certificate subject (CN) for this service.
    /// </summary>
    public string? CertificateSubject { get; private set; }

    /// <summary>
    /// Gets or sets the certificate issuer for this service.
    /// </summary>
    public string? CertificateIssuer { get; private set; }

    /// <summary>
    /// Gets or sets the certificate thumbprint for this service.
    /// </summary>
    public string? CertificateThumbprint { get; private set; }

    /// <summary>
    /// Gets or sets when this mapping was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Gets or sets when this mapping was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Gets or sets whether this mapping is active.
    /// </summary>
    public bool IsActive { get; private set; }

    private PermissionMapping() : base(Guid.Empty) { } // For EF Core

    public PermissionMapping(
        Guid id,
        string serviceName,
        List<string> permissions,
        string? certificateSubject = null,
        string? certificateIssuer = null,
        string? certificateThumbprint = null)
        : base(id)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
            throw new ArgumentException("Service name cannot be null or empty", nameof(serviceName));

        if (permissions == null || !permissions.Any())
            throw new ArgumentException("Permissions cannot be null or empty", nameof(permissions));

        ServiceName = serviceName;
        Permissions = permissions;
        CertificateSubject = certificateSubject;
        CertificateIssuer = certificateIssuer;
        CertificateThumbprint = certificateThumbprint;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void UpdatePermissions(List<string> permissions)
    {
        if (permissions == null || !permissions.Any())
            throw new ArgumentException("Permissions cannot be null or empty", nameof(permissions));

        Permissions = permissions;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateCertificateInfo(string? certificateSubject, string? certificateIssuer, string? certificateThumbprint)
    {
        CertificateSubject = certificateSubject;
        CertificateIssuer = certificateIssuer;
        CertificateThumbprint = certificateThumbprint;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}

