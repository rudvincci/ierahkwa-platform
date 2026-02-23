using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface ISICBClient
{
    Task<List<MonetaryInstrumentInfo>> GetMonetaryInstrumentsAsync();
    Task<List<LedgerReserveInfo>> GetLedgerReservesAsync();
    Task<List<LendingInfo>> GetLendingOperationsAsync();
    Task<List<MonetaryPolicyInfo>> GetMonetaryPoliciesAsync();
    Task<List<FiscalOperationInfo>> GetFiscalOperationsAsync();
    Task<List<TreasuryProgramInfo>> GetTreasuryProgramsAsync();
    Task<List<ForeignExchangeInfo>> GetForeignExchangesAsync();
    Task<List<ComplianceEnforcementInfo>> GetComplianceEnforcementsAsync();
    Task<List<CitizenToolInfo>> GetCitizenToolsAsync();
    Task<List<SystemIntegrityInfo>> GetSystemIntegrityChecksAsync();
    Task<List<TreasuryInstrumentInfo>> GetTreasuryInstrumentsAsync();
}

