using Mamey.FWID.Identities.Application.Clients;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Application.Services;

/// <summary>
/// Service for handling identity operations with logging.
/// Follows the pattern: Services have ILogger<T>, handlers delegate to services.
/// </summary>
internal sealed class IdentityService : IIdentityService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly ILedgerTransactionClient _ledgerClient;
    private readonly IMameyNodeBankingClient? _mameyNodeClient;
    private readonly ILogger<IdentityService> _logger;

    public IdentityService(
        IIdentityRepository identityRepository,
        ILedgerTransactionClient ledgerClient,
        ILogger<IdentityService> logger)
        : this(identityRepository, ledgerClient, null, logger)
    {
    }

    public IdentityService(
        IIdentityRepository identityRepository,
        ILedgerTransactionClient ledgerClient,
        IMameyNodeBankingClient? mameyNodeClient,
        ILogger<IdentityService> logger)
    {
        _identityRepository = identityRepository;
        _ledgerClient = ledgerClient;
        _mameyNodeClient = mameyNodeClient;
        _logger = logger;
    }

    public async Task<Identity> CreateIdentityAsync(
        IdentityId identityId,
        Name name,
        PersonalDetails personalDetails,
        ContactInformation contactInformation,
        BiometricData biometricData,
        string? zone = null,
        Guid? clanRegistrarId = null,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating identity: {IdentityId}, Name: {Name}", identityId, name?.FullName);

        var existing = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (existing is not null)
        {
            _logger.LogWarning("Identity already exists: {IdentityId}", identityId);
            throw new InvalidOperationException($"Identity with ID {identityId} already exists");
        }

        var identity = new Identity(
            identityId,
            name,
            personalDetails,
            contactInformation,
            biometricData,
            zone,
            clanRegistrarId);

        await _identityRepository.AddAsync(identity, cancellationToken);

        // Create MameyNode blockchain account per TDD requirement (FSD-FWID-001, FSD-FWID-025)
        // TDD Sequence: IdentityService->>LedgerService: Log Identity Creation, LedgerService->>LedgerService: Log to Blockchain
        string? blockchainAccountAddress = null;
        if (_mameyNodeClient != null)
        {
            try
            {
                _logger.LogInformation(
                    "Creating MameyNode blockchain account for identity: {IdentityId}",
                    identityId);

                blockchainAccountAddress = await _mameyNodeClient.CreateAccountAsync(
                    identityId.Value.ToString(),
                    "USD",
                    cancellationToken);

                if (!string.IsNullOrEmpty(blockchainAccountAddress))
                {
                    // Store blockchain account address in identity metadata per TDD requirement
                    // This allows future operations to reference the blockchain account
                    identity.Metadata["BlockchainAccount"] = blockchainAccountAddress;
                    identity.Metadata["BlockchainAccountCreatedAt"] = DateTime.UtcNow.ToString("O");
                    identity.Metadata["BlockchainCurrency"] = "USD";

                    // Store blockchain account in metadata and update identity
                    identity.Metadata["BlockchainAccount"] = blockchainAccountAddress;
                    identity.Metadata["BlockchainAccountCreatedAt"] = DateTime.UtcNow.ToString("O");
                    identity.Metadata["BlockchainCurrency"] = "USD";
                    await _identityRepository.UpdateAsync(identity, cancellationToken);

                    // Note: Blockchain account address is stored in metadata, not in events
                    // Events are immutable once created - downstream services should query the identity
                    // for blockchain account information rather than relying on event data

                    _logger.LogInformation(
                        "Created MameyNode blockchain account for identity: {IdentityId}, Account: {BlockchainAccount}",
                        identityId,
                        blockchainAccountAddress);
                }
                else
                {
                    _logger.LogWarning(
                        "MameyNode account creation returned null for identity: {IdentityId}",
                        identityId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create MameyNode blockchain account for identity: {IdentityId}. Identity created but blockchain account not available.",
                    identityId);
                
                // Store failure in metadata for retry/compensation logic
                identity.Metadata["BlockchainAccountCreationFailed"] = true;
                identity.Metadata["BlockchainAccountCreationError"] = ex.Message;
                identity.Metadata["BlockchainAccountCreationFailedAt"] = DateTime.UtcNow.ToString("O");
                
                await _identityRepository.UpdateAsync(identity, cancellationToken);
                
                // Don't fail identity creation if blockchain account creation fails
                // Per TDD: Blockchain logging is best-effort, identity creation should succeed
            }
        }
        else
        {
            _logger.LogDebug("MameyNode integration is disabled, skipping blockchain account creation for identity: {IdentityId}", identityId);
        }

        // Log transaction to FutureWampumLedger per TDD requirement
        // TDD Sequence: IdentityService->>LedgerService: Log Identity Creation
        try
        {
            var transactionMetadata = new Dictionary<string, object>
            {
                { "Zone", zone ?? "Unknown" },
                { "ClanRegistrarId", clanRegistrarId?.ToString() ?? "Unknown" }
            };

            // Include blockchain account if created
            if (!string.IsNullOrEmpty(blockchainAccountAddress))
            {
                transactionMetadata["BlockchainAccount"] = blockchainAccountAddress;
            }

            var transactionRequest = new TransactionLogRequest
            {
                TransactionType = "IdentityCreated",
                EntityType = "Identity",
                EntityId = identityId.Value,
                Description = $"Identity created: {name?.FullName ?? "Unknown"}",
                Metadata = transactionMetadata,
                Timestamp = DateTime.UtcNow,
                CorrelationId = correlationId
            };

            await _ledgerClient.LogTransactionAsync(transactionRequest, cancellationToken);
            _logger.LogInformation(
                "Logged IdentityCreated transaction to FutureWampumLedger for IdentityId: {IdentityId}, BlockchainAccount: {BlockchainAccount}",
                identityId.Value,
                blockchainAccountAddress ?? "N/A");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to log IdentityCreated transaction to FutureWampumLedger for IdentityId: {IdentityId}",
                identityId.Value);
            // Don't fail identity creation if ledger logging fails per TDD
        }

        _logger.LogInformation("Identity created successfully: {IdentityId}", identityId);
        return identity;
    }

    public async Task RevokeIdentityAsync(
        IdentityId identityId,
        string? reason,
        Guid? revokedBy,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Revoking identity: {IdentityId}, Reason: {Reason}", identityId, reason);

        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity == null)
        {
            _logger.LogWarning("Identity not found for revocation: {IdentityId}", identityId);
            throw new InvalidOperationException($"Identity with ID {identityId} not found");
        }

        // Get blockchain account from metadata if available
        var blockchainAccount = identity.Metadata?.ContainsKey("BlockchainAccount") == true
            ? identity.Metadata["BlockchainAccount"]?.ToString()
            : null;

        identity.Revoke(reason, revokedBy);
        await _identityRepository.UpdateAsync(identity, cancellationToken);

        // Optionally suspend blockchain account on revocation (per TDD business rule)
        // Note: This is a business decision - we may want to keep the account active for audit purposes
        if (_mameyNodeClient != null && !string.IsNullOrEmpty(blockchainAccount))
        {
            try
            {
                _logger.LogInformation(
                    "Identity revoked - blockchain account remains active for audit: {IdentityId}, Account: {BlockchainAccount}",
                    identityId,
                    blockchainAccount);
                // Future: Could implement account suspension if required by business rules
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Error processing blockchain account during revocation for identity: {IdentityId}",
                    identityId);
            }
        }

        // Log transaction to ledger
        try
        {
            var transactionRequest = new TransactionLogRequest
            {
                TransactionType = "IdentityRevoked",
                EntityType = "Identity",
                EntityId = identityId.Value,
                Description = $"Identity revoked. Reason: {reason ?? "Not specified"}",
                Metadata = new Dictionary<string, object>
                {
                    { "Reason", reason ?? "Not specified" },
                    { "RevokedBy", revokedBy?.ToString() ?? "Unknown" }
                },
                Timestamp = DateTime.UtcNow,
                CorrelationId = correlationId
            };

            await _ledgerClient.LogTransactionAsync(transactionRequest, cancellationToken);
            _logger.LogInformation("Logged IdentityRevoked transaction to ledger for IdentityId: {IdentityId}", identityId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log transaction to ledger for IdentityId: {IdentityId}", identityId.Value);
        }

        _logger.LogInformation("Identity revoked successfully: {IdentityId}", identityId);
    }

    public async Task UpdateBiometricDataAsync(
        IdentityId identityId,
        BiometricData biometricData,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating biometric data for identity: {IdentityId}", identityId);

        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity == null)
        {
            _logger.LogWarning("Identity not found for biometric update: {IdentityId}", identityId);
            throw new InvalidOperationException($"Identity with ID {identityId} not found");
        }

        identity.UpdateBiometric(biometricData, null, null, false);
        await _identityRepository.UpdateAsync(identity, cancellationToken);

        // Log transaction to ledger
        try
        {
            var transactionRequest = new TransactionLogRequest
            {
                TransactionType = "BiometricDataUpdated",
                EntityType = "Identity",
                EntityId = identityId.Value,
                Description = "Biometric data updated",
                Timestamp = DateTime.UtcNow,
                CorrelationId = correlationId
            };

            await _ledgerClient.LogTransactionAsync(transactionRequest, cancellationToken);
            _logger.LogInformation("Logged BiometricDataUpdated transaction to ledger for IdentityId: {IdentityId}", identityId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log transaction to ledger for IdentityId: {IdentityId}", identityId.Value);
        }

        _logger.LogInformation("Biometric data updated successfully for identity: {IdentityId}", identityId);
    }

    public async Task LogBiometricUpdateAsync(
        IdentityId identityId,
        string biometricType,
        bool isEnrollment,
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Logging biometric update for identity: {IdentityId}, Type: {BiometricType}", identityId, biometricType);

        // Log transaction to ledger
        try
        {
            var transactionRequest = new TransactionLogRequest
            {
                TransactionType = "BiometricUpdated",
                EntityType = "Identity",
                EntityId = identityId.Value,
                Description = $"Biometric updated: {biometricType}",
                Metadata = new Dictionary<string, object>
                {
                    { "BiometricType", biometricType },
                    { "IsEnrollment", isEnrollment.ToString() }
                },
                Timestamp = DateTime.UtcNow,
                CorrelationId = correlationId
            };

            await _ledgerClient.LogTransactionAsync(transactionRequest, cancellationToken);
            _logger.LogInformation("Logged BiometricUpdated transaction to ledger for IdentityId: {IdentityId}", identityId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log transaction to ledger for IdentityId: {IdentityId}", identityId.Value);
        }
    }

    public async Task<string?> GetBlockchainAccountAsync(
        IdentityId identityId,
        CancellationToken cancellationToken = default)
    {
        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity == null)
        {
            _logger.LogWarning("Identity not found when retrieving blockchain account: {IdentityId}", identityId);
            return null;
        }

        // Get blockchain account from metadata
        if (identity.Metadata?.ContainsKey("BlockchainAccount") == true)
        {
            var account = identity.Metadata["BlockchainAccount"]?.ToString();
            _logger.LogDebug("Retrieved blockchain account for identity: {IdentityId}, Account: {Account}", identityId, account);
            return account;
        }

        // If not in metadata, try to get from MameyNode
        if (_mameyNodeClient != null)
        {
            try
            {
                var accountInfo = await _mameyNodeClient.GetAccountInfoAsync(
                    identityId.Value.ToString(),
                    cancellationToken);

                if (accountInfo != null && !string.IsNullOrEmpty(accountInfo.BlockchainAccount))
                {
                    // Store in metadata for future use
                    identity.Metadata["BlockchainAccount"] = accountInfo.BlockchainAccount;
                    await _identityRepository.UpdateAsync(identity, cancellationToken);

                    _logger.LogInformation(
                        "Retrieved blockchain account from MameyNode for identity: {IdentityId}, Account: {Account}",
                        identityId,
                        accountInfo.BlockchainAccount);

                    return accountInfo.BlockchainAccount;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to retrieve blockchain account from MameyNode for identity: {IdentityId}",
                    identityId);
            }
        }

        return null;
    }

    public async Task<string?> RetryBlockchainAccountCreationAsync(
        IdentityId identityId,
        string currency = "USD",
        string? correlationId = null,
        CancellationToken cancellationToken = default)
    {
        if (_mameyNodeClient == null)
        {
            _logger.LogWarning("MameyNode integration is disabled, cannot retry account creation for identity: {IdentityId}", identityId);
            return null;
        }

        var identity = await _identityRepository.GetAsync(identityId, cancellationToken);
        if (identity == null)
        {
            _logger.LogWarning("Identity not found for blockchain account retry: {IdentityId}", identityId);
            throw new InvalidOperationException($"Identity with ID {identityId} not found");
        }

        // Check if account already exists
        var existingAccount = await GetBlockchainAccountAsync(identityId, cancellationToken);
        if (!string.IsNullOrEmpty(existingAccount))
        {
            _logger.LogInformation(
                "Blockchain account already exists for identity: {IdentityId}, Account: {Account}",
                identityId,
                existingAccount);
            return existingAccount;
        }

        // Check if previous failure exists
        var hadPreviousFailure = identity.Metadata?.ContainsKey("BlockchainAccountCreationFailed") == true &&
                                 identity.Metadata["BlockchainAccountCreationFailed"]?.ToString() == "True";

        if (!hadPreviousFailure)
        {
            _logger.LogInformation(
                "No previous blockchain account creation failure found for identity: {IdentityId}",
                identityId);
        }

        try
        {
            _logger.LogInformation(
                "Retrying MameyNode blockchain account creation for identity: {IdentityId}, Currency: {Currency}",
                identityId,
                currency);

            var blockchainAccountAddress = await _mameyNodeClient.CreateAccountAsync(
                identityId.Value.ToString(),
                currency,
                cancellationToken);

            if (!string.IsNullOrEmpty(blockchainAccountAddress))
            {
                // Store blockchain account in metadata
                identity.Metadata["BlockchainAccount"] = blockchainAccountAddress;
                identity.Metadata["BlockchainAccountCreatedAt"] = DateTime.UtcNow.ToString("O");
                identity.Metadata["BlockchainCurrency"] = currency;

                // Clear previous failure indicators
                identity.Metadata.Remove("BlockchainAccountCreationFailed");
                identity.Metadata.Remove("BlockchainAccountCreationError");
                identity.Metadata.Remove("BlockchainAccountCreationFailedAt");

                await _identityRepository.UpdateAsync(identity, cancellationToken);

                _logger.LogInformation(
                    "Successfully created MameyNode blockchain account on retry for identity: {IdentityId}, Account: {Account}",
                    identityId,
                    blockchainAccountAddress);

                return blockchainAccountAddress;
            }
            else
            {
                _logger.LogWarning(
                    "MameyNode account creation returned null on retry for identity: {IdentityId}",
                    identityId);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to create MameyNode blockchain account on retry for identity: {IdentityId}",
                identityId);

            // Update failure metadata
            identity.Metadata["BlockchainAccountCreationFailed"] = true;
            identity.Metadata["BlockchainAccountCreationError"] = ex.Message;
            identity.Metadata["BlockchainAccountCreationFailedAt"] = DateTime.UtcNow.ToString("O");
            identity.Metadata["BlockchainAccountRetryCount"] = 
                (int.TryParse(identity.Metadata.GetValueOrDefault("BlockchainAccountRetryCount")?.ToString(), out var count) ? count : 0) + 1;

            await _identityRepository.UpdateAsync(identity, cancellationToken);

            return null;
        }
    }
}

