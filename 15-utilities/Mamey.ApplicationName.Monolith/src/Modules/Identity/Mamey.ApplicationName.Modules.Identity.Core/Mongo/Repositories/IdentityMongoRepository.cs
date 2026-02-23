// using Mamey.ApplicationName.Modules.Identity.Core.Repositories;
// using Mamey.ApplicationName.Modules.Identity.Core.Domain.Entities;
// using Mamey.ApplicationName.Modules.Identity.Core.Domain.Types;
// using Mamey.ApplicationName.Modules.Identity.Core.Mongo.Documents;
// using Mamey.MicroMonolith.Infrastructure.Mongo;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Mongo.Repositories
// {
//     public class IdentityMongoRepository : IIdentityRepository
//     {
//         private readonly IMongoRepository<ApplicationUserDocument, Guid> _repository;
//
//         public IdentityMongoRepository(IMongoRepository<ApplicationUserDocument, Guid> repository)
//         {
//             _repository = repository;
//         }
//
//         public async Task AddAsync(ApplicationUser applicationuser)
//             => await _repository.AddAsync(new ApplicationUserDocument(applicationuser)); 
//
//         public async Task<IReadOnlyList<ApplicationUser>> BrowseAsync()
//             => (await _repository.FindAsync(_ => true))
//             .Select(c => c.AsEntity())
//             .ToList();
//
//         public async Task<bool> ExistsAsync(Guid id)
//             => await _repository.ExistsAsync(c => c.Id == id);
//
//         public async Task<ApplicationUser> GetAsync(ApplicationUserId id)
//         {
//             var applicationuser = await _repository.GetAsync(id);
//             return applicationuser?.AsEntity();
//         }
//
//         public async Task UpdateAsync(ApplicationUser applicationuser)
//             => await _repository.UpdateAsync(new ApplicationUserDocument(applicationuser));
//     }
// }
