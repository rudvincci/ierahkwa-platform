using System;

namespace OutlookExtractor.Core.Models;

/// <summary>
/// Represents an extracted email address with metadata
/// </summary>
public class ExtractedEmail
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string EmailAddress { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty; // "Email", "Calendar", "Contacts"
    public string SourceId { get; set; } = string.Empty; // Original item ID
    public DateTime ExtractedAt { get; set; } = DateTime.UtcNow;
    public int Frequency { get; set; } = 1; // How many times this email appeared
    
    public EmailMetadata? Metadata { get; set; }
}

/// <summary>
/// Additional metadata about the email extraction
/// </summary>
public class EmailMetadata
{
    public string? Subject { get; set; }
    public DateTime? MessageDate { get; set; }
    public string? FolderPath { get; set; }
    public string? EventTitle { get; set; }
    public DateTime? EventDate { get; set; }
    public bool IsOrganizer { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// Summary statistics of the extraction process
/// </summary>
public class ExtractionSummary
{
    public int TotalEmailsFound { get; set; }
    public int UniqueEmailsFound { get; set; }
    public int EmailsScanned { get; set; }
    public int CalendarEventsScanned { get; set; }
    public int ContactsScanned { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration { get; set; }
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, int> EmailsBySource { get; set; } = new();
}

/// <summary>
/// Configuration for the extraction process
/// </summary>
public class ExtractionConfig
{
    public bool IncludeEmails { get; set; } = true;
    public bool IncludeCalendar { get; set; } = true;
    public bool IncludeContacts { get; set; } = true;
    public bool IncludeSent { get; set; } = true;
    public bool IncludeInbox { get; set; } = true;
    public bool IncludeArchive { get; set; } = false;
    public bool IncludeDeleted { get; set; } = false;
    
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public int MaxEmailsToScan { get; set; } = 10000;
    public int MaxCalendarEvents { get; set; } = 5000;
    
    public bool RemoveDuplicates { get; set; } = true;
    public bool IncludeMetadata { get; set; } = true;
    
    public List<string> ExcludeDomains { get; set; } = new();
    public List<string> IncludeOnlyDomains { get; set; } = new();
}

/// <summary>
/// Export format options
/// </summary>
public enum ExportFormat
{
    Text,
    Excel,
    Both
}

/// <summary>
/// Export result information
/// </summary>
public class ExportResult
{
    public bool Success { get; set; }
    public string? TextFilePath { get; set; }
    public string? ExcelFilePath { get; set; }
    public string? Message { get; set; }
    public int RecordsExported { get; set; }
}
