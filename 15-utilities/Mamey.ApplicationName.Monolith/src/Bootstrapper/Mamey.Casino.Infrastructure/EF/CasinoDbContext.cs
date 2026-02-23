using Mamey.Auth.Identity.Abstractions;
using Mamey.Auth.Identity.Abstractions.Entities;
using Mamey.Casino.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;


namespace Mamey.Casino.Infrastructure.EF;

/// <summary>
/// EF Core <see cref="DbContext"/> for the StakeGames domain.
/// Manages entity sets and applies all mappings.
/// </summary>
public class CasinoDbContext :  IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
    ApplicationUserClaim,
    ApplicationUserRole,      // ← here
    ApplicationUserLogin,
    ApplicationRoleClaim,
    ApplicationUserToken
>
{

    /// <summary>
    /// Creates a new <see cref="StakeGamesDbContext"/>.
    /// </summary>
    /// <param name="options">EF Core options (provider, connection string, etc.).</param>
    public CasinoDbContext(
        DbContextOptions<CasinoDbContext> options)
        : base(options)
    {

    }

    #region DbSets
    

    // public DbSet<CrashRound> CrashRounds { get; set; }
    // public DbSet<CrashBet> CrashBets { get; set; }
    //
    // public DbSet<EuropeanRouletteRound> EuropeanRouletteRounds { get; set; }
    // public DbSet<RouletteBet> RouletteBets { get; set; }
    //
    // public DbSet<MultiplayerRouletteRound> MultiplayerRouletteRounds { get; set; }
    //
    // public DbSet<Dice3DRoll> Dice3DRolls { get; set; }
    // public DbSet<Dice3DBet> Dice3DBets { get; set; }
    // public DbSet<DiceRoll> DiceRolls { get; set; }
    //
    // public DbSet<KenoGame> KenoGames { get; set; }
    // public DbSet<KenoBet> KenoBets { get; set; }
    //
    // public DbSet<Slots3DRound> Slots3DRounds { get; set; }
    // public DbSet<SlotsGame> SlotsGames { get; set; }
    //
    // public DbSet<CaribbeanPokerGame> CaribbeanPokerGames { get; set; }
    // public DbSet<CasinoHoldemGame> CasinoHoldemGames { get; set; }
    // public DbSet<BlackjackGame> BlackjackGames { get; set; }
    // public DbSet<MultiplayerBlackjackRoom> MultiplayerBlackjackRooms { get; set; }
    //
    // public DbSet<AmericanRouletteRound> AmericanRouletteRounds { get; set; }
    // public DbSet<HeadsOrTailsGame> HeadsOrTailsGames { get; set; }
    //
    // public DbSet<SicBoGame> SicBoGames { get; set; }
    // public DbSet<SicBoBet> SicBoBets { get; set; }
    //
    // public DbSet<VideoPokerGame> VideoPokerGames { get; set; }
    // public DbSet<Raffle> Raffles { get; set; }
    // public DbSet<RaffleTicket> RaffleTickets { get; set; }
    //
    // public DbSet<CryptoPredictionGame> CryptoPredictionGames { get; set; }
    // public DbSet<CryptoPredictionBet> CryptoPredictionBets { get; set; }
    //
    // public DbSet<HorseRacingRace> HorseRacingRaces { get; set; }
    // public DbSet<HorseBet> HorseBets { get; set; }
    //
    // public DbSet<LuckyWheelSpin> LuckyWheelSpins { get; set; }
    // public DbSet<LuckyWheelBet> LuckyWheelBets { get; set; }
    //
    // public DbSet<PlinkoGame> PlinkoGames { get; set; }
// public DbSet<CrashRound> CrashRounds { get; set; }
    // public DbSet<CrashBet> CrashBets { get; set; }
    //
    // public DbSet<EuropeanRouletteRound> EuropeanRouletteRounds { get; set; }
    // public DbSet<RouletteBet> RouletteBets { get; set; }
    //
    // public DbSet<MultiplayerRouletteRound> MultiplayerRouletteRounds { get; set; }
    //
    // public DbSet<Dice3DRoll> Dice3DRolls { get; set; }
    // public DbSet<Dice3DBet> Dice3DBets { get; set; }
    // public DbSet<DiceRoll> DiceRolls { get; set; }
    //
    // public DbSet<KenoGame> KenoGames { get; set; }
    // public DbSet<KenoBet> KenoBets { get; set; }
    //
    // public DbSet<Slots3DRound> Slots3DRounds { get; set; }
    // public DbSet<SlotsGame> SlotsGames { get; set; }
    //
    // public DbSet<CaribbeanPokerGame> CaribbeanPokerGames { get; set; }
    // public DbSet<CasinoHoldemGame> CasinoHoldemGames { get; set; }
    // public DbSet<BlackjackGame> BlackjackGames { get; set; }
    // public DbSet<MultiplayerBlackjackRoom> MultiplayerBlackjackRooms { get; set; }
    //
    // public DbSet<AmericanRouletteRound> AmericanRouletteRounds { get; set; }
    // public DbSet<HeadsOrTailsGame> HeadsOrTailsGames { get; set; }
    //
    // public DbSet<SicBoGame> SicBoGames { get; set; }
    // public DbSet<SicBoBet> SicBoBets { get; set; }
    //
    // public DbSet<VideoPokerGame> VideoPokerGames { get; set; }
    // public DbSet<Raffle> Raffles { get; set; }
    // public DbSet<RaffleTicket> RaffleTickets { get; set; }
    //
    // public DbSet<CryptoPredictionGame> CryptoPredictionGames { get; set; }
    // public DbSet<CryptoPredictionBet> CryptoPredictionBets { get; set; }
    //
    // public DbSet<HorseRacingRace> HorseRacingRaces { get; set; }
    // public DbSet<HorseBet> HorseBets { get; set; }
    //
    // public DbSet<LuckyWheelSpin> LuckyWheelSpins { get; set; }
    // public DbSet<LuckyWheelBet> LuckyWheelBets { get; set; }
    //
    // public DbSet<PlinkoGame> PlinkoGames { get; set; }
    public new DbSet<ApplicationUser> Users { get; set; }
    public new DbSet<ApplicationRole> Roles { get; set; }
    public new DbSet<ApplicationUserRole> UserRoles { get; set; }
    public new DbSet<ApplicationUserClaim> UserClaims { get; set; }
    public new DbSet<ApplicationRoleClaim> RoleClaims { get; set; }
    public new DbSet<ApplicationUserLogin> UserLogins { get; set; }
    public new DbSet<ApplicationUserToken> UserTokens { get; set; }
    #endregion

    /// <summary>
    /// Applies all entity configurations and conventions.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        // global conventions
        // modelBuilder.UseSnakeCaseNamingConvention();
        // modelBuilder.ApplyUtcDateTimeConverter();
        // modelBuilder.Entity<ApplicationUser>().ToTable("Users", "identity");
        // modelBuilder.Entity<ApplicationRole>().ToTable("Roles", "identity");
        // modelBuilder.Entity<ApplicationUserRole>().ToTable("UserRoles", "identity");
        // modelBuilder.Entity<ApplicationUserClaim>().ToTable("UserClaims", "identity");
        // modelBuilder.Entity<ApplicationRoleClaim>().ToTable("RoleClaims", "identity");
        // modelBuilder.Entity<ApplicationUserLogin>().ToTable("UserLogins", "identity");
        // modelBuilder.Entity<ApplicationUserToken>().ToTable("UserTokens", "identity");
        // apply each IEntityTypeConfiguration<T>
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        // base.OnModelCreating(modelBuilder);
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            Console.WriteLine($"Entity: {entity.Name}, Table: {entity.GetTableName()}, Schema: {entity.GetSchema()}");
        }
    }
    public Task<int> SaveChangesAsync()
    {
        return base.SaveChangesAsync();
    }
    /// <summary>
    /// If provider is not configured, configures SQL Server with retry and logging.
    /// </summary>
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (!optionsBuilder.IsConfigured)
    //     {
    //         // fallback if DI didn't configure
    //         // optionsBuilder
    //         //     .EnableDetailedErrors()
    //         //     .EnableSensitiveDataLogging()
    //         //     .UseSqlServer(
    //         //         // placeholder, replace with real connection string
    //         //         "Server=.;Database=StakeGames;Trusted_Connection=True;",
    //         //         sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    //         //     );
    //         _logger.LogInformation("StakeGamesDbContext configured with SQL Server fallback.");
    //     }
    // }
}

/// <summary>
/// Conventions extension: snake_case for all tables and columns.
/// </summary>
internal static class ModelBuilderExtensions
{
    public static void UseSnakeCaseNamingConvention(this ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()!.ToSnakeCase());
            foreach (var prop in entity.GetProperties())
                prop.SetColumnName(prop.GetColumnName().ToSnakeCase());
        }
    }

    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        var dateTimeConverter = new ValueConverter<DateTimeOffset, DateTime>(
            v => v.UtcDateTime,
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
        );

        foreach (var prop in builder.Model.GetEntityTypes().SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(DateTimeOffset) || p.ClrType == typeof(DateTimeOffset?)))
        {
            prop.SetValueConverter(dateTimeConverter);
        }
    }

    private static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c) && i > 0)
                sb.Append('_').Append(char.ToLowerInvariant(c));
            else
                sb.Append(char.ToLowerInvariant(c));
        }
        return sb.ToString();
    }
    
}