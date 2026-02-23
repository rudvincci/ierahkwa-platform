# Mamey.Biometrics

A comprehensive biometric face recognition library for the Mamey FutureWampum Identity (FWID) ecosystem. This library provides enrollment, verification, identification, and template management capabilities with MongoDB and MinIO storage integration.

## Features

- **Face Recognition**: Extract face encodings and perform 1:1 verification and 1:N identification
- **Template Management**: Store, retrieve, update, and delete biometric templates
- **Image Storage**: Store original images in MinIO for audit and re-enrollment
- **Caching**: In-memory caching for frequently accessed templates
- **Resilience**: Polly retry policies and circuit breaker patterns
- **Performance**: Optimized for high-throughput scenarios
- **Security**: Secure storage and transmission of biometric data

## Quick Start

### Installation

```bash
dotnet add package Mamey.Biometrics
```

### Configuration

Add to your `appsettings.json`:

```json
{
  "Biometrics": {
    "BiometricsEngineBaseUrl": "http://localhost:6020",
    "TimeoutSeconds": 30,
    "MaxImageSizeBytes": 10485760,
    "DefaultVerificationThreshold": 0.6,
    "DefaultIdentificationThreshold": 0.7,
    "CacheExpirationMinutes": 15
  },
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fwid_biometrics"
  },
  "MinIO": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minioadmin",
    "SecretKey": "minioadmin",
    "BucketName": "biometric-images",
    "UseSSL": false
  }
}
```

### Service Registration

```csharp
using Mamey.Biometrics;

var builder = WebApplication.CreateBuilder(args);

// Add biometric services
builder.Services.AddMameyBiometrics(builder.Configuration);
builder.Services.AddMameyBiometricsMongoDB(builder.Configuration);
builder.Services.AddMameyBiometricsMinIO(builder.Configuration);

var app = builder.Build();
```

### Basic Usage

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
    public async Task<IActionResult> Enroll([FromBody] EnrollmentRequest request)
    {
        var result = await _biometricService.EnrollAsync(request);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerificationRequest request)
    {
        var result = await _biometricService.VerifyAsync(request);
        return Ok(result);
    }

    [HttpPost("identify")]
    public async Task<IActionResult> Identify([FromBody] IdentificationRequest request)
    {
        var result = await _biometricService.IdentifyAsync(request);
        return Ok(result);
    }
}
```

## API Reference

### Enrollment

Enroll a new biometric template:

```csharp
var request = new EnrollmentRequest
{
    SubjectId = Guid.NewGuid(),
    ImageData = Convert.ToBase64String(imageBytes),
    ImageFormat = "JPEG",
    Tags = new List<string> { "passport", "verified" },
    MinQualityScore = 0.7
};

var result = await _biometricService.EnrollAsync(request);
```

### Verification (1:1 Matching)

Verify a face against a stored template:

```csharp
var request = new VerificationRequest
{
    TemplateId = templateId,
    ImageData = Convert.ToBase64String(imageBytes),
    ImageFormat = "JPEG",
    Threshold = 0.6
};

var result = await _biometricService.VerifyAsync(request);
```

### Identification (1:N Matching)

Identify a face against all stored templates:

```csharp
var request = new IdentificationRequest
{
    ImageData = Convert.ToBase64String(imageBytes),
    ImageFormat = "JPEG",
    MaxResults = 10,
    Threshold = 0.7
};

var result = await _biometricService.IdentifyAsync(request);
```

### Template Management

```csharp
// Get a template
var template = await _biometricService.GetTemplateAsync(templateId);

// Get templates with pagination
var pagedResult = await _biometricService.GetTemplatesAsync(page: 1, pageSize: 20);

// Get templates for a subject
var subjectTemplates = await _biometricService.GetTemplatesBySubjectAsync(subjectId);

// Delete a template
var deleted = await _biometricService.DeleteTemplateAsync(templateId);
```

### Image Management

```csharp
// Get original image
var imageStream = await _biometricService.GetOriginalImageAsync(templateId);

