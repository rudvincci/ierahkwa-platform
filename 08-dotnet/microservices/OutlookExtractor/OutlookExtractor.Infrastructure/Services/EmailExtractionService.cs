using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using OutlookExtractor.Core.Interfaces;
using OutlookExtractor.Core.Models;

namespace OutlookExtractor.Infrastructure.Services;

/// <summary>
/// Email Extraction Service Implementation
/// Extracts email addresses from Office 365/Outlook emails, calendars, and contacts
/// IERAHKWA Platform - Sovereign Government Tool
/// </summary>
public class EmailExtractionService : IEmailExtractionService
{
    private readonly MicrosoftGraphService _graphService;
    private readonly List<ExtractedEmail> _extractedEmails = new();
    private static readonly Regex EmailRegex = new(
        @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public EmailExtractionService(MicrosoftGraphService graphService)
    {
        _graphService = graphService;
    }

    public async Task<List<ExtractedEmail>> ExtractFromEmailsAsync(ExtractionConfig config)
    {
        var emails = new List<ExtractedEmail>();
        var graphClient = _graphService.GetGraphClient();

        try
        {
            var folders = new List<string>();
            if (config.IncludeInbox) folders.Add("inbox");
            if (config.IncludeSent) folders.Add("sentitems");
            if (config.IncludeArchive) folders.Add("archive");
            if (config.IncludeDeleted) folders.Add("deleteditems");

            foreach (var folderName in folders)
            {
                var messages = await graphClient.Me.MailFolders[folderName].Messages
                    .GetAsync(requestConfig =>
                    {
                        requestConfig.QueryParameters.Top = config.MaxEmailsToScan;
                        requestConfig.QueryParameters.Select = new[] { "from", "toRecipients", "ccRecipients", "bccRecipients", "subject", "receivedDateTime" };
                    });

                if (messages?.Value != null)
                {
                    foreach (var message in messages.Value)
                    {
                        if (config.StartDate.HasValue && message.ReceivedDateTime < config.StartDate.Value)
                            continue;
                        if (config.EndDate.HasValue && message.ReceivedDateTime > config.EndDate.Value)
                            continue;

                        // Extract from sender
                        if (message.From?.EmailAddress != null)
                        {
                            emails.Add(CreateExtractedEmail(
                                message.From.EmailAddress.Address ?? "",
                                message.From.EmailAddress.Name ?? "",
                                "Email",
                                message.Id ?? "",
                                new EmailMetadata
                                {
                                    Subject = message.Subject,
                                    MessageDate = message.ReceivedDateTime?.DateTime,
                                    FolderPath = folderName
                                }
                            ));
                        }

                        // Extract from recipients
                        if (message.ToRecipients != null)
                        {
                            foreach (var recipient in message.ToRecipients)
                            {
                                if (recipient.EmailAddress != null)
                                {
                                    emails.Add(CreateExtractedEmail(
                                        recipient.EmailAddress.Address ?? "",
                                        recipient.EmailAddress.Name ?? "",
                                        "Email",
                                        message.Id ?? "",
                                        new EmailMetadata
                                        {
                                            Subject = message.Subject,
                                            MessageDate = message.ReceivedDateTime?.DateTime,
                                            FolderPath = folderName
                                        }
                                    ));
                                }
                            }
                        }

                        // Extract from CC
                        if (message.CcRecipients != null)
                        {
                            foreach (var recipient in message.CcRecipients)
                            {
                                if (recipient.EmailAddress != null)
                                {
                                    emails.Add(CreateExtractedEmail(
                                        recipient.EmailAddress.Address ?? "",
                                        recipient.EmailAddress.Name ?? "",
                                        "Email",
                                        message.Id ?? "",
                                        new EmailMetadata
                                        {
                                            Subject = message.Subject,
                                            MessageDate = message.ReceivedDateTime?.DateTime,
                                            FolderPath = folderName
                                        }
                                    ));
                                }
                            }
                        }
                    }
                }
            }

            if (config.RemoveDuplicates)
            {
                emails = RemoveDuplicates(emails);
            }

            _extractedEmails.AddRange(emails);
            return emails;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting from emails: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ExtractedEmail>> ExtractFromCalendarAsync(ExtractionConfig config)
    {
        var emails = new List<ExtractedEmail>();
        var graphClient = _graphService.GetGraphClient();

        try
        {
            var events = await graphClient.Me.Calendar.Events
                .GetAsync(requestConfig =>
                {
                    requestConfig.QueryParameters.Top = config.MaxCalendarEvents;
                    requestConfig.QueryParameters.Select = new[] { "subject", "organizer", "attendees", "start" };
                });

            if (events?.Value != null)
            {
                foreach (var evt in events.Value)
                {
                    if (config.StartDate.HasValue && evt.Start?.DateTime != null)
                    {
                        var eventDate = DateTime.Parse(evt.Start.DateTime);
                        if (eventDate < config.StartDate.Value)
                            continue;
                    }

                    if (config.EndDate.HasValue && evt.Start?.DateTime != null)
                    {
                        var eventDate = DateTime.Parse(evt.Start.DateTime);
                        if (eventDate > config.EndDate.Value)
                            continue;
                    }

                    // Extract organizer
                    if (evt.Organizer?.EmailAddress != null)
                    {
                        emails.Add(CreateExtractedEmail(
                            evt.Organizer.EmailAddress.Address ?? "",
                            evt.Organizer.EmailAddress.Name ?? "",
                            "Calendar",
                            evt.Id ?? "",
                            new EmailMetadata
                            {
                                EventTitle = evt.Subject,
                                EventDate = evt.Start?.DateTime != null ? DateTime.Parse(evt.Start.DateTime) : null,
                                IsOrganizer = true
                            }
                        ));
                    }

                    // Extract attendees
                    if (evt.Attendees != null)
                    {
                        foreach (var attendee in evt.Attendees)
                        {
                            if (attendee.EmailAddress != null)
                            {
                                emails.Add(CreateExtractedEmail(
                                    attendee.EmailAddress.Address ?? "",
                                    attendee.EmailAddress.Name ?? "",
                                    "Calendar",
                                    evt.Id ?? "",
                                    new EmailMetadata
                                    {
                                        EventTitle = evt.Subject,
                                        EventDate = evt.Start?.DateTime != null ? DateTime.Parse(evt.Start.DateTime) : null,
                                        IsOrganizer = false
                                    }
                                ));
                            }
                        }
                    }
                }
            }

            if (config.RemoveDuplicates)
            {
                emails = RemoveDuplicates(emails);
            }

            _extractedEmails.AddRange(emails);
            return emails;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting from calendar: {ex.Message}");
            throw;
        }
    }

    public async Task<List<ExtractedEmail>> ExtractFromContactsAsync(ExtractionConfig config)
    {
        var emails = new List<ExtractedEmail>();
        var graphClient = _graphService.GetGraphClient();

        try
        {
            var contacts = await graphClient.Me.Contacts
                .GetAsync(requestConfig =>
                {
                    requestConfig.QueryParameters.Top = 1000;
                    requestConfig.QueryParameters.Select = new[] { "displayName", "emailAddresses", "companyName", "jobTitle", "mobilePhone" };
                });

            if (contacts?.Value != null)
            {
                foreach (var contact in contacts.Value)
                {
                    if (contact.EmailAddresses != null)
                    {
                        foreach (var emailAddr in contact.EmailAddresses)
                        {
                            if (!string.IsNullOrEmpty(emailAddr.Address))
                            {
                                emails.Add(CreateExtractedEmail(
                                    emailAddr.Address,
                                    contact.DisplayName ?? "",
                                    "Contact",
                                    contact.Id ?? "",
                                    new EmailMetadata
                                    {
                                        Company = contact.CompanyName,
                                        JobTitle = contact.JobTitle,
                                        PhoneNumber = contact.MobilePhone
                                    }
                                ));
                            }
                        }
                    }
                }
            }

            if (config.RemoveDuplicates)
            {
                emails = RemoveDuplicates(emails);
            }

            _extractedEmails.AddRange(emails);
            return emails;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting from contacts: {ex.Message}");
            throw;
        }
    }

    public async Task<ExtractionSummary> ExtractAllAsync(ExtractionConfig config)
    {
        var startTime = DateTime.UtcNow;
        var summary = new ExtractionSummary
        {
            StartTime = startTime
        };

        try
        {
            var allEmails = new List<ExtractedEmail>();

            if (config.IncludeEmails)
            {
                var emailExtracted = await ExtractFromEmailsAsync(config);
                allEmails.AddRange(emailExtracted);
                summary.EmailsScanned = emailExtracted.Count;
                summary.EmailsBySource["Email"] = emailExtracted.Count;
            }

            if (config.IncludeCalendar)
            {
                var calendarExtracted = await ExtractFromCalendarAsync(config);
                allEmails.AddRange(calendarExtracted);
                summary.CalendarEventsScanned = calendarExtracted.Count;
                summary.EmailsBySource["Calendar"] = calendarExtracted.Count;
            }

            if (config.IncludeContacts)
            {
                var contactsExtracted = await ExtractFromContactsAsync(config);
                allEmails.AddRange(contactsExtracted);
                summary.ContactsScanned = contactsExtracted.Count;
                summary.EmailsBySource["Contact"] = contactsExtracted.Count;
            }

            if (config.RemoveDuplicates)
            {
                allEmails = RemoveDuplicates(allEmails);
            }

            summary.TotalEmailsFound = allEmails.Count;
            summary.UniqueEmailsFound = allEmails.Select(e => e.EmailAddress.ToLower()).Distinct().Count();
            summary.EndTime = DateTime.UtcNow;
            summary.Duration = summary.EndTime - summary.StartTime;

            return summary;
        }
        catch (Exception ex)
        {
            summary.Errors.Add(ex.Message);
            summary.EndTime = DateTime.UtcNow;
            summary.Duration = summary.EndTime - summary.StartTime;
            return summary;
        }
    }

    public Task<List<ExtractedEmail>> GetAllExtractedEmailsAsync()
    {
        return Task.FromResult(_extractedEmails.ToList());
    }

    public Task ClearExtractedDataAsync()
    {
        _extractedEmails.Clear();
        return Task.CompletedTask;
    }

    private ExtractedEmail CreateExtractedEmail(string email, string displayName, string source, string sourceId, EmailMetadata? metadata)
    {
        return new ExtractedEmail
        {
            EmailAddress = email.ToLower().Trim(),
            DisplayName = displayName.Trim(),
            Source = source,
            SourceId = sourceId,
            ExtractedAt = DateTime.UtcNow,
            Metadata = metadata
        };
    }

    private List<ExtractedEmail> RemoveDuplicates(List<ExtractedEmail> emails)
    {
        var grouped = emails
            .GroupBy(e => e.EmailAddress.ToLower())
            .Select(g => new ExtractedEmail
            {
                EmailAddress = g.First().EmailAddress,
                DisplayName = g.First().DisplayName,
                Source = g.First().Source,
                SourceId = g.First().SourceId,
                ExtractedAt = g.First().ExtractedAt,
                Frequency = g.Count(),
                Metadata = g.First().Metadata
            })
            .ToList();

        return grouped;
    }
}
