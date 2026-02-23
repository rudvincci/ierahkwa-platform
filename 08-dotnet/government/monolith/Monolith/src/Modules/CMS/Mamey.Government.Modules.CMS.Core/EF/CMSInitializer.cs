using System.Linq;
using Mamey.Government.Modules.CMS.Core.Domain.Entities;
using Mamey.Government.Modules.CMS.Core.Domain.Repositories;
using Mamey.Government.Modules.CMS.Core.Domain.ValueObjects;
using Mamey.Government.Shared.Abstractions;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using GovTenantId = Mamey.Types.TenantId;

namespace Mamey.Government.Modules.CMS.Core.EF;

internal class CMSInitializer : IInitializer
{
    private readonly IContentRepository _contentRepository;
    private readonly ILogger<CMSInitializer> _logger;
    
    private static readonly GovTenantId TenantId = new(SeedData.TenantId);

    private static readonly string[] ContentTypes = { "page", "article", "news", "announcement", "faq" };
    
    private static readonly string[] Titles = {
        "Welcome to Government Services", "Citizenship Application Process", "Required Documents",
        "Application Fees and Payment", "Processing Times", "Frequently Asked Questions",
        "Contact Information", "Office Locations", "Holiday Schedule", "Public Notices",
        "Legal Requirements", "Residency Requirements", "Naturalization Process", "Rights and Responsibilities"
    };

    public CMSInitializer(
        IContentRepository contentRepository,
        ILogger<CMSInitializer> logger)
    {
        _contentRepository = contentRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting CMS database initialization...");

        // Check if data already exists
        var existingContents = await _contentRepository.BrowseAsync(cancellationToken);

        if (existingContents.Any())
        {
            _logger.LogInformation("Database already contains {Count} content items. Skipping seed.", 
                existingContents.Count);
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var contents = new List<Content>();

        for (int i = 0; i < Titles.Length; i++)
        {
            var contentId = new ContentId(SeedData.GenerateDeterministicGuid(i + 1, "content"));
            var title = Titles[i];
            var slug = title.ToLower().Replace(" ", "-").Replace("'", "");
            var contentType = ContentTypes[random.Next(ContentTypes.Length)];
            
            // Vary the status distribution
            var statusRoll = random.Next(100);
            var status = statusRoll switch
            {
                < 20 => ContentStatus.Draft,
                < 40 => ContentStatus.Review,
                < 90 => ContentStatus.Published,
                _ => ContentStatus.Archived
            };
            
            var content = new Content(
                contentId,
                TenantId,
                title,
                slug,
                contentType,
                status);
            
            // Set body and excerpt
            var body = $"<p>This is the content body for {title}. It contains important information about government services and procedures.</p>";
            var excerpt = $"Learn about {title.ToLower()} and related government services.";
            content.UpdateContent(body, excerpt);
            
            // Publish if status is Published
            if (status == ContentStatus.Published)
            {
                content.Publish();
            }

            contents.Add(content);
        }

        _logger.LogInformation("Created {Count} mock content items", contents.Count);

        // Add contents using repository
        foreach (var content in contents)
        {
            await _contentRepository.AddAsync(content, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {Count} content items", contents.Count);
    }
}
