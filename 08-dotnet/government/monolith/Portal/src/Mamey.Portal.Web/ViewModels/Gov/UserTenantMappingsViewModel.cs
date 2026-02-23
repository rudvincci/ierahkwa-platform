using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Shared.Storage.DocumentNaming;
using Mamey.Portal.Tenant.Application.Models;
using Mamey.Portal.Tenant.Application.Services;
using Mamey.Portal.Web.Auth;
using Microsoft.Extensions.Options;
using ReactiveUI;
using System.Reactive;

namespace Mamey.Portal.Web.ViewModels.Gov;

public sealed class UserTenantMappingsViewModel : ReactiveObject
{
    private readonly ITenantUserMappingService _mappings;
    private readonly ITenantOnboardingService _tenants;
    private readonly PortalAuthOptions _authOptions;

    public UserTenantMappingsViewModel(
        ITenantUserMappingService mappings,
        ITenantOnboardingService tenants,
        IOptions<PortalAuthOptions> authOptions)
    {
        _mappings = mappings;
        _tenants = tenants;
        _authOptions = authOptions.Value;

        Reload = ReactiveCommand.CreateFromTask(ReloadAsync);
        Save = ReactiveCommand.CreateFromTask(SaveAsync);
        Delete = ReactiveCommand.CreateFromTask<TenantUserMapping>(DeleteAsync);
        CreateTenant = ReactiveCommand.CreateFromTask(CreateTenantAsync);
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

    private IReadOnlyList<TenantSummary> _tenantsList = Array.Empty<TenantSummary>();
    public IReadOnlyList<TenantSummary> TenantsList
    {
        get => _tenantsList;
        private set => this.RaiseAndSetIfChanged(ref _tenantsList, value);
    }

    private IReadOnlyList<TenantUserMapping> _allMappings = Array.Empty<TenantUserMapping>();
    public IReadOnlyList<TenantUserMapping> AllMappings
    {
        get => _allMappings;
        private set => this.RaiseAndSetIfChanged(ref _allMappings, value);
    }

    private string _filter = string.Empty;
    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public IReadOnlyList<TenantUserMapping> FilteredMappings
    {
        get
        {
            var f = (Filter ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(f))
            {
                return AllMappings;
            }

            return AllMappings
                .Where(x =>
                    (x.TenantId?.Contains(f, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (x.Email?.Contains(f, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (x.Subject?.Contains(f, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (x.Issuer?.Contains(f, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }
    }

    // Form fields
    private string _issuer = string.Empty;
    public string Issuer
    {
        get => _issuer;
        set => this.RaiseAndSetIfChanged(ref _issuer, value);
    }

    private string _subject = string.Empty;
    public string Subject
    {
        get => _subject;
        set => this.RaiseAndSetIfChanged(ref _subject, value);
    }

    private string _tenantId = "default";
    public string TenantId
    {
        get => _tenantId;
        set => this.RaiseAndSetIfChanged(ref _tenantId, value);
    }

    private string? _email;
    public string? Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    // Inline tenant creation
    private string _newTenantId = string.Empty;
    public string NewTenantId
    {
        get => _newTenantId;
        set => this.RaiseAndSetIfChanged(ref _newTenantId, value);
    }

    private string _newTenantDisplayName = string.Empty;
    public string NewTenantDisplayName
    {
        get => _newTenantDisplayName;
        set => this.RaiseAndSetIfChanged(ref _newTenantDisplayName, value);
    }

    private string? _newTenantEmail;
    public string? NewTenantEmail
    {
        get => _newTenantEmail;
        set => this.RaiseAndSetIfChanged(ref _newTenantEmail, value);
    }

    public ReactiveCommand<Unit, Unit> Reload { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<TenantUserMapping, Unit> Delete { get; }
    public ReactiveCommand<Unit, Unit> CreateTenant { get; }

    public void EnsureDefaultIssuer()
    {
        if (!string.IsNullOrWhiteSpace(Issuer)) return;
        Issuer = OidcIssuerNormalizer.Normalize(_authOptions.Oidc.Authority);
    }

    public void LoadIntoForm(TenantUserMapping m)
    {
        Issuer = m.Issuer;
        Subject = m.Subject;
        TenantId = m.TenantId;
        Email = m.Email;
    }

    public void PrefillTenantFromSelection()
    {
        NewTenantId = TenantId;
        if (string.IsNullOrWhiteSpace(NewTenantDisplayName))
        {
            NewTenantDisplayName = TenantId;
        }
    }

    private async Task ReloadAsync()
    {
        Error = null;
        Busy = true;
        try
        {
            TenantsList = await _tenants.GetTenantsAsync();
            AllMappings = await _mappings.GetAllAsync(500);
            this.RaisePropertyChanged(nameof(FilteredMappings));
            EnsureDefaultIssuer();
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

    private async Task SaveAsync()
    {
        Error = null;
        Busy = true;
        try
        {
            await _mappings.UpsertAsync(Issuer, Subject, TenantId, Email);
            await ReloadAsync();
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

    private async Task DeleteAsync(TenantUserMapping m)
    {
        Error = null;
        Busy = true;
        try
        {
            await _mappings.DeleteAsync(m.Issuer, m.Subject);
            await ReloadAsync();
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
            var displayName = string.IsNullOrWhiteSpace(NewTenantDisplayName) ? NewTenantId : NewTenantDisplayName;

            var created = await _tenants.CreateTenantAsync(
                NewTenantId,
                displayName,
                branding: new TenantBranding(
                    DisplayName: displayName,
                    SealLine1: null,
                    SealLine2: null,
                    ContactEmail: string.IsNullOrWhiteSpace(NewTenantEmail) ? null : NewTenantEmail.Trim()),
                namingPattern: DocumentNamingPattern.Default);

            await ReloadAsync();
            TenantId = created.TenantId;
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
}




