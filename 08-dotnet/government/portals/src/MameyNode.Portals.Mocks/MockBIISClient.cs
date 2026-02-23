using Bogus;
using MameyNode.Portals.Mocks.Interfaces;
using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks;

public class MockBIISClient : IBIISClient
{
    private readonly Faker _faker = new();
    private readonly List<LiquidityPoolInfo> _liquidityPools = new();
    private readonly List<CurrencyExchangeInfo> _currencyExchanges = new();
    private readonly List<CrossBorderSettlementInfo> _crossBorderSettlements = new();
    private readonly List<InterbankChannelInfo> _interbankChannels = new();
    private readonly List<BlockchainTransparencyInfo> _blockchainTransactions = new();
    private readonly List<AssetCollateralizationInfo> _collateralizedAssets = new();
    private readonly List<IdentityComplianceInfo> _identityComplianceChecks = new();
    private readonly List<ZKPPrivacyInfo> _zkpProofs = new();
    private readonly List<TreatyEnforcementInfo> _treatyEnforcements = new();
    private readonly List<LiquidityRiskInfo> _liquidityRisks = new();

    public MockBIISClient()
    {
        InitializeMockData();
    }

    private void InitializeMockData()
    {
        var poolFaker = new Faker<LiquidityPoolInfo>()
            .RuleFor(p => p.PoolId, f => $"POOL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(p => p.TreatyId, f => $"TREATY-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(p => p.TotalLiquidity, f => f.Finance.Amount(1000000, 100000000, 2).ToString("F2"))
            .RuleFor(p => p.Currency, f => f.PickRandom("USD", "EUR", "GBP", "CAD"))
            .RuleFor(p => p.Status, f => f.PickRandom<PoolStatus>())
            .RuleFor(p => p.CreatedAt, f => f.Date.Past(2))
            .RuleFor(p => p.SourceBank, f => $"BANK-{f.Random.AlphaNumeric(6)}")
            .RuleFor(p => p.TargetBank, f => $"BANK-{f.Random.AlphaNumeric(6)}")
            .RuleFor(p => p.AvailableLiquidity, (f, p) => (decimal.Parse(p.TotalLiquidity) * 0.7m).ToString("F2"))
            .RuleFor(p => p.ReservedLiquidity, (f, p) => (decimal.Parse(p.TotalLiquidity) * 0.3m).ToString("F2"));

        _liquidityPools.AddRange(poolFaker.Generate(50));

        var exchangeFaker = new Faker<CurrencyExchangeInfo>()
            .RuleFor(e => e.ExchangeId, f => $"EXCH-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(e => e.FromCurrency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(e => e.ToCurrency, f => f.PickRandom("USD", "EUR", "GBP", "CAD", "AUD"))
            .RuleFor(e => e.ExchangeRate, f => f.Random.Double(0.5, 2.0).ToString("F4"))
            .RuleFor(e => e.Amount, f => f.Finance.Amount(1000, 100000, 2).ToString("F2"))
            .RuleFor(e => e.ConvertedAmount, (f, e) => (decimal.Parse(e.Amount) * decimal.Parse(e.ExchangeRate)).ToString("F2"))
            .RuleFor(e => e.Status, f => f.PickRandom<TransactionStatus>())
            .RuleFor(e => e.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(e => e.TreatyId, f => $"TREATY-{f.Random.AlphaNumeric(8).ToUpper()}");

        _currencyExchanges.AddRange(exchangeFaker.Generate(100));

        var settlementFaker = new Faker<CrossBorderSettlementInfo>()
            .RuleFor(s => s.SettlementId, f => $"SETTLE-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(s => s.SourceBank, f => $"BANK-{f.Random.AlphaNumeric(6)}")
            .RuleFor(s => s.TargetBank, f => $"BANK-{f.Random.AlphaNumeric(6)}")
            .RuleFor(s => s.Amount, f => f.Finance.Amount(10000, 1000000, 2).ToString("F2"))
            .RuleFor(s => s.Currency, f => f.PickRandom("USD", "EUR", "GBP"))
            .RuleFor(s => s.Status, f => f.PickRandom("Pending", "Processing", "Completed", "Failed"))
            .RuleFor(s => s.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(s => s.ProcessedAt, (f, s) => s.Status == "Completed" ? f.Date.Between(s.CreatedAt, DateTime.Now) : null)
            .RuleFor(s => s.TransactionId, f => $"TXN-{f.Random.AlphaNumeric(12).ToUpper()}")
            .RuleFor(s => s.TreatyId, f => $"TREATY-{f.Random.AlphaNumeric(8).ToUpper()}");

        _crossBorderSettlements.AddRange(settlementFaker.Generate(75));

        // Generate remaining mock data...
        var channelFaker = new Faker<InterbankChannelInfo>()
            .RuleFor(c => c.ChannelId, f => $"CHAN-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.BankA, f => $"BANK-{f.Random.AlphaNumeric(6)}")
            .RuleFor(c => c.BankB, f => $"BANK-{f.Random.AlphaNumeric(6)}")
            .RuleFor(c => c.ChannelType, f => f.PickRandom("Direct", "Indirect", "Hub"))
            .RuleFor(c => c.IsActive, f => f.Random.Bool(0.8f))
            .RuleFor(c => c.EstablishedAt, f => f.Date.Past(1))
            .RuleFor(c => c.Capacity, f => f.Finance.Amount(500000, 5000000, 2).ToString("F2"))
            .RuleFor(c => c.CurrentUsage, (f, c) => (decimal.Parse(c.Capacity) * f.Random.Decimal(0.1m, 0.9m)).ToString("F2"));

        _interbankChannels.AddRange(channelFaker.Generate(30));

        var transactionFaker = new Faker<BlockchainTransparencyInfo>()
            .RuleFor(t => t.BlockHash, f => $"0x{f.Random.AlphaNumeric(64)}")
            .RuleFor(t => t.TransactionId, f => $"TXN-{f.Random.AlphaNumeric(12).ToUpper()}")
            .RuleFor(t => t.FromAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(t => t.ToAccount, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(t => t.Amount, f => f.Finance.Amount(100, 50000, 2).ToString("F2"))
            .RuleFor(t => t.Currency, "USD")
            .RuleFor(t => t.Timestamp, f => f.Date.Recent(7))
            .RuleFor(t => t.BlockHeight, f => (ulong)f.Random.Long(1000000, 5000000))
            .RuleFor(t => t.IsConfirmed, f => f.Random.Bool(0.9f));

        _blockchainTransactions.AddRange(transactionFaker.Generate(200));

        // Add more mock data generators for remaining types...
        var collateralFaker = new Faker<AssetCollateralizationInfo>()
            .RuleFor(c => c.CollateralId, f => $"COLL-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.AssetId, f => $"ASSET-{f.Random.AlphaNumeric(8)}")
            .RuleFor(c => c.AssetType, f => f.PickRandom("RealEstate", "Commodity", "Security", "Currency"))
            .RuleFor(c => c.CollateralValue, f => f.Finance.Amount(50000, 5000000, 2).ToString("F2"))
            .RuleFor(c => c.Currency, "USD")
            .RuleFor(c => c.LoanAmount, (f, c) => (decimal.Parse(c.CollateralValue) * 0.7m).ToString("F2"))
            .RuleFor(c => c.CollateralRatio, (f, c) => (decimal.Parse(c.CollateralValue) / decimal.Parse(c.LoanAmount)).ToString("F2"))
            .RuleFor(c => c.CreatedAt, f => f.Date.Past(1))
            .RuleFor(c => c.Status, f => f.PickRandom("Active", "Released", "Liquidated"));

        _collateralizedAssets.AddRange(collateralFaker.Generate(40));

        var complianceFaker = new Faker<IdentityComplianceInfo>()
            .RuleFor(c => c.ComplianceId, f => $"COMP-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(c => c.AccountId, f => $"ACC-{f.Random.AlphaNumeric(8)}")
            .RuleFor(c => c.ComplianceType, f => f.PickRandom("KYC", "AML", "Sanctions", "PEP"))
            .RuleFor(c => c.Status, f => f.PickRandom("Pending", "Verified", "Rejected"))
            .RuleFor(c => c.VerifiedAt, f => f.Date.Recent(60))
            .RuleFor(c => c.TreatyId, f => $"TREATY-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(c => c.IsCompliant, f => f.Random.Bool(0.85f))
            .RuleFor(c => c.Violations, f => f.Random.Bool(0.1f) ? f.Lorem.Words(f.Random.Int(1, 3)).ToList() : new List<string>());

        _identityComplianceChecks.AddRange(complianceFaker.Generate(150));

        var zkpFaker = new Faker<ZKPPrivacyInfo>()
            .RuleFor(z => z.ProofId, f => $"ZKP-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(z => z.TransactionId, f => $"TXN-{f.Random.AlphaNumeric(12).ToUpper()}")
            .RuleFor(z => z.ProofType, f => f.PickRandom("RangeProof", "MembershipProof", "EqualityProof"))
            .RuleFor(z => z.IsVerified, f => f.Random.Bool(0.95f))
            .RuleFor(z => z.CreatedAt, f => f.Date.Recent(30))
            .RuleFor(z => z.PublicInput, f => $"0x{f.Random.AlphaNumeric(64)}");

        _zkpProofs.AddRange(zkpFaker.Generate(80));

        var enforcementFaker = new Faker<TreatyEnforcementInfo>()
            .RuleFor(e => e.EnforcementId, f => $"ENF-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(e => e.TreatyId, f => $"TREATY-{f.Random.AlphaNumeric(8).ToUpper()}")
            .RuleFor(e => e.ViolationType, f => f.PickRandom("NonCompliance", "Breach", "Fraud", "Misrepresentation"))
            .RuleFor(e => e.Severity, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(e => e.Status, f => f.PickRandom("Open", "UnderReview", "Resolved", "Dismissed"))
            .RuleFor(e => e.ReportedAt, f => f.Date.Recent(90))
            .RuleFor(e => e.ResolvedAt, (f, e) => e.Status == "Resolved" ? f.Date.Between(e.ReportedAt, DateTime.Now) : null)
            .RuleFor(e => e.Description, f => f.Lorem.Paragraph());

        _treatyEnforcements.AddRange(enforcementFaker.Generate(25));

        var riskFaker = new Faker<LiquidityRiskInfo>()
            .RuleFor(r => r.RiskId, f => $"RISK-{f.Random.AlphaNumeric(10).ToUpper()}")
            .RuleFor(r => r.PoolId, f => _liquidityPools[f.Random.Int(0, _liquidityPools.Count - 1)].PoolId)
            .RuleFor(r => r.RiskLevel, f => f.PickRandom("Low", "Medium", "High", "Critical"))
            .RuleFor(r => r.RiskScore, f => f.Random.Double(0, 100).ToString("F2"))
            .RuleFor(r => r.AssessedAt, f => f.Date.Recent(30))
            .RuleFor(r => r.RiskFactors, f => f.Lorem.Words(f.Random.Int(1, 5)).ToList())
            .RuleFor(r => r.Recommendation, f => f.Lorem.Sentence());

        _liquidityRisks.AddRange(riskFaker.Generate(60));
    }

    public Task<List<LiquidityPoolInfo>> GetLiquidityPoolsAsync() => Task.FromResult(_liquidityPools);
    public Task<LiquidityPoolInfo?> GetLiquidityPoolAsync(string poolId) => Task.FromResult(_liquidityPools.FirstOrDefault(p => p.PoolId == poolId));
    public Task<List<CurrencyExchangeInfo>> GetCurrencyExchangesAsync() => Task.FromResult(_currencyExchanges);
    public Task<List<CrossBorderSettlementInfo>> GetCrossBorderSettlementsAsync() => Task.FromResult(_crossBorderSettlements);
    public Task<List<InterbankChannelInfo>> GetInterbankChannelsAsync() => Task.FromResult(_interbankChannels);
    public Task<List<BlockchainTransparencyInfo>> GetBlockchainTransactionsAsync(int limit = 50) => Task.FromResult(_blockchainTransactions.Take(limit).ToList());
    public Task<List<AssetCollateralizationInfo>> GetCollateralizedAssetsAsync() => Task.FromResult(_collateralizedAssets);
    public Task<List<IdentityComplianceInfo>> GetIdentityComplianceChecksAsync() => Task.FromResult(_identityComplianceChecks);
    public Task<List<ZKPPrivacyInfo>> GetZKPProofsAsync() => Task.FromResult(_zkpProofs);
    public Task<List<TreatyEnforcementInfo>> GetTreatyEnforcementsAsync() => Task.FromResult(_treatyEnforcements);
    public Task<List<LiquidityRiskInfo>> GetLiquidityRisksAsync() => Task.FromResult(_liquidityRisks);
}

