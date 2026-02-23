using System.Runtime.CompilerServices;
using Mamey.FWID.Identities.Domain.Events;
using Mamey.Types;

[assembly: InternalsVisibleTo("Mamey.FWID.Identities.Tests.Unit.Core.Entities")]
namespace Mamey.FWID.Identities.Domain.Entities;

/// <summary>
/// Represents a wallet interoperability aggregate root for DIDComm and OpenID4VCI.
/// </summary>
internal class WalletInterop : AggregateRoot<WalletInteropId>
{
    /// <summary>
    /// Private parameterless constructor for Entity Framework Core.
    /// </summary>
    private WalletInterop()
    {
        DIDCommConnections = new List<DIDCommConnection>();
        CredentialOffers = new List<CredentialOffer>();
        PresentationRequests = new List<PresentationRequest>();
        ProtocolMessages = new List<ProtocolMessage>();
        Metadata = new Dictionary<string, object>();
    }

    /// <summary>
    /// Initializes a new instance of the WalletInterop aggregate root.
    /// </summary>
    /// <param name="id">The wallet interoperability identifier.</param>
    /// <param name="identityId">The identity that owns this wallet connection.</param>
    /// <param name="walletDid">The DID of the connected wallet.</param>
    /// <param name="walletType">The type of wallet being connected.</param>
    public WalletInterop(
        WalletInteropId id,
        IdentityId identityId,
        string walletDid,
        WalletType walletType)
        : base(id)
    {
        IdentityId = identityId ?? throw new ArgumentNullException(nameof(identityId));
        WalletDid = walletDid ?? throw new ArgumentNullException(nameof(walletDid));
        WalletType = walletType;
        Status = InteropStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        DIDCommConnections = new List<DIDCommConnection>();
        CredentialOffers = new List<CredentialOffer>();
        PresentationRequests = new List<PresentationRequest>();
        ProtocolMessages = new List<ProtocolMessage>();
        Metadata = new Dictionary<string, object>();
        Version = 1;

        AddEvent(new WalletConnectionInitiated(Id, IdentityId, WalletDid, WalletType, CreatedAt));
    }

    #region Properties

    /// <summary>
    /// The identity that owns this wallet connection.
    /// </summary>
    public IdentityId IdentityId { get; private set; }

    /// <summary>
    /// The DID of the connected wallet.
    /// </summary>
    public string WalletDid { get; private set; }

    /// <summary>
    /// The type of wallet being connected.
    /// </summary>
    public WalletType WalletType { get; private set; }

    /// <summary>
    /// The current status of the wallet connection.
    /// </summary>
    public InteropStatus Status { get; private set; }

    /// <summary>
    /// When the wallet connection was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// When the wallet connection was last active.
    /// </summary>
    public DateTime? LastActivityAt { get; private set; }

    /// <summary>
    /// The DIDComm endpoint for this wallet.
    /// </summary>
    public string? DIDCommEndpoint { get; private set; }

    /// <summary>
    /// The OpenID4VCI issuer URL.
    /// </summary>
    public string? OpenID4VCIEndpoint { get; private set; }

    /// <summary>
    /// The supported protocols.
    /// </summary>
    public List<string> SupportedProtocols { get; private set; } = new();

    /// <summary>
    /// The DIDComm connections established.
    /// </summary>
    public List<DIDCommConnection> DIDCommConnections { get; private set; }

    /// <summary>
    /// The credential offers sent to the wallet.
    /// </summary>
    public List<CredentialOffer> CredentialOffers { get; private set; }

    /// <summary>
    /// The presentation requests from the wallet.
    /// </summary>
    public List<PresentationRequest> PresentationRequests { get; private set; }

    /// <summary>
    /// The protocol messages exchanged.
    /// </summary>
    public List<ProtocolMessage> ProtocolMessages { get; private set; }

    /// <summary>
    /// Additional metadata.
    /// </summary>
    public Dictionary<string, object> Metadata { get; private set; }

    #endregion

    #region Domain Methods

