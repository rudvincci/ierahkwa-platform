using System.Linq;
using Mamey.Government.Modules.Citizens.Core.Domain.Entities;
using Mamey.Government.Modules.Citizens.Core.Domain.Repositories;
using Mamey.Government.Modules.Citizens.Core.Domain.ValueObjects;
using Mamey.Government.Shared.Abstractions;
using Mamey.Types;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Citizens.Core.EF;

internal class CitizensInitializer : IInitializer
{
    private readonly ICitizenRepository _citizenRepository;
    private readonly ILogger<CitizensInitializer> _logger;
    
    private static readonly TenantId TenantId = new(SeedData.TenantId);
    
    private static readonly string[] FirstNames = {
        "James", "Mary", "John", "Patricia", "Robert", "Jennifer", "Michael", "Linda",
        "William", "Elizabeth", "David", "Barbara", "Richard", "Susan", "Joseph", "Jessica",
        "Thomas", "Sarah", "Charles", "Karen", "Christopher", "Nancy", "Daniel", "Lisa",
        "Matthew", "Betty", "Anthony", "Margaret", "Mark", "Sandra", "Donald", "Ashley",
        "Steven", "Kimberly", "Paul", "Emily", "Andrew", "Donna", "Joshua", "Michelle",
        "Kenneth", "Carol", "Kevin", "Amanda", "Brian", "Dorothy", "George", "Melissa",
        "Edward", "Deborah", "Ronald", "Stephanie", "Timothy", "Rebecca", "Jason", "Sharon",
        "Jeffrey", "Laura", "Ryan", "Cynthia", "Jacob", "Kathleen", "Gary", "Amy",
        "Nicholas", "Angela", "Eric", "Shirley", "Jonathan", "Brenda", "Stephen", "Emma",
        "Larry", "Olivia", "Justin", "Catherine", "Scott", "Christine", "Brandon", "Samantha",
        "Benjamin", "Debra", "Samuel", "Rachel", "Frank", "Carolyn", "Gregory", "Janet",
        "Raymond", "Virginia", "Alexander", "Maria", "Patrick", "Heather", "Jack", "Diane"
    };
    
    private static readonly string[] LastNames = {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis",
        "Rodriguez", "Martinez", "Hernandez", "Lopez", "Wilson", "Anderson", "Thomas", "Taylor",
        "Moore", "Jackson", "Martin", "Lee", "Thompson", "White", "Harris", "Sanchez",
        "Clark", "Ramirez", "Lewis", "Robinson", "Walker", "Young", "Allen", "King",
        "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores", "Green", "Adams",
        "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
        "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards",
        "Collins", "Reyes", "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers",
        "Gutierrez", "Ortiz", "Morgan", "Cooper", "Peterson", "Bailey", "Reed", "Kelly",
        "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson", "Watson", "Brooks",
        "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
        "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers", "Long", "Ross"
    };
    
    private static readonly string[] EmailDomains = {
        "gmail.com", "yahoo.com", "outlook.com", "hotmail.com", "aol.com",
        "icloud.com", "mail.com", "protonmail.com", "live.com", "msn.com"
    };

    public CitizensInitializer(
        ICitizenRepository citizenRepository,
        ILogger<CitizensInitializer> logger)
    {
        _citizenRepository = citizenRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting citizens database initialization...");

        // Check if data already exists
        var existingCitizens = await _citizenRepository.GetByTenantAsync(TenantId, cancellationToken);

        if (existingCitizens.Any())
        {
            _logger.LogInformation("Database already contains {Count} citizens for tenant {TenantId}. Skipping seed.", 
                existingCitizens.Count, SeedData.TenantId);
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var citizens = new List<Citizen>();

        for (int i = 1; i <= 100; i++)
        {
            // Use deterministic ID so other modules can reference this citizen
            var citizenId = new CitizenId(SeedData.GetCitizenId(i));
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var emailDomain = EmailDomains[random.Next(EmailDomains.Length)];
            var email = new Email($"{firstName.ToLower()}.{lastName.ToLower()}{i}@{emailDomain}");
            var dateOfBirth = new DateTime(1950 + random.Next(50), random.Next(1, 13), random.Next(1, 29));
            var citizenName = new Name(firstName, lastName);
            
            // Vary the status distribution
            var statusRoll = random.Next(100);
            var status = statusRoll switch
            {
                < 30 => CitizenshipStatus.Probationary,
                < 60 => CitizenshipStatus.Resident,
                < 90 => CitizenshipStatus.Citizen,
                < 95 => CitizenshipStatus.Suspended,
                _ => CitizenshipStatus.Inactive
            };
            
            // Create citizen with deterministic application ID
            var citizen = new Citizen(
                citizenId,
                TenantId,
                citizenName,
                email,
                null, // phone - will add some below
                null, // address - will add some below
                dateOfBirth,
                status,
                SeedData.GetApplicationId(i)); // applicationId - deterministic
            
            // Add phone numbers to 70% of citizens
            if (random.Next(100) < 70)
            {
                var phone = new Phone("1", $"{random.Next(200, 999)}{random.Next(100, 999)}{random.Next(1000, 9999)}");
                citizen.UpdateContact(email, phone, null);
            }
            
            // Add addresses to 60% of citizens
            if (random.Next(100) < 60)
            {
                var address = new Address(
                    line: $"{random.Next(100, 9999)} Main St",
                    city: GetRandomCity(random),
                    state: GetRandomState(random),
                    zip5: $"{random.Next(10000, 99999)}",
                    country: "US",
                    type: Address.AddressType.Home);
                citizen.UpdateContact(citizen.Email, citizen.Phone, address);
            }
            
            // Set photo path for some citizens
            if (random.Next(100) < 50)
            {
                citizen.UpdatePhoto($"photos/{SeedData.TenantId:N}/{citizenId.Value:N}/photo.jpg");
            }

            citizens.Add(citizen);
        }

        _logger.LogInformation("Created {Count} mock citizens", citizens.Count);

        // Add citizens using repository
        foreach (var citizen in citizens)
        {
            await _citizenRepository.AddAsync(citizen, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {Count} citizens", citizens.Count);
    }

    private static string GetRandomCity(Random random)
    {
        var cities = new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" };
        return cities[random.Next(cities.Length)];
    }

    private static string GetRandomState(Random random)
    {
        var states = new[] { "NY", "CA", "IL", "TX", "AZ", "PA", "FL", "OH", "GA", "NC" };
        return states[random.Next(states.Length)];
    }
}
