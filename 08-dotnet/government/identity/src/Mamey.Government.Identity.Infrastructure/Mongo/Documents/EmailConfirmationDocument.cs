using Mamey.Government.Identity.Contracts.DTO;
using Mamey.Government.Identity.Domain.Entities;
using Mamey.Types;
using Mamey;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Infrastructure.Mongo.Documents;

internal class EmailConfirmationDocument : IIdentifiable<Guid>
{
    public EmailConfirmationDocument()
    {
    }

    public EmailConfirmationDocument(EmailConfirmation emailConfirmation)
    {
        if (emailConfirmation is null)
        {
            throw new NullReferenceException();
        }

        Id = emailConfirmation.Id.Value;
        UserId = emailConfirmation.UserId.Value;
        Email = emailConfirmation.Email;
        ConfirmationCode = emailConfirmation.ConfirmationCode;
        CreatedAt = emailConfirmation.CreatedAt.ToUnixTimeMilliseconds();
        ExpiresAt = emailConfirmation.ExpiresAt.ToUnixTimeMilliseconds();
        Status = emailConfirmation.Status.ToString();
        ConfirmedAt = emailConfirmation.ConfirmedAt?.ToUnixTimeMilliseconds();
        IpAddress = emailConfirmation.IpAddress;
        UserAgent = emailConfirmation.UserAgent;
        AttemptCount = emailConfirmation.AttemptCount;
        Version = emailConfirmation.Version;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; }
    public string ConfirmationCode { get; set; }
    public long CreatedAt { get; set; }
    public long ExpiresAt { get; set; }
    public string Status { get; set; }
    public long? ConfirmedAt { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public int AttemptCount { get; set; }
    public int Version { get; set; }

    public EmailConfirmation AsEntity()
        => new EmailConfirmation(
            Id,
            UserId,
            Email,
            ConfirmationCode,
            CreatedAt.GetDate(),
            ExpiresAt.GetDate(),
            Status.ToEnum<EmailConfirmationStatus>(),
            ConfirmedAt?.GetDate(),
            IpAddress,
            UserAgent,
            Version);

    public EmailConfirmationDto AsDto()
        => new EmailConfirmationDto(
            Id,
            UserId,
            Email,
            ConfirmationCode,
            ExpiresAt.GetDate(),
            IpAddress,
            UserAgent,
            Status,
            CreatedAt.GetDate(),
            ConfirmedAt?.GetDate());
}

