using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Infrastructure.Mongo.Documents;

internal class MultiFactorAuthDocument : IIdentifiable<Guid>
{
    public MultiFactorAuthDocument()
    {
    }

    public MultiFactorAuthDocument(MultiFactorAuth multiFactorAuth)
    {
        if (multiFactorAuth is null)
        {
            throw new NullReferenceException();
        }

        Id = multiFactorAuth.Id.Value;
        UserId = multiFactorAuth.UserId.Value;
        CreatedAt = multiFactorAuth.CreatedAt.ToUnixTimeMilliseconds();
        ModifiedAt = multiFactorAuth.ModifiedAt?.ToUnixTimeMilliseconds();
        ActivatedAt = multiFactorAuth.ActivatedAt?.ToUnixTimeMilliseconds();
        Status = multiFactorAuth.Status.ToString();
        EnabledMethods = multiFactorAuth.EnabledMethods.Select(m => m.ToString()).ToList();
        RequiredMethods = multiFactorAuth.RequiredMethods;
        LastUsedAt = multiFactorAuth.LastUsedAt?.ToUnixTimeMilliseconds();
        UsageCount = multiFactorAuth.UsageCount;
        FailedAttempts = multiFactorAuth.FailedAttempts;
        LastFailedAt = multiFactorAuth.LastFailedAt?.ToUnixTimeMilliseconds();
        Version = multiFactorAuth.Version;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public long CreatedAt { get; set; }
    public long? ModifiedAt { get; set; }
    public long? ActivatedAt { get; set; }
    public string Status { get; set; }
    public List<string> EnabledMethods { get; set; } = new();
    public int RequiredMethods { get; set; }
    public long? LastUsedAt { get; set; }
    public int UsageCount { get; set; }
    public int FailedAttempts { get; set; }
    public long? LastFailedAt { get; set; }
    public int Version { get; set; }

    public MultiFactorAuth AsEntity()
        => new MultiFactorAuth(
            Id,
            UserId,
            CreatedAt.GetDate(),
            EnabledMethods.Select(m => m.ToEnum<MfaMethod>()),
            Status.ToEnum<MultiFactorAuthStatus>(),
            Version);

    public MultiFactorAuthDto AsDto()
        => new MultiFactorAuthDto(
            Id,
            UserId,
            EnabledMethods,
            RequiredMethods,
            Status,
            CreatedAt.GetDate(),
            ActivatedAt?.GetDate());
}

