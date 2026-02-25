using Microsoft.Data.SqlClient;
using Dapper;

var connectionString = args.Length > 0 ? args[0]
    : "Server=localhost;Database=IerahkwaDb;User=sa;Password=Ierahkwa2026!;TrustServerCertificate=true";

Console.WriteLine("=== IERAHKWA SOVEREIGN PLATFORM — DATABASE MIGRATION ===");
Console.WriteLine($"Server: {new SqlConnectionStringBuilder(connectionString).DataSource}");

using var conn = new SqlConnection(connectionString);
await conn.OpenAsync();

// Create migration tracking table
await conn.ExecuteAsync(@"
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__IerahkwaMigrations')
    CREATE TABLE __IerahkwaMigrations (
        Id INT IDENTITY PRIMARY KEY,
        MigrationName NVARCHAR(500) NOT NULL,
        ServiceName NVARCHAR(200) NOT NULL,
        AppliedAt DATETIME2 DEFAULT GETUTCDATE(),
        UNIQUE(MigrationName)
    )");

// All 83 microservice schemas
var schemas = new (string Service, string[] Tables)[]
{
    // NEXUS Orbital
    ("SpaceService", new[] { "Missions", "Satellites", "LaunchPads", "Orbits", "SpaceDebris" }),
    ("TelecomService", new[] { "Towers", "Frequencies", "Subscriptions", "Coverages", "SIMs" }),
    ("GenomicsService", new[] { "Genomes", "Sequences", "Analyses", "BioBanks", "Studies" }),
    ("IoTRoboticsService", new[] { "Devices", "Robots", "Sensors", "Automations", "Firmware" }),
    ("QuantumService", new[] { "Circuits", "Qubits", "Experiments", "Algorithms", "Results" }),
    ("AIEngineService", new[] { "Models", "Trainings", "Datasets", "Predictions", "Pipelines" }),
    ("NetworkService", new[] { "Nodes", "Links", "Protocols", "Bandwidth", "Routes" }),
    ("DevToolsService", new[] { "Repos", "Builds", "Artifacts", "Environments" }),
    // NEXUS Escudo
    ("MilitaryService", new[] { "Units", "Personnel", "Operations", "Equipment", "Bases" }),
    ("DroneService", new[] { "Drones", "Flights", "Missions", "Payloads", "Zones" }),
    ("CyberSecService", new[] { "Threats", "Incidents", "Firewalls", "Audits", "Vulnerabilities" }),
    ("IntelligenceService", new[] { "Reports", "Sources", "Analyses", "Briefs", "Assets" }),
    ("EmergencyService", new[] { "Alerts", "Responses", "Units", "Shelters", "Incidents" }),
    // NEXUS Cerebro
    ("EducationService", new[] { "Courses", "Students", "Teachers", "Enrollments", "Grades", "Certifications" }),
    ("ResearchService", new[] { "Projects", "Papers", "Grants", "Labs", "Citations" }),
    ("LanguageService", new[] { "Languages", "Translations", "Dictionaries", "Lessons", "Dialects" }),
    ("SearchService", new[] { "Indexes", "Queries", "Results", "Suggestions", "Crawls" }),
    // NEXUS Tesoro
    ("CommerceService", new[] { "Products", "Orders", "Payments", "Merchants", "Reviews", "Coupons" }),
    ("BlockchainService", new[] { "Blocks", "Transactions", "Wallets", "SmartContracts", "Tokens" }),
    ("BankingService", new[] { "Accounts", "Transfers", "Loans", "Cards", "Statements" }),
    ("InsuranceService", new[] { "Policies", "Claims", "Premiums", "Beneficiaries", "Assessments" }),
    ("EmploymentService", new[] { "Jobs", "Applicants", "Employers", "Interviews", "Contracts" }),
    ("SmartFactoryService", new[] { "Factories", "ProductionLines", "Inventories", "QualityChecks", "Workers" }),
    ("ArtisanService", new[] { "Artisans", "Crafts", "Workshops", "Certifications", "Orders" }),
    ("TourismService", new[] { "Destinations", "Bookings", "Tours", "Guides", "Packages" }),
    // NEXUS Voces
    ("MediaContentService", new[] { "Articles", "Videos", "Podcasts", "Channels", "Playlists" }),
    ("MessagingService", new[] { "Conversations", "Messages", "Channels", "Contacts", "Attachments" }),
    ("CultureArchiveService", new[] { "Artifacts", "Collections", "Exhibits", "Traditions", "Archives" }),
    ("SportsService", new[] { "Teams", "Athletes", "Events", "Scores", "Leagues" }),
    ("SocialService", new[] { "Posts", "Comments", "Follows", "Groups", "Reactions" }),
    // NEXUS Consejo
    ("GovernanceService", new[] { "Councils", "Resolutions", "Elections", "Representatives", "Policies" }),
    ("JusticeService", new[] { "Cases", "Courts", "Judges", "Verdicts", "Laws" }),
    ("DiplomacyService", new[] { "Treaties", "Embassies", "Diplomats", "Agreements", "Summits" }),
    ("CitizenService", new[] { "Citizens", "Registrations", "Documents", "BirthRecords", "Addresses" }),
    ("SocialWelfareService", new[] { "Programs", "Beneficiaries", "Distributions", "Assessments", "Budgets" }),
    // NEXUS Tierra
    ("AgricultureService", new[] { "Farms", "Crops", "Harvests", "Markets", "Cooperatives" }),
    ("NaturalResourceService", new[] { "Resources", "Deposits", "Concessions", "Surveys", "Reserves" }),
    ("EnvironmentService", new[] { "Ecosystems", "Monitors", "Reports", "Species", "Protections" }),
    ("WasteService", new[] { "Collections", "Facilities", "Recyclables", "Pickups" }),
    ("EnergyService", new[] { "Plants", "Grids", "Meters", "Tariffs", "Outages" }),
    // NEXUS Forja
    ("DevOpsService", new[] { "Pipelines", "Deployments", "Monitors", "Alerts" }),
    ("LowCodeDesignService", new[] { "Templates", "Components", "Pages", "Themes" }),
    ("BrowserService", new[] { "Bookmarks", "Histories", "Tabs", "Extensions", "Profiles" }),
    ("ProductivityService", new[] { "Tasks", "Notes", "Calendars", "Reminders", "Projects" }),
    ("CloudService", new[] { "Instances", "Storage", "Functions", "Databases", "Domains" }),
    // NEXUS Urbe
    ("UrbanService", new[] { "Zones", "Buildings", "Permits", "Infrastructure" }),
    ("TransportService", new[] { "Vehicles", "Routes", "Stations", "Schedules", "Tickets" }),
    ("PostalMapsService", new[] { "Addresses", "Packages", "Zones", "Deliveries", "Maps" }),
    ("HousingService", new[] { "Properties", "Tenants", "Leases", "Maintenance", "Inspections" }),
    // NEXUS Raíces
    ("IdentityService", new[] { "FWIDs", "Verifications", "BiometricRecords", "Credentials" }),
    ("HealthService", new[] { "Patients", "Records", "Appointments", "Prescriptions", "Providers" }),
    ("NexusAggregationService", new[] { "Dashboards", "Widgets", "Feeds", "Aggregations", "Metrics" }),
    ("LicensingService", new[] { "Licenses", "Tenants", "Plans", "Invoices", "Features" }),
};

// Standard columns for every table
var standardColumns = @"
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(500) NOT NULL DEFAULT '',
    Description NVARCHAR(MAX) NOT NULL DEFAULT '',
    Status NVARCHAR(100) NOT NULL DEFAULT 'Active',
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CreatedBy NVARCHAR(200) NOT NULL DEFAULT '',
    TenantId NVARCHAR(200) NOT NULL DEFAULT ''";

var created = 0;
var skipped = 0;

foreach (var (service, tables) in schemas)
{
    var schemaName = service.Replace("Service", "");

    // Create schema
    var schemaMigration = $"Create_Schema_{schemaName}";
    var exists = await conn.QuerySingleOrDefaultAsync<int>(
        "SELECT COUNT(*) FROM __IerahkwaMigrations WHERE MigrationName = @Name",
        new { Name = schemaMigration });

    if (exists == 0)
    {
        await conn.ExecuteAsync($@"
            IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = '{schemaName}')
            EXEC('CREATE SCHEMA [{schemaName}]')");
        await conn.ExecuteAsync(
            "INSERT INTO __IerahkwaMigrations (MigrationName, ServiceName) VALUES (@Name, @Svc)",
            new { Name = schemaMigration, Svc = service });
    }

    foreach (var table in tables)
    {
        var migrationName = $"Create_{schemaName}_{table}";
        exists = await conn.QuerySingleOrDefaultAsync<int>(
            "SELECT COUNT(*) FROM __IerahkwaMigrations WHERE MigrationName = @Name",
            new { Name = migrationName });

        if (exists == 0)
        {
            await conn.ExecuteAsync($@"
                IF NOT EXISTS (SELECT * FROM sys.tables t JOIN sys.schemas s ON t.schema_id = s.schema_id
                               WHERE s.name = '{schemaName}' AND t.name = '{table}')
                CREATE TABLE [{schemaName}].[{table}] ({standardColumns})");

            // Add TenantId index
            await conn.ExecuteAsync($@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_{schemaName}_{table}_TenantId')
                CREATE INDEX IX_{schemaName}_{table}_TenantId ON [{schemaName}].[{table}] (TenantId)");

            await conn.ExecuteAsync(
                "INSERT INTO __IerahkwaMigrations (MigrationName, ServiceName) VALUES (@Name, @Svc)",
                new { Name = migrationName, Svc = service });
            created++;
        }
        else skipped++;
    }
    Console.WriteLine($"  [{(created > 0 ? "+" : "=")}] {service}: {tables.Length} tables");
}

Console.WriteLine($"\nMigration complete: {created} tables created, {skipped} already existed.");
Console.WriteLine($"Total schemas: {schemas.Length}");
Console.WriteLine($"Total tables: {schemas.Sum(s => s.Tables.Length)}");
