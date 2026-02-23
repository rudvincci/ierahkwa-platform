using Microsoft.Graph.Models;

namespace Mamey.Graph;

public interface IGraphService
{
    Task DisplayAccessTokenAsync();
    Task<List<User>?> ListUsersAsync();
    Task MakeGraphCallAsync();
    Task<List<Message>> GetSharedMailboxMessagesAsync(string sharedMailboxEmail);
    Task<Message?> SendSharedMailboxMessageAsync(string sharedMailboxEmail,
        Message message, CancellationToken cancellationToken = default);

    Task<User?> CreateUserAsync(User user);
}
