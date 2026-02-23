using System.Collections.ObjectModel;
using Mamey.Government.UI.Models;
using ReactiveUI;

namespace Mamey.Government.UI.ViewModels;

/// <summary>
/// ViewModel for managing citizenship applications list view.
/// Provides reactive properties for filtering, pagination, and status tracking.
/// </summary>
public class ApplicationsViewModel : ReactiveObject
{
    private bool _isLoading;
    private string? _searchTerm;
    private string? _selectedStatus;
    private string? _selectedStep;
    private int _currentPage = 1;
    private int _pageSize = 20;
    private long _totalCount;
    private ObservableCollection<ApplicationSummaryModel> _applications = new();

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }

    public string? SearchTerm
    {
        get => _searchTerm;
        set => this.RaiseAndSetIfChanged(ref _searchTerm, value);
    }

    public string? SelectedStatus
    {
        get => _selectedStatus;
        set => this.RaiseAndSetIfChanged(ref _selectedStatus, value);
    }

    public string? SelectedStep
    {
        get => _selectedStep;
        set => this.RaiseAndSetIfChanged(ref _selectedStep, value);
    }

    public int CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    public int PageSize
    {
        get => _pageSize;
        set => this.RaiseAndSetIfChanged(ref _pageSize, value);
    }

    public long TotalCount
    {
        get => _totalCount;
        set => this.RaiseAndSetIfChanged(ref _totalCount, value);
    }

    public ObservableCollection<ApplicationSummaryModel> Applications
    {
        get => _applications;
        set => this.RaiseAndSetIfChanged(ref _applications, value);
    }

    // Status counts for filter cards
    private int _draftCount;
    private int _submittedCount;
    private int _reviewPendingCount;
    private int _approvedCount;
    private int _rejectedCount;
    private int _kycPendingCount;
    private int _validatingCount;

    public int DraftCount
    {
        get => _draftCount;
        set => this.RaiseAndSetIfChanged(ref _draftCount, value);
    }

    public int SubmittedCount
    {
        get => _submittedCount;
        set => this.RaiseAndSetIfChanged(ref _submittedCount, value);
    }

    public int ReviewPendingCount
    {
        get => _reviewPendingCount;
        set => this.RaiseAndSetIfChanged(ref _reviewPendingCount, value);
    }

    public int ApprovedCount
    {
        get => _approvedCount;
        set => this.RaiseAndSetIfChanged(ref _approvedCount, value);
    }

    public int RejectedCount
    {
        get => _rejectedCount;
        set => this.RaiseAndSetIfChanged(ref _rejectedCount, value);
    }

    public int KycPendingCount
    {
        get => _kycPendingCount;
        set => this.RaiseAndSetIfChanged(ref _kycPendingCount, value);
    }

    public int ValidatingCount
    {
        get => _validatingCount;
        set => this.RaiseAndSetIfChanged(ref _validatingCount, value);
    }

    // Computed properties and helper methods
    /// <summary>
    /// Indicates whether any filters are currently active.
    /// </summary>
    public bool HasFilters => !string.IsNullOrWhiteSpace(SearchTerm) || 
                              !string.IsNullOrWhiteSpace(SelectedStatus) || 
                              !string.IsNullOrWhiteSpace(SelectedStep);

    /// <summary>
    /// Total count of all status categories combined.
    /// </summary>
    public int TotalStatusCount => DraftCount + SubmittedCount + ReviewPendingCount + 
                                   ApprovedCount + RejectedCount + KycPendingCount + ValidatingCount;

    /// <summary>
    /// Clears all active filters and resets to default state.
    /// </summary>
    public void ClearFilters()
    {
        SearchTerm = null;
        SelectedStatus = null;
        SelectedStep = null;
        CurrentPage = 1;
    }

    /// <summary>
    /// Resets all status counts to zero.
    /// </summary>
    public void ResetStatusCounts()
    {
        DraftCount = 0;
        SubmittedCount = 0;
        ReviewPendingCount = 0;
        ApprovedCount = 0;
        RejectedCount = 0;
        KycPendingCount = 0;
        ValidatingCount = 0;
    }

    /// <summary>
    /// Gets a user-friendly display name for the given status.
    /// </summary>
    public string GetStatusDisplayName(string status) => status switch
    {
        "Draft" => "Draft",
        "Submitted" => "Submitted",
        "ReviewPending" => "In Review",
        "KycPending" => "KYC Pending",
        "Validating" => "Validating",
        "Approved" => "Approved",
        "Rejected" => "Rejected",
        _ => status
    };

    /// <summary>
    /// Gets a user-friendly display name for the given application step.
    /// </summary>
    public string GetStepDisplayName(string step) => step switch
    {
        "Initial" => "Initial",
        "PersonalDetailsComplete" => "Personal Details",
        "ContactInformationComplete" => "Contact Info",
        "PassportAndIdentificationComplete" => "Identification",
        "ResidencyAndImmigrationComplete" => "Residency",
        "EmploymentAndEducationComplete" => "Employment",
        _ => step
    };

    /// <summary>
    /// Gets the color for a status chip based on the status value.
    /// </summary>
    public string GetStatusColor(string status) => status switch
    {
        "Draft" => "Default",
        "Submitted" => "Primary",
        "ReviewPending" or "KycPending" or "Validating" => "Warning",
        "Approved" => "Success",
        "Rejected" => "Error",
        _ => "Default"
    };

    /// <summary>
    /// Gets the color for a step chip.
    /// </summary>
    public string GetStepColor(string step) => "Info";
}
