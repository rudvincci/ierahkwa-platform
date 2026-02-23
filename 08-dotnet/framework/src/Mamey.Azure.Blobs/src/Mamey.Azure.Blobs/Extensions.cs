using Azure.Storage.Blobs;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.Azure.Blobs;

public static class Extensions
{
    public static IMameyBuilder AddAzureBlobs(this IMameyBuilder builder)
    {
        var azureBlobOptions = builder.Services.GetOptions<AzureBlobOptions>("azureStorage");
        if (string.IsNullOrEmpty(azureBlobOptions.ConnectionString))
        {
            throw new ApplicationException("Azure Blob connection string empty. Check appsettings.json");
        }
        builder.Services.AddSingleton(azureBlobOptions);
        builder.Services.AddSingleton(new BlobServiceClient(azureBlobOptions.ConnectionString));
        builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
        return builder; 
    }
    
}
