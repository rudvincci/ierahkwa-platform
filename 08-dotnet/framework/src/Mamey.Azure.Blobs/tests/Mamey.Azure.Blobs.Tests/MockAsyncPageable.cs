using Azure;
using Moq;

namespace Mamey.Azure.Blobs.Tests;

// Helper class to mock AsyncPageable for listing blobs
public class MockAsyncPageable<T> : AsyncPageable<T>
{
    private readonly IReadOnlyList<T> _items;

    public MockAsyncPageable(IReadOnlyList<T> items)
    {
        _items = items;
    }

    public override async IAsyncEnumerable<Page<T>> AsPages(string continuationToken = null, int? pageSizeHint = null)
    {
        yield return Page<T>.FromValues(_items, null, Mock.Of<Response>());
        await Task.CompletedTask;
    }
}



