using Microsoft.Extensions.DependencyInjection;

namespace Mamey.OpenTracingContrib.Configuration
{
    internal class OpenTracingBuilder : IOpenTracingBuilder
    {
        public IServiceCollection Services { get; }

        public OpenTracingBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
    }
}
