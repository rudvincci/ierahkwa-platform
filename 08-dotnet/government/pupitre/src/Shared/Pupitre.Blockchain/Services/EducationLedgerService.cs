using System.Globalization;
using Google.Protobuf.Collections;
using Mamey.Blockchain.Government;
using Mamey.Blockchain.LedgerIntegration;
using Mamey.Blockchain.Node;
using Mamey.Node;
using Mamey.Ledger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pupitre.Blockchain.Models;
using Pupitre.Blockchain.Options;

namespace Pupitre.Blockchain.Services;

internal sealed class EducationLedgerService : IEducationLedgerService
{
    private readonly GovernmentClient _governmentClient;
    private readonly MameyLedgerClient _ledgerClient;
    private readonly MameyNodeClient _nodeClient;
    private readonly ILogger<EducationLedgerService> _logger;
    private readonly MameyNodeOptions _options;

    public EducationLedgerService(
        GovernmentClient governmentClient,
        MameyLedgerClient ledgerClient,
        MameyNodeClient nodeClient,
        IOptions<MameyNodeOptions> options,
        ILogger<EducationLedgerService> logger)
    {
        _governmentClient = governmentClient;
        _ledgerClient = ledgerClient;
        _nodeClient = nodeClient;
        _logger = logger;
        _options = options.Value;
    }

    public async Task<EducationLedgerReceipt> PublishCredentialAsync(
        EducationLedgerPayload payload,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var fullName = BuildFullName(payload.FirstName, payload.LastName);
        var nationality = string.IsNullOrWhiteSpace(payload.Nationality)
            ? _options.DefaultNationality
            : payload.Nationality!;

        // 1. Ensure a government identity exists.
        var (identityId, blockchainAccount) = await EnsureIdentityAsync(
            payload,
            fullName,
            nationality,
            cancellationToken);

        // 2. Upload credential document if provided.
        var (documentId, documentHash) = await UploadCredentialDocumentAsync(
            identityId,
            payload,
            cancellationToken);

        // 3. Log the transaction on the ledger (best-effort).
        var ledgerTransactionId = await LogLedgerTransactionAsync(
            payload,
            identityId,
            blockchainAccount,
            cancellationToken);

        return new EducationLedgerReceipt
        {
            IdentityId = identityId,
            BlockchainAccount = blockchainAccount,
            DocumentId = documentId,
            DocumentHash = documentHash,
            LedgerTransactionId = ledgerTransactionId,
            CredentialIssuedAt = payload.CompletionDate ?? DateTime.UtcNow,
            PublishedToLedger = !string.IsNullOrWhiteSpace(ledgerTransactionId)
        };
    }

    private async Task<(string IdentityId, string? BlockchainAccount)> EnsureIdentityAsync(
        EducationLedgerPayload payload,
        string fullName,
        string nationality,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(payload.IdentityId))
        {
            _logger.LogDebug("Using provided identity {IdentityId} for ministry data {MinistryDataId}",
                payload.IdentityId, payload.MinistryDataId);
            return (payload.IdentityId!, payload.BlockchainAccount);
        }

        var request = new Mamey.Blockchain.Government.CreateIdentityRequest
        {
            CitizenId = payload.CitizenId ?? payload.MinistryDataId.ToString("N"),
            FirstName = payload.FirstName ?? fullName.Split(' ').FirstOrDefault() ?? "Unknown",
            LastName = payload.LastName ?? fullName.Split(' ').Skip(1).FirstOrDefault() ?? "Unknown",
            DateOfBirth = payload.DateOfBirth?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? "2000-01-01",
            Nationality = nationality
        };

        if (payload.Metadata is { Count: > 0 })
        {
            foreach (var (key, value) in payload.Metadata)
            {
                request.Metadata[key] = value;
            }
        }

        var response = await _governmentClient.CreateIdentityAsync(request, cancellationToken);
        if (!response.Success)
        {
            throw new InvalidOperationException($"Unable to create MameyNode identity: {response.ErrorMessage}");
        }