// Delete image
var deleted = await _biometricService.DeleteImageAsync(templateId);
```

## Configuration Options

### BiometricsOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| BiometricsEngineBaseUrl | string | "http://localhost:6020" | Python biometric engine URL |
| TimeoutSeconds | int | 30 | HTTP client timeout |
| MaxImageSizeBytes | long | 10485760 | Maximum image size (10MB) |
| DefaultVerificationThreshold | double | 0.6 | Default verification threshold |
| DefaultIdentificationThreshold | double | 0.7 | Default identification threshold |
| CacheExpirationMinutes | int | 15 | Cache expiration time |
| MaxIdentificationResults | int | 100 | Maximum identification results |
| EnableDetailedLogging | bool | false | Enable detailed logging |

### MongoDBOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| ConnectionString | string | Required | MongoDB connection string |
| DatabaseName | string | "fwid_biometrics" | Database name |
| TemplatesCollectionName | string | "biometric_templates" | Collection name |
| UseSSL | bool | false | Use SSL connection |
| ConnectionTimeoutSeconds | int | 30 | Connection timeout |

### MinIOOptions

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| Endpoint | string | Required | MinIO endpoint URL |
| AccessKey | string | Required | Access key |
| SecretKey | string | Required | Secret key |
| BucketName | string | "biometric-images" | Bucket name |
| UseSSL | bool | false | Use SSL connection |
| Region | string | "us-east-1" | AWS region |

## Data Models

### BiometricTemplate

```csharp
public class BiometricTemplate
{
    public Guid TemplateId { get; set; }
    public Guid SubjectId { get; set; }
    public BiometricTemplateMetadata Metadata { get; set; }
    public string MinioObjectId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<string> Tags { get; set; }
}
```

### Face Location

```csharp
public class FaceLocation
{
    public int Top { get; set; }
    public int Right { get; set; }
    public int Bottom { get; set; }
    public int Left { get; set; }
}
```

## Performance Considerations

- **Caching**: Templates are cached in memory for 15 minutes by default
- **Batch Operations**: Use identification for 1:N matching instead of multiple verifications
- **Image Size**: Keep images under 10MB for optimal performance
- **Thresholds**: Adjust thresholds based on your accuracy requirements
- **Database Indexes**: MongoDB indexes are automatically created for optimal query performance

## Security

- **Data Encryption**: Images are stored securely in MinIO
- **Access Control**: Implement proper authentication and authorization
- **Audit Logging**: All operations are logged for audit purposes
- **Data Retention**: Implement data retention policies as required

## Error Handling

The library provides comprehensive error handling:

```csharp
var result = await _biometricService.EnrollAsync(request);

if (!result.Success)
{
    // Handle error
    Console.WriteLine($"Error: {result.Error}");
    Console.WriteLine($"Message: {result.Message}");
}
```

## Health Monitoring

Check service health:

```csharp
var isHealthy = await _biometricService.IsHealthyAsync();

var statistics = await _biometricService.GetStatisticsAsync();
Console.WriteLine($"Total Templates: {statistics.TotalTemplates}");
Console.WriteLine($"Engine Healthy: {statistics.EngineHealthy}");
```

## Dependencies

- MongoDB.Driver (2.23.0)
- Minio (6.0.2)
- Microsoft.Extensions.Caching.Memory (9.0.0)
- Microsoft.Extensions.Http.Polly (9.0.0)
- Polly (8.2.0)

## Documentation

- [Library Documentation](LIBRARY_DOCUMENTATION.md) - Complete library reference with examples
- [Quick Start Guide](../../QUICK_START_GUIDE.md) - Get up and running in 5 minutes
- [System Overview](../../BIOMETRICS_SYSTEM_OVERVIEW.md) - Complete system architecture
- [Sample Code Examples](../../SAMPLE_CODE_EXAMPLES.md) - Comprehensive code examples

## License

MIT License - see LICENSE file for details.

## Support

For support and questions, please contact the Mamey Digital Solutions team.
