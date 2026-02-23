using Mamey.CQRS.Events;
using Mamey.MessageBrokers;
using Mamey.Microservice.Abstractions.Messaging;

namespace Mamey.FWID.Identities.Application.Events.Integration.Auth;

/// <summary>
/// Integration event raised when an identity signs in.
/// </summary>
[Contract]
[Message("auth.sign-in")]
internal record SignInIntegrationEvent(
    Guid IdentityId,
    Guid SessionId,
    string AuthenticationMethod,
    string? IpAddress,
    string? UserAgent,
    DateTime SignedInAt) : IEvent;

