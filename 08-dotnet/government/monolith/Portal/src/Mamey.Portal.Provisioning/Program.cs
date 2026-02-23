using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;
using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Tenant.Application.Models;
using Mamey.Portal.Tenant.Application.Services;
using Mamey.Portal.Tenant.Infrastructure.Persistence;
using Mamey.Portal.Tenant.Infrastructure.Services;

static int Usage()
{
    Console.WriteLine("""
        Mamey.Portal.Provisioning

        Idempotent provisioning for the Portal SaaS: seed tenants/settings/naming/templates.

        Usage:
          dotnet run --project src/Mamey.Portal.Provisioning -- --seed-file provisioning/seed-tenants.json --apply

        Options:
          --seed-file <path>              Seed JSON file (default: provisioning/seed-tenants.json)
          --apply                         Actually write changes (default is dry-run)
          --seed-sample-applications      Create 1 sample citizenship application per tenant if none exist
          --update-templates              Update existing templates if different (default: insert-only)
          --portaldb <connectionString>   Override PortalDb connection string
          --issuer <issuer>               Override OIDC issuer for user invites (default: Oidc Authority from web config)

        Connection string resolution order:
          1) --portaldb
          2) Environment variable ConnectionStrings__PortalDb
          3) src/Mamey.Portal.Web/appsettings*.json (including *.Local.json if present)
        """);
    return 2;
}

static bool HasArg(string[] args, string name)
    => args.Any(a => string.Equals(a, name, StringComparison.OrdinalIgnoreCase));

static string? GetArgValue(string[] args, string name)
{
    for (var i = 0; i < args.Length - 1; i++)
    {
        if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
        {
            return args[i + 1];
        }
    }
    return null;
}

static string NormalizeTenantId(string? id)
{
    id = (id ?? string.Empty).Trim().ToLowerInvariant();
    id = id.Replace(' ', '-');

    // match TenantOnboardingService behavior closely: keep [a-z0-9-], trim '-', cap at 128
    var cleaned = new string(id.Where(c => (c is >= 'a' and <= 'z') || (c is >= '0' and <= '9') || c == '-').ToArray());
    cleaned = cleaned.Trim('-');
    return cleaned.Length > 128 ? cleaned[..128] : cleaned;
}

static string NewApplicationNumber(string tenantId, DateTimeOffset now)
{
    var suffix = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant();
    return $"APP-{tenantId.ToUpperInvariant()}-{now:yyyyMMdd}-{suffix}";
}

var seedFilePath = GetArgValue(args, "--seed-file") ?? "provisioning/seed-tenants.json";
var apply = HasArg(args, "--apply");
var seedSampleApps = HasArg(args, "--seed-sample-applications");
var updateTemplates = HasArg(args, "--update-templates");
var portalDbOverride = GetArgValue(args, "--portaldb");
var issuerOverride = GetArgValue(args, "--issuer");

if (HasArg(args, "--help") || HasArg(args, "-h") || HasArg(args, "/?"))
{
    return Usage();
}

var cwd = Directory.GetCurrentDirectory();
var config = new ConfigurationBuilder()
    .SetBasePath(cwd)
    .AddJsonFile("src/Mamey.Portal.Web/appsettings.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"src/Mamey.Portal.Web/appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.json", optional: true, reloadOnChange: false)
    .AddJsonFile("src/Mamey.Portal.Web/appsettings.Local.json", optional: true, reloadOnChange: false)
    .AddJsonFile($"src/Mamey.Portal.Web/appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development"}.Local.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables()
    .Build();

var portalDb = portalDbOverride
              ?? config.GetConnectionString("PortalDb")
              ?? config["ConnectionStrings:PortalDb"];

var issuerFromConfig =
    config["Oidc:Authority"]
    ?? config["PortalAuthOptions:Oidc:Authority"]
    ?? config["PortalAuth:Oidc:Authority"]
    ?? config["Auth:Oidc:Authority"];
var issuer = OidcIssuerNormalizer.Normalize(issuerOverride ?? issuerFromConfig);

if (string.IsNullOrWhiteSpace(portalDb))
{
    Console.Error.WriteLine("ERROR: PortalDb connection string not found. Provide --portaldb or set ConnectionStrings__PortalDb.");
    return 1;
}

seedFilePath = Path.GetFullPath(Path.Combine(cwd, seedFilePath));
if (!File.Exists(seedFilePath))
{
    Console.Error.WriteLine($"ERROR: Seed file not found: {seedFilePath}");
    return 1;
}

var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    ReadCommentHandling = JsonCommentHandling.Skip,
    AllowTrailingCommas = true
};

