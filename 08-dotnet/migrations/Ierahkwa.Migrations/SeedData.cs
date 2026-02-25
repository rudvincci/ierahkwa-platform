using Microsoft.Data.SqlClient;
using Dapper;

namespace Ierahkwa.Migrations;

public static class SeedData
{
    public static async Task SeedAllAsync(string connectionString)
    {
        using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        Console.WriteLine("\n=== SEEDING DATA ===\n");

        await SeedTenantsAsync(conn);
        await SeedNexusDomainsAsync(conn);
        await SeedLicensingPlansAsync(conn);
        await SeedLanguagesAsync(conn);

        Console.WriteLine("\nSeed complete.");
    }

    private static async Task SeedTenantsAsync(SqlConnection conn)
    {
        var tenants = new[]
        {
            ("navajo", "Navajo Nation", "US"), ("cherokee", "Cherokee Nation", "US"),
            ("lakota", "Lakota Sioux", "US"), ("ojibwe", "Ojibwe Nation", "US"),
            ("apache", "Apache Tribe", "US"), ("iroquois", "Haudenosaunee Confederacy", "US"),
            ("seminole", "Seminole Tribe", "US"), ("choctaw", "Choctaw Nation", "US"),
            ("creek", "Muscogee Creek", "US"), ("blackfeet", "Blackfeet Nation", "US"),
            ("crow", "Crow Nation", "US"), ("hopi", "Hopi Tribe", "US"),
            ("zuni", "Zuni Pueblo", "US"), ("comanche", "Comanche Nation", "US"),
            ("shoshone", "Shoshone-Bannock", "US"), ("ute", "Ute Mountain Tribe", "US"),
            ("pawnee", "Pawnee Nation", "US"), ("chickasaw", "Chickasaw Nation", "US"),
            ("potawatomi", "Potawatomi Nation", "US"), ("menominee", "Menominee Tribe", "US"),
            ("cree", "Cree Nation", "CA"), ("metis", "Metis Nation", "CA"),
            ("inuit", "Inuit Tapiriit Kanatami", "CA"), ("haida", "Haida Nation", "CA"),
            ("mohawk", "Mohawk Council", "CA"), ("algonquin", "Algonquin Nation", "CA"),
            ("maya", "Maya Nations", "MX"), ("nahua", "Nahua Peoples", "MX"),
            ("zapotec", "Zapotec Nation", "MX"), ("mixtec", "Mixtec Nation", "MX"),
            ("tarahumara", "Raramuri (Tarahumara)", "MX"), ("otomi", "Otomi Nation", "MX"),
            ("garifuna", "Garifuna Nation", "HN"), ("miskito", "Miskito Kingdom", "NI"),
            ("kuna", "Guna Yala", "PA"), ("bribri", "Bribri Nation", "CR"),
            ("quechua", "Quechua Nations", "PE"), ("aymara", "Aymara Nation", "BO"),
            ("mapuche", "Mapuche Nation", "CL"), ("guarani", "Guarani Nation", "PY"),
            ("yanomami", "Yanomami Territory", "BR"), ("kayapo", "Kayapo Nation", "BR"),
            ("wayuu", "Wayuu Nation", "CO"), ("embera", "Embera Nation", "CO"),
            ("aboriginal-au", "Aboriginal Australian Council", "AU"),
            ("maori", "Te Iwi Maori", "NZ"),
            ("hawaiian", "Native Hawaiian Council", "US"),
            ("samoan", "Samoan Chiefs Council", "WS"),
            ("ierahkwa-global", "Ierahkwa Global Platform", "GLOBAL"),
        };

        var existingCount = await conn.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM [Licensing].[Tenants] WHERE 1=1");

        if (existingCount > 0)
        {
            Console.WriteLine($"  [=] Tenants: {existingCount} already seeded");
            return;
        }

        foreach (var (id, name, country) in tenants)
        {
            await conn.ExecuteAsync(@"
                INSERT INTO [Licensing].[Tenants] (Name, Description, Status, TenantId, CreatedBy)
                VALUES (@Name, @Desc, 'Active', @TenantId, 'system-seed')",
                new { Name = name, Desc = $"Sovereign tenant — {country}", TenantId = id });
        }
        Console.WriteLine($"  [+] Tenants: {tenants.Length} seeded");
    }

