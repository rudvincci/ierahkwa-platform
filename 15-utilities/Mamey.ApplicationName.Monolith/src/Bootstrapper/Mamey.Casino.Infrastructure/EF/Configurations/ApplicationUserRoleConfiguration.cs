// // Infrastructure/EF/Configurations/ApplicationUserRoleConfiguration.cs
//
// using Mamey.Auth.Identity.Abstractions;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Mamey.Casino.Infrastructure.EF.Configurations
// {
//     internal class ApplicationUserRoleConfiguration
//         : IEntityTypeConfiguration<ApplicationUserRole>
//     {
//         public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
//         {
//             builder.ToTable("UserRoles", "identity");
//             builder.HasKey(ur => new { ur.UserId, ur.RoleId });
//
//             // Explicitly map properties to columns
//             builder.Property(ur => ur.UserId).HasColumnName("UserId");
//             builder.Property(ur => ur.RoleId).HasColumnName("RoleId");
//
//             // Define relationship with ApplicationUser
//             // builder.HasOne(ur => ur.User)
//             //     .WithMany(u => u.UserRoles)
//             //     .HasForeignKey(ur => ur.UserId)
//             //     .HasConstraintName("FK_UserRoles_Users_UserId")
//             //     .OnDelete(DeleteBehavior.Cascade)
//             //     .IsRequired();
//
//             // Define relationship with ApplicationRole
//             builder.HasOne(ur => ur.Role)
//                 .WithMany(r => r.UserRoles)
//                 .HasForeignKey(ur => ur.RoleId)
//                 .HasConstraintName("FK_UserRoles_Roles_RoleId")
//                 .OnDelete(DeleteBehavior.Cascade)
//                 .IsRequired();
//         }
//     }
// }