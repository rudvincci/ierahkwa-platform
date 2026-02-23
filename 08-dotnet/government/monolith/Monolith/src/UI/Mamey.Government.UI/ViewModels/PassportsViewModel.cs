using ReactiveUI;

namespace Mamey.Government.UI.ViewModels;

public class PassportsViewModel : ReactiveObject
{
    private bool _isLoading;
    private string? _searchTerm;
    private string? _selectedStatus;
    private int _currentPage = 1;
    private int _pageSize = 20;
    private int _totalCount;

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

    public int TotalCount
    {
        get => _totalCount;
        set => this.RaiseAndSetIfChanged(ref _totalCount, value);
    }

    // Status counts for filter cards
    public int TotalPassports { get; set; }
    public int ActiveCount { get; set; }
    public int ExpiringSoonCount { get; set; }
    public int ExpiredCount { get; set; }
    public int RevokedCount { get; set; }
}
