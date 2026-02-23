using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Shared.Storage.Templates;
using Mamey.Portal.Citizenship.Infrastructure.Persistence;
using Mamey.Portal.Citizenship.Application.Services;
using Mamey.Portal.Tenant.Application.Models;
using Mamey.Portal.Tenant.Application.Services;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;
using System.Reactive;

namespace Mamey.Portal.Web.ViewModels.Gov;

public sealed class TenantsViewModel : ReactiveObject
{
    private readonly ITenantOnboardingService _tenantOnboarding;
    private readonly IDocumentTemplateStore _templates;
    private readonly CitizenshipDbContext _citizenshipDb;

    public TenantsViewModel(
        ITenantOnboardingService tenantOnboarding,
        IDocumentTemplateStore templates,
        CitizenshipDbContext citizenshipDb)
    {
        _tenantOnboarding = tenantOnboarding;
        _templates = templates;
        _citizenshipDb = citizenshipDb;

        Reload = ReactiveCommand.CreateFromTask(ReloadAsync);
        CreateTenant = ReactiveCommand.CreateFromTask(CreateTenantAsync);
        SaveSettings = ReactiveCommand.CreateFromTask(SaveSettingsAsync);
        LoadSelected = ReactiveCommand.CreateFromTask(LoadSelectedAsync);
        SaveVariantTemplate = ReactiveCommand.CreateFromTask(SaveVariantTemplateAsync);
        DeleteVariantTemplate = ReactiveCommand.CreateFromTask<string>(DeleteVariantTemplateAsync);
        EditVariantTemplate = ReactiveCommand.CreateFromTask<string>(EditVariantTemplateAsync);
        NewVariantFromBase = ReactiveCommand.Create<string>(NewVariantFromBaseImpl);
    }

    private bool _busy;
    public bool Busy
    {
        get => _busy;
        private set => this.RaiseAndSetIfChanged(ref _busy, value);
    }

    private string? _error;
    public string? Error
    {
        get => _error;
        private set => this.RaiseAndSetIfChanged(ref _error, value);
    }

    private IReadOnlyList<TenantSummary> _tenants = Array.Empty<TenantSummary>();
    public IReadOnlyList<TenantSummary> Tenants
    {
        get => _tenants;
        private set => this.RaiseAndSetIfChanged(ref _tenants, value);
    }

    // Create
    private string _newTenantId = string.Empty;
    public string NewTenantId
    {
        get => _newTenantId;
        set => this.RaiseAndSetIfChanged(ref _newTenantId, value);
    }

    private string _newDisplayName = string.Empty;
    public string NewDisplayName
    {
        get => _newDisplayName;
        set => this.RaiseAndSetIfChanged(ref _newDisplayName, value);
    }

    private string? _newSeal1;
    public string? NewSeal1
    {
        get => _newSeal1;
        set => this.RaiseAndSetIfChanged(ref _newSeal1, value);
    }

    private string? _newSeal2;
    public string? NewSeal2
    {
        get => _newSeal2;
        set => this.RaiseAndSetIfChanged(ref _newSeal2, value);
    }

    private string? _newEmail;
    public string? NewEmail
    {
        get => _newEmail;
        set => this.RaiseAndSetIfChanged(ref _newEmail, value);
    }

    // Edit
    private string _selectedTenantId = "default";
    public string SelectedTenantId
    {
        get => _selectedTenantId;
        set => this.RaiseAndSetIfChanged(ref _selectedTenantId, value);
    }

    private TenantSettings? _settings;
    public TenantSettings? Settings
    {
        get => _settings;
        private set => this.RaiseAndSetIfChanged(ref _settings, value);
    }

    private string _brandingDisplayName = string.Empty;
    public string BrandingDisplayName
    {
        get => _brandingDisplayName;
        set => this.RaiseAndSetIfChanged(ref _brandingDisplayName, value);
    }

    private string? _brandingSeal1;
    public string? BrandingSeal1
    {
        get => _brandingSeal1;
        set => this.RaiseAndSetIfChanged(ref _brandingSeal1, value);
    }

