using Microsoft.AspNetCore.Http;
using Mamey.CQRS.Commands;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace Mamey.Policies;

public class PolicyEnforcementMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IPolicyDispatcher _policyDispatcher;

    public PolicyEnforcementMiddleware(RequestDelegate next, IPolicyDispatcher policyDispatcher)
    {
        _next = next;
        _policyDispatcher = policyDispatcher;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0; // Rewind for further use

        var commandType = GetCommandTypeFromRequest(context.Request);
        if (commandType != null)
        {
            var command = DeserializeCommand(body, commandType);
            if (command != null)
            {
                await _policyDispatcher.EvaluateAsync(command, context.RequestAborted);
                
            }
        }

        await _next(context);
    }

    private Type? GetCommandTypeFromRequest(HttpRequest request)
        => EndpointReflectionUtil.GetAllEndpointInfos()
            .Where(endpointInfo => endpointInfo.Route == request.Path && endpointInfo.HttpMethod == request.Method)
            .Select(c => c.RequestPayloadType)
            .SingleOrDefault();
        

    private ICommand DeserializeCommand(string body, Type commandType)
    {
        try
        {
            var command = JsonSerializer.Deserialize(body, commandType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) as ICommand;
            return command;
        }
        catch
        {
            // Handle deserialization error
            return null;
        }
    }
}
//internal sealed class PolicyInspector : IPolicyInspector
//{
//    private readonly IServiceScopeFactory _serviceScopeFactory;
//    private readonly IPolicyMapper _policyMapper;
//    //private readonly IMessageBroker _messageBroker;
//    private readonly ILogger<PolicyInspector> _logger;

//    public PolicyInspector(IServiceScopeFactory serviceScopeFactory, IPolicyMapper policyMapper,
//        /*IMessageBroker messageBroker, */ILogger<PolicyInspector> logger)
//    {
//        _serviceScopeFactory = serviceScopeFactory;
//        _policyMapper = policyMapper;
//        //_messageBroker = messageBroker;
//        _logger = logger;
//    }

//    public async Task EvaluateAsync(ICommand command)
//    {
//        command
//        _logger.LogTrace("Processing command policies events...");
//        var integrationEvents = await InspectPolicies(policies);
//        if (!integrationEvents.Any())
//        {
//            return;
//        }

//        if (policies is null)
//        {
//            _logger.LogTrace("Inpecting policies...");
//            return;
//        }
//        //await _messageBroker.PublishAsync(integrationEvents);
//    }

//    private async Task InspectPolicies(ICommand command)
//    {
//        if (!policies.Any())
//        {
//            await Task.CompletedTask;
//        }
//        using var scope = _serviceScopeFactory.CreateScope();
//        foreach (var policy in policies)
//        {

//            var policyType = policy.GetType();
//            _logger.LogTrace($"Inspecting policy for command: {policyType.Name}");
//            var handlerType = typeof(IPolicyHandler<>).MakeGenericType(policyType);
//            dynamic handlers = scope.ServiceProvider.GetServices(handlerType);
//            foreach (var handler in handlers)
//            {
//                await handler.HandleAsync((dynamic)policy);
//            }

//            //var policy = _policyMapper.Map(policy);
//            if (integrationEvent is null)
//            {
//                continue;
//            }

//            integrationEvents.Add(integrationEvent);
//        }


//    }
//}

public class EndpointInfo
{
    public string HttpMethod { get; set; }
    public string Route { get; set; }
    public Type RequestPayloadType { get; set; }
}

public static class EndpointReflectionUtil
{
    public static List<EndpointInfo> GetAllEndpointInfos()
    {
        var endpointInfos = new List<EndpointInfo>();

        // Assuming 'Assembly.GetExecutingAssembly()' but you might want to search other assemblies as well
        var assembliesToScan = new[] { Assembly.GetExecutingAssembly() };

        foreach (var assembly in assembliesToScan)
        {
            var controllerActions = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(ControllerBase)))
                .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly),
                            (parent, method) => new { Controller = parent, Method = method });

            foreach (var action in controllerActions)
            {
                var routeAttributes = action.Method.GetCustomAttributes<RouteAttribute>(inherit: true);
                var methodAttributes = action.Method.GetCustomAttributes<HttpMethodAttribute>(inherit: true);

                foreach (var routeAttribute in routeAttributes)
                {
                    foreach (var methodAttribute in methodAttributes)
                    {
                        var controllerRoute = action.Controller.GetCustomAttribute<RouteAttribute>()?.Template ?? "";
                        var actionRoute = routeAttribute.Template ?? "";
                        var fullRoutePath = $"{controllerRoute}/{actionRoute}".Replace("//", "/");
                        var methods = string.Join(", ", methodAttribute.HttpMethods);

                        // Identify the request payload type, if any
                        Type requestPayloadType = action.Method.GetParameters()
                            .FirstOrDefault(p => !p.ParameterType.IsPrimitive && p.ParameterType != typeof(string))?.ParameterType;

                        endpointInfos.Add(new EndpointInfo
                        {
                            HttpMethod = methods,
                            Route = fullRoutePath,
                            RequestPayloadType = requestPayloadType
                        });
                    }
                }
            }
        }

        return endpointInfos;
    }
}