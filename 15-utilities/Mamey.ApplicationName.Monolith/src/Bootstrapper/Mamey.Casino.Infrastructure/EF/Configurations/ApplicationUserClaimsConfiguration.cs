// // Infrastructure/EF/Configurations/ApplicationUserClaimsConfiguration.cs
//
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Abstractions.Entities;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Mamey.Casino.Infrastructure.EF.Configurations
// {
//     internal class ApplicationUserClaimsConfiguration
//         : IEntityTypeConfiguration<ApplicationUserClaim>
//     {
//         public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
//         {
//             builder.ToTable("UserClaims", "identity");
//             builder.HasKey(uc => uc.Id);
//
//             builder.HasOne<ApplicationUser>()
//                 .WithMany(u => u.Claims)
//                 .HasForeignKey(uc => uc.UserId)
//                 .IsRequired();
//         }
//     }
// }