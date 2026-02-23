using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AITutors.Application.Commands;
using Pupitre.AITutors.Application.Queries;
using Pupitre.AITutors.Contracts.Commands;
using MetricsOptions = Mamey.Metrics.AppMetrics.MetricsOptions;

namespace Pupitre.AITutors.Infrastructure.Metrics
{
    public class CustomMetricsMiddleware : IMiddleware
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly bool _enabled;

        private readonly IDictionary<string, CounterOptions> _metrics = new Dictionary<string, CounterOptions>
        {
            [GetKey("GET", "/aitutors")] = Query(typeof(GetTutor).Name),
            [GetKey("POST", "/aitutors")] = Command(typeof(AddTutor).Name),
            [GetKey("PUT", "/aitutors")] = Command(typeof(UpdateTutor).Name),
            [GetKey("DELETE", "/aitutors")] = Command(typeof(DeleteTutor).Name)
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