var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Mamey SICB Whistleblower Reports", Version = "v1" }));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

var reports = new Dictionary<Guid, WhistleblowerReport>();
long counter = 1000000;

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.SICB.WhistleblowerReports" }));

app.MapPost("/api/v1/reports", (SubmitReportRequest req) =>
{
    var id = Interlocked.Increment(ref counter);
    var report = new WhistleblowerReport
    {
        Id = Guid.NewGuid(),
        ReportId = $"WB-{DateTime.UtcNow:yyyyMMdd}-{id}",
        Category = req.Category,
        Subject = req.Subject,
        Description = req.Description,
        EvidenceUrls = req.EvidenceUrls,
        IsAnonymous = req.IsAnonymous,
        ReporterAlias = req.IsAnonymous ? $"Anonymous-{Guid.NewGuid().ToString()[..8]}" : req.ReporterName,
        ContactMethod = req.ContactMethod,
        Status = ReportStatus.Submitted,
        CreatedAt = DateTime.UtcNow
    };
    reports[report.Id] = report;
    return Results.Ok(new { success = true, report = new { report.ReportId, report.Status, report.ReporterAlias } });
});

app.MapGet("/api/v1/reports/{reportId}", (string reportId) =>
{
    var report = reports.Values.FirstOrDefault(r => r.ReportId == reportId);
    return report != null ? Results.Ok(new { report }) : Results.NotFound();
});

app.MapPost("/api/v1/reports/{reportId}/update-status", (string reportId, UpdateStatusRequest req) =>
{
    var report = reports.Values.FirstOrDefault(r => r.ReportId == reportId);
    if (report == null) return Results.NotFound();
    report.Status = req.Status;
    report.UpdatedAt = DateTime.UtcNow;
    if (req.InvestigatorNotes != null) report.InvestigatorNotes = req.InvestigatorNotes;
    return Results.Ok(new { success = true });
});

app.MapGet("/api/v1/reports", (ReportStatus? status) =>
{
    var result = status.HasValue 
        ? reports.Values.Where(r => r.Status == status.Value).ToList()
        : reports.Values.ToList();
    return Results.Ok(new { reports = result.Select(r => new { r.ReportId, r.Category, r.Subject, r.Status, r.CreatedAt }) });
});

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║       MAMEY SICB WHISTLEBLOWER REPORTS v1.0.0                ║");
Console.WriteLine("║       Anonymous & Secure Reporting Channel                   ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();

record SubmitReportRequest(string Category, string Subject, string Description, List<string>? EvidenceUrls, bool IsAnonymous, string? ReporterName, string? ContactMethod);
record UpdateStatusRequest(ReportStatus Status, string? InvestigatorNotes);

class WhistleblowerReport
{
    public Guid Id { get; set; }
    public string ReportId { get; set; } = "";
    public string Category { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string>? EvidenceUrls { get; set; }
    public bool IsAnonymous { get; set; }
    public string? ReporterAlias { get; set; }
    public string? ContactMethod { get; set; }
    public ReportStatus Status { get; set; }
    public string? InvestigatorNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

enum ReportStatus { Submitted, UnderReview, Investigating, ActionTaken, Closed, Dismissed }
