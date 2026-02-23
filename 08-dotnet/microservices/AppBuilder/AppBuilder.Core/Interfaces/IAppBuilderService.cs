namespace AppBuilder.Core.Interfaces;

using AppBuilder.Core.Models;

/// <summary>
/// AppBuilder Service Interface - IERAHKWA
/// Appy-style: Enter URL, Customize Design, Build App, Download & Publish
/// </summary>
public interface IAppBuilderService
{
    // Projects
    IReadOnlyList<AppProject> GetAllProjects();
    IReadOnlyList<AppProject> GetProjectsByUser(string? userId);
    AppProject? GetProjectById(string id);
    AppProject CreateProject(AppProject project);
    AppProject? UpdateProject(string id, AppProject project);
    bool DeleteProject(string id);

    // Builds
    IReadOnlyList<AppBuild> GetBuildsByProject(string projectId);
    IReadOnlyList<AppBuild> GetAllBuilds();
    AppBuild? GetBuildById(string id);
    AppBuild CreateBuild(BuildRequest request);
    AppBuild? UpdateBuildStatus(string id, BuildStatus status, string? errorMessage = null, string? downloadUrl = null);

    // Config generation (for Capacitor/Tauri/PWA)
    string GenerateCapacitorConfig(AppProject project);
    string GeneratePwaManifest(AppProject project);
}
