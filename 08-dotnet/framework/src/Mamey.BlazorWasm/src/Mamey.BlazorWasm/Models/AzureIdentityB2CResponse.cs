namespace Mamey.BlazorWasm.Models
{
    public class AzureIdentityB2CResponse
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public long ExpiresIn { get; set; }
        public string IdToken { get; set; }
    }
}