SeedFile seed;
try
{
    seed = JsonSerializer.Deserialize<SeedFile>(await File.ReadAllTextAsync(seedFilePath), jsonOptions)
           ?? new SeedFile(Array.Empty<SeedTenant>());
}
catch (Exception ex)
{
    Console.Error.WriteLine($"ERROR: Failed to parse seed file: {ex.Message}");
    return 1;
}

if (seed.Tenants.Count == 0)
{
    Console.WriteLine("No tenants in seed file. Nothing to do.");
    return 0;
}

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(config);
services.AddDbContext<TenantDbContext>(o => o.UseNpgsql(portalDb));
services.AddDbContext<CitizenshipDbContext>(o => o.UseNpgsql(portalDb));
services.AddScoped<ITenantOnboardingService, TenantOnboardingService>();

await using var sp = services.BuildServiceProvider();
await using var scope = sp.CreateAsyncScope();
var onboarding = scope.ServiceProvider.GetRequiredService<ITenantOnboardingService>();
var tenantDb = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
var citizenshipDb = scope.ServiceProvider.GetRequiredService<CitizenshipDbContext>();

try
{
    await tenantDb.Database.MigrateAsync();
    await citizenshipDb.Database.MigrateAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine($"ERROR: Could not connect / migrate schema: {ex.Message}");
    return 1;
}

Console.WriteLine($"Seed file: {seedFilePath}");
Console.WriteLine($"PortalDb: {(portalDbOverride is null ? "(from config/env)" : "(from --portaldb)")}");
Console.WriteLine($"Mode: {(apply ? "APPLY" : "DRY-RUN")}");
Console.WriteLine($"Seed sample applications: {seedSampleApps}");
Console.WriteLine($"Update existing templates: {updateTemplates}");
Console.WriteLine($"Invite issuer: {(string.IsNullOrWhiteSpace(issuer) ? "(empty - invites disabled unless you pass --issuer)" : issuer)}");
Console.WriteLine();

var now = DateTimeOffset.UtcNow;
var created = 0;
var updated = 0;
var templateUpserts = 0;
var sampleApps = 0;
var inviteUpserts = 0;

