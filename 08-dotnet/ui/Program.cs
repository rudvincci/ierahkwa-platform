using Mamey.Blockchain.Node;
using Mamey.Blockchain.Banking;
using Mamey.Blockchain.DID;
using Mamey.Blockchain.Government;
using Mamey.Blockchain.Wallet;
using Mamey.Blockchain.Crypto;
using Mamey.Blockchain.Metrics;
using Mamey.Blockchain.General;
using MameyNode.UI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Configure MameyNode clients
var mameyNodeConfig = builder.Configuration.GetSection("MameyNode");
var grpcTlsConfig = mameyNodeConfig.GetSection("GrpcTls");

static Mamey.Blockchain.Node.GrpcTlsOptions LoadNodeTls(IConfigurationSection grpcTlsConfig) => new()
{
    CaCertificatePath = grpcTlsConfig["CaCertificatePath"],
    ClientCertificatePath = grpcTlsConfig["ClientCertificatePath"],
    ClientKeyPath = grpcTlsConfig["ClientKeyPath"],
    SkipServerCertificateValidation = bool.TryParse(grpcTlsConfig["SkipServerCertificateValidation"], out var v) && v
};

static Mamey.Blockchain.Banking.GrpcTlsOptions LoadBankingTls(IConfigurationSection grpcTlsConfig) => new()
{
    CaCertificatePath = grpcTlsConfig["CaCertificatePath"],
    ClientCertificatePath = grpcTlsConfig["ClientCertificatePath"],
    ClientKeyPath = grpcTlsConfig["ClientKeyPath"],
    SkipServerCertificateValidation = bool.TryParse(grpcTlsConfig["SkipServerCertificateValidation"], out var v) && v
};

static Mamey.Blockchain.Government.GrpcTlsOptions LoadGovernmentTls(IConfigurationSection grpcTlsConfig) => new()
{
    CaCertificatePath = grpcTlsConfig["CaCertificatePath"],
    ClientCertificatePath = grpcTlsConfig["ClientCertificatePath"],
    ClientKeyPath = grpcTlsConfig["ClientKeyPath"],
    SkipServerCertificateValidation = bool.TryParse(grpcTlsConfig["SkipServerCertificateValidation"], out var v) && v
};

static Mamey.Blockchain.DID.GrpcTlsOptions LoadDidTls(IConfigurationSection grpcTlsConfig) => new()
{
    CaCertificatePath = grpcTlsConfig["CaCertificatePath"],
    ClientCertificatePath = grpcTlsConfig["ClientCertificatePath"],
    ClientKeyPath = grpcTlsConfig["ClientKeyPath"],
    SkipServerCertificateValidation = bool.TryParse(grpcTlsConfig["SkipServerCertificateValidation"], out var v) && v
};

static Mamey.Blockchain.Wallet.GrpcTlsOptions LoadWalletTls(IConfigurationSection grpcTlsConfig) => new()
{
    CaCertificatePath = grpcTlsConfig["CaCertificatePath"],
    ClientCertificatePath = grpcTlsConfig["ClientCertificatePath"],
    ClientKeyPath = grpcTlsConfig["ClientKeyPath"],
    SkipServerCertificateValidation = bool.TryParse(grpcTlsConfig["SkipServerCertificateValidation"], out var v) && v
};

static Mamey.Blockchain.Crypto.GrpcTlsOptions LoadCryptoTls(IConfigurationSection grpcTlsConfig) => new()
{
    CaCertificatePath = grpcTlsConfig["CaCertificatePath"],
    ClientCertificatePath = grpcTlsConfig["ClientCertificatePath"],
    ClientKeyPath = grpcTlsConfig["ClientKeyPath"],
    SkipServerCertificateValidation = bool.TryParse(grpcTlsConfig["SkipServerCertificateValidation"], out var v) && v
};

static Mamey.Blockchain.Metrics.GrpcTlsOptions LoadMetricsTls(IConfigurationSection grpcTlsConfig) => new()
{
    CaCertificatePath = grpcTlsConfig["CaCertificatePath"],
    ClientCertificatePath = grpcTlsConfig["ClientCertificatePath"],
    ClientKeyPath = grpcTlsConfig["ClientKeyPath"],
    SkipServerCertificateValidation = bool.TryParse(grpcTlsConfig["SkipServerCertificateValidation"], out var v) && v
};

