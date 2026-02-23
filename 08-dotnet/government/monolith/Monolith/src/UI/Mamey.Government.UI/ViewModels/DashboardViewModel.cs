using System.Reactive.Subjects;
using ReactiveUI;

namespace Mamey.Government.UI.ViewModels;

public class DashboardViewModel : ReactiveObject
{
    private int _totalCitizens;
    private int _totalApplications;
    private int _pendingApplications;
    private int _totalPassports;
    private int _totalTravelIds;
    private int _totalCertificates;
    private bool _isLoading;

    public int TotalCitizens
    {
        get => _totalCitizens;
        set => this.RaiseAndSetIfChanged(ref _totalCitizens, value);
    }

    public int TotalApplications
    {
        get => _totalApplications;
        set => this.RaiseAndSetIfChanged(ref _totalApplications, value);
    }

    public int PendingApplications
    {
        get => _pendingApplications;
        set => this.RaiseAndSetIfChanged(ref _pendingApplications, value);
    }

    public int TotalPassports
    {
        get => _totalPassports;
        set => this.RaiseAndSetIfChanged(ref _totalPassports, value);
    }

    public int TotalTravelIds
    {
        get => _totalTravelIds;
        set => this.RaiseAndSetIfChanged(ref _totalTravelIds, value);
    }

    public int TotalCertificates
    {
        get => _totalCertificates;
        set => this.RaiseAndSetIfChanged(ref _totalCertificates, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }
}
