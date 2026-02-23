namespace IERAHKWA.Platform.Models;

// ============= CODE GENERATION =============

public class CodeGenerateRequest
{
    public string Prompt { get; set; } = "";
    public string Language { get; set; } = "csharp";
    public string? Framework { get; set; }
    public bool IncludeComments { get; set; } = true;
    public bool IncludeTests { get; set; } = false;
}

public class CodeGenerateResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Code { get; set; } = "";
    public string Language { get; set; } = "";
    public string? FileName { get; set; }
    public List<string>? AdditionalFiles { get; set; }
    public int LinesOfCode { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

// ============= WEB GENERATION =============

public class WebGenerateRequest
{
    public string Prompt { get; set; } = "";
    public string Type { get; set; } = "landing"; // landing, portfolio, ecommerce, blog, dashboard, saas
    public string Style { get; set; } = "modern"; // modern, dark, neon, corporate, creative
    public bool IncludeJS { get; set; } = true;
    public bool Responsive { get; set; } = true;
}

public class WebGenerateResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Html { get; set; } = "";
    public string Css { get; set; } = "";
    public string Js { get; set; } = "";
    public string FullPage { get; set; } = ""; // Combined HTML with inline CSS/JS
    public string PreviewUrl { get; set; } = "";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

// ============= APP GENERATION =============

public class AppGenerateRequest
{
    public string Prompt { get; set; } = "";
    public string Platform { get; set; } = "react-native"; // react-native, flutter, ios, android
    public string Type { get; set; } = "social"; // social, ecommerce, fintech, fitness, delivery, education
    public string Features { get; set; } = "basic"; // basic, advanced, full
}

public class AppGenerateResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Platform { get; set; } = "";
    public Dictionary<string, string> Files { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
    public string SetupInstructions { get; set; } = "";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

// ============= API GENERATION =============

public class APIGenerateRequest
{
    public string Prompt { get; set; } = "";
    public string Framework { get; set; } = "aspnet"; // aspnet, express, fastapi, django, spring
    public string Database { get; set; } = "postgresql"; // postgresql, mysql, mongodb, sqlite
    public bool IncludeAuth { get; set; } = true;
    public bool IncludeSwagger { get; set; } = true;
}

public class APIGenerateResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Framework { get; set; } = "";
    public Dictionary<string, string> Files { get; set; } = new();
    public List<APIEndpoint> Endpoints { get; set; } = new();
    public string ConnectionString { get; set; } = "";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class APIEndpoint
{
    public string Method { get; set; } = "GET";
    public string Path { get; set; } = "";
    public string Description { get; set; } = "";
    public string? RequestBody { get; set; }
    public string? ResponseBody { get; set; }
}

// ============= BOT GENERATION =============

public class BotGenerateRequest
{
    public string Prompt { get; set; } = "";
    public string Platform { get; set; } = "telegram"; // telegram, whatsapp, discord, slack, web
    public string Purpose { get; set; } = "support"; // support, sales, booking, faq, assistant
    public List<string>? Commands { get; set; }
}

public class BotGenerateResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Platform { get; set; } = "";
    public string MainCode { get; set; } = "";
    public Dictionary<string, string> Files { get; set; } = new();
    public List<BotCommand> Commands { get; set; } = new();
    public string SetupInstructions { get; set; } = "";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class BotCommand
{
    public string Command { get; set; } = "";
    public string Description { get; set; } = "";
    public string Response { get; set; } = "";
}

// ============= DOCUMENT ANALYSIS =============

public class DocumentAnalyzeRequest
{
    public string Text { get; set; } = "";
    public string Action { get; set; } = "summarize"; // summarize, extract, translate, analyze
    public string? TargetLanguage { get; set; }
}

public class DocumentAnalyzeResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Summary { get; set; } = "";
    public List<string> KeyPoints { get; set; } = new();
    public Dictionary<string, string> ExtractedData { get; set; } = new();
    public string? Translation { get; set; }
    public int WordCount { get; set; }
    public string Sentiment { get; set; } = "neutral";
    public DateTime AnalyzedAt { get; set; } = DateTime.UtcNow;
}

// ============= SMART CONTRACT =============

public class SmartContractRequest
{
    public string Prompt { get; set; } = "";
    public string Type { get; set; } = "token"; // token, nft, defi, dao, marketplace
    public string Standard { get; set; } = "ERC20"; // ERC20, ERC721, ERC1155, custom
    public string? TokenName { get; set; }
    public string? TokenSymbol { get; set; }
    public decimal? TotalSupply { get; set; }
}

public class SmartContractResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ContractCode { get; set; } = "";
    public string ContractName { get; set; } = "";
    public string Standard { get; set; } = "";
    public List<ContractFunction> Functions { get; set; } = new();
    public string DeployScript { get; set; } = "";
    public string TestCode { get; set; } = "";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class ContractFunction
{
    public string Name { get; set; } = "";
    public string Visibility { get; set; } = "public";
    public string[] Parameters { get; set; } = Array.Empty<string>();
    public string Returns { get; set; } = "";
    public string Description { get; set; } = "";
}

// ============= TEMPLATES =============

public class AITemplate
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = ""; // code, web, app, api, bot, contract
    public string Language { get; set; } = "";
    public string Code { get; set; } = "";
    public string[] Tags { get; set; } = Array.Empty<string>();
}

// ============= STATS =============

public class AIStudioStats
{
    public int TotalGenerations { get; set; }
    public int CodeGenerations { get; set; }
    public int WebGenerations { get; set; }
    public int AppGenerations { get; set; }
    public int APIGenerations { get; set; }
    public int BotGenerations { get; set; }
    public int ContractGenerations { get; set; }
    public int DocumentsAnalyzed { get; set; }
    public long TotalLinesGenerated { get; set; }
    public double AverageResponseTime { get; set; }
}
