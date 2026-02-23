using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.EF.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Mongo.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Redis.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Composite;

internal static class Extensions
{
    public static IServiceCollection AddCompositeRepositories(this IServiceCollection services)
    {
        // Application repositories
        services.AddScoped<ApplicationPostgresRepository>();
        services.AddScoped<ApplicationMongoRepository>();
        services.AddScoped<ApplicationRedisRepository>();
        services.AddScoped<IApplicationRepository, CompositeApplicationRepository>();
        
        // Document repositories
        services.AddScoped<EF.Repositories.UploadedDocumentPostgresRepository>();
        services.AddScoped<Mongo.Repositories.UploadedDocumentMongoRepository>();
        services.AddScoped<Redis.Repositories.UploadedDocumentRedisRepository>();
        services.AddScoped<Domain.Repositories.IUploadedDocumentRepository, CompositeUploadedDocumentRepository>();
        
        return services;
    }
}
