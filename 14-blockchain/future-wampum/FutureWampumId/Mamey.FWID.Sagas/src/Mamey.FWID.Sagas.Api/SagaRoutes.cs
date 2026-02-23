using System.Diagnostics;
using Mamey.FWID.Sagas.Core.Commands;
using Mamey.Types;
using Mamey.WebApi;
using Mamey.WebApi.CQRS;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;

namespace Mamey.FWID.Sagas.Api;

internal static class SagaRoutes
{
    public static IApplicationBuilder AddFWIDSagaRoutes(this IApplicationBuilder app)
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
                    AppName = appOptions?.Name ?? "FWID Sagas Service",
                    AppVersion = appOptions?.Version ?? "1.0.0",
                    Timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsJsonAsync(serverStatus);
            })
        );

        app.UseDispatcherEndpoints(endpoints =>
        endpoints
            .Post<StartIdentityRegistration>("/api/sagas/identity-registration",
                beforeDispatch: null,
                afterDispatch: ([FromBody] cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 201;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Identity Registration saga started", sagaId = cmd.IdentityId ?? Guid.NewGuid() });
                },
                auth: false)
            
            .Post<StartCredentialIssuance>("/api/sagas/credential-issuance",
                beforeDispatch: null,
                afterDispatch: ([FromBody] cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 201;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Credential Issuance saga started", identityId = cmd.IdentityId });
                },
                auth: false)
            
            .Post<StartAccessControlGranting>("/api/sagas/access-control-granting",
                beforeDispatch: null,
                afterDispatch: ([FromBody] cmd, ctx) =>
                {
                    ctx.Response.StatusCode = 201;
                    return ctx.Response.WriteAsJsonAsync(new { message = "Access Control Granting saga started", identityId = cmd.IdentityId, zoneId = cmd.ZoneId });
                },
                auth: false)
        );
        return app;
    }

    // Helper Functions
    private static string GetUptime()
    {
        var uptime = TimeSpan.FromMilliseconds(Environment.TickCount64);
        return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
    }

    private static string GetCpuUsage()
    {
        var process = Process.GetCurrentProcess();
        return $"{process.TotalProcessorTime.TotalMilliseconds / Environment.ProcessorCount:F2}%";
    }

    private static string GetMemoryUsage()
    {
        var process = Process.GetCurrentProcess();
        return $"{process.WorkingSet64 / (1024 * 1024)} MB used";
    }

    private static string GetTotalMemory()
    {
        return $"{GC.GetTotalMemory(false) / (1024 * 1024)} MB";
    }

    private static string GetDiskSpace()
    {
        var driveInfo = new DriveInfo(Directory.GetCurrentDirectory());
        var availableSpace = driveInfo.AvailableFreeSpace / (1024 * 1024 * 1024); // GB
        var totalSpace = driveInfo.TotalSize / (1024 * 1024 * 1024); // GB
        return $"{availableSpace} GB free of {totalSpace} GB";
    }

    public class ServerStatus
    {
        public string Uptime { get; set; } = string.Empty;
        public string CpuUsage { get; set; } = string.Empty;
        public string MemoryUsage { get; set; } = string.Empty;
        public string TotalMemory { get; set; } = string.Empty;
        public string DiskSpace { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string AppVersion { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}



