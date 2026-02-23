using MameyNode.Portals.Mocks.Models;

namespace MameyNode.Portals.Mocks.Interfaces;

public interface IFutureWampumGovClient
{
    Task<List<FutureWampumGovDisbursementInfo>> GetDisbursementsAsync();
    Task<List<DisbursementBatchInfo>> GetDisbursementBatchesAsync();
    Task<List<UBIProgramInfo>> GetUBIProgramsAsync();
    Task<List<UBIRecipientInfo>> GetUBIRecipientsAsync();
    Task<List<BudgetAllocationInfo>> GetBudgetAllocationsAsync();
    Task<List<ProgramInfo>> GetProgramsAsync();
    Task<TransparencyDashboardData> GetTransparencyDashboardAsync();
}

