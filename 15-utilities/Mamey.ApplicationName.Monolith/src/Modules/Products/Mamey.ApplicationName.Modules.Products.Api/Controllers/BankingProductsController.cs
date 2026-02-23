using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mamey.ApplicationName.Modules.Products.Core.Commands;
using Mamey.ApplicationName.Modules.Products.Core.DTO;
using Mamey.ApplicationName.Modules.Products.Core.Queries;
using Mamey.MicroMonolith.Abstractions.Contexts;
using Mamey.MicroMonolith.Abstractions.Dispatchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Http;


namespace Mamey.ApplicationName.Modules.Products.Api.Controllers
{
    
    internal class BankingProductsController : BaseController
    {
        private const string Policy = "products";
        private readonly IDispatcher _dispatcher;
        private readonly IContext _context;

        public BankingProductsController(IDispatcher dispatcher, IContext context)
        {
            _dispatcher = dispatcher;
            _context = context;
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Retrieve all banking products",
            Description = "Fetches all banking products along with their related details.",
            OperationId = "GetBakingProductById",
            Tags = new[] { "Banking Products" }
        )]
        [ProducesResponseType(typeof(IEnumerable<BankingProductDto>), 200)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<BankingProductDto>> GetProductAsync(Guid id) 
            =>  OkOrNotFound(await _dispatcher.QueryAsync(new GetBakingProductById(id)));
        
        /// <summary>
        /// Get all banking products.
        /// </summary>
        /// <returns>A list of banking products.</returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Retrieve all banking products",
            Description = "Fetches all banking products along with their related details.",
            OperationId = "GetAllBankingProducts",
            Tags = new[] { "Banking Products" }
        )]
        [ProducesResponseType(typeof(IEnumerable<BankingProductDto>), 200)]
        public async Task<ActionResult<IEnumerable<BankingProductDto>>> GetAllBankingProductsAsync() 
            => OkOrNotFound(await _dispatcher.QueryAsync(new GetAllBankingProducts()));

        [HttpPost("")]
        [Authorize]
        [SwaggerOperation(
            Summary = "Create a new banking product",
            Description = "Adds a new banking product to the system.",
            OperationId = "CreateBankingProduct",
            Tags = new[] { "Banking Products" }
        )]
        [ProducesResponseType(typeof(BankingProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> CreateProductAsync(CreateBankingProduct command)
        {
            await _dispatcher.SendAsync(command);
            var product = await _dispatcher.QueryAsync(new GetBakingProductById(command.Id));
            return CreatedAtAction(nameof(GetProductAsync), new {id = product.Id}, product);
        }

        // [Authorize]
        [HttpPut("{productId:guid}")]
        [SwaggerOperation(
            Summary = "UpdateAsync an existing banking product",
            Description = "Modifies the details of an existing banking product.",
            OperationId = "UpdateBankingProduct",
            Tags = new[] { "Banking Products" }
        )]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> UpdateBankingProductAsync(UpdateBankingProduct command)
        {
            var product = await _dispatcher.QueryAsync(new GetBakingProductById(command.Id));
            if (product == null)
            {
                return NotFound();
            }
            product.Id = command.Id;
            await _dispatcher.SendAsync(command);
            return Accepted();
        }
        
        [Authorize]
        [HttpDelete("{productId:guid}")]
        [SwaggerOperation(
            Summary = "Delete a banking product",
            Description = "Soft deletes an existing banking product.",
            OperationId = "DeleteBankingProduct",
            Tags = new[] { "Banking Products" }
        )]
        public async Task<ActionResult> DeleteBankingProductAsync(DeleteBankingProduct command)
        {
            await _dispatcher.QueryAsync(new GetBakingProductById(command.Id));
            return NoContent();
        }
        /// <summary>
        /// Get banking products by account category.
        /// </summary>
        /// <param name="category">The account category (e.g., Corporate, Individual).</param>
        /// <returns>A list of banking products in the specified category.</returns>
        [HttpGet("category/{category}")]
        [SwaggerOperation(
            Summary = "Retrieve banking products by account category",
            Description = "Fetches all banking products for a specific account category.",
            OperationId = "GetBankingProductsByAccountCategory",
            Tags = new[] { "Banking Products" }
        )]
        [ProducesResponseType(typeof(IEnumerable<BankingProductDto>), 200)]
        public async Task<ActionResult<IEnumerable<BankingProductDto>>> GetByCategoryAsync(GetBankingProductsByAccountCategory query)
        {
            return Ok(await _dispatcher.QueryAsync(new GetBankingProductsByAccountCategory(query.Category)));
        }

        /// <summary>
        /// Get active banking products.
        /// </summary>
        /// <returns>A list of active banking products.</returns>
        [HttpGet("active")]
        [AllowAnonymous]
        [SwaggerOperation(
            Summary = "Retrieve all active banking products",
            Description = "Fetches all banking products that are currently active.",
            OperationId = "GetActiveBankingProducts",
            Tags = new[] { "Banking Products" }
        )]
        [ProducesResponseType(typeof(IEnumerable<BankingProductDto>), 200)]
        public async Task<ActionResult<IEnumerable<BankingProductDto>>> GetActive()
        {
            return Ok(await _dispatcher.QueryAsync(new GetAllActiveBankingProducts()));;
        }

        /// <summary>
        /// Add multiple banking products in bulk.
        /// </summary>
        /// <param name="dtos">A collection of banking product details.</param>
        /// <returns>No content.</returns>
        [HttpPost("bulk")]
        [SwaggerOperation(
            Summary = "Add multiple banking products in bulk",
            Description = "Adds multiple banking products to the system in one operation.",
            OperationId = "AddBankingProductsInBulk",
            Tags = new[] { "Banking Products" }
        )]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBulk([FromBody] IEnumerable<CreateBankingProduct> commands)
        {
            await _dispatcher.SendAsync(new CreateBulkProducts(commands));
            return Accepted();
        }
    }
}