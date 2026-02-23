using System.Runtime.CompilerServices;
using Mamey.CQRS.Commands;
using Mamey.Microservice.Abstractions.Messaging;

[assembly: InternalsVisibleTo("Mamey.Government.Identity.Tests.Integration.Async")]
namespace Mamey.Government.Identity.Contracts.Commands;

[Contract]
public record CreateEmailConfirmation : ICommand
{
    public CreateEmailConfirmation(Guid id, Guid userId, string email, string confirmationCode, DateTime expiresAt, string? ipAddress = null, string? userAgent = null)
    {
        Id = id;
        UserId = userId;
        Email = email;
        ConfirmationCode = confirmationCode;
        ExpiresAt = expiresAt;
        IpAddress = ipAddress;
        UserAgent = userAgent;
    }

    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string ConfirmationCode { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string? IpAddress { get; init; }
    public string? UserAgent { get; init; }
}
