// ============================================================================
// Mamey.FWID.Identities.GrpcClient - gRPC Client Library
// ============================================================================
// This library provides a client wrapper for making gRPC calls to the
// Mamey.FWID.Identities.Api service. It should be referenced by other
// microservices or applications that need to interact with the Identities
// service via gRPC.
//
// Usage:
//   1. Add a project reference to Mamey.FWID.Identities.GrpcClient
//   2. Call AddIdentitiesGrpcInfrastructure() in your Program.cs
//   3. Inject BiometricServiceClient where needed
//
// See README.md for detailed usage instructions.
// ============================================================================

using Grpc.Core;
using Grpc.Net.Client;
using Google.Protobuf;
using Mamey.FWID.Identities.Api.Protos;
using Mamey.FWID.Identities.GrpcClient.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Mamey.FWID.Identities.GrpcClient;

/// <summary>
/// Extension methods for registering the Identities gRPC client infrastructure.
/// </summary>
public static class Extensions
{
    private static BiometricService.BiometricServiceClient? _client;

    public static IMameyBuilder AddIdentitiesGrpcInfrastructure(
        this IMameyBuilder builder, 
        string[] args,
        string? jwtToken = null,
        X509Certificate2? clientCertificate = null)
    {
        var address = GetAddress(args);

        // Configure HttpClientHandler for certificate authentication
        var httpClientHandler = new HttpClientHandler();
        
        if (clientCertificate != null)
        {
            httpClientHandler.ClientCertificates.Add(clientCertificate);
        }
        
        // Only for local development purposes - accept any server certificate
        httpClientHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

        var httpClient = new HttpClient(httpClientHandler)
        {
            // Set timeout to prevent hanging on connection attempts
            Timeout = TimeSpan.FromSeconds(30)
        };
        
        // Add JWT token to default request headers if provided
        if (!string.IsNullOrEmpty(jwtToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
        }

        var channelOptions = new GrpcChannelOptions
        {
            HttpClient = httpClient,
            // Set max receive message size (default is 4MB)
            MaxReceiveMessageSize = 4 * 1024 * 1024,
            // Set max send message size (default is 4MB)
            MaxSendMessageSize = 4 * 1024 * 1024,
            // Disable connection validation on channel creation to prevent blocking
            ThrowOperationCanceledOnCancellation = true
        };

        // Create channel lazily - GrpcChannel.ForAddress doesn't connect immediately
        // but we'll create it here and register it as a factory to avoid blocking
        try
        {
            var channel = GrpcChannel.ForAddress(address, channelOptions);
            _client = new BiometricService.BiometricServiceClient(channel);
            
            // Register the channel and client as singletons
            builder.Services.AddSingleton(channel);
            builder.Services.AddSingleton(_client);
            builder.Services.AddSingleton<BiometricService.BiometricServiceClient>(_client);
            
            // Register BiometricServiceClient only if client was successfully created
            builder.Services.AddScoped<Services.BiometricServiceClient>(sp =>
            {
                var client = sp.GetRequiredService<BiometricService.BiometricServiceClient>();
                var logger = sp.GetRequiredService<ILogger<Services.BiometricServiceClient>>();
                return new Services.BiometricServiceClient(client, logger);
            });
            
            Console.WriteLine($"Registered GRPC client for address: '{address}' (connection will be established on first use)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to create gRPC channel for address '{address}': {ex.Message}");
            Console.WriteLine("gRPC client will not be available. The service may not be running or accessible.");
            // Don't throw - allow the application to start even if gRPC client can't be created
            // Register a null client factory that will throw a helpful exception when used
            builder.Services.AddScoped<Services.BiometricServiceClient>(sp =>
            {
                throw new InvalidOperationException(
                    $"gRPC client is not available. Failed to create channel for address '{address}'. " +
                    $"Original error: {ex.Message}. Ensure the gRPC service is running and accessible.");
            });
        }
        
        // Note: InitAsync() is for console applications only, not web applications
        // It should not be called in a web app context as it uses Console.ReadLine()
        
        return builder;
    }

    private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.Indented
    };

    private static readonly IDictionary<string, Func<Task>> Actions = new Dictionary<string, Func<Task>>
    {
        ["1"] = VerifyBiometricAsync,
        ["2"] = VerifyBiometricStreamAsync,
    };

