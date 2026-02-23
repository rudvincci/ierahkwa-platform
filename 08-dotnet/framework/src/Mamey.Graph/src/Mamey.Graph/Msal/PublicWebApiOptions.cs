namespace Mamey.Graph.Msal;
public class PublicWebApiOptions
{
    public string? Instance { get; set; }
    public string? ClientId { get; set; }
    public string? Domain { get; set; }
    public string? SignedOutCallBackPath { get; set; }
    public string? SignUpSignInPolicyId { get; set; }
    public string? ResetPasswordPolicyId { get; set; }
    public string? EditProfilePolicyId { get; set; }
}