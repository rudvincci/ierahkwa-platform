namespace NET10.Core.Interfaces;

// ═══════════════════════════════════════════════════════════════════════════════
// GOOGLE MAPS DATA SCRAPER PRO - COMPLETE IMPLEMENTATION
// Professional data scraper for Google Maps
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Google Maps Scraping Service
/// </summary>
public interface IGoogleMapsScraperService
{
    Task<ScrapingProject> CreateProjectAsync(CreateScrapingProjectRequest request);
    Task<ScrapingProject?> GetProjectAsync(Guid projectId);
    Task<List<ScrapingProject>> GetAllProjectsAsync();
    Task<bool> DeleteProjectAsync(Guid projectId);
    
    Task<ScrapingProject> StartScrapingAsync(Guid projectId);
    Task<ScrapingProject> StopScrapingAsync(Guid projectId);
    Task<ScrapingProject> PauseScrapingAsync(Guid projectId);
    Task<ScrapingProject> ResumeScrapingAsync(Guid projectId);
    
    Task<ScrapingProgress> GetProgressAsync(Guid projectId);
    Task<List<ScrapedOrganization>> GetOrganizationsAsync(Guid projectId, int page = 1, int pageSize = 50);
    Task<ScrapedOrganization?> GetOrganizationAsync(Guid organizationId);
    Task<List<Review>> GetOrganizationReviewsAsync(Guid organizationId);
    
    Task<ExportResult> ExportToXlsxAsync(Guid projectId, string? outputPath = null);
    Task<ExportResult> ExportToCsvAsync(Guid projectId, string? outputPath = null);
    Task<ExportResult> ExportToJsonAsync(Guid projectId, string? outputPath = null);
    Task<ExportResult> ExportToMySqlAsync(Guid projectId, string connectionString);
    Task<ExportResult> ExportToXmlAsync(Guid projectId, string? outputPath = null);
}

// ═══════════════════════════════════════════════════════════════════════════════
// SCRAPING MODELS
// ═══════════════════════════════════════════════════════════════════════════════

public class ScrapingProject
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public List<string> GeoLocations { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public ScrapingStatus Status { get; set; } = ScrapingStatus.Pending;
    public ScrapingSettings Settings { get; set; } = new();
    public ScrapingProgress Progress { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public enum ScrapingStatus
{
    Pending,
    Running,
    Paused,
    Completed,
    Failed,
    Stopped
}

public class ScrapingSettings
{
    public int MaxOrganizations { get; set; } = 100;
    public int MaxOrganizationsPerSegment { get; set; } = 0; // 0 = unlimited
    public bool SkipDuplicates { get; set; } = true;
    public bool ScrapeReviews { get; set; } = true;
    public bool ScrapePhotos { get; set; } = true;
    public bool ScrapeWebsiteData { get; set; } = true;
    public bool ScrapeAllReviews { get; set; } = true;
    
    // Filters
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public bool? HasReviews { get; set; }
    public int? MinReviewLength { get; set; }
    public int? MaxReviewLength { get; set; }
    public string? OrganizationNameFilter { get; set; }
    
    // Website scraping
    public List<string> ContactPageSubstrings { get; set; } = new() { "contact", "contacts", "kontakt" };
    
    // Proxy
    public ProxySettings? Proxy { get; set; }
}

public class ProxySettings
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public ProxyType Type { get; set; } = ProxyType.Http;
}

public enum ProxyType
{
    Http,
    Https,
    Socks4,
    Socks5
}

public class ScrapingProgress
{
    public int TotalOrganizations { get; set; }
    public int ScrapedOrganizations { get; set; }
    public int FailedOrganizations { get; set; }
    public int TotalReviews { get; set; }
    public int ScrapedReviews { get; set; }
    public int TotalPhotos { get; set; }
    public int DownloadedPhotos { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public TimeSpan? EstimatedTimeRemaining { get; set; }
    public double ProgressPercent { get; set; }
    public string CurrentAction { get; set; } = string.Empty;
}

public class CreateScrapingProjectRequest
{
    public string Name { get; set; } = string.Empty;
    public List<string> GeoLocations { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
    public ScrapingSettings? Settings { get; set; }
}

public class ScrapedOrganization
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public string OrganizationGUID { get; set; } = Guid.NewGuid().ToString();
    public string OrganizationGMapsId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal? Rating { get; set; }
    public int ReviewCount { get; set; }
    public string? PricePolicy { get; set; } // $, $$, $$$, $$$$
    public string? Category { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Area { get; set; }
    public string? Website { get; set; }
    public string? Phone { get; set; }
    public string? PlusCode { get; set; }
    public List<WorkingHours>? WorkingHours { get; set; }
    public bool? IsOpen { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Description { get; set; }
    public string? GoogleMapsUrl { get; set; }
    public string? ShareLink { get; set; }
    public string? EmbedCode { get; set; }
    public Dictionary<string, int>? PopularHours { get; set; }
    public int PositionInResults { get; set; }
    public int SearchQueryOrder { get; set; }
    
    // Website scraped data
    public string? Email { get; set; }
    public string? FacebookUrl { get; set; }
    public string? InstagramUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public string? YelpUrl { get; set; }
    public string? TripAdvisorUrl { get; set; }
    public string? TikTokUrl { get; set; }
    public string? Skype { get; set; }
    public string? Telegram { get; set; }
    public string? ContactPageUrl { get; set; }
    public List<string>? WebsitePhones { get; set; }
    public string? MenuUrl { get; set; } // For restaurants
    
    // Related data
    public List<Review> Reviews { get; set; } = new();
    public List<string> PhotoUrls { get; set; } = new();
    public List<string> PhotoPaths { get; set; } = new(); // Local file paths
    
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;
}

public class WorkingHours
{
    public DayOfWeek Day { get; set; }
    public string? Hours { get; set; }
    public bool IsClosed { get; set; }
}

public class Review
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public string OrganizationGUID { get; set; } = string.Empty;
    public string OrganizationGMapsId { get; set; } = string.Empty;
    public string ReviewerName { get; set; } = string.Empty;
    public string? ReviewerProfileUrl { get; set; }
    public int? ReviewerRank { get; set; }
    public int? ReviewerReviewCount { get; set; }
    public int? ReviewerPhotoCount { get; set; }
    public decimal Rating { get; set; }
    public string Text { get; set; } = string.Empty;
    public string? FullText { get; set; }
    public int? Likes { get; set; }
    public DateTime? ReviewDate { get; set; }
    public List<string> PhotoUrls { get; set; } = new();
    public List<string> PhotoPaths { get; set; } = new();
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;
}

public class ExportResult
{
    public bool Success { get; set; }
    public string? FilePath { get; set; }
    public string? DirectoryPath { get; set; }
    public int OrganizationsExported { get; set; }
    public int ReviewsExported { get; set; }
    public int PhotosExported { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ExportedAt { get; set; } = DateTime.UtcNow;
}