    private static async Task SeedNexusDomainsAsync(SqlConnection conn)
    {
        var domains = new[]
        {
            ("NEXUS Orbital", "#00bcd4", "Science & Technology", 8),
            ("NEXUS Escudo", "#f44336", "Defense & Security", 5),
            ("NEXUS Cerebro", "#7c4dff", "Education & Research", 4),
            ("NEXUS Tesoro", "#ffd600", "Economy & Finance", 8),
            ("NEXUS Voces", "#e040fb", "Culture & Communication", 5),
            ("NEXUS Consejo", "#1565c0", "Governance & Law", 5),
            ("NEXUS Tierra", "#43a047", "Environment & Resources", 5),
            ("NEXUS Forja", "#00e676", "Technology & Innovation", 5),
            ("NEXUS Urbe", "#ff9100", "Infrastructure & Urban", 4),
            ("NEXUS Raices", "#d4a853", "Identity & Heritage", 4),
        };

        var existing = await conn.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM [NexusAggregation].[Dashboards] WHERE CreatedBy = 'system-seed'");
        if (existing > 0) { Console.WriteLine("  [=] NEXUS domains already seeded"); return; }

        foreach (var (name, color, desc, serviceCount) in domains)
        {
            await conn.ExecuteAsync(@"
                INSERT INTO [NexusAggregation].[Dashboards] (Name, Description, Status, TenantId, CreatedBy)
                VALUES (@Name, @Desc, @Color, 'ierahkwa-global', 'system-seed')",
                new { Name = name, Desc = $"{desc} — {serviceCount} microservices", Color = color });
        }
        Console.WriteLine($"  [+] NEXUS domains: {domains.Length} seeded");
    }

    private static async Task SeedLicensingPlansAsync(SqlConnection conn)
    {
        var plans = new[]
        {
            ("Member (Free)", "Free tier — basic access to all platforms", "member"),
            ("Resident ($2.99/mo)", "Enhanced features, priority support, no ads", "resident"),
            ("Citizen ($9.99/mo)", "Full access, governance participation, premium features", "citizen"),
            ("Government — Community ($500/mo)", "Up to 5,000 citizens, basic governance", "gov-community"),
            ("Government — Regional ($2,500/mo)", "Up to 50,000 citizens, full governance", "gov-regional"),
            ("Government — National ($15,000/mo)", "Unlimited citizens, sovereign infrastructure", "gov-national"),
            ("White-Label Starter ($25K/yr)", "10 platforms, basic customization", "wl-starter"),
            ("White-Label Professional ($100K/yr)", "50 platforms, full customization", "wl-professional"),
            ("White-Label Sovereign ($500K/yr)", "All platforms, sovereign branding", "wl-sovereign"),
            ("White-Label Enterprise ($1M+/yr)", "Dedicated infrastructure, custom development", "wl-enterprise"),
        };

        var existing = await conn.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM [Licensing].[Plans] WHERE CreatedBy = 'system-seed'");
        if (existing > 0) { Console.WriteLine("  [=] Licensing plans already seeded"); return; }

        foreach (var (name, desc, tier) in plans)
        {
            await conn.ExecuteAsync(@"
                INSERT INTO [Licensing].[Plans] (Name, Description, Status, TenantId, CreatedBy)
                VALUES (@Name, @Desc, @Tier, 'ierahkwa-global', 'system-seed')",
                new { Name = name, Desc = desc, Tier = tier });
        }
        Console.WriteLine($"  [+] Licensing plans: {plans.Length} seeded");
    }

    private static async Task SeedLanguagesAsync(SqlConnection conn)
    {
        var languages = new[]
        {
            ("Navajo", "Dine Bizaad", "nav", "US"), ("Cherokee", "Tsalagi Gawonihisdi", "chr", "US"),
            ("Lakota", "Lakhotiyapi", "lkt", "US"), ("Ojibwe", "Anishinaabemowin", "ojb", "US"),
            ("Cree", "Nehiyawewin", "cre", "CA"), ("Inuktitut", "Inuktitut", "iku", "CA"),
            ("Nahuatl", "Nahuatl", "nah", "MX"), ("Maya Yucateco", "Maaya Taan", "yua", "MX"),
            ("Zapoteco", "Diidxaza", "zap", "MX"), ("Quechua", "Runasimi", "que", "PE"),
            ("Aymara", "Aymar Aru", "aym", "BO"), ("Guarani", "Avane'e", "grn", "PY"),
            ("Mapudungun", "Mapuzugun", "arn", "CL"), ("Wayuunaiki", "Wayuunaiki", "guc", "CO"),
            ("Garifuna", "Garifuna", "cab", "HN"), ("Miskito", "Miskitu", "miq", "NI"),
            ("Te Reo Maori", "Te Reo Maori", "mri", "NZ"), ("Hawaiian", "Olelo Hawaii", "haw", "US"),
            ("English", "English", "eng", "GLOBAL"), ("Spanish", "Espanol", "spa", "GLOBAL"),
            ("Portuguese", "Portugues", "por", "BR"), ("French", "Francais", "fra", "CA"),
        };

        var existing = await conn.QuerySingleAsync<int>(
            "SELECT COUNT(*) FROM [Language].[Languages] WHERE CreatedBy = 'system-seed'");
        if (existing > 0) { Console.WriteLine("  [=] Languages already seeded"); return; }

        foreach (var (name, nativeName, code, country) in languages)
        {
            await conn.ExecuteAsync(@"
                INSERT INTO [Language].[Languages] (Name, Description, Status, TenantId, CreatedBy)
                VALUES (@Name, @Desc, @Code, 'ierahkwa-global', 'system-seed')",
                new { Name = name, Desc = $"{nativeName} ({code})", Code = code });
        }
        Console.WriteLine($"  [+] Languages: {languages.Length} seeded");
    }
}
