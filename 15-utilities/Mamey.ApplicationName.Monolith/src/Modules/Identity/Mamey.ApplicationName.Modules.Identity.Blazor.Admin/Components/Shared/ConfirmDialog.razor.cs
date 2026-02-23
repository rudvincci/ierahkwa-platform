using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Mamey.ApplicationName.Modules.Identity.Blazor.Admin.Components.Shared;

/// <summary>
/// A reusable confirmation dialog component.
/// </summary>
public partial class ConfirmDialog : ComponentBase
{
    [Inject] private ILogger<ConfirmDialog> Logger { get; set; } = default!;

    private bool _isBusy;

    /// <summary>
    /// Whether the dialog is visible. Two-way bindable.
    /// </summary>
    [Parameter]
    public bool IsOpen { get; set; }

    /// <summary>
    /// EventCallback invoked when <see cref="IsOpen"/> changes.
    /// </summary>
    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    /// <summary>
    /// Title text displayed at the top of the dialog.
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Confirm";

    /// <summary>
    /// Message body displayed below the title.
    /// </summary>
    [Parameter]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Text for the confirmation button.
    /// </summary>
    [Parameter]
    public string ConfirmText { get; set; } = "Yes";

    /// <summary>
    /// Text for the cancel button.
    /// </summary>
    [Parameter]
    public string CancelText { get; set; } = "No";

    /// <summary>
    /// Invoked when the user confirms.
    /// </summary>
    [Parameter]
    public EventCallback OnConfirmed { get; set; }

    /// <summary>
    /// Invoked when the user cancels.
    /// </summary>
    [Parameter]
    public EventCallback OnCancelled { get; set; }

    /// <summary>
    /// True while the confirm action is in progress.
    /// Disables the confirm button and shows a spinner.
    /// </summary>
    protected bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Handles confirm button click: runs <see cref="OnConfirmed"/>, then closes.
    /// </summary>
    private async Task HandleConfirmAsync()
    {
        IsBusy = true;
        try
        {
            await OnConfirmed.InvokeAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in ConfirmDialog OnConfirmed");
        }
        finally
        {
            IsBusy = false;
            await CloseAsync();
        }
    }

    /// <summary>
    /// Handles cancel button click: runs <see cref="OnCancelled"/>, then closes.
    /// </summary>
    private async Task HandleCancelAsync()
    {
        try
        {
            await OnCancelled.InvokeAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error in ConfirmDialog OnCancelled");
        }
        finally
        {
            await CloseAsync();
        }
    }

    /// <summary>
    /// Closes the dialog by setting <see cref="IsOpen"/> to false.
    /// </summary>
    private async Task CloseAsync()
    {
        if (IsOpen)
        {
            IsOpen = false;
            await IsOpenChanged.InvokeAsync(false);
        }
    }
}