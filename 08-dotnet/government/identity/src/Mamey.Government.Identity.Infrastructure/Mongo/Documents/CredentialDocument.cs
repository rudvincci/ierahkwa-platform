using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Infrastructure.Mongo.Documents;

internal class CredentialDocument : IIdentifiable<Guid>
{
    public CredentialDocument()
    {
    }

    public CredentialDocument(Credential credential)
    {
        if (credential is null)
        {
            throw new NullReferenceException();
        }

        Id = credential.Id.Value;
        UserId = credential.UserId.Value;
        Type = credential.Type.ToString();
        Value = credential.Value;
        CreatedAt = credential.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = credential.ModifiedAt?.ToUnixTimeMilliseconds();
        ExpiresAt = credential.ExpiresAt?.ToUnixTimeMilliseconds();
        Status = credential.Status.ToString();
        LastUsedAt = credential.LastUsedAt?.ToUnixTimeMilliseconds();
        UsageCount = credential.UsageCount;
        Version = credential.Version;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public long? ExpiresAt { get; set; }
    public string Status { get; set; }
    public long? LastUsedAt { get; set; }
    public int UsageCount { get; set; }
    public int Version { get; set; }

    public Credential AsEntity()
        => new Credential(
            Id,
            UserId,
            Type.ToEnum<CredentialType>(),
            Value,
            CreatedAt.GetDate(),
            ModifiedAt?.GetDate(),
            ExpiresAt?.GetDate(),
            Status.ToEnum<CredentialStatus>(),
            Version);
}

