using Mamey;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Mamey.Logging;
using Mamey.Secrets.Vault;
using System.Diagnostics;
using Mamey.FWID.Operations.Api.Infrastructure;
using Mamey.FWID.Operations.Api.Hubs;

namespace Mamey.FWID.Operations.Api;

public class Program
{
    public static async Task Main(string[] args)
        => await CreateHostBuilder(args)
            .Build()
            .RunAsync();
    private static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args).
              ConfigureWebHostDefaults(webhostBuilder =>
              {
                  webhostBuilder.ConfigureServices(services =>
                    services.AddMamey()
                        .AddWebApi()
                        .AddInfrastructure()
                        .Build()
                    )
                  .Configure(app =>

                    app
                       .AddFWIDOperationsRoutes()
                       .UseEndpoints(endpoints =>
                       {
                           endpoints.MapHub<FWIDHub>("/fwid");
                           endpoints.MapGrpcService<GrpcServiceHost>();
                       })
                  );
              })
              .UseLogging()
              .UseVault()
        ;
}

internal static class FWIDOperationsRoutes
{
    public static IApplicationBuilder AddFWIDOperationsRoutes(this IApplicationBuilder app)
    {
        app.UseEndpoints(endpoints =>
            endpoints
            .Get("", async context =>
            {
                var appOptions = context.RequestServices.GetService<AppOptions>();
                var serverStatus = new ServerStatus
                {
                    Uptime = GetUptime(),
                    CpuUsage = GetCpuUsage(),
                    MemoryUsage = GetMemoryUsage(),
                    TotalMemory = GetTotalMemory(),
                    DiskSpace = GetDiskSpace(),
                    AppName = appOptions.Name,
                    AppVersion = appOptions.Version,
                    Timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsJsonAsync(serverStatus);
            })
        );

        return app;
    }

    // Helper Functions
    // Get system uptime
    private static string GetUptime()
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
    }

    // Get CPU usage
    private static string GetCpuUsage()
    {
        var process = Process.GetCurrentProcess();
        return $"{process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount:F2}%";
    }

    // Get memory usage
    private static string GetMemoryUsage()
    {
        var process = Process.GetCurrentProcess();
        return $"{process.WorkingSet64 / (1024 * 1024)} MB used";
    }

    // Get total memory used by the application
    private static string GetTotalMemory()
    {
        return $"{GC.GetTotalMemory(false) / (1024 * 1024)} MB";
    }

    // Get available disk space
    private static string GetDiskSpace()
    {
        var driveInfo = new DriveInfo(Directory.GetCurrentDirectory());
        var availableSpace = driveInfo.AvailableFreeSpace / (1024 * 1024 * 1024); // GB
        var totalSpace = driveInfo.TotalSize / (1024 * 1024 * 1024); // GB
        return $"{availableSpace} GB free of {totalSpace} GB";
    }
    public class ServerStatus
    {
        public string Uptime { get; set; }
        public string CpuUsage { get; set; }
        public string MemoryUsage { get; set; }
        public string TotalMemory { get; set; }
        public string DiskSpace { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }
        public DateTime Timestamp { get; set; }
    }
}



