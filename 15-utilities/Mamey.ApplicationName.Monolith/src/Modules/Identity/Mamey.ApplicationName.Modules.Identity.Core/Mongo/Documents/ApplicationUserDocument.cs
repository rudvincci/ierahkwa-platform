// using System;
// using Mamey.ApplicationName.Modules.Identity.Core.Domain.Entities;
// using Mamey.MicroMonolith.Infrastructure.Mongo;
//
// namespace Mamey.ApplicationName.Modules.Identity.Core.Mongo.Documents
// {
//     public class ApplicationUserDocument : IIdentifiable<Guid>
//     {
//         public ApplicationUserDocument()
//         {
//         }
//
//         public ApplicationUserDocument(IdentityAuditLog applicationuser)
//         {
//             Id = applicationuser.Id;
//         }
//
//         public Guid Id { get; set; }
//         
//         public IdentityAuditLog AsEntity()
//             => new IdentityAuditLog(Id);
//     }
// }
