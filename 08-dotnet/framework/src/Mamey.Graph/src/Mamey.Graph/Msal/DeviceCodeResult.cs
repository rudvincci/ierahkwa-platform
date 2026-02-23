namespace Mamey.Graph.Msal;
public class DeviceCodeResult
{
    public string UserCode { get; set; }
    public string DeviceCode { get; set; }
    public string VerificationUrl { get; set; }
    public DateTimeOffset ExpiresOn { get; set; }
    public int Interval { get; set; }
    public string Message { get; set; }

    public DeviceCodeResult(string userCode, string deviceCode, string verificationUrl, DateTimeOffset expiresOn, int interval, string message)
    {
        UserCode = userCode;
        DeviceCode = deviceCode;
        VerificationUrl = verificationUrl;
        ExpiresOn = expiresOn;
        Interval = interval;
        Message = message;
    }
}
