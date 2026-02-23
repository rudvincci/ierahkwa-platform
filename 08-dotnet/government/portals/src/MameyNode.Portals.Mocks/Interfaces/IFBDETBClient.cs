using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IFBDETBClient
{
    Task<List<AccountInfo>> GetAccountsAsync();
    Task<AccountInfo?> GetAccountAsync(string accountId);
    Task<List<WalletInfo>> GetWalletsAsync();
    Task<List<CardInfo>> GetCardsAsync();
    Task<List<TerminalInfo>> GetTerminalsAsync();
    Task<List<LoanInfo>> GetLoansAsync();
    Task<List<CreditRiskInfo>> GetCreditRisksAsync();
    Task<List<FBDETBCollateralInfo>> GetCollateralsAsync();
    Task<List<ExchangeInfo>> GetExchangesAsync();
    Task<List<TreasuryInfo>> GetTreasuryInfoAsync();
    Task<List<ComplianceInfo>> GetComplianceChecksAsync();
    Task<List<SecurityInfo>> GetSecurityAlertsAsync();
    Task<List<InsuranceInfo>> GetInsurancePoliciesAsync();
}

