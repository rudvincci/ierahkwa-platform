using SmartAccounting.Domain.Entities;

namespace SmartAccounting.Application.DTOs;

public class JournalDto
{
    public int Id { get; set; }
    public string JournalNumber { get; set; } = string.Empty;
    public DateTime JournalDate { get; set; }
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public JournalStatus Status { get; set; }
    public int? CostCenterId { get; set; }
    public string? CostCenterName { get; set; }
    public bool IsPosted { get; set; }
    public DateTime? PostedAt { get; set; }
    public IEnumerable<JournalEntryDto> Entries { get; set; } = new List<JournalEntryDto>();
}

public class JournalEntryDto
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public string? AccountCode { get; set; }
    public string? AccountName { get; set; }
    public string? Description { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public int? CostCenterId { get; set; }
    public string? CostCenterName { get; set; }
}

public class CreateJournalDto
{
    public DateTime JournalDate { get; set; } = DateTime.UtcNow;
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public int? CostCenterId { get; set; }
    public IEnumerable<CreateJournalEntryDto> Entries { get; set; } = new List<CreateJournalEntryDto>();
}

public class CreateJournalEntryDto
{
    public int AccountId { get; set; }
    public string? Description { get; set; }
    public decimal DebitAmount { get; set; } = 0;
    public decimal CreditAmount { get; set; } = 0;
    public int? CostCenterId { get; set; }
}

public class AccountDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public AccountType Type { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public int Level { get; set; }
    public bool IsActive { get; set; }
    public bool IsSystemAccount { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal CurrentBalance { get; set; }
    public IEnumerable<AccountDto> Children { get; set; } = new List<AccountDto>();
}

public class CreateAccountDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public AccountType Type { get; set; }
    public int? ParentId { get; set; }
    public decimal OpeningBalance { get; set; } = 0;
}

public class CostCenterDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ParentName { get; set; }
    public bool IsActive { get; set; }
    public IEnumerable<CostCenterDto> Children { get; set; } = new List<CostCenterDto>();
}

public class CreateCostCenterDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
}
