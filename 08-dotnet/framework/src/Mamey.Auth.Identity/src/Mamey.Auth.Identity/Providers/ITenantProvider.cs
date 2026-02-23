using Mamey.Types;
using Microsoft.AspNetCore.Http;

namespace Mamey.Auth.Identity.Providers;

/// <summary>Provides the current tenant & user for this request/scope.</summary>
public interface ITenantProvider
{
    TenantId CurrentTenantId { get; }
    UserId   CurrentUserId   { get; }
    bool TryGetTenantId(out TenantId tenantId);
    bool TryGetUserId(out UserId userId);
}
public interface ITenantProviderSetter
{
    void SetCurrentTenantId(TenantId tenantId);
    void ClearCurrentTenantId();
}


    public sealed class TenantProvider : ITenantProvider, ITenantProviderSetter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private TenantId _currentTenantId;
        private UserId _currentUserId;

        public TenantProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public TenantId CurrentTenantId
        {
            get
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    var tenantIdStr = _httpContextAccessor.HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                    if (Guid.TryParse(tenantIdStr, out var tenantIdGuid))
                    {
                        return new TenantId(tenantIdGuid);
                    }
                }
                return _currentTenantId.Value != Guid.Empty ? _currentTenantId : Guid.Empty;
            }
        }

        public UserId CurrentUserId
        {
            get
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    var userIdStr = _httpContextAccessor.HttpContext.Request.Headers["X-User-Id"].FirstOrDefault();
                    if (Guid.TryParse(userIdStr, out var userIdGuid))
                    {
                        return new UserId(userIdGuid);
                    }
                }
                return _currentUserId.Value != Guid.Empty ? _currentUserId : Guid.Empty;
            }
        }

        public bool TryGetTenantId(out TenantId tenantId)
        {
            tenantId = CurrentTenantId;
            return tenantId.Value != Guid.Empty;
        }

        public bool TryGetUserId(out UserId userId)
        {
            userId = CurrentUserId;
            return userId.Value != Guid.Empty;
        }

        public void SetCurrentTenantId(TenantId tenantId)
        {
            _currentTenantId = tenantId;
        }

        public void ClearCurrentTenantId()
        {
            _currentTenantId = default;
        }
    }
