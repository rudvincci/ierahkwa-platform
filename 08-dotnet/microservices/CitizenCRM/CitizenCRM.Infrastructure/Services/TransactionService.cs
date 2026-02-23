using CitizenCRM.Core.Interfaces;
using CitizenCRM.Core.Models;

namespace CitizenCRM.Infrastructure.Services
{
    public class TransactionService : ITransactionService
    {
        private List<VipTransaction> _transactions;
        private readonly Dictionary<string, int> _categorySequence;

        public TransactionService()
        {
            _transactions = TransactionCodes.GetAllTransactions();
            _categorySequence = new Dictionary<string, int>
            {
                { "SWIFT", 5 },
                { "Government", 3 },
                { "Asset", 3 },
                { "Crypto", 2 },
                { "API", 4 },
                { "Card", 2 }
            };
        }

        public List<VipTransaction> GetAllTransactions()
        {
            return _transactions.OrderBy(t => t.Code).ToList();
        }

        public VipTransaction? GetByCode(string code)
        {
            return _transactions.FirstOrDefault(t => 
                t.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        }

        public List<VipTransaction> GetByCategory(string category)
        {
            return _transactions
                .Where(t => t.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.Code)
                .ToList();
        }

        public List<VipTransaction> GetByPriority(string priority)
        {
            return _transactions
                .Where(t => t.Priority.Equals(priority, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(t => t.CompletionPercentage)
                .ToList();
        }

        public List<VipTransaction> GetByStatus(string status)
        {
            return _transactions
                .Where(t => t.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .OrderBy(t => t.Code)
                .ToList();
        }

        public TransactionSummary GetSummary()
        {
            return TransactionCodes.GetSummary();
        }

        public VipTransaction UpdateTransaction(string code, VipTransaction updated)
        {
            var existing = GetByCode(code);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Transaction {code} not found");
            }

            var index = _transactions.FindIndex(t => t.Code == code);
            updated.Code = code; // Preserve original code
            updated.UpdatedAt = DateTime.UtcNow;
            _transactions[index] = updated;
            
            return updated;
        }

        public string GenerateNewCode(string category)
        {
            var categoryCode = category.ToUpper() switch
            {
                "SWIFT" => "SWIFT",
                "GOVERNMENT" or "GOV" => "GOV",
                "ASSET" => "ASSET",
                "CRYPTO" => "CRYPTO",
                "API" => "API",
                "CARD" => "CARD",
                _ => "MISC"
            };

            if (!_categorySequence.ContainsKey(category))
            {
                _categorySequence[category] = 1;
            }
            else
            {
                _categorySequence[category]++;
            }

            var sequence = _categorySequence[category];
            return $"NET.{categoryCode}.{sequence:D3}";
        }
    }
}
