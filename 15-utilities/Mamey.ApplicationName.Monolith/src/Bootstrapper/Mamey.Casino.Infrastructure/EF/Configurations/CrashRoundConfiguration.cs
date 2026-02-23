// using Mamey.Casino.Domain.Entities;
// using Mamey.Casino.Domain.ValueObjects;
// using Mamey.Types;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;
// using Money = Mamey.Casino.Domain.ValueObjects.Money;
//
// namespace Mamey.Casino.Infrastructure.EF.Configurations;
//
// /// <summary>
// /// EF configuration for <see cref="CrashRound"/>.
// /// </summary>
// public class CrashRoundConfiguration : IEntityTypeConfiguration<CrashRound>
// {
//     public void Configure(EntityTypeBuilder<CrashRound> builder)
//     {
//         builder.ToTable("crash_rounds");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         builder.Property(x => x.CrashedAt).IsRequired();
//         builder.Property(x => x.EndTime);
//
//         builder
//             .HasMany(x => x.Bets)
//             .WithOne()
//             .HasForeignKey(b => b.RoundId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="CrashBet"/>.
// /// </summary>
// public class CrashBetConfiguration : IEntityTypeConfiguration<CrashBet>
// {
//     public void Configure(EntityTypeBuilder<CrashBet> builder)
//     {
//         builder.ToTable("crash_bets");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.RoundId).IsRequired();
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.Amount).HasConversion(m => m.Amount, a => new Money(a)).IsRequired();
//         builder.Property(x => x.HasCashedOut).IsRequired();
//         builder.Property(x => x.CashOutTime);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="EuropeanRouletteRound"/>.
// /// </summary>
// public class EuropeanRouletteConfiguration : IEntityTypeConfiguration<EuropeanRouletteRound>
// {
//     public void Configure(EntityTypeBuilder<EuropeanRouletteRound> builder)
//     {
//         builder.ToTable("european_roulette_rounds");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         builder.Property(x => x.EndTime);
//         builder.Property(x => x.WinningNumber);
//
//         builder
//             .HasMany(x => x.Bets)
//             .WithOne()
//             .HasForeignKey(b => b.RoundId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="RouletteBet"/>.
// /// </summary>
// public class RouletteBetConfiguration : IEntityTypeConfiguration<RouletteBet>
// {
//     public void Configure(EntityTypeBuilder<RouletteBet> builder)
//     {
//         builder.ToTable("roulette_bets");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.RoundId).IsRequired();
//         builder.Property(x => x.UserId).HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.Number);
//         builder.Property(x => x.Color);
//         builder.Property(x => x.Amount).HasConversion(m => m.Amount, a => new Money(a)).IsRequired();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="MultiplayerRouletteRound"/>.
// /// </summary>
// public class MultiplayerRouletteConfiguration : IEntityTypeConfiguration<MultiplayerRouletteRound>
// {
//     public void Configure(EntityTypeBuilder<MultiplayerRouletteRound> builder)
//     {
//         builder.ToTable("multiplayer_roulette_rounds");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         builder.Property(x => x.EndTime);
//         builder.Property(x => x.WinningNumber);
//
//         builder
//             .HasMany(x => x.Bets)
//             .WithOne()
//             .HasForeignKey(b => b.RoundId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="Dice3DRoll"/>.
// /// </summary>
// public class Dice3DRollConfiguration : IEntityTypeConfiguration<Dice3DRoll>
// {
//     public void Configure(EntityTypeBuilder<Dice3DRoll> builder)
//     {
//         builder.ToTable("dice3d_rolls");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         builder.Property(x => x.EndTime);
//         builder.Property(x => x.Die1);
//         builder.Property(x => x.Die2);
//         builder.Property(x => x.Die3);
//
//         builder
//             .HasMany<DiceRoll>()
//             .WithOne()
//             .HasForeignKey(r => r.RoundId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="Dice3DBet"/>.
// /// </summary>
// public class Dice3DBetConfiguration : IEntityTypeConfiguration<Dice3DBet>
// {
//     public void Configure(EntityTypeBuilder<Dice3DBet> builder)
//     {
//         builder.ToTable("dice3d_bets");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.RoundId).IsRequired();
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.BetType).HasConversion<int>().IsRequired();
//         builder.Property(x => x.Amount)
//             .HasConversion<decimal>(c=> c.Amount, c=> new Money(c)).IsRequired();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="DiceRoll"/>.
// /// </summary>
// public class DiceRollConfiguration : IEntityTypeConfiguration<DiceRoll>
// {
//     public void Configure(EntityTypeBuilder<DiceRoll> builder)
//     {
//         builder.ToTable("dice_rolls");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.RoundId).IsRequired();
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         // builder.Property(x => x.PredictedValue).HasColumnName("predicted_value").IsRequired();
//         builder.Property(x => x.Amount).HasConversion(m => m.Amount, a => new Money(a)).IsRequired();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="KenoGame"/>.
// /// </summary>
// public class KenoGameConfiguration : IEntityTypeConfiguration<KenoGame>
// {
//     public void Configure(EntityTypeBuilder<KenoGame> builder)
//     {
//         builder.ToTable("keno_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         builder.Property(x => x.CompletedAt);
//        
//         // Stored as JSON array of ints
//         // builder
//         //     .Property(x => x.DrawnNumbers)
//         //     .HasColumnName("drawn_numbers")
//         //     .HasConversion(
//         //         v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //         v => System.Text.Json.JsonSerializer.Deserialize<int[]>(v, null) ?? Array.Empty<int>())
//         //     .HasColumnType("nvarchar(max)");
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="KenoBet"/>.
// /// </summary>
// public class KenoBetConfiguration : IEntityTypeConfiguration<KenoBet>
// {
//     public void Configure(EntityTypeBuilder<KenoBet> builder)
//     {
//         builder.ToTable("keno_bets");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id).HasColumnName("id");
//         builder.Property(x => x.GameId).HasColumnName("game_id").IsRequired();
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id)).IsRequired();
//         builder.Property(x => x.Amount).HasConversion(m => m.Amount, a => new Money(a)).IsRequired();
//         // Stored as JSON array of ints
//         // builder
//         //     .Property(x => x.Numbers)
//         //     .HasColumnName("numbers")
//         //     .HasConversion(
//         //         v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //         v => System.Text.Json.JsonSerializer.Deserialize<int[]>(v, null) ?? Array.Empty<int>())
//         //     .HasColumnType("nvarchar(max)");
//         // builder.Property(x => x.Amount).HasColumnName("amount").HasConversion<decimal>().IsRequired();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="Slots3DRound"/>.
// /// </summary>
// public class Slots3DRoundConfiguration : IEntityTypeConfiguration<Slots3DRound>
// {
//     public void Configure(EntityTypeBuilder<Slots3DRound> builder)
//     {
//         builder.ToTable("slots3d_rounds");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.Variation)
//             .HasConversion<int>()
//             .IsRequired();
//         builder.Property(x => x.BetAmount)
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//         builder.Property(x => x.PlacedAt).IsRequired();
//         builder.Property(x => x.CompletedAt);
//         builder.Property(x => x.PayoutMultiplier)
//             .HasConversion(d => d.Value, v => new Multiplier(v));
//         builder.Property(x => x.PayoutAmount)
//             .HasConversion(m => m.Amount, a => new Money(a));
//
//         // grid stored as JSON array of arrays
//         // builder.Property("_grid")
//         //        .HasColumnName("grid")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer
//         //                     .Deserialize<int[][]>(v, null) ?? Array.Empty<int[]>())
//         //        .HasColumnType("nvarchar(max)");
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="SlotsGame"/>.
// /// </summary>
// public class SlotsGameConfiguration : IEntityTypeConfiguration<SlotsGame>
// {
//     public void Configure(EntityTypeBuilder<SlotsGame> builder)
//     {
//         builder.ToTable("slots_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.Variation)
//             .HasConversion<int>()
//             .IsRequired();
//         builder.Property(x => x.BetAmount)
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//         builder.Property(x => x.PlacedAt).IsRequired();
//         builder.Property(x => x.CompletedAt);
//         builder.Property(x => x.PayoutMultiplier)
//             .HasConversion(d => d.Value, v => new Multiplier(v));
//         builder.Property(x => x.PayoutAmount)
//             .HasConversion(m => m.Amount, a => new Money(a));
//
//         // builder.Property("_grid")
//         //        .HasColumnName("grid")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer
//         //                     .Deserialize<int[][]>(v, null) ?? Array.Empty<int[]>())
//         //        .HasColumnType("nvarchar(max)");
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="CaribbeanPokerGame"/>.
// /// </summary>
// public class CaribbeanPokerConfiguration : IEntityTypeConfiguration<CaribbeanPokerGame>
// {
//     public void Configure(EntityTypeBuilder<CaribbeanPokerGame> builder)
//     {
//         builder.ToTable("caribbean_poker_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.BetAmount)
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//         builder.Property(x => x.PlacedAt).IsRequired();
//         builder.Property(x => x.CompletedAt);
//         builder.Property(x => x.PayoutMultiplier)
//            ;
//         builder.Property(x => x.PayoutAmount)
//             
//             .HasConversion(m => m.Amount, a => new Money(a));
//
//         // builder.Property("_playerCards")
//         //        .HasColumnName("player_cards")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer
//         //                     .Deserialize<string[]>(v, null) ?? Array.Empty<string>())
//         //        .HasColumnType("nvarchar(max)");
//
//         // builder.Property("_dealerCards")
//         //        .HasColumnName("dealer_cards")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer
//         //                     .Deserialize<string[]>(v, null) ?? Array.Empty<string>())
//         //        .HasColumnType("nvarchar(max)");
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="CasinoHoldemGame"/>.
// /// </summary>
// public class CasinoHoldemConfiguration : IEntityTypeConfiguration<CasinoHoldemGame>
// {
//     public void Configure(EntityTypeBuilder<CasinoHoldemGame> builder)
//     {
//         builder.ToTable("casino_holdem_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id)).IsRequired();
//         builder.Property(x => x.AnteAmount)
//             
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//         builder.Property(x => x.RaiseAmount)
//            
//             .HasConversion(m => m.Amount, a => new Money(a));
//         builder.Property(x => x.PlacedAt).IsRequired();
//         builder.Property(x => x.CompletedAt);
//         builder.Property(x => x.PayoutMultiplier)
//            ;
//         builder.Property(x => x.PayoutAmount)
//             
//             .HasConversion(m => m.Amount, a => new Money(a));
//
//         // cards arrays
//         // builder.Property("_playerCards")
//         //        .HasColumnName("player_cards")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer
//         //                     .Deserialize<string[]>(v, null) ?? Array.Empty<string>())
//         //        .HasColumnType("nvarchar(max)");
//
//         // builder.Property("_dealerCards")
//         //        .HasColumnName("dealer_cards")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer
//         //                     .Deserialize<string[]>(v, null) ?? Array.Empty<string>())
//         //        .HasColumnType("nvarchar(max)");
//
//         // builder.Property("_communityCards")
//         //        .HasColumnName("community_cards")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer
//         //                     .Deserialize<string[]>(v, null) ?? Array.Empty<string>())
//         //        .HasColumnType("nvarchar(max)");
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="BlackjackGame"/>.
// /// </summary>
// public class BlackjackConfiguration : IEntityTypeConfiguration<BlackjackGame>
// {
//     public void Configure(EntityTypeBuilder<BlackjackGame> builder)
//     {
//         builder.ToTable("blackjack_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.BetAmount)
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//         builder.Property(x => x.PlacedAt).IsRequired();
//         builder.Property(x => x.CompletedAt);
//         // builder.Property(x => x.PayoutMultiplier)
//         //        .HasColumnName("payout_multiplier");
//         builder.Property(x => x.Payout)
//                //.HasColumnName("payout_amount")
//                .HasConversion(m => m.Amount, a => new Money(a));
//
//         // assume card lists
//         // builder.Property("_playerCards")
//         //     .HasColumnName("player_cards")
//         //     // .HasConversion(
//         //     //     v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //     //     v => System.Text.Json.JsonSerializer
//         //     //              .Deserialize<string[]>(v, null) ?? Array.Empty<string>())
//         //     .HasColumnType("nvarchar(max)");
//
//         // builder.Property("_dealerCards")
//         //     .HasColumnName("dealer_cards")
//         //     // .HasConversion(
//         //     //     v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //     //     v => System.Text.Json.JsonSerializer
//         //     //              .Deserialize<string[]>(v, null) ?? Array.Empty<string>())
//         //     .HasColumnType("nvarchar(max)");
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="AmericanRouletteRound"/>.
// /// </summary>
// public class AmericanRouletteConfiguration : IEntityTypeConfiguration<AmericanRouletteRound>
// {
//     public void Configure(EntityTypeBuilder<AmericanRouletteRound> builder)
//     {
//         builder.ToTable("american_roulette_rounds");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         builder.Property(x => x.EndTime);
//         builder.Property(x => x.WinningNumber);
//         // builder.Property(x => x.WinningColor)
//         //        .HasColumnName("winning_color")
//         //        .HasConversion<int>();
//
//         builder
//             .HasMany(x => x.Bets)
//             .WithOne()
//             .HasForeignKey(b => b.RoundId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="HeadsOrTailsGame"/>.
// /// </summary>
// public class HeadsOrTailsConfiguration : IEntityTypeConfiguration<HeadsOrTailsGame>
// {
//     public void Configure(EntityTypeBuilder<HeadsOrTailsGame> builder)
//     {
//         builder.ToTable("heads_or_tails_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.BetAmount)
//           
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//         builder.Property(x => x.ChosenSide)
//          
//             .HasConversion<int>()
//             .IsRequired();
//         builder.Property(x => x.PlacedAt).IsRequired();
//         builder.Property(x => x.CompletedAt);
//         builder.Property(x => x.ResultSide)
//             
//             .HasConversion<int?>();
//         builder.Property(x => x.IsWin).IsRequired();
//         builder.Property(x => x.PayoutAmount)
//             .HasConversion(m => m.Amount, a => new Money(a));
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="SicBoGame"/>.
// /// </summary>
// public class SicBoConfiguration : IEntityTypeConfiguration<SicBoGame>
// {
//     public void Configure(EntityTypeBuilder<SicBoGame> builder)
//     {
//         builder.ToTable("sicbo_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         builder.Property(x => x.CompletedAt);
//         // JSON for three dice
//         builder.Property(x => x.DiceResults)
//             
//             .HasConversion(
//                 v => System.Text.Json.JsonSerializer.Serialize(v, Mamey.JsonExtensions.SerializerOptions),
//                 v => System.Text.Json.JsonSerializer.Deserialize<int[]>(v, Mamey.JsonExtensions.SerializerOptions) ?? Array.Empty<int>())
//             ;
//
//         builder
//             .HasMany(x => x.Bets)
//             .WithOne()
//             .HasForeignKey(b => b.RoundId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="SicBoBet"/>.
// /// </summary>
// public class SicBoBetConfiguration : IEntityTypeConfiguration<SicBoBet>
// {
//     public void Configure(EntityTypeBuilder<SicBoBet> builder)
//     {
//         builder.ToTable("sicbo_bets");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.RoundId).IsRequired();
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id)).IsRequired();
//         builder.Property(x => x.BetType)
//             .HasConversion<int>()
//             .IsRequired();
//         builder.Property(x => x.Amount)
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="VideoPokerGame"/>.
// /// </summary>
// public class VideoPokerConfiguration : IEntityTypeConfiguration<VideoPokerGame>
// {
//     public void Configure(EntityTypeBuilder<VideoPokerGame> builder)
//     {
//         builder.ToTable("video_poker_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.BetAmount)
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//         builder.Property(x => x.PlacedAt).IsRequired();
//         builder.Property(x => x.CompletedAt);
//         // builder.Property(x => x.PayoutMultiplier)
//         //        .HasColumnName("payout_multiplier");
//         builder.Property(x => x.Payout)
//             .HasConversion(m => m.Amount, a => new Money(a));
//
//         //
//         // builder.Property("_cards")
//         //        .HasColumnName("cards")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, options: Mamey.JsonExtensions.SerializerOptions),
//         //            v => System.Text.Json.JsonSerializer.Deserialize<string[]>(v , options:  Mamey.JsonExtensions.SerializerOptions) ?? Array.Empty<string>())
//         //        .HasColumnType("nvarchar(max)");
//
//         // builder.Property(x => x.Combination)
//         //        .HasColumnName("combination")
//         //        .HasConversion<int>();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="Raffle"/>.
// /// </summary>
// public class RaffleConfiguration : IEntityTypeConfiguration<Raffle>
// {
//     public void Configure(EntityTypeBuilder<Raffle> builder)
//     {
//         builder.ToTable("raffles");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.Name).IsRequired();
//         builder.Property(x => x.OpensAt).IsRequired();
//         builder.Property(x => x.ClosesAt).IsRequired();
//         builder.Property(x => x.DrawnAt);
//         builder.Property(x => x.WinnerCount).IsRequired();
//
//         // // store winning ticket IDs as JSON array of GUID
//         // builder.Property(x => x.WinningTicketIds)
//         //        .HasColumnName("winning_ticket_ids")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer.Deserialize<Guid[]>(v, null) ?? Array.Empty<Guid>())
//         //        .HasColumnType("nvarchar(max)");
//
//         builder
//             .HasMany(x => x.Tickets)
//             .WithOne()
//             .HasForeignKey(t => t.RaffleId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="RaffleTicket"/>.
// /// </summary>
// public class RaffleTicketConfiguration : IEntityTypeConfiguration<RaffleTicket>
// {
//     public void Configure(EntityTypeBuilder<RaffleTicket> builder)
//     {
//         builder.ToTable("raffle_tickets");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.RaffleId).IsRequired();
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.PurchasedAt).IsRequired();
//         builder.Property(x => x.Price).HasConversion<decimal>(c => c.Amount, a => new Money(a)).IsRequired();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="CryptoPredictionGame"/>.
// /// </summary>
// public class CryptoPredictionConfiguration : IEntityTypeConfiguration<CryptoPredictionGame>
// {
//     public void Configure(EntityTypeBuilder<CryptoPredictionGame> builder)
//     {
//         builder.ToTable("crypto_prediction_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         // builder.Property(x => x.EndTime).HasColumnName("end_time");
//         // builder.Property(x => x.CryptoSymbol).HasColumnName("crypto_symbol").HasMaxLength(10).IsRequired();
//
//         builder
//             .HasMany(x => x.Bets)
//             .WithOne()
//             .HasForeignKey(b => b.GameId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="CryptoPredictionBet"/>.
// /// </summary>
// public class CryptoPredictionBetConfiguration : IEntityTypeConfiguration<CryptoPredictionBet>
// {
//     public void Configure(EntityTypeBuilder<CryptoPredictionBet> builder)
//     {
//         builder.ToTable("crypto_prediction_bets");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.GameId).IsRequired();
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.Prediction)
//       
//             .HasConversion<int>()
//             .IsRequired();
//         builder.Property(x => x.Amount)
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="HorseRacingRace"/>.
// /// </summary>
// public class HorseRacingConfiguration : IEntityTypeConfiguration<HorseRacingRace>
// {
//     public void Configure(EntityTypeBuilder<HorseRacingRace> builder)
//     {
//         builder.ToTable("horse_racing_races");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         // builder.Property(x => x.EndTime).HasColumnName("end_time");
//         // builder.Property(x => x.TrackName).HasColumnName("track_name").HasMaxLength(100);
//
//         builder
//             .HasMany(x => x.Bets)
//             .WithOne()
//             .HasForeignKey(b => b.RaceId)
//             .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="HorseBet"/>.
// /// </summary>
// public class HorseBetConfiguration : IEntityTypeConfiguration<HorseBet>
// {
//     public void Configure(EntityTypeBuilder<HorseBet> builder)
//     {
//         builder.ToTable("horse_bets");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.RaceId).IsRequired();
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         builder.Property(x => x.PickOrder)
//             
//             .HasConversion<int>()
//             .IsRequired();
//         builder.Property(x => x.Amount)
//             
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="LuckyWheelSpin"/>.
// /// </summary>
// public class LuckyWheelConfiguration : IEntityTypeConfiguration<LuckyWheelSpin>
// {
//     public void Configure(EntityTypeBuilder<LuckyWheelSpin> builder)
//     {
//         builder.ToTable("lucky_wheel_spins");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.StartTime).IsRequired();
//         builder.Property(x => x.CompletedAt);
//
//         // store result section enum
//         builder.Property(x => x.ResultSection)
//
//             .HasConversion<int?>();
//
//         // store configured sections as JSON
//         // builder.Property("_sections")
//         //        .HasColumnName("sections")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer
//         //                     .Deserialize<StakeGames.Domain.Enums.LuckyWheelSection[]>(v, null) 
//         //                ?? Array.Empty<StakeGames.Domain.Enums.LuckyWheelSection>())
//         //        .HasColumnType("nvarchar(max)");
//
//         // builder
//         //     .HasMany(x => x.Bets)
//         //     .WithOne()
//         //     .HasForeignKey(b => b.SpinId)
//         //     .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="LuckyWheelBet"/>.
// /// </summary>
// public class LuckyWheelBetConfiguration : IEntityTypeConfiguration<LuckyWheelBet>
// {
//     public void Configure(EntityTypeBuilder<LuckyWheelBet> builder)
//     {
//         builder.ToTable("lucky_wheel_bets");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         // builder.Property(x => x.SpinId).HasColumnName("spin_id").IsRequired();
//         builder.Property(x => x.Section)
//            
//             .HasConversion<int>()
//             .IsRequired();
//         builder.Property(x => x.Amount)
//            
//             .HasConversion(m => m.Amount, a => new Money(a))
//             .IsRequired();
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="PlinkoGame"/>.
// /// </summary>
// public class PlinkoConfiguration : IEntityTypeConfiguration<PlinkoGame>
// {
//     public void Configure(EntityTypeBuilder<PlinkoGame> builder)
//     {
//         builder.ToTable("plinko_games");
//         builder.HasKey(x => x.Id);
//         builder.Property(x => x.Id);
//         builder.Property(x => x.UserId)
//             .HasConversion<Guid>(c=> c.Value, id => new UserId(id))
//             .IsRequired();
//         
//         // builder.Property(x => x.BetAmount)
//         //        .HasColumnName("bet_amount")
//         //        .HasConversion(m => m.Amount, a => new Money(a))
//         //        .IsRequired();
//         builder.Property(x => x.StartTime).IsRequired();
//         builder.Property(x => x.CompletedAt);
//         builder.Property(x=> x.Bets)
//             .HasConversion(
//                     v => System.Text.Json.JsonSerializer.Serialize(v, Mamey.JsonExtensions.SerializerOptions),
//                     v => System.Text.Json.JsonSerializer.Deserialize<PlinkoBet[]>(v, Mamey.JsonExtensions.SerializerOptions)
//                         ?? Array.Empty<PlinkoBet>());
//         // store slot outcomes as JSON
//         // builder.Property(x => x.SlotResults)
//         //        .HasColumnName("slot_results")
//         //        .HasConversion(
//         //            v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //            v => System.Text.Json.JsonSerializer.Deserialize<PlinkoSlot[]>(v, null)
//         //                ?? Array.Empty<PlinkoSlot>())
//         //        .HasColumnType("nvarchar(max)");
//
//         // builder
//         //     .HasMany(x => x.Bets)
//         //     
//         //     .WithOne()
//         //     .HasForeignKey(b => b.GameId)
//         //     
//         //     .OnDelete(DeleteBehavior.Cascade);
//     }
// }
//
// /// <summary>
// /// EF configuration for <see cref="MultiplayerBlackjackRoom"/>.
// /// </summary>
// public class MultiplayerBlackjackRoomConfiguration : IEntityTypeConfiguration<MultiplayerBlackjackRoom>
// {
//     public void Configure(EntityTypeBuilder<MultiplayerBlackjackRoom> builder)
//     {
//         builder.ToTable("multiplayer_blackjack_rooms");
//         builder.HasKey(r => r.Id);
//
//         builder.Property(r => r.Id);
//
//         // builder.Property(r => r.RoomName)
//         //     .HasColumnName("room_name")
//         //     .HasMaxLength(100)
//         //     .IsRequired();
//
//         // builder.Property(r => r.MaxPlayers)
//         //     .HasColumnName("max_players")
//         //     .IsRequired();
//
//         // builder.Property(r => r.MinimumBet)
//         //     .HasColumnName("minimum_bet")
//         //     .HasConversion(m => m.Amount, a => new Money(a))
//         //     .IsRequired();
//
//         // builder.Property(r => r.StartedAt)
//         //     .HasColumnName("started_at");
//
//         // builder.Property(r => r.CompletedAt)
//         //     .HasColumnName("completed_at");
//
//         // Players collection stored as JSON array of GUIDs
//         // builder.Property("_playerIds")
//         //     .HasColumnName("player_ids")
//         //     .HasConversion(
//         //         v => System.Text.Json.JsonSerializer.Serialize(v, null),
//         //         v => System.Text.Json.JsonSerializer.Deserialize<Guid[]>(v, null) ?? Array.Empty<Guid>())
//         //     .HasColumnType("nvarchar(max)");
//
//         // Relationship: one room has many rounds
//         // builder
//         //     .HasMany(r => r.Rounds)
//         //     .WithOne()
//         //     .HasForeignKey(round => round.RoomId)
//         //     .OnDelete(DeleteBehavior.Cascade);
//     }
// }