using Grpc.Core;
using Grpc.Net.Client;
using Mamey.Rbac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mamey.Blockchain.Rbac;

/// <summary>
/// gRPC client implementation for MameyNode RBAC service.
/// </summary>
public class RbacClient : IRbacClient
{
    private readonly RbacService.RbacServiceClient _client;
    private readonly GrpcChannel _channel;
    private readonly ILogger<RbacClient>? _logger;
    private readonly RbacClientOptions _options;
    private bool _disposed;

    public RbacClient(IOptions<RbacClientOptions> options, ILogger<RbacClient>? logger = null)
    {
        _options = options.Value;
        _logger = logger;
        _channel = GrpcChannel.ForAddress(_options.NodeUrl);
        _client = new RbacService.RbacServiceClient(_channel);
    }

    private CallOptions CreateCallOptions(CancellationToken cancellationToken)
    {
        var deadline = DateTime.UtcNow.AddSeconds(_options.TimeoutSeconds);
        return new CallOptions(deadline: deadline, cancellationToken: cancellationToken);
    }

    // Role management
    public async Task<CreateRoleResponse> CreateRoleAsync(CreateRoleRequest request, CancellationToken cancellationToken = default)
    {
        _logger?.LogDebug("Creating role: {RoleName}", request.RoleName);
        return await _client.CreateRoleAsync(request, CreateCallOptions(cancellationToken));
    }

    public async Task<GetRoleResponse> GetRoleAsync(GetRoleRequest request, CancellationToken cancellationToken = default)
        => await _client.GetRoleAsync(request, CreateCallOptions(cancellationToken));

    public async Task<ListRolesResponse> ListRolesAsync(ListRolesRequest request, CancellationToken cancellationToken = default)
        => await _client.ListRolesAsync(request, CreateCallOptions(cancellationToken));

    public async Task<UpdateRoleResponse> UpdateRoleAsync(UpdateRoleRequest request, CancellationToken cancellationToken = default)
        => await _client.UpdateRoleAsync(request, CreateCallOptions(cancellationToken));

    public async Task<DeleteRoleResponse> DeleteRoleAsync(DeleteRoleRequest request, CancellationToken cancellationToken = default)
        => await _client.DeleteRoleAsync(request, CreateCallOptions(cancellationToken));

    // User-role assignment
    public async Task<AssignRoleResponse> AssignRoleAsync(AssignRoleRequest request, CancellationToken cancellationToken = default)
        => await _client.AssignRoleAsync(request, CreateCallOptions(cancellationToken));

    public async Task<RevokeRoleResponse> RevokeRoleAsync(RevokeRoleRequest request, CancellationToken cancellationToken = default)
        => await _client.RevokeRoleAsync(request, CreateCallOptions(cancellationToken));

    public async Task<GetUserRolesResponse> GetUserRolesAsync(GetUserRolesRequest request, CancellationToken cancellationToken = default)
        => await _client.GetUserRolesAsync(request, CreateCallOptions(cancellationToken));

    public async Task<GetRoleUsersResponse> GetRoleUsersAsync(GetRoleUsersRequest request, CancellationToken cancellationToken = default)
        => await _client.GetRoleUsersAsync(request, CreateCallOptions(cancellationToken));

    // Permission management
    public async Task<CreatePermissionResponse> CreatePermissionAsync(CreatePermissionRequest request, CancellationToken cancellationToken = default)
        => await _client.CreatePermissionAsync(request, CreateCallOptions(cancellationToken));

    public async Task<CheckPermissionResponse> CheckPermissionAsync(CheckPermissionRequest request, CancellationToken cancellationToken = default)
        => await _client.CheckPermissionAsync(request, CreateCallOptions(cancellationToken));

    public async Task<GetRolePermissionsResponse> GetRolePermissionsAsync(GetRolePermissionsRequest request, CancellationToken cancellationToken = default)
        => await _client.GetRolePermissionsAsync(request, CreateCallOptions(cancellationToken));

    // Hierarchy
    public async Task<AddInheritanceResponse> AddInheritanceAsync(AddInheritanceRequest request, CancellationToken cancellationToken = default)
        => await _client.AddInheritanceAsync(request, CreateCallOptions(cancellationToken));

    public async Task<RemoveInheritanceResponse> RemoveInheritanceAsync(RemoveInheritanceRequest request, CancellationToken cancellationToken = default)
        => await _client.RemoveInheritanceAsync(request, CreateCallOptions(cancellationToken));

    public async Task<GetRoleHierarchyResponse> GetRoleHierarchyAsync(GetRoleHierarchyRequest request, CancellationToken cancellationToken = default)
        => await _client.GetRoleHierarchyAsync(request, CreateCallOptions(cancellationToken));

    public void Dispose()
    {
        if (!_disposed)
        {
            _channel?.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
