using Mamey.CQRS.Commands;

namespace Mamey.Microservice.Infrastructure.Jaeger
{
    internal static class Extensions
    {
        public static IMameyBuilder AddJaegerDecorators(this IMameyBuilder builder)
        {
            builder.Services.TryDecorate(typeof(ICommandHandler<>), typeof(JaegerCommandHandlerDecorator<>));

            return builder;
        }
    }
}