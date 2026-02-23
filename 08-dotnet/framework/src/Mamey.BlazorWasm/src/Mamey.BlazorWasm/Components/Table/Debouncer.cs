namespace Mamey.BlazorWasm.Components.Table;

public class Debouncer
{
    private CancellationTokenSource _cancellationTokenSource;
    private readonly TimeSpan _delay;

    public Debouncer(TimeSpan delay)
    {
        _delay = delay;
    }

    public async Task DebounceAsync(Func<Task> action)
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();

        try
        {
            await Task.Delay(_delay, _cancellationTokenSource.Token);
            await action();
        }
        catch (TaskCanceledException)
        {
            // Ignore cancellation
        }
    }
}