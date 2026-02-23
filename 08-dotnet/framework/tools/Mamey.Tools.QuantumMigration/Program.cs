using System.CommandLine;
using System.CommandLine.Invocation;
using System.Text.Json;
using Mamey.Security;
using Mamey.Security.PostQuantum.Models;
using Mamey.Tools.QuantumMigration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var rootCommand = new RootCommand("Mamey key migration tool (classical → PQC / hybrid)");

var migrateKeysCommand = new Command("migrate-keys", "Migrate classical keys to post-quantum or hybrid equivalents")
{
    new Option<string>("--source-wallet", description: "Path to source wallet or key store" ) { IsRequired = true },
    new Option<string>("--target-algorithm", () => "ml-dsa-65", "Target PQC algorithm (e.g. ml-dsa-65)"),
    new Option<bool>("--hybrid-mode", () => false, "Enable hybrid mode (link classical and PQC keys)"),
    new Option<bool>("--migrate-transactions", () => false, "Re-sign transactions with new keys"),
    new Option<string?>("--report", () => "migration-report.json", "Path to write migration report JSON")
};

migrateKeysCommand.SetHandler(async (InvocationContext ctx) =>
{
    var sourceWallet = ctx.ParseResult.GetValueForOption<string>("--source-wallet");
    var targetAlg = ctx.ParseResult.GetValueForOption<string>("--target-algorithm");
    var hybrid = ctx.ParseResult.GetValueForOption<bool>("--hybrid-mode");
    var migrateTx = ctx.ParseResult.GetValueForOption<bool>("--migrate-transactions");
    var reportPath = ctx.ParseResult.GetValueForOption<string?>("--report");

    using var host = Host.CreateDefaultBuilder()
        .ConfigureServices(services =>
        {
            services.AddSecurity(); // Assumes AddSecurity extension available in hosting app
        })
        .Build();

    var securityProvider = host.Services.GetRequiredService<ISecurityProvider>();
    var tool = new KeyMigrationTool(securityProvider);

    // In this shared tool we don't know how to read the concrete wallet format,
    // so we treat this as a placeholder list; real integrations should adapt
    // their wallet key representation to WalletKeyDescriptor.
    var keys = new List<WalletKeyDescriptor>();

    var options = new MigrationOptions
    {
        EnableHybridMode = hybrid,
        MigrateTransactions = migrateTx,
        TargetAlgorithm = targetAlg.ToUpperInvariant(),
        ReportPath = reportPath
    };

    var report = await tool.MigrateAllKeysAsync(
        keys,
        options,
        async migratedKey => await Task.CompletedTask,
        rollbackKeyAsync: async originalKey => await Task.CompletedTask,
        ctx.GetCancellationToken());

    Console.WriteLine("Migration finished:");
    Console.WriteLine(report);

    if (!string.IsNullOrWhiteSpace(reportPath))
    {
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(reportPath, json, ctx.GetCancellationToken());
        Console.WriteLine($"Report written to {reportPath}");
    }
});

var verifyCommand = new Command("verify-migration", "Verify a previous migration report")
{
    new Option<string>("--migration-report", description: "Path to migration report JSON" ) { IsRequired = true }
};

verifyCommand.SetHandler(async (InvocationContext ctx) =>
{
    var reportPath = ctx.ParseResult.GetValueForOption<string>("--migration-report");
    if (!File.Exists(reportPath))
    {
        Console.WriteLine($"Report file not found: {reportPath}");
        ctx.ExitCode = 1;
        return;
    }

    var json = await File.ReadAllTextAsync(reportPath, ctx.GetCancellationToken());
    var report = JsonSerializer.Deserialize<MigrationReport>(json);
    if (report is null)
    {
        Console.WriteLine("Failed to deserialize migration report.");
        ctx.ExitCode = 1;
        return;
    }

    Console.WriteLine("Loaded migration report:");
    Console.WriteLine(report);
    if (report.Errors.Count > 0)
    {
        Console.WriteLine("Errors:");
        foreach (var error in report.Errors)
        {
            Console.WriteLine($"- KeyId={error.KeyId}: {error.ErrorMessage}");
        }
    }
});

rootCommand.AddCommand(migrateKeysCommand);
rootCommand.AddCommand(verifyCommand);

return await rootCommand.InvokeAsync(args);


