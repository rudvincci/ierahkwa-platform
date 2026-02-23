# Mamey.Image

Mamey.Image is a comprehensive .NET library that provides AI-powered image processing capabilities, including background removal through integration with the Mamey Image Background Removal service. Built on top of the Mamey.Http framework, it offers robust, production-ready image processing with full async support and dependency injection.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [Basic Usage](#basic-usage)
- [Advanced Usage](#advanced-usage)
- [Web API Integration](#web-api-integration)
- [Background Services](#background-services)
- [Error Handling](#error-handling)
- [Performance Optimization](#performance-optimization)
- [Testing](#testing)
- [API Reference](#api-reference)
- [Troubleshooting](#troubleshooting)

## Features

- **🤖 AI-Powered Background Removal**: Intelligent background detection while preserving foreground subjects
- **📦 Batch Processing**: Process multiple images efficiently
- **🎨 Multiple Output Formats**: PNG (with transparency) and JPEG support
- **🌐 HTTP Client Integration**: Built on Mamey.Http for robust API communication
- **💉 Dependency Injection**: Seamless integration with .NET DI container
- **⚡ Async/Await Support**: Full async support for all operations
- **🔧 Configurable**: Extensive configuration options
- **📊 Health Monitoring**: Built-in service health checks
- **🛡️ Error Handling**: Comprehensive error handling and retry policies
- **📈 Performance Optimized**: Efficient memory and network usage

## Installation

### Package Manager
```bash
Install-Package Mamey.Image
```

### .NET CLI
```bash
dotnet add package Mamey.Image
```

### PackageReference
```xml
<PackageReference Include="Mamey.Image" Version="2.0.5" />
```

### Prerequisites
- .NET 9.0 or later
- Mamey.Http package
- Background removal service running (Python service)

## Quick Start

### 1. Configure Services

```csharp
// Program.cs
using Mamey.Image;

var builder = WebApplication.CreateBuilder(args);

// Add background removal services
builder.Services.AddMameyImageBackgroundRemoval(builder.Configuration);

var app = builder.Build();
```

### 2. Basic Usage

```csharp
using Mamey.Image.BackgroundRemoval;

public class ImageController : ControllerBase
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;

    public ImageController(IBackgroundRemovalClient backgroundRemovalClient)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
    }

    [HttpPost("remove-background")]
    public async Task<IActionResult> RemoveBackground(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var result = await _backgroundRemovalClient.RemoveBackgroundAsync(stream, "PNG");
        
        return File(result, "image/png", $"no_bg_{file.FileName}");
    }
}
```

### 3. Configuration

```json
// appsettings.json
{
  "BackgroundRemovalApi": {
    "BaseUrl": "http://localhost:5000",
    "TimeoutSeconds": 300,
    "DefaultOutputFormat": "PNG"
  }
}
```

## Configuration

### appsettings.json Configuration

```json
{
  "BackgroundRemovalApi": {
    "BaseUrl": "http://localhost:5000",
    "TimeoutSeconds": 300,
    "MaxRetryAttempts": 3,
    "RetryDelayMs": 1000,
    "DefaultOutputFormat": "PNG",
    "MaxFileSizeBytes": 10485760,
    "MaxBatchSize": 10
  }
}
```

### Programmatic Configuration

```csharp
// Option 1: Using configuration section
builder.Services.AddMameyImageBackgroundRemoval(builder.Configuration);

// Option 2: Using lambda configuration
builder.Services.AddMameyImageBackgroundRemoval(options =>
{
    options.BaseUrl = "http://localhost:5000";
    options.TimeoutSeconds = 300;
    options.MaxRetryAttempts = 3;
    options.RetryDelayMs = 1000;
    options.DefaultOutputFormat = "PNG";
    options.MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB
    options.MaxBatchSize = 10;
});

// Option 3: Using IConfiguration
builder.Services.AddMameyImageBackgroundRemoval(
    builder.Configuration.GetSection("BackgroundRemovalApi")
);
```

### Environment-Specific Configuration

```json
// appsettings.Development.json
{
  "BackgroundRemovalApi": {
    "BaseUrl": "http://localhost:5000",
    "TimeoutSeconds": 60,
    "MaxRetryAttempts": 1
  }
}

// appsettings.Production.json
{
  "BackgroundRemovalApi": {
    "BaseUrl": "https://image-bg.mamey.io",
    "TimeoutSeconds": 300,
    "MaxRetryAttempts": 3,
    "RetryDelayMs": 2000
  }
}
```

## Basic Usage

### Single Image Processing

```csharp
using Mamey.Image.BackgroundRemoval;

public class ImageService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;

    public ImageService(IBackgroundRemovalClient backgroundRemovalClient)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
    }

    public async Task<string> RemoveBackgroundAsync(string inputPath, string outputPath)
    {
        return await _backgroundRemovalClient.RemoveBackgroundFromFileAsync(
            inputPath, 
            outputPath, 
            "PNG"
        );
    }

    public async Task<Stream> RemoveBackgroundAsync(Stream imageStream)
    {
        return await _backgroundRemovalClient.RemoveBackgroundAsync(
            imageStream, 
            "PNG"
        );
    }

    public async Task<byte[]> RemoveBackgroundAsync(byte[] imageBytes)
    {
        return await _backgroundRemovalClient.RemoveBackgroundFromBytesAsync(
            imageBytes, 
            "PNG"
        );
    }
}
```

### Raw Bytes Processing

```csharp
public class BytesImageService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;

    public BytesImageService(IBackgroundRemovalClient backgroundRemovalClient)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
    }

    public async Task<byte[]> ProcessImageBytesAsync(byte[] imageBytes)
    {
        return await _backgroundRemovalClient.RemoveBackgroundFromBytesAsync(
            imageBytes, 
            "PNG"
        );
    }

    public async Task<byte[]> ProcessImageFileAsync(string filePath)
    {
        // Read file as bytes
        byte[] imageBytes = await File.ReadAllBytesAsync(filePath);
        
        // Process bytes
        byte[] processedBytes = await _backgroundRemovalClient.RemoveBackgroundFromBytesAsync(
            imageBytes, 
            "PNG"
        );
        
        return processedBytes;
    }
}
```

### Batch Processing

```csharp
public class BatchImageService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;

    public BatchImageService(IBackgroundRemovalClient backgroundRemovalClient)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
    }

    public async Task<Stream> ProcessBatchAsync(string[] imagePaths)
    {
        return await _backgroundRemovalClient.RemoveBackgroundBatchAsync(
            imagePaths, 
            "PNG"
        );
    }

    public async Task ProcessDirectoryAsync(string inputDir, string outputDir)
    {
        var imageFiles = Directory.GetFiles(inputDir, "*.jpg")
            .Concat(Directory.GetFiles(inputDir, "*.png"))
            .ToArray();

        if (imageFiles.Length == 0)
        {
            Console.WriteLine("No image files found in input directory");
            return;
        }

        // Process in batches
        const int batchSize = 5;
        for (int i = 0; i < imageFiles.Length; i += batchSize)
        {
            var batch = imageFiles.Skip(i).Take(batchSize).ToArray();
            var zipStream = await _backgroundRemovalClient.RemoveBackgroundBatchAsync(batch, "PNG");
            
            // Extract ZIP to output directory
            using var zipArchive = new ZipArchive(zipStream);
            zipArchive.ExtractToDirectory(outputDir, true);
            
            Console.WriteLine($"Processed batch {i / batchSize + 1}");
        }
    }
}
```

### Health Monitoring

```csharp
public class HealthService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;

    public HealthService(IBackgroundRemovalClient backgroundRemovalClient)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
    }

    public async Task<bool> IsServiceHealthyAsync()
    {
        return await _backgroundRemovalClient.IsHealthyAsync();
    }

    public async Task<string[]> GetAvailableModelsAsync()
    {
        return await _backgroundRemovalClient.GetAvailableModelsAsync();
    }

    public async Task<ServiceStatus> GetServiceStatusAsync()
    {
        var isHealthy = await IsServiceHealthyAsync();
        var models = await GetAvailableModelsAsync();
        
        return new ServiceStatus
        {
            IsHealthy = isHealthy,
            AvailableModels = models,
            Timestamp = DateTime.UtcNow
        };
    }
}

public class ServiceStatus
{
    public bool IsHealthy { get; set; }
    public string[] AvailableModels { get; set; } = Array.Empty<string>();
    public DateTime Timestamp { get; set; }
}
```

## Advanced Usage

### Custom HTTP Client Configuration

```csharp
builder.Services.AddMameyImageBackgroundRemoval(builder.Configuration);

// Configure additional HTTP client settings
builder.Services.ConfigureHttpClientDefaults(httpClientBuilder =>
{
    httpClientBuilder.AddStandardResilienceHandler(options =>
    {
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromSeconds(1);
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromMinutes(2);
    });
});
```

### Custom Serialization

```csharp
public class CustomImageService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;
    private readonly ILogger<CustomImageService> _logger;

    public CustomImageService(
        IBackgroundRemovalClient backgroundRemovalClient,
        ILogger<CustomImageService> logger)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
        _logger = logger;
    }

    public async Task<ProcessedImageResult> ProcessImageWithMetadataAsync(
        string inputPath, 
        string outputPath,
        ImageMetadata metadata)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            _logger.LogInformation("Processing image: {InputPath}", inputPath);
            
            var resultPath = await _backgroundRemovalClient.RemoveBackgroundFromFileAsync(
                inputPath, 
                outputPath, 
                "PNG"
            );
            
            var processingTime = DateTime.UtcNow - startTime;
            
            return new ProcessedImageResult
            {
                InputPath = inputPath,
                OutputPath = resultPath,
                ProcessingTime = processingTime,
                Metadata = metadata,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process image: {InputPath}", inputPath);
            
            return new ProcessedImageResult
            {
                InputPath = inputPath,
                OutputPath = null,
                ProcessingTime = DateTime.UtcNow - startTime,
                Metadata = metadata,
                Success = false,
                Error = ex.Message
            };
        }
    }
}

public class ProcessedImageResult
{
    public string InputPath { get; set; } = string.Empty;
    public string? OutputPath { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public ImageMetadata Metadata { get; set; } = new();
    public bool Success { get; set; }
    public string? Error { get; set; }
}

public class ImageMetadata
{
    public string OriginalFileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    public string? ModelUsed { get; set; }
}
```

### Concurrent Processing

```csharp
public class ConcurrentImageService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;
    private readonly SemaphoreSlim _semaphore;

    public ConcurrentImageService(IBackgroundRemovalClient backgroundRemovalClient)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
        _semaphore = new SemaphoreSlim(5, 5); // Max 5 concurrent operations
    }

    public async Task<ProcessedImageResult[]> ProcessImagesConcurrentlyAsync(
        string[] imagePaths, 
        string outputDirectory)
    {
        var tasks = imagePaths.Select(path => ProcessSingleImageAsync(path, outputDirectory));
        return await Task.WhenAll(tasks);
    }

    private async Task<ProcessedImageResult> ProcessSingleImageAsync(
        string inputPath, 
        string outputDirectory)
    {
        await _semaphore.WaitAsync();
        
        try
        {
            var fileName = Path.GetFileNameWithoutExtension(inputPath);
            var outputPath = Path.Combine(outputDirectory, $"{fileName}_no_bg.png");
            
            var result = await _backgroundRemovalClient.RemoveBackgroundFromFileAsync(
                inputPath, 
                outputPath, 
                "PNG"
            );
            
            return new ProcessedImageResult
            {
                InputPath = inputPath,
                OutputPath = result,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ProcessedImageResult
            {
                InputPath = inputPath,
                Success = false,
                Error = ex.Message
            };
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

## Web API Integration

### ASP.NET Core Web API Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class ImagesController : ControllerBase
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;
    private readonly ILogger<ImagesController> _logger;

    public ImagesController(
        IBackgroundRemovalClient backgroundRemovalClient,
        ILogger<ImagesController> logger)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
        _logger = logger;
    }

    [HttpPost("remove-background")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<IActionResult> RemoveBackground(
        IFormFile file,
        [FromForm] string outputFormat = "PNG")
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "No file provided",
                Detail = "Please upload an image file"
            });
        }

        try
        {
            _logger.LogInformation("Processing image: {FileName}", file.FileName);
            
            using var stream = file.OpenReadStream();
            var result = await _backgroundRemovalClient.RemoveBackgroundAsync(stream, outputFormat);
            
            var contentType = outputFormat.ToLowerInvariant() switch
            {
                "png" => "image/png",
                "jpeg" => "image/jpeg",
                _ => "image/png"
            };
            
            return File(result, contentType, $"no_bg_{file.FileName}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing image: {FileName}", file.FileName);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Processing failed",
                Detail = ex.Message
            });
        }
    }

    [HttpPost("remove-background-batch")]
    [ProducesResponseType(typeof(FileResult), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    public async Task<IActionResult> RemoveBackgroundBatch(
        IFormFileCollection files,
        [FromForm] string outputFormat = "PNG")
    {
        if (files == null || files.Count == 0)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "No files provided",
                Detail = "Please upload at least one image file"
            });
        }

        if (files.Count > 10)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Too many files",
                Detail = "Maximum 10 files allowed per batch"
            });
        }

        try
        {
            var filePaths = files.Select(f => f.FileName).ToArray();
            var result = await _backgroundRemovalClient.RemoveBackgroundBatchAsync(filePaths, outputFormat);
            
            return File(result, "application/zip", "processed_images.zip");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing batch of {Count} files", files.Count);
            return StatusCode(500, new ProblemDetails
            {
                Title = "Batch processing failed",
                Detail = ex.Message
            });
        }
    }

    [HttpGet("health")]
    [ProducesResponseType(typeof(HealthStatus), 200)]
    public async Task<IActionResult> Health()
    {
        try
        {
            var isHealthy = await _backgroundRemovalClient.IsHealthyAsync();
            var models = await _backgroundRemovalClient.GetAvailableModelsAsync();
            
            return Ok(new HealthStatus
            {
                IsHealthy = isHealthy,
                AvailableModels = models,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new HealthStatus
            {
                IsHealthy = false,
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    [HttpGet("models")]
    [ProducesResponseType(typeof(string[]), 200)]
    public async Task<IActionResult> GetModels()
    {
        try
        {
            var models = await _backgroundRemovalClient.GetAvailableModelsAsync();
            return Ok(models);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available models");
            return StatusCode(500, new ProblemDetails
            {
                Title = "Failed to get models",
                Detail = ex.Message
            });
        }
    }
}

public class HealthStatus
{
    public bool IsHealthy { get; set; }
    public string[] AvailableModels { get; set; } = Array.Empty<string>();
    public DateTime Timestamp { get; set; }
    public string? Error { get; set; }
}
```

### Minimal API Example

```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMameyImageBackgroundRemoval(builder.Configuration);

var app = builder.Build();

app.MapPost("/api/remove-background", async (
    IFormFile file,
    IBackgroundRemovalClient client,
    string outputFormat = "PNG") =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest("No file provided");

    using var stream = file.OpenReadStream();
    var result = await client.RemoveBackgroundAsync(stream, outputFormat);
    
    var contentType = outputFormat.ToLowerInvariant() switch
    {
        "png" => "image/png",
        "jpeg" => "image/jpeg",
        _ => "image/png"
    };
    
    return Results.File(result, contentType, $"no_bg_{file.FileName}");
});

app.MapGet("/api/health", async (IBackgroundRemovalClient client) =>
{
    var isHealthy = await client.IsHealthyAsync();
    var models = await client.GetAvailableModelsAsync();
    
    return new { isHealthy, models, timestamp = DateTime.UtcNow };
});

app.Run();
```

## Background Services

### Hosted Service for Batch Processing

```csharp
public class ImageProcessingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ImageProcessingBackgroundService> _logger;
    private readonly Channel<ImageProcessingJob> _channel;

    public ImageProcessingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ImageProcessingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _channel = Channel.CreateUnbounded<ImageProcessingJob>();
    }

    public async Task QueueJobAsync(ImageProcessingJob job)
    {
        await _channel.Writer.WriteAsync(job);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var job in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var client = scope.ServiceProvider.GetRequiredService<IBackgroundRemovalClient>();
                
                await ProcessJobAsync(client, job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing job {JobId}", job.Id);
            }
        }
    }

    private async Task ProcessJobAsync(IBackgroundRemovalClient client, ImageProcessingJob job)
    {
        _logger.LogInformation("Processing job {JobId} with {FileCount} files", 
            job.Id, job.FilePaths.Length);

        var result = await client.RemoveBackgroundBatchAsync(job.FilePaths, job.OutputFormat);
        
        // Save result or notify completion
        await job.OnCompletedAsync(result);
        
        _logger.LogInformation("Completed job {JobId}", job.Id);
    }
}

public class ImageProcessingJob
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string[] FilePaths { get; set; } = Array.Empty<string>();
    public string OutputFormat { get; set; } = "PNG";
    public Func<Stream, Task> OnCompletedAsync { get; set; } = _ => Task.CompletedTask;
}
```

### Worker Service Example

```csharp
public class ImageProcessingWorker : BackgroundService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;
    private readonly ILogger<ImageProcessingWorker> _logger;
    private readonly IConfiguration _configuration;

    public ImageProcessingWorker(
        IBackgroundRemovalClient backgroundRemovalClient,
        ILogger<ImageProcessingWorker> logger,
        IConfiguration configuration)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var inputDirectory = _configuration["ImageProcessing:InputDirectory"];
        var outputDirectory = _configuration["ImageProcessing:OutputDirectory"];
        
        if (string.IsNullOrEmpty(inputDirectory) || string.IsNullOrEmpty(outputDirectory))
        {
            _logger.LogError("Input or output directory not configured");
            return;
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingImagesAsync(inputDirectory, outputDirectory, stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in image processing worker");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task ProcessPendingImagesAsync(
        string inputDirectory, 
        string outputDirectory, 
        CancellationToken cancellationToken)
    {
        var imageFiles = Directory.GetFiles(inputDirectory, "*.jpg")
            .Concat(Directory.GetFiles(inputDirectory, "*.png"))
            .ToArray();

        if (imageFiles.Length == 0)
            return;

        _logger.LogInformation("Found {Count} images to process", imageFiles.Length);

        // Process in batches
        const int batchSize = 5;
        for (int i = 0; i < imageFiles.Length; i += batchSize)
        {
            var batch = imageFiles.Skip(i).Take(batchSize).ToArray();
            
            try
            {
                var zipStream = await _backgroundRemovalClient.RemoveBackgroundBatchAsync(
                    batch, 
                    "PNG", 
                    cancellationToken);
                
                using var zipArchive = new ZipArchive(zipStream);
                zipArchive.ExtractToDirectory(outputDirectory, true);
                
                _logger.LogInformation("Processed batch {BatchNumber}", i / batchSize + 1);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch starting at index {Index}", i);
            }
        }
    }
}
```

## Error Handling

### Comprehensive Error Handling

```csharp
public class RobustImageService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;
    private readonly ILogger<RobustImageService> _logger;

    public RobustImageService(
        IBackgroundRemovalClient backgroundRemovalClient,
        ILogger<RobustImageService> logger)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
        _logger = logger;
    }

    public async Task<ImageProcessingResult> ProcessImageSafelyAsync(
        string inputPath, 
        string outputPath)
    {
        try
        {
            // Validate input
            if (!File.Exists(inputPath))
            {
                return ImageProcessingResult.Failure(
                    inputPath, 
                    "Input file does not exist");
            }

            var fileInfo = new FileInfo(inputPath);
            if (fileInfo.Length == 0)
            {
                return ImageProcessingResult.Failure(
                    inputPath, 
                    "Input file is empty");
            }

            // Check service health
            var isHealthy = await _backgroundRemovalClient.IsHealthyAsync();
            if (!isHealthy)
            {
                return ImageProcessingResult.Failure(
                    inputPath, 
                    "Background removal service is not healthy");
            }

            // Process image
            var result = await _backgroundRemovalClient.RemoveBackgroundFromFileAsync(
                inputPath, 
                outputPath, 
                "PNG");

            return ImageProcessingResult.Success(inputPath, result);
        }
        catch (FileNotFoundException ex)
        {
            _logger.LogWarning(ex, "File not found: {InputPath}", inputPath);
            return ImageProcessingResult.Failure(inputPath, "File not found");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error processing {InputPath}", inputPath);
            return ImageProcessingResult.Failure(inputPath, "Service communication error");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogWarning(ex, "Timeout processing {InputPath}", inputPath);
            return ImageProcessingResult.Failure(inputPath, "Processing timeout");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing {InputPath}", inputPath);
            return ImageProcessingResult.Failure(inputPath, "Unexpected error");
        }
    }
}

public class ImageProcessingResult
{
    public string InputPath { get; set; } = string.Empty;
    public string? OutputPath { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    public static ImageProcessingResult Success(string inputPath, string outputPath)
    {
        return new ImageProcessingResult
        {
            InputPath = inputPath,
            OutputPath = outputPath,
            Success = true
        };
    }

    public static ImageProcessingResult Failure(string inputPath, string error)
    {
        return new ImageProcessingResult
        {
            InputPath = inputPath,
            Success = false,
            Error = error
        };
    }
}
```

### Retry Policy with Polly

```csharp
builder.Services.AddMameyImageBackgroundRemoval(builder.Configuration);

// Add Polly for retry policies
builder.Services.AddHttpClient<IBackgroundRemovalClient, BackgroundRemovalClient>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (outcome, timespan, retryCount, context) =>
            {
                Console.WriteLine($"Retry {retryCount} after {timespan} seconds");
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,
            durationOfBreak: TimeSpan.FromSeconds(30));
}
```

## Performance Optimization

### Memory-Efficient Processing

```csharp
public class MemoryEfficientImageService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;
    private readonly ILogger<MemoryEfficientImageService> _logger;

    public MemoryEfficientImageService(
        IBackgroundRemovalClient backgroundRemovalClient,
        ILogger<MemoryEfficientImageService> logger)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
        _logger = logger;
    }

    public async Task ProcessLargeImageAsync(string inputPath, string outputPath)
    {
        const int bufferSize = 1024 * 1024; // 1MB buffer
        
        using var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize);
        using var outputStream = await _backgroundRemovalClient.RemoveBackgroundAsync(inputStream, "PNG");
        using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize);
        
        await outputStream.CopyToAsync(fileStream);
    }

    public async Task ProcessImagesInBatchesAsync(
        string[] imagePaths, 
        string outputDirectory,
        int batchSize = 5)
    {
        for (int i = 0; i < imagePaths.Length; i += batchSize)
        {
            var batch = imagePaths.Skip(i).Take(batchSize).ToArray();
            
            try
            {
                using var zipStream = await _backgroundRemovalClient.RemoveBackgroundBatchAsync(batch, "PNG");
                using var zipArchive = new ZipArchive(zipStream);
                zipArchive.ExtractToDirectory(outputDirectory, true);
                
                _logger.LogInformation("Processed batch {StartIndex}-{EndIndex}", i, i + batch.Length - 1);
                
                // Force garbage collection after each batch
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch starting at index {Index}", i);
            }
        }
    }
}
```

### Caching and Optimization

```csharp
public class CachedImageService
{
    private readonly IBackgroundRemovalClient _backgroundRemovalClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachedImageService> _logger;

    public CachedImageService(
        IBackgroundRemovalClient backgroundRemovalClient,
        IMemoryCache cache,
        ILogger<CachedImageService> logger)
    {
        _backgroundRemovalClient = backgroundRemovalClient;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Stream> GetProcessedImageAsync(string inputPath, string outputFormat = "PNG")
    {
        var cacheKey = $"processed_{Path.GetFileName(inputPath)}_{outputFormat}";
        
        if (_cache.TryGetValue(cacheKey, out Stream? cachedStream))
        {
            _logger.LogDebug("Returning cached result for {InputPath}", inputPath);
            return cachedStream!;
        }

        _logger.LogDebug("Processing and caching {InputPath}", inputPath);
        
        using var inputStream = File.OpenRead(inputPath);
        var result = await _backgroundRemovalClient.RemoveBackgroundAsync(inputStream, outputFormat);
        
        // Cache the result for 1 hour
        var cacheOptions = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            Size = result.Length
        };
        
        _cache.Set(cacheKey, result, cacheOptions);
        
        return result;
    }
}
```

## Testing

### Unit Tests

```csharp
[TestClass]
public class ImageServiceTests
{
    private Mock<IBackgroundRemovalClient> _mockClient = null!;
    private ImageService _imageService = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockClient = new Mock<IBackgroundRemovalClient>();
        _imageService = new ImageService(_mockClient.Object);
    }

    [TestMethod]
    public async Task RemoveBackgroundAsync_ValidInput_ReturnsOutputPath()
    {
        // Arrange
        var inputPath = "input.jpg";
        var outputPath = "output.png";
        var expectedResult = "processed_output.png";
        
        _mockClient.Setup(x => x.RemoveBackgroundFromFileAsync(
            inputPath, outputPath, "PNG", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _imageService.RemoveBackgroundAsync(inputPath, outputPath);

        // Assert
        Assert.AreEqual(expectedResult, result);
        _mockClient.Verify(x => x.RemoveBackgroundFromFileAsync(
            inputPath, outputPath, "PNG", It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task RemoveBackgroundAsync_ServiceThrowsException_ThrowsException()
    {
        // Arrange
        var inputPath = "input.jpg";
        var outputPath = "output.png";
        
        _mockClient.Setup(x => x.RemoveBackgroundFromFileAsync(
            inputPath, outputPath, "PNG", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Service unavailable"));

        // Act & Assert
        await Assert.ThrowsExceptionAsync<HttpRequestException>(() =>
            _imageService.RemoveBackgroundAsync(inputPath, outputPath));
    }
}
```

### Integration Tests

```csharp
[TestClass]
public class ImageControllerIntegrationTests
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [TestInitialize]
    public void Setup()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace with test implementation
                    services.AddScoped<IBackgroundRemovalClient, TestBackgroundRemovalClient>();
                });
            });
        
        _client = _factory.CreateClient();
    }

    [TestMethod]
    public async Task RemoveBackground_ValidFile_ReturnsProcessedImage()
    {
        // Arrange
        using var content = new MultipartFormDataContent();
        using var fileStream = File.OpenRead("test-image.jpg");
        content.Add(new StreamContent(fileStream), "file", "test-image.jpg");
        content.Add(new StringContent("PNG"), "outputFormat");

        // Act
        var response = await _client.PostAsync("/api/images/remove-background", content);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        Assert.AreEqual("image/png", response.Content.Headers.ContentType?.MediaType);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}

public class TestBackgroundRemovalClient : IBackgroundRemovalClient
{
    public Task<Stream> RemoveBackgroundAsync(Stream imageStream, string outputFormat = "PNG", CancellationToken cancellationToken = default)
    {
        // Return a test image stream
        var testImageBytes = File.ReadAllBytes("test-output.png");
        return Task.FromResult<Stream>(new MemoryStream(testImageBytes));
    }

    public Task<string> RemoveBackgroundFromFileAsync(string filePath, string? outputPath = null, string outputFormat = "PNG", CancellationToken cancellationToken = default)
    {
        return Task.FromResult(outputPath ?? "test-output.png");
    }

    public Task<Stream> RemoveBackgroundBatchAsync(string[] filePaths, string outputFormat = "PNG", CancellationToken cancellationToken = default)
    {
        // Return a test ZIP stream
        var testZipBytes = File.ReadAllBytes("test-batch.zip");
        return Task.FromResult<Stream>(new MemoryStream(testZipBytes));
    }

    public Task<string[]> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new[] { "u2net", "u2net_human_seg" });
    }

    public Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}
```

## API Reference

### IBackgroundRemovalClient Interface

#### Methods

##### `RemoveBackgroundAsync(Stream imageStream, string outputFormat, CancellationToken cancellationToken)`
Remove background from an image stream.

**Parameters:**
- `imageStream` (Stream): The image stream to process
- `outputFormat` (string): Output format ("PNG" or "JPEG")
- `cancellationToken` (CancellationToken): Cancellation token

**Returns:** `Task<Stream>` - Processed image stream with transparent background

**Example:**
```csharp
using var inputStream = File.OpenRead("input.jpg");
var result = await client.RemoveBackgroundAsync(inputStream, "PNG");
```

##### `RemoveBackgroundFromBytesAsync(byte[] imageBytes, string outputFormat, CancellationToken cancellationToken)`
Remove background from raw image bytes.

**Parameters:**
- `imageBytes` (byte[]): The raw image bytes to process
- `outputFormat` (string): Output format ("PNG" or "JPEG")
- `cancellationToken` (CancellationToken): Cancellation token

**Returns:** `Task<byte[]>` - Raw bytes of processed image with transparent background

**Example:**
```csharp
byte[] imageBytes = await File.ReadAllBytesAsync("input.jpg");
byte[] result = await client.RemoveBackgroundFromBytesAsync(imageBytes, "PNG");
```

##### `RemoveBackgroundFromFileAsync(string filePath, string? outputPath, string outputFormat, CancellationToken cancellationToken)`
Remove background from an image file.

**Parameters:**
- `filePath` (string): Path to input image file
- `outputPath` (string?): Path for output file (optional)
- `outputFormat` (string): Output format ("PNG" or "JPEG")
- `cancellationToken` (CancellationToken): Cancellation token

**Returns:** `Task<string>` - Path to processed file

**Example:**
```csharp
var result = await client.RemoveBackgroundFromFileAsync("input.jpg", "output.png", "PNG");
```

##### `RemoveBackgroundBatchAsync(string[] filePaths, string outputFormat, CancellationToken cancellationToken)`
Remove background from multiple images.

**Parameters:**
- `filePaths` (string[]): Paths to input image files
- `outputFormat` (string): Output format ("PNG" or "JPEG")
- `cancellationToken` (CancellationToken): Cancellation token

**Returns:** `Task<Stream>` - ZIP stream containing processed images

**Example:**
```csharp
var filePaths = new[] { "img1.jpg", "img2.jpg", "img3.jpg" };
var zipStream = await client.RemoveBackgroundBatchAsync(filePaths, "PNG");
```

##### `GetAvailableModelsAsync(CancellationToken cancellationToken)`
Get available background removal models.

**Parameters:**
- `cancellationToken` (CancellationToken): Cancellation token

**Returns:** `Task<string[]>` - Array of available model names

**Example:**
```csharp
var models = await client.GetAvailableModelsAsync();
// Returns: ["u2net", "u2net_human_seg", "u2netp", ...]
```

##### `IsHealthyAsync(CancellationToken cancellationToken)`
Check if the background removal service is healthy.

**Parameters:**
- `cancellationToken` (CancellationToken): Cancellation token

**Returns:** `Task<bool>` - True if service is healthy

**Example:**
```csharp
var isHealthy = await client.IsHealthyAsync();
```

### BackgroundRemovalOptions Class

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BaseUrl` | string | "http://localhost:5000" | Base URL of the background removal API service |
| `TimeoutSeconds` | int | 300 | API timeout in seconds |
| `MaxRetryAttempts` | int | 3 | Maximum retry attempts for failed requests |
| `RetryDelayMs` | int | 1000 | Retry delay in milliseconds |
| `DefaultOutputFormat` | string | "PNG" | Default output format for processed images |
| `MaxFileSizeBytes` | long | 10485760 | Maximum file size in bytes (10MB) |
| `MaxBatchSize` | int | 10 | Maximum number of files for batch processing |

### Extension Methods

#### `AddMameyImageBackgroundRemoval(IServiceCollection services, IConfiguration configuration)`
Add background removal services using configuration.

**Parameters:**
- `services` (IServiceCollection): Service collection
- `configuration` (IConfiguration): Configuration instance

**Returns:** `IServiceCollection` - Service collection for chaining

#### `AddMameyImageBackgroundRemoval(IServiceCollection services, Action<BackgroundRemovalOptions> configureOptions)`
Add background removal services using lambda configuration.

**Parameters:**
- `services` (IServiceCollection): Service collection
- `configureOptions` (Action<BackgroundRemovalOptions>): Configuration action

**Returns:** `IServiceCollection` - Service collection for chaining

## Troubleshooting

### Common Issues

#### 1. Service Not Available
**Problem:** `HttpRequestException: No connection could be made`

**Solutions:**
```csharp
// Check service health
var isHealthy = await client.IsHealthyAsync();
if (!isHealthy)
{
    // Handle service unavailable
}

// Configure retry policy
builder.Services.AddHttpClient<IBackgroundRemovalClient, BackgroundRemovalClient>()
    .AddPolicyHandler(GetRetryPolicy());
```

#### 2. File Size Exceeded
**Problem:** `ArgumentException: File size exceeds maximum allowed size`

**Solutions:**
```csharp
// Increase file size limit
builder.Services.AddMameyImageBackgroundRemoval(options =>
{
    options.MaxFileSizeBytes = 50 * 1024 * 1024; // 50MB
});

// Or resize image before processing
using var image = Image.Load(inputStream);
image.Mutate(x => x.Resize(1024, 1024));
```

#### 3. Timeout Issues
**Problem:** `TaskCanceledException: The operation was canceled`

**Solutions:**
```csharp
// Increase timeout
builder.Services.AddMameyImageBackgroundRemoval(options =>
{
    options.TimeoutSeconds = 600; // 10 minutes
});

// Use cancellation token with longer timeout
using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
var result = await client.RemoveBackgroundAsync(stream, "PNG", cts.Token);
```

#### 4. Memory Issues
**Problem:** `OutOfMemoryException` during batch processing

**Solutions:**
```csharp
// Process in smaller batches
const int batchSize = 3;
for (int i = 0; i < filePaths.Length; i += batchSize)
{
    var batch = filePaths.Skip(i).Take(batchSize).ToArray();
    await client.RemoveBackgroundBatchAsync(batch, "PNG");
    
    // Force garbage collection
    GC.Collect();
    GC.WaitForPendingFinalizers();
}
```

### Debugging

#### Enable Detailed Logging

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Or in appsettings.json
{
  "Logging": {
    "LogLevel": {
      "Mamey.Image.BackgroundRemoval": "Debug"
    }
  }
}
```

#### Health Monitoring

```csharp
public class HealthMonitoringService : BackgroundService
{
    private readonly IBackgroundRemovalClient _client;
    private readonly ILogger<HealthMonitoringService> _logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var isHealthy = await _client.IsHealthyAsync();
                var models = await _client.GetAvailableModelsAsync();
                
                _logger.LogInformation("Service health: {IsHealthy}, Models: {ModelCount}", 
                    isHealthy, models.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
```

### Performance Monitoring

```csharp
public class PerformanceMonitoringService
{
    private readonly IBackgroundRemovalClient _client;
    private readonly ILogger<PerformanceMonitoringService> _logger;

    public async Task<ProcessingMetrics> ProcessWithMetricsAsync(string inputPath)
    {
        var stopwatch = Stopwatch.StartNew();
        var memoryBefore = GC.GetTotalMemory(false);
        
        try
        {
            var result = await _client.RemoveBackgroundFromFileAsync(inputPath);
            
            stopwatch.Stop();
            var memoryAfter = GC.GetTotalMemory(false);
            
            var metrics = new ProcessingMetrics
            {
                ProcessingTime = stopwatch.Elapsed,
                MemoryUsed = memoryAfter - memoryBefore,
                Success = true
            };
            
            _logger.LogInformation("Processing completed in {ElapsedMs}ms, Memory: {MemoryMB}MB",
                metrics.ProcessingTime.TotalMilliseconds,
                metrics.MemoryUsed / 1024 / 1024);
            
            return metrics;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Processing failed after {ElapsedMs}ms", 
                stopwatch.Elapsed.TotalMilliseconds);
            
            return new ProcessingMetrics
            {
                ProcessingTime = stopwatch.Elapsed,
                Success = false,
                Error = ex.Message
            };
        }
    }
}

public class ProcessingMetrics
{
    public TimeSpan ProcessingTime { get; set; }
    public long MemoryUsed { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}
```

## License

Proprietary - Copyright (c) 2025 Mamey.io

## Support

For support and questions:
- 📧 Email: support@mamey.io
- 📚 Documentation: [Mamey.io Docs](https://docs.mamey.io)
- 🐛 Issues: [GitHub Issues](https://github.com/Mamey-io/Mamey/issues)
- 💬 Discussions: [GitHub Discussions](https://github.com/Mamey-io/Mamey/discussions)
