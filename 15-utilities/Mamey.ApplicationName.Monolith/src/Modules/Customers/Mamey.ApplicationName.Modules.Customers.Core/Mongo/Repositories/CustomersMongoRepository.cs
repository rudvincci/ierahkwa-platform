// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;

// using Mamey.ApplicationName.Modules.Customers.Core.Mongo.Documents;
// using Mamey.MicroMonolith.Infrastructure.Mongo;
// using Mamey.ApplicationName.Modules.Customers.Core.Domain.Entities;
// using Mamey.ApplicationName.Modules.Customers.Core.Domain.Repositories;

// namespace Mamey.ApplicationName.Modules.Customers.Core.Mongo.Repositories
// {
//     public class CustomersMongoRepository : ICustomerRepository
//     {
//         private readonly IMongoRepository<CustomerDocument, Guid> _repository;

//         public CustomersMongoRepository(IMongoRepository<CustomerDocument, Guid> repository)
//         {
//             _repository = repository;
//         }

//         public async Task AddAsync(Customer customer)
//             => await _repository.AddAsync(new CustomerDocument(customer)); 

//         public async Task<IReadOnlyList<Customer>> BrowseAsync()
//             => (await _repository.FindAsync(_ => true))
//             .Select(c => c.AsEntity())
//             .ToList();

//         public async Task<bool> ExistsAsync(Guid id)
//             => await _repository.ExistsAsync(c => c.Id == id);

//         public Task<bool> ExistsAsync(string email)
//             => await _repository.ExistsAsync(c => c. == id);

//         public async Task<Customer> GetAsync(Guid id)
//         {
//             var customer = await _repository.GetAsync(id);
//             return customer?.AsEntity();
//         }

//         public async Task UpdateAsync(Customer customer)
//             => await _repository.UpdateAsync(new CustomerDocument(customer));
//     }
// }
