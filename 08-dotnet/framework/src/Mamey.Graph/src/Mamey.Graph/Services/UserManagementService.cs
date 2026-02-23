using Microsoft.Graph.Models;

namespace Mamey.Graph;

public class UserManagementService
{
    public UserManagementService(GraphOptions options)
    {
        GraphHelper.InitializeGraphForAppOnlyAuth(options);
    }
    public static Task<UserCollectionResponse?> GetUsersAsync()
    {
        //// Ensure client isn't null
        //_ = _appClient ??
        //    throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

        //return _appClient.Users.GetAsync((config) =>
        //{
        //    // Only request specific properties
        //    config.QueryParameters.Select = new[] { "displayName", "id", "mail" };
        //    // Get at most 25 results
        //    config.QueryParameters.Top = 25;
        //    // Sort by display name
        //    config.QueryParameters.Orderby = new[] { "displayName" };
        //});
        throw new NotImplementedException();
    }

    public async Task<T?> CreateUserAsync<T>(T user) where T : Microsoft.Graph.Models.User
            => (T?)await GraphHelper.B2CUserManagement.CreateUser(user);
    
}

