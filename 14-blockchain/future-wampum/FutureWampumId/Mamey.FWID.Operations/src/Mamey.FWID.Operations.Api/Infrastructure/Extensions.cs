using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using Grpc.Core;
using Mamey.Contexts;
using Mamey.CQRS.Commands;
using Mamey.CQRS.Events;
using Mamey.FWID.Operations.Api.DTO;
using Mamey.FWID.Operations.Api.Types;
using Mamey.MessageBrokers;
using Newtonsoft.Json;
using Mamey.FWID.Operations.Api.Services;
using Mamey.FWID.Operations.Api.Handlers;
using Mamey.Microservice.Infrastructure;
using Mamey.WebApi;
using Mamey.FWID.Operations.GrpcClient.Protos;
using Mamey.FWID.Operations.Api.Infrastructure.Grpc.Interceptors;
using Mamey.Security;

namespace Mamey.FWID.Operations.Api.Infrastructure;

internal static class Extensions
{
    public static string ToUserGroup(this Guid userId) => userId.ToString("N").ToUserGroup();
    public static string ToUserGroup(this string userId) => $"users:{userId}";

    public static IMameyBuilder AddOperationsInfrastructure(this IMameyBuilder builder)
    {
        return builder;
    }
    public static CorrelationContext GetCorrelationContext(this ICorrelationContextAccessor accessor)
    {
        if (accessor.CorrelationContext is null)
        {
            return null;
        }

        var payload = JsonConvert.SerializeObject(accessor.CorrelationContext);

        return string.IsNullOrWhiteSpace(payload)
            ? null
            : JsonConvert.DeserializeObject<CorrelationContext>(payload);
    }

    public static IMameyBuilder AddInfrastructure(this IMameyBuilder builder)
    {
        var requestsOptions = builder.GetOptions<RequestsOptions>("requests");
        builder.Services.AddSingleton(requestsOptions);
        builder.Services.AddTransient<ICommandHandler<ICommand>, GenericCommandHandler<ICommand>>()
            .AddTransient<IEventHandler<IEvent>, GenericEventHandler<IEvent>>()
            .AddTransient<IEventHandler<IRejectedEvent>, GenericRejectedEventHandler<IRejectedEvent>>()
            .AddTransient<IHubService, HubService>()
            .AddTransient<IHubWrapper, HubWrapper>()
            .AddSingleton<IOperationsService, OperationsService>();
        
        // Register gRPC authentication interceptors
        builder.Services.AddScoped<JwtAuthenticationInterceptor>();
        builder.Services.AddScoped<CertificateAuthenticationInterceptor>();
        
        // Register gRPC with authentication interceptors
        builder.Services.AddGrpc(options =>
        {
            // Add authentication interceptors globally
            options.Interceptors.Add<JwtAuthenticationInterceptor>();
            options.Interceptors.Add<CertificateAuthenticationInterceptor>();
        });

        builder.Services.AddSignalR();

        return builder
            .AddErrorHandler<ExceptionToResponseMapper>()
            .AddSecurity()  // Register Mamey.Security for encryption/hashing
            .AddMicroserviceSharedInfrastructure()
            .AddCommandHandlers()
            .AddEventHandlers();
    }
}
public class GrpcServiceHost : GrpcOperationsService.GrpcOperationsServiceBase
{
    private readonly IOperationsService _operationsService;
    private readonly ILogger<GrpcServiceHost> _logger;
    private readonly BlockingCollection<OperationDto> _operations = new BlockingCollection<OperationDto>();

    public GrpcServiceHost(IOperationsService operationsService, ILogger<GrpcServiceHost> logger)
    {
        _operationsService = operationsService;
        _logger = logger;
        _operationsService.OperationUpdated += (s, e) => _operations.TryAdd(e.Operation);
    }

    public override async Task<GetOperationResponse> GetOperation(GetOperationRequest request,
        ServerCallContext context)
    {
        _logger.LogInformation($"Received 'Get operation' (id: {request.Id}) request from: {context.Peer}");

        return Map(await _operationsService.GetAsync(request.Id));
    }

    public override async Task SubscribeOperations(Empty request,
        IServerStreamWriter<GetOperationResponse> responseStream, ServerCallContext context)
    {
        _logger.LogInformation($"Received 'Subscribe operations' request from: {context.Peer}");
        while (true)
        {
            var operation = _operations.Take();
            await responseStream.WriteAsync(Map(operation));
        }
    }

    private static GetOperationResponse Map(OperationDto operation)
        => operation is null
            ? new GetOperationResponse()
            : new GetOperationResponse
            {
                Id = operation.Id,
                UserId = operation.UserId,
                Name = operation.Name,
                Code = operation.Code,
                Reason = operation.Reason,
                State = operation.State.ToString().ToLowerInvariant()
            };
}


