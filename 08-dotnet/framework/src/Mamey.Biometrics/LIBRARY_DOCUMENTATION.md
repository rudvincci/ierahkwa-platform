# Mamey.Biometrics - C# Library Documentation

## Overview

The `Mamey.Biometrics` library is a comprehensive .NET 9.0 library that provides biometric face recognition capabilities for the Mamey Framework. It orchestrates calls to the Python biometrics engine and manages storage in MongoDB and MinIO.

## Features

- **Face Enrollment**: Register new biometric templates
- **1:1 Verification**: Compare two faces for identity verification
- **1:N Identification**: Search for a face in a collection of known faces
- **Template Management**: CRUD operations for biometric templates
- **Image Storage**: Original image storage in MinIO
- **Caching**: In-memory caching for frequently accessed templates
- **Resilience**: Polly retry policies and circuit breakers
- **Async/Await**: Full async support throughout

## Installation

### Package Reference

```xml
<PackageReference Include="Mamey.Biometrics" Version="0.1.0" />
```

### Dependencies

The library requires the following packages (automatically included):

```xml
<PackageReference Include="MongoDB.Driver" Version="2.25.0" />
<PackageReference Include="Minio" Version="6.0.2" />
<PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="9.0.0-preview.4.24266.1" />
<PackageReference Include="Microsoft.Extensions.Http.Polly" Version="9.0.0-preview.4.24266.1" />
<PackageReference Include="System.Drawing.Common" Version="9.0.0-preview.4.24266.1" />
```

## Quick Start

### 1. Configuration

Add configuration to your `appsettings.json`:

```json
{
  "Biometrics": {
    "BiometricEngineBaseUrl": "http://localhost:6020",
    "SimilarityThreshold": 0.6,
    "MaxFacesPerImage": 1,
    "TemplateCacheDurationMinutes": 60
  },
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "biometrics",
    "CollectionName": "templates"
  },
  "MinIO": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "UseSSL": false,
    "BucketName": "biometric-images"
  }
}
```

### 2. Service Registration

Register services in your `Program.cs` or `Startup.cs`:

```csharp
using Mamey.Biometrics;

var builder = WebApplication.CreateBuilder(args);

// Add Mamey Biometrics services
builder.Services.AddMameyBiometrics(builder.Configuration);

var app = builder.Build();
```

### 3. Basic Usage

```csharp
using Mamey.Biometrics.Services;
using Mamey.Biometrics.Services.Models;

public class BiometricController : ControllerBase
{
    private readonly IBiometricService _biometricService;

    public BiometricController(IBiometricService biometricService)
    {
        _biometricService = biometricService;
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollFace([FromForm] IFormFile imageFile)
    {
        // Convert image to base64
        using var memoryStream = new MemoryStream();
        await imageFile.CopyToAsync(memoryStream);
        var imageBytes = memoryStream.ToArray();
        var imageBase64 = Convert.ToBase64String(imageBytes);

        // Create enrollment request
        var request = new EnrollmentRequest
        {
            SubjectId = Guid.NewGuid(),
            ImageData = imageBase64,
            ImageFormat = "JPEG",
            Metadata = new Dictionary<string, object>
            {
                ["source"] = "web_upload",
                ["quality"] = "high"
            }
        };

        // Enroll face
        var result = await _biometricService.EnrollAsync(request);

        if (result.Success)
        {
            return Ok(new { TemplateId = result.TemplateId, Message = "Face enrolled successfully" });
        }

        return BadRequest(new { Error = result.ErrorMessage });
    }

    [HttpPost("verify")]
    public async Task<IActionResult> VerifyFace([FromForm] IFormFile imageFile, [FromQuery] Guid templateId)
    {
        // Convert image to base64
        using var memoryStream = new MemoryStream();
        await imageFile.CopyToAsync(memoryStream);
        var imageBytes = memoryStream.ToArray();
        var imageBase64 = Convert.ToBase64String(imageBytes);

        // Create verification request
        var request = new VerificationRequest
        {
            TemplateId = templateId,
            ImageData = imageBase64,
            ImageFormat = "JPEG"
        };

        // Verify face
        var result = await _biometricService.VerifyAsync(request);

        return Ok(new { 
            Match = result.IsMatch, 
            Confidence = result.Confidence,
            Distance = result.Distance 
        });
    }
}
```

