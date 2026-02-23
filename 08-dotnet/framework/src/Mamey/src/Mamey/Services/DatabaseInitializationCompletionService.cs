using System.Threading;
using System.Threading.Tasks;

namespace Mamey.Services;

public class DatabaseInitializationCompletionService
{
    private readonly TaskCompletionSource<bool> _completionSource = new();
    private bool _isCompleted = false;

    public void SignalCompletion()
    {
        if (!_isCompleted)
        {
            _isCompleted = true;
            _completionSource.TrySetResult(true);
        }
    }

    public async Task WaitForCompletionAsync(CancellationToken cancellationToken = default)
    {
        if (_isCompleted)
        {
            return;
        }

        // For .NET Standard 2.1 compatibility, use Task.WhenAny instead of WaitAsync
        if (cancellationToken.CanBeCanceled)
        {
            var cancellationTaskSource = new TaskCompletionSource<bool>();
            using (cancellationToken.Register(() => cancellationTaskSource.TrySetCanceled()))
            {
                var completedTask = await Task.WhenAny(_completionSource.Task, cancellationTaskSource.Task);
                if (completedTask == cancellationTaskSource.Task)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }
        }
        else
        {
            await _completionSource.Task;
        }
    }
}
