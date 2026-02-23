using Adobe.PDFServicesSDK;
using Adobe.PDFServicesSDK.auth;
//using ExecutionContext = Adobe.PDFServicesSDK.ExecutionContext;

namespace Mamey.Adobe;

public class PDFServiceFactory
{
    private readonly AdobePdfServicesOptions _options;

    public static PDFServices GeneratePDFService(string clientId, string clientSecret)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            throw new ArgumentException($"'{nameof(clientId)}' cannot be null or empty.", nameof(clientId));
        }

        if (string.IsNullOrEmpty(clientSecret))
        {
            throw new ArgumentException($"'{nameof(clientSecret)}' cannot be null or empty.", nameof(clientSecret));
        }

        return new PDFServices(new ServicePrincipalCredentials(clientId, clientSecret));
    }
}