## Core Services

### IBiometricService

The main service interface for biometric operations.

```csharp
public interface IBiometricService
{
    // Enrollment
    Task<EnrollmentResult> EnrollAsync(EnrollmentRequest request, CancellationToken cancellationToken = default);
    
    // Verification (1:1)
    Task<VerificationResult> VerifyAsync(VerificationRequest request, CancellationToken cancellationToken = default);
    
    // Identification (1:N)
    Task<IdentificationResult> IdentifyAsync(IdentificationRequest request, CancellationToken cancellationToken = default);
    
    // Template Management
    Task<BiometricTemplate?> GetTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<PagedResult<BiometricTemplate>> GetTemplatesAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> DeleteTemplateAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<List<BiometricTemplate>> GetTemplatesBySubjectAsync(Guid subjectId, CancellationToken cancellationToken = default);
    
    // Image Management
    Task<Stream?> GetOriginalImageAsync(Guid templateId, CancellationToken cancellationToken = default);
    Task<bool> DeleteImageAsync(Guid templateId, CancellationToken cancellationToken = default);
    
    // Statistics
    Task<BiometricStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
```

## Data Models

### EnrollmentRequest

```csharp
public class EnrollmentRequest
{
    public Guid SubjectId { get; set; }
    public string ImageData { get; set; } = string.Empty;  // Base64 encoded
    public string ImageFormat { get; set; } = "JPEG";
    public Dictionary<string, object>? Metadata { get; set; }
    public List<string>? Tags { get; set; }
}
```

### EnrollmentResult

```csharp
public class EnrollmentResult
{
    public bool Success { get; set; }
    public Guid? TemplateId { get; set; }
    public string? ErrorMessage { get; set; }
    public float QualityScore { get; set; }
    public BiometricTemplate? Template { get; set; }
}
```

### VerificationRequest

```csharp
public class VerificationRequest
{
    public Guid TemplateId { get; set; }
    public string ImageData { get; set; } = string.Empty;  // Base64 encoded
    public string ImageFormat { get; set; } = "JPEG";
    public float? Threshold { get; set; }  // Optional custom threshold
}
```

### VerificationResult

```csharp
public class VerificationResult
{
    public bool IsMatch { get; set; }
    public float Confidence { get; set; }
    public float Distance { get; set; }
    public Guid TemplateId { get; set; }
    public string? ErrorMessage { get; set; }
}
```

### IdentificationRequest

```csharp
public class IdentificationRequest
{
    public string ImageData { get; set; } = string.Empty;  // Base64 encoded
    public string ImageFormat { get; set; } = "JPEG";
    public float? Threshold { get; set; }
    public int MaxResults { get; set; } = 10;
    public List<Guid>? SubjectIds { get; set; }  // Optional: limit search to specific subjects
    public List<string>? Tags { get; set; }  // Optional: filter by tags
}
```

### IdentificationResult

```csharp
public class IdentificationResult
{
    public List<IdentificationMatch> Matches { get; set; } = new();
    public int TotalTemplatesSearched { get; set; }
    public string? ErrorMessage { get; set; }
}

public class IdentificationMatch
{
    public Guid TemplateId { get; set; }
    public Guid SubjectId { get; set; }
    public float Confidence { get; set; }
    public float Distance { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
```

### BiometricTemplate

```csharp
public class BiometricTemplate
{
    public Guid Id { get; set; }
    public Guid SubjectId { get; set; }
    public List<float> Encoding { get; set; } = new();
    public Dictionary<string, object> Metadata { get; set; } = new();
    public string MinioObjectId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Tags { get; set; } = new();
}
```

## Advanced Usage Examples

### 1. Bulk Enrollment

```csharp
public async Task<List<EnrollmentResult>> BulkEnrollAsync(List<EnrollmentRequest> requests)
{
    var results = new List<EnrollmentResult>();
    
    // Process in parallel (be careful with rate limits)
    var tasks = requests.Select(async request =>
    {
        try
        {
            return await _biometricService.EnrollAsync(request);
        }
        catch (Exception ex)
        {
            return new EnrollmentResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    });
    
    results.AddRange(await Task.WhenAll(tasks));
    return results;
}
```

