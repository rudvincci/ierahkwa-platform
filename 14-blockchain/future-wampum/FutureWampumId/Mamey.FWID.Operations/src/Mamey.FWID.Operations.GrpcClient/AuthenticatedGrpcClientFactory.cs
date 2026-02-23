using Grpc.Core;
using Grpc.Net.Client;
using Mamey.FWID.Operations.GrpcClient.Protos;
using System.Security.Cryptography.X509Certificates;

namespace Mamey.FWID.Operations.GrpcClient;

/// <summary>
/// Factory for creating authenticated gRPC clients with JWT and/or certificate authentication.
/// </summary>
public class AuthenticatedGrpcClientFactory
{
    /// <summary>
    /// Creates an authenticated gRPC client with JWT authentication.
    /// </summary>
    /// <param name="address">The gRPC server address.</param>
    /// <param name="jwtToken">The JWT token for authentication.</param>
    /// <returns>An authenticated gRPC client.</returns>
    public static GrpcOperationsService.GrpcOperationsServiceClient CreateJwtAuthenticatedClient(
        string address, 
        string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
            throw new ArgumentException("JWT token cannot be null or empty.", nameof(jwtToken));

        var httpClientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var httpClient = new HttpClient(httpClientHandler);
        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

        var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });

        return new GrpcOperationsService.GrpcOperationsServiceClient(channel);
    }

    /// <summary>
    /// Creates an authenticated gRPC client with certificate authentication.
    /// </summary>
    /// <param name="address">The gRPC server address.</param>
    /// <param name="clientCertificate">The client certificate for authentication.</param>
    /// <returns>An authenticated gRPC client.</returns>
    public static GrpcOperationsService.GrpcOperationsServiceClient CreateCertificateAuthenticatedClient(
        string address, 
        X509Certificate2 clientCertificate)
    {
        if (clientCertificate == null)
            throw new ArgumentNullException(nameof(clientCertificate));

        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ClientCertificates.Add(clientCertificate);
        httpClientHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var httpClient = new HttpClient(httpClientHandler);

        var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });

        return new GrpcOperationsService.GrpcOperationsServiceClient(channel);
    }

    /// <summary>
    /// Creates an authenticated gRPC client with both JWT and certificate authentication.
    /// </summary>
    /// <param name="address">The gRPC server address.</param>
    /// <param name="jwtToken">The JWT token for authentication.</param>
    /// <param name="clientCertificate">The client certificate for authentication.</param>
    /// <returns>An authenticated gRPC client.</returns>
    public static GrpcOperationsService.GrpcOperationsServiceClient CreateDualAuthenticatedClient(
        string address, 
        string jwtToken, 
        X509Certificate2 clientCertificate)
    {
        if (string.IsNullOrEmpty(jwtToken))
            throw new ArgumentException("JWT token cannot be null or empty.", nameof(jwtToken));
        if (clientCertificate == null)
            throw new ArgumentNullException(nameof(clientCertificate));

        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.ClientCertificates.Add(clientCertificate);
        httpClientHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var httpClient = new HttpClient(httpClientHandler);
        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

        var channel = GrpcChannel.ForAddress(address, new GrpcChannelOptions
        {
            HttpClient = httpClient
        });

        return new GrpcOperationsService.GrpcOperationsServiceClient(channel);
    }

    /// <summary>
    /// Creates call options with JWT token in metadata headers.
    /// </summary>
    /// <param name="jwtToken">The JWT token.</param>
    /// <returns>Call options with JWT authentication headers.</returns>
    public static CallOptions CreateJwtCallOptions(string jwtToken)
    {
        if (string.IsNullOrEmpty(jwtToken))
            throw new ArgumentException("JWT token cannot be null or empty.", nameof(jwtToken));

        var headers = new Metadata
        {
            { "Authorization", $"Bearer {jwtToken}" }
        };

        return new CallOptions(headers: headers);
    }
}


