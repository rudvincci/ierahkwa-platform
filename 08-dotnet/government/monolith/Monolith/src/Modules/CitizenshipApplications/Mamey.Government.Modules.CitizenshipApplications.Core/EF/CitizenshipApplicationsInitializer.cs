using System.Linq;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Entities;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.Repositories;
using Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects;
using Mamey.Types;
using Microsoft.Extensions.Logging;
using AppId = Mamey.Government.Modules.CitizenshipApplications.Core.Domain.ValueObjects.ApplicationId;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.EF;

internal class CitizenshipApplicationsInitializer : IInitializer
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly ILogger<CitizenshipApplicationsInitializer> _logger;
    
    private static readonly Guid TenantIdValue = Guid.Parse("6d1c92c5-eb7c-4bf1-ac35-eb4e3db9c115");
    private static readonly TenantId TenantId = new(TenantIdValue);
    
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
    
    private static readonly string[] DocumentTypes = {
        "Passport", "BirthCertificate", "DriverLicense", "IDCard", "ProofOfResidence",
        "MarriageCertificate", "DivorceCertificate", "EducationCertificate", "EmploymentLetter"
    };

    public CitizenshipApplicationsInitializer(
        IApplicationRepository applicationRepository,
        ILogger<CitizenshipApplicationsInitializer> logger)
    {
        _applicationRepository = applicationRepository;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting citizenship applications database initialization...");

        // Check if data already exists
        var existingApplications = Enumerable.Empty<CitizenshipApplication>();
        try
        {
            existingApplications = await _applicationRepository.GetByTenantAsync(TenantId, cancellationToken);
        }
        catch (Exception ex) when (ex.Message.Contains("does not exist") || 
                                   ex.Message.Contains("relation") || 
                                   ex.Message.Contains("Invalid object name") ||
                                   ex.Message.Contains("Table") && ex.Message.Contains("doesn't exist") ||
                                   ex.Message.Contains("Unknown table"))
        {
            // Table doesn't exist yet (migrations may still be running) - treat as no data
            _logger.LogDebug("Table does not exist yet for CitizenshipApplications. Treating as empty database.");
            existingApplications = Enumerable.Empty<CitizenshipApplication>();
        }

        if (existingApplications.Any())
        {
            _logger.LogInformation("Database already contains {Count} applications for tenant {TenantId}. Skipping seed.", 
                existingApplications.Count(), TenantIdValue);
            return;
        }

        var random = new Random(42); // Fixed seed for reproducible data
        var applications = new List<CitizenshipApplication>();

        for (int i = 1; i <= 100; i++)
        {
            var applicationId = new AppId(Guid.NewGuid());
            var firstName = FirstNames[random.Next(FirstNames.Length)];
            var lastName = LastNames[random.Next(LastNames.Length)];
            var emailDomain = EmailDomains[random.Next(EmailDomains.Length)];
            var email = new Email($"{firstName.ToLower()}.{lastName.ToLower()}{i}@{emailDomain}");
            var dateOfBirth = new DateTime(1950 + random.Next(50), random.Next(1, 13), random.Next(1, 29));
            
            // Generate application number: INKG-CITAPP-YYYYMMDD-XXXXXX
            var date = DateTime.UtcNow.AddDays(-random.Next(365)).ToString("yyyyMMdd");
            var randomSuffix = GenerateRandomSuffix(random, 6);
            var applicationNumber = new ApplicationNumber($"INKG-CITAPP-{date}-{randomSuffix}");
            
            // Vary the status distribution
            var statusRoll = random.Next(100);
            var status = statusRoll switch
            {
                < 20 => ApplicationStatus.Draft,
                < 40 => ApplicationStatus.Submitted,
                < 55 => ApplicationStatus.Validating,
                < 70 => ApplicationStatus.KycPending,
                < 85 => ApplicationStatus.ReviewPending,
                < 95 => ApplicationStatus.Approved,
                _ => ApplicationStatus.Rejected
            };
            
            // Vary the step based on status
            var step = status switch
            {
                ApplicationStatus.Draft => CitizenshipApplicationStep.Initial,
                ApplicationStatus.Submitted => CitizenshipApplicationStep.PersonalDetailsComplete,
                ApplicationStatus.Validating => CitizenshipApplicationStep.ContactInformationComplete,
                ApplicationStatus.KycPending => CitizenshipApplicationStep.PassportAndIdentificationComplete,
                ApplicationStatus.ReviewPending => CitizenshipApplicationStep.ResidencyAndImmigrationComplete,
                ApplicationStatus.Approved => CitizenshipApplicationStep.EmploymentAndEducationComplete,
                _ => CitizenshipApplicationStep.Initial
            };

            var applicantName = new Name(firstName, lastName);
            
            // Create application
            var application = new CitizenshipApplication(
                applicationId,
                TenantId,
                applicationNumber,
                applicantName,
                dateOfBirth,
                email,
                status,
                0);

            // Set additional properties via reflection for properties that don't have public setters
            SetProperty(application, nameof(CitizenshipApplication.Step), step);
            SetProperty(application, nameof(CitizenshipApplication.CreatedAt), DateTime.UtcNow.AddDays(-random.Next(365)));
            SetProperty(application, nameof(CitizenshipApplication.UpdatedAt), DateTime.UtcNow.AddDays(-random.Next(30)));
            
            // Add some applications with phone numbers
            if (random.Next(100) < 70)
            {
                var phone = new Phone("1", $"{random.Next(200, 999)}{random.Next(100, 999)}{random.Next(1000, 9999)}");
                SetProperty(application, nameof(CitizenshipApplication.Phone), phone);
            }
            
            // Add some applications with addresses
            if (random.Next(100) < 60)
            {
                var address = new Address(
                    line: $"{random.Next(100, 9999)} Main St",
                    city: GetRandomCity(random),
                    state: GetRandomState(random),
                    zip5: $"{random.Next(10000, 99999)}",
                    country: "US",
                    type: Address.AddressType.Home);
                SetProperty(application, nameof(CitizenshipApplication.Address), address);
            }
            
            // Set rejection reason for rejected applications
            if (status == ApplicationStatus.Rejected)
            {
                var reasons = new[] { "Incomplete documentation", "Failed background check", "Insufficient residency period", "Missing required information" };
                SetProperty(application, nameof(CitizenshipApplication.RejectionReason), reasons[random.Next(reasons.Length)]);
            }
            
            // Set submitted/approved/rejected dates based on status
            if (status >= ApplicationStatus.Submitted)
            {
                SetProperty(application, nameof(CitizenshipApplication.SubmittedAt), DateTime.UtcNow.AddDays(-random.Next(200, 365)));
            }
            
            if (status == ApplicationStatus.Approved)
            {
                SetProperty(application, nameof(CitizenshipApplication.ApprovedAt), DateTime.UtcNow.AddDays(-random.Next(30, 200)));
            }
            
            if (status == ApplicationStatus.Rejected)
            {
                SetProperty(application, nameof(CitizenshipApplication.RejectedAt), DateTime.UtcNow.AddDays(-random.Next(30, 200)));
            }

            // Add 0-3 uploaded documents per application using AddUpload method
            var documentCount = random.Next(4);
            for (int j = 0; j < documentCount; j++)
            {
                var documentId = Guid.NewGuid();
                var documentType = DocumentTypes[random.Next(DocumentTypes.Length)];
                var fileName = $"{documentType}_{applicationId.Value:N}_{j}.pdf";
                var storagePath = $"documents/{TenantIdValue:N}/{applicationId.Value:N}/{fileName}";
                var fileSize = random.Next(100000, 5000000); // 100KB to 5MB
                
                var document = new UploadedDocument(
                    documentId,
                    fileName,
                    storagePath,
                    documentType,
                    fileSize);
                
                // Set ApplicationId and add to application
                document.ApplicationId = applicationId;
                application.AddUpload(document);
            }

            applications.Add(application);
        }

        _logger.LogInformation("Created {Count} mock applications", applications.Count);

        // Add applications using repository (which will handle documents via cascade)
        var totalDocuments = applications.Sum(a => a.Uploads.Count);
        foreach (var application in applications)
        {
            await _applicationRepository.AddAsync(application, cancellationToken);
        }
        
        _logger.LogInformation("Successfully seeded {ApplicationCount} applications and {DocumentCount} documents", 
            applications.Count, totalDocuments);
    }

    private static string GenerateRandomSuffix(Random random, int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, length)
            .Select(_ => chars[random.Next(chars.Length)])
            .ToArray());
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

    private static void SetProperty<T>(object obj, string propertyName, T value)
    {
        var property = typeof(CitizenshipApplication).GetProperty(propertyName, 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (property != null && property.CanWrite)
        {
            property.SetValue(obj, value);
        }
        else
        {
            // Try private setter via reflection
            var field = typeof(CitizenshipApplication).GetField($"<{propertyName}>k__BackingField", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(obj, value);
            }
        }
    }
}
