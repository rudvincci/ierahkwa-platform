using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Mamey.Authentik.IntegrationTests.TestFixtures;

/// <summary>
/// Test fixture for Authentik integration tests.
/// Connects to the running Authentik Docker container.
/// </summary>
public class AuthentikTestFixture : IAsyncLifetime
{
    private readonly ILogger<AuthentikTestFixture> _logger;
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Gets the Authentik base URL from environment or running container.
    /// </summary>
    public string BaseUrl { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the API token from environment or configuration.
    /// </summary>
    public string? ApiToken { get; private set; }

    /// <summary>
    /// Gets whether the Authentik container is running and accessible.
    /// </summary>
    public bool IsContainerRunning { get; private set; }

    public AuthentikTestFixture()
    {
        _httpClient = new HttpClient();
        _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AuthentikTestFixture>();
    }

    public async Task InitializeAsync()
    {
        // Get configuration from environment variables
        // If not set, tests will skip gracefully (CI/CD friendly)
        BaseUrl = Environment.GetEnvironmentVariable("AUTHENTIK_BASE_URL") ?? string.Empty;
        ApiToken = Environment.GetEnvironmentVariable("AUTHENTIK_API_TOKEN");

        // If AUTHENTIK_BASE_URL is not set, skip all tests gracefully
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            _logger.LogInformation(
                "AUTHENTIK_BASE_URL environment variable is not set. " +
                "Integration tests will be skipped. " +
                "Set AUTHENTIK_BASE_URL to run integration tests (e.g., http://localhost:9100)");
            IsContainerRunning = false;
            return;
        }

        // For local development, optionally check if a specific container is running
        // This is informational only - tests will work with any Authentik instance
        var checkLocalContainer = Environment.GetEnvironmentVariable("AUTHENTIK_CHECK_LOCAL_CONTAINER") == "true";
        if (checkLocalContainer)
        {
            var localContainerRunning = await CheckContainerRunningAsync();
            if (localContainerRunning)
            {
                _logger.LogInformation("Local Authentik container detected");
            }
        }

        // Verify instance accessibility
        IsContainerRunning = await CheckContainerAccessibleAsync();

        if (!IsContainerRunning)
        {
            _logger.LogWarning(
                "Authentik instance is not accessible at {BaseUrl}. " +
                "Integration tests will be skipped. " +
                "Ensure AUTHENTIK_BASE_URL points to a running Authentik instance.",
                BaseUrl);
            return;
        }

        _logger.LogInformation("Authentik instance is accessible at {BaseUrl}", BaseUrl);

        // If no API token is provided, log a warning
        if (string.IsNullOrWhiteSpace(ApiToken))
        {
            _logger.LogWarning(
                "AUTHENTIK_API_TOKEN environment variable is not set. " +
                "Integration tests that require authentication will be skipped. " +
                "Get a token from: {BaseUrl}/if/admin/",
                BaseUrl);
        }
        else
        {
            _logger.LogInformation("API token is configured (length: {Length})", ApiToken.Length);
        }
    }

    public async Task DisposeAsync()
    {
        _httpClient?.Dispose();
        await Task.CompletedTask;
    }

    /// <summary>
    /// Checks if a local Authentik Docker container is running (optional, for local development only).
    /// </summary>
    private async Task<bool> CheckContainerRunningAsync()
    {
        // Only check for local container if explicitly requested
        var containerName = Environment.GetEnvironmentVariable("AUTHENTIK_CONTAINER_NAME") ?? "mamey-authentik-server";
        
        try
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"ps --filter name={containerName} --format {{{{.Status}}}}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null)
            {
                return false;
            }

            var output = await process.StandardOutput.ReadToEndAsync();
            await process.WaitForExitAsync();

            // Check if container is running (status contains "Up")
            return output.Contains("Up", StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to check if Docker container is running (this is OK in CI/CD)");
            return false;
        }
    }

    /// <summary>
    /// Checks if the Authentik instance is accessible via HTTP.
    /// </summary>
    private async Task<bool> CheckContainerAccessibleAsync()
    {
        try
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(5);
            var response = await _httpClient.GetAsync($"{BaseUrl.TrimEnd('/')}/api/v3/");
            
            // Any response (even 401/403) means the instance is accessible
            return response != null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to access Authentik instance at {BaseUrl}", BaseUrl);
            return false;
        }
    }

    /// <summary>
    /// Gets the Authentik version from the running instance.
    /// </summary>
    public async Task<string?> GetVersionAsync()
    {
        if (!IsContainerRunning)
        {
            return null;
        }

        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl.TrimEnd('/')}/api/v3/");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // Try to parse version from response if available
                return "Unknown";
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get Authentik version");
        }

        return null;
    }
}