        _logger.LogInformation(
            "Created blockchain identity {IdentityId} for ministry data {MinistryDataId}",
            response.IdentityId,
            payload.MinistryDataId);

        return (response.IdentityId, response.BlockchainAccount);
    }

    private async Task<(string? DocumentId, string? DocumentHash)> UploadCredentialDocumentAsync(
        string identityId,
        EducationLedgerPayload payload,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(payload.CredentialDocumentBase64))
        {
            return (null, null);
        }

        try
        {
            var buffer = Convert.FromBase64String(payload.CredentialDocumentBase64);
            var uploadResult = await _governmentClient.UploadDocumentAsync(
                new Mamey.Blockchain.Government.UploadDocumentRequest
                {
                    IdentityId = identityId,
                    DocumentType = payload.CredentialType ?? "education-credential",
                    DocumentData = buffer,
                    MimeType = payload.CredentialMimeType,
                    Metadata = payload.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ?? new Dictionary<string, string>()
                },
                cancellationToken);

            if (!uploadResult.Success)
            {
                _logger.LogWarning("Failed to upload credential document: {Message}", uploadResult.ErrorMessage);
                return (null, null);
            }

            return (uploadResult.DocumentId, uploadResult.DocumentHash);
        }
        catch (FormatException ex)
        {
            _logger.LogWarning(ex, "Credential document for ministry data {MinistryDataId} is not valid base64",
                payload.MinistryDataId);
            return (null, null);
        }
    }

    private async Task<string?> LogLedgerTransactionAsync(
        EducationLedgerPayload payload,
        string identityId,
        string? blockchainAccount,
        CancellationToken cancellationToken)
    {
        try
        {
            var request = new LogTransactionRequest
            {
                TransactionId = payload.TransactionId ?? $"edu-{payload.MinistryDataId:N}",
                TransactionType = payload.CredentialType ?? "education-credential",
                FromAccount = payload.SourceAccount ?? blockchainAccount ?? "pupitre",
                ToAccount = payload.TargetAccount ?? blockchainAccount ?? "pupitre",
                Amount = payload.Amount ?? "0",
                Currency = payload.Currency ?? "EDU"
            };

            request.Metadata.Add("ministryDataId", payload.MinistryDataId.ToString());
            request.Metadata.Add("identityId", identityId);
            request.Metadata.Add("programCode", payload.ProgramCode ?? "n/a");
            request.Metadata.Add("network", _options.Network);

            await AppendNodeMetadataAsync(request.Metadata, cancellationToken);

            if (payload.Metadata is { Count: > 0 })
            {
                foreach (var (key, value) in payload.Metadata)
                {
                    request.Metadata[key] = value;
                }
            }

            var response = await _ledgerClient.Client.LogTransactionAsync(request, cancellationToken: cancellationToken);
            if (!response.Success)
            {
                _logger.LogWarning("Ledger logging failed: {Message}", response.ErrorMessage);
                return null;
            }

            return response.LogId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Unexpected error while logging ledger transaction for ministry data {MinistryDataId}",
                payload.MinistryDataId);
            return null;
        }
    }

    private async Task AppendNodeMetadataAsync(MapField<string, string> metadata, CancellationToken cancellationToken)
    {
        try
        {
            var info = await _nodeClient.Client.GetNodeInfoAsync(new GetNodeInfoRequest(), cancellationToken: cancellationToken);

            metadata["nodeVersion"] = info.Version;
            metadata["peerCount"] = info.PeerCount.ToString(CultureInfo.InvariantCulture);
        }
        catch (Exception)
        {
            // Intentionally swallow; metadata is optional and we do not want to block publishing.
        }
    }

    private static string BuildFullName(string? firstName, string? lastName)
        => string.Join(' ', new[] { firstName, lastName }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .Select(part => part!.Trim()));
}
