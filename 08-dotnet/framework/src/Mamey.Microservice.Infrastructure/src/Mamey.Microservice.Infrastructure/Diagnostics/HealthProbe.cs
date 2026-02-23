using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Mamey.Microservice.Infrastructure.Diagnostics;

public static class HealthProbe
{
    public static HealthResponse Build(
        HttpContext context,
        IWebHostEnvironment env,
        string? appName = null,
        string? version = null,
        string? gitSha = null,
        HealthProbeOptions? options = null)
    {
        options ??= new();

        var now = DateTimeOffset.UtcNow;

        // Request / client
        var userAgent = context.Request.Headers.UserAgent.ToString();
        var cfCountry = context.Request.Headers["CF-IPCountry"].ToString();
        var trueClientIp = context.Request.Headers["True-Client-IP"].ToString();
        var cfConnectingIp = context.Request.Headers["CF-Connecting-IP"].ToString();
        var xff = context.Request.Headers["X-Forwarded-For"].ToString();
        var remoteIp = GetClientIp(context, cfConnectingIp, trueClientIp, xff) ?? context.Connection.RemoteIpAddress?.ToString();

        var tls = context.Features.Get<ITlsHandshakeFeature>();
        var activityId = Activity.Current?.Id;
        var traceId = Activity.Current?.TraceId.ToString();
        var requestId = context.TraceIdentifier;

        // Server / runtime
        var proc = Process.GetCurrentProcess();
        var gcInfo = GC.GetGCMemoryInfo();

        var serverInfo = new ServerInfo(
            Machine: Environment.MachineName,
            OS: RuntimeInformation.OSDescription,
            Framework: RuntimeInformation.FrameworkDescription,
            ProcessArchitecture: RuntimeInformation.ProcessArchitecture.ToString(),
            CpuCount: Environment.ProcessorCount,
            StartedAtUtc: proc.StartTime.ToUniversalTime(),
            UptimeSeconds: (now - proc.StartTime.ToUniversalTime()).TotalSeconds,
            IsHttps: context.Request.IsHttps,
            LocalIpAddress: options.MaskSensitive ? null : context.Connection.LocalIpAddress?.ToString(),
            LocalPort: options.MaskSensitive ? null : context.Connection.LocalPort,
            TlsProtocol: tls?.Protocol.ToString(),
            TlsCipher: tls?.CipherAlgorithm.ToString(),
            Memory: new MemoryInfo(
                WorkingSetBytes: proc.WorkingSet64,
                GcHeapBytes: GC.GetTotalMemory(false),
                TotalAvailableMemoryBytes: gcInfo.TotalAvailableMemoryBytes > 0 ? gcInfo.TotalAvailableMemoryBytes : null
            ),
            Network: options.IncludeNetwork ? GetServerNics(options.IncludeIPv6) : new List<NicInfo>()
        );

        var clientInfo = new ClientInfo(
            UserAgent: userAgent,
            RemoteIpAddress: remoteIp,
            RemotePort: context.Connection.RemotePort,
            ProxyChain: xff,
            OriginatingCountry: string.IsNullOrWhiteSpace(cfCountry) ? null : cfCountry,
            OperatingSystemGuess: GuessClientOs(userAgent),
            ClientMacAddress: null // not available over HTTP
        );

        return new HealthResponse(
            TimestampUtc: now,
            Environment: env.EnvironmentName,
            AppName: appName ?? env.ApplicationName,
            Version: version ?? typeof(HealthProbe).Assembly.GetName().Version?.ToString(),
            GitSha: gitSha ?? Environment.GetEnvironmentVariable("GIT_SHA"),
            Request: new RequestInfo(
                Scheme: context.Request.Scheme,
                Host: context.Request.Host.Value ?? string.Empty,
                Path: context.Request.Path.Value,
                Query: context.Request.QueryString.HasValue ? context.Request.QueryString.Value : null,
                TraceId: traceId,
                ActivityId: activityId,
                RequestId: requestId
            ),
            Client: clientInfo,
            Server: serverInfo
        );
    }

    // Minimal API helper usable on IEndpointRouteBuilder
    public static RouteHandlerBuilder MapHealthEndpoint(
        this IEndpointRouteBuilder app,
        string pattern = "/health",
        Action<HealthProbeOptions>? configure = null)
    {
        var opt = new HealthProbeOptions();
        configure?.Invoke(opt);

        return app.MapGet(pattern, (HttpContext ctx, IWebHostEnvironment env) =>
        {
            var payload = Build(ctx, env, opt.AppName, opt.Version, opt.GitSha, opt);
            return Results.Json(payload);
        })
        .WithName("Health")
        .Produces<HealthResponse>(StatusCodes.Status200OK, "application/json");
    }

