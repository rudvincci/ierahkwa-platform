using Mamey.Contexts;

namespace Mamey.Microservice.Infrastructure.Contexts
{
    internal sealed class Context : IContext
    {
        public string RequestId { get; }
        public IIdentityContext Identity { get; }
        public Guid CorrelationId { get; }
        public string TraceId { get; }
        public string IpAddress { get; }
        public string UserAgent { get; }

        internal Context() : this(Guid.NewGuid().ToString("N"), IdentityContext.Empty)
        {
            CorrelationId = Guid.NewGuid();
            TraceId = string.Empty;
            IpAddress = string.Empty;
            UserAgent = string.Empty;
        }

        internal Context(CorrelationContext context) : this(
            context.CorrelationId ?? Guid.NewGuid().ToString("N"),
            context.User is null ? IdentityContext.Empty : new IdentityContext(context.User))
        {
            CorrelationId = Guid.TryParse(context.CorrelationId, out var correlationId) 
                ? correlationId 
                : Guid.NewGuid();
            TraceId = context.TraceId ?? string.Empty;
            IpAddress = string.Empty;
            UserAgent = string.Empty;
        }

        internal Context(string requestId, IIdentityContext identity)
        {
            RequestId = requestId ?? Guid.NewGuid().ToString("N");
            Identity = identity ?? IdentityContext.Empty;
            CorrelationId = Guid.TryParse(requestId, out var correlationId) 
                ? correlationId 
                : Guid.NewGuid();
            TraceId = string.Empty;
            IpAddress = string.Empty;
            UserAgent = string.Empty;
        }
        

        internal static IContext Empty => new Context();
       
    }
}

