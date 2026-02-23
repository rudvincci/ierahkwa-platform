using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mamey.Government;

namespace Mamey.Blockchain.Government;

/// <summary>
/// Client for interacting with MameyNode government operations via gRPC
/// </summary>
public class GovernmentClient : IDisposable
{
    private readonly GrpcChannel _channel;
    private readonly GovernmentService.GovernmentServiceClient _client;
    private readonly ILogger<GovernmentClient>? _logger;
    private readonly GovernmentClientOptions _options;

    /// <summary>
    /// Initializes a new instance of the GovernmentClient
    /// </summary>
    /// <param name="options">Client options</param>
    /// <param name="logger">Optional logger</param>
    public GovernmentClient(GovernmentClientOptions options, ILogger<GovernmentClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger;

        var address = $"http://{options.Host}:{options.Port}";
        _channel = GrpcChannel.ForAddress(address);
        _client = new GovernmentService.GovernmentServiceClient(_channel);

        _logger?.LogInformation("Government client initialized for {Address}", address);
    }

    /// <summary>
    /// Initializes a new instance using IOptions pattern
    /// </summary>
    public GovernmentClient(IOptions<GovernmentClientOptions> options, ILogger<GovernmentClient>? logger = null)
        : this(options.Value, logger)
    {
    }

    // Identity operations
    public async Task<CreateIdentityResult> CreateIdentityAsync(Mamey.Blockchain.Government.CreateIdentityRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var grpcRequest = new Mamey.Government.CreateIdentityRequest
            {
                CitizenId = request.CitizenId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Nationality = request.Nationality
            };
            foreach (var kvp in request.Metadata)
            {
                grpcRequest.Metadata[kvp.Key] = kvp.Value;
            }

            var response = await _client.CreateIdentityAsync(grpcRequest, cancellationToken: cancellationToken);

            return new CreateIdentityResult
            {
                IdentityId = response.IdentityId,
                BlockchainAccount = response.BlockchainAccount,
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create identity");
            throw;
        }
    }

    public async Task<IdentityInfo?> GetIdentityAsync(string identityId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetIdentityRequest { IdentityId = identityId };
            var response = await _client.GetIdentityAsync(request, cancellationToken: cancellationToken);

            if (!response.Success)
            {
                return null;
            }

            return new IdentityInfo
            {
                IdentityId = response.IdentityId,
                CitizenId = response.CitizenId,
                FirstName = response.FirstName,
                LastName = response.LastName,
                DateOfBirth = response.DateOfBirth,
                Nationality = response.Nationality,
                Status = (IdentityStatus)(int)response.Status
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get identity {IdentityId}", identityId);
            throw;
        }
    }

    // Document operations
    public async Task<UploadDocumentResult> UploadDocumentAsync(Mamey.Blockchain.Government.UploadDocumentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var grpcRequest = new Mamey.Government.UploadDocumentRequest
            {
                IdentityId = request.IdentityId,
                DocumentType = request.DocumentType,
                DocumentData = Google.Protobuf.ByteString.CopyFrom(request.DocumentData),
                MimeType = request.MimeType
            };
            foreach (var kvp in request.Metadata)
            {
                grpcRequest.Metadata[kvp.Key] = kvp.Value;
            }

            var response = await _client.UploadDocumentAsync(grpcRequest, cancellationToken: cancellationToken);

            return new UploadDocumentResult
            {
                DocumentId = response.DocumentId,
                DocumentHash = response.DocumentHash,
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to upload document");
            throw;
        }
    }

    // Voting operations
    public async Task<CreateVoteResult> CreateVoteAsync(Mamey.Blockchain.Government.CreateVoteRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var grpcRequest = new Mamey.Government.CreateVoteRequest
            {
                VoteId = request.VoteId,
                Title = request.Title,
                Description = request.Description,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                EligibilityCriteria = request.EligibilityCriteria
            };
            grpcRequest.Options.AddRange(request.Options);

            var response = await _client.CreateVoteAsync(grpcRequest, cancellationToken: cancellationToken);

            return new CreateVoteResult
            {
                VoteId = response.VoteId,
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to create vote");
            throw;
        }
    }

    public async Task<bool> CastVoteAsync(string voteId, string identityId, string selectedOption, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new CastVoteRequest
            {
                VoteId = voteId,
                IdentityId = identityId,
                SelectedOption = selectedOption
            };

            var response = await _client.CastVoteAsync(request, cancellationToken: cancellationToken);
            return response.Success;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to cast vote");
            throw;
        }
    }

    public async Task<VoteResults?> GetVoteResultsAsync(string voteId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetVoteResultsRequest { VoteId = voteId };
            var response = await _client.GetVoteResultsAsync(request, cancellationToken: cancellationToken);

            if (!response.Success)
            {
                return null;
            }

            return new VoteResults
            {
                VoteId = response.VoteId,
                TotalVotes = (int)response.TotalVotes,
                Results = response.Results.Select(r => new VoteResult
                {
                    Option = r.Option,
                    Count = (int)r.Count,
                    Percentage = r.Percentage
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to get vote results");
            throw;
        }
    }

    // Compliance operations
    public async Task<ComplianceCheckResult> CheckComplianceAsync(string identityId, string complianceType, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new CheckComplianceRequest
            {
                IdentityId = identityId,
                ComplianceType = complianceType
            };

            var response = await _client.CheckComplianceAsync(request, cancellationToken: cancellationToken);

            return new ComplianceCheckResult
            {
                Compliant = response.Compliant,
                Violations = response.Violations.ToList(),
                Success = response.Success,
                ErrorMessage = response.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to check compliance");
            throw;
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}

