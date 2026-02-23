using Mamey.Government.Modules.Notifications.Core.Clients.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Notifications.Core.Clients;

internal static class Extensions
{
   public static IServiceCollection AddModuleClients(this IServiceCollection serivces)
   {
      serivces.AddScoped<IUsersApiClient, UsersApiClient>();
      return serivces;
   }
}