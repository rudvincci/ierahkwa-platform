using Microsoft.Extensions.Hosting;
using Mamey.OpenTracingContrib.Internal;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mamey.OpenTracingContrib.Benchmarks")]
[assembly: InternalsVisibleTo("Mamey.OpenTracingContrib.Tests")]
[assembly: InternalsVisibleTo("Mamey.OpenTracingContrib.Benchmarks.CoreFx")]
namespace Mamey.OpenTracingContrib
{
    /// <summary>
    /// Starts and stops all OpenTracing instrumentation components.
    /// </summary>
    internal class InstrumentationService : IHostedService
    {
        private readonly DiagnosticManager _diagnosticsManager;

        public InstrumentationService(DiagnosticManager diagnosticManager)
        {
            _diagnosticsManager = diagnosticManager ?? throw new ArgumentNullException(nameof(diagnosticManager));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _diagnosticsManager.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _diagnosticsManager.Stop();

            return Task.CompletedTask;
        }
    }
}
