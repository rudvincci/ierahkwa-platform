using LowCodeDesignService.Domain;
using LowCodeDesignService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowCodeDesignService.API;

[ApiController]
[Route("api/[controller]")]
public class AppDefinitionsController : ControllerBase
{
    private readonly LowCodeDesignServiceDbContext _context;
    public AppDefinitionsController(LowCodeDesignServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppDefinition>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.AppDefinitions.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppDefinition>> GetById(Guid id)
    {
        var entity = await _context.AppDefinitions.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<AppDefinition>> Create(AppDefinition entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.AppDefinitions.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, AppDefinition entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.AppDefinitions.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class UIComponentsController : ControllerBase
{
    private readonly LowCodeDesignServiceDbContext _context;
    public UIComponentsController(LowCodeDesignServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UIComponent>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.UIComponents.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UIComponent>> GetById(Guid id)
    {
        var entity = await _context.UIComponents.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<UIComponent>> Create(UIComponent entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.UIComponents.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UIComponent entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.UIComponents.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class DesignAssetsController : ControllerBase
{
    private readonly LowCodeDesignServiceDbContext _context;
    public DesignAssetsController(LowCodeDesignServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DesignAsset>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.DesignAssets.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DesignAsset>> GetById(Guid id)
    {
        var entity = await _context.DesignAssets.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<DesignAsset>> Create(DesignAsset entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.DesignAssets.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, DesignAsset entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.DesignAssets.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class FormSchemasController : ControllerBase
{
    private readonly LowCodeDesignServiceDbContext _context;
    public FormSchemasController(LowCodeDesignServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FormSchema>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.FormSchemas.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FormSchema>> GetById(Guid id)
    {
        var entity = await _context.FormSchemas.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<FormSchema>> Create(FormSchema entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.FormSchemas.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, FormSchema entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.FormSchemas.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class PageLayoutsController : ControllerBase
{
    private readonly LowCodeDesignServiceDbContext _context;
    public PageLayoutsController(LowCodeDesignServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PageLayout>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.PageLayouts.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PageLayout>> GetById(Guid id)
    {
        var entity = await _context.PageLayouts.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<PageLayout>> Create(PageLayout entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.PageLayouts.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, PageLayout entity)
    {
        if (id != entity.Id) return BadRequest();
        entity.UpdatedAt = DateTime.UtcNow;
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _context.PageLayouts.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
