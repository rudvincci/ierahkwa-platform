using Microsoft.AspNetCore.Mvc;
using CitizenCRM.Core.Models;

namespace CitizenCRM.API.Controllers
{
    /// <summary>
    /// API de Base de Datos de Transacciones
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController : ControllerBase
    {
        public DatabaseController()
        {
            TransactionDatabase.Initialize();
        }

        [HttpGet]
        public ActionResult<IEnumerable<VipTransactionRecord>> GetAll()
        {
            return Ok(TransactionDatabase.GetAll());
        }

        [HttpGet("stats")]
        public ActionResult<TransactionStats> GetStats()
        {
            return Ok(TransactionDatabase.GetStats());
        }

        [HttpGet("type/{type}")]
        public ActionResult<IEnumerable<VipTransactionRecord>> GetByType(string type)
        {
            return Ok(TransactionDatabase.GetByType(type.ToUpper()));
        }

        [HttpGet("bank/{bankCode}")]
        public ActionResult<IEnumerable<VipTransactionRecord>> GetByBank(string bankCode)
        {
            return Ok(TransactionDatabase.GetByBank(bankCode.ToUpper()));
        }

        [HttpGet("ierahkwa/{code}")]
        public ActionResult<VipTransactionRecord> GetByIerahkwaCode(string code)
        {
            var tx = TransactionDatabase.GetByIerahkwaCode(code);
            if (tx == null) return NotFound();
            return Ok(tx);
        }

        [HttpPost]
        public ActionResult<VipTransactionRecord> Add([FromBody] VipTransactionRecord tx)
        {
            var added = TransactionDatabase.Add(tx);
            return CreatedAtAction(nameof(GetByIerahkwaCode), new { code = added.IerahkwaCode }, added);
        }

        [HttpPut("{id}")]
        public ActionResult<VipTransactionRecord> Update(int id, [FromBody] VipTransactionRecord tx)
        {
            var updated = TransactionDatabase.Update(id, tx);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
    }
}
