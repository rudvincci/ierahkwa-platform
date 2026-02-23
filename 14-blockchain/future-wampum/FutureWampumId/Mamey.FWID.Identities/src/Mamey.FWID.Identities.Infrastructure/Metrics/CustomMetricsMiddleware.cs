using System.Collections.Generic;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Counter;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Mamey.FWID.Identities.Contracts.Commands;
using Mamey.FWID.Identities.Contracts.Queries;
using UpdateZone = Mamey.FWID.Identities.Contracts.Commands.UpdateZone;
using UpdateContactInformation = Mamey.FWID.Identities.Contracts.Commands.UpdateContactInformation;
using MetricsOptions = Mamey.Metrics.AppMetrics.MetricsOptions;

namespace Mamey.FWID.Identities.Infrastructure.Metrics
{
    public class CustomMetricsMiddleware : IMiddleware
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly bool _enabled;

        private readonly IDictionary<string, CounterOptions> _metrics = new Dictionary<string, CounterOptions>
        {
            [GetKey("GET", "/api/identities")] = Query(typeof(GetIdentity).Name),
            [GetKey("POST", "/api/identities")] = Command(typeof(AddIdentity).Name),
            [GetKey("POST", "/api/identities/verify")] = Command(typeof(VerifyBiometric).Name),
            [GetKey("PUT", "/api/identities/biometric")] = Command(typeof(UpdateBiometric).Name),
            [GetKey("POST", "/api/identities/revoke")] = Command(typeof(RevokeIdentity).Name),
            [GetKey("PUT", "/api/identities/zone")] = Command(typeof(UpdateZone).Name),
            [GetKey("PUT", "/api/identities/contact")] = Command(typeof(UpdateContactInformation).Name)
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