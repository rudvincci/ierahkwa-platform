using Mamey.FWID.Identities.Contracts.DTOs;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Integration.Async")]
namespace Mamey.FWID.Identities.Infrastructure.Mongo.Documents;

/// <summary>
/// MongoDB document for Identity read model.
/// This is the read model that gets synced from PostgreSQL.
/// </summary>
internal class IdentityDocument : IIdentifiable<Guid>
{
    public IdentityDocument()
    {
    }

    public IdentityDocument(Identity identity)
    {
        if (identity is null)
        {
            throw new NullReferenceException();
        }

        Id = identity.Id.Value;
        FirstName = identity.Name.FirstName;
        LastName = identity.Name.LastName;
        Status = identity.Status.ToString();
        Zone = identity.Zone;
        CreatedAt = identity.CreatedAt.ToUnixTimeMilliseconds();
        VerifiedAt = identity.VerifiedAt?.ToUnixTimeMilliseconds();
        LastVerifiedAt = identity.LastVerifiedAt?.ToUnixTimeMilliseconds();
        Version = identity.Version;
        AzureUserId = identity.AzureUserId;
        ServiceId = identity.ServiceId;
    }

    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Zone { get; set; }
    public long CreatedAt { get; set; }
    public long? VerifiedAt { get; set; }
    public long? LastVerifiedAt { get; set; }
    public int Version { get; set; }
    public string? AzureUserId { get; set; }
    public string? ServiceId { get; set; }

    // Note: AsEntity() creates a minimal entity for caching purposes
    // Full entity data should come from PostgreSQL (source of truth)
    // This is used by the composite repository pattern for read caching
    // 
    // Design Decision: IdentityDocument is intentionally a minimal read model for performance.
    // It only contains frequently queried fields (Name, Status, Zone, timestamps).
    // Full entity reconstruction is not implemented because:
    // 1. PostgreSQL is the source of truth for complete entity data
    // 2. Composite repository pattern automatically falls back to PostgreSQL when full data is needed
    // 3. This keeps MongoDB read model lightweight and fast for common queries
    // 
    // If full entity reconstruction is needed in the future, it should:
    // - Query PostgreSQL repository for complete data
    // - Or extend IdentityDocument to include all fields (with performance trade-offs)
    public Identity AsEntity()
    {
        throw new InvalidOperationException(
            "MongoDB read model cannot be converted back to full entity. " +
            "Use PostgreSQL repository for entity retrieval. " +
            "This is by design - IdentityDocument is a minimal read model for performance.");
    }

    public IdentityDto AsDto()
        => new IdentityDto
        {
            Id = new IdentityId(Id),
            Name = new Name(FirstName, LastName),
            Status = (Contracts.IdentityStatus)(int)Status.ToEnum<Domain.ValueObjects.IdentityStatus>(),
            Zone = Zone,
            CreatedAt = CreatedAt.GetDate(),
            VerifiedAt = VerifiedAt?.GetDate(),
            LastVerifiedAt = LastVerifiedAt?.GetDate()
        };
}

