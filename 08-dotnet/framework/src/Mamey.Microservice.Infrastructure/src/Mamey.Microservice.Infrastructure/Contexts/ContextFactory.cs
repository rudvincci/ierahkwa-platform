using Mamey.MessageBrokers;
using Mamey.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Mamey.Microservice.Infrastructure.Contexts;

internal sealed class ContextFactory : IContextFactory
{
    private readonly ICorrelationContextAccessor _contextAccessor;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ContextFactory(ICorrelationContextAccessor contextAccessor, IHttpContextAccessor httpContextAccessor)
    {
        _contextAccessor = contextAccessor;
        _httpContextAccessor = httpContextAccessor;
    }

        public IContext Create()
        {
            if (_contextAccessor.CorrelationContext is { })
            {
                var payload = JsonConvert.SerializeObject(_contextAccessor.CorrelationContext);

                if (string.IsNullOrWhiteSpace(payload))
                {
                    return Context.Empty;
                }

                var correlationContext = JsonConvert.DeserializeObject<CorrelationContext>(payload);
                return correlationContext is null ? Context.Empty : new Context(correlationContext);
            }

            var context = _httpContextAccessor.GetCorrelationContext();
            
            //OrganizationId organizationIdHeader = Guid.Empty;

            //TryGetOrganizationIdHeader(out organizationIdHeader);

            // return context is null
            //     ?
            //     (organizationIdHeader.IsEmpty
            //         ? Context.Empty
            //         : Context.EmptyOrgContext(organizationIdHeader))
            //     : new Context(context);
            return context is null ? Context.Empty : new Context(context);
        }

    public bool TryGetOrganizationIdHeader(out OrganizationId organizationId)
    {
        var ctx = _httpContextAccessor.HttpContext;
        organizationId = Guid.Empty;
        var hasOrgId = false;
        StringValues orgHeaders;
        StringValues internalRequest;
        bool isMameyService = false;
        if (ctx is not null && ctx.Request.Headers.TryGetValue("X-MameySrv", out internalRequest))
        {
            var internalRequestValue = internalRequest.FirstOrDefault();
            if (!string.IsNullOrEmpty(internalRequestValue))
            {
                isMameyService = bool.Parse(internalRequestValue);
            }
            var orgHeaderExists = ctx.Request.Headers.TryGetValue("X-ORG", out orgHeaders);
            if (!isMameyService && !orgHeaderExists && orgHeaders.Count == 0)
            {
                throw new InvalidOrganizationException();
            }

            hasOrgId = OrganizationId.TryParse(orgHeaders.FirstOrDefault(), out organizationId);
            if (!hasOrgId)
            {
                organizationId = Guid.Empty;
            }
        }
        
        return hasOrgId;

    }
}

