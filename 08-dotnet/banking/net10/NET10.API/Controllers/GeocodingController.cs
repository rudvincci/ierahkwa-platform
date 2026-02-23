using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.Geocoding;

namespace NET10.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GeocodingController : ControllerBase
    {
        private readonly IGeocodingService _geocodingService;
        
        public GeocodingController(IGeocodingService geocodingService)
        {
            _geocodingService = geocodingService;
        }
        
        /// <summary>
        /// Geocode a single address to lat/lng
        /// </summary>
        [HttpPost("geocode")]
        public async Task<ActionResult<GeocodingResult>> Geocode([FromBody] GeocodeSingleRequest request)
        {
            var geoRequest = new GeocodingRequest
            {
                Address = request.Address,
                Street = request.Street,
                City = request.City,
                State = request.State,
                Country = request.Country,
                PostalCode = request.PostalCode
            };
            
            var options = new GeocodingOptions
            {
                ApiKey = request.ApiKey,
                Language = request.Language ?? "en"
            };
            
            var result = await _geocodingService.GeocodeAsync(geoRequest, options);
            return Ok(result);
        }
        
        /// <summary>
        /// Reverse geocode lat/lng to address
        /// </summary>
        [HttpPost("reverse")]
        public async Task<ActionResult<GeocodingResult>> ReverseGeocode([FromBody] ReverseGeocodeSingleRequest request)
        {
            var reverseRequest = new ReverseGeocodingRequest
            {
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Language = request.Language ?? "en"
            };
            
            var options = new GeocodingOptions
            {
                ApiKey = request.ApiKey,
                Language = request.Language ?? "en"
            };
            
            var result = await _geocodingService.ReverseGeocodeAsync(reverseRequest, options);
            return Ok(result);
        }
        
        /// <summary>
        /// Import CSV and create batch
        /// </summary>
        [HttpPost("batch/import")]
        public async Task<ActionResult<BatchImportResponse>> ImportCsv([FromBody] CsvImportRequest request)
        {
            var options = new GeocodingOptions
            {
                ApiKey = request.ApiKey,
                Delimiter = request.Delimiter ?? ",",
                UseHeaderRow = request.UseHeaderRow,
                AddressField = request.AddressField,
                StreetField = request.StreetField,
                CityField = request.CityField,
                StateField = request.StateField,
                CountryField = request.CountryField,
                PostalCodeField = request.PostalCodeField,
                LatitudeField = request.LatitudeField,
                LongitudeField = request.LongitudeField,
                Language = request.Language ?? "en"
            };
            
            var importResult = await _geocodingService.ImportCsvFromContentAsync(request.CsvContent, options);
            
            if (!importResult.Success)
            {
                return BadRequest(new { error = importResult.ErrorMessage });
            }
            
            var mode = request.Mode == "reverse" ? GeocodingMode.ReverseGeocoding : GeocodingMode.Geocoding;
            var batch = await _geocodingService.CreateBatchAsync(request.Name ?? "Batch", mode, options);
            
            // Create batch items from CSV rows
            int rowNum = 1;
            foreach (var row in importResult.Rows)
            {
                var item = new GeocodingBatchItem
                {
                    RowNumber = rowNum++,
                    OriginalData = row
                };
                
                if (mode == GeocodingMode.Geocoding)
                {
                    item.GeoRequest = new GeocodingRequest
                    {
                        Address = GetFieldValue(row, options.AddressField),
                        Street = GetFieldValue(row, options.StreetField),
                        City = GetFieldValue(row, options.CityField),
                        State = GetFieldValue(row, options.StateField),
                        Country = GetFieldValue(row, options.CountryField),
                        PostalCode = GetFieldValue(row, options.PostalCodeField)
                    };
                }
                else
                {
                    var latStr = GetFieldValue(row, options.LatitudeField);
                    var lngStr = GetFieldValue(row, options.LongitudeField);
                    
                    if (double.TryParse(latStr, out var lat) && double.TryParse(lngStr, out var lng))
                    {
                        item.ReverseRequest = new ReverseGeocodingRequest
                        {
                            Latitude = lat,
                            Longitude = lng,
                            Language = options.Language
                        };
                    }
                    else
                    {
                        item.Status = ProcessingStatus.Failed;
                        item.StatusDescription = "Invalid latitude/longitude values";
                    }
                }
                
                batch.Items.Add(item);
            }
            
            return Ok(new BatchImportResponse
            {
                BatchId = batch.Id,
                TotalRows = batch.TotalRows,
                Headers = importResult.Headers,
                Mode = mode.ToString()
            });
        }
        
        /// <summary>
        /// Process batch
        /// </summary>
        [HttpPost("batch/{batchId}/process")]
        public async Task<ActionResult<GeocodingBatch>> ProcessBatch(Guid batchId)
        {
            var batch = await _geocodingService.ProcessBatchAsync(batchId);
            return Ok(batch);
        }
        
        /// <summary>
        /// Get batch status
        /// </summary>
        [HttpGet("batch/{batchId}")]
        public async Task<ActionResult<GeocodingBatch>> GetBatch(Guid batchId)
        {
            var batch = await _geocodingService.GetBatchAsync(batchId);
            if (batch == null)
                return NotFound();
            return Ok(batch);
        }
        
        /// <summary>
        /// Cancel batch processing
        /// </summary>
        [HttpPost("batch/{batchId}/cancel")]
        public async Task<ActionResult> CancelBatch(Guid batchId)
        {
            var result = await _geocodingService.CancelBatchAsync(batchId);
            return result ? Ok() : NotFound();
        }
        
        /// <summary>
        /// Retry failed items
        /// </summary>
        [HttpPost("batch/{batchId}/retry")]
        public async Task<ActionResult> RetryFailed(Guid batchId)
        {
            var result = await _geocodingService.RetryFailedAsync(batchId);
            return result ? Ok() : NotFound();
        }
        
        /// <summary>
        /// Export batch to CSV
        /// </summary>
        [HttpGet("batch/{batchId}/export")]
        public async Task<ActionResult> ExportBatch(Guid batchId)
        {
            var batch = await _geocodingService.GetBatchAsync(batchId);
            if (batch == null)
                return NotFound();
            
            var csv = await _geocodingService.ExportToCsvAsync(batch, batch.Options);
            return Content(csv, "text/csv", System.Text.Encoding.UTF8);
        }
        
        /// <summary>
        /// Validate API key
        /// </summary>
        [HttpPost("validate-key")]
        public async Task<ActionResult<bool>> ValidateApiKey([FromBody] ValidateKeyRequest request)
        {
            var isValid = await _geocodingService.ValidateApiKeyAsync(request.ApiKey);
            return Ok(new { valid = isValid });
        }
        
        /// <summary>
        /// Get supported languages
        /// </summary>
        [HttpGet("languages")]
        public ActionResult<List<string>> GetLanguages()
        {
            return Ok(_geocodingService.GetSupportedLanguages());
        }
        
        /// <summary>
        /// Get available result properties
        /// </summary>
        [HttpGet("properties")]
        public ActionResult<List<string>> GetProperties()
        {
            return Ok(_geocodingService.GetAvailableResultProperties());
        }
        
        private string GetFieldValue(Dictionary<string, string> row, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName)) return string.Empty;
            return row.TryGetValue(fieldName, out var value) ? value : string.Empty;
        }
    }
    
    // Request DTOs
    public class GeocodeSingleRequest
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string? Language { get; set; }
    }
    
    public class ReverseGeocodeSingleRequest
    {
        public string ApiKey { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Language { get; set; }
    }
    
    public class CsvImportRequest
    {
        public string ApiKey { get; set; } = string.Empty;
        public string CsvContent { get; set; } = string.Empty;
        public string Mode { get; set; } = "geocode"; // geocode or reverse
        public string? Name { get; set; }
        public string? Delimiter { get; set; }
        public bool UseHeaderRow { get; set; } = true;
        public string AddressField { get; set; } = string.Empty;
        public string StreetField { get; set; } = string.Empty;
        public string CityField { get; set; } = string.Empty;
        public string StateField { get; set; } = string.Empty;
        public string CountryField { get; set; } = string.Empty;
        public string PostalCodeField { get; set; } = string.Empty;
        public string LatitudeField { get; set; } = string.Empty;
        public string LongitudeField { get; set; } = string.Empty;
        public string? Language { get; set; }
    }
    
    public class ValidateKeyRequest
    {
        public string ApiKey { get; set; } = string.Empty;
    }
    
    public class BatchImportResponse
    {
        public Guid BatchId { get; set; }
        public int TotalRows { get; set; }
        public List<string> Headers { get; set; } = new();
        public string Mode { get; set; } = string.Empty;
    }
}
