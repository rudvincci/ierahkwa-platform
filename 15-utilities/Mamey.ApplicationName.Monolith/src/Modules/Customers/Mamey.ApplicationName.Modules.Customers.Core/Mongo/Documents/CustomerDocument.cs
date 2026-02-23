// using System;
// using Mamey.ApplicationName.Modules.Customers.Core.Domain.Entities;
// using Mamey.ApplicationName.Modules.Customers.Core.Entities;
// using Mamey.MicroMonolith.Infrastructure.Mongo;

// namespace Mamey.ApplicationName.Modules.Customers.Core.Mongo.Documents
// {
//     public class CustomerDocument : IIdentifiable<Guid>
//     {
//         public CustomerDocument()
//         {
//         }

//         public CustomerDocument(Customer customer)
//         {
//             Id = customer.Id;
//         }

//         public Guid Id { get; set; }
        
//         public Customer AsEntity()
//             => new Customer
//             {
//                 Id = Id
//             };
//     }
// }
