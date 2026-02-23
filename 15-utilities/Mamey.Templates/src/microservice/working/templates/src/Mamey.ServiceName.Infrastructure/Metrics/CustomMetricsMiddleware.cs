using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Mamey.ServiceName.Application.Commands;
using Mamey.ServiceName.Application.Queries;
using Mamey.ServiceName.Contracts.Commands;
using MetricsOptions = Mamey.Metrics.AppMetrics.MetricsOptions;

namespace Mamey.ServiceName.Infrastructure.Metrics
{
    public class CustomMetricsMiddleware : IMiddleware
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly bool _enabled;

        private readonly IDictionary<string, CounterOptions> _metrics = new Dictionary<string, CounterOptions>
        {
            [GetKey("GET", "/servicename")] = Query(typeof(GetEntityName).Name),
            [GetKey("POST", "/servicename")] = Command(typeof(AddEntityName).Name),
            [GetKey("PUT", "/servicename")] = Command(typeof(UpdateEntityName).Name),
            [GetKey("DELETE", "/servicename")] = Command(typeof(DeleteEntityName).Name)
        };

        public CustomMetricsMiddleware(IServiceScopeFactory serviceScopeFactory, MetricsOptions options)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _enabled = options.Enabled;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!_enabled)
            {
                return next(context);
            }

            var request = context.Request;
            if (!_metrics.TryGetValue(GetKey(request.Method, request.Path.ToString()), out var metrics))
            {
                return next(context);
            }

            using var scope = _serviceScopeFactory.CreateScope();
            var metricsRoot = scope.ServiceProvider.GetRequiredService<IMetricsRoot>();
            metricsRoot.Measure.Counter.Increment(metrics);

            return next(context);
        }

        private static string GetKey(string method, string path)
            => $"{method}:{path}";

        private static CounterOptions Command(string command)
            => new CounterOptions
            {
                Name = "commands",
                Tags = new MetricTags("command", command)
            };

        private static CounterOptions Query(string query)
            => new CounterOptions
            {
                Name = "queries",
                Tags = new MetricTags("query", query)
            };
    }
}