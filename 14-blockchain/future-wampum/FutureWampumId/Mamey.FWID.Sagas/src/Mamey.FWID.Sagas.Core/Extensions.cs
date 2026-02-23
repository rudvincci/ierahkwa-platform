using System.Runtime.CompilerServices;
using Mamey.FWID.Sagas.Core.Exceptions;
using Mamey.WebApi;
using Mamey.MessageBrokers.RabbitMQ;
using Microsoft.AspNetCore.Builder;
using Mamey.Microservice.Infrastructure;
using Mamey.WebApi.Swagger;
using Microsoft.Extensions.Hosting;
using Mamey.WebApi.CQRS;
using Chronicle;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Chronicle.Persistance.Mongo;

[assembly: InternalsVisibleTo("Mamey.FWID.Sagas.Api")]
namespace Mamey.FWID.Sagas.Core;

internal static class Extensions
{
    public static IMameyBuilder AddFWIDSagaCore(this IMameyBuilder builder)
    {
        var mongoOptions = builder.Services.GetOptions<ChronicleMongoSettings>("mongo");
        builder.Services.AddChronicle(config =>
        {
           config.UseMongoPersistence(mongoOptions);
        });
        builder
                .AddErrorHandler<ExceptionToResponseMapper>()
                .AddWebApiSwaggerDocs()
                .AddSagaInfrastructure();
        return builder;
    }

    public static IApplicationBuilder UseFWIDSagaCore(this IApplicationBuilder app)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var isDevelopment = environment == Environments.Development.ToLower() || environment == "local";
        if(isDevelopment)
        {

        }
        var coordinator = app.ApplicationServices.GetService<ISagaCoordinator>();

        var context = SagaContext
            .Create()
            .WithSagaId(SagaId.NewSagaId())
            .Build();

        app.UseHttpsRedirection();

        app.UseSagaSharedInfrastructure()
            .UseRabbitMq()
            .AddRabbitMQCommandSubscriptions()
            .AddRabbitMQEventSubscriptions();
        return app;
    }
}



