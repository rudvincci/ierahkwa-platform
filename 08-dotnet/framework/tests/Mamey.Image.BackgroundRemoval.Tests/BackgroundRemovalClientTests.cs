using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;
using Mamey.Image.BackgroundRemoval;
using Mamey.Image.BackgroundRemoval.Models;

namespace Mamey.Image.BackgroundRemoval.Tests;

public class BackgroundRemovalClientTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _httpClient;
    private readonly Mock<IOptions<BackgroundRemovalOptions>> _mockOptions;
    private readonly Mock<ILogger<BackgroundRemovalClient>> _mockLogger;
    private readonly BackgroundRemovalOptions _options;

    public BackgroundRemovalClientTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _mockLogger = new Mock<ILogger<BackgroundRemovalClient>>();
        
        _options = new BackgroundRemovalOptions
        {
            BaseUrl = "http://localhost:5000",
            TimeoutSeconds = 30,
            MaxFileSizeBytes = 10 * 1024 * 1024,
            MaxBatchSize = 10
        };

        _mockOptions = new Mock<IOptions<BackgroundRemovalOptions>>();
        _mockOptions.Setup(x => x.Value).Returns(_options);
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldInitialize()
    {
        // Act
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BackgroundRemovalClient(
            _httpClient,
            null!,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullHttpClient_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BackgroundRemovalClient(
            null!,
            _mockOptions.Object,
            _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            null!));
    }

    [Fact]
    public async Task RemoveBackgroundFromBytesAsync_WithValidBytes_ShouldReturnBytes()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        var inputBytes = Encoding.UTF8.GetBytes("test image data");
        var expectedResponse = Encoding.UTF8.GetBytes("processed image data");

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(expectedResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.RemoveBackgroundFromBytesAsync(inputBytes);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedResponse, result);
    }

    [Fact]
    public async Task RemoveBackgroundFromBytesAsync_WithNullBytes_ShouldThrowArgumentException()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.RemoveBackgroundFromBytesAsync(null!));
    }

    [Fact]
    public async Task RemoveBackgroundFromBytesAsync_WithEmptyBytes_ShouldThrowArgumentException()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.RemoveBackgroundFromBytesAsync(Array.Empty<byte>()));
    }

    [Fact]
    public async Task RemoveBackgroundFromBytesAsync_WithLargeBytes_ShouldThrowArgumentException()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        var largeBytes = new byte[_options.MaxFileSizeBytes + 1];

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.RemoveBackgroundFromBytesAsync(largeBytes));
    }

    [Fact]
    public async Task RemoveBackgroundAsync_WithValidStream_ShouldReturnStream()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("test image data"));
        var expectedResponse = new MemoryStream(Encoding.UTF8.GetBytes("processed image data"));

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(expectedResponse)
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.RemoveBackgroundAsync(inputStream);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task RemoveBackgroundAsync_WithNullStream_ShouldThrowArgumentException()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => client.RemoveBackgroundAsync(null!));
    }

    [Fact]
    public async Task RemoveBackgroundFromFileAsync_WithValidFile_ShouldReturnOutputPath()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        var inputPath = "test_input.png";
        var expectedOutputPath = "test_input_no_bg.png";

        // Create a temporary file for testing
        File.WriteAllBytes(inputPath, Encoding.UTF8.GetBytes("test image data"));

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes("processed image data")))
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        try
        {
            // Act
            var result = await client.RemoveBackgroundFromFileAsync(inputPath);

            // Assert
            Assert.Equal(expectedOutputPath, result);
            Assert.True(File.Exists(result));
        }
        finally
        {
            // Cleanup
            if (File.Exists(inputPath)) File.Delete(inputPath);
            if (File.Exists(expectedOutputPath)) File.Delete(expectedOutputPath);
        }
    }

    [Fact]
    public async Task GetAvailableModelsAsync_WithValidResponse_ShouldReturnModels()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        var expectedModels = new[] { "u2net", "u2net_human_seg" };
        var modelsResponse = new ModelsResponse { Models = expectedModels, CurrentModel = "u2net" };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(modelsResponse), Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.GetAvailableModelsAsync();

        // Assert
        Assert.Equal(expectedModels, result);
    }

    [Fact]
    public async Task IsHealthyAsync_WithHealthyService_ShouldReturnTrue()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        var healthResponse = new HealthCheckResponse { Status = "healthy", Service = "test" };

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(healthResponse), Encoding.UTF8, "application/json")
        };

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.IsHealthyAsync();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsHealthyAsync_WithUnhealthyService_ShouldReturnFalse()
    {
        // Arrange
        var client = new BackgroundRemovalClient(
            _httpClient,
            _mockOptions.Object,
            _mockLogger.Object);

        var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        // Act
        var result = await client.IsHealthyAsync();

        // Assert
        Assert.False(result);
    }
}