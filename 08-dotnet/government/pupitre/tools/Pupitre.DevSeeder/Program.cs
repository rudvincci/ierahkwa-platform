using System.Net;
using System.Net.Http.Json;
using Pupitre.Assessments.Contracts.Commands;
using Pupitre.GLEs.Contracts.Commands;
using Pupitre.Lessons.Contracts.Commands;
using Pupitre.Users.Contracts.Commands;

var options = DevSeedOptions.FromArgs(args);
Console.WriteLine($"[DevSeeder] Using API base: {options.ApiBaseUri} (dry-run: {options.DryRun})");

using var httpClient = new HttpClient
{
    BaseAddress = options.ApiBaseUri
};

var seeder = new DevSeeder(httpClient, options);
var summary = await seeder.SeedAsync();

Console.WriteLine($"[DevSeeder] Created: {summary.Created}, Skipped: {summary.Skipped}, Errors: {summary.Errors.Count}");
if (summary.Errors.Count > 0)
{
    Console.WriteLine("[DevSeeder] Error details:");
    foreach (var error in summary.Errors)
    {
        Console.WriteLine($"  - {error}");
    }
}

return summary.Errors.Count == 0 ? 0 : 1;

internal sealed record DevSeedOptions(Uri ApiBaseUri, bool DryRun)
{
    private const string DefaultBase = "http://localhost:60000/api/";

    public static DevSeedOptions FromArgs(string[] args)
    {
        static string? GetArg(string[] args, string key)
            => args.FirstOrDefault(a => a.StartsWith(key, StringComparison.OrdinalIgnoreCase))?
                .Split('=', 2)
                .ElementAtOrDefault(1);

        var baseUrl = GetArg(args, "--api-base")
                      ?? Environment.GetEnvironmentVariable("PUPITRE_API_BASE")
                      ?? DefaultBase;

        if (!baseUrl.EndsWith('/'))
        {
            baseUrl += "/";
        }

        var dryRun = args.Contains("--dry-run", StringComparer.OrdinalIgnoreCase)
                     || string.Equals(Environment.GetEnvironmentVariable("PUPITRE_SEED_DRYRUN"), "true", StringComparison.OrdinalIgnoreCase)
                     || Environment.GetEnvironmentVariable("PUPITRE_SEED_DRYRUN") == "1";

        return new DevSeedOptions(new Uri(baseUrl, UriKind.Absolute), dryRun);
    }
}

internal sealed class DevSeeder
{
    private readonly HttpClient _httpClient;
    private readonly DevSeedOptions _options;

    public DevSeeder(HttpClient httpClient, DevSeedOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<SeedSummary> SeedAsync(CancellationToken cancellationToken = default)
    {
        var summary = new SeedSummary();

        summary.Append(await SeedAsync("users", SampleData.Users(), cancellationToken));
        summary.Append(await SeedAsync("gles", SampleData.GLEs(), cancellationToken));
        summary.Append(await SeedAsync("lessons", SampleData.Lessons(), cancellationToken));
        summary.Append(await SeedAsync("assessments", SampleData.Assessments(), cancellationToken));

        return summary;
    }

    private async Task<SeedResult> SeedAsync<T>(string relativePath, IEnumerable<T> payloads, CancellationToken cancellationToken)
    {
        var created = 0;
        var skipped = 0;
        var errors = new List<string>();

        foreach (var payload in payloads)
        {
            if (_options.DryRun)
            {
                skipped++;
                Console.WriteLine($"[DevSeeder] DRY-RUN would POST {relativePath}: {System.Text.Json.JsonSerializer.Serialize(payload)}");
                continue;
            }

            HttpResponseMessage? response = null;
            try
            {
                response = await _httpClient.PostAsJsonAsync(relativePath, payload, cancellationToken);
                if (response.IsSuccessStatusCode)
                {
                    created++;
                    Console.WriteLine($"[DevSeeder] Created {relativePath} resource ({response.StatusCode}).");
                }
                else if (response.StatusCode is HttpStatusCode.Conflict or HttpStatusCode.BadRequest or HttpStatusCode.UnprocessableEntity)
                {
                    skipped++;
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    Console.WriteLine($"[DevSeeder] Skipped {relativePath} resource ({response.StatusCode}): {content}");
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    errors.Add($"POST {relativePath} failed with {(int)response.StatusCode} {response.StatusCode}: {content}");
                }
            }
            catch (Exception ex)
            {
                errors.Add($"POST {relativePath} failed: {ex.Message}");
            }
            finally
            {
                response?.Dispose();
            }
        }

        return new SeedResult(created, skipped, errors);
    }
}

internal sealed class SeedSummary
{
    private readonly List<string> _errors = new();

    public int Created { get; private set; }
    public int Skipped { get; private set; }
    public IReadOnlyList<string> Errors => _errors;

