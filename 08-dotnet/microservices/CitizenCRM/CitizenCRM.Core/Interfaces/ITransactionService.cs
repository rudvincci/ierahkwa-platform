using CitizenCRM.Core.Models;

namespace CitizenCRM.Core.Interfaces
{
    public interface ITransactionService
    {
        List<VipTransaction> GetAllTransactions();
        VipTransaction? GetByCode(string code);
        List<VipTransaction> GetByCategory(string category);
        List<VipTransaction> GetByPriority(string priority);
        List<VipTransaction> GetByStatus(string status);
        TransactionSummary GetSummary();
        VipTransaction UpdateTransaction(string code, VipTransaction transaction);
        string GenerateNewCode(string category);
    }
}
