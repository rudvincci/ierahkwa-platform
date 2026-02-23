// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Abstractions.Entities;
// using Mamey.Types;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Mamey.ApplicationName.Modules.Identity.Api.Controllers;
//
// /// <summary>
// /// CRUD operations for application roles.
// /// </summary>
// [Authorize(Policy = "Permission:Role.Manage")]
// internal class RoleController : BaseController
// {
//     private readonly IRoleManager _roleManager;
//
//     public RoleController(IRoleManager roleManager)
//         => _roleManager = roleManager;
//
//     [HttpGet]
//     public async Task<ActionResult<IEnumerable<ApplicationRole>>> GetAll()
//         => Ok(await _roleManager.GetAllAsync());
//
//     [HttpGet("{id:guid}")]
//     public async Task<ActionResult<ApplicationRole>> GetById(Guid id)
//     {
//         var role = await _roleManager.FindByIdAsync(new RoleId(id));
//         if (role == null) return NotFound();
//         return Ok(role);
//     }
//
//     [HttpPost]
//     public async Task<ActionResult> Create([FromBody] ApplicationRole role)
//     {
//         var created = await _roleManager.CreateAsync(role, User.FindFirst("sub")!.Value);
//         return CreatedAtAction(nameof(GetById), new { id = created.Id.Value }, created);
//     }
//
//     [HttpPut("{id:guid}")]
//     public async Task<ActionResult> Update(Guid id, [FromBody] ApplicationRole updated)
//     {
//         var role = await _roleManager.FindByIdAsync(new RoleId(id));
//         if (role == null) return NotFound();
//
//         role.Description    = updated.Description;
//         role.IsSystemRole   = updated.IsSystemRole;
//         role.Name           = updated.Name;
//         role.NormalizedName = updated.NormalizedName;
//
//         await _roleManager.UpdateAsync(role, User.FindFirst("sub")!.Value);
//         return NoContent();
//     }
//
//     [HttpDelete("{id:guid}")]
//     public async Task<ActionResult> Delete(Guid id)
//     {
//         await _roleManager.DeleteAsync(new RoleId(id), User.FindFirst("sub")!.Value);
//         return NoContent();
//     }
// }