using NET10.Core.Architecture;
using NET10.Core.ERP;

namespace NET10.Infrastructure.Services.WebERP;

// ═══════════════════════════════════════════════════════════════════════════════
// DATA ACCESS LAYER (DAL) - LAYER 3
// Three-Tier Architecture Implementation
// Repository Pattern + Unit of Work
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Generic In-Memory Repository (Replace with SQL/Oracle in production)
/// </summary>
public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly List<T> _entities = [];
    
    public virtual Task<T?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_entities.FirstOrDefault(e => e.Id == id && !e.IsDeleted));
    }
    
    public virtual Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult(_entities.Where(e => !e.IsDeleted).AsEnumerable());
    }
    
    public virtual Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
    {
        return Task.FromResult(_entities.Where(e => !e.IsDeleted && predicate(e)));
    }
    
    public virtual Task<T> AddAsync(T entity)
    {
        entity.Id = Guid.NewGuid();
        entity.CreatedAt = DateTime.UtcNow;
        _entities.Add(entity);
        return Task.FromResult(entity);
    }
    
    public virtual Task<T> UpdateAsync(T entity)
    {
        var index = _entities.FindIndex(e => e.Id == entity.Id);
        if (index >= 0)
        {
            entity.UpdatedAt = DateTime.UtcNow;
            _entities[index] = entity;
        }
        return Task.FromResult(entity);
    }
    
    public virtual Task<bool> DeleteAsync(Guid id)
    {
        var entity = _entities.FirstOrDefault(e => e.Id == id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
    
    public virtual Task<int> CountAsync()
    {
        return Task.FromResult(_entities.Count(e => !e.IsDeleted));
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// SPECIFIC REPOSITORIES
// ═══════════════════════════════════════════════════════════════════════════════

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> GetByCodeAsync(string code);
}

public class OrganizationRepository : InMemoryRepository<Organization>, IOrganizationRepository
{
    public Task<Organization?> GetByCodeAsync(string code)
    {
        return Task.FromResult(_entities.FirstOrDefault(o => 
            o.Code.Equals(code, StringComparison.OrdinalIgnoreCase) && !o.IsDeleted));
    }
}

public interface IERPUserRepository : IRepository<ERPUser>
{
    Task<ERPUser?> GetByUsernameAsync(string username);
    Task<ERPUser?> GetByEmailAsync(string email);
    Task<List<ERPUser>> GetByOrganizationAsync(Guid organizationId);
    Task<ERPUser?> GetByRefreshTokenAsync(string refreshToken);
}

public class ERPUserRepository : InMemoryRepository<ERPUser>, IERPUserRepository
{
    public Task<ERPUser?> GetByUsernameAsync(string username)
    {
        return Task.FromResult(_entities.FirstOrDefault(u => 
            u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && !u.IsDeleted));
    }
    
    public Task<ERPUser?> GetByEmailAsync(string email)
    {
        return Task.FromResult(_entities.FirstOrDefault(u => 
            u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && !u.IsDeleted));
    }
    
    public Task<List<ERPUser>> GetByOrganizationAsync(Guid organizationId)
    {
        return Task.FromResult(_entities
            .Where(u => u.OrganizationId == organizationId && !u.IsDeleted)
            .ToList());
    }
    
    public Task<ERPUser?> GetByRefreshTokenAsync(string refreshToken)
    {
        return Task.FromResult(_entities.FirstOrDefault(u => 
            u.RefreshToken == refreshToken && !u.IsDeleted));
    }
}

public interface ISalesOrderRepository : IRepository<SalesOrder>
{
    Task<SalesOrder?> GetByNumberAsync(string orderNumber);
    Task<List<SalesOrder>> GetByCustomerAsync(Guid customerId);
    Task<List<SalesOrder>> GetByOrganizationAsync(Guid organizationId);
    Task<List<SalesOrder>> GetByDateRangeAsync(Guid organizationId, DateTime from, DateTime to);
}

public class SalesOrderRepository : InMemoryRepository<SalesOrder>, ISalesOrderRepository
{
    public Task<SalesOrder?> GetByNumberAsync(string orderNumber)
    {
        return Task.FromResult(_entities.FirstOrDefault(o => 
            o.OrderNumber.Equals(orderNumber, StringComparison.OrdinalIgnoreCase) && !o.IsDeleted));
    }
    
    public Task<List<SalesOrder>> GetByCustomerAsync(Guid customerId)
    {
        return Task.FromResult(_entities
            .Where(o => o.CustomerId == customerId && !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToList());
    }
    
    public Task<List<SalesOrder>> GetByOrganizationAsync(Guid organizationId)
    {
        return Task.FromResult(_entities
            .Where(o => o.OrganizationId == organizationId && !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToList());
    }
    
    public Task<List<SalesOrder>> GetByDateRangeAsync(Guid organizationId, DateTime from, DateTime to)
    {
        return Task.FromResult(_entities
            .Where(o => o.OrganizationId == organizationId && 
                       o.OrderDate >= from && o.OrderDate <= to && 
                       !o.IsDeleted)
            .OrderByDescending(o => o.OrderDate)
            .ToList());
    }
}

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<List<Employee>> GetByOrganizationAsync(Guid organizationId);
    Task<List<Employee>> GetByDepartmentAsync(Guid departmentId);
    Task<List<Employee>> GetDirectReportsAsync(Guid managerId);
    Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber);
}

public class EmployeeRepository : InMemoryRepository<Employee>, IEmployeeRepository
{
    public Task<List<Employee>> GetByOrganizationAsync(Guid organizationId)
    {
        return Task.FromResult(_entities
            .Where(e => e.OrganizationId == organizationId && !e.IsDeleted)
            .ToList());
    }
    
    public Task<List<Employee>> GetByDepartmentAsync(Guid departmentId)
    {
        return Task.FromResult(_entities
            .Where(e => e.DepartmentId == departmentId && !e.IsDeleted)
            .ToList());
    }
    
    public Task<List<Employee>> GetDirectReportsAsync(Guid managerId)
    {
        return Task.FromResult(_entities
            .Where(e => e.ManagerId == managerId && !e.IsDeleted)
            .ToList());
    }
    
    public Task<Employee?> GetByEmployeeNumberAsync(string employeeNumber)
    {
        return Task.FromResult(_entities.FirstOrDefault(e => 
            e.EmployeeNumber.Equals(employeeNumber, StringComparison.OrdinalIgnoreCase) && !e.IsDeleted));
    }
}

public interface IProjectRepository : IRepository<Project>
{
    Task<List<Project>> GetByOrganizationAsync(Guid organizationId);
    Task<List<Project>> GetByManagerAsync(Guid managerId);
    Task<Project?> GetByCodeAsync(Guid organizationId, string code);
}

public class ProjectRepository : InMemoryRepository<Project>, IProjectRepository
{
    public Task<List<Project>> GetByOrganizationAsync(Guid organizationId)
    {
        return Task.FromResult(_entities
            .Where(p => p.OrganizationId == organizationId && !p.IsDeleted)
            .ToList());
    }
    
    public Task<List<Project>> GetByManagerAsync(Guid managerId)
    {
        return Task.FromResult(_entities
            .Where(p => p.ProjectManagerId == managerId && !p.IsDeleted)
            .ToList());
    }
    
    public Task<Project?> GetByCodeAsync(Guid organizationId, string code)
    {
        return Task.FromResult(_entities.FirstOrDefault(p => 
            p.OrganizationId == organizationId && 
            p.ProjectCode.Equals(code, StringComparison.OrdinalIgnoreCase) && 
            !p.IsDeleted));
    }
}

public interface ILeadRepository : IRepository<Lead>
{
    Task<List<Lead>> GetByOrganizationAsync(Guid organizationId);
    Task<List<Lead>> GetByAssigneeAsync(Guid userId);
    Task<List<Lead>> GetByStatusAsync(Guid organizationId, LeadStatus status);
}

public class LeadRepository : InMemoryRepository<Lead>, ILeadRepository
{
    public Task<List<Lead>> GetByOrganizationAsync(Guid organizationId)
    {
        return Task.FromResult(_entities
            .Where(l => l.OrganizationId == organizationId && !l.IsDeleted)
            .ToList());
    }
    
    public Task<List<Lead>> GetByAssigneeAsync(Guid userId)
    {
        return Task.FromResult(_entities
            .Where(l => l.AssignedToId == userId && !l.IsDeleted)
            .ToList());
    }
    
    public Task<List<Lead>> GetByStatusAsync(Guid organizationId, LeadStatus status)
    {
        return Task.FromResult(_entities
            .Where(l => l.OrganizationId == organizationId && l.Status == status && !l.IsDeleted)
            .ToList());
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// UNIT OF WORK
// ═══════════════════════════════════════════════════════════════════════════════

public class WebERPUnitOfWork : IUnitOfWork
{
    public IOrganizationRepository Organizations { get; }
    public IERPUserRepository Users { get; }
    public ISalesOrderRepository SalesOrders { get; }
    public IEmployeeRepository Employees { get; }
    public IProjectRepository Projects { get; }
    public ILeadRepository Leads { get; }
    
    public WebERPUnitOfWork()
    {
        Organizations = new OrganizationRepository();
        Users = new ERPUserRepository();
        SalesOrders = new SalesOrderRepository();
        Employees = new EmployeeRepository();
        Projects = new ProjectRepository();
        Leads = new LeadRepository();
    }
    
    public Task<int> SaveChangesAsync()
    {
        // In-memory doesn't need save, but in SQL this would commit
        return Task.FromResult(1);
    }
    
    public Task BeginTransactionAsync()
    {
        // Start transaction
        return Task.CompletedTask;
    }
    
    public Task CommitAsync()
    {
        // Commit transaction
        return Task.CompletedTask;
    }
    
    public Task RollbackAsync()
    {
        // Rollback transaction
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        // Cleanup resources
    }
}
