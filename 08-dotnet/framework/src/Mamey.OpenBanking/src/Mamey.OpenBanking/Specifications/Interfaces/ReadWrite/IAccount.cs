using Newtonsoft.Json;

public interface IAccount
{
    [JsonProperty("openapi")]
    public string OpenApi { get; set; }
    public IAccountInformation AccountInformation { get; set; }
    public Dictionary<string, IPath> Paths { get; set; }
}
