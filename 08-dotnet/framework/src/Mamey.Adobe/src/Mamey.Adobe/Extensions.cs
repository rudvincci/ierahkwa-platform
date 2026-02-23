using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Adobe;

public static class Extensions
{
    public static IMameyBuilder AddAdobePdf(this IMameyBuilder builder)
    {
        var options = builder.GetOptions<AdobePdfServicesOptions>("adobe");
        builder.Services.AddSingleton(options);
        builder.Services.AddScoped<IPdfServices, PdfServices>();
        return builder;
    }
    
}