    public void Append(SeedResult result)
    {
        Created += result.Created;
        Skipped += result.Skipped;
        _errors.AddRange(result.Errors);
    }
}

internal sealed record SeedResult(int Created, int Skipped, IReadOnlyList<string> Errors);

internal static class SampleData
{
    public static IEnumerable<AddUser> Users()
    {
        yield return new AddUser(
            id: Guid.Parse("11111111-1111-4000-8000-000000000001"),
            name: "Ava Martinez",
            tags: new[] { "seed", "student" },
            citizenId: "STUDENT-001",
            firstName: "Ava",
            lastName: "Martinez",
            dateOfBirth: new DateOnly(2010, 5, 2),
            nationality: "USA",
            programCode: "STEM-EXCEL",
            credentialType: "Profile",
            completionDate: null,
            publishToLedger: false)
        {
            Metadata = new Dictionary<string, string>
            {
                ["source"] = "dev-seed",
                ["cohort"] = "Beta-2025"
            }
        };

        yield return new AddUser(
            id: Guid.Parse("11111111-1111-4000-8000-000000000002"),
            name: "Leo Nakamura",
            tags: new[] { "seed", "student" },
            citizenId: "STUDENT-002",
            firstName: "Leo",
            lastName: "Nakamura",
            dateOfBirth: new DateOnly(2009, 11, 15),
            nationality: "CAN",
            programCode: "LANG-IMMERSION",
            credentialType: "Profile",
            completionDate: null,
            publishToLedger: false)
        {
            Metadata = new Dictionary<string, string>
            {
                ["source"] = "dev-seed",
                ["cohort"] = "Beta-2025"
            }
        };
    }

    public static IEnumerable<AddGLE> GLEs()
    {
        yield return new AddGLE(
            id: Guid.Parse("22222222-1111-4000-8000-000000000001"),
            name: "Mathematics Foundations GLE",
            tags: new[] { "grade-4", "math" },
            citizenId: "STUDENT-001",
            nationality: "USA",
            programCode: "GLE-MATH-04",
            credentialType: "GLE",
            completionDate: DateTime.UtcNow.AddDays(-7),
            publishToLedger: false)
        {
            Metadata = new Dictionary<string, string>
            {
                ["strand"] = "Number Sense",
                ["source"] = "dev-seed"
            }
        };

        yield return new AddGLE(
            id: Guid.Parse("22222222-1111-4000-8000-000000000002"),
            name: "Literacy Competency GLE",
            tags: new[] { "grade-5", "literacy" },
            citizenId: "STUDENT-002",
            nationality: "CAN",
            programCode: "GLE-LIT-05",
            credentialType: "GLE",
            completionDate: DateTime.UtcNow.AddDays(-3),
            publishToLedger: false)
        {
            Metadata = new Dictionary<string, string>
            {
                ["strand"] = "Reading Comprehension",
                ["source"] = "dev-seed"
            }
        };
    }

    public static IEnumerable<AddLesson> Lessons()
    {
        yield return new AddLesson(
            id: Guid.Parse("33333333-1111-4000-8000-000000000001"),
            name: "Fraction Models with Robotics",
            tags: new[] { "math", "robotics", "hands-on" },
            citizenId: "TEACHER-001",
            firstName: "Sofia",
            lastName: "Ramirez",
            programCode: "LESSON-MATH-STEAM",
            credentialType: "LessonPlan",
            completionDate: null,
            publishToLedger: false)
        {
            Metadata = new Dictionary<string, string>
            {
                ["duration"] = "45",
                ["materials"] = "VEX GO Kit",
                ["source"] = "dev-seed"
            }
        };

        yield return new AddLesson(
            id: Guid.Parse("33333333-1111-4000-8000-000000000002"),
            name: "Community Story Circles",
            tags: new[] { "literacy", "project-based" },
            citizenId: "TEACHER-002",
            firstName: "Noah",
            lastName: "Singh",
            programCode: "LESSON-LIT-SOCIAL",
            credentialType: "LessonPlan",
            completionDate: null,
            publishToLedger: false)
        {
            Metadata = new Dictionary<string, string>
            {
                ["duration"] = "60",
                ["materials"] = "Story circle kit",
                ["source"] = "dev-seed"
            }
        };
    }

    public static IEnumerable<AddAssessment> Assessments()
    {
        yield return new AddAssessment(
            id: Guid.Parse("44444444-1111-4000-8000-000000000001"),
            name: "Quarterly Math Benchmark",
            tags: new[] { "math", "benchmark" },
            citizenId: "ASSESS-001",
            programCode: "ASSESS-MATH-Q2",
            credentialType: "Benchmark",
            completionDate: DateTime.UtcNow.AddDays(-1),
            publishToLedger: false)
        {
            Metadata = new Dictionary<string, string>
            {
                ["grade"] = "4",
                ["items"] = "32",
                ["source"] = "dev-seed"
            }
        };

        yield return new AddAssessment(
            id: Guid.Parse("44444444-1111-4000-8000-000000000002"),
            name: "Project-Based Literacy Rubric",
            tags: new[] { "literacy", "rubric" },
            citizenId: "ASSESS-002",
            programCode: "ASSESS-LIT-RUBRIC",
            credentialType: "Rubric",
            completionDate: DateTime.UtcNow,
            publishToLedger: false)
        {
            Metadata = new Dictionary<string, string>
            {
                ["grade"] = "5",
                ["dimensions"] = "4",
                ["source"] = "dev-seed"
            }
        };
    }
}
