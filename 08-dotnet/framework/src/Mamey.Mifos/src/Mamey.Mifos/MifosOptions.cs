namespace Mamey.Mifos
{
    public class MifosOptions
    {
		public bool   Enabled      { get; set; }
        public string HostUrl      { get; set; }
		public string AuthType     { get; set; } // basic or oauth
		public string Username     { get; set; }
		public string Password     { get; set; }
		public string ClientId     { get; set; }
		public string GrantType    { get; set; }
		public string ClientSecret { get; set; }
		public string TenantId { get; set; }

    }
}

