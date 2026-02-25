using Microsoft.EntityFrameworkCore;
using Ierahkwa.EmploymentService.Domain;

namespace Ierahkwa.EmploymentService.Infrastructure;

public class EmploymentServiceDbContext : DbContext
{
    public EmploymentServiceDbContext(DbContextOptions<EmploymentServiceDbContext> options) : base(options) { }

    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<Applicant> Applicants => Set<Applicant>();
    public DbSet<Resume> Resumes => Set<Resume>();
    public DbSet<Employer> Employers => Set<Employer>();
    public DbSet<Interview> Interviews => Set<Interview>();}
