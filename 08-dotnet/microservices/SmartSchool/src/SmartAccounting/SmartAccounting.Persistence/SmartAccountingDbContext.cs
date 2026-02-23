using Common.Application.Interfaces;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using SmartAccounting.Domain.Entities;

namespace SmartAccounting.Persistence;

public class SmartAccountingDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly ITenantService? _tenantService;

    public SmartAccountingDbContext(DbContextOptions<SmartAccountingDbContext> options) : base(options)
    {
    }

    public SmartAccountingDbContext(
        DbContextOptions<SmartAccountingDbContext> options,
        ICurrentUserService currentUserService,
        ITenantService tenantService) : base(options)
    {
        _currentUserService = currentUserService;
        _tenantService = tenantService;
    }

    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<SchoolYear> SchoolYears => Set<SchoolYear>();
    public DbSet<Fees> Fees => Set<Fees>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
    public DbSet<InvoicePayment> InvoicePayments => Set<InvoicePayment>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Journal> Journals => Set<Journal>();
    public DbSet<JournalEntry> JournalEntries => Set<JournalEntry>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Unit>(entity =>
        {
            entity.ToTable("Units");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ShortName).HasMaxLength(20);
            entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Barcode).HasMaxLength(50);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.SalePrice).HasPrecision(18, 2);
            entity.Property(e => e.Quantity).HasPrecision(18, 3);
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Unit)
                .WithMany(u => u.Products)
                .HasForeignKey(e => e.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.ToTable("Suppliers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Phone).HasMaxLength(50);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
        });

        modelBuilder.Entity<SchoolYear>(entity =>
        {
            entity.ToTable("SchoolYears");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Fees>(entity =>
        {
            entity.ToTable("Fees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne(e => e.SchoolYear)
                .WithMany(y => y.Fees)
                .HasForeignKey(e => e.SchoolYearId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.SubTotal).HasPrecision(18, 2);
            entity.Property(e => e.TaxAmount).HasPrecision(18, 2);
            entity.Property(e => e.DiscountAmount).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.Property(e => e.PaidAmount).HasPrecision(18, 2);
            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.Invoices)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.OriginalInvoice)
                .WithMany()
                .HasForeignKey(e => e.OriginalInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TenantId, e.InvoiceNumber }).IsUnique();
        });

        modelBuilder.Entity<InvoiceItem>(entity =>
        {
            entity.ToTable("InvoiceItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Quantity).HasPrecision(18, 3);
            entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Product)
                .WithMany(p => p.InvoiceItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Fees)
                .WithMany()
                .HasForeignKey(e => e.FeesId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<InvoicePayment>(entity =>
        {
            entity.ToTable("InvoicePayments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Reference).HasMaxLength(100);
            entity.HasOne(e => e.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(e => e.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NameAr).HasMaxLength(200);
            entity.Property(e => e.OpeningBalance).HasPrecision(18, 2);
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
            entity.HasOne(e => e.Parent)
                .WithMany(a => a.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
        });

        modelBuilder.Entity<Journal>(entity =>
        {
            entity.ToTable("Journals");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JournalNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Reference).HasMaxLength(100);
            entity.Property(e => e.TotalDebit).HasPrecision(18, 2);
            entity.Property(e => e.TotalCredit).HasPrecision(18, 2);
            entity.HasOne(e => e.CostCenter)
                .WithMany(c => c.Journals)
                .HasForeignKey(e => e.CostCenterId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TenantId, e.JournalNumber }).IsUnique();
        });

        modelBuilder.Entity<JournalEntry>(entity =>
        {
            entity.ToTable("JournalEntries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DebitAmount).HasPrecision(18, 2);
            entity.Property(e => e.CreditAmount).HasPrecision(18, 2);
            entity.HasOne(e => e.Journal)
                .WithMany(j => j.Entries)
                .HasForeignKey(e => e.JournalId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Account)
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.CostCenter)
                .WithMany(c => c.JournalEntries)
                .HasForeignKey(e => e.CostCenterId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CostCenter>(entity =>
        {
            entity.ToTable("CostCenters");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Parent)
                .WithMany(c => c.Children)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TenantId, e.Code }).IsUnique();
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
