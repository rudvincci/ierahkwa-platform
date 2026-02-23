
using Grpc.Core;
using Grpc.Net.Client;
using Mamey.FWID.Operations.GrpcClient.Protos;
using Mamey.FWID.Operations.GrpcClient.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Mamey.FWID.Operations.GrpcClient;

public static class Extensions
{
    private static GrpcOperationsService.GrpcOperationsServiceClient _client;

    public static async Task<IMameyBuilder> AddOperationsGrpcInfrastructure(
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

        var httpClient = new HttpClient(httpClientHandler);
        
        // Add JWT token to default request headers if provided
        if (!string.IsNullOrEmpty(jwtToken))
        {
            httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);
        }

        var channelOptions = new GrpcChannelOptions
        {
            HttpClient = httpClient
        };

        var channel = GrpcChannel.ForAddress(address, channelOptions);
        _client = new GrpcOperationsService.GrpcOperationsServiceClient(channel);
        
        // Register the client as a singleton for DI
        builder.Services.AddSingleton(_client);
        builder.Services.AddSingleton<GrpcOperationsService.GrpcOperationsServiceClient>(_client);
        builder.Services.AddScoped<Services.OperationsServiceClient>();
        
        Console.WriteLine($"Created a GRPC client for an address: '{address}'");
        
        // Run interactive mode if not in service mode
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Production")
        {
            _ = Task.Run(async () => await InitAsync());
        }
        
        return builder;
    }

    private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.Indented
    };

    private static readonly IDictionary<string, Func<Task>> Actions = new Dictionary<string, Func<Task>>
    {
        ["1"] = GetOperationAsync,
        ["2"] = SubscribeOperationsStreamAsync,
    };


    private static string GetAddress(string[] args)
    {
        var host = string.Empty;
        var port = 0;

        if (args?.Any() == true && args.Length >= 2)
        {
            host = args[0];
            if (int.TryParse(args[1], out var providedPort))
            {
                port = providedPort;
            }
        }

        if (string.IsNullOrWhiteSpace(host))
        {
            host = "localhost";
        }

        if (port <= 0)
        {
            port = 50050;
        }

        return $"https://{host}:{port}";
    }

    private static async Task InitAsync()
    {
        const string message = "\nOptions (1-2):" +
                               "\n1. Get the single operation by id" +
                               "\n2. Subscribe to the operations stream" +
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

    private static async Task GetOperationAsync()
    {
        Console.Write("Type the operation id: ");
        var id = Console.ReadLine();
        Console.WriteLine("Sending the request...");
        
        var request = new GetOperationRequest
        {
            Id = id ?? string.Empty
        };
        
        // Create call options with authentication headers if needed
        var callOptions = new CallOptions();
        var response = await _client.GetOperationAsync(request, callOptions);
        
        if (string.IsNullOrWhiteSpace(response.Id))
        {
            Console.WriteLine($"* Operation was not found for id: {id} *");
            return;
        }

        Console.WriteLine($"* Operation was found for id: {id} *");
        DisplayOperation(response);
    }

    private static async Task SubscribeOperationsStreamAsync()
    {
        Console.WriteLine("Subscribing to the operations stream...");
        
        // Create call options with authentication headers if needed
        var callOptions = new CallOptions();
        using (var stream = _client.SubscribeOperations(new Empty(), callOptions))
        {
            while (await stream.ResponseStream.MoveNext())
            {
                Console.WriteLine("* Received the data from the operations stream *");
                DisplayOperation(stream.ResponseStream.Current);
            }
        }
    }

    private static void DisplayOperation(GetOperationResponse response)
        => Console.WriteLine(JsonConvert.SerializeObject(response, JsonSerializerSettings));

    
}