    private string? _brandingSeal2;
    public string? BrandingSeal2
    {
        get => _brandingSeal2;
        set => this.RaiseAndSetIfChanged(ref _brandingSeal2, value);
    }

    private string? _brandingEmail;
    public string? BrandingEmail
    {
        get => _brandingEmail;
        set => this.RaiseAndSetIfChanged(ref _brandingEmail, value);
    }

    private DocumentNamingPattern _pattern = DocumentNamingPattern.Default;
    public DocumentNamingPattern Pattern
    {
        get => _pattern;
        set => this.RaiseAndSetIfChanged(ref _pattern, value);
    }

    private bool _activationBrandingComplete;
    public bool ActivationBrandingComplete
    {
        get => _activationBrandingComplete;
        set => this.RaiseAndSetIfChanged(ref _activationBrandingComplete, value);
    }

    private bool _activationNamingComplete;
    public bool ActivationNamingComplete
    {
        get => _activationNamingComplete;
        set => this.RaiseAndSetIfChanged(ref _activationNamingComplete, value);
    }

    private bool _activationTemplatesComplete;
    public bool ActivationTemplatesComplete
    {
        get => _activationTemplatesComplete;
        set => this.RaiseAndSetIfChanged(ref _activationTemplatesComplete, value);
    }

    private bool _activationAdminMembershipEstablished;
    public bool ActivationAdminMembershipEstablished
    {
        get => _activationAdminMembershipEstablished;
        set => this.RaiseAndSetIfChanged(ref _activationAdminMembershipEstablished, value);
    }

    private bool _activationFeatureFlagsReviewed;
    public bool ActivationFeatureFlagsReviewed
    {
        get => _activationFeatureFlagsReviewed;
        set => this.RaiseAndSetIfChanged(ref _activationFeatureFlagsReviewed, value);
    }

    public string CitizenshipCertificateTemplateHtml
    {
        get => _citizenshipCertificateTemplateHtml;
        set => this.RaiseAndSetIfChanged(ref _citizenshipCertificateTemplateHtml, value);
    }
    private string _citizenshipCertificateTemplateHtml = string.Empty;

    public string BirthCertificateTemplateHtml
    {
        get => _birthCertificateTemplateHtml;
        set => this.RaiseAndSetIfChanged(ref _birthCertificateTemplateHtml, value);
    }
    private string _birthCertificateTemplateHtml = string.Empty;

    public string MarriageCertificateTemplateHtml
    {
        get => _marriageCertificateTemplateHtml;
        set => this.RaiseAndSetIfChanged(ref _marriageCertificateTemplateHtml, value);
    }
    private string _marriageCertificateTemplateHtml = string.Empty;

    public string NameChangeCertificateTemplateHtml
    {
        get => _nameChangeCertificateTemplateHtml;
        set => this.RaiseAndSetIfChanged(ref _nameChangeCertificateTemplateHtml, value);
    }
    private string _nameChangeCertificateTemplateHtml = string.Empty;

    public string PassportTemplateHtml
    {
        get => _passportTemplateHtml;
        set => this.RaiseAndSetIfChanged(ref _passportTemplateHtml, value);
    }
    private string _passportTemplateHtml = string.Empty;

    public string IdCardTemplateHtml
    {
        get => _idCardTemplateHtml;
        set => this.RaiseAndSetIfChanged(ref _idCardTemplateHtml, value);
    }
    private string _idCardTemplateHtml = string.Empty;

    public string VehicleTagTemplateHtml
    {
        get => _vehicleTagTemplateHtml;
        set => this.RaiseAndSetIfChanged(ref _vehicleTagTemplateHtml, value);
    }
    private string _vehicleTagTemplateHtml = string.Empty;

    private IReadOnlyList<DocumentTemplateSummary> _allTemplates = Array.Empty<DocumentTemplateSummary>();
    public IReadOnlyList<DocumentTemplateSummary> AllTemplates
    {
        get => _allTemplates;
        private set => this.RaiseAndSetIfChanged(ref _allTemplates, value);
    }

