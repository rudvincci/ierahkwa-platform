using System.Text.Json;
using System.Text.Json.Serialization;
using BCrypt.Net;

namespace AdvocateOffice.Services;

public class DbService
{
    private readonly string _dataPath;
    private Database _db = null!;
    private readonly object _lock = new();
    private readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    public DbService(IConfiguration config)
    {
        var rel = config["AdvocateOffice:DataFile"] ?? "Data/database.json";
        _dataPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), rel));
        var dir = Path.GetDirectoryName(_dataPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        if (!File.Exists(_dataPath))
            File.WriteAllText(_dataPath, "{}");
        Load();
        SeedIfEmpty();
    }

    private void Load()
    {
        lock (_lock)
        {
            var json = File.ReadAllText(_dataPath);
            _db = JsonSerializer.Deserialize<Database>(json, _jsonOpts) ?? new Database();
            _db ??= new Database();
            _db.EnsureMeta();
        }
    }

    private void Save()
    {
        lock (_lock)
        {
            var json = JsonSerializer.Serialize(_db, _jsonOpts);
            File.WriteAllText(_dataPath, json);
        }
    }

    private void SeedIfEmpty()
    {
        lock (_lock)
        {
            if (_db.Users.Count > 0) return;
            // admin / advocate
            _db.Users.Add(new User
            {
                Id = NextId("users"),
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Environment.GetEnvironmentVariable("DEFAULT_ADMIN_PASSWORD") ?? "changeme-dev", 10),
                FullName = "Administrator",
                Role = "admin",
                Active = true,
                CreatedAt = DateTime.UtcNow
            });
            _db.CaseCategories.AddRange(
                new CaseCategory { Id = NextId("caseCategories"), Name = "Civil", CreatedAt = DateTime.UtcNow },
                new CaseCategory { Id = NextId("caseCategories"), Name = "Criminal", CreatedAt = DateTime.UtcNow },
                new CaseCategory { Id = NextId("caseCategories"), Name = "Family", CreatedAt = DateTime.UtcNow }
            );
            _db.CaseStages.AddRange(
                new CaseStage { Id = NextId("caseStages"), Name = "Filed", CreatedAt = DateTime.UtcNow },
                new CaseStage { Id = NextId("caseStages"), Name = "In Progress", CreatedAt = DateTime.UtcNow },
                new CaseStage { Id = NextId("caseStages"), Name = "Closed", CreatedAt = DateTime.UtcNow }
            );
            Save();
        }
    }

    public int NextId(string collection)
    {
        if (_db.Meta?.NextId == null) _db.EnsureMeta();
        var v = _db.Meta!.NextId.GetValueOrDefault(collection, 1);
        _db.Meta.NextId[collection] = v + 1;
        return v;
    }

    public User? FindUserByUsername(string username) =>
        _db.Users.FirstOrDefault(u => u.Username == username && u.Active);

    public User? GetUserById(int id) =>
        _db.Users.FirstOrDefault(u => u.Id == id);

    public bool VerifyPassword(string hash, string password) =>
        BCrypt.Net.BCrypt.Verify(password, hash);

    public List<Client> Clients => _db.Clients;
    public List<Court> Courts => _db.Courts;
    public List<CaseCategory> CaseCategories => _db.CaseCategories;
    public List<CaseStage> CaseStages => _db.CaseStages;
    public List<Case> Cases => _db.Cases;
    public List<Document> Documents => _db.Documents;
    public List<Invoice> Invoices => _db.Invoices;
    public List<InvoiceItem> InvoiceItems => _db.InvoiceItems;
    public List<CaseTask> Tasks => _db.Tasks;
    public List<Employee> Employees => _db.Employees;
    public List<BankAccount> BankAccounts => _db.BankAccounts;
    public List<Vendor> Vendors => _db.Vendors;
    public List<Product> Products => _db.Products;
    public List<Contact> Contacts => _db.Contacts;
    public List<Testimonial> Testimonials => _db.Testimonials;
    public List<Sponsor> Sponsors => _db.Sponsors;
    public List<Update> Updates => _db.Updates;
    public List<Appointment> Appointments => _db.Appointments;

    public void SaveChanges() => Save();
}

public class Database
{
    public List<User> Users { get; set; } = new();
    public List<Client> Clients { get; set; } = new();
    public List<Court> Courts { get; set; } = new();
    public List<CaseCategory> CaseCategories { get; set; } = new();
    public List<CaseStage> CaseStages { get; set; } = new();
    public List<Case> Cases { get; set; } = new();
    public List<Document> Documents { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();
    public List<InvoiceItem> InvoiceItems { get; set; } = new();
    public List<CaseTask> Tasks { get; set; } = new();
    public List<Employee> Employees { get; set; } = new();
    public List<BankAccount> BankAccounts { get; set; } = new();
    public List<Vendor> Vendors { get; set; } = new();
    public List<Product> Products { get; set; } = new();
    public List<Contact> Contacts { get; set; } = new();
    public List<Testimonial> Testimonials { get; set; } = new();
    public List<Sponsor> Sponsors { get; set; } = new();
    public List<Update> Updates { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
    public DatabaseMeta? Meta { get; set; }

    public void EnsureMeta()
    {
        Meta ??= new DatabaseMeta { NextId = new Dictionary<string, int>() };
        foreach (var c in new[] { "users", "clients", "courts", "caseCategories", "caseStages", "cases", "documents", "invoices", "invoiceItems", "tasks", "employees", "bankAccounts", "vendors", "products", "contacts", "testimonials", "sponsors", "updates", "appointments" })
            if (!Meta.NextId.ContainsKey(c)) Meta.NextId[c] = 1;
    }
}

public class DatabaseMeta
{
    public Dictionary<string, int> NextId { get; set; } = new();
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Role { get; set; } = "advocate";
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Court
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CaseCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class CaseStage
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class Case
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? CaseNumber { get; set; }
    public int ClientId { get; set; }
    public int? CourtId { get; set; }
    public int? CaseCategoryId { get; set; }
    public int? CaseStageId { get; set; }
    public string? Description { get; set; }
    public DateTime? FilingDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Document
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string Name { get; set; } = "";
    public string? FilePath { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Invoice
{
    public int Id { get; set; }
    public string? InvoiceNumber { get; set; }
    public int ClientId { get; set; }
    public int? CaseId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = "draft";
    public DateTime CreatedAt { get; set; }
}

public class InvoiceItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string Description { get; set; } = "";
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
}

public class CaseTask
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public int? CaseId { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Designation { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BankAccount
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public decimal Balance { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Vendor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Type { get; set; } // product | service
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Message { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Testimonial
{
    public int Id { get; set; }
    public string? ClientName { get; set; }
    public string? Content { get; set; }
    public int? Rating { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Sponsor
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? LogoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Update
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OppositeLawyer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Firm { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LobbyList
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public DateTime PutUpDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Appointment
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int? ClientId { get; set; }
    public int? CaseId { get; set; }
    public DateTime StartAt { get; set; }
    public DateTime? EndAt { get; set; }
    public string? Location { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "scheduled";
    public DateTime CreatedAt { get; set; }
}

public class ContactCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class ClientCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class CourtCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class LeaveType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int DaysPerYear { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LeaveDefine
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public int LeaveTypeId { get; set; }
    public int Days { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; }
}

public class Holiday
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime Date { get; set; }
    public int Year { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
