// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
//
// using Mamey.ApplicationName.Modules.Products.Core.Repositories;
// using Mamey.ApplicationName.Modules.Products.Core.Entities;
// using Mamey.ApplicationName.Modules.Products.Core.Mongo.Documents;
// using Mamey.MicroMonolith.Infrastructure.Mongo;
//
// namespace Mamey.ApplicationName.Modules.Products.Core.Mongo.Repositories
// {
//     public class BankingProductsMongoRepository : IProductsRepository
//     {
//         private readonly IMongoRepository<ProductDocument, Guid> _repository;
//
//         public BankingProductsMongoRepository(IMongoRepository<ProductDocument, Guid> repository)
//         {
//             _repository = repository;
//         }
//
//         public async Task AddAsync(Product product)
//             => await _repository.AddAsync(new ProductDocument(product)); 
//
//         public async Task<IReadOnlyList<Product>> BrowseAsync()
//             => (await _repository.FindAsync(_ => true))
//             .Select(c => c.AsEntity())
//             .ToList();
//
//         public async Task<bool> ExistsAsync(Guid id)
//             => await _repository.ExistsAsync(c => c.Id == id);
//
//         public async Task<Product> GetAsync(Guid id)
//         {
//             var product = await _repository.GetAsync(id);
//             return product?.AsEntity();
//         }
//
//         public async Task UpdateAsync(Product product)
//             => await _repository.UpdateAsync(new ProductDocument(product));
//     }
// }
