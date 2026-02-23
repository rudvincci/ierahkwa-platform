using Common.Application.Interfaces;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.Domain.Entities;

namespace OnlineSchool.Persistence;

public class OnlineSchoolDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;
    private readonly ITenantService? _tenantService;

    public OnlineSchoolDbContext(DbContextOptions<OnlineSchoolDbContext> options) : base(options)
    {
    }

    public OnlineSchoolDbContext(
        DbContextOptions<OnlineSchoolDbContext> options,
        ICurrentUserService currentUserService,
        ITenantService tenantService) : base(options)
    {
        _currentUserService = currentUserService;
        _tenantService = tenantService;
    }

    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<ClassRoom> ClassRooms => Set<ClassRoom>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<TeacherMaterial> TeacherMaterials => Set<TeacherMaterial>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Parent> Parents => Set<Parent>();
    public DbSet<StudentParent> StudentParents => Set<StudentParent>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<ScheduleSettings> ScheduleSettings => Set<ScheduleSettings>();
    public DbSet<Homework> Homeworks => Set<Homework>();
    public DbSet<HomeworkContent> HomeworkContents => Set<HomeworkContent>();
    public DbSet<HomeworkQuestion> HomeworkQuestions => Set<HomeworkQuestion>();
    public DbSet<HomeworkAnswer> HomeworkAnswers => Set<HomeworkAnswer>();
    public DbSet<QuestionAnswer> QuestionAnswers => Set<QuestionAnswer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply global query filter for multi-tenancy
        modelBuilder.Entity<Grade>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<ClassRoom>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Material>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Teacher>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Student>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Parent>().HasQueryFilter(e => !e.IsDeleted);

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.ToTable("Grades");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
        });

        modelBuilder.Entity<ClassRoom>(entity =>
        {
            entity.ToTable("ClassRooms");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.HasOne(e => e.Grade)
                .WithMany(g => g.ClassRooms)
                .HasForeignKey(e => e.GradeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.TenantId, e.GradeId, e.Name }).IsUnique();
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.ToTable("Materials");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.HasIndex(e => new { e.TenantId, e.Name }).IsUnique();
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.ToTable("Teachers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.EmployeeId).HasMaxLength(50);
            entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
        });

        modelBuilder.Entity<TeacherMaterial>(entity =>
        {
            entity.ToTable("TeacherMaterials");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.TeacherMaterials)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Material)
                .WithMany(m => m.TeacherMaterials)
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.TeacherId, e.MaterialId }).IsUnique();
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.StudentId).HasMaxLength(50);
            entity.HasOne(e => e.ClassRoom)
                .WithMany(c => c.Students)
                .HasForeignKey(e => e.ClassRoomId)
                .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
        });

        modelBuilder.Entity<Parent>(entity =>
        {
            entity.ToTable("Parents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
        });

        modelBuilder.Entity<StudentParent>(entity =>
        {
            entity.ToTable("StudentParents");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Student)
                .WithMany(s => s.StudentParents)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Parent)
                .WithMany(p => p.StudentParents)
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.StudentId, e.ParentId }).IsUnique();
        });

        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.ToTable("Schedules");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.ClassRoom)
                .WithMany(c => c.Schedules)
                .HasForeignKey(e => e.ClassRoomId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Schedules)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Material)
                .WithMany(m => m.Schedules)
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ScheduleSettings>(entity =>
        {
            entity.ToTable("ScheduleSettings");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Homework>(entity =>
        {
            entity.ToTable("Homeworks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.Teacher)
                .WithMany(t => t.Homeworks)
                .HasForeignKey(e => e.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Material)
                .WithMany(m => m.Homeworks)
                .HasForeignKey(e => e.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.ClassRoom)
                .WithMany()
                .HasForeignKey(e => e.ClassRoomId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HomeworkContent>(entity =>
        {
            entity.ToTable("HomeworkContents");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
            entity.HasOne(e => e.Homework)
                .WithMany(h => h.Contents)
                .HasForeignKey(e => e.HomeworkId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HomeworkQuestion>(entity =>
        {
            entity.ToTable("HomeworkQuestions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Question).IsRequired();
            entity.HasOne(e => e.Homework)
                .WithMany(h => h.Questions)
                .HasForeignKey(e => e.HomeworkId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HomeworkAnswer>(entity =>
        {
            entity.ToTable("HomeworkAnswers");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Homework)
                .WithMany(h => h.Answers)
                .HasForeignKey(e => e.HomeworkId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Student)
                .WithMany(s => s.HomeworkAnswers)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => new { e.HomeworkId, e.StudentId }).IsUnique();
        });

        modelBuilder.Entity<QuestionAnswer>(entity =>
        {
            entity.ToTable("QuestionAnswers");
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.HomeworkAnswer)
                .WithMany(a => a.QuestionAnswers)
                .HasForeignKey(e => e.HomeworkAnswerId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Question)
                .WithMany()
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
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
