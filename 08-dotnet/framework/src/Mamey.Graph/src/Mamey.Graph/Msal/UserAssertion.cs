namespace Mamey.Graph.Msal;
public class UserAssertion
{
    public string AssertionType { get; set; }
    public string Assertion { get; set; }

    public UserAssertion(string assertion, string assertionType = "urn:ietf:params:oauth:grant-type:jwt-bearer")
    {
        Assertion = assertion;
        AssertionType = assertionType;
    }
}