    /// <summary>
    /// Establishes a DIDComm connection with the wallet.
    /// </summary>
    /// <param name="connectionId">The connection identifier.</param>
    /// <param name="endpoint">The DIDComm endpoint.</param>
    /// <param name="protocols">The supported protocols.</param>
    public void EstablishDIDCommConnection(
        string connectionId,
        string endpoint,
        List<string> protocols)
    {
        var connection = new DIDCommConnection(
            connectionId,
            endpoint,
            protocols,
            ConnectionStatus.Active,
            DateTime.UtcNow);

        DIDCommConnections.Add(connection);
        DIDCommEndpoint = endpoint;
        SupportedProtocols = protocols;
        Status = InteropStatus.Connected;
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new DIDCommConnectionEstablished(Id, connectionId, endpoint, protocols, LastActivityAt.Value));
    }

    /// <summary>
    /// Configures OpenID4VCI for the wallet.
    /// </summary>
    /// <param name="issuerEndpoint">The OpenID4VCI issuer endpoint.</param>
    /// <param name="supportedCredentialTypes">The supported credential types.</param>
    public void ConfigureOpenID4VCI(
        string issuerEndpoint,
        List<string> supportedCredentialTypes)
    {
        OpenID4VCIEndpoint = issuerEndpoint;
        Metadata["openid4vci_supported_types"] = supportedCredentialTypes;
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new OpenID4VCIConfigured(Id, issuerEndpoint, supportedCredentialTypes, LastActivityAt.Value));
    }

    /// <summary>
    /// Sends a credential offer to the wallet.
    /// </summary>
    /// <param name="offerId">The offer identifier.</param>
    /// <param name="credentialType">The type of credential being offered.</param>
    /// <param name="credentialData">The credential data.</param>
    /// <param name="expiresAt">When the offer expires.</param>
    public void SendCredentialOffer(
        string offerId,
        string credentialType,
        string credentialData,
        DateTime? expiresAt = null)
    {
        var offer = new CredentialOffer(
            offerId,
            credentialType,
            credentialData,
            OfferStatus.Pending,
            DateTime.UtcNow,
            expiresAt);

        CredentialOffers.Add(offer);
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new CredentialOfferSent(Id, offerId, credentialType, expiresAt, LastActivityAt.Value));
    }

    /// <summary>
    /// Accepts a credential offer.
    /// </summary>
    /// <param name="offerId">The offer identifier.</param>
    public void AcceptCredentialOffer(string offerId)
    {
        var offer = CredentialOffers.FirstOrDefault(o => o.OfferId == offerId);
        if (offer == null)
            throw new InvalidOperationException("Credential offer not found");

        offer.Status = OfferStatus.Accepted;
        offer.AcceptedAt = DateTime.UtcNow;
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new CredentialOfferAccepted(Id, offerId, LastActivityAt.Value));
    }

    /// <summary>
    /// Rejects a credential offer.
    /// </summary>
    /// <param name="offerId">The offer identifier.</param>
    /// <param name="reason">The reason for rejection.</param>
    public void RejectCredentialOffer(string offerId, string reason)
    {
        var offer = CredentialOffers.FirstOrDefault(o => o.OfferId == offerId);
        if (offer == null)
            throw new InvalidOperationException("Credential offer not found");

        offer.Status = OfferStatus.Rejected;
        offer.RejectedAt = DateTime.UtcNow;
        offer.RejectionReason = reason;
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new CredentialOfferRejected(Id, offerId, reason, LastActivityAt.Value));
    }

    /// <summary>
    /// Receives a presentation request from the wallet.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="presentationDefinition">The presentation definition.</param>
    /// <param name="replyTo">The reply endpoint.</param>
    public void ReceivePresentationRequest(
        string requestId,
        string presentationDefinition,
        string replyTo)
    {
        var request = new PresentationRequest(
            requestId,
            presentationDefinition,
            replyTo,
            RequestStatus.Pending,
            DateTime.UtcNow);

        PresentationRequests.Add(request);
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new PresentationRequestReceived(Id, requestId, replyTo, LastActivityAt.Value));
    }

    /// <summary>
    /// Sends a presentation response.
    /// </summary>
    /// <param name="requestId">The request identifier.</param>
    /// <param name="presentationData">The presentation data.</param>
    public void SendPresentationResponse(string requestId, string presentationData)
    {
        var request = PresentationRequests.FirstOrDefault(r => r.RequestId == requestId);
        if (request == null)
            throw new InvalidOperationException("Presentation request not found");

        request.Status = RequestStatus.Completed;
        request.CompletedAt = DateTime.UtcNow;
        request.ResponseData = presentationData;
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new PresentationResponseSent(Id, requestId, presentationData, LastActivityAt.Value));
    }

    /// <summary>
    /// Records a protocol message exchange.
    /// </summary>
    /// <param name="messageId">The message identifier.</param>
    /// <param name="protocol">The protocol used.</param>
    /// <param name="messageType">The type of message.</param>
    /// <param name="direction">The direction of the message.</param>
    /// <param name="content">The message content.</param>
    public void RecordProtocolMessage(
        string messageId,
        string protocol,
        string messageType,
        MessageDirection direction,
        string content)
    {
        var message = new ProtocolMessage(
            messageId,
            protocol,
            messageType,
            direction,
            content,
            DateTime.UtcNow);

        ProtocolMessages.Add(message);
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();
    }

    /// <summary>
    /// Disconnects the wallet.
    /// </summary>
    /// <param name="reason">The reason for disconnection.</param>
    public void Disconnect(string reason)
    {
        Status = InteropStatus.Disconnected;
        LastActivityAt = DateTime.UtcNow;
        IncrementVersion();

        AddEvent(new WalletDisconnected(Id, reason, LastActivityAt.Value));
    }

    /// <summary>
    /// Checks if the wallet connection is active.
    /// </summary>
    /// <returns>True if the wallet is connected and active.</returns>
    public bool IsActive()
    {
        return Status == InteropStatus.Connected &&
               LastActivityAt.HasValue &&
               (DateTime.UtcNow - LastActivityAt.Value) < TimeSpan.FromDays(30); // 30-day inactivity timeout
    }

    /// <summary>
    /// Gets the supported protocols for this wallet.
    /// </summary>
    /// <returns>The list of supported protocols.</returns>
    public IEnumerable<string> GetSupportedProtocols()
    {
        var protocols = new List<string>();

        if (!string.IsNullOrEmpty(DIDCommEndpoint))
            protocols.Add("didcomm");

        if (!string.IsNullOrEmpty(OpenID4VCIEndpoint))
            protocols.Add("openid4vci");

        protocols.AddRange(SupportedProtocols);

        return protocols.Distinct();
    }

    #endregion
}