foreach (var t in seed.Tenants)
{
    var tenantId = NormalizeTenantId(t.TenantId);
    if (string.IsNullOrWhiteSpace(tenantId))
    {
        Console.WriteLine($"- SKIP: invalid tenant id '{t.TenantId}'");
        continue;
    }

    var displayName = (t.DisplayName ?? string.Empty).Trim();
    if (string.IsNullOrWhiteSpace(displayName))
    {
        Console.WriteLine($"- SKIP: tenant '{tenantId}' has empty displayName");
        continue;
    }

    var desiredBranding = new TenantBranding(
        DisplayName: displayName,
        SealLine1: t.Branding?.SealLine1,
        SealLine2: t.Branding?.SealLine2,
        ContactEmail: t.Branding?.ContactEmail);

    var desiredNaming = t.DocumentNaming ?? DocumentNamingPattern.Default;

    var current = await onboarding.GetSettingsAsync(tenantId);
    if (current is null)
    {
        Console.WriteLine($"- CREATE tenant '{tenantId}' ({displayName})");
        if (apply)
        {
            await onboarding.CreateTenantAsync(
                tenantId,
                displayName,
                branding: desiredBranding,
                namingPattern: desiredNaming);
            created++;
        }
    }
    else
    {
        var needsUpdate =
            !string.Equals(current.Branding.DisplayName, desiredBranding.DisplayName, StringComparison.Ordinal)
            || !string.Equals(current.Branding.SealLine1, desiredBranding.SealLine1, StringComparison.Ordinal)
            || !string.Equals(current.Branding.SealLine2, desiredBranding.SealLine2, StringComparison.Ordinal)
            || !string.Equals(current.Branding.ContactEmail, desiredBranding.ContactEmail, StringComparison.Ordinal);

        Console.WriteLine(needsUpdate
            ? $"- UPDATE tenant '{tenantId}' ({displayName})"
            : $"- OK tenant '{tenantId}' ({displayName})");

        if (apply && needsUpdate)
        {
            await onboarding.UpdateSettingsAsync(tenantId, desiredBranding, desiredNaming);
            updated++;
        }
    }

    // Templates can be provided inline (Templates) or by file path (TemplateFiles).
    // Inline wins if the same kind appears in both.
    var templateInputs = new Dictionary<string, string>(StringComparer.Ordinal);
    if (t.TemplateFiles is { Count: > 0 })
    {
        foreach (var kv in t.TemplateFiles)
        {
            var kind = (kv.Key ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(kind)) continue;

            var relPath = (kv.Value ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(relPath)) continue;

            var seedDir = Path.GetDirectoryName(seedFilePath) ?? cwd;
            var filePath = Path.GetFullPath(Path.Combine(seedDir, relPath));

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"  - TEMPLATE file missing for '{kind}': {filePath}");
                continue;
            }

            var html = await File.ReadAllTextAsync(filePath);
            templateInputs[kind] = html;
        }
    }
    if (t.Templates is { Count: > 0 })
    {
        foreach (var kv in t.Templates)
        {
            var kind = (kv.Key ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(kind)) continue;
            templateInputs[kind] = kv.Value ?? string.Empty;
        }
    }

    if (templateInputs.Count > 0)
    {
        foreach (var kv in templateInputs)
        {
            var kind = (kv.Key ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(kind)) continue;

            var html = kv.Value ?? string.Empty;
            Console.WriteLine($"  - TEMPLATE ensure '{kind}' ({html.Length} chars)");

            if (!apply) continue;

            var row = await tenantDb.DocumentTemplates.SingleOrDefaultAsync(x => x.TenantId == tenantId && x.Kind == kind);
            if (row is null)
            {
                tenantDb.DocumentTemplates.Add(new TenantDocumentTemplateRow
                {
                    TenantId = tenantId,
                    Kind = kind,
                    TemplateHtml = html,
                    UpdatedAt = now
                });
                templateUpserts++;
            }
            else if (!string.Equals(row.TemplateHtml, html, StringComparison.Ordinal))
            {
                if (updateTemplates)
                {
                    row.TemplateHtml = html;
                    row.UpdatedAt = now;
                    templateUpserts++;
                }
                else
                {
                    Console.WriteLine($"    (skip update; pass --update-templates to overwrite existing '{kind}')");
                }
            }
        }

        if (apply)
        {
            await tenantDb.SaveChangesAsync();
        }
    }

    if (seedSampleApps)
    {
        var hasAny = await citizenshipDb.Applications.AsNoTracking().AnyAsync(a => a.TenantId == tenantId);
        if (!hasAny)
        {
            Console.WriteLine($"  - SAMPLE citizenship application for '{tenantId}'");
            if (apply)
            {
                var appNo = NewApplicationNumber(tenantId, now);
                citizenshipDb.Applications.Add(new CitizenshipApplicationRow
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ApplicationNumber = appNo,
                    Status = "Submitted",
                    FirstName = "Test",
                    LastName = "Applicant",
                    DateOfBirth = new DateOnly(1990, 1, 1),
                    Email = $"{tenantId}.applicant@example.com",
                    AddressLine1 = "123 Main St",
                    City = "Testville",
                    Region = "TS",
                    PostalCode = "00000",
                    CreatedAt = now,
                    UpdatedAt = now
                });
                await citizenshipDb.SaveChangesAsync();
                sampleApps++;
            }
        }
    }

    if (t.UserInvites is { Count: > 0 } && !string.IsNullOrWhiteSpace(issuer))
    {
        foreach (var inv in t.UserInvites)
        {
            var email = (inv.Email ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email)) continue;

            Console.WriteLine($"  - INVITE ensure '{email}' -> {tenantId}");
            if (!apply) continue;

            var row = await tenantDb.UserInvites.SingleOrDefaultAsync(x => x.Issuer == issuer && x.Email == email);
            if (row is null)
            {
                tenantDb.UserInvites.Add(new TenantUserInviteRow
                {
                    Issuer = issuer,
                    Email = email,
                    TenantId = tenantId,
                    CreatedAt = now,
                    UpdatedAt = now,
                    UsedAt = null
                });
                inviteUpserts++;
            }
            else if (!string.Equals(row.TenantId, tenantId, StringComparison.OrdinalIgnoreCase))
            {
                row.TenantId = tenantId;
                row.UpdatedAt = now;
                inviteUpserts++;
            }
        }

        if (apply)
        {
            await tenantDb.SaveChangesAsync();
        }
    }
}

Console.WriteLine();
Console.WriteLine($"Done. created={created}, updated={updated}, templateUpserts={templateUpserts}, sampleApps={sampleApps}, inviteUpserts={inviteUpserts}");
return 0;

public sealed record SeedFile(IReadOnlyList<SeedTenant> Tenants)
{
    public IReadOnlyList<SeedTenant> Tenants { get; init; } = Tenants ?? Array.Empty<SeedTenant>();
}

public sealed record SeedTenant(
    string TenantId,
    string DisplayName,
    SeedBranding? Branding = null,
    DocumentNamingPattern? DocumentNaming = null,
    Dictionary<string, string>? Templates = null,
    Dictionary<string, string>? TemplateFiles = null,
    List<SeedUserInvite>? UserInvites = null);

public sealed record SeedBranding(
    string? SealLine1 = null,
    string? SealLine2 = null,
    string? ContactEmail = null);

public sealed record SeedUserInvite(string Email);
