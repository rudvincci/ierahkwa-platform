using System.Linq;
using Mamey.Government.Modules.Identity.Core.Domain.Entities;
using Mamey.Government.Modules.Identity.Core.Domain.Repositories;
using Mamey.Government.Shared.Abstractions;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Identity.Core.EF;

internal class IdentityInitializer : IInitializer
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<IdentityInitializer> _logger;

    private static readonly string[] FirstNames = {
        "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda",
        "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica"
    };
    
    private static readonly string[] LastNames = {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Wilson", "Anderson", "Thomas", "Taylor"
    };
    
    private static readonly string[] EmailDomains = {
        "gmail.com", "yahoo.com", "outlook.com", "hotmail.com", "aol.com"
    };

    public IdentityInitializer(
        IUserProfileRepository userProfileRepository,
        ILogger<IdentityInitializer> logger)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting identity database initialization...");

        // Check if data already exists
        var existingProfiles = await _userProfileRepository.BrowseAsync(cancellationToken);

        if (existingProfiles.Any())
        {
            _logger.LogInformation("Database already contains {Count} user profiles. Skipping seed.", 
                existingProfiles.Count);
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var profiles = new List<UserProfile>();

        for (int i = 1; i <= 50; i++)
        {
            // Use deterministic ID so Notifications can reference these users
            var userId = new UserId(SeedData.GetUserId(i));
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var emailDomain = EmailDomains[random.Next(EmailDomains.Length)];
            var email = $"{firstName.ToLower()}.{lastName.ToLower()}{i}@{emailDomain}";
            var displayName = $"{firstName} {lastName}";
            var authenticatorIssuer = "authentik";
            var authenticatorSubject = SeedData.GetUserId(i).ToString(); // Use deterministic subject
            
            var profile = new UserProfile(
                userId,
                authenticatorIssuer,
                authenticatorSubject,
                email,
                displayName,
                SeedData.TenantId);
            
            profiles.Add(profile);
        }

        _logger.LogInformation("Created {Count} mock user profiles", profiles.Count);

        // Add profiles using repository
        foreach (var profile in profiles)
        {
            await _userProfileRepository.AddAsync(profile, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {Count} user profiles", profiles.Count);
    }
}
