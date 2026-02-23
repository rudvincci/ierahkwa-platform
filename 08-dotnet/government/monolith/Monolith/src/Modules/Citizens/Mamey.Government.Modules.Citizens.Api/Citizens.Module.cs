using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.Government.Modules.Citizens.Core;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.BlazorServer")]
namespace Mamey.Government.Modules.Citizens.Api
{
    internal class CitizensModule : IModule
    {
        public const string BasePath = "api/citizens";        
        public string Name { get; } = "Citizens";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"citizens"};
        public Dictionary<Type, Dictionary<string, long>> RolesPermissionMapping { get; }

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
