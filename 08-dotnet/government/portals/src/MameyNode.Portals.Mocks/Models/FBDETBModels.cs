namespace MameyNode.Portals.Mocks.Models;

// Models based on banking.proto, payments.proto, and wallet.proto for FBDETB

public class AccountInfo
{
    public string AccountId { get; set; } = string.Empty;
    public string BlockchainAccount { get; set; } = string.Empty;
    public string Balance { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public AccountStatus Status { get; set; }
    public string AccountType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string OwnerId { get; set; } = string.Empty;
}

public class WalletInfo
{
    public string WalletId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public Dictionary<string, string> Balances { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string WalletType { get; set; } = "Standard";
    public bool IsActive { get; set; } = true;
}

public class CardInfo
{
    public string CardId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string CardNumber { get; set; } = string.Empty;
    public string CardType { get; set; } = string.Empty; // Virtual, Physical, Biometric
    public DateTime ExpiryDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime IssuedAt { get; set; }
    public string Status { get; set; } = "Active";
}

public class TerminalInfo
{
    public string TerminalId { get; set; } = string.Empty;
    public string MerchantId { get; set; } = string.Empty;
    public string TerminalType { get; set; } = string.Empty; // Mobile, POS, Kiosk
    public bool IsActive { get; set; }
    public DateTime RegisteredAt { get; set; }
    public string Location { get; set; } = string.Empty;
}

// LoanInfo and CollateralInfo are already defined in LendingModels.cs

public class CreditRiskInfo
{
    public string RiskId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string RiskScore { get; set; } = "0";
    public string RiskLevel { get; set; } = "Low";
    public DateTime AssessedAt { get; set; }
    public List<string> RiskFactors { get; set; } = new();
    public string Recommendation { get; set; } = string.Empty;
}

public class FBDETBCollateralInfo
{
    public string CollateralId { get; set; } = string.Empty;
    public string LoanId { get; set; } = string.Empty;
    public string AssetType { get; set; } = string.Empty;
    public string AssetValue { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = "Active";
}

public class ExchangeInfo
{
    public string ExchangeId { get; set; } = string.Empty;
    public string FromCurrency { get; set; } = string.Empty;
    public string ToCurrency { get; set; } = string.Empty;
    public string ExchangeRate { get; set; } = "1.0";
    public string Amount { get; set; } = "0";
    public string ConvertedAmount { get; set; } = "0";
    public DateTime ExecutedAt { get; set; }
    public string Status { get; set; } = "Completed";
}

public class TreasuryInfo
{
    public string TreasuryId { get; set; } = string.Empty;
    public string Currency { get; set; } = "USD";
    public string TotalSupply { get; set; } = "0";
    public string CirculatingSupply { get; set; } = "0";
    public string TreasuryBalance { get; set; } = "0";
    public DateTime LastUpdated { get; set; }
}

public class ComplianceInfo
{
    public string ComplianceId { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string ComplianceType { get; set; } = string.Empty; // KYC, AML, etc.
    public string Status { get; set; } = "Pending";
    public DateTime CheckedAt { get; set; }
    public bool IsCompliant { get; set; }
    public List<string> Violations { get; set; } = new();
}

public class SecurityInfo
{
    public string SecurityId { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public string SecurityType { get; set; } = string.Empty; // Fraud, Suspicious, etc.
    public string Severity { get; set; } = "Low";
    public string Status { get; set; } = "Open";
    public DateTime DetectedAt { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class InsuranceInfo
{
    public string InsuranceId { get; set; } = string.Empty;
    public string PolicyType { get; set; } = string.Empty; // ClanMutual, Sovereign, etc.
    public string CoverageAmount { get; set; } = "0";
    public string Premium { get; set; } = "0";
    public string Currency { get; set; } = "USD";
    public DateTime EffectiveDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Status { get; set; } = "Active";
    public string InsuredId { get; set; } = string.Empty;
}

