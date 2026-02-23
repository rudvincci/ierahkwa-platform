using NET10.Core.Models.Geocoding;

namespace NET10.Core.Interfaces;

/// <summary>
/// Geocoding Service Interface - Address ↔ Coordinates conversion
/// </summary>
public interface IGeocodingService
{
    // Single operations
    Task<GeocodingResult> GeocodeAsync(GeocodingRequest request, GeocodingOptions options);
    Task<GeocodingResult> ReverseGeocodeAsync(ReverseGeocodingRequest request, GeocodingOptions options);
    
    // Batch operations
    Task<GeocodingBatch> CreateBatchAsync(string name, GeocodingMode mode, GeocodingOptions options);
    Task<GeocodingBatch?> GetBatchAsync(Guid batchId);
    Task<GeocodingBatch> ProcessBatchAsync(Guid batchId);
    Task<bool> CancelBatchAsync(Guid batchId);
    Task<bool> RetryFailedAsync(Guid batchId);
    
    // CSV operations
    Task<CsvImportResult> ImportCsvFromContentAsync(string csvContent, GeocodingOptions options);
    Task<string> ExportToCsvAsync(GeocodingBatch batch, GeocodingOptions options);
    CsvImportResult ParseCsv(string csvContent, GeocodingOptions options);
    string ExportToCsv(GeocodingBatch batch, GeocodingOptions options);
    
    // Validation and metadata
    Task<bool> ValidateApiKeyAsync(string apiKey);
    List<string> GetSupportedLanguages();
    List<string> GetAvailableResultProperties();
}
