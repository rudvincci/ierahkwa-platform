using Microsoft.AspNetCore.Mvc;
using CitizenCRM.Core.Interfaces;
using CitizenCRM.Core.Models;

namespace CitizenCRM.API.Controllers
{
    /// <summary>
    /// VIP Transactions API Controller
    /// Sovereign Government of Ierahkwa Ne Kanienke
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        /// <summary>
        /// GET /api/transactions
        /// Returns all VIP transactions with their codes
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<VipTransaction>> GetAll()
        {
            var transactions = _transactionService.GetAllTransactions();
            return Ok(transactions);
        }

        /// <summary>
        /// GET /api/transactions/summary
        /// Returns summary statistics
        /// </summary>
        [HttpGet("summary")]
        public ActionResult<TransactionSummary> GetSummary()
        {
            var summary = _transactionService.GetSummary();
            return Ok(summary);
        }

        /// <summary>
        /// GET /api/transactions/codes
        /// Returns just the codes and names for quick reference
        /// </summary>
        [HttpGet("codes")]
        public ActionResult<IEnumerable<object>> GetCodes()
        {
            var transactions = _transactionService.GetAllTransactions();
            var codes = transactions.Select(t => new
            {
                t.Code,
                t.Name,
                t.Category,
                t.CompletionPercentage,
                t.Priority
            });
            return Ok(codes);
        }

        /// <summary>
        /// GET /api/transactions/{code}
        /// Returns a specific transaction by code
        /// Example: GET /api/transactions/NET.SWIFT.001
        /// </summary>
        [HttpGet("{code}")]
        public ActionResult<VipTransaction> GetByCode(string code)
        {
            var transaction = _transactionService.GetByCode(code);
            if (transaction == null)
            {
                return NotFound(new { message = $"Transaction {code} not found" });
            }
            return Ok(transaction);
        }

        /// <summary>
        /// GET /api/transactions/category/{category}
        /// Returns transactions filtered by category
        /// Categories: SWIFT, Government, Asset, Crypto, API, Card
        /// </summary>
        [HttpGet("category/{category}")]
        public ActionResult<IEnumerable<VipTransaction>> GetByCategory(string category)
        {
            var transactions = _transactionService.GetByCategory(category);
            return Ok(transactions);
        }

        /// <summary>
        /// GET /api/transactions/priority/{priority}
        /// Returns transactions filtered by priority
        /// Priorities: Critical, High, Medium, Low
        /// </summary>
        [HttpGet("priority/{priority}")]
        public ActionResult<IEnumerable<VipTransaction>> GetByPriority(string priority)
        {
            var transactions = _transactionService.GetByPriority(priority);
            return Ok(transactions);
        }

        /// <summary>
        /// GET /api/transactions/status/{status}
        /// Returns transactions filtered by status
        /// Status: Pending, In Progress, Completed, Cancelled
        /// </summary>
        [HttpGet("status/{status}")]
        public ActionResult<IEnumerable<VipTransaction>> GetByStatus(string status)
        {
            var transactions = _transactionService.GetByStatus(status);
            return Ok(transactions);
        }

        /// <summary>
        /// PUT /api/transactions/{code}
        /// Updates a transaction
        /// </summary>
        [HttpPut("{code}")]
        public ActionResult<VipTransaction> Update(string code, [FromBody] VipTransaction transaction)
        {
            try
            {
                var updated = _transactionService.UpdateTransaction(code, transaction);
                return Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Transaction {code} not found" });
            }
        }

        /// <summary>
        /// GET /api/transactions/generate-code/{category}
        /// Generates a new transaction code for a category
        /// </summary>
        [HttpGet("generate-code/{category}")]
        public ActionResult<string> GenerateCode(string category)
        {
            var newCode = _transactionService.GenerateNewCode(category);
            return Ok(new { code = newCode });
        }
    }
}
