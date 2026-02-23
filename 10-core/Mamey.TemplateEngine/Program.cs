using System.Text.RegularExpressions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v1", new() { Title = "Mamey Template Engine", Version = "v1" }));
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

var templates = new Dictionary<string, Template>
{
    ["INVOICE"] = new Template { Name = "INVOICE", Content = @"
<html><head><title>Invoice {{invoice_number}}</title></head>
<body>
<h1>INVOICE #{{invoice_number}}</h1>
<p>Date: {{date}}</p>
<p>To: {{customer_name}}</p>
<p>Amount: {{amount}} {{currency}}</p>
<p>Description: {{description}}</p>
</body></html>", Type = TemplateType.Html },
    
    ["RECEIPT"] = new Template { Name = "RECEIPT", Content = @"
RECEIPT - Sovereign Government of Ierahkwa
==========================================
Receipt #: {{receipt_number}}
Date: {{date}}
From: {{payer_name}}
Amount: {{amount}} {{currency}}
For: {{description}}
==========================================
Thank you for your contribution.", Type = TemplateType.Text },
    
    ["CERTIFICATE"] = new Template { Name = "CERTIFICATE", Content = @"
<html><head><title>Certificate</title></head>
<body style='text-align:center;'>
<h1>CERTIFICATE OF {{certificate_type}}</h1>
<p>This certifies that</p>
<h2>{{recipient_name}}</h2>
<p>FutureWampumID: {{fwid}}</p>
<p>{{description}}</p>
<p>Issued: {{date}}</p>
<p>Certificate ID: {{certificate_id}}</p>
</body></html>", Type = TemplateType.Html },
    
    ["CONTRACT"] = new Template { Name = "CONTRACT", Content = @"
CONTRACT AGREEMENT
==================

Contract ID: {{contract_id}}
Date: {{date}}

PARTIES:
1. {{party1_name}} ({{party1_role}})
2. {{party2_name}} ({{party2_role}})

TERMS:
{{terms}}

SIGNATURES:
Party 1: _________________________ Date: _________
Party 2: _________________________ Date: _________", Type = TemplateType.Text }
};

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "Mamey.TemplateEngine" }));

app.MapGet("/api/v1/templates", () => Results.Ok(new { templates = templates.Values.Select(t => new { t.Name, t.Type }) }));

app.MapGet("/api/v1/templates/{name}", (string name) =>
{
    return templates.TryGetValue(name.ToUpper(), out var t) ? Results.Ok(new { template = t }) : Results.NotFound();
});

app.MapPost("/api/v1/templates", (CreateTemplateRequest req) =>
{
    var template = new Template { Name = req.Name.ToUpper(), Content = req.Content, Type = req.Type, Variables = ExtractVariables(req.Content) };
    templates[template.Name] = template;
    return Results.Ok(new { success = true, template });
});

app.MapPost("/api/v1/render", (RenderRequest req) =>
{
    if (!templates.TryGetValue(req.TemplateName.ToUpper(), out var template))
        return Results.NotFound(new { error = "Template not found" });
    
    var output = template.Content;
    foreach (var (key, value) in req.Variables)
        output = output.Replace($"{{{{{key}}}}}", value?.ToString() ?? "");
    
    return Results.Ok(new { 
        success = true, 
        output,
        contentType = template.Type == TemplateType.Html ? "text/html" : "text/plain"
    });
});

app.MapPost("/api/v1/render-batch", (BatchRenderRequest req) =>
{
    if (!templates.TryGetValue(req.TemplateName.ToUpper(), out var template))
        return Results.NotFound(new { error = "Template not found" });
    
    var results = req.DataRows.Select(row =>
    {
        var output = template.Content;
        foreach (var (key, value) in row)
            output = output.Replace($"{{{{{key}}}}}", value?.ToString() ?? "");
        return output;
    }).ToList();
    
    return Results.Ok(new { success = true, outputs = results, count = results.Count });
});

List<string> ExtractVariables(string content) => 
    Regex.Matches(content, @"\{\{(\w+)\}\}").Select(m => m.Groups[1].Value).Distinct().ToList();

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║            MAMEY TEMPLATE ENGINE v1.0.0                      ║");
Console.WriteLine("║            Dynamic Document Generation                       ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");

app.Run();

record CreateTemplateRequest(string Name, string Content, TemplateType Type);
record RenderRequest(string TemplateName, Dictionary<string, object> Variables);
record BatchRenderRequest(string TemplateName, List<Dictionary<string, object>> DataRows);

class Template
{
    public string Name { get; set; } = "";
    public string Content { get; set; } = "";
    public TemplateType Type { get; set; }
    public List<string>? Variables { get; set; }
}

enum TemplateType { Text, Html, Markdown, Pdf }
