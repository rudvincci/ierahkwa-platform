using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Mamey.CQRS.Queries;
using Mamey.Government.Modules.Documents.Core;
using Mamey.Government.Modules.Documents.Core.DTO;
using Mamey.Government.Modules.Documents.Core.Queries;
using Mamey.MicroMonolith.Infrastructure.Modules;
using Mamey.Modules;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Mamey.Government.BlazorServer")]
namespace Mamey.Government.Modules.Documents.Api
{
    internal class DocumentsModule : IModule
    {
        public const string BasePath = "api/documents";        
        public string Name { get; } = "Documents";
        public string Path => BasePath;

        public IEnumerable<string> Policies { get; } = new[] {"documents"};
        public Dictionary<Type, Dictionary<string, long>> RolesPermissionMapping { get; }

        public void Register(IServiceCollection services)
        {
            services.AddCore();
        }

        public async Task Use(IApplicationBuilder app)
        {
            app.UseModuleRequests()
                .Subscribe<GetDocument, DocumentDto?>("documents/get",
                    async (query, serviceProvider, cancellationToken)
                        => await serviceProvider.GetRequiredService<IQueryDispatcher>()
                            .QueryAsync<GetDocument, DocumentDto?>(query, cancellationToken));

            await app.UseCoreAsync();
        }
    }
}
