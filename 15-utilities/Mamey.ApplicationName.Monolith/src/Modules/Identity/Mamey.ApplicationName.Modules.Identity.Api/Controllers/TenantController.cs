// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Abstractions.EF;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
//
// namespace Mamey.ApplicationName.Modules.Identity.Api.Controllers;
//
// /// <summary>
// /// CRUD operations for tenants (super‑admin only).
// /// </summary>
// [Authorize(Policy = "Permission:Tenant.Manage")]
// internal class TenantController : BaseController
// {
//     private readonly MameyIdentityDbContext _db;
//
//     public TenantController(MameyIdentityDbContext db) => _db = db;
//
//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<Tenant>>> GetAll()
//         => Ok(await _db.Tenants.ToListAsync());
//
//     [HttpGet("{id:guid}")]
//     public async Task<ActionResult<Tenant>> GetById(Guid id)
//     {
//         var tenant = await _db.Tenants.FindAsync(id);
//         if (tenant == null) return NotFound();
//         return Ok(tenant);
//     }
//
//     [HttpPost]
//     public async Task<ActionResult> Create([FromBody] Tenant t)
//     {
//         _db.Tenants.Add(t);
//         await _db.SaveChangesAsync();
//         return CreatedAtAction(nameof(GetById), new { id = t.TenantId }, t);
//     }
//
//     [HttpPut("{id:guid}")]
//     public async Task<ActionResult> Update(Guid id, [FromBody] Tenant updated)
//     {
//         var tenant = await _db.Tenants.FindAsync(id);
//         if (tenant == null) return NotFound();
//
//         tenant.Name      = updated.Name;
//         tenant.Plan      = updated.Plan;
//         tenant.Status    = updated.Status;
//         tenant.UserLimit = updated.UserLimit;
//
//         _db.Tenants.Update(tenant);
//         await _db.SaveChangesAsync();
//         return NoContent();
//     }
//
//     [HttpDelete("{id:guid}")]
//     public async Task<ActionResult> Delete(Guid id)
//     {
//         var tenant = await _db.Tenants.FindAsync(id);
//         if (tenant == null) return NotFound();
//
//         _db.Tenants.Remove(tenant);
//         await _db.SaveChangesAsync();
//         return NoContent();
//     }
// }