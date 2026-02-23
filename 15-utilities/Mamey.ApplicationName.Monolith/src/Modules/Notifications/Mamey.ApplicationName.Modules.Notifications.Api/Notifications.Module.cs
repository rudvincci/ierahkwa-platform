using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Notifications.Core;
using Mamey.ApplicationName.Modules.Notifications.Core.Events.External.Users;
using Mamey.Emails;
using Mamey.MicroMonolith.Infrastructure.Contracts;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.ApplicationName.Modules.Notifications.Api
{
    internal class NotificationsModule : IModule
    {
        public const string BasePath = "notifications-module";        
        public string Name { get; } = "Notifications";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"notifications"};

        public void Register(IServiceCollection services)
        {
            services
                .AddCore()
                .AddEmail();
        }
        
        public Task Use(IApplicationBuilder app)
        {
            // app
            //     .UseModuleRequests()
            //     .Subscribe<NotificationDto, object>("notifications/create", async (dto, sp, cancellationToken) =>
            //     {
            //         var service = sp.GetRequiredService<INotificationsService>();
            //         await service.CreateAsync(dto);
            //         return null;
            //     })
            //     ;
            
            app.UseContracts()
                // .Register<SignedUpContract>()
                // .Register<PasswordResetRequestedContract>()
                ;
            
            app.UseCore();
            return Task.CompletedTask;
        }
    }
}