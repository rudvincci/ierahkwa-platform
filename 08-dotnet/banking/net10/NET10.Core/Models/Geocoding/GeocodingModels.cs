using System;
using System.Collections.Generic;

namespace NET10.Core.Models.Geocoding
{
    /// <summary>
    /// Geocoding Request - Address to Lat/Lng
    /// </summary>
    public class GeocodingRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Address { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        
        public string FullAddress => string.IsNullOrEmpty(Address) 
            ? $"{Street}, {City}, {State}, {Country} {PostalCode}".Trim(' ', ',')
            : Address;
    }
    
    /// <summary>
    /// Reverse Geocoding Request - Lat/Lng to Address
    /// </summary>
    public class ReverseGeocodingRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Language { get; set; } = "en";
    }
    
    /// <summary>
    /// Geocoding Result
    /// </summary>
    public class GeocodingResult
    {
        public Guid RequestId { get; set; }
        public bool Success { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        
        // Location
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        
        // Address Components
        public string FormattedAddress { get; set; } = string.Empty;
        public string StreetNumber { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Locality { get; set; } = string.Empty;
        public string AdminAreaLevel1 { get; set; } = string.Empty;
        public string AdminAreaLevel2 { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        
        // Metadata
        public string PlaceId { get; set; } = string.Empty;
        public string LocationType { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
    
    /// <summary>
    /// Batch Processing Item
    /// </summary>
    public class GeocodingBatchItem
    {
        public int RowNumber { get; set; }
        public GeocodingRequest? GeoRequest { get; set; }
        public ReverseGeocodingRequest? ReverseRequest { get; set; }
        public GeocodingResult? Result { get; set; }
        public ProcessingStatus Status { get; set; } = ProcessingStatus.Pending;
        public string StatusDescription { get; set; } = "Not processed";
        public Dictionary<string, string> OriginalData { get; set; } = new();
    }
    
    public enum ProcessingStatus
    {
        Pending,      // White - Not processed
        InProgress,   // Blue - Processing
        Success,      // Green - Completed
        Failed,       // Red - Error
        Retry         // Yellow - Needs retry
    }
    
    /// <summary>
    /// Batch Processing Job
    /// </summary>
    public class GeocodingBatch
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public GeocodingMode Mode { get; set; }
        public List<GeocodingBatchItem> Items { get; set; } = new();
        public GeocodingOptions Options { get; set; } = new();
        
        // Progress
        public int TotalRows => Items.Count;
        public int ProcessedRows => Items.Count(i => i.Status == ProcessingStatus.Success || i.Status == ProcessingStatus.Failed);
        public int SuccessRows => Items.Count(i => i.Status == ProcessingStatus.Success);
        public int FailedRows => Items.Count(i => i.Status == ProcessingStatus.Failed);
        public double ProgressPercent => TotalRows > 0 ? (ProcessedRows * 100.0 / TotalRows) : 0;
        
        // Timestamps
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public BatchStatus BatchStatus { get; set; } = BatchStatus.Created;
    }
    
    public enum GeocodingMode
    {
        Geocoding,        // Address to Lat/Lng
        ReverseGeocoding  // Lat/Lng to Address
    }
    
    public enum BatchStatus
    {
        Created,
        Processing,
        Completed,
        Cancelled,
        Error
    }
    
    /// <summary>
    /// Geocoding Options
    /// </summary>
    public class GeocodingOptions
    {
        // API Settings
        public string ApiKey { get; set; } = string.Empty;
        public string ApiProvider { get; set; } = "Google"; // Google, OpenStreetMap, etc.
        
        // CSV Settings
        public string Delimiter { get; set; } = ",";
        public string Encoding { get; set; } = "UTF-8";
        public bool UseHeaderRow { get; set; } = true;
        public bool UseCultureSpecificDelimiter { get; set; } = false;
        
        // Geocoding Fields (column names or indices)
        public string AddressField { get; set; } = string.Empty;
        public string StreetField { get; set; } = string.Empty;
        public string CityField { get; set; } = string.Empty;
        public string StateField { get; set; } = string.Empty;
        public string CountryField { get; set; } = string.Empty;
        public string PostalCodeField { get; set; } = string.Empty;
        
        // Reverse Geocoding Fields
        public string LatitudeField { get; set; } = string.Empty;
        public string LongitudeField { get; set; } = string.Empty;
        
        // Output Settings
        public string Language { get; set; } = "en";
        public List<string> ResultProperties { get; set; } = new()
        {
            "FormattedAddress",
            "Latitude",
            "Longitude",
            "Locality",
            "Country",
            "PostalCode"
        };
        
        // Processing Settings
        public int DelayBetweenRequests { get; set; } = 100; // milliseconds
        public int MaxRetries { get; set; } = 3;
        public bool StopOnError { get; set; } = false;
    }
    
    /// <summary>
    /// CSV Import Result
    /// </summary>
    public class CsvImportResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> Headers { get; set; } = new();
        public List<Dictionary<string, string>> Rows { get; set; } = new();
        public int TotalRows { get; set; }
    }
    
    /// <summary>
    /// Google Geocoding API Response Models
    /// </summary>
    public class GoogleGeocodingResponse
    {
        public string status { get; set; } = string.Empty;
        public List<GoogleGeocodingResult> results { get; set; } = new();
        public string error_message { get; set; } = string.Empty;
    }
    
    public class GoogleGeocodingResult
    {
        public string formatted_address { get; set; } = string.Empty;
        public GoogleGeometry geometry { get; set; } = new();
        public List<GoogleAddressComponent> address_components { get; set; } = new();
        public string place_id { get; set; } = string.Empty;
        public List<string> types { get; set; } = new();
    }
    
    public class GoogleGeometry
    {
        public GoogleLocation location { get; set; } = new();
        public string location_type { get; set; } = string.Empty;
    }
    
    public class GoogleLocation
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
    
    public class GoogleAddressComponent
    {
        public string long_name { get; set; } = string.Empty;
        public string short_name { get; set; } = string.Empty;
        public List<string> types { get; set; } = new();
    }
}
