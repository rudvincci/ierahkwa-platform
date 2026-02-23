// using Mamey.Casino.Domain.Entities;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//
// namespace Mamey.Casino.Infrastructure.EF.Storage;
//
// public class ApplicationUserStore : UserStore<ApplicationUser>
// {
//     private readonly CasinoDbContext _context;
//
//     public ApplicationUserStore(CasinoDbContext context)
//         : base(context)
//     {
//         _context = context;
//     }
//
//     public void Dispose()
//     {
//         // TODO release managed resources here
//     }
//
//     public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
//
//     public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
//     {
//         throw new NotImplementedException();
//     }
// }