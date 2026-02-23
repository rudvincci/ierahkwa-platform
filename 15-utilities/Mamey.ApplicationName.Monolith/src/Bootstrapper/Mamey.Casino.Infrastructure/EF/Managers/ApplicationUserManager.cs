// using Mamey.Casino.Domain.Entities;
// using Mamey.Casino.Shared;
// using Mamey.Persistence.Redis;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.Extensions.Logging;
// using Microsoft.Extensions.Options;
//
//
// namespace Mamey.Casino.Infrastructure.EF.Managers;
//
// public class ApplicationUserManager : UserManager<ApplicationUser>
// {
//     private readonly ICache _cache;
//     public ApplicationUserManager(
//         IUserStore<ApplicationUser> store,
//         IOptions<IdentityOptions> optionsAccessor,
//         IPasswordHasher<ApplicationUser> passwordHasher,
//         IEnumerable<IUserValidator<ApplicationUser>> userValidators,
//         IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
//         ILookupNormalizer keyNormalizer,
//         IdentityErrorDescriber errors,
//         IServiceProvider services,
//         ILogger<ApplicationUserManager> logger, ICache cache)
//         : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
//     {
//         _cache = cache;
//     }
//
//     public async Task<ApplicationUser?> GetUserFromCacheAsync(Guid userId)
//     {
//         var cacheKey = $"user:{userId}";
//         // TODO: Decrypt
//         var cachedUser = await _cache.GetAsync<ApplicationUser>(cacheKey);
//         ApplicationUser? user = cachedUser;
//         
//         
//         if (user == null)
//         {
//             
//             user = await FindByIdAsync(userId.ToString());
//             if (user != null)
//             {
//                 await _cache.SetAsync(cacheKey, user);
//             }
//         }
//         return user;
//     }
//
//     public async Task CacheUserAsync(ApplicationUser user, TimeSpan? expiration = null)
//     {
//         var cacheKey = $"user:{user.Id}";
//         await _cache.DeleteAsync<AuthenticatedUser>(cacheKey);
//     
//         // TODO: Encrypt user
//         // TODO: Get Expiry from configuration
//         await _cache.SetAsync(cacheKey, user, expiration ?? TimeSpan.FromMinutes(30));
//     }
//     
//     // Override or add custom user-related logic as needed.
// }