// using System;
// using Mamey.ApplicationName.Modules.Products.Core.Entities;
// using Mamey.MicroMonolith.Infrastructure.Mongo;
//
// namespace Mamey.ApplicationName.Modules.Products.Core.Mongo.Documents
// {
//     public class BankingProductDocument : IIdentifiable<Guid>
//     {
//         public BankingProductDocument()
//         {
//         }
//
//         public BankingProductDocument(Product product)
//         {
//             Id = product.Id;
//         }
//
//         public Guid Id { get; set; }
//         
//         public Product AsEntity()
//             => new Product
//             {
//                 Id = Id
//             };
//     }
// }
