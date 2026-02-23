using TaxAuthority.Core.Models;

namespace TaxAuthority.Core.Interfaces;

public interface ITaxFilingService
{
    Task<IEnumerable<TaxFiling>> GetAllAsync();
    Task<TaxFiling?> GetByIdAsync(string id);
    Task<IEnumerable<TaxFiling>> GetByCitizenIdAsync(string citizenId);
    Task<TaxFiling> CreateAsync(TaxFiling filing);
    Task UpdateAsync(TaxFiling filing);
    Task DeleteAsync(string id);
    Task SubmitFilingAsync(string id);
    Task<TaxStats> GetStatsAsync();
    Task<decimal> CalculateTax(decimal income, decimal deductions);
}
