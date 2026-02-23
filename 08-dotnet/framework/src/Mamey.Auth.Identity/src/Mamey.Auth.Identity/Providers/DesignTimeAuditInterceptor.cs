// using Mamey.Persistence.SQL;
// using Mamey.Types;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Diagnostics;
//
// namespace Mamey.Auth.Identity.Providers;
//
// public sealed class DesignTimeAuditInterceptor : AuditableSaveChangesInterceptor
// {
//     public DesignTimeAuditInterceptor() : base(DesignTimeTenantProvider.Instance) { }
// }
//
// public class AuditableSaveChangesInterceptor : SaveChangesInterceptor
// {
//     private readonly ITenantProvider _tenant;
//
//     public AuditableSaveChangesInterceptor(ITenantProvider tenant) => _tenant = tenant;
//
//     public override InterceptionResult<int> SavingChanges(
//         DbContextEventData eventData, InterceptionResult<int> result)
//     {
//         ApplyAudit(eventData.Context);
//         return base.SavingChanges(eventData, result);
//     }
//
//     public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
//         DbContextEventData eventData,
//         InterceptionResult<int> result,
//         CancellationToken cancellationToken = default)
//     {
//         ApplyAudit(eventData.Context);
//         return base.SavingChangesAsync(eventData, result, cancellationToken);
//     }
//
//     private void ApplyAudit(DbContext? ctx)
//     {
//         if (ctx == null) return;
//
//         var now   = DateTime.UtcNow;
//         var user  = _tenant.CurrentUserId;
//
//         foreach (var entry in ctx.ChangeTracker.Entries()
//                      .Where(e => e.Entity is IAuditable<UserId>)
//                      .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
//         {
//             var aud = (IAuditable<UserId>)entry.Entity;
//
//             if (entry.State == EntityState.Added)
//             {
//                 if (aud.CreatedAt == default)  aud.SetAudit(user);
//             }
//             else
//             {
//                 aud.Touch(user);
//             }
//         }
//     }
// }