    private static string GetAddress(string[] args)
    {
        var host = string.Empty;
        var port = 0;

        // First, try to get from command-line arguments
        if (args?.Any() == true && args.Length >= 2)
        {
            host = args[0];
            if (int.TryParse(args[1], out var providedPort))
            {
                port = providedPort;
            }
        }

        // Then, try environment variables
        if (string.IsNullOrWhiteSpace(host))
        {
            host = Environment.GetEnvironmentVariable("IDENTITIES_SERVICE_HOST") 
                ?? Environment.GetEnvironmentVariable("IDENTITIES_HOST")
                ?? "localhost";
        }

        if (port <= 0)
        {
            var portEnv = Environment.GetEnvironmentVariable("IDENTITIES_SERVICE_PORT")
                ?? Environment.GetEnvironmentVariable("IDENTITIES_PORT");
            port = int.TryParse(portEnv, out var envPort) ? envPort : 5001; // Identities service default port
        }

        return $"https://{host}:{port}";
    }

    private static async Task InitAsync()
    {
        const string message = "\nOptions (1-2):" +
                               "\n1. Verify biometric" +
                               "\n2. Verify biometric stream" +
                               "\nType 'q' to quit.\n";

        var option = string.Empty;
        while (option != "q")
        {
            Console.WriteLine(message);
            Console.Write("> ");
            option = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(option))
            {
                Console.WriteLine("Missing option");
                continue;
            }

            Console.WriteLine();
            if (Actions.ContainsKey(option))
            {
                try
                {
                    await Actions[option]();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                continue;
            }

            Console.WriteLine($"Invalid option: {option}");
        }
    }

    private static async Task VerifyBiometricAsync()
    {
        if (_client == null)
        {
            Console.WriteLine("Error: gRPC client not initialized");
            return;
        }

        Console.Write("Type the identity id: ");
        var identityId = Console.ReadLine();
        Console.Write("Type the biometric type (Fingerprint, Face, Iris): ");
        var biometricType = Console.ReadLine();
        Console.Write("Type the biometric data (base64): ");
        var biometricData = Console.ReadLine();
        Console.Write("Type the threshold (0.0-1.0, default 0.8): ");
        var thresholdInput = Console.ReadLine();
        var threshold = double.TryParse(thresholdInput, out var t) ? t : 0.8;

        Console.WriteLine("Sending the request...");
        
        var request = new VerifyBiometricRequest
        {
            IdentityId = identityId ?? string.Empty,
            Threshold = threshold,
            ProvidedBiometric = new BiometricDataMessage
            {
                Type = biometricType ?? "Fingerprint",
                Data = string.IsNullOrEmpty(biometricData) 
                    ? ByteString.Empty 
                    : ByteString.CopyFrom(Convert.FromBase64String(biometricData))
            }
        };

        var callOptions = new CallOptions();
        var response = await _client.VerifyBiometricAsync(request, callOptions);
        
        if (!response.IsVerified)
        {
            Console.WriteLine($"* Biometric verification failed for identity: {identityId} *");
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                Console.WriteLine($"Error: {response.ErrorMessage}");
            }
            return;
        }

        Console.WriteLine($"* Biometric verified for identity: {identityId} *");
        Console.WriteLine($"Match Score: {response.MatchScore:F2}");
        Console.WriteLine($"Threshold: {response.Threshold:F2}");
        DisplayResponse(response);
    }

    private static async Task VerifyBiometricStreamAsync()
    {
        if (_client == null)
        {
            Console.WriteLine("Error: gRPC client not initialized");
            return;
        }

        Console.WriteLine("Subscribing to the biometric verification stream...");
        using (var stream = _client.VerifyBiometricStream())
        {
            // Send test request
            var request = new VerifyBiometricRequest
            {
                IdentityId = Guid.NewGuid().ToString(),
                Threshold = 0.8,
                ProvidedBiometric = new BiometricDataMessage
                {
                    Type = "Fingerprint",
                    Data = ByteString.Empty
                }
            };
            
            await stream.RequestStream.WriteAsync(request);
            await stream.RequestStream.CompleteAsync();

            while (await stream.ResponseStream.MoveNext())
            {
                Console.WriteLine("* Received the data from the biometric verification stream *");
                DisplayResponse(stream.ResponseStream.Current);
            }
        }
    }

    private static void DisplayResponse(VerifyBiometricResponse response)
        => Console.WriteLine(JsonConvert.SerializeObject(response, JsonSerializerSettings));
}