static Mamey.Blockchain.General.GrpcTlsOptions LoadGeneralTls(IConfigurationSection grpcTlsConfig) => new()
{
    CaCertificatePath = grpcTlsConfig["CaCertificatePath"],
    ClientCertificatePath = grpcTlsConfig["ClientCertificatePath"],
    ClientKeyPath = grpcTlsConfig["ClientKeyPath"],
    SkipServerCertificateValidation = bool.TryParse(grpcTlsConfig["SkipServerCertificateValidation"], out var v) && v
};

builder.Services.AddMameyNodeClient(options =>
{
    options.NodeUrl = mameyNodeConfig["GrpcEndpoint"] ?? "http://localhost:50051";
    options.TimeoutSeconds = int.Parse(mameyNodeConfig["TimeoutSeconds"] ?? "30");
    options.DefaultInstitutionId = mameyNodeConfig["DefaultInstitutionId"];
    options.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
    options.Tls = LoadNodeTls(grpcTlsConfig);
});

builder.Services.AddMameyBankingClient(options =>
{
    options.NodeUrl = mameyNodeConfig["GrpcEndpoint"] ?? "http://localhost:50051";
    options.TimeoutSeconds = int.Parse(mameyNodeConfig["TimeoutSeconds"] ?? "30");
    options.DefaultInstitutionId = mameyNodeConfig["DefaultInstitutionId"];
    options.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
    options.Tls = LoadBankingTls(grpcTlsConfig);
});

builder.Services.AddMameyGovernmentClient(options =>
{
    options.NodeUrl = mameyNodeConfig["GrpcEndpoint"] ?? "http://localhost:50051";
    options.TimeoutSeconds = int.Parse(mameyNodeConfig["TimeoutSeconds"] ?? "30");
    options.DefaultInstitutionId = mameyNodeConfig["DefaultInstitutionId"];
    options.CorrelationIdGenerator = () => Guid.NewGuid().ToString();
    options.Tls = LoadGovernmentTls(grpcTlsConfig);
});

builder.Services.AddDIDClient(options =>
{
    options.NodeUrl = mameyNodeConfig["GrpcEndpoint"] ?? "http://localhost:50051";
    options.TimeoutSeconds = int.Parse(mameyNodeConfig["TimeoutSeconds"] ?? "30");
    options.Tls = LoadDidTls(grpcTlsConfig);
});

builder.Services.AddMameyWalletClient(options =>
{
    options.NodeUrl = mameyNodeConfig["GrpcEndpoint"] ?? "http://localhost:50051";
    options.TimeoutSeconds = int.Parse(mameyNodeConfig["TimeoutSeconds"] ?? "30");
    options.Tls = LoadWalletTls(grpcTlsConfig);
});

builder.Services.AddMameyCryptoClient(options =>
{
    options.NodeUrl = mameyNodeConfig["GrpcEndpoint"] ?? "http://localhost:50051";
    options.TimeoutSeconds = int.Parse(mameyNodeConfig["TimeoutSeconds"] ?? "30");
    options.Tls = LoadCryptoTls(grpcTlsConfig);
});

builder.Services.AddMameyMetricsClient(options =>
{
    options.NodeUrl = mameyNodeConfig["GrpcEndpoint"] ?? "http://localhost:50051";
    options.TimeoutSeconds = int.Parse(mameyNodeConfig["TimeoutSeconds"] ?? "30");
    options.Tls = LoadMetricsTls(grpcTlsConfig);
});

builder.Services.AddMameyGeneralClient(options =>
{
    options.NodeUrl = mameyNodeConfig["GrpcEndpoint"] ?? "http://localhost:50051";
    options.TimeoutSeconds = int.Parse(mameyNodeConfig["TimeoutSeconds"] ?? "30");
    options.Tls = LoadGeneralTls(grpcTlsConfig);
});

// Add metadata service
builder.Services.AddScoped<IMetadataService, MetadataService>();
builder.Services.AddScoped<AuthSessionService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<DidAuthService>();
builder.Services.AddSingleton<AuthorityRegistryStore>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

Log.Information("MameyNode.UI starting on {Urls}", string.Join(", ", app.Urls));

app.Run();
