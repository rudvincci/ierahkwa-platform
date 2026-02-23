// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Mamey.ApplicationName.Modules.Identity.Core.Domain.Entities;
// using Mamey.ApplicationName.Modules.Identity.Core.Domain.Types;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Repositories
// {
//     public interface IIdentityRepository
//     {
//         Task AddAsync(ApplicationUser applicationuser); 
//         Task<IReadOnlyList<ApplicationUser>> BrowseAsync();
//         Task<bool> ExistsAsync(Guid id);
//         Task<ApplicationUser?> GetAsync(ApplicationUserId id);
//         Task UpdateAsync(ApplicationUser applicationuser);
//     }
// }