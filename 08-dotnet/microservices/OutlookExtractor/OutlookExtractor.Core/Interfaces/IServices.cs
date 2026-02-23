using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OutlookExtractor.Core.Models;

namespace OutlookExtractor.Core.Interfaces;

/// <summary>
/// Microsoft Graph Service Interface
/// Handles authentication and connection to Microsoft 365
/// </summary>
public interface IMicrosoftGraphService
{
    Task<bool> AuthenticateAsync(string tenantId, string clientId, string clientSecret);
    Task<bool> AuthenticateInteractiveAsync(string clientId);
    Task<bool> IsAuthenticatedAsync();
    Task<string> GetUserEmailAsync();
    Task<string> GetUserDisplayNameAsync();
}

/// <summary>
/// Email Extraction Service Interface
/// Extracts email addresses from various sources
/// </summary>
public interface IEmailExtractionService
{
    Task<List<ExtractedEmail>> ExtractFromEmailsAsync(ExtractionConfig config);
    Task<List<ExtractedEmail>> ExtractFromCalendarAsync(ExtractionConfig config);
    Task<List<ExtractedEmail>> ExtractFromContactsAsync(ExtractionConfig config);
    Task<ExtractionSummary> ExtractAllAsync(ExtractionConfig config);
    Task<List<ExtractedEmail>> GetAllExtractedEmailsAsync();
    Task ClearExtractedDataAsync();
}

/// <summary>
/// Export Service Interface
/// Handles exporting extracted emails to files
/// </summary>
public interface IExportService
{
    Task<ExportResult> ExportToTextAsync(List<ExtractedEmail> emails, string outputPath);
    Task<ExportResult> ExportToExcelAsync(List<ExtractedEmail> emails, string outputPath);
    Task<ExportResult> ExportAsync(List<ExtractedEmail> emails, string outputDirectory, ExportFormat format);
}

/// <summary>
/// Statistics Service Interface
/// Provides analytics on extracted data
/// </summary>
public interface IStatisticsService
{
    Task<Dictionary<string, int>> GetEmailsByDomainAsync(List<ExtractedEmail> emails);
    Task<Dictionary<string, int>> GetEmailsBySourceAsync(List<ExtractedEmail> emails);
    Task<List<ExtractedEmail>> GetTopFrequentEmailsAsync(List<ExtractedEmail> emails, int count);
    Task<ExtractionSummary> GenerateSummaryAsync(List<ExtractedEmail> emails, DateTime startTime, DateTime endTime);
}
