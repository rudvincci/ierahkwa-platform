using System.Text.Json;
using System.Text.RegularExpressions;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Tenant.Application.Models;

namespace Mamey.Portal.Tenant.Application.Services;

public sealed class TenantOnboardingService : ITenantOnboardingService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private static readonly Regex InvalidTenantChars = new(@"[^a-z0-9\-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly ITenantOnboardingStore _store;

    public TenantOnboardingService(ITenantOnboardingStore store)
    {
        _store = store;
    }

    public Task<IReadOnlyList<TenantSummary>> GetTenantsAsync(CancellationToken ct = default)
        => _store.GetTenantsAsync(500, ct);

    public async Task<TenantSettings?> GetSettingsAsync(string tenantId, CancellationToken ct = default)
    {
        tenantId = NormalizeTenantId(tenantId);
        if (string.IsNullOrWhiteSpace(tenantId)) return null;

        var tenant = await _store.GetTenantAsync(tenantId, ct);
        if (tenant is null) return null;

        var settings = await _store.GetSettingsAsync(tenantId, ct);
        var naming = await _store.GetNamingAsync(tenantId, ct);

        var branding = Deserialize(settings?.BrandingJson, () => new TenantBranding(
            DisplayName: tenant.DisplayName,
            SealLine1: null,
            SealLine2: null,
            ContactEmail: null));

        // Keep DisplayName in sync (the tenant row is source of truth)
        branding = branding with { DisplayName = tenant.DisplayName };

        var pattern = Deserialize(naming?.PatternJson, () => DocumentNamingPattern.Default);
        var activation = Deserialize(settings?.ActivationJson, () => TenantActivationChecklist.Empty);

        return new TenantSettings(tenantId, branding, pattern, activation);
    }

    public async Task<TenantSettings> CreateTenantAsync(
        string tenantId,
        string displayName,
        TenantBranding? branding = null,
        DocumentNamingPattern? namingPattern = null,
        TenantActivationChecklist? activationChecklist = null,
        CancellationToken ct = default)
    {
        tenantId = NormalizeTenantId(tenantId);
        displayName = (displayName ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException("tenantId is required.");
        }
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("displayName is required.");
        }

        var exists = await _store.TenantExistsAsync(tenantId, ct);
        if (exists)
        {
            throw new InvalidOperationException($"Tenant '{tenantId}' already exists.");
        }

        var now = DateTimeOffset.UtcNow;
        var resolvedBranding = (branding ?? new TenantBranding(displayName, null, null, null)) with { DisplayName = displayName };
        var resolvedPattern = namingPattern ?? DocumentNamingPattern.Default;
        var resolvedActivation = activationChecklist ?? TenantActivationChecklist.Empty;

        await _store.CreateTenantAsync(
            tenantId,
            displayName,
            JsonSerializer.Serialize(resolvedBranding, JsonOptions),
            JsonSerializer.Serialize(resolvedActivation, JsonOptions),
            JsonSerializer.Serialize(resolvedPattern, JsonOptions),
            now,
            ct);

        return new TenantSettings(tenantId, resolvedBranding, resolvedPattern, resolvedActivation);
    }

    public async Task<TenantSettings> UpdateSettingsAsync(
        string tenantId,
        TenantBranding branding,
        DocumentNamingPattern namingPattern,
        TenantActivationChecklist? activationChecklist = null,
        CancellationToken ct = default)
    {
        tenantId = NormalizeTenantId(tenantId);
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new ArgumentException("tenantId is required.");
        }

        var tenant = await _store.GetTenantAsync(tenantId, ct)
                     ?? throw new InvalidOperationException("Tenant not found.");

        var displayName = (branding.DisplayName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Branding.DisplayName is required.");
        }

        var now = DateTimeOffset.UtcNow;
        var resolvedBranding = branding with { DisplayName = displayName };
        var resolvedPattern = namingPattern ?? DocumentNamingPattern.Default;
        var existingActivation = Deserialize((await _store.GetSettingsAsync(tenantId, ct))?.ActivationJson, () => TenantActivationChecklist.Empty);
        var resolvedActivation = activationChecklist ?? existingActivation;

        await _store.UpdateTenantAsync(
            tenantId,
            displayName,
            JsonSerializer.Serialize(resolvedBranding, JsonOptions),
            JsonSerializer.Serialize(resolvedActivation, JsonOptions),
            JsonSerializer.Serialize(resolvedPattern, JsonOptions),
            now,
            ct);

        return new TenantSettings(
            tenantId,
            resolvedBranding,
            resolvedPattern,
            resolvedActivation);
    }

    private static string NormalizeTenantId(string tenantId)
    {
        tenantId = (tenantId ?? string.Empty).Trim().ToLowerInvariant();
        tenantId = tenantId.Replace(' ', '-');
        tenantId = InvalidTenantChars.Replace(tenantId, "");
        tenantId = tenantId.Trim('-');
        return tenantId.Length > 128 ? tenantId[..128] : tenantId;
    }

    private static T Deserialize<T>(string? json, Func<T> fallback)
    {
        if (string.IsNullOrWhiteSpace(json)) return fallback();
        try
        {
            return JsonSerializer.Deserialize<T>(json, JsonOptions) ?? fallback();
        }
        catch
        {
            return fallback();
        }
    }
}
