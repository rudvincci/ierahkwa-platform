namespace Mamey.Government.UI.Models;

/// <summary>
/// Dashboard statistics aggregated from multiple services.
/// </summary>
public class DashboardStats
{
    // Application stats
    public int PendingApplications { get; set; }
    public int InReviewApplications { get; set; }
    public int ApprovedApplications { get; set; }
    public int RejectedApplications { get; set; }
    public List<ApplicationSummaryModel> RecentApplications { get; set; } = new();
    
    // Citizen stats
    public int TotalCitizens { get; set; }
    public int ProbationaryCitizens { get; set; }
    public int ResidentCitizens { get; set; }
    public int FullCitizens { get; set; }
    
    // Passport stats
    public int ActivePassports { get; set; }
    public int ExpiringPassports { get; set; }
    public int ExpiredPassports { get; set; }
    
    // Travel ID stats
    public int ActiveTravelIds { get; set; }
    public int ExpiringTravelIds { get; set; }
    public int ExpiredTravelIds { get; set; }
    
    // Certificate stats
    public int ActiveCertificates { get; set; }
    public int ArchivedCertificates { get; set; }
    public int RevokedCertificates { get; set; }
    
    // Summary stats
    public int DocumentsIssued { get; set; }
    public int ProgressionRequests { get; set; }
}
