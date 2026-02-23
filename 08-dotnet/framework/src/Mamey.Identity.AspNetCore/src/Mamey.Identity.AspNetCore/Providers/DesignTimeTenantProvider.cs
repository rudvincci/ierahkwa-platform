// using Mamey.Persistence.SQL;
// using Mamey.Types;
//
// namespace Mamey.Identity.AspNetCore.Providers;
//
// public sealed class DesignTimeTenantProvider : ITenantProvider, ITenantProviderSetter
// {
//     public static readonly DesignTimeTenantProvider Instance = new();
//     private TenantId _currentTenantId;
//     private UserId _currentUserId;
//
//     private DesignTimeTenantProvider() { }
//
//     public TenantId CurrentTenantId => _currentTenantId.Value != Guid.Empty ? _currentTenantId : new TenantId(Guid.Empty);
//     public UserId CurrentUserId => _currentUserId.Value != Guid.Empty ? _currentUserId : UserId.Empty;
//
//     public bool TryGetTenantId(out TenantId tenant)
//     {
//         tenant = CurrentTenantId;
//         return tenant.Value != Guid.Empty;
//     }
//
//     public bool TryGetUserId(out UserId user)
//     {
//         user = CurrentUserId;
//         return user.Value != Guid.Empty;
//     }
//
//     public void SetCurrentTenantId(TenantId tenantId)
//     {
//         _currentTenantId = tenantId;
//     }
//
//     public void ClearCurrentTenantId()
//     {
//         _currentTenantId = default;
//     }
// }

