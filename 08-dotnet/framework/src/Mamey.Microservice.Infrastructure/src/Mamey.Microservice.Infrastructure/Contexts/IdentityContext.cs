using Mamey.Contexts;

namespace Mamey.Microservice.Infrastructure.Contexts
{
    internal sealed class IdentityContext : IIdentityContext
    {
        public Guid Id { get; }
        // public Guid OrganizationId { get; private set; }
        public string Role { get; } = string.Empty;
        public bool IsAuthenticated { get; }
        public bool IsAdmin { get; }
        public IDictionary<string, string> Claims { get; } = new Dictionary<string, string>();

        internal IdentityContext()
        {
        }

        internal IdentityContext(CorrelationContext.UserContext context)
            : this(context.Id ?? string.Empty, /* context.OrganizationId, */ context.Role ?? string.Empty, context.IsAuthenticated, context.Claims ?? new Dictionary<string, string>())
        {
        }

        internal IdentityContext(string id, /*string orgId,*/ string role, bool isAuthenticated, IDictionary<string, string> claims)
        {
            Id = Guid.TryParse(id, out var userId) ? userId : Guid.Empty;
            //OrganizationId = Guid.TryParse(orgId, out var organizationId) ? organizationId : Guid.Empty;

            Role = role ?? string.Empty;
            IsAuthenticated = isAuthenticated;
            IsAdmin = Role.Equals("admin", StringComparison.InvariantCultureIgnoreCase);
            Claims = claims ?? new Dictionary<string, string>();
        }

        internal static IIdentityContext Empty => new IdentityContext();

        // public void SetOrganizationId(Guid organizationId)
        // {
        //     OrganizationId = organizationId;
        // }
    }
}