### 2. Batch Verification

```csharp
public async Task<List<VerificationResult>> BatchVerifyAsync(
    string imageData, 
    List<Guid> templateIds)
{
    var results = new List<VerificationResult>();
    
    var tasks = templateIds.Select(async templateId =>
    {
        var request = new VerificationRequest
        {
            TemplateId = templateId,
            ImageData = imageData,
            ImageFormat = "JPEG"
        };
        
        return await _biometricService.VerifyAsync(request);
    });
    
    results.AddRange(await Task.WhenAll(tasks));
    return results;
}
```

### 3. Template Search with Filters

```csharp
public async Task<List<BiometricTemplate>> SearchTemplatesAsync(
    string? tag = null,
    DateTime? createdAfter = null,
    int pageSize = 50)
{
    var templates = new List<BiometricTemplate>();
    int page = 1;
    
    while (true)
    {
        var result = await _biometricService.GetTemplatesAsync(page, pageSize);
        
        // Apply filters
        var filtered = result.Items.AsQueryable();
        
        if (!string.IsNullOrEmpty(tag))
        {
            filtered = filtered.Where(t => t.Tags.Contains(tag));
        }
        
        if (createdAfter.HasValue)
        {
            filtered = filtered.Where(t => t.CreatedAt >= createdAfter.Value);
        }
        
        templates.AddRange(filtered);
        
        if (result.Items.Count < pageSize)
            break;
            
        page++;
    }
    
    return templates;
}
```

### 4. Image Management

```csharp
public async Task<IActionResult> GetTemplateImage(Guid templateId)
{
    var imageStream = await _biometricService.GetOriginalImageAsync(templateId);
    
    if (imageStream == null)
    {
        return NotFound("Image not found");
    }
    
    return File(imageStream, "image/jpeg");
}

public async Task<IActionResult> UpdateTemplateImage(Guid templateId, IFormFile newImage)
{
    // Get existing template
    var template = await _biometricService.GetTemplateAsync(templateId);
    if (template == null)
    {
        return NotFound("Template not found");
    }
    
    // Convert new image to base64
    using var memoryStream = new MemoryStream();
    await newImage.CopyToAsync(memoryStream);
    var imageBytes = memoryStream.ToArray();
    var imageBase64 = Convert.ToBase64String(imageBytes);
    
    // Create new enrollment request
    var request = new EnrollmentRequest
    {
        SubjectId = template.SubjectId,
        ImageData = imageBase64,
        ImageFormat = "JPEG",
        Metadata = template.Metadata,
        Tags = template.Tags
    };
    
    // Enroll new image (this will create a new template)
    var result = await _biometricService.EnrollAsync(request);
    
    if (result.Success)
    {
        // Optionally delete old template
        await _biometricService.DeleteTemplateAsync(templateId);
        
        return Ok(new { NewTemplateId = result.TemplateId });
    }
    
    return BadRequest(new { Error = result.ErrorMessage });
}
```

### 5. Performance Monitoring

```csharp
public async Task<IActionResult> GetSystemStats()
{
    var stats = await _biometricService.GetStatisticsAsync();
    
    return Ok(new
    {
        TotalTemplates = stats.TotalTemplates,
        TotalSubjects = stats.TotalSubjects,
        AverageEnrollmentTime = stats.AverageEnrollmentTimeMs,
        AverageVerificationTime = stats.AverageVerificationTimeMs,
        AverageIdentificationTime = stats.AverageIdentificationTimeMs,
        CacheHitRate = stats.CacheHitRate,
        LastUpdated = stats.LastUpdated
    });
}
```

## Configuration Options

### BiometricsOptions

```csharp
public class BiometricsOptions
{
    public string BiometricEngineBaseUrl { get; set; } = string.Empty;
    public string? BiometricEngineApiKey { get; set; }
    public double SimilarityThreshold { get; set; } = 0.6;
    public int MaxFacesPerImage { get; set; } = 1;
    public int TemplateCacheDurationMinutes { get; set; } = 60;
}
```

### MongoDBOptions

```csharp
public class MongoDBOptions
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "biometrics";
    public string CollectionName { get; set; } = "templates";
}
```

### MinIOOptions