    public IReadOnlyList<DocumentTemplateSummary> VariantTemplates
        => AllTemplates.Where(x => !IsBaseKind(x.Kind)).ToList();

    private IReadOnlyList<string> _recentApplicationNumbers = Array.Empty<string>();
    public IReadOnlyList<string> RecentApplicationNumbers
    {
        get => _recentApplicationNumbers;
        private set => this.RaiseAndSetIfChanged(ref _recentApplicationNumbers, value);
    }

    private string _previewApplicationNumber = string.Empty;
    public string PreviewApplicationNumber
    {
        get => _previewApplicationNumber;
        set => this.RaiseAndSetIfChanged(ref _previewApplicationNumber, value);
    }

    private static bool IsBaseKind(string k)
        => string.Equals(k, "CitizenshipCertificate", StringComparison.OrdinalIgnoreCase)
           || string.Equals(k, "BirthCertificate", StringComparison.OrdinalIgnoreCase)
           || string.Equals(k, "MarriageCertificate", StringComparison.OrdinalIgnoreCase)
           || string.Equals(k, "NameChangeCertificate", StringComparison.OrdinalIgnoreCase)
           || string.Equals(k, "Passport", StringComparison.OrdinalIgnoreCase)
           || string.Equals(k, "IdCard", StringComparison.OrdinalIgnoreCase)
           || string.Equals(k, "VehicleTag", StringComparison.OrdinalIgnoreCase);

    private string _customTemplateKind = string.Empty;
    public string CustomTemplateKind
    {
        get => _customTemplateKind;
        set => this.RaiseAndSetIfChanged(ref _customTemplateKind, value);
    }

    private string _customTemplateHtml = string.Empty;
    public string CustomTemplateHtml
    {
        get => _customTemplateHtml;
        set => this.RaiseAndSetIfChanged(ref _customTemplateHtml, value);
    }

    public ReactiveCommand<Unit, Unit> Reload { get; }
    public ReactiveCommand<Unit, Unit> CreateTenant { get; }
    public ReactiveCommand<Unit, Unit> SaveSettings { get; }
    public ReactiveCommand<Unit, Unit> LoadSelected { get; }
    public ReactiveCommand<Unit, Unit> SaveVariantTemplate { get; }
    public ReactiveCommand<string, Unit> DeleteVariantTemplate { get; }
    public ReactiveCommand<string, Unit> EditVariantTemplate { get; }
    public ReactiveCommand<string, Unit> NewVariantFromBase { get; }

    public void ApplyPrefill(string? prefillTenantId)
    {
        prefillTenantId = (prefillTenantId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(prefillTenantId)) return;

        if (string.IsNullOrWhiteSpace(NewTenantId))
        {
            NewTenantId = prefillTenantId;
        }

        if (string.IsNullOrWhiteSpace(NewDisplayName))
        {
            NewDisplayName = prefillTenantId;
        }
    }

    private async Task ReloadAsync()
    {
        Error = null;
        Busy = true;
        try
        {
            Tenants = await _tenantOnboarding.GetTenantsAsync();
            if (Tenants.Count > 0 && string.IsNullOrWhiteSpace(SelectedTenantId))
            {
                SelectedTenantId = Tenants[0].TenantId;
            }

            await LoadSelectedAsync();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            Busy = false;
        }
    }

    private async Task CreateTenantAsync()
    {
        Error = null;
        Busy = true;
        try
        {
            var branding = new TenantBranding(
                DisplayName: NewDisplayName,
                SealLine1: string.IsNullOrWhiteSpace(NewSeal1) ? null : NewSeal1.Trim(),
                SealLine2: string.IsNullOrWhiteSpace(NewSeal2) ? null : NewSeal2.Trim(),
                ContactEmail: string.IsNullOrWhiteSpace(NewEmail) ? null : NewEmail.Trim());

            var created = await _tenantOnboarding.CreateTenantAsync(NewTenantId, NewDisplayName, branding, DocumentNamingPattern.Default);

            NewTenantId = string.Empty;
            NewDisplayName = string.Empty;
            NewSeal1 = null;
            NewSeal2 = null;
            NewEmail = null;

            Tenants = await _tenantOnboarding.GetTenantsAsync();
            SelectedTenantId = created.TenantId;
            await LoadSelectedAsync();
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            Busy = false;
        }
    }

