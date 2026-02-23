using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using Mamey.Portal.Citizenship.Application.Models;
using Mamey.Portal.Citizenship.Application.Services;

namespace Mamey.Portal.Web.ViewModels.Citizenship;

public sealed class ApplicationStatusViewModel : ReactiveObject
{
    private readonly IApplicationStatusService _statusService;

    private string _applicationNumber = string.Empty;
    public string ApplicationNumber
    {
        get => _applicationNumber;
        set => this.RaiseAndSetIfChanged(ref _applicationNumber, value);
    }

    private ApplicationStatusDto? _status;
    public ApplicationStatusDto? Status
    {
        get => _status;
        private set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    private string _message = string.Empty;
    public string Message
    {
        get => _message;
        private set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        private set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public bool CanLookup => !string.IsNullOrWhiteSpace(ApplicationNumber);

    public ReactiveCommand<Unit, Unit> Lookup { get; }

    public ApplicationStatusViewModel(IApplicationStatusService statusService)
    {
        _statusService = statusService;
        var canLookup = this.WhenAnyValue(x => x.ApplicationNumber, n => !string.IsNullOrWhiteSpace(n));

        Lookup = ReactiveCommand.CreateFromTask(async ct =>
        {
            try
            {
                IsLoading = true;
                Status = null;
                Message = string.Empty;

                var result = await _statusService.GetApplicationStatusByNumberAsync(ApplicationNumber, ct);
                
                if (result is null)
                {
                    Message = "Application not found. Please check your application number.";
                    Status = null;
                    return;
                }

                Status = result;
                Message = $"Application {result.ApplicationNumber} - Status: {result.Status}";
            }
            catch (Exception ex)
            {
                Status = null;
                Message = $"Error: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }, canLookup);
    }

    public async Task LoadByApplicationNumberAsync(string applicationNumber, CancellationToken ct = default)
    {
        ApplicationNumber = applicationNumber;
        await Lookup.Execute().ToTask(ct);
    }

    public async Task LoadByEmailAsync(string email, CancellationToken ct = default)
    {
        try
        {
            IsLoading = true;
            Status = null;
            Message = string.Empty;

            var result = await _statusService.GetApplicationStatusByEmailAsync(email, ct);
            
            if (result is null)
            {
                Message = "No application found for your email address.";
                Status = null;
                return;
            }

            Status = result;
            ApplicationNumber = result.ApplicationNumber;
            Message = $"Application {result.ApplicationNumber} - Status: {result.Status}";
        }
        catch (Exception ex)
        {
            Status = null;
            Message = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void Reset()
    {
        ApplicationNumber = string.Empty;
        Status = null;
        Message = string.Empty;
    }
}

