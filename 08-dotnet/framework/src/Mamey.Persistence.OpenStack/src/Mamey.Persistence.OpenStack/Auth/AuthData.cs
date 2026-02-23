namespace Mamey.Persistence.OpenStack.OCS.Auth;

internal class AuthData
{
    private AuthData() { }

    public AuthData(string subjectToken)
    {
        SubjectToken = subjectToken;
    }

    public string SubjectToken { get; }
}