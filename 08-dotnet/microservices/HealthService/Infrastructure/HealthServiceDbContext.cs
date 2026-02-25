using Microsoft.EntityFrameworkCore;
using Ierahkwa.HealthService.Domain;

namespace Ierahkwa.HealthService.Infrastructure;

public class HealthServiceDbContext : DbContext
{
    public HealthServiceDbContext(DbContextOptions<HealthServiceDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<LabResult> LabResults => Set<LabResult>();
    public DbSet<MentalHealthRecord> MentalHealthRecords => Set<MentalHealthRecord>();
}
