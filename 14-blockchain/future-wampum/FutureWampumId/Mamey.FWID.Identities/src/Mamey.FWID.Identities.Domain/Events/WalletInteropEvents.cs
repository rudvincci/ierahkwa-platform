using Mamey.CQRS;
using Mamey.FWID.Identities.Domain.Entities;

namespace Mamey.FWID.Identities.Domain.Events;

/// <summary>
/// Event raised when a wallet connection is initiated.
/// </summary>
internal record WalletConnectionInitiated(
    WalletInteropId WalletInteropId,
    IdentityId IdentityId,
    string WalletDid,
    WalletType WalletType,
    DateTime InitiatedAt) : IDomainEvent;

/// <summary>
/// Event raised when a DIDComm connection is established.
/// </summary>
internal record DIDCommConnectionEstablished(
    WalletInteropId WalletInteropId,
    string ConnectionId,
    string Endpoint,
    List<string> Protocols,
    DateTime EstablishedAt) : IDomainEvent;

/// <summary>
/// Event raised when OpenID4VCI is configured.
/// </summary>
internal record OpenID4VCIConfigured(
    WalletInteropId WalletInteropId,
    string IssuerEndpoint,
    List<string> SupportedCredentialTypes,
    DateTime ConfiguredAt) : IDomainEvent;

/// <summary>
/// Event raised when a credential offer is sent.
/// </summary>
internal record CredentialOfferSent(
    WalletInteropId WalletInteropId,
    string OfferId,
    string CredentialType,
    DateTime? ExpiresAt,
    DateTime SentAt) : IDomainEvent;

/// <summary>
/// Event raised when a credential offer is accepted.
/// </summary>
internal record CredentialOfferAccepted(
    WalletInteropId WalletInteropId,
    string OfferId,
    DateTime AcceptedAt) : IDomainEvent;

/// <summary>
/// Event raised when a credential offer is rejected.
/// </summary>
internal record CredentialOfferRejected(
    WalletInteropId WalletInteropId,
    string OfferId,
    string Reason,
    DateTime RejectedAt) : IDomainEvent;

/// <summary>
/// Event raised when a presentation request is received.
/// </summary>
internal record PresentationRequestReceived(
    WalletInteropId WalletInteropId,
    string RequestId,
    string ReplyTo,
    DateTime ReceivedAt) : IDomainEvent;

/// <summary>
/// Event raised when a presentation response is sent.
/// </summary>
internal record PresentationResponseSent(
    WalletInteropId WalletInteropId,
    string RequestId,
    string PresentationData,
    DateTime SentAt) : IDomainEvent;

/// <summary>
/// Event raised when a wallet is disconnected.
/// </summary>
internal record WalletDisconnected(
    WalletInteropId WalletInteropId,
    string Reason,
    DateTime DisconnectedAt) : IDomainEvent;
