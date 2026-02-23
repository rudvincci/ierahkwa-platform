using Grpc.Core;
using Mamey.FWID.Identities.Api.Protos;
using Mamey.FWID.Identities.Application.Exceptions;
using Mamey.FWID.Identities.Application.Mappers;
using Mamey.FWID.Identities.Application.Services;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Domain.Entities;
using Mamey.FWID.Identities.Domain.Exceptions;
using Mamey.FWID.Identities.Domain.Repositories;
using Mamey.FWID.Identities.Domain.ValueObjects;
using Mamey.CQRS.Commands;
using Microsoft.Extensions.Logging;

namespace Mamey.FWID.Identities.Api.Infrastructure.Grpc;

internal class BiometricGrpcService : BiometricService.BiometricServiceBase
{
    private readonly IIdentityRepository _repository;
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IBiometricStorageService _storageService;
    private readonly ILogger<BiometricGrpcService> _logger;

    public BiometricGrpcService(
        IIdentityRepository repository,
        ICommandDispatcher commandDispatcher,
        IBiometricStorageService storageService,
        ILogger<BiometricGrpcService> logger)
    {
        _repository = repository;
        _commandDispatcher = commandDispatcher;
        _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
        _logger = logger;
    }

    public override async Task<VerifyBiometricResponse> VerifyBiometric(
        VerifyBiometricRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation("Received gRPC VerifyBiometric request for IdentityId: {IdentityId}", request.IdentityId);

        try
        {
            var identityId = new IdentityId(Guid.Parse(request.IdentityId));
            var identity = await _repository.GetAsync(identityId, context.CancellationToken);
            
            if (identity == null)
            {
                return new VerifyBiometricResponse
                {
                    IsVerified = false,
                    IdentityId = request.IdentityId,
                    ErrorMessage = $"Identity with ID {request.IdentityId} was not found"
                };
            }

            var providedBiometric = await MapBiometricDataAsync(request.ProvidedBiometric, identityId, context.CancellationToken);
            var threshold = request.Threshold > 0 ? request.Threshold : 0.95;

            // Calculate match score first
            var matchScore = identity.BiometricData.Match(providedBiometric);
            
            // If match score meets threshold, use command handler to update identity
            if (matchScore >= threshold)
            {
                var command = new VerifyBiometric
                {
                    IdentityId = identityId.Value,
                    ProvidedBiometric = providedBiometric.ToDto(),
                    Threshold = threshold
                };

                await _commandDispatcher.SendAsync(command, context.CancellationToken);
            }
            else
            {
                // Match score below threshold - return failure without updating
                return new VerifyBiometricResponse
                {
                    IsVerified = false,
                    MatchScore = matchScore,
                    Threshold = threshold,
                    IdentityId = request.IdentityId,
                    ErrorMessage = $"Biometric verification failed. Match score: {matchScore:F2}, Threshold: {threshold:F2}"
                };
            }

            return new VerifyBiometricResponse
            {
                IsVerified = true,
                MatchScore = matchScore,
                Threshold = threshold,
                IdentityId = request.IdentityId,
                VerifiedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }
        catch (BiometricVerificationFailedException ex)
        {
            _logger.LogWarning(ex, "Biometric verification failed for IdentityId: {IdentityId}", request.IdentityId);
            return new VerifyBiometricResponse
            {
                IsVerified = false,
                MatchScore = ex.MatchScore,
                Threshold = ex.Threshold,
                IdentityId = request.IdentityId,
                ErrorMessage = ex.Message
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying biometric for IdentityId: {IdentityId}", request.IdentityId);
            return new VerifyBiometricResponse
            {
                IsVerified = false,
                IdentityId = request.IdentityId,
                ErrorMessage = ex.Message
            };
        }
    }

    public override async Task VerifyBiometricStream(
        IAsyncStreamReader<VerifyBiometricRequest> requestStream,
        IServerStreamWriter<VerifyBiometricResponse> responseStream,
        ServerCallContext context)
    {
        _logger.LogInformation("Started gRPC VerifyBiometricStream");

        await foreach (var request in requestStream.ReadAllAsync(context.CancellationToken))
        {
            var response = await VerifyBiometric(request, context);
            await responseStream.WriteAsync(response);
        }
    }

    private async Task<BiometricData> MapBiometricDataAsync(BiometricDataMessage message, IdentityId identityId, CancellationToken cancellationToken)
    {
        var biometricType = Enum.Parse<BiometricType>(message.Type, ignoreCase: true);
        var data = message.Data.ToArray();
        
        // If storageReference is provided, download the data from MinIO
        // Otherwise, use the provided data directly
        if (!string.IsNullOrEmpty(message.StorageReference) && (data == null || data.Length == 0))
        {
            try
            {
                _logger.LogInformation("Downloading biometric data from MinIO for IdentityId: {IdentityId}, StorageReference: {StorageReference}",
                    identityId.Value, message.StorageReference);
                
                data = await _storageService.DownloadBiometricAsync(identityId, biometricType, message.StorageReference, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to download biometric data from MinIO for IdentityId: {IdentityId}, StorageReference: {StorageReference}",
                    identityId.Value, message.StorageReference);
                // Fall back to empty data - will be handled by validation
                data = Array.Empty<byte>();
            }
        }
        
        // BiometricData constructor: (BiometricType type, byte[] encryptedTemplate, string hash)
        // Use storageReference as hash if provided, otherwise generate hash from data
        var hash = message.StorageReference ?? string.Empty;
        if (string.IsNullOrEmpty(hash) && data != null && data.Length > 0)
        {
            // Generate a hash from the data (this would typically be done by the domain)
            hash = Convert.ToBase64String(data);
        }
        
        return new BiometricData(
            biometricType,
            data ?? Array.Empty<byte>(),
            hash
        );
    }
}

