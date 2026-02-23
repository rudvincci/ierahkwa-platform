using System.Reflection;
using Jaeger;
using Jaeger.Reporters;
using Jaeger.Samplers;
using OpenTracing;

namespace Mamey.Tracing.Jaeger.Tracers;

internal sealed class MameyDefaultTracer
{
    public static ITracer Create()
        => new Tracer.Builder(Assembly.GetEntryAssembly().FullName)
            .WithReporter(new NoopReporter())
            .WithSampler(new ConstSampler(false))
            .Build();
}