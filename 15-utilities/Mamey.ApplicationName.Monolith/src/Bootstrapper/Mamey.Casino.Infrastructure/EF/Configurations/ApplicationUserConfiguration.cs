// // Infrastructure/EF/Configurations/ApplicationUserConfiguration.cs
//
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Abstractions.Entities;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using Mamey.Types;
//
// namespace Mamey.Casino.Infrastructure.EF.Configurations
// {
//     internal class ApplicationUserConfiguration
//         : IEntityTypeConfiguration<ApplicationUser>
//     {
//         public void Configure(EntityTypeBuilder<ApplicationUser> builder)
//         {
//             builder.ToTable("Users", "identity");
//             builder.HasKey(u => u.Id.Value);
//             builder.Property(u => u.Id)
//                    .HasConversion(x => x, x => new UserId(x))
//                    .ValueGeneratedNever();
//
//             builder.OwnsOne(u => u.Name, name =>
//             {
//                 name.Property(n => n.FirstName)
//                     .HasColumnName("FirstName").HasMaxLength(50);
//                 name.Property(n => n.LastName)
//                     .HasColumnName("LastName").HasMaxLength(50);
//                 name.Property(n => n.MiddleName)
//                     .HasColumnName("MiddleName").HasMaxLength(50);
//                 name.Property(n => n.Nickname)
//                     .HasColumnName("Nickname").HasMaxLength(50);
//             });
//
//             builder.Property(u => u.FullName).HasMaxLength(200);
//             builder.Property(u => u.DateRegistered)
//                    .ValueGeneratedOnAdd();
//
//             builder.HasIndex(u => u.Email)
//                    .HasDatabaseName("IX_Users_Email");
//
//             // → link to ApplicationUserRole
//             // builder.HasMany(u => u.UserRoles)
//             //        .WithOne(ur => ur.User)
//             //        .HasForeignKey(ur => ur.UserId)
//             //        .IsRequired();
//
//             // → IdentityUserClaim<Guid>
//             builder.HasMany(u => u.Claims)
//                    .WithOne()
//                    .HasForeignKey(c => c.UserId)
//                    .IsRequired();
//
//             builder.HasMany<ApplicationUserLogin>(u => u.Logins)
//                    .WithOne()
//                    .HasForeignKey(l => l.UserId)
//                    .IsRequired();
//
//             builder.HasMany<ApplicationUserToken>(u => u.Tokens)
//                    .WithOne()
//                    .HasForeignKey(t => t.UserId)
//                    .IsRequired();
//             builder.HasOne(u => u.Tenant)
//                 .WithMany()
//                 .HasForeignKey(u => u.TenantId)
//                 .OnDelete(DeleteBehavior.Cascade);
//             
//             builder.Property(u => u.RowVersion).IsRowVersion();
//             
//             builder.HasQueryFilter(u => !u.IsDeleted);
//             
//             builder.HasIndex(u => new { u.NormalizedUserName, u.TenantId }).IsUnique();
//             builder.HasIndex(u => new { u.NormalizedEmail, u.TenantId }).IsUnique();
//         }
//     }
// }
