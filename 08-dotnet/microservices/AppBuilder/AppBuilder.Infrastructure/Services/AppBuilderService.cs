using System.Text.Json;
using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.Extensions.Logging;

namespace AppBuilder.Infrastructure.Services;

/// <summary>
/// AppBuilder Service - IERAHKWA Platform
/// Implements Appy-style: URL → Design → Build → Download. Multi-platform, version management, build tracking.
/// </summary>
public class AppBuilderService : IAppBuilderService
{
    private static readonly List<AppProject> _projects = new();
    private static readonly List<AppBuild> _builds = new();
    private static readonly object _lock = new();
    private readonly ILogger<AppBuilderService> _logger;

    public AppBuilderService(ILogger<AppBuilderService> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<AppProject> GetAllProjects()
    {
        lock (_lock)
            return _projects.OrderByDescending(p => p.UpdatedAt).ToList();
    }

    public IReadOnlyList<AppProject> GetProjectsByUser(string? userId)
    {
        lock (_lock)
        {
            if (string.IsNullOrEmpty(userId)) return _projects.OrderByDescending(p => p.UpdatedAt).ToList();
            return _projects.Where(p => p.CreatedBy == userId).OrderByDescending(p => p.UpdatedAt).ToList();
        }
    }

    public AppProject? GetProjectById(string id)
    {
        lock (_lock)
            return _projects.FirstOrDefault(p => p.Id == id);
    }

    public AppProject CreateProject(AppProject project)
    {
        lock (_lock)
        {
            project.Id = Guid.NewGuid().ToString();
            project.CreatedAt = DateTime.UtcNow;
            project.UpdatedAt = DateTime.UtcNow;
            _projects.Add(project);
            _logger.LogInformation("IERAHKWA AppBuilder: Created project {Id} - {Name}", project.Id, project.Name);
            return project;
        }
    }

    public AppProject? UpdateProject(string id, AppProject project)
    {
        lock (_lock)
        {
            var existing = _projects.FirstOrDefault(p => p.Id == id);
            if (existing == null) return null;
            project.Id = id;
            project.CreatedAt = existing.CreatedAt;
            project.UpdatedAt = DateTime.UtcNow;
            _projects.Remove(existing);
            _projects.Add(project);
            _logger.LogInformation("IERAHKWA AppBuilder: Updated project {Id}", id);
            return project;
        }
    }

    public bool DeleteProject(string id)
    {
        lock (_lock)
        {
            var p = _projects.FirstOrDefault(x => x.Id == id);
            if (p == null) return false;
            _projects.Remove(p);
            var toRemove = _builds.Where(b => b.AppProjectId == id).ToList();
            foreach (var b in toRemove) _builds.Remove(b);
            _logger.LogInformation("IERAHKWA AppBuilder: Deleted project {Id}", id);
            return true;
        }
    }

    public IReadOnlyList<AppBuild> GetBuildsByProject(string projectId)
    {
        lock (_lock)
            return _builds.Where(b => b.AppProjectId == projectId).OrderByDescending(b => b.CreatedAt).ToList();
    }

    public IReadOnlyList<AppBuild> GetAllBuilds()
    {
        lock (_lock)
            return _builds.OrderByDescending(b => b.CreatedAt).ToList();
    }

    public AppBuild? GetBuildById(string id)
    {
        lock (_lock)
            return _builds.FirstOrDefault(b => b.Id == id);
    }

    public AppBuild CreateBuild(BuildRequest request)
    {
        lock (_lock)
        {
            var project = _projects.FirstOrDefault(p => p.Id == request.AppProjectId);
            if (project == null)
                throw new ArgumentException($"Project {request.AppProjectId} not found");

            var build = new AppBuild
            {
                Id = Guid.NewGuid().ToString(),
                AppProjectId = request.AppProjectId,
                Platform = request.Platform,
                Version = request.Version ?? "1.0.0",
                VersionCode = request.VersionCode,
                Status = BuildStatus.Pending,
                StartedAt = null,
                CompletedAt = null,
                CreatedAt = DateTime.UtcNow
            };

            _builds.Add(build);
            _logger.LogInformation("IERAHKWA AppBuilder: Created build {Id} for project {Project} [{Platform}]", build.Id, project.Name, request.Platform);

            // Simulate async build: in production would trigger Capacitor/Tauri/CI
            _ = SimulateBuildAsync(build, project);

            return build;
        }
    }

    public AppBuild? UpdateBuildStatus(string id, BuildStatus status, string? errorMessage = null, string? downloadUrl = null)
    {
        lock (_lock)
        {
            var b = _builds.FirstOrDefault(x => x.Id == id);
            if (b == null) return null;
            b.Status = status;
            b.ErrorMessage = errorMessage;
            b.DownloadUrl = downloadUrl;
            if (status == BuildStatus.Building && !b.StartedAt.HasValue)
                b.StartedAt = DateTime.UtcNow;
            if (status is BuildStatus.Success or BuildStatus.Failed or BuildStatus.Cancelled)
            {
                b.CompletedAt = DateTime.UtcNow;
                if (b.StartedAt.HasValue)
                    b.DurationSeconds = (int)(b.CompletedAt.Value - b.StartedAt.Value).TotalSeconds;
            }
            return b;
        }
    }

    public string GenerateCapacitorConfig(AppProject project)
    {
        var appId = $"com.ierahkwa.{SanitizeId(project.Name)}";
        var cfg = new
        {
            appId,
            appName = project.Name,
            webDir = "www",
            server = new { url = project.SourceUrl, cleartext = true },
            plugins = new
            {
                SplashScreen = new
                {
                    showSpinner = false,
                    backgroundColor = project.Design.SplashBackgroundColor ?? project.Design.BackgroundColor,
                    launchShowDuration = 2000
                },
                PushNotifications = project.PushNotificationsEnabled ? new { } : (object?)null
            }
        };
        return JsonSerializer.Serialize(cfg, new JsonSerializerOptions { WriteIndented = true });
    }

    public string GeneratePwaManifest(AppProject project)
    {
        var d = project.Design;
        var manifest = new
        {
            name = project.Name,
            short_name = project.Name.Length > 12 ? project.Name[..12] : project.Name,
            description = project.Description ?? $"IERAHKWA App - {project.Name}",
            start_url = project.SourceUrl,
            display = "standalone",
            background_color = d.BackgroundColor,
            theme_color = d.PrimaryColor,
            orientation = "any",
            icons = new[]
            {
                new { src = d.AppIconUrl ?? "/icon-192.png", sizes = "192x192", type = "image/png" },
                new { src = d.AppIconUrl ?? "/icon-512.png", sizes = "512x512", type = "image/png" }
            }
        };
        return JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string SanitizeId(string name)
    {
        return new string(name.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToLowerInvariant();
    }

    /// <summary>Simulates build process. In production: run Capacitor/Tauri/CI and set DownloadUrl.</summary>
    private async Task SimulateBuildAsync(AppBuild build, AppProject project)
    {
        await Task.Delay(500);
        UpdateBuildStatus(build.Id, BuildStatus.Building);
        await Task.Delay(2000 + Random.Shared.Next(2000));

        // Simulate ~80% success
        if (Random.Shared.Next(100) < 80)
        {
            UpdateBuildStatus(build.Id, BuildStatus.Success,
                downloadUrl: $"/api/builds/{build.Id}/download?platform={build.Platform}");
        }
        else
        {
            UpdateBuildStatus(build.Id, BuildStatus.Failed, errorMessage: "Simulated build failure (demo). Configure Capacitor/Tauri for real builds.");
        }
    }
}
