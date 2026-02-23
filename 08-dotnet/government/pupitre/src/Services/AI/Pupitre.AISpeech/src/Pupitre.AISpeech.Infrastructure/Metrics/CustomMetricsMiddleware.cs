using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Pupitre.AISpeech.Application.Commands;
using Pupitre.AISpeech.Application.Queries;
using Pupitre.AISpeech.Contracts.Commands;
using MetricsOptions = Mamey.Metrics.AppMetrics.MetricsOptions;

namespace Pupitre.AISpeech.Infrastructure.Metrics
{
    public class CustomMetricsMiddleware : IMiddleware
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly bool _enabled;

        private readonly IDictionary<string, CounterOptions> _metrics = new Dictionary<string, CounterOptions>
        {
            [GetKey("GET", "/aispeech")] = Query(typeof(GetSpeechRequest).Name),
            [GetKey("POST", "/aispeech")] = Command(typeof(AddSpeechRequest).Name),
            [GetKey("PUT", "/aispeech")] = Command(typeof(UpdateSpeechRequest).Name),
            [GetKey("DELETE", "/aispeech")] = Command(typeof(DeleteSpeechRequest).Name)
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