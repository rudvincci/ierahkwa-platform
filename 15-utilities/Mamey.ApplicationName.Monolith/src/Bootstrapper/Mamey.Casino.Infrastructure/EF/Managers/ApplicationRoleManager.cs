// using Mamey.Casino.Domain.Entities;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Logging;
//
//
// namespace Mamey.Casino.Infrastructure.EF.Managers;
//
// public class ApplicationRoleManager : RoleManager<ApplicationRole>
// {
//     public ApplicationRoleManager(
//         IRoleStore<ApplicationRole> store,
//         IEnumerable<IRoleValidator<ApplicationRole>> roleValidators,
//         ILookupNormalizer keyNormalizer,
//         IdentityErrorDescriber errors,
//         ILogger<RoleManager<ApplicationRole>> logger)
//         : base(store, roleValidators, keyNormalizer, errors, logger)
//     {
//     }
//     
//
//     // Override or add custom role-related logic as needed.
// }