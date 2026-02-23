namespace NFT.Core.Models;

public class NFTCertificate
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TokenId { get; set; } = string.Empty;
    public string OwnerId { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public CertificateType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IssuedBy { get; set; } = "Ierahkwa Government";
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    public string MetadataUri { get; set; } = string.Empty;
    public string ImageUri { get; set; } = string.Empty;
    public string ContractAddress { get; set; } = string.Empty;
    public string TransactionHash { get; set; } = string.Empty;
    public bool IsVerified { get; set; } = true;
    public Dictionary<string, string> Attributes { get; set; } = new();
}

public enum CertificateType
{
    Citizenship,
    Education,
    Professional,
    Property,
    Business,
    Marriage,
    Birth,
    Achievement,
    Membership,
    License
}