    private async Task LoadSelectedAsync()
    {
        Error = null;
        try
        {
            var tenantId = SelectedTenantId;
            Settings = await _tenantOnboarding.GetSettingsAsync(tenantId);
            if (Settings is not null)
            {
                BrandingDisplayName = Settings.Branding.DisplayName ?? string.Empty;
                BrandingSeal1 = Settings.Branding.SealLine1;
                BrandingSeal2 = Settings.Branding.SealLine2;
                BrandingEmail = Settings.Branding.ContactEmail;
                Pattern = Settings.DocumentNaming;
                ActivationBrandingComplete = Settings.ActivationChecklist.BrandingComplete;
                ActivationNamingComplete = Settings.ActivationChecklist.NamingComplete;
                ActivationTemplatesComplete = Settings.ActivationChecklist.TemplatesComplete;
                ActivationAdminMembershipEstablished = Settings.ActivationChecklist.AdminMembershipEstablished;
                ActivationFeatureFlagsReviewed = Settings.ActivationChecklist.FeatureFlagsReviewed;
            }
            else
            {
                BrandingDisplayName = tenantId;
                BrandingSeal1 = null;
                BrandingSeal2 = null;
                BrandingEmail = null;
                Pattern = DocumentNamingPattern.Default;
                ActivationBrandingComplete = false;
                ActivationNamingComplete = false;
                ActivationTemplatesComplete = false;
                ActivationAdminMembershipEstablished = false;
                ActivationFeatureFlagsReviewed = false;
            }

            CitizenshipCertificateTemplateHtml = (await _templates.GetTemplateAsync(tenantId, "CitizenshipCertificate")) ?? string.Empty;
            BirthCertificateTemplateHtml = (await _templates.GetTemplateAsync(tenantId, "BirthCertificate")) ?? string.Empty;
            MarriageCertificateTemplateHtml = (await _templates.GetTemplateAsync(tenantId, "MarriageCertificate")) ?? string.Empty;
            NameChangeCertificateTemplateHtml = (await _templates.GetTemplateAsync(tenantId, "NameChangeCertificate")) ?? string.Empty;
            PassportTemplateHtml = (await _templates.GetTemplateAsync(tenantId, "Passport")) ?? string.Empty;
            IdCardTemplateHtml = (await _templates.GetTemplateAsync(tenantId, "IdCard")) ?? string.Empty;
            VehicleTagTemplateHtml = (await _templates.GetTemplateAsync(tenantId, "VehicleTag")) ?? string.Empty;
            AllTemplates = await _templates.ListTemplatesAsync(tenantId, 200);

            try
            {
                RecentApplicationNumbers = await _citizenshipDb.Applications.AsNoTracking()
                    .Where(a => a.TenantId == tenantId)
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(a => a.ApplicationNumber)
                    .Take(25)
                    .ToListAsync();
            }
            catch
            {
                RecentApplicationNumbers = Array.Empty<string>();
            }

            // Ensure VariantTemplates updates in UI
            this.RaisePropertyChanged(nameof(VariantTemplates));
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
    }

    private async Task SaveSettingsAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedTenantId))
        {
            return;
        }

        Error = null;
        Busy = true;
        try
        {
            var tenantId = SelectedTenantId;
            var branding = new TenantBranding(
                DisplayName: BrandingDisplayName,
                SealLine1: string.IsNullOrWhiteSpace(BrandingSeal1) ? null : BrandingSeal1.Trim(),
                SealLine2: string.IsNullOrWhiteSpace(BrandingSeal2) ? null : BrandingSeal2.Trim(),
                ContactEmail: string.IsNullOrWhiteSpace(BrandingEmail) ? null : BrandingEmail.Trim());

            var activation = new TenantActivationChecklist(
                ActivationBrandingComplete,
                ActivationNamingComplete,
                ActivationTemplatesComplete,
                ActivationAdminMembershipEstablished,
                ActivationFeatureFlagsReviewed);

            Settings = await _tenantOnboarding.UpdateSettingsAsync(tenantId, branding, Pattern, activation);

            // Templates are saved separately. Empty clears override.
            await UpsertOrDelete(tenantId, "CitizenshipCertificate", CitizenshipCertificateTemplateHtml);
            await UpsertOrDelete(tenantId, "BirthCertificate", BirthCertificateTemplateHtml);
            await UpsertOrDelete(tenantId, "MarriageCertificate", MarriageCertificateTemplateHtml);
            await UpsertOrDelete(tenantId, "NameChangeCertificate", NameChangeCertificateTemplateHtml);
            await UpsertOrDelete(tenantId, "Passport", PassportTemplateHtml);
            await UpsertOrDelete(tenantId, "IdCard", IdCardTemplateHtml);
            await UpsertOrDelete(tenantId, "VehicleTag", VehicleTagTemplateHtml);

            AllTemplates = await _templates.ListTemplatesAsync(tenantId, 200);
            this.RaisePropertyChanged(nameof(VariantTemplates));
        }
        catch (Exception ex)
        {
            Error = ex.Message;
        }
        finally
        {
            Busy = false;
        }
    }

    private async Task UpsertOrDelete(string tenantId, string kind, string html)
    {
        if (string.IsNullOrWhiteSpace(html))
        {
            await _templates.DeleteTemplateAsync(tenantId, kind);
        }
        else
        {
            await _templates.UpsertTemplateAsync(tenantId, kind, html);
        }
    }

    private void NewVariantFromBaseImpl(string baseKind)
    {
        baseKind = (baseKind ?? string.Empty).Trim();
        var suggestedVariant = baseKind.Equals("IdCard", StringComparison.OrdinalIgnoreCase) ? "MedicinalCannabis" : "Standard";
        CustomTemplateKind = $"{baseKind}:{suggestedVariant}";

        CustomTemplateHtml = baseKind switch
        {
            "IdCard" => IdCardTemplateHtml,
            "VehicleTag" => VehicleTagTemplateHtml,
            _ => string.Empty
        };
    }

    private async Task EditVariantTemplateAsync(string kind)
    {
        kind = (kind ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(kind)) return;

        CustomTemplateKind = kind;
        CustomTemplateHtml = (await _templates.GetTemplateAsync(SelectedTenantId, kind)) ?? string.Empty;
    }

    private async Task SaveVariantTemplateAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedTenantId))
        {
            return;
        }

        var kind = (CustomTemplateKind ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(kind))
        {
            Error = "Kind is required (e.g. IdCard:MedicinalCannabis).";
            return;
        }
        if (!kind.Contains(':', StringComparison.Ordinal))
        {
            Error = "Custom templates should be variant kinds (must include ':'). Example: IdCard:MedicinalCannabis";
            return;
        }

        await _templates.UpsertTemplateAsync(SelectedTenantId, kind, CustomTemplateHtml ?? string.Empty);
        AllTemplates = await _templates.ListTemplatesAsync(SelectedTenantId, 200);
        this.RaisePropertyChanged(nameof(VariantTemplates));
    }

    private async Task DeleteVariantTemplateAsync(string kind)
    {
        kind = (kind ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(kind)) return;

        await _templates.DeleteTemplateAsync(SelectedTenantId, kind);
        AllTemplates = await _templates.ListTemplatesAsync(SelectedTenantId, 200);
        this.RaisePropertyChanged(nameof(VariantTemplates));

        if (string.Equals(CustomTemplateKind, kind, StringComparison.OrdinalIgnoreCase))
        {
            CustomTemplateKind = string.Empty;
            CustomTemplateHtml = string.Empty;
        }
    }

    public async Task<string> BuildPreviewHtmlAsync(string kind, string templateHtml)
    {
        // Uses persisted application data (if available) for the currently selected tenant.
        // Falls back to synthetic values if no application is selected/found.
        var tenantId = SelectedTenantId;

        var appNo = (PreviewApplicationNumber ?? string.Empty).Trim();
        string fullName = "Jane Doe";
        DateOnly dob = new DateOnly(1990, 01, 05);

        if (!string.IsNullOrWhiteSpace(appNo))
        {
            try
            {
                var app = await _citizenshipDb.Applications.AsNoTracking()
                    .Where(a => a.TenantId == tenantId && a.ApplicationNumber == appNo)
                    .Select(a => new { a.Id, a.ApplicationNumber, a.FirstName, a.LastName, a.DateOfBirth })
                    .SingleOrDefaultAsync();

                if (app is not null)
                {
                    appNo = app.ApplicationNumber;
                    fullName = $"{app.FirstName} {app.LastName}";
                    dob = app.DateOfBirth;
                }
            }
            catch
            {
                // ignore; use synthetic
            }
        }

        var now = DateTimeOffset.UtcNow;

        var variant = string.Empty;
        if (kind.StartsWith("IdCard:", StringComparison.OrdinalIgnoreCase))
        {
            variant = kind["IdCard:".Length..].Trim();
        }
        else if (kind.StartsWith("VehicleTag:", StringComparison.OrdinalIgnoreCase))
        {
            variant = kind["VehicleTag:".Length..].Trim();
        }

        // Use a stable application id for preview numbers if we didn't load a real app
        var previewAppId = Guid.Parse("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee");
        var docNo = kind switch
        {
            "BirthCertificate" => $"BIRTH-{tenantId.ToUpperInvariant()}-{now:yyMMdd}-ABC123",
            "MarriageCertificate" => $"MARR-{tenantId.ToUpperInvariant()}-{now:yyMMdd}-ABC123",
            "NameChangeCertificate" => $"NAME-{tenantId.ToUpperInvariant()}-{now:yyMMdd}-ABC123",
            "Passport" => IssuedDocumentNumberGenerator.Passport(tenantId, now, previewAppId),
            "IdCard" => IssuedDocumentNumberGenerator.IdCard(tenantId, now, previewAppId, variant),
            "VehicleTag" => IssuedDocumentNumberGenerator.VehicleTag(tenantId, now, previewAppId, variant),
            _ when kind.StartsWith("IdCard:", StringComparison.OrdinalIgnoreCase)
                => IssuedDocumentNumberGenerator.IdCard(tenantId, now, previewAppId, variant),
            _ when kind.StartsWith("VehicleTag:", StringComparison.OrdinalIgnoreCase)
                => IssuedDocumentNumberGenerator.VehicleTag(tenantId, now, previewAppId, variant),
            _ => string.Empty
        };

        var expiresAt = kind.StartsWith("VehicleTag", StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : now.AddYears(5).ToString("yyyy-MM-dd");

        var tokens = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["TenantId"] = tenantId,
            ["Kind"] = kind,
            ["Variant"] = string.IsNullOrWhiteSpace(variant) ? "" : variant,
            ["ApplicationNumber"] = string.IsNullOrWhiteSpace(appNo) ? "APP-YYYYMMDD-XXXXXX" : appNo,
            ["FullName"] = fullName,
            ["DateOfBirth"] = dob.ToString("yyyy-MM-dd"),
            ["IssuedAt"] = now.ToString("yyyy-MM-dd"),
            ["DocumentNumber"] = docNo,
            ["ExpiresAt"] = expiresAt,
        };

        return TemplateTokenRenderer.Apply(templateHtml, tokens);
    }

    public async Task<string> LoadTemplateHtmlAsync(string kind)
    {
        kind = (kind ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(kind))
        {
            return string.Empty;
        }

        var tenantId = SelectedTenantId;
        return (await _templates.GetTemplateAsync(tenantId, kind)) ?? string.Empty;
    }
}
