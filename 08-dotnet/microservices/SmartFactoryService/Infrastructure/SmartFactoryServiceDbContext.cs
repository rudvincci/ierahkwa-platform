using Microsoft.EntityFrameworkCore;
using Ierahkwa.SmartFactoryService.Domain;

namespace Ierahkwa.SmartFactoryService.Infrastructure;

public class SmartFactoryServiceDbContext : DbContext
{
    public SmartFactoryServiceDbContext(DbContextOptions<SmartFactoryServiceDbContext> options) : base(options) { }

    public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<QualityCheck> QualityChecks => Set<QualityCheck>();
    public DbSet<FactoryInventory> FactoryInventorys => Set<FactoryInventory>();}
