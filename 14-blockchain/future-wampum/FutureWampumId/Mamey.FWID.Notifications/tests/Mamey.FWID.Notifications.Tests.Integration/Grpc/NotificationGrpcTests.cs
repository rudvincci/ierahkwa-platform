using Xunit;
using Grpc.Net.Client;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Security.Cryptography.X509Certificates;
using Mamey.FWID.Notifications.Api.Protos;

namespace Mamey.FWID.Notifications.Tests.Integration.Grpc;

/// <summary>
/// Integration tests for Notification gRPC service with authentication
/// </summary>
public class NotificationGrpcTests : IClassFixture<GrpcTestFixture>
{
    private readonly GrpcTestFixture _fixture;
    private readonly NotificationService.NotificationServiceClient _client;
    private readonly HttpClient _httpClient;

    public NotificationGrpcTests(GrpcTestFixture fixture)
    {
        _fixture = fixture;
        var channel = GrpcChannel.ForAddress("http://localhost:5007");
        _client = new NotificationService.NotificationServiceClient(channel);
        _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5007") };
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Type", "gRPC")]
    [Trait("Auth", "JWT")]
    public async Task GetNotifications_WithValidJWT_ReturnsSuccess()
    {
        // Arrange
        var token = await _fixture.GetJwtTokenAsync();
        var headers = new Metadata { { "authorization", $"Bearer {token}" } };
        var request = new GetNotificationsRequest
        {
            IdentityId = Guid.NewGuid().ToString()
        };

        // Act
        var response = await _client.GetNotificationsAsync(
            request,
            new CallOptions(headers));

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Notifications);
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Type", "gRPC")]
    [Trait("Auth", "JWT")]
    public async Task GetNotifications_WithoutJWT_ReturnsUnauthenticated()
    {
        // Arrange
        var request = new GetNotificationsRequest
        {
            IdentityId = Guid.NewGuid().ToString()
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(
            () => _client.GetNotificationsAsync(request).ResponseAsync);

        Assert.Equal(StatusCode.Unauthenticated, ex.StatusCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Type", "gRPC")]
    [Trait("Auth", "JWT")]
    public async Task GetNotifications_WithExpiredJWT_ReturnsUnauthenticated()
    {
        // Arrange
        var expiredToken = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2MDAwMDAwMDB9.expired";
        var headers = new Metadata { { "authorization", $"Bearer {expiredToken}" } };
        var request = new GetNotificationsRequest
        {
            IdentityId = Guid.NewGuid().ToString()
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(
            () => _client.GetNotificationsAsync(request, new CallOptions(headers)).ResponseAsync);

        Assert.Equal(StatusCode.Unauthenticated, ex.StatusCode);
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Type", "gRPC")]
    [Trait("Auth", "Certificate")]
    public async Task GetNotifications_WithValidCertificate_ReturnsSuccess()
    {
        // Arrange
        var certificate = await _fixture.GetClientCertificateAsync();
        if (certificate == null)
        {
            // Skip test if certificate not available
            return;
        }

        var certBase64 = Convert.ToBase64String(certificate.GetRawCertData());
        var headers = new Metadata { { "x-client-certificate", certBase64 } };
        var request = new GetNotificationsRequest
        {
            IdentityId = Guid.NewGuid().ToString()
        };

        // Act
        var response = await _client.GetNotificationsAsync(
            request,
            new CallOptions(headers));

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.Notifications);
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Type", "gRPC")]
    [Trait("Auth", "Certificate")]
    public async Task GetNotifications_WithInvalidCertificate_ReturnsUnauthenticated()
    {
        // Arrange
        var invalidCert = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("invalid-cert"));
        var headers = new Metadata { { "x-client-certificate", invalidCert } };
        var request = new GetNotificationsRequest
        {
            IdentityId = Guid.NewGuid().ToString()
        };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<RpcException>(
            () => _client.GetNotificationsAsync(request, new CallOptions(headers)).ResponseAsync);

        Assert.True(ex.StatusCode == StatusCode.Unauthenticated || ex.StatusCode == StatusCode.PermissionDenied);
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Type", "gRPC")]
    [Trait("Auth", "JWT")]
    public async Task MarkAsRead_WithValidJWT_ReturnsSuccess()
    {
        // Arrange
        var token = await _fixture.GetJwtTokenAsync();
        var headers = new Metadata { { "authorization", $"Bearer {token}" } };
        var request = new MarkAsReadRequest
        {
            NotificationId = Guid.NewGuid().ToString(),
            IdentityId = Guid.NewGuid().ToString()
        };

        // Act
        var response = await _client.MarkAsReadAsync(
            request,
            new CallOptions(headers));

        // Assert
        Assert.NotNull(response);
        // Note: Actual response validation depends on service implementation
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Trait("Type", "gRPC")]
    [Trait("Auth", "JWT")]
    public async Task SendNotification_WithValidJWT_ReturnsSuccess()
    {
        // Arrange
        var token = await _fixture.GetJwtTokenAsync();
        var headers = new Metadata { { "authorization", $"Bearer {token}" } };
        var request = new SendNotificationRequest
        {
            IdentityId = Guid.NewGuid().ToString(),
            Title = "Test Notification",
            Description = "Test Description",
            Message = "Test Message",
            NotificationType = "InApp"
        };

        // Act
        var response = await _client.SendNotificationAsync(
            request,
            new CallOptions(headers));

        // Assert
        Assert.NotNull(response);
        Assert.True(response.Success);
        Assert.NotEmpty(response.NotificationId);
    }
}

/// <summary>
/// Test fixture for gRPC integration tests
/// </summary>
public class GrpcTestFixture : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private string? _cachedToken;

    public GrpcTestFixture()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5007")
        };

        _configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public async Task<string> GetJwtTokenAsync()
    {
        if (!string.IsNullOrEmpty(_cachedToken))
        {
            return _cachedToken;
        }

        try
        {
            // Attempt to login via REST API
            var loginRequest = new
            {
                username = _configuration["Test:Username"] ?? "test",
                password = _configuration["Test:Password"] ?? "test"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                _cachedToken = loginResponse?.AccessToken;
                return _cachedToken ?? throw new InvalidOperationException("Failed to get JWT token");
            }
        }
        catch (Exception ex)
        {
            // If login fails, return a test token (tests will fail with Unauthenticated)
            Console.WriteLine($"Warning: Failed to get JWT token: {ex.Message}");
        }

        throw new InvalidOperationException("JWT token not available. Ensure service is running and credentials are correct.");
    }

    public async Task<X509Certificate2?> GetClientCertificateAsync()
    {
        try
        {
            var certPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", "..", "..", "..", "certs", "client.pfx");

            if (File.Exists(certPath))
            {
                var password = _configuration["Test:CertificatePassword"] ?? "password";
                return new X509Certificate2(certPath, password);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Warning: Failed to load certificate: {ex.Message}");
        }

        return null;
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}

internal class LoginResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? Expires { get; set; }
}







