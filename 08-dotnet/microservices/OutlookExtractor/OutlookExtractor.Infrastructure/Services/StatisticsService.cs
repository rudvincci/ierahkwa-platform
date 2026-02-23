using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OutlookExtractor.Core.Interfaces;
using OutlookExtractor.Core.Models;

namespace OutlookExtractor.Infrastructure.Services;

/// <summary>
/// Statistics Service Implementation
/// Provides analytics on extracted email data
/// IERAHKWA Platform - Sovereign Government Tool
/// </summary>
public class StatisticsService : IStatisticsService
{
    public Task<Dictionary<string, int>> GetEmailsByDomainAsync(List<ExtractedEmail> emails)
    {
        var domainStats = emails
            .GroupBy(e => GetDomain(e.EmailAddress))
            .ToDictionary(g => g.Key, g => g.Count())
            .OrderByDescending(kvp => kvp.Value)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return Task.FromResult(domainStats);
    }

    public Task<Dictionary<string, int>> GetEmailsBySourceAsync(List<ExtractedEmail> emails)
    {
        var sourceStats = emails
            .GroupBy(e => e.Source)
            .ToDictionary(g => g.Key, g => g.Count())
            .OrderByDescending(kvp => kvp.Value)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return Task.FromResult(sourceStats);
    }

    public Task<List<ExtractedEmail>> GetTopFrequentEmailsAsync(List<ExtractedEmail> emails, int count)
    {
        var topEmails = emails
            .OrderByDescending(e => e.Frequency)
            .ThenBy(e => e.EmailAddress)
            .Take(count)
            .ToList();

        return Task.FromResult(topEmails);
    }

    public Task<ExtractionSummary> GenerateSummaryAsync(List<ExtractedEmail> emails, DateTime startTime, DateTime endTime)
    {
        var summary = new ExtractionSummary
        {
            StartTime = startTime,
            EndTime = endTime,
            Duration = endTime - startTime,
            TotalEmailsFound = emails.Count,
            UniqueEmailsFound = emails.Select(e => e.EmailAddress.ToLower()).Distinct().Count(),
            EmailsBySource = emails
                .GroupBy(e => e.Source)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        var emailSources = emails.Where(e => e.Source == "Email").ToList();
        var calendarSources = emails.Where(e => e.Source == "Calendar").ToList();
        var contactSources = emails.Where(e => e.Source == "Contact").ToList();

        summary.EmailsScanned = emailSources.Count;
        summary.CalendarEventsScanned = calendarSources.Count;
        summary.ContactsScanned = contactSources.Count;

        return Task.FromResult(summary);
    }

    private string GetDomain(string email)
    {
        try
        {
            var atIndex = email.LastIndexOf('@');
            if (atIndex > 0 && atIndex < email.Length - 1)
            {
                return email.Substring(atIndex + 1).ToLower();
            }
            return "unknown";
        }
        catch
        {
            return "unknown";
        }
    }
}
