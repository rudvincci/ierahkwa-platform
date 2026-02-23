using Mamey.Docs.Swagger;
using Mamey.WebApi.Swagger.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.WebApi.Swagger;

public static class Extensions
{
    private const string SectionName = "swagger";

    public static IMameyBuilder AddWebApiSwaggerDocs(this IMameyBuilder builder, string sectionName = SectionName)
    {
        if (string.IsNullOrWhiteSpace(sectionName))
        {
            sectionName = SectionName;
        }

        return builder.AddWebApiSwaggerDocs(b => b.AddSwaggerDocs(sectionName));
    }
        
    public static IMameyBuilder AddWebApiSwaggerDocs(this IMameyBuilder builder, 
        Func<ISwaggerOptionsBuilder, ISwaggerOptionsBuilder> buildOptions)
        => builder.AddWebApiSwaggerDocs(b => b.AddSwaggerDocs(buildOptions));
        
    public static IMameyBuilder AddWebApiSwaggerDocs(this IMameyBuilder builder, SwaggerOptions options)
        => builder.AddWebApiSwaggerDocs(b => b.AddSwaggerDocs(options));
        
    private static IMameyBuilder AddWebApiSwaggerDocs(this IMameyBuilder builder, Action<IMameyBuilder> registerSwagger)
    {
        registerSwagger(builder);
        builder.Services.AddSwaggerGen(c => c.DocumentFilter<WebApiDocumentFilter>());
        return builder;
    }
}