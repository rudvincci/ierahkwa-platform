using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.Government.Modules.CitizenshipApplications.Core;
using Mamey.Government.Modules.CitizenshipApplications.Core.Events;
using Mamey.MicroMonolith.Infrastructure.Contracts;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.BlazorServer")]
namespace Mamey.Government.Modules.CitizenshipApplications.Api
{
    internal class CitizenshipApplicationsModule : IModule
    {
        public const string BasePath = "api/citizenship-applications";        
        public string Name { get; } = "CitizenshipApplications";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"citizenship-applications"};

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }

        public async Task Use(IApplicationBuilder app)
        {
            app.UseModuleRequests();

    
            await app.UseCoreAsync();
        }
    }
}
