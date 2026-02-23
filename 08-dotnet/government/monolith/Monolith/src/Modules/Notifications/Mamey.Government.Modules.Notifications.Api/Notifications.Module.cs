using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.CQRS.Commands;
using Mamey.Government.Modules.Notifications.Core;
using Mamey.Government.Modules.Notifications.Core.Commands;
using Mamey.Government.Modules.Notifications.Core.Events.External.Users;
using Mamey.Emails;
using Mamey.MicroMonolith.Infrastructure.Contracts;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.Notifications.Api
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
        
        public async Task Use(IApplicationBuilder app)
        {
            app
                .UseModuleRequests()
                .Subscribe<SendCitizenshipApplicationLink, object>(
                    "notifications/send-citizenship-application-link",
                    async (command, sp, cancellationToken) =>
                    {
                        var dispatcher = sp.GetRequiredService<ICommandDispatcher>();
                        await dispatcher.SendAsync(command, cancellationToken);
                        return null;
                    })
                .Subscribe<SendCitizenshipApplicationResumeLink, object>(
                    "notifications/send-citizenship-application-resume-link",
                    async (command, sp, cancellationToken) =>
                    {
                        var dispatcher = sp.GetRequiredService<ICommandDispatcher>();
                        await dispatcher.SendAsync(command, cancellationToken);
                        return null;
                    });
            
            app.UseContracts()
                // .Register<SignedUpContract>()
                // .Register<PasswordResetRequestedContract>()
                ;
            
            await app.UseCoreAsync();
        }
    }
}
