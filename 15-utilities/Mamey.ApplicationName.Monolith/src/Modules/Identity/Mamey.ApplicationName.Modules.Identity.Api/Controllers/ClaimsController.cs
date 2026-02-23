// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Abstractions.EF;
// using Mamey.Auth.Identity.Abstractions.Entities;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
//
// namespace Mamey.ApplicationName.Modules.Identity.Api.Controllers;
//
// /// <summary>
// /// CRUD operations for permission definitions.
// /// </summary>
// [Authorize(Policy = "Permission:Permission.Manage")]
// internal class ClaimController : BaseController
// {
//     private readonly MameyIdentityDbContext _db;
//
//     public ClaimController(IdentityDbContext db) => _db = db;
//
//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<Permission>>> GetAll()
//         => Ok(await _db.Permissions.ToListAsync());
//
//     [HttpGet("{id:guid}")]
//     public async Task<ActionResult<Permission>> GetById(Guid id)
//     {
//         var perm = await _db.Permissions.FindAsync(id);
//         if (perm == null) return NotFound();
//         return Ok(perm);
//     }
//
//     [HttpPost]
//     public async Task<ActionResult> Create([FromBody] Permission p)
//     {
//         _db.Permissions.Add(p);
//         await _db.SaveChangesAsync();
//         return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
//     }
//
//     [HttpPut("{id:guid}")]
//     public async Task<ActionResult> Update(Guid id, [FromBody] Permission updated)
//     {
//         var perm = await _db.Permissions.FindAsync(id);
//         if (perm == null) return NotFound();
//
//         perm.Name            = updated.Name;
//         perm.Description     = updated.Description;
//         perm.Scope           = updated.Scope;
//         perm.PermissionLevel = updated.PermissionLevel;
//
//         _db.Permissions.Update(perm);
//         await _db.SaveChangesAsync();
//         return NoContent();
//     }
//
//     [HttpDelete("{id:guid}")]
//     public async Task<ActionResult> Delete(Guid id)
//     {
//         var perm = await _db.Permissions.FindAsync(id);
//         if (perm == null) return NotFound();
//
//         _db.Permissions.Remove(perm);
//         await _db.SaveChangesAsync();
//         return NoContent();
//     }
// }