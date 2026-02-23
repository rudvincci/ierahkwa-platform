using Common.Application.Interfaces;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Librarian.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Persistence;

public class LibrarianDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly ITenantService? _tenantService;

    public LibrarianDbContext(DbContextOptions<LibrarianDbContext> options) : base(options)
    {
    }

    public LibrarianDbContext(
        DbContextOptions<LibrarianDbContext> options,
        ICurrentUserService currentUserService,
        ITenantService tenantService) : base(options)
    {
        _currentUserService = currentUserService;
        _tenantService = tenantService;
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCategory> BookCategories => Set<BookCategory>();
    public DbSet<LibraryMember> LibraryMembers => Set<LibraryMember>();
    public DbSet<BorrowTransaction> BorrowTransactions => Set<BorrowTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Books");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.ISBN).HasMaxLength(50);
            entity.Property(e => e.Author).HasMaxLength(200);
            entity.Property(e => e.Publisher).HasMaxLength(200);
            entity.Property(e => e.Edition).HasMaxLength(50);
            entity.Property(e => e.Subject).HasMaxLength(200);
            entity.Property(e => e.RackNumber).HasMaxLength(50);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.TenantId, e.ISBN }).IsUnique();
        });

        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.ToTable("BookCategories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LibraryMember>(entity =>
        {
            entity.ToTable("LibraryMembers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MemberNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.HasIndex(e => new { e.TenantId, e.MemberNumber }).IsUnique();
        });

        modelBuilder.Entity<BorrowTransaction>(entity =>
        {
            entity.ToTable("BorrowTransactions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TransactionNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FineAmount).HasPrecision(18, 2);
            entity.HasOne(e => e.Member)
                .WithMany(m => m.BorrowTransactions)
                .HasForeignKey(e => e.MemberId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Book)
                .WithMany(b => b.BorrowTransactions)
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TenantId, e.TransactionNumber }).IsUnique();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = _tenantService?.GetCurrentTenantId();
        
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService?.UserName;
                    if (entry.Entity is TenantEntity tenantEntity && tenantId.HasValue)
                        tenantEntity.TenantId = tenantId.Value;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedBy = _currentUserService?.UserName;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