    // ------- helpers (were missing) -------
    private static string? GetClientIp(HttpContext ctx, string cfConnectingIp, string trueClientIp, string xff)
    {
        if (!string.IsNullOrWhiteSpace(cfConnectingIp)) return cfConnectingIp;
        if (!string.IsNullOrWhiteSpace(trueClientIp)) return trueClientIp;
        if (!string.IsNullOrWhiteSpace(xff))
        {
            var first = xff.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(first)) return first;
        }
        return ctx.Connection.RemoteIpAddress?.ToString();
    }

    private static List<NicInfo> GetServerNics(bool includeIPv6)
    {
        var list = new List<NicInfo>();
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces()
                     .Where(n => n.OperationalStatus == OperationalStatus.Up
                              && n.NetworkInterfaceType != NetworkInterfaceType.Loopback
                              && n.GetPhysicalAddress().GetAddressBytes().Length > 0))
        {
            var ipProps = nic.GetIPProperties();

            var ipv4 = ipProps.UnicastAddresses
                .Where(a => a.Address.AddressFamily == AddressFamily.InterNetwork)
                .Select(a => a.Address.ToString())
                .ToList();

            var ipv6 = includeIPv6
                ? ipProps.UnicastAddresses
                    .Where(a => a.Address.AddressFamily == AddressFamily.InterNetworkV6)
                    .Select(a => a.Address.ToString())
                    .ToList()
                : new List<string>();

            list.Add(new NicInfo(
                Name: nic.Name,
                Description: nic.Description,
                Type: nic.NetworkInterfaceType.ToString(),
                MacAddress: nic.GetPhysicalAddress().ToString(),
                IPv4: ipv4,
                IPv6: ipv6
            ));
        }
        return list;
    }

    private static string? GuessClientOs(string? ua)
    {
        if (string.IsNullOrEmpty(ua)) return null;
        ua = ua.ToLowerInvariant();
        if (ua.Contains("windows nt 10")) return "Windows 10/11";
        if (ua.Contains("windows nt 6.3")) return "Windows 8.1";
        if (ua.Contains("windows nt 6.1")) return "Windows 7";
        if (ua.Contains("mac os x")) return "macOS";
        if (ua.Contains("android")) return "Android";
        if (ua.Contains("iphone") || ua.Contains("ipad")) return "iOS";
        if (ua.Contains("linux")) return "Linux";
        return "Unknown";
    }
}

// ---- Host-level integration via IStartupFilter ----
public static class HealthProbeHostingExtensions
{
    public static IHostBuilder UseHealthProbe(
        this IHostBuilder hostBuilder,
        Action<HealthProbeHostOptions>? configure = null)
    {
        var opts = new HealthProbeHostOptions();
        configure?.Invoke(opts);

        return hostBuilder.ConfigureServices((_, services) =>
        {
            services.AddSingleton(opts);
            services.AddSingleton<IStartupFilter, HealthProbeStartupFilter>();
        });
    }
}

public sealed class HealthProbeHostOptions
{
    public string Path { get; set; } = "/health";
    public string? DetailedPath { get; set; }
    public Action<HealthProbeOptions>? Configure { get; set; }
    public Action<HealthProbeOptions>? ConfigureDetailed { get; set; }
}

internal sealed class HealthProbeStartupFilter : IStartupFilter
{
    private readonly HealthProbeHostOptions _hostOptions;
    public HealthProbeStartupFilter(HealthProbeHostOptions hostOptions) => _hostOptions = hostOptions;

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            next(app);

            // Use the BCL UseEndpoints explicitly to avoid ambiguous overloads
            Microsoft.AspNetCore.Builder.EndpointRoutingApplicationBuilderExtensions.UseEndpoints(
                app,
                endpoints =>
                {
                    if (!string.IsNullOrWhiteSpace(_hostOptions.Path))
                    {
                        endpoints.MapHealthEndpoint(_hostOptions.Path, _hostOptions.Configure);
                    }

                    if (!string.IsNullOrWhiteSpace(_hostOptions.DetailedPath))
                    {
                        endpoints.MapHealthEndpoint(_hostOptions.DetailedPath!, opts =>
                        {
                            // show more internals by default
                            opts.MaskSensitive = false;
                            opts.IncludeNetwork = true;
                            opts.IncludeIPv6 = true;
                            _hostOptions.ConfigureDetailed?.Invoke(opts);
                        });
                    }
                });
        };
    }
}

// ---- Options & DTOs ----
public sealed class HealthProbeOptions
{
    public bool MaskSensitive { get; set; } = true;
    public bool IncludeNetwork { get; set; } = false;
    public bool IncludeIPv6 { get; set; } = false;
    public string? AppName { get; set; }
    public string? Version { get; set; }
    public string? GitSha { get; set; }
}

public sealed record HealthResponse(
    DateTimeOffset TimestampUtc,
    string Environment,
    string AppName,
    string? Version,
    string? GitSha,
    RequestInfo Request,
    ClientInfo Client,
    ServerInfo Server);

public sealed record RequestInfo(
    string Scheme,
    string Host,
    string? Path,
    string? Query,
    string? TraceId,
    string? ActivityId,
    string? RequestId);

public sealed record ClientInfo(
    string? UserAgent,
    string? RemoteIpAddress,
    int RemotePort,
    string? ProxyChain,
    string? OriginatingCountry,
    string? OperatingSystemGuess,
    string? ClientMacAddress);

public sealed record ServerInfo(
    string Machine,
    string OS,
    string Framework,
    string ProcessArchitecture,
    int CpuCount,
    DateTimeOffset StartedAtUtc,
    double UptimeSeconds,
    bool IsHttps,
    string? LocalIpAddress,
    int? LocalPort,
    string? TlsProtocol,
    string? TlsCipher,
    MemoryInfo Memory,
    List<NicInfo> Network);

public sealed record MemoryInfo(
    long WorkingSetBytes,
    long GcHeapBytes,
    long? TotalAvailableMemoryBytes);

public sealed record NicInfo(
    string Name,
    string Description,
    string Type,
    string MacAddress,
    List<string> IPv4,
    List<string> IPv6);