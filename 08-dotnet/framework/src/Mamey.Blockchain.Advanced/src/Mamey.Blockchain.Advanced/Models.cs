namespace Mamey.Blockchain.Advanced;

public class TokenizeAssetRequest
{
    public string AssetId { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string TotalSupply { get; set; } = string.Empty;
    public string Metadata { get; set; } = string.Empty;
    public Dictionary<string, string> Properties { get; set; } = new();
}

public class TokenizeAssetResult
{
    public string TokenId { get; set; } = string.Empty;
    public string ContractAddress { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}




