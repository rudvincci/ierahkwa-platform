using Mamey.MessageBrokers.Outbox.EntityFramework.Internals;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.MessageBrokers.Outbox.EntityFramework;

public static class Extensions
{
    public static IMessageOutboxConfigurator AddEntityFramework<T>(this IMessageOutboxConfigurator configurator)
        where T : DbContext
    {
        var builder = configurator.Builder;
        builder.Services.AddTransient<IMessageOutbox, EntityFrameworkMessageOutbox<T>>();
        builder.Services.AddTransient<IMessageOutboxAccessor, EntityFrameworkMessageOutbox<T>>();

        return configurator;
    }
    public static IMessageOutboxConfigurator AddEntityFramework<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>(this IMessageOutboxConfigurator configurator)
        where TContext :  IdentityDbContext<TUser,TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim: IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>

    {
        var builder = configurator.Builder;
        builder.Services.AddTransient<IMessageOutbox, EntityFrameworkMessageOutbox<TContext>>();
        builder.Services.AddTransient<IMessageOutboxAccessor, EntityFrameworkMessageOutbox<TContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken >> ();

        return configurator;
    }
}
/***
 * 
 * : IdentityRole<Guid> { }

public class AppRoleClaim : IdentityRoleClaim<Guid> { }




public class AppUserToken : IdentityUserToken<Guid> { }
*/