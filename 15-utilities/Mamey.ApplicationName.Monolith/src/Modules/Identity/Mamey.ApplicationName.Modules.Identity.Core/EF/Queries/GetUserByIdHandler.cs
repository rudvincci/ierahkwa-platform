// using Mamey.ApplicationName.Modules.Identity.Core.DTO;
// using Mamey.ApplicationName.Modules.Identity.Core.Queries;
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Abstractions.Entities;
// using Mamey.CQRS.Queries;
// using Microsoft.AspNetCore.Identity;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.EF.Queries;
//
// internal sealed class GetUserByIdHandler : IQueryHandler<GetUserById, ApplicationUserDto?>
// {
//     private readonly UserManager<ApplicationUser> _userManager;
//     private readonly RoleManager<ApplicationRole> _roleManager; 
//
//     public GetUserByIdHandler(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
//     {
//         _userManager = userManager;
//         _roleManager = roleManager;
//     }
//
//     public async Task<ApplicationUserDto?> HandleAsync(GetUserById query, CancellationToken cancellationToken = default)
//     {
//         var user = await _userManager.FindByIdAsync(query.UserId.ToString());
//         if (user == null)
//             return null;
//         var roles = _roleManager.Roles
//             .Where(r => 
//                 user.UserRoles.Select(c=> c.RoleId)
//                     .Contains(r.Id)).Select(c=> c.Name);
//         return new ApplicationUserDto(user.Id, user.Email, user.UserName, string.Join(", ", roles), 
//             (user.LockoutEnabled? "Active"  :"Locked"),  user.Name, 
//             user.EmailConfirmed, user.LockoutEnabled);
//     }
// }