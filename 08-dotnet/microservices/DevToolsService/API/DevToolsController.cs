using DevToolsService.Domain;
using DevToolsService.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevToolsService.API;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly DevToolsServiceDbContext _context;
    public ProjectsController(DevToolsServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.Projects.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetById(Guid id)
    {
        var entity = await _context.Projects.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<Project>> Create(Project entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.Projects.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Project entity)
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
        var entity = await _context.Projects.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class RepositoriesController : ControllerBase
{
    private readonly DevToolsServiceDbContext _context;
    public RepositoriesController(DevToolsServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Repository>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.Repositories.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Repository>> GetById(Guid id)
    {
        var entity = await _context.Repositories.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<Repository>> Create(Repository entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.Repositories.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Repository entity)
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
        var entity = await _context.Repositories.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class CodeSnippetsController : ControllerBase
{
    private readonly DevToolsServiceDbContext _context;
    public CodeSnippetsController(DevToolsServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CodeSnippet>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.CodeSnippets.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CodeSnippet>> GetById(Guid id)
    {
        var entity = await _context.CodeSnippets.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<CodeSnippet>> Create(CodeSnippet entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.CodeSnippets.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CodeSnippet entity)
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
        var entity = await _context.CodeSnippets.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class TemplatesController : ControllerBase
{
    private readonly DevToolsServiceDbContext _context;
    public TemplatesController(DevToolsServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Template>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.Templates.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Template>> GetById(Guid id)
    {
        var entity = await _context.Templates.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<Template>> Create(Template entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.Templates.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Template entity)
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
        var entity = await _context.Templates.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
public class BuildPipelinesController : ControllerBase
{
    private readonly DevToolsServiceDbContext _context;
    public BuildPipelinesController(DevToolsServiceDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BuildPipeline>>> GetAll([FromQuery] Guid tenantId)
    {
        return await _context.BuildPipelines.Where(e => e.TenantId == tenantId && e.IsActive).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BuildPipeline>> GetById(Guid id)
    {
        var entity = await _context.BuildPipelines.FindAsync(id);
        if (entity == null) return NotFound();
        return entity;
    }

    [HttpPost]
    public async Task<ActionResult<BuildPipeline>> Create(BuildPipeline entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.IsActive = true;
        _context.BuildPipelines.Add(entity);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, BuildPipeline entity)
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
        var entity = await _context.BuildPipelines.FindAsync(id);
        if (entity == null) return NotFound();
        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
