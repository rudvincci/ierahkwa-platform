using Grpc.Core;
using Grpc.Net.Client;
using Mamey.FWID.Notifications.Api.Protos;
using Mamey.FWID.Notifications.GrpcClient.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Mamey.FWID.Notifications.GrpcClient;

public static class Extensions
{
    private static NotificationService.NotificationServiceClient _client;

    public static async Task<IMameyBuilder> AddNotificationsGrpcInfrastructure(
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
        _client = new NotificationService.NotificationServiceClient(channel);
        
        // Register the client as a singleton for DI
        builder.Services.AddSingleton(_client);
        builder.Services.AddSingleton<NotificationService.NotificationServiceClient>(_client);
        builder.Services.AddScoped<Services.NotificationServiceClient>();
        
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
        ["1"] = GetNotificationsAsync,
        ["2"] = MarkAsReadAsync,
        ["3"] = SendNotificationAsync,
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
            port = 5007; // Notifications service default port
        }

        return $"https://{host}:{port}";
    }

    private static async Task InitAsync()
    {
        const string message = "\nOptions (1-3):" +
                               "\n1. Get notifications" +
                               "\n2. Mark as read" +
                               "\n3. Send notification" +
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

    private static async Task GetNotificationsAsync()
    {
        Console.Write("Type the identity id: ");
        var identityId = Console.ReadLine();

        Console.WriteLine("Sending the request...");
        
        var request = new GetNotificationsRequest
        {
            IdentityId = identityId ?? string.Empty
        };

        var callOptions = new CallOptions();
        var response = await _client.GetNotificationsAsync(request, callOptions);
        
        Console.WriteLine($"* Retrieved {response.Notifications.Count} notifications for identity: {identityId} *");
        DisplayResponse(response);
    }

    private static async Task MarkAsReadAsync()
    {
        Console.Write("Type the notification id: ");
        var notificationId = Console.ReadLine();
        Console.Write("Type the identity id: ");
        var identityId = Console.ReadLine();

        Console.WriteLine("Sending the request...");
        
        var request = new MarkAsReadRequest
        {
            NotificationId = notificationId ?? string.Empty,
            IdentityId = identityId ?? string.Empty
        };

        var callOptions = new CallOptions();
        var response = await _client.MarkAsReadAsync(request, callOptions);
        
        if (!response.Success)
        {
            Console.WriteLine($"* Failed to mark notification as read: {notificationId} *");
            Console.WriteLine($"Error: {response.Message}");
            return;
        }

        Console.WriteLine($"* Notification marked as read: {notificationId} *");
        DisplayResponse(response);
    }

    private static async Task SendNotificationAsync()
    {
        Console.Write("Type the identity id: ");
        var identityId = Console.ReadLine();
        Console.Write("Type the title: ");
        var title = Console.ReadLine();
        Console.Write("Type the description: ");
        var description = Console.ReadLine();
        Console.Write("Type the message: ");
        var message = Console.ReadLine();
        Console.Write("Type the notification type (Email, Sms, Push, InApp): ");
        var notificationType = Console.ReadLine();

        Console.WriteLine("Sending the request...");
        
        var request = new SendNotificationRequest
        {
            IdentityId = identityId ?? string.Empty,
            Title = title ?? string.Empty,
            Description = description ?? string.Empty,
            Message = message ?? string.Empty,
            NotificationType = notificationType ?? "InApp"
        };

        var callOptions = new CallOptions();
        var response = await _client.SendNotificationAsync(request, callOptions);
        
        if (!response.Success)
        {
            Console.WriteLine($"* Failed to send notification *");
            Console.WriteLine($"Error: {response.Message}");
            return;
        }

        Console.WriteLine($"* Notification sent successfully. NotificationId: {response.NotificationId} *");
        DisplayResponse(response);
    }

    private static void DisplayResponse<T>(T response)
        => Console.WriteLine(JsonConvert.SerializeObject(response, JsonSerializerSettings));
}







