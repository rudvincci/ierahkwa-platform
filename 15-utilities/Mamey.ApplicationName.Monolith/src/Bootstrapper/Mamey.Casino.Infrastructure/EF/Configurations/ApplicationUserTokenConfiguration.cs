// // Infrastructure/EF/Configurations/ApplicationUserTokenConfiguration.cs
//
// using Mamey.Auth.Identity.Abstractions;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
//
// namespace Mamey.Casino.Infrastructure.EF.Configurations
// {
//     internal class ApplicationUserTokenConfiguration
//         : IEntityTypeConfiguration<ApplicationUserToken>
//     {
//         public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
//         {
//             builder.ToTable("UserTokens", "identity");
//             builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
//
//             builder.HasOne<ApplicationUser>()
//                 .WithMany(u => u.Tokens)
//                 .HasForeignKey(t => t.UserId)
//                 .IsRequired();
//         }
//     }
// }