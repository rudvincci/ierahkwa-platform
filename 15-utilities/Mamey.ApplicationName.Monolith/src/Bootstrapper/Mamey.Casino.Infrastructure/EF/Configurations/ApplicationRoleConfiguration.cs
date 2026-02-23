// // Infrastructure/EF/Configurations/ApplicationRoleConfiguration.cs
//
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Abstractions.Entities;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Mamey.Casino.Infrastructure.EF.Configurations
// {
//     internal class ApplicationRoleConfiguration
//         : IEntityTypeConfiguration<ApplicationRole>
//     {
//         public void Configure(EntityTypeBuilder<ApplicationRole> builder)
//         {
//             builder.ToTable("Roles", "identity");
//
//             builder.Property(r => r.Description)
//                 .HasMaxLength(250);
//
//             builder.HasMany(r => r.UserRoles)
//                 .WithOne(ur => ur.Role)
//                 .HasForeignKey(ur => ur.RoleId)
//                 .IsRequired();
//
//             builder.HasMany<ApplicationRoleClaim>(r => r.Claims)
//                 .WithOne()
//                 .HasForeignKey(rc => rc.RoleId)
//                 .IsRequired();
//         }
//     }
// }