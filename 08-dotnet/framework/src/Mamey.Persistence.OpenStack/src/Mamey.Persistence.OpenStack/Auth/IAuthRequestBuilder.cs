using Mamey.Persistence.OpenStack.OCS.OpenStack.Requests;

namespace Mamey.Persistence.OpenStack.OCS.Auth;

internal interface IAuthRequestBuilder
{
    IAuthRequestBuilder WithMethod(string method);
    IAuthRequestBuilder WithMethods(IEnumerable<string> methods);
    IAuthRequestBuilder WithProject(string projectId);
    IAuthRequestBuilder WithUser(string id, string password);
    AuthRequest Build();
}