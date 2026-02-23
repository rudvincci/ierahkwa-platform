// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Types;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Identity.Data;
// using Microsoft.AspNetCore.Mvc;
//
// namespace Mamey.ApplicationName.Modules.Identity.Api.Controllers;
//
// /// <summary>
//     /// CRUD operations for application users.
//     /// </summary>
//
//     [Authorize(Policy = "Permission:User.Manage")]
//     public class UserController : BaseController
//     {
//         private readonly IUserManager _userManager;
//
//         public UserController(IUserManager userManager)
//             => _userManager = userManager;
//
//         /// <summary>
//         /// Lists all users in the current tenant.
//         /// </summary>
//         [HttpGet]
//         public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetAll()
//         {
//             // For performance, paging would be recommended
//             var users = await _userManager.GetAllAsync();
//             return Ok(users);
//         }
//
//         /// <summary>
//         /// Gets a single user by ID.
//         /// </summary>
//         [HttpGet("{id:guid}")]
//         public async Task<ActionResult<ApplicationUser>> GetById(Guid id)
//         {
//             var user = await _userManager.FindByIdAsync(new UserId(id));
//             if (user == null) return NotFound();
//             return Ok(user);
//         }
//
//         /// <summary>
//         /// Creates a new user.
//         /// </summary>
//         [HttpPost]
//         public async Task<ActionResult> Create([FromBody] RegisterRequest req)
//         {
//             var user = new ApplicationUser
//             {
//                 UserName           = req.UserName,
//                 NormalizedUserName = req.UserName.ToUpperInvariant(),
//                 Email              = req.Email,
//                 NormalizedEmail    = req.Email.ToUpperInvariant(),
//                 FullName           = req.FullName,
//                 PasswordHash       = req.Password
//             };
//
//             var created = await _userManager.CreateAsync(user, User.FindFirst("sub")!.Value);
//             return CreatedAtAction(nameof(GetById), new { id = created.Id.Value }, created);
//         }
//
//         /// <summary>
//         /// Updates an existing user.
//         /// </summary>
//         [HttpPut("{id:guid}")]
//         public async Task<ActionResult> Update(Guid id, [FromBody] RegisterRequest req)
//         {
//             var user = await _userManager.FindByIdAsync(new UserId(id));
//             if (user == null) return NotFound();
//
//             user.Email              = req.Email;
//             user.NormalizedEmail    = req.Email.ToUpperInvariant();
//             user.FullName           = req.FullName;
//             user.UserName           = req.UserName;
//             user.NormalizedUserName = req.UserName.ToUpperInvariant();
//
//             await _userManager.UpdateAsync(user, User.FindFirst("sub")!.Value);
//             return NoContent();
//         }
//
//         /// <summary>
//         /// Deletes (soft‑deletes) a user.
//         /// </summary>
//         [HttpDelete("{id:guid}")]
//         public async Task<ActionResult> Delete(Guid id)
//         {
//             await _userManager.DeleteAsync(new UserId(id), User.FindFirst("sub")!.Value);
//             return NoContent();
//         }
//     }