```csharp
public class MinIOOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UseSSL { get; set; } = false;
    public string BucketName { get; set; } = "biometric-images";
}
```

## Error Handling

The library provides comprehensive error handling:

```csharp
try
{
    var result = await _biometricService.EnrollAsync(request);
    
    if (!result.Success)
    {
        // Handle business logic errors
        _logger.LogError("Enrollment failed: {Error}", result.ErrorMessage);
        return BadRequest(result.ErrorMessage);
    }
    
    return Ok(result);
}
catch (HttpRequestException ex)
{
    // Handle network errors (Python engine unavailable)
    _logger.LogError(ex, "Biometric engine unavailable");
    return StatusCode(503, "Biometric service temporarily unavailable");
}
catch (MongoException ex)
{
    // Handle database errors
    _logger.LogError(ex, "Database error during enrollment");
    return StatusCode(500, "Database error");
}
catch (Exception ex)
{
    // Handle unexpected errors
    _logger.LogError(ex, "Unexpected error during enrollment");
    return StatusCode(500, "Internal server error");
}
```

## Caching

The library includes intelligent caching:

```csharp
// Templates are automatically cached after first access
var template1 = await _biometricService.GetTemplateAsync(templateId); // Database hit
var template2 = await _biometricService.GetTemplateAsync(templateId); // Cache hit

// Cache configuration
services.Configure<BiometricsOptions>(options =>
{
    options.TemplateCacheDurationMinutes = 120; // 2 hours
});
```

## Resilience Features

### Retry Policy

```csharp
// Automatic retries for transient failures
// 3 retries with exponential backoff: 1s, 3s, 5s
```

### Circuit Breaker

```csharp
// Circuit opens after 5 consecutive failures
// Stays open for 30 seconds before trying again
```

### Health Checks

```csharp
// Register health checks
builder.Services.AddHealthChecks()
    .AddCheck<BiometricServiceHealthCheck>("biometrics");

// Check health
app.MapHealthChecks("/health");
```

## Performance Considerations

### Memory Usage

- **Template Cache**: ~1KB per cached template
- **Encoding Size**: 128 floats × 4 bytes = 512 bytes
- **Image Storage**: Original images stored in MinIO (not in memory)

### Latency Targets

- **Enrollment**: < 2 seconds
- **Verification**: < 500ms (cached), < 1 second (uncached)
- **Identification**: < 1 second per 1000 templates

### Scaling

```csharp
// For high-load scenarios, consider:
// 1. Multiple Python engine instances behind load balancer
// 2. MongoDB replica set for read scaling
// 3. MinIO cluster for image storage
// 4. Redis for distributed caching
```

## Security Considerations

### Data Protection

- **Encryption**: Use TLS for all communications
- **Access Control**: Implement proper authentication/authorization
- **Data Retention**: Implement template deletion policies
- **Audit Logging**: Log all biometric operations

### Privacy

- **GDPR Compliance**: Implement data subject rights
- **Data Minimization**: Store only necessary data
- **Consent Management**: Track user consent for biometric data

## Troubleshooting

### Common Issues

1. **"Biometric engine unavailable"**
   - Check Python service is running
   - Verify network connectivity
   - Check service health endpoint

2. **"Template not found"**
   - Verify template ID exists
   - Check database connectivity
   - Review template deletion logs

3. **"Image processing failed"**
   - Validate image format and size
   - Check image contains clear face
   - Review Python service logs

4. **"Cache miss rate high"**
   - Increase cache duration
   - Check memory usage
   - Review cache eviction policies

### Debug Logging

```csharp
// Enable detailed logging
builder.Logging.AddFilter("Mamey.Biometrics", LogLevel.Debug);

// Or configure specific services
builder.Logging.AddFilter("Mamey.Biometrics.Services.BiometricService", LogLevel.Trace);
```

## Best Practices

1. **Always use async/await** for all operations
2. **Implement proper error handling** with specific exception types
3. **Use cancellation tokens** for long-running operations
4. **Monitor performance metrics** and set up alerts
5. **Implement proper logging** for audit trails
6. **Test with various image qualities** and formats
7. **Consider rate limiting** for public APIs
8. **Implement proper cleanup** for failed operations
9. **Use connection pooling** for database connections
10. **Monitor memory usage** and cache hit rates
