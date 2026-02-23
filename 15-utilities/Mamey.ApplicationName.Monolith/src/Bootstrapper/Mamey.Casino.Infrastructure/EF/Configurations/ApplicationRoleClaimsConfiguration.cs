// // Infrastructure/EF/Configurations/ApplicationRoleClaimsConfiguration.cs
//
// using Mamey.Auth.Identity.Abstractions;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Mamey.Casino.Infrastructure.EF.Configurations
// {
//     internal class ApplicationRoleClaimsConfiguration
//         : IEntityTypeConfiguration<ApplicationRoleClaim>
//     {
//         public void Configure(EntityTypeBuilder<ApplicationRoleClaim> builder)
//         {
//             builder.ToTable("RoleClaims", "identity");
//             builder.HasKey(rc => rc.Id);
//
//             builder.HasOne(e => e.Role)
//                 .WithMany(e => e.Claims)
//                 .HasForeignKey(e => e.RoleId)
//                 .HasConstraintName("FK_RoleClaims_Roles_RoleId")
//                 .OnDelete(DeleteBehavior.Cascade);
//             
//         }
//     }
// }