/// <summary>
/// Represents the type of wallet.
/// </summary>
internal enum WalletType
{
    Mobile,
    Desktop,
    Web,
    Hardware,
    Cloud
}

/// <summary>
/// Represents the status of wallet interoperability.
/// </summary>
internal enum InteropStatus
{
    Pending,
    Connecting,
    Connected,
    Disconnected,
    Error
}

/// <summary>
/// Represents the status of a credential offer.
/// </summary>
internal enum OfferStatus
{
    Pending,
    Accepted,
    Rejected,
    Expired
}

/// <summary>
/// Represents the status of a presentation request.
/// </summary>
internal enum RequestStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}

/// <summary>
/// Represents the status of a DIDComm connection.
/// </summary>
internal enum ConnectionStatus
{
    Pending,
    Active,
    Inactive,
    Closed
}

/// <summary>
/// Represents the direction of a protocol message.
/// </summary>
internal enum MessageDirection
{
    Inbound,
    Outbound
}

/// <summary>
/// Represents a DIDComm connection.
/// </summary>
internal class DIDCommConnection
{
    public string ConnectionId { get; set; }
    public string Endpoint { get; set; }
    public List<string> Protocols { get; set; } = new();
    public ConnectionStatus Status { get; set; }
    public DateTime EstablishedAt { get; set; }

    public DIDCommConnection(
        string connectionId,
        string endpoint,
        List<string> protocols,
        ConnectionStatus status,
        DateTime establishedAt)
    {
        ConnectionId = connectionId;
        Endpoint = endpoint;
        Protocols = protocols;
        Status = status;
        EstablishedAt = establishedAt;
    }
}

/// <summary>
/// Represents a credential offer.
/// </summary>
internal class CredentialOffer
{
    public string OfferId { get; set; }
    public string CredentialType { get; set; }
    public string CredentialData { get; set; }
    public OfferStatus Status { get; set; }
    public DateTime OfferedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public string? RejectionReason { get; set; }

    public CredentialOffer(
        string offerId,
        string credentialType,
        string credentialData,
        OfferStatus status,
        DateTime offeredAt,
        DateTime? expiresAt = null)
    {
        OfferId = offerId;
        CredentialType = credentialType;
        CredentialData = credentialData;
        Status = status;
        OfferedAt = offeredAt;
        ExpiresAt = expiresAt;
    }
}

/// <summary>
/// Represents a presentation request.
/// </summary>
internal class PresentationRequest
{
    public string RequestId { get; set; }
    public string PresentationDefinition { get; set; }
    public string ReplyTo { get; set; }
    public RequestStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ResponseData { get; set; }

    public PresentationRequest(
        string requestId,
        string presentationDefinition,
        string replyTo,
        RequestStatus status,
        DateTime requestedAt)
    {
        RequestId = requestId;
        PresentationDefinition = presentationDefinition;
        ReplyTo = replyTo;
        Status = status;
        RequestedAt = requestedAt;
    }
}

/// <summary>
/// Represents a protocol message.
/// </summary>
internal class ProtocolMessage
{
    public string MessageId { get; set; }
    public string Protocol { get; set; }
    public string MessageType { get; set; }
    public MessageDirection Direction { get; set; }
    public string Content { get; set; }
    public DateTime SentAt { get; set; }

    public ProtocolMessage(
        string messageId,
        string protocol,
        string messageType,
        MessageDirection direction,
        string content,
        DateTime sentAt)
    {
        MessageId = messageId;
        Protocol = protocol;
        MessageType = messageType;
        Direction = direction;
        Content = content;
        SentAt = sentAt;
    }
}
