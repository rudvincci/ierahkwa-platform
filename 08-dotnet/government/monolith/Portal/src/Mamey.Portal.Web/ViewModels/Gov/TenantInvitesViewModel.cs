using Mamey.Portal.Shared.Auth;
using Mamey.Portal.Tenant.Application.Models;
using Mamey.Portal.Tenant.Application.Services;
using Mamey.Portal.Web.Auth;
using Microsoft.Extensions.Options;
using ReactiveUI;
using System.Reactive;

namespace Mamey.Portal.Web.ViewModels.Gov;

public sealed class TenantInvitesViewModel : ReactiveObject
{
    private readonly ITenantUserInviteService _invites;
    private readonly ITenantOnboardingService _tenants;
    private readonly PortalAuthOptions _authOptions;

    public TenantInvitesViewModel(
        ITenantUserInviteService invites,
        ITenantOnboardingService tenants,
        IOptions<PortalAuthOptions> authOptions)
    {
        _invites = invites;
        _tenants = tenants;
        _authOptions = authOptions.Value;

        Reload = ReactiveCommand.CreateFromTask(ReloadAsync);
        Save = ReactiveCommand.CreateFromTask(SaveAsync);
        Delete = ReactiveCommand.CreateFromTask<TenantUserInvite>(DeleteAsync);
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

    private IReadOnlyList<TenantUserInvite> _allInvites = Array.Empty<TenantUserInvite>();
    public IReadOnlyList<TenantUserInvite> AllInvites
    {
        get => _allInvites;
        private set => this.RaiseAndSetIfChanged(ref _allInvites, value);
    }

    private string _filter = string.Empty;
    public string Filter
    {
        get => _filter;
        set => this.RaiseAndSetIfChanged(ref _filter, value);
    }

    public IReadOnlyList<TenantUserInvite> FilteredInvites
    {
        get
        {
            var f = (Filter ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(f))
            {
                return AllInvites;
            }

            return AllInvites
                .Where(x =>
                    x.Email.Contains(f, StringComparison.OrdinalIgnoreCase)
                    || x.Issuer.Contains(f, StringComparison.OrdinalIgnoreCase)
                    || x.TenantId.Contains(f, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    private string _issuer = string.Empty;
    public string Issuer
    {
        get => _issuer;
        set => this.RaiseAndSetIfChanged(ref _issuer, value);
    }

    private string _email = string.Empty;
    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    private string _tenantId = "default";
    public string TenantId
    {
        get => _tenantId;
        set => this.RaiseAndSetIfChanged(ref _tenantId, value);
    }

    public ReactiveCommand<Unit, Unit> Reload { get; }
    public ReactiveCommand<Unit, Unit> Save { get; }
    public ReactiveCommand<TenantUserInvite, Unit> Delete { get; }

    public void EnsureDefaultIssuer()
    {
        if (!string.IsNullOrWhiteSpace(Issuer)) return;
        Issuer = OidcIssuerNormalizer.Normalize(_authOptions.Oidc.Authority);
    }

    public void LoadIntoForm(TenantUserInvite invite)
    {
        Issuer = invite.Issuer;
        Email = invite.Email;
        TenantId = invite.TenantId;
    }

    private async Task ReloadAsync()
    {
        Error = null;
        Busy = true;
        try
        {
            TenantsList = await _tenants.GetTenantsAsync();
            if (TenantsList.Count > 0 && string.IsNullOrWhiteSpace(TenantId))
            {
                TenantId = TenantsList[0].TenantId;
            }

            AllInvites = string.IsNullOrWhiteSpace(TenantId)
                ? Array.Empty<TenantUserInvite>()
                : await _invites.GetByTenantAsync(TenantId);

            this.RaisePropertyChanged(nameof(FilteredInvites));
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
            await _invites.CreateOrUpdateAsync(Issuer, Email, TenantId);
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

    private async Task DeleteAsync(TenantUserInvite invite)
    {
        Error = null;
        Busy = true;
        try
        {
            await _invites.RevokeAsync(invite.Issuer, invite.Email);
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
}
