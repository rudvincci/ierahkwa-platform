using System.Text.Json;
using NET10.Core.Interfaces;
using NET10.Core.Models.Geocoding;

namespace NET10.Infrastructure.Services;

/// <summary>
/// Geocoding Service - Address ↔ Coordinates conversion
/// Supports Google Geocoding API
/// </summary>
public class GeocodingService : IGeocodingService
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<Guid, GeocodingBatch> _batches = new();
    private const string GoogleBaseUrl = "https://maps.googleapis.com/maps/api/geocode/json";
    
    public GeocodingService()
    {
        _httpClient = new HttpClient();
    }
    
    // ═══════════════════════════════════════════════════════════════
    // SINGLE OPERATIONS
    // ═══════════════════════════════════════════════════════════════
    
    public async Task<GeocodingResult> GeocodeAsync(GeocodingRequest request, GeocodingOptions options)
    {
        var result = new GeocodingResult { RequestId = request.Id };
        
        try
        {
            if (string.IsNullOrEmpty(options.ApiKey))
            {
                return SimulateGeocoding(request);
            }
            
            var url = $"{GoogleBaseUrl}?address={Uri.EscapeDataString(request.FullAddress)}&key={options.ApiKey}";
            if (!string.IsNullOrEmpty(options.Language))
                url += $"&language={options.Language}";
            
            var response = await _httpClient.GetStringAsync(url);
            var googleResult = JsonSerializer.Deserialize<GoogleGeocodingResponse>(response);
            
            if (googleResult?.status == "OK" && googleResult.results?.Count > 0)
            {
                var first = googleResult.results[0];
                result.Success = true;
                result.Status = "OK";
                result.FormattedAddress = first.formatted_address;
                result.Latitude = first.geometry?.location?.lat ?? 0;
                result.Longitude = first.geometry?.location?.lng ?? 0;
                result.PlaceId = first.place_id ?? "";
                result.LocationType = first.geometry?.location_type ?? "";
                
                ParseAddressComponents(first.address_components, result);
            }
            else
            {
                result.Success = false;
                result.Status = googleResult?.status ?? "UNKNOWN_ERROR";
                result.ErrorMessage = googleResult?.error_message ?? "Unknown error";
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Status = "EXCEPTION";
            result.ErrorMessage = ex.Message;
        }
        
        result.ProcessedAt = DateTime.UtcNow;
        return result;
    }
    
    public async Task<GeocodingResult> ReverseGeocodeAsync(ReverseGeocodingRequest request, GeocodingOptions options)
    {
        var result = new GeocodingResult { RequestId = request.Id };
        
        try
        {
            if (string.IsNullOrEmpty(options.ApiKey))
            {
                return SimulateReverseGeocoding(request);
            }
            
            var url = $"{GoogleBaseUrl}?latlng={request.Latitude},{request.Longitude}&key={options.ApiKey}";
            if (!string.IsNullOrEmpty(request.Language))
                url += $"&language={request.Language}";
            
            var response = await _httpClient.GetStringAsync(url);
            var googleResult = JsonSerializer.Deserialize<GoogleGeocodingResponse>(response);
            
            if (googleResult?.status == "OK" && googleResult.results?.Count > 0)
            {
                var first = googleResult.results[0];
                result.Success = true;
                result.Status = "OK";
                result.FormattedAddress = first.formatted_address;
                result.Latitude = request.Latitude;
                result.Longitude = request.Longitude;
                result.PlaceId = first.place_id ?? "";
                result.LocationType = first.geometry?.location_type ?? "";
                
                ParseAddressComponents(first.address_components, result);
            }
            else
            {
                result.Success = false;
                result.Status = googleResult?.status ?? "UNKNOWN_ERROR";
                result.ErrorMessage = googleResult?.error_message ?? "Unknown error";
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Status = "EXCEPTION";
            result.ErrorMessage = ex.Message;
        }
        
        result.ProcessedAt = DateTime.UtcNow;
        return result;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // BATCH OPERATIONS
    // ═══════════════════════════════════════════════════════════════
    
    public Task<GeocodingBatch> CreateBatchAsync(string name, GeocodingMode mode, GeocodingOptions options)
    {
        var batch = new GeocodingBatch
        {
            Name = name,
            Mode = mode,
            Options = options
        };
        
        _batches[batch.Id] = batch;
        return Task.FromResult(batch);
    }
    
    public Task<GeocodingBatch?> GetBatchAsync(Guid batchId)
    {
        _batches.TryGetValue(batchId, out var batch);
        return Task.FromResult(batch);
    }
    
    public async Task<GeocodingBatch> ProcessBatchAsync(Guid batchId)
    {
        if (!_batches.TryGetValue(batchId, out var batch))
        {
            throw new KeyNotFoundException($"Batch {batchId} not found");
        }
        
        batch.BatchStatus = BatchStatus.Processing;
        batch.StartedAt = DateTime.UtcNow;
        
        foreach (var item in batch.Items)
        {
            if (batch.BatchStatus == BatchStatus.Cancelled) break;
            
            item.Status = ProcessingStatus.InProgress;
            
            try
            {
                GeocodingResult? result = null;
                
                if (batch.Mode == GeocodingMode.Geocoding && item.GeoRequest != null)
                {
                    result = await GeocodeAsync(item.GeoRequest, batch.Options);
                }
                else if (batch.Mode == GeocodingMode.ReverseGeocoding && item.ReverseRequest != null)
                {
                    result = await ReverseGeocodeAsync(item.ReverseRequest, batch.Options);
                }
                
                item.Result = result;
                item.Status = result?.Success == true ? ProcessingStatus.Success : ProcessingStatus.Failed;
                item.StatusDescription = result?.Success == true ? "Success" : result?.ErrorMessage ?? "Failed";
            }
            catch (Exception ex)
            {
                item.Status = ProcessingStatus.Failed;
                item.StatusDescription = ex.Message;
            }
            
            if (batch.Options.DelayBetweenRequests > 0)
            {
                await Task.Delay(batch.Options.DelayBetweenRequests);
            }
        }
        
        if (batch.BatchStatus != BatchStatus.Cancelled)
        {
            batch.BatchStatus = BatchStatus.Completed;
        }
        batch.CompletedAt = DateTime.UtcNow;
        
        return batch;
    }
    
    public Task<bool> CancelBatchAsync(Guid batchId)
    {
        if (_batches.TryGetValue(batchId, out var batch))
        {
            batch.BatchStatus = BatchStatus.Cancelled;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public async Task<bool> RetryFailedAsync(Guid batchId)
    {
        if (!_batches.TryGetValue(batchId, out var batch))
        {
            return false;
        }
        
        var failedItems = batch.Items.Where(i => i.Status == ProcessingStatus.Failed).ToList();
        
        foreach (var item in failedItems)
        {
            item.Status = ProcessingStatus.Pending;
            item.StatusDescription = "Retry pending";
        }
        
        // Reprocess
        await ProcessBatchAsync(batchId);
        return true;
    }
    
    // ═══════════════════════════════════════════════════════════════
    // CSV OPERATIONS
    // ═══════════════════════════════════════════════════════════════
    
    public Task<CsvImportResult> ImportCsvFromContentAsync(string csvContent, GeocodingOptions options)
    {
        return Task.FromResult(ParseCsv(csvContent, options));
    }
    
    public Task<string> ExportToCsvAsync(GeocodingBatch batch, GeocodingOptions options)
    {
        return Task.FromResult(ExportToCsv(batch, options));
    }
    
    public CsvImportResult ParseCsv(string csvContent, GeocodingOptions options)
    {
        var result = new CsvImportResult();
        
        try
        {
            var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
            {
                result.ErrorMessage = "Empty CSV file";
                return result;
            }
            
            var delimiter = options.UseCultureSpecificDelimiter 
                ? System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator 
                : options.Delimiter;
            
            var startIndex = 0;
            if (options.UseHeaderRow)
            {
                result.Headers = ParseCsvLine(lines[0], delimiter);
                startIndex = 1;
            }
            
            for (int i = startIndex; i < lines.Length; i++)
            {
                var values = ParseCsvLine(lines[i], delimiter);
                var row = new Dictionary<string, string>();
                
                for (int j = 0; j < values.Count; j++)
                {
                    var key = j < result.Headers.Count ? result.Headers[j] : $"Column{j + 1}";
                    row[key] = values[j];
                }
                
                result.Rows.Add(row);
            }
            
            result.Success = true;
            result.TotalRows = result.Rows.Count;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }
        
        return result;
    }
    
    public string ExportToCsv(GeocodingBatch batch, GeocodingOptions options)
    {
        var lines = new List<string>();
        var delimiter = options.Delimiter;
        
        var firstItem = batch.Items.FirstOrDefault();
        var headers = firstItem != null 
            ? new List<string>(firstItem.OriginalData.Keys) 
            : new List<string>();
        headers.AddRange(options.ResultProperties);
        headers.Add("Status");
        headers.Add("StatusDescription");
        
        lines.Add(string.Join(delimiter, headers.Select(EscapeCsvField)));
        
        foreach (var item in batch.Items)
        {
            var values = new List<string>();
            
            foreach (var key in firstItem?.OriginalData.Keys ?? Enumerable.Empty<string>())
            {
                values.Add(item.OriginalData.GetValueOrDefault(key, ""));
            }
            
            foreach (var prop in options.ResultProperties)
            {
                values.Add(GetResultProperty(item.Result, prop));
            }
            
            values.Add(item.Status.ToString());
            values.Add(item.StatusDescription);
            
            lines.Add(string.Join(delimiter, values.Select(EscapeCsvField)));
        }
        
        return string.Join(Environment.NewLine, lines);
    }
    
    // ═══════════════════════════════════════════════════════════════
    // VALIDATION & METADATA
    // ═══════════════════════════════════════════════════════════════
    
    public async Task<bool> ValidateApiKeyAsync(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey)) return false;
        
        try
        {
            var url = $"{GoogleBaseUrl}?address=test&key={apiKey}";
            var response = await _httpClient.GetStringAsync(url);
            var result = JsonSerializer.Deserialize<GoogleGeocodingResponse>(response);
            
            return result?.status != "REQUEST_DENIED";
        }
        catch
        {
            return false;
        }
    }
    
    public List<string> GetSupportedLanguages()
    {
        return new List<string>
        {
            "en", "es", "fr", "de", "it", "pt", "ru", "zh", "ja", "ko",
            "ar", "nl", "pl", "tr", "vi", "th", "id", "hi", "he", "uk"
        };
    }
    
    public List<string> GetAvailableResultProperties()
    {
        return new List<string>
        {
            "FormattedAddress", "Latitude", "Longitude", "StreetNumber", "Route",
            "Locality", "AdminAreaLevel1", "AdminAreaLevel2", "Country", 
            "CountryCode", "PostalCode", "PlaceId", "LocationType"
        };
    }
    
    // ═══════════════════════════════════════════════════════════════
    // HELPERS
    // ═══════════════════════════════════════════════════════════════
    
    private void ParseAddressComponents(List<GoogleAddressComponent>? components, GeocodingResult result)
    {
        if (components == null) return;
        
        foreach (var comp in components)
        {
            if (comp.types == null) continue;
            
            if (comp.types.Contains("street_number"))
                result.StreetNumber = comp.long_name;
            else if (comp.types.Contains("route"))
                result.Route = comp.long_name;
            else if (comp.types.Contains("locality"))
                result.Locality = comp.long_name;
            else if (comp.types.Contains("administrative_area_level_1"))
                result.AdminAreaLevel1 = comp.long_name;
            else if (comp.types.Contains("administrative_area_level_2"))
                result.AdminAreaLevel2 = comp.long_name;
            else if (comp.types.Contains("country"))
            {
                result.Country = comp.long_name;
                result.CountryCode = comp.short_name;
            }
            else if (comp.types.Contains("postal_code"))
                result.PostalCode = comp.long_name;
        }
    }
    
    private List<string> ParseCsvLine(string line, string delimiter)
    {
        var values = new List<string>();
        var inQuotes = false;
        var current = "";
        
        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];
            
            if (c == '"')
                inQuotes = !inQuotes;
            else if (c.ToString() == delimiter && !inQuotes)
            {
                values.Add(current.Trim());
                current = "";
            }
            else
                current += c;
        }
        
        values.Add(current.Trim());
        return values;
    }
    
    private string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            return $"\"{field.Replace("\"", "\"\"")}\"";
        return field;
    }
    
    private string GetResultProperty(GeocodingResult? result, string propertyName)
    {
        if (result == null) return "";
        
        return propertyName switch
        {
            "FormattedAddress" => result.FormattedAddress,
            "Latitude" => result.Latitude.ToString("F6"),
            "Longitude" => result.Longitude.ToString("F6"),
            "StreetNumber" => result.StreetNumber,
            "Route" => result.Route,
            "Locality" => result.Locality,
            "AdminAreaLevel1" => result.AdminAreaLevel1,
            "AdminAreaLevel2" => result.AdminAreaLevel2,
            "Country" => result.Country,
            "CountryCode" => result.CountryCode,
            "PostalCode" => result.PostalCode,
            "PlaceId" => result.PlaceId,
            "LocationType" => result.LocationType,
            _ => ""
        };
    }
    
    // ═══════════════════════════════════════════════════════════════
    // DEMO MODE
    // ═══════════════════════════════════════════════════════════════
    
    private GeocodingResult SimulateGeocoding(GeocodingRequest request)
    {
        var hash = request.FullAddress.GetHashCode();
        return new GeocodingResult
        {
            RequestId = request.Id,
            Success = true,
            Status = "OK",
            FormattedAddress = request.FullAddress,
            Latitude = 19.4326 + (hash % 1000) / 10000.0,
            Longitude = -99.1332 + (hash % 500) / 10000.0,
            PlaceId = $"demo_{Math.Abs(hash)}",
            Locality = request.City,
            AdminAreaLevel1 = request.State,
            Country = request.Country,
            PostalCode = request.PostalCode,
            LocationType = "APPROXIMATE"
        };
    }
    
    private GeocodingResult SimulateReverseGeocoding(ReverseGeocodingRequest request)
    {
        return new GeocodingResult
        {
            RequestId = request.Id,
            Success = true,
            Status = "OK",
            FormattedAddress = $"Demo Address at {request.Latitude:F4}, {request.Longitude:F4}",
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            PlaceId = $"demo_reverse_{request.Latitude:F2}_{request.Longitude:F2}",
            Locality = "Demo City",
            AdminAreaLevel1 = "Demo State",
            Country = "Demo Country",
            CountryCode = "DC",
            PostalCode = "00000",
            LocationType = "APPROXIMATE"
        };
    }
}
