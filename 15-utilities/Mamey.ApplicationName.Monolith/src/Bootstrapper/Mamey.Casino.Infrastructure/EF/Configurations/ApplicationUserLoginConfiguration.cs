// // Infrastructure/EF/Configurations/ApplicationUserLoginConfiguration.cs
//
// using Mamey.Auth.Identity.Abstractions;
// using Mamey.Auth.Identity.Abstractions.Entities;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Mamey.Casino.Infrastructure.EF.Configurations
// {
//     internal class ApplicationUserLoginConfiguration
//         : IEntityTypeConfiguration<ApplicationUserLogin>
//     {
//         public void Configure(EntityTypeBuilder<ApplicationUserLogin> builder)
//         {
//             builder.ToTable("UserLogins", "identity");
//             builder.HasKey(l => new { l.LoginProvider, l.ProviderKey });
//
//             builder.HasOne<ApplicationUser>()
//                 .WithMany(u => u.Logins)
//                 .HasForeignKey(l => l.UserId)
//                 .IsRequired();
//         }
//     }
// }