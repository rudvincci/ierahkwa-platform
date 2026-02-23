using Microsoft.AspNetCore.Mvc;
using NET10.Core.Interfaces;
using NET10.Core.Models.ERP;

namespace NET10.API.Controllers.ERP
{
    [ApiController]
    [Route("api/erp/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        
        /// <summary>
        /// Get all products for a company
        /// </summary>
        [HttpGet("company/{companyId}")]
        public async Task<ActionResult<List<Product>>> GetAll(Guid companyId)
        {
            var products = await _productService.GetAllAsync(companyId);
            return Ok(products);
        }
        
        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetById(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }
        
        /// <summary>
        /// Get product by SKU
        /// </summary>
        [HttpGet("company/{companyId}/sku/{sku}")]
        public async Task<ActionResult<Product>> GetBySKU(Guid companyId, string sku)
        {
            var product = await _productService.GetBySKUAsync(companyId, sku);
            if (product == null)
                return NotFound();
            return Ok(product);
        }
        
        /// <summary>
        /// Create new product
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Product>> Create([FromBody] Product product)
        {
            var created = await _productService.CreateAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        
        /// <summary>
        /// Update product
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<Product>> Update(Guid id, [FromBody] Product product)
        {
            if (id != product.Id)
                return BadRequest("ID mismatch");
            
            var updated = await _productService.UpdateAsync(product);
            return Ok(updated);
        }
        
        /// <summary>
        /// Delete product
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result)
                return NotFound();
            return NoContent();
        }
        
        /// <summary>
        /// Search products
        /// </summary>
        [HttpGet("company/{companyId}/search")]
        public async Task<ActionResult<List<Product>>> Search(Guid companyId, [FromQuery] string q)
        {
            if (string.IsNullOrEmpty(q))
                return BadRequest("Search term required");
            
            var products = await _productService.SearchAsync(companyId, q);
            return Ok(products);
        }
        
        /// <summary>
        /// Get low stock products
        /// </summary>
        [HttpGet("company/{companyId}/low-stock")]
        public async Task<ActionResult<List<Product>>> GetLowStock(Guid companyId)
        {
            var products = await _productService.GetLowStockAsync(companyId);
            return Ok(products);
        }
        
        /// <summary>
        /// Get product categories
        /// </summary>
        [HttpGet("company/{companyId}/categories")]
        public async Task<ActionResult<List<ProductCategory>>> GetCategories(Guid companyId)
        {
            var categories = await _productService.GetCategoriesAsync(companyId);
            return Ok(categories);
        }
    }